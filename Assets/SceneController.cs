using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void MoveScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void MoveMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
