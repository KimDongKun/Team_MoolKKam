using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private void Start()
    {
        SetWindow(true);
    }
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void MoveMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
    public void MoveGameScene(GameObject obj)
    {
        obj.SetActive(true);
        SceneManager.LoadScene("GameScene");
    }
    public void SetWindow(bool isFullScreen)
    {
        Screen.SetResolution(1920, 1080, isFullScreen);
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
