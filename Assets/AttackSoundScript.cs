using UnityEngine;
using UnityEngine.Audio;

public class AttackSoundScript : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;                 // �÷��̾�/���� ������Ʈ�� ����
    public AudioMixerGroup outputMixer;             // ���� (������/ȿ���� ���� ����)

    [Header("Clips")]
    public AudioClip parry;
    public AudioClip guardAttack;
    public AudioClip upperSkill;
    public AudioClip dashSkill;
    public AudioClip chargeLv1;
    public AudioClip chargeLv2;
    public AudioClip chargeLv3;

    public AudioClip combo1Swing;
    public AudioClip combo2Swing;
    public AudioClip combo3Swing;

    public AudioClip jumpAttackSwing1;               // �������� ����
    public AudioClip jumpAttackSwing2;               // �������� ����
    public AudioClip jumpAttackSwing3;               // �������� ����
    public AudioClip[] randomWhooshes;             // ���� �� �� ���� ���(����)

    [Header("Options")]
    [Range(0.0f, 0.5f)] public float pitchJitter = 0.05f; // �̼��� ��ġ ����
    [Range(0.0f, 1.0f)] public float minInterval = 0.03f; // ������ �ߺ� ��� ����

    private float lastPlayTime;

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (outputMixer) audioSource.outputAudioMixerGroup = outputMixer;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D ȿ����(���ϸ� 3D�� 1f)
    }

    // ===== �ִϸ��̼� �̺�Ʈ���� ȣ���� �Լ��� =====

    // ���ڿ� Ű�� ���: �̺�Ʈ�� string �Ķ���ͷ� �־� ȣ��
    public void PlaySfx(string key)
    {
        AudioClip clip = ResolveClip(key);
        Play(clip);
    }

    // �ε��� ���(��: �޺� �ܰ� ����): �̺�Ʈ�� int �Ķ���ͷ� �־� ȣ��
    public void PlayComboSwing(int comboIndex)
    {
        AudioClip clip = comboIndex switch
        {
            1 => combo1Swing,
            2 => combo2Swing,
            3 => combo3Swing,
            _ => combo1Swing
        };
        Play(clip);
    }

    // ���� �Ľ�(����)
    public void PlayRandomWhoosh()
    {
        if (randomWhooshes != null && randomWhooshes.Length > 0)
        {
            var clip = randomWhooshes[Random.Range(0, randomWhooshes.Length)];
            Play(clip);
        }
    }

    // ===== ���� ��ƿ =====
    private void Play(AudioClip clip)
    {
        Debug.Log($"Playing sound: {clip?.name}");
        if (!clip || !audioSource) return;

        // �ߺ� ���� ����
        if (Time.time - lastPlayTime < minInterval) return;
        lastPlayTime = Time.time;
           
        // ��ġ ��¦ ����ȭ�� ������
        float basePitch = 1f;
        audioSource.pitch = basePitch + Random.Range(-pitchJitter, pitchJitter);

        audioSource.PlayOneShot(clip);
    }

    private AudioClip ResolveClip(string key)
    {
        switch (key)
        {
            case "Parry": return parry;
            case "GuardAttack": return guardAttack;
            case "UpperSkill": return upperSkill;
            case "ChargeLv1": return chargeLv1;
            case "ChargeLv2": return chargeLv2;
            case "ChargeLv3": return chargeLv3;
            case "Combo1": return combo1Swing;
            case "Combo2": return combo2Swing;
            case "Combo3": return combo3Swing;
            case "JumpAttack1": return jumpAttackSwing1;
            case "JumpAttack2": return jumpAttackSwing2;
            case "JumpAttack3": return jumpAttackSwing3;
            case "DashSkill": return dashSkill;
            default: return null;
        }
    }
}
