using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Transform player;
    public AudioPlayPoint[] audioPlayPoints;

    private void Start()
    {
        audioPlayPoints.ToList().ForEach(x => x.audioSource.Play());
        audioPlayPoints.ToList().ForEach(x => x.audioSource.volume = 0);
    }
    private void Update()
    {
        if(audioPlayPoints.Length>0)
        AudioPlayTrigger();
    }
    private void AudioPlayTrigger()
    {
        float playerX = player.position.x;
        var playPoints = audioPlayPoints.Where(p => p.audioPositon.position.x <= playerX).ToList();
        for(int i = 0; i < playPoints.Count; i++)
        {
            float dis = playerX - playPoints[i].audioPositon.position.x;
            playPoints[i].audioSource.volume = Mathf.Max(dis, 1);//�Ÿ��� ���� ��������, �ִ� : 1
        }
    }

}
[System.Serializable]
public class AudioPlayPoint
{
    public AudioSource audioSource; //����� List
    public Transform audioPositon; //��ǥ �� ����� ���, ����� List�� ũ��(Length) ����
}