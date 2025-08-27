using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private void Start()
    {
        Screen.SetResolution(1920, 1080, true);
    }
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void MoveMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}
