using UnityEngine;
using UnityEngine.SceneManagement;

public class Commands : MonoBehaviour
{
    public void loadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
