using UnityEngine;

public class AttackMovement : StateMachineBehaviour
{
    public float _movePowerX = 3f;
    public float _movePowerY = 0f;
    public bool _moveX = true;
    public bool _moveY = false;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Lấy Rigidbody từ GameObject nhân vật
        Rigidbody2D rb = animator.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Xác định hướng mặt nhân vật
            float direction = animator.transform.localScale.x > 0 ? 1 : -1;
            if (_moveX)
                rb.linearVelocity = new Vector2(_movePowerX * direction, rb.linearVelocity.y);
            if (_moveY)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, _movePowerY);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
