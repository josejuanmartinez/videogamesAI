using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    public Canvas difficultyCanvas;
    public LoadingCanvasManager loadingCanvasManager;
    public AudioManager audioManager;
    public AudioRepo audioRepo;

    private float lastClick = 0f;

    void Awake()
    {
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        audioRepo = GameObject.Find("AudioRepo").GetComponent<AudioRepo>();
        loadingCanvasManager.Hide();
        // audioManager.PlaySound(audioRepo.GetAudio("cards"));
    }

    public void Easy()
    {
        GameObject.Find("Settings").GetComponent<Settings>().SetDifficulty(DifficultiesEnum.Easy);
        StartGame();
    }
    public void Medium()
    {
        GameObject.Find("Settings").GetComponent<Settings>().SetDifficulty(DifficultiesEnum.Medium);
        StartGame();
    }
    public void Hard()
    {
        GameObject.Find("Settings").GetComponent<Settings>().SetDifficulty(DifficultiesEnum.Hard);
        StartGame();
    }

    public void StartGame()
    {
        bool doubleClick = Time.time - lastClick < 0.5f;
        lastClick = Time.time;

        if (doubleClick)
        {
            loadingCanvasManager.Show();
            difficultyCanvas.enabled = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
