using TMPro;
using UnityEngine;

public class GuideMessage : MonoBehaviour
{
    public GameObject messageUI;
    public TMP_Text messageText;

    public static GuideMessage Instance;
    private void Awake()
    {
        Instance = this;
        messageUI.SetActive(false);
    }
    public void OnMessage(string message)
    {
        if (messageUI.activeSelf) messageUI.SetActive(false);

        messageText.text = message;
        messageUI.SetActive(true);
    }
    public void DisableMessage()
    {
        if (messageUI.activeSelf) messageUI.SetActive(false);
    }
}
