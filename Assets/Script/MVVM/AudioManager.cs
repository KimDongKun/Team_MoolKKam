using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource; //πË∞Ê¿Ω

    public float volume = 0.5f;
    public float pitch = 0.8f;

    public void PlaySoundEffect(AudioClip clip)
    {
        if (audioSource == null)
            return;

        audioSource.volume = volume;
        audioSource.pitch = pitch;

        audioSource.clip = clip;

        audioSource.Stop();
        audioSource.Play();
    }
}