using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void LoadGame(int sceneIndex)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneIndex);
    }

    public void QuitGame()
    {
        Debug.Log("I Quit The Game");
        Application.Quit();
    }
}
