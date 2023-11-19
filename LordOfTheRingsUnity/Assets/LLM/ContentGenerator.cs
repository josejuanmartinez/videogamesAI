using Cysharp.Threading.Tasks;
using LLama;
using LLama.Common;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum LLM
{
    Mistral7BQ3,
    Mistral7BQ5
}

public class ContentGenerator : MonoBehaviour
{
    public static string MISTRAL7BQ3 = "models/mistral-7b-instruct-v0.1.Q3_K_M.gguf";
    public static string MISTRAL7BQ5 = "models/mistral-7b-instruct-v0.1.Q3_K_M.gguf";

    [Header("Hyperparams")]
    public LLM llm;
    [TextArea(5,10)]
    public string PROMPT = "You are J.R.R. Tolkien. A User will provide a concept and you will create a 1 sentence quote mentioning that concept, using the lore from Middle-Earth or making up something in the style of the Lord of the Rings.\r\nUser: ";
    public uint ContextSize = 512;
    public float Temperature = 1f;
    public int MaxTokens = 200;
    public uint Seed = 49;
    [Header("Saving")]
    public string prefix = "quote_";
    public string OutputJsonPath = "Locales/en-US.ai.json";
    [Header("Interactive mode (Optional)")]
    public TMP_Text OutputText;
    public TMP_InputField InputText;
    public Button SubmitButton;

    private string submittedText = "";
    private string cardName = "";
    private List<string> toIgnore;
    private Board board;
    private float ignoreTime = -1;
    
    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
        toIgnore = new();
    }

    async UniTaskVoid Start()
    {
        if(SubmitButton != null)
        {
            SubmitButton.interactable = false;
            SubmitButton.onClick.AddListener(() =>
            {
                OutputText.text = "";
                submittedText = PROMPT + InputText.text;
                Debug.Log(submittedText);
            });
        }

        // Load a model
        string ModelPath = MISTRAL7BQ3;
        switch(llm)
        {
            case LLM.Mistral7BQ3:
                ModelPath = MISTRAL7BQ3;
                break;
            case LLM.Mistral7BQ5:
                ModelPath = MISTRAL7BQ5;
                break;
        }

        var parameters = new ModelParams(Application.streamingAssetsPath + "/" + ModelPath)
        {
            ContextSize = ContextSize,
            Seed = Seed,
            GpuLayerCount = 35
        };

        // Switch to the thread pool for long-running operations
        await UniTask.SwitchToThreadPool();
        using var model = LLamaWeights.LoadFromFile(parameters);
        await UniTask.SwitchToMainThread();

        // Initialize a chat session
        using var context = model.CreateContext(parameters);
        var ex = new InteractiveExecutor(context);
        ChatSession session = new (ex);

        if(SubmitButton != null)
            SubmitButton.interactable = true;

        string response = "";
        // Run the inference in a loop to chat with LLM
        while (true)
        {
            await UniTask.WaitUntil(() => submittedText != "");
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
                response += token;
            }

            if (OutputText != null)
                OutputText.text = response;

            Store(cardName, response);

            submittedText = "";
            response = "";
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
        {
            yield return token;
        }
    }

    public void AddIgnore(string cardName, bool sleep=false)
    {
        toIgnore.Add(cardName);
        if(sleep)
            StartCoroutine(WakeUp());
    }

    private IEnumerator WakeUp()
    {
        yield return new WaitForSeconds(60 * 5);
        ignoreTime = -1;
    }

    public bool IsSleeping()
    {
        return ignoreTime != -1;
    }

    public bool Ignored(string cardName)
    {
        return toIgnore.Contains(cardName);
    }

    public bool Generating()
    {
        return submittedText != "";
    }

    public bool StillNotGenerated(string cardName)
    {
        string path = Application.streamingAssetsPath + "/" + OutputJsonPath;
        if (!File.Exists(path))
            return true;
        string existingJson = File.ReadAllText(path);
        Dictionary<string, string> existingData = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson);
        return !existingData.ContainsKey(prefix+cardName);
    }
    private void Store(string cardName, string response)
    {
        if(response.Length < 15)
        {
            Debug.Log(string.Format("DISCARDED: {0}:{1}", cardName, response));
            return;
        }

        #if UNITY_EDITOR
        if (!UnityEditor.EditorUtility.DisplayDialog(string.Format("Are you ok with this description for {0}?", cardName), response, "OK", "Discard"))
            return;
        #endif

        string path = Application.streamingAssetsPath + "/" + OutputJsonPath;
        if (!File.Exists(path))
            File.WriteAllText(path, "{}");
        
        string existingJson = File.ReadAllText(path);
        Dictionary<string, string> existingData = JsonSerializer.Deserialize<Dictionary<string, string>>(existingJson);
        if(!existingData.ContainsKey(prefix + cardName))
            existingData.Add(prefix + cardName, response);

        string updatedJson = JsonSerializer.Serialize(existingData, new JsonSerializerOptions { WriteIndented = true });

        // Write the updated JSON back to the file
        File.WriteAllText(path, updatedJson);

        Debug.Log(string.Format("SUCCESS: {0}:{1}", cardName, response));
    }

    public bool Generate(
        string cardName,
        string cardDesc,
        uint ContextSize = 0, 
        float Temperature = 0,
        int MaxTokens = 0,
        uint Seed = 0)
    {
        if (!board.IsAllLoaded())
            return false;
        if (Generating())
            return false;
        submittedText = PROMPT + cardDesc;

        this.cardName = cardName;
        if(ContextSize != 0)
            this.ContextSize = ContextSize;
        if (Temperature != 0)
            this.Temperature = Temperature;
        if (MaxTokens != 0)
            this.MaxTokens = MaxTokens;
        if (Seed != 0)
            this.Seed = Seed;

        Debug.Log(string.Format("Submitted: {0}:{1}", cardName, cardDesc));

        return true;
    }
}