using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UISoundController : MonoBehaviour
{
    public AudioSource UISound;
    public AudioUIData[] AudioUIArray;

    public static UISoundController Instance;
    private void Awake()
    {
        Instance = this;
    }
    public void Start()
    {
        Setting();
    }
    private void Setting()
    {

        UISound.resource = null;
        UISound.loop = false;
        UISound.volume = 0.8f;
        UISound.playOnAwake = false;
    }
    public void UIClickSound()
    {
        PlaySound(FindSound("Click"));
    }
    public void PlayUISound(string soundName)
    {
        PlaySound(FindSound(soundName));
    }
    private void PlaySound(AudioClip selectSound)
    {
        if(UISound.isPlaying) UISound.Stop();

        UISound.resource = selectSound;
        UISound.Play();
    }
    private AudioClip FindSound(string soundName)
    {
        var sound = AudioUIArray.Where(x => x.name == soundName);
        return sound.First().audioClip;
    }
}
[System.Serializable]
public class AudioUIData
{
    public string name;
    public AudioClip audioClip;
}
