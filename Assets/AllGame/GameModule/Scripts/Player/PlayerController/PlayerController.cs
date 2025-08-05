using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D _rb;
    private PlayerInput _playerInput;
    private PlayerAnimationManager _animManger;

    // check gounded
    private bool _isGround;
    private bool _canCheckGrounded = true;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Vector2 _boxSize = new Vector2(0.5f, 0.1f);
    [SerializeField] private Transform _groundCheck;

    // move
    private bool _canMove => _animManger.getBoolCanMove();


    // Dash
    private bool _canDash = true;
    public bool _isDashing;
    private float _originalGravity;
    [SerializeField] private TrailRenderer _tr;

    // Jump
    private int _jumpCount = 0;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerInput = GetComponent<PlayerInput>();
        _animManger = GetComponent<PlayerAnimationManager>();
        _originalGravity = _rb.gravityScale;
    }

    void Update()
    {
        checkGrounded();
        //debug();
        if (_canMove)
        {
            handleJump();
            if (_isDashing) return;
            handleMove();
            handleSit();
            handleDash();
        }
        handleAttack();
    }

    void FixedUpdate()
    {
        _animManger.airState(_rb.linearVelocity.y);
    }

    // check gorunded
    private void checkGrounded()
    {
        if (_canCheckGrounded)
            _isGround = Physics2D.OverlapBox(_groundCheck.position, _boxSize, 0f, _groundLayer);
        else _isGround = false;

        _animManger.setIsGround(_isGround);
        if (_isGround)
            _jumpCount = 0;
        _playerInput.setJumping(_isGround);

        if (_isGround && !_playerInput._isMoving && !_isDashing && !_playerInput._isJumping && !PlayerManager.Instance._canMoveAttack)
        {
            _rb.linearVelocity = new Vector2(0, 0);
        }
    }
    private IEnumerator resetCanCheckGrounded()
    {
        yield return new WaitForSeconds(0.3f);
        _canCheckGrounded = true;
    }

    void OnDrawGizmos()
    {
        if (_groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_groundCheck.position, _boxSize);
    }


    // Move
    private void handleMove()
    {
        float _moveSpeed = PlayerManager.Instance.Stats.GetMoveSpeed(_playerInput._isRunning);
        if (_playerInput._isDash) return;
        if (_playerInput._isSiting) _moveSpeed -= 0.5f;
        if (_playerInput._isJumping) _moveSpeed += 1;
        if (_playerInput._isRunning && PlayerManager.Instance._stamina > 0)
        {
            PlayerManager.Instance._stamina -= (Time.deltaTime * 25);
        }
        if (_playerInput._isMoving)
            _rb.linearVelocity = new Vector2(_playerInput._moveInput * _moveSpeed, _rb.linearVelocity.y);
        _animManger.setMove(_playerInput._isMoving, _playerInput._isRunning);
        flip(_playerInput._moveInput);
    }

    // Flip (localScale)
    private void flip(float moveinput)
    {
        if (moveinput > 0.1) transform.localScale = new Vector3(1, 1, 1);
        else if (moveinput < -0.1) transform.localScale = new Vector3(-1, 1, 1);
    }

    // Sit
    private void handleSit()
    {
        _animManger.setSiting(_playerInput._isSiting);
    }


    private void handleDash()
    {
        if (_playerInput._isDash && _canDash)
        {
            if (PlayerManager.Instance.dash())
                StartCoroutine(startDash());
        }
    }
    private IEnumerator startDash()
    {
        _canDash = false;
        _isDashing = true;
        _rb.gravityScale = 0;
        _rb.linearVelocity = new Vector2(PlayerManager.Instance.Stats.getDashPower() * transform.localScale.x, 0f);
        _animManger.setDashing();
        _tr.emitting = true;
        yield return new WaitForSeconds(PlayerManager.Instance.Stats._dashingTime);
        resetDash();
        StartCoroutine(resetDashCooldown());
    }
    private void resetDash()
    {
        _playerInput.resetDash();
        _rb.linearVelocity = new Vector2(0f, 0f);
        _tr.emitting = false;
        _rb.gravityScale = _originalGravity;
        _isDashing = false;
    }
    private IEnumerator resetDashCooldown()
    {
        float _current = 0;
        float _time = PlayerManager.Instance.Stats.getDashingCooldown();
        while (_current < _time)
        {
            _current += Time.deltaTime;
            PlayerManager.Instance._dashTime = _current;
            yield return null;
        }
        _canDash = true;
    }


    // Jump
    private void handleJump()
    {
        bool _doubleJump = PlayerManager.Instance.Stats._doubleJump;
        // if (_doubleJump)
        //     Debug.Log("Có thể nhảy đôi, jumpCount: " + _jumpCount);
        // else 
        //     Debug.LogError("Không thể nhảy đôi");

        if ((_playerInput._isJump && _isGround) || (_playerInput._isJump && _jumpCount < 1 && PlayerManager.Instance.Stats._doubleJump))
        {
            StartCoroutine(fixDash());
            float _jumpForce = PlayerManager.Instance.Stats._jumpForce;
            // reset Velocity
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);

            if (!_isGround) _jumpCount++;
            //Debug.Log("Nhảy, jumpCount: " + _jumpCount);
            _animManger.setJumping();
            if (_isGround)
            {
                _canCheckGrounded = false;
                StartCoroutine(resetCanCheckGrounded());
            }
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            
        }
    }
    private IEnumerator fixDash()
    {
        yield return new WaitForSeconds(0.1f);
        if (_isDashing)
        {
            resetDash();
            StartCoroutine(resetDashCooldown());
        }
    }


    // attack
    private void handleAttack()
    {
        if (_playerInput._isAttack && _isGround && !_isDashing)
        {
            //Debug.Log("Đánh Nè");
            _animManger.setAttack();
        }
    }


    private void debug()
    {
        if (_playerInput._isMoving)
        {
            if (_playerInput._isRunning)
                Debug.Log("Đang chạy");
            else
                Debug.Log("Đang đi");
        }
        if (_playerInput._isSiting) Debug.Log("Đang ngồi");
        if (_isDashing) Debug.Log("đang Dash");
        if (!_canMove) Debug.Log("Đang Đánh");
        if (_isGround) Debug.Log("Đang đứng trên mặt đất");
        else if (_playerInput._isJumping) Debug.Log("Đang nhảy");
    }
}
