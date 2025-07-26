using UnityEngine;

public class WalkStateReset : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Idle �ִϸ��̼ǿ� �������� �� �����
        PlayerController pc = animator.GetComponent<PlayerController>();
        PlayerModel model = pc.playerModel;
        if (model != null)
        {
            model.IsAttacking = false;
            model.IsRolling = false;
        }
    }
}
