using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{

    public void Exit()
    {
        Application.Quit();
    }

    public void PlayLordOfTheRings()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
