using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class TutorialManager : MonoBehaviour
{
    public GameManager GameManager;
    private TutorialStep currentStep = TutorialStep.None;

    public ItemData tutorial_potion;
    public ItemData tutorial_log;
    public ItemData tutorial_moonStone;
    public ItemData tutorial_enchantStone_1;
    public ItemData tutorial_enchantStone_2;
    public ItemData tutorial_enchantStone_3;

    public static bool isUseSkill = false;
    public static bool isUserGetPotion = false;
    public static bool isUserGetLog = false;
    public static bool isUserGetBuild = false;
    public static bool isUserGetStone = false;
    public static bool isUserEnchant = false;

    public static bool isTutorial = true;

    public Transform target_Potion;
    public Transform target_Log;
    public Transform target_Build;
    public Transform target_GetStone;
    public Transform target_Enchant;
    public Transform target_End;

    public GameObject introUI;
    public Transform player;
    public Image arrowImage;

    private Transform target;
    private bool isArrow = false;
    private enum TutorialStep
    {
        None,
        Start,
        Move,
        Jump,
        Roll,
        Attack,
        Guard,
        Skill,
        GetPotion,
        GetLog,
        Build,
        GetStone,
        Enchant,
        End_1,
        End_2,
        End_3,
        End
    }

    void Start()
    {
        // 튜토리얼 시작
        target = null;
        SetStep(TutorialStep.Start);
    }

    void Update()
    {
        GuideArrow();

        switch (currentStep)
        {
            case TutorialStep.Start:
                if (Input.GetKeyDown(KeyCode.P))
                {
                    introUI.SetActive(false);
                    SetStep(TutorialStep.End);
                }
                else if (Input.GetKeyDown(KeyCode.Return)) // Enter
                {
                    introUI.SetActive(false);
                    SetStep(TutorialStep.Move);
                }
                break;

            case TutorialStep.Move:
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    SetStep(TutorialStep.Jump);
                }
                break;

            case TutorialStep.Jump:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SetStep(TutorialStep.Roll);
                }
                break;

            case TutorialStep.Roll:
                if (Input.GetKeyDown(KeyCode.LeftShift))
                {
                    SetStep(TutorialStep.Attack);
                }
                break;
            case TutorialStep.Attack:
                if (Input.GetMouseButtonDown(0))
                {
                    SetStep(TutorialStep.Guard);
                }
                break;
            case TutorialStep.Guard:
                if (Input.GetMouseButtonUp(1))
                {
                    SetStep(TutorialStep.Skill);
                }
                break;
            case TutorialStep.Skill:
                if (isUseSkill)
                {
                    SetStep(TutorialStep.GetPotion);
                }
                break;
            case TutorialStep.GetPotion:
                if (isUserGetPotion)
                {
                    SetStep(TutorialStep.GetLog);
                }
                break;
            case TutorialStep.GetLog:
                if (isUserGetLog)
                {
                    SetStep(TutorialStep.Build);
                }
                break;
            case TutorialStep.Build:
                if (isUserGetBuild)
                {
                    SetStep(TutorialStep.GetStone);
                }
                break;
            case TutorialStep.GetStone:
                if (isUserGetStone)
                {
                    SetStep(TutorialStep.Enchant);
                }
                break;
            case TutorialStep.Enchant:
                if (isUserEnchant)
                {
                    SetStep(TutorialStep.End_1);
                }
                break;
            case TutorialStep.End_1:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SetStep(TutorialStep.End_2);
                }
                break;
            case TutorialStep.End_2:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SetStep(TutorialStep.End_3);
                }
                break;
            case TutorialStep.End_3:
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SetStep(TutorialStep.End);
                }
                break;
        }
    }

    private void SetStep(TutorialStep step)
    {
        currentStep = step;

        switch (step)
        {
            case TutorialStep.Start:
                GuideMessage.Instance.OnMessage("튜토리얼을 스킵하려면 P, 진행하려면 Enter를 누르세요.");
                break;
            case TutorialStep.Move:
                GuideMessage.Instance.OnMessage("A, D 키를 눌러 이동할 수 있습니다.");
                break;
            case TutorialStep.Jump:
                GuideMessage.Instance.OnMessage("스페이스바를 눌러 점프할 수 있습니다.");
                break;
            case TutorialStep.Roll:
                GuideMessage.Instance.OnMessage("Shift 키를 눌러 구르기로 공격을 회피할 수 있습니다.");
                break;
            case TutorialStep.Attack:
                GuideMessage.Instance.OnMessage("마우스 좌클릭을 통해 기본공격을 할 수 있습니다.\n 꾹 누르면 콤보공격을 합니다.");
                break;
            case TutorialStep.Guard:
                GuideMessage.Instance.OnMessage("마우스 우클릭을 통해 가드를 할 수 있습니다.\n 적 공격타이밍에 맞추어 가드를 하면 Parring(패링)스킬이 발동합니다.");
                break;
            case TutorialStep.Skill:
                isUseSkill = false;
                SetAddItem(tutorial_potion, 1);
                player.GetComponent<PlayerController>().playerViewModel.HealingPotion(50);
                GuideMessage.Instance.OnMessage("대쉬어택(스킬) : 가드(마우스 우클릭)상태에서 기본공격(마우스 좌클릭)\n올려치기(스킬) : 가드(마우스 우클릭)상태에서 W + 기본공격(마우스 좌클릭)\n단, 스킬 사용시 MP(노란색 게이지-10 소모 합니다.)");
                break;
            case TutorialStep.GetPotion:
                isUserGetPotion = false;
                SetArrowTarget(target_Potion);
                GuideMessage.Instance.OnMessage("포션(말풍선)에 접근하여 E키를 꾹 누르면 포션을 얻을수있습니다.\n포션은 Q를 눌러 사용할수있습니다.");
                break;
            case TutorialStep.GetLog:
                isUserGetLog = false;
                SetArrowTarget(target_Log);
                GuideMessage.Instance.OnMessage("화살표를 따라 나무에 접근하여 E키를 꾹 눌러 벌목을 할수있습니다.");
                break;
            case TutorialStep.Build:
                isUserGetBuild = false;
                SetAddItem(tutorial_log, 10);
                SetArrowTarget(target_Build);
                GuideMessage.Instance.OnMessage("나무를 10개 제공해드렸습니다.\n우측 상단의 건설버튼을 눌러 아무 건축물을 지어주세요. (Esc키를 누르면 UI를 닫을수있습니다.)");
                break;
            case TutorialStep.GetStone:
                isUserGetStone = false;
                SetAddItem(tutorial_moonStone, 30);
                SetArrowTarget(target_GetStone);
                GuideMessage.Instance.OnMessage("달의 파편을 30개 제공해드렸습니다. 화살표를 따라 E를 눌러 상호작용 해주세요.\n달의 파편을 10개 지불하면 확률로 강화석을 지급 받을 수있습니다. (Esc키를 누르면 UI를 닫을수있습니다.)");
                break;
            case TutorialStep.Enchant:
                isUserEnchant = false;
                SetAddItem(tutorial_enchantStone_1, 100);
                SetAddItem(tutorial_enchantStone_2, 100);
                SetAddItem(tutorial_enchantStone_3, 100);
                SetArrowTarget(target_Enchant);
                GuideMessage.Instance.OnMessage("강화석을 각각 100개씩 제공해드렸습니다.\n 화살표를 따라 E를 눌러 상호작용 해주세요.\n강화석을 통해 무기를 강화하면 더욱 강한 피해를 입힐 수 있습니다. (Esc키를 누르면 UI를 닫을수있습니다.)");
                break;
            case TutorialStep.End_1:
                SetArrowTarget(target_End);
                GuideMessage.Instance.OnMessage("적을 잡아 달의 파편을 모으고, 달의 파편을 강화석으로 바꾼 다음 무기를 강화하시면 됩니다.\n달의 침략자들은 맵 중앙의 관리자를 노리고있습니다. 관리자를 지켜내세요.\n(Enter를 눌러 다음으로 넘기기)");
                break;
            case TutorialStep.End_2:
                target = null;
                GuideMessage.Instance.OnMessage("적 공격타이밍에 맞추어 가드를 하면 Parring(패링)스킬을 통해 다양한 스킬연계도 가능합니다.\n예시) Parring(패링)스킬 후 바로 기본공격(마우스 좌클릭)\n(Enter를 눌러 다음으로 넘기기)");
                break;
            case TutorialStep.End_3:
                GuideMessage.Instance.OnMessage("여기까지 튜토리얼이였습니다. 잘 따라와 주셔서 감사합니다.\n바로 게임을 시작하도록 하겠습니다.\n(Enter를 눌러 다음으로 넘기기)");
                break;
            case TutorialStep.End:
                target = null;
                isTutorial = false;
                GameManager.ResetPlayerSetting();
                GuideMessage.Instance.DisableMessage();
                break;
        }
    }

    private void SetAddItem(ItemData data, int Quantity)
    {
        player.GetComponent<PlayerController>().playerViewModel.AddItem(data, Quantity);
    }
    private void SetArrowTarget(Transform tr)
    {
        arrowImage.gameObject.SetActive(true);
        target = tr;
    }
    private void GuideArrow()
    {
        if (target == null || player == null)
        {
            arrowImage.gameObject.SetActive(false); 
            return;
        }

        Vector3 dir = target.position - player.position;
        dir.y = 0f; // y는 무시 (2D 관점)

        // 각도 구하기 (라디안 → 도)
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        // 화살표가 "위쪽"을 바라보므로, 90도 보정 필요
        angle -= 90f;

        // UI 회전 적용 (z축 회전)
        arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void SkipTutorial()
    {
        currentStep = TutorialStep.End;
        GuideMessage.Instance.OnMessage("튜토리얼을 스킵했습니다!");
        // 추가로 UI 숨기거나 본 게임 시작 로직 추가 가능
    }

}
