using UnityEngine;
using UnityEngine.Audio;

public class AttackSoundScript : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;                 // 플레이어/무기 오브젝트에 부착
    public AudioMixerGroup outputMixer;             // 선택 (마스터/효과음 버스 연결)

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

    public AudioClip jumpAttackSwing1;               // 점프공격 공통
    public AudioClip jumpAttackSwing2;               // 점프공격 공통
    public AudioClip jumpAttackSwing3;               // 점프공격 공통
    public AudioClip[] randomWhooshes;             // 여러 개 중 랜덤 재생(선택)

    [Header("Options")]
    [Range(0.0f, 0.5f)] public float pitchJitter = 0.05f; // 미세한 피치 랜덤
    [Range(0.0f, 1.0f)] public float minInterval = 0.03f; // 과도한 중복 재생 방지

    private float lastPlayTime;

    void Awake()
    {
        if (!audioSource) audioSource = GetComponent<AudioSource>();
        if (outputMixer) audioSource.outputAudioMixerGroup = outputMixer;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f; // 2D 효과음(원하면 3D로 1f)
    }

    // ===== 애니메이션 이벤트에서 호출할 함수들 =====

    // 문자열 키로 재생: 이벤트에 string 파라미터로 넣어 호출
    public void PlaySfx(string key)
    {
        AudioClip clip = ResolveClip(key);
        Play(clip);
    }

    // 인덱스 기반(예: 콤보 단계 숫자): 이벤트에 int 파라미터로 넣어 호출
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

    // 랜덤 후쉬(선택)
    public void PlayRandomWhoosh()
    {
        if (randomWhooshes != null && randomWhooshes.Length > 0)
        {
            var clip = randomWhooshes[Random.Range(0, randomWhooshes.Length)];
            Play(clip);
        }
    }

    // ===== 내부 유틸 =====
    private void Play(AudioClip clip)
    {
        Debug.Log($"Playing sound: {clip?.name}");
        if (!clip || !audioSource) return;

        // 중복 폭주 방지
        if (Time.time - lastPlayTime < minInterval) return;
        lastPlayTime = Time.time;
           
        // 피치 살짝 랜덤화로 생동감
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
