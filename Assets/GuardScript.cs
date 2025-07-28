using UnityEngine;

public class GuardScript : StateMachineBehaviour
{
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsTag("walk"))
        {
            Debug.Log("GuardScript: OnStateMove called, player is walking.");
        }
        // ���°� �̵��� �� ȣ��˴ϴ�.
    }

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsTag("guard_end"))
        {
        PlayerController playerController = animator.GetComponent<PlayerController>();
        animator.SetBool("Block", false);
        playerController.GarudExit();
        Debug.Log("GuardScript: OnStateMachineExit called, exiting guard state.");

        }
    }
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        PlayerController playerController = animator.GetComponent<PlayerController>();
        animator.SetBool("Block", false);
        playerController.GarudExit();
        Debug.Log("GuardScript: OnStateMachineExit called, exiting guard state.");
    }
}
