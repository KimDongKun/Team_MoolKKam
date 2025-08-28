using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingController : MonoBehaviour
{
    public AudioSource[] audioArray;

    public List<float> volumeMaxList = new List<float> ();
    private void Start()
    {
        audioArray.ToList().ForEach(audio => volumeMaxList.Add(audio.volume));
    }
    public void SFX_Setting(float volume)
    {
        for (int i = 0; i < audioArray.Length; i++)
        {
            audioArray[i].volume= volumeMaxList[i] * volume;
        }
    }
}
