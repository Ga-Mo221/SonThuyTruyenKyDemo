using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator _anim;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        if (_anim == null)
            Debug.LogError("_anim là null");
    }

    // set gorunded
    public void setIsGround(bool isGround)
    {
        _anim.SetBool(AnimationStrings._isGround, isGround);
    }

    // move
    public bool getBoolCanMove()
    {
        return _anim.GetBool(AnimationStrings._isCanMove);
    }
    public void setMove(bool isMoving, bool isRunning)
    {
        _anim.SetBool(AnimationStrings._isMoving, isMoving);
        _anim.SetBool(AnimationStrings._isRunning, isRunning);
    }

    // sit
    public void setSiting(bool isSiting)
    {
        _anim.SetBool(AnimationStrings._isSit, isSiting);
    }

    // dash
    public void setDashing()
    {
        _anim.SetTrigger(AnimationStrings._isDash);
    }

    // jump
    public void setJumping()
    {
        _anim.SetTrigger(AnimationStrings._isJumping);
    }
    public void airState(float yVelocity)
    {
        _anim.SetFloat(AnimationStrings._yVelocity, yVelocity);
    }

    // attack
    public void setAttack()
    {
        _anim.SetTrigger(AnimationStrings._isAttack);
    }

    public void setAttackSpeed()
    {
        Debug.Log("setAttackSpeed được gọi");

        if (PlayerManager.Instance == null)
            Debug.LogError("PlayerManager.Instance là null");

        if (PlayerManager.Instance.Stats == null)
            Debug.LogError("PlayerManager.Instance.Stats là null");

        if (_anim != null && PlayerManager.Instance != null && PlayerManager.Instance.Stats != null)
            _anim.SetFloat(AnimationStrings._attackSpeed, PlayerManager.Instance.Stats.getAttackSpeed());
    }
}
