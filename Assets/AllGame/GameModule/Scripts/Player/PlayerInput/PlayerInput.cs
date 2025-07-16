using System.Collections;
using UnityEngine;
using UnityEngine.U2D.IK;

public class PlayerInput : MonoBehaviour
{
    public float _moveInput { get; private set; }
    public bool _isMoving { get; private set; }
    public bool _isRunning { get; private set; }
    public bool _isDash { get; private set; }
    public bool _isSiting { get; private set; }
    public bool _isJumping { get; private set; }
    public bool _isJump { get; private set; }
    public bool _isAttack { get; private set; }


    private void Update()
    {
        if (PlayerManager.Instance._isAlive && !PlayerManager.Instance._knocked)
        {
            handleMove();
            handleSitAndDash();
            handleJump();
            handleAttack();
        }
    }

    // move
    private float _lastDirection = 0;
    private bool _isrun = false;
    private Coroutine _resetIsRun;
    private bool _recoverStamina = false;
    private bool _startResetIsRun = true;
    private void handleMove()
    {
        _moveInput = Input.GetAxisRaw("Horizontal");
        _isMoving = _moveInput != 0;
        if (PlayerManager.Instance._stamina < 0)
        {
            PlayerManager.Instance._stamina = 0;
            _isRunning = false;
        }
        if (_isMoving && _isrun && _lastDirection != _moveInput && !_isSiting)
            {
                if (_resetIsRun != null)
                {
                    StopCoroutine(_resetIsRun);
                    _resetIsRun = null;
                }
                if (PlayerManager.Instance._stamina > 0)
                {
                    _isRunning = true;
                }
                _recoverStamina = false;
                _lastDirection = _moveInput;
                _startResetIsRun = true;
            }
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSiting && _isMoving && !_isRunning && PlayerManager.Instance._stamina > 0)
        {
            _lastDirection = _moveInput;
            _isRunning = !_isRunning;
            _isrun = _isRunning;
        }
        if (!_isMoving && _startResetIsRun)
        {
            _isRunning = _isMoving;
            if (_resetIsRun != null)
            {
                StopCoroutine(_resetIsRun);
                _resetIsRun = null;
            }
            _startResetIsRun = false;
            _resetIsRun = StartCoroutine(resetisrun());
        }
        recoverStamina();
    }
    private IEnumerator resetisrun()
    {
        yield return new WaitForSeconds(0.2f);
        _isrun = false;
        _startResetIsRun = true;
        _recoverStamina = true;
    }
    private void recoverStamina()
    {
        if (!_isrun && _recoverStamina && PlayerManager.Instance._stamina < PlayerManager.Instance.Stats._stamina)
        {
            PlayerManager.Instance._stamina += (Time.deltaTime * 20);
        }
    }





    // dash and sit
    private Coroutine _resetDash;
    private void handleSitAndDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (_isJumping || _isRunning)
            {
                _isDash = true;
                if (_resetDash != null)
                    StopCoroutine(_resetDash);
                _resetDash = StartCoroutine(resetDash());
            }
            else _isSiting = !_isSiting;
        }
    }
    public IEnumerator resetDash()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _isDash = false;
    }




    // jump
    private void handleJump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (_isSiting) _isSiting = false;
            else { _isJump = true; }
        }
    }
    public void resetJump() => _isJump = false;
    public void setJumping() => _isJumping = true;
    public void resetJumping() => _isJumping = false;



    //attack basic
    public bool _canAttack = true;
    private float _canAttackCooldown = 0.3f;
    private void handleAttack()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            _isAttack = true;
            StartCoroutine(resetAttack());
        }
    }
    public IEnumerator resetAttack()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _isAttack = false;
    }
    public void resetCanAttact()
    {
        StartCoroutine(_canAttackk());
    }
    public void setCanAttack() => _canAttack = false;
    private IEnumerator _canAttackk()
    {
        yield return new WaitForSeconds(_canAttackCooldown);
        _canAttack = true;
    }
}
