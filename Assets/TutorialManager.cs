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
        // Ʃ�丮�� ����
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
                GuideMessage.Instance.OnMessage("Ʃ�丮���� ��ŵ�Ϸ��� P, �����Ϸ��� Enter�� ��������.");
                break;
            case TutorialStep.Move:
                GuideMessage.Instance.OnMessage("A, D Ű�� ���� �̵��� �� �ֽ��ϴ�.");
                break;
            case TutorialStep.Jump:
                GuideMessage.Instance.OnMessage("�����̽��ٸ� ���� ������ �� �ֽ��ϴ�.");
                break;
            case TutorialStep.Roll:
                GuideMessage.Instance.OnMessage("Shift Ű�� ���� ������� ������ ȸ���� �� �ֽ��ϴ�.");
                break;
            case TutorialStep.Attack:
                GuideMessage.Instance.OnMessage("���콺 ��Ŭ���� ���� �⺻������ �� �� �ֽ��ϴ�.\n �� ������ �޺������� �մϴ�.");
                break;
            case TutorialStep.Guard:
                GuideMessage.Instance.OnMessage("���콺 ��Ŭ���� ���� ���带 �� �� �ֽ��ϴ�.\n �� ����Ÿ�ֿ̹� ���߾� ���带 �ϸ� Parring(�и�)��ų�� �ߵ��մϴ�.");
                break;
            case TutorialStep.Skill:
                isUseSkill = false;
                SetAddItem(tutorial_potion, 1);
                player.GetComponent<PlayerController>().playerViewModel.HealingPotion(50);
                GuideMessage.Instance.OnMessage("�뽬����(��ų) : ����(���콺 ��Ŭ��)���¿��� �⺻����(���콺 ��Ŭ��)\n�÷�ġ��(��ų) : ����(���콺 ��Ŭ��)���¿��� W + �⺻����(���콺 ��Ŭ��)\n��, ��ų ���� MP(����� ������-10 �Ҹ� �մϴ�.)");
                break;
            case TutorialStep.GetPotion:
                isUserGetPotion = false;
                SetArrowTarget(target_Potion);
                GuideMessage.Instance.OnMessage("����(��ǳ��)�� �����Ͽ� EŰ�� �� ������ ������ �������ֽ��ϴ�.\n������ Q�� ���� ����Ҽ��ֽ��ϴ�.");
                break;
            case TutorialStep.GetLog:
                isUserGetLog = false;
                SetArrowTarget(target_Log);
                GuideMessage.Instance.OnMessage("ȭ��ǥ�� ���� ������ �����Ͽ� EŰ�� �� ���� ������ �Ҽ��ֽ��ϴ�.");
                break;
            case TutorialStep.Build:
                isUserGetBuild = false;
                SetAddItem(tutorial_log, 10);
                SetArrowTarget(target_Build);
                GuideMessage.Instance.OnMessage("������ 10�� �����ص�Ƚ��ϴ�.\n���� ����� �Ǽ���ư�� ���� �ƹ� ���๰�� �����ּ���. (EscŰ�� ������ UI�� �������ֽ��ϴ�.)");
                break;
            case TutorialStep.GetStone:
                isUserGetStone = false;
                SetAddItem(tutorial_moonStone, 30);
                SetArrowTarget(target_GetStone);
                GuideMessage.Instance.OnMessage("���� ������ 30�� �����ص�Ƚ��ϴ�. ȭ��ǥ�� ���� E�� ���� ��ȣ�ۿ� ���ּ���.\n���� ������ 10�� �����ϸ� Ȯ���� ��ȭ���� ���� ���� ���ֽ��ϴ�. (EscŰ�� ������ UI�� �������ֽ��ϴ�.)");
                break;
            case TutorialStep.Enchant:
                isUserEnchant = false;
                SetAddItem(tutorial_enchantStone_1, 100);
                SetAddItem(tutorial_enchantStone_2, 100);
                SetAddItem(tutorial_enchantStone_3, 100);
                SetArrowTarget(target_Enchant);
                GuideMessage.Instance.OnMessage("��ȭ���� ���� 100���� �����ص�Ƚ��ϴ�.\n ȭ��ǥ�� ���� E�� ���� ��ȣ�ۿ� ���ּ���.\n��ȭ���� ���� ���⸦ ��ȭ�ϸ� ���� ���� ���ظ� ���� �� �ֽ��ϴ�. (EscŰ�� ������ UI�� �������ֽ��ϴ�.)");
                break;
            case TutorialStep.End_1:
                SetArrowTarget(target_End);
                GuideMessage.Instance.OnMessage("���� ��� ���� ������ ������, ���� ������ ��ȭ������ �ٲ� ���� ���⸦ ��ȭ�Ͻø� �˴ϴ�.\n���� ħ���ڵ��� �� �߾��� �����ڸ� �븮���ֽ��ϴ�. �����ڸ� ���ѳ�����.\n(Enter�� ���� �������� �ѱ��)");
                break;
            case TutorialStep.End_2:
                target = null;
                GuideMessage.Instance.OnMessage("�� ����Ÿ�ֿ̹� ���߾� ���带 �ϸ� Parring(�и�)��ų�� ���� �پ��� ��ų���赵 �����մϴ�.\n����) Parring(�и�)��ų �� �ٷ� �⺻����(���콺 ��Ŭ��)\n(Enter�� ���� �������� �ѱ��)");
                break;
            case TutorialStep.End_3:
                GuideMessage.Instance.OnMessage("������� Ʃ�丮���̿����ϴ�. �� ����� �ּż� �����մϴ�.\n�ٷ� ������ �����ϵ��� �ϰڽ��ϴ�.\n(Enter�� ���� �������� �ѱ��)");
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
        dir.y = 0f; // y�� ���� (2D ����)

        // ���� ���ϱ� (���� �� ��)
        float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;

        // ȭ��ǥ�� "����"�� �ٶ󺸹Ƿ�, 90�� ���� �ʿ�
        angle -= 90f;

        // UI ȸ�� ���� (z�� ȸ��)
        arrowImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void SkipTutorial()
    {
        currentStep = TutorialStep.End;
        GuideMessage.Instance.OnMessage("Ʃ�丮���� ��ŵ�߽��ϴ�!");
        // �߰��� UI ����ų� �� ���� ���� ���� �߰� ����
    }

}
