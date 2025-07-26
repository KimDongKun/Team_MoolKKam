using UnityEngine;

public class WalkStateReset : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Idle 애니메이션에 진입했을 때 실행됨
        PlayerController pc = animator.GetComponent<PlayerController>();
        PlayerModel model = pc.playerModel;
        if (model != null)
        {
            model.IsAttacking = false;
            model.IsRolling = false;
        }
    }
}
