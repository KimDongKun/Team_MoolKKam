using UnityEngine;

public class GuardScript : StateMachineBehaviour
{
    
   

    override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        PlayerController playerController = animator.GetComponent<PlayerController>();
        animator.SetBool("Block", false);
        playerController.GarudExit();

    }
}
