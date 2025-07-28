using UnityEngine;

public class AttackAnimStateScript : StateMachineBehaviour
{
    // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        string dbug = stateInfo.ToString();
       // Debug.Log($"AttackAnimStateScript: OnStateEnter - State Info: {dbug}");
        PlayerController playerController = animator.GetComponent<PlayerController>();
        playerController.playerModel.IsAttacking = true;
    }


    // OnStateExit is called before OnStateExit is called on any state inside this state machine
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    // OnStateMove is called before OnStateMove is called on any state inside this state machine
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateIK is called before OnStateIK is called on any state inside this state machine
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMachineEnter is called when entering a state machine via its Entry Node
    override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
    {

    }

    // OnStateMachineExit is called when exiting a state machine via its Exit Node
    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)

    {
        Debug.Log("AttackAnimStateScript: OnStateMachineExit called, exiting attack state.");
        PlayerController playerController = animator.GetComponent<PlayerController>();
        playerController.DisableDamage();
    }
}
