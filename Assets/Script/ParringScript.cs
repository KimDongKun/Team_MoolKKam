using UnityEngine;

public class ParringScript : StateMachineBehaviour
{
   
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsTag("ParryCounter"))
        {
            Debug.Log("����");
            animator.SetBool("Parring", false);
        }

    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.IsTag("Parry"))
        {
           
           // Debug.Log($"�и� ��� �ð�: {stateInfo.normalizedTime}");
            // �и� ��� �ð� (��: �ִϸ��̼� ���� �� 0.3����)
            if (stateInfo.normalizedTime < 0.8f && animator.GetBool("Block"))
            {
                animator.SetBool("Parring", true);
                animator.SetBool("Block", false);
            

                PlayerController pc = animator.GetComponent<PlayerController>();
                pc.EnableDamage("Parring");
            }
        }
    }


    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

     
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}

}
