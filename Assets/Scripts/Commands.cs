using UnityEngine;
using UnityEngine.SceneManagement;

public class Commands : MonoBehaviour
{
    public void loadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void quitGame()
    {
        Application.Quit();
    }
}
