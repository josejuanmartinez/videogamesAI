using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelector : MonoBehaviour
{
    public Canvas canvas;
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
        canvas.enabled = false;
        SceneManager.LoadScene(2);
    }
}
