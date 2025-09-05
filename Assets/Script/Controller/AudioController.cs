
using UnityEngine;

public class AudioController : MonoBehaviour
{
    public NightWaveManager nightWaveManager;

    public AudioClip[] audioClip;
    public AudioSource audioSource;

    private bool isNight = true;
    public bool isBoss = false;
    private AudioClip selectAudioClip;
    public float maxVolume = 1f;
    public void Start()
    {
        BGM_Setting();
    }

    private void Update()
    {

        if (nightWaveManager.boss.activeSelf)
        {
            audioSource.volume = Mathf.Clamp01(audioSource.volume + (Time.deltaTime * 0.1f));
            if (!isBoss)
            {
                BGM_BossPlay();
            }
        }
        else
        {
            if (isNight != nightWaveManager.IsNight)
            {
                BGM_VolumeLerp();
            }
            else
            {
                BGM_Setting();
            }
        }

    }
    public void BGM_Setting()
    {
        isNight = !nightWaveManager.IsNight;

        if (!isNight)
        {
            selectAudioClip = audioClip[1];
        }
        else
        {
            selectAudioClip = audioClip[0];
        }

        audioSource.resource = selectAudioClip;
        audioSource.Play();
        audioSource.volume = 0;
    }
    public void BGM_VolumeLerp()
    {
        audioSource.volume = Mathf.Clamp(audioSource.volume + (Time.deltaTime * 0.1f), 0, maxVolume);

    }
    public void BGM_Setting(float volume)
    {
        maxVolume = volume;
    }
    public void BGM_BossPlay()
    {
        isBoss = true;
        selectAudioClip = audioClip[2];
        audioSource.resource = selectAudioClip;
        audioSource.Play();
        audioSource.volume = 0;
    }
    public void BGM_BossEnding()
    {
        audioSource.resource = null;
        audioSource.enabled = false;
    }
}
