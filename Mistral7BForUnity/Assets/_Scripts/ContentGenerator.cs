using Cysharp.Threading.Tasks;
using LLama;
using LLama.Common;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum LLM
{
    Mistral7BQ3,
    Mistral7BQ5
}

public class ContentGenerator : MonoBehaviour
{
    public static string MISTRAL7BQ3 = "mistral-7b-instruct-v0.1.Q3_K_M.gguf";
    public static string MISTRAL7BQ5 = "mistral-7b-instruct-v0.1.Q5_K_S.gguf";

    [Header("Hyperparams")]
    [SerializeField]
    private LLM llm;
    [TextArea(5,10)]
    [SerializeField]
    private string PROMPT = "You are J.R.R. Tolkien. A User will provide a concept and you will create a 1 sentence quote mentioning that concept, using the lore from Middle-Earth or making up something in the style of the Lord of the Rings.\r\nUser: ";
    [SerializeField]
    private uint ContextSize = 512;
    [SerializeField]
    private float Temperature = 1f;
    [SerializeField]
    private int MaxTokens = 200;
    [SerializeField] 
    private uint Seed = 49;
    [SerializeField]
    private int GpuLayerCount = 35;

    [Header("Widget")]
    [SerializeField]
    private TMP_Text outputText;
    [SerializeField]
    private TMP_InputField InputText;
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private TMP_Text submitButtonText;

    private string modelPath;
    private string submittedText;
    private ModelParams hyperParams;

    void Awake()
    {
        submitButton.interactable = false;
        // Initializes the text as empty. Submit button will change this and trigger the LLM generation.
        submittedText = "";
        // Sets the path depending on the LLM selected in the dropdown
        ModelNameToPath();
        // Sets the hyperparams
        ProcessHyperparams();
        // Adds an "OnClick" listener on the flight to "Submit" button
        // that will change `submittedText` when clicked
        AddListenerToSubmitButton();
    }

    /// <summary>
    /// Start functions with full Thread Control and Async/awaits
    /// </summary>    
    async UniTaskVoid Start()
    {
        // Switch to the thread pool for long-running operations
        await UniTask.SwitchToThreadPool();
        // I load the Model
        using var model = LLamaWeights.LoadFromFile(hyperParams);
        // Switch to main thread
        await UniTask.SwitchToMainThread();

        // Initialize a AI-generation session
        using var context = model.CreateContext(hyperParams);
        var ex = new InteractiveExecutor(context);
        ChatSession session = new (ex);

        // After initialization, I enable the button
        submitButton.interactable = true;

        // Run the inference in a loop to chat with LLM
        while (true)
        {
            // Sleeping Until I get some text after clicking submit
            await UniTask.WaitUntil(() => submittedText != "");
            
            // While thinking, I disable the button
            submitButton.interactable = false;
            
            // Async background token generation
            await foreach (var token in ChatConcurrent(
                session.ChatAsync(
                    submittedText,
                    new InferenceParams()
                    {
                        Temperature = Temperature,
                        MaxTokens = MaxTokens,
                        AntiPrompts = new List<string> { "User:", "You:"}
                    }
                )
            ))
            {
                outputText.text += token;
            }

            // Upon finishing, we reset the submitted text so that the generation stops.
            submittedText = "";
            // And I enable the button again
            submitButton.interactable = true;
        }
    }
    /// <summary>
    /// Wraps AsyncEnumerable with transition to the thread pool. 
    /// </summary>
    /// <param name="tokens"></param>
    /// <returns>IAsyncEnumerable computed on a thread pool</returns>
    private async IAsyncEnumerable<string> ChatConcurrent(IAsyncEnumerable<string> tokens)
    {
        await UniTask.SwitchToThreadPool();
        await foreach (var token in tokens)
            yield return token;
    }

    /// <summary>
    /// Initializes the button to set `submittedText` variable, being observed by the LLM generation loop
    /// </summary>
    private void AddListenerToSubmitButton()
    {
        submitButton.onClick.AddListener(() =>
        {
            outputText.text = "";
            submittedText = string.Format("{0} {1}", PROMPT, InputText.text);
            Debug.Log(submittedText);
        });
    }

    /// <summary>
    /// Enum to Path to avoid setting manually a path in the Editor
    /// </summary>
    private void ModelNameToPath()
    {
        switch (llm)
        {
            case LLM.Mistral7BQ3:
                modelPath = MISTRAL7BQ3;
                break;
            case LLM.Mistral7BQ5:
                modelPath = MISTRAL7BQ5;
                break;
        }
    }

    /// <summary>
    /// Sets the hyperparam variable
    /// </summary>
    public void ProcessHyperparams()
    {
        hyperParams = new ModelParams(Application.streamingAssetsPath + "/" + modelPath)
        {
            ContextSize = ContextSize,
            Seed = Seed,
            GpuLayerCount = GpuLayerCount
        };
    }

    /// <summary>
    /// Update function with full Thread Control and Async/awaits. It shows if the model is ready to generate text or if it's processing.
    /// </summary>    
    async UniTaskVoid Update()
    {
        submitButtonText.text = !submitButton.interactable ? "Loading..." : "Generate";
        await UniTask.Yield();
    }
}