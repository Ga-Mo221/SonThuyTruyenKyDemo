using System.Collections;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private Transform _Profile;
    private GameObject _inventoryMenu;
    private GameObject _Option;

    public float _moveInput { get; private set; }
    public bool _isMoving { get; private set; }
    public bool _isRunning { get; private set; }
    public bool _isDash { get; private set; }
    public bool _isSiting { get; private set; }
    public bool _isJumping { get; private set; }
    public bool _isJump { get; private set; }
    public bool _isAttack { get; private set; }

    private bool _openIventory = false;

    void Start()
    {
        if (_Profile == null)
        {
            Debug.LogError("Chưa gắn Profile và prefab Player/PlayerInput");
        }
        _inventoryMenu = _Profile.Find("InventoryMenu").gameObject;
        _Option = _Profile.Find("Option").gameObject;
    }

    private void Update()
    {
        if (PlayerManager.Instance._isAlive && !PlayerManager.Instance._knocked)
        {
            if (PlayerManager.Instance.Stats._tutorialSit)
                handleSitAndDash();
            if (PlayerManager.Instance.Stats._tutorialJump)
                handleJump();
            if (PlayerManager.Instance.Stats._tutorialAttack)
                handleAttack();
            handleMove();
        }
        else
        {
            _isMoving = false;
            _isRunning = false;
        }
        openInventory();
        openOption();
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
        if (_isMoving && _isrun && _lastDirection != _moveInput && !_isSiting && !_isJumping)
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && !_isSiting && _isMoving && !_isRunning && PlayerManager.Instance._stamina > 0 && PlayerManager.Instance.Stats._tutorialRun && !_isJumping)
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
        if (Input.GetKeyDown(KeyCode.S))
        {
            if ((_isJumping || _isRunning) && PlayerManager.Instance.Stats._tutorialDash)
            {
                _isDash = true;
                if (_resetDash != null)
                    StopCoroutine(_resetDash);
                _resetDash = StartCoroutine(resetDash());
            }
            else
            {
                if (!_isRunning)
                    _isSiting = !_isSiting;
            }
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _isSiting = false;
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
            else
            {
                if (!_isDash)
                {
                    _isJump = true;
                    StartCoroutine(resetJump());
                }
            }
        }
    }
    private IEnumerator resetJump()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
#endif
        _isJump = false;
    }
    public void setJumping(bool _isGrounded) => _isJumping = !_isGrounded;



    //attack basic
    public bool _canAttack = true;
    private float _canAttackCooldown = 0.3f;
    private void handleAttack()
    {
        if (Input.GetMouseButtonDown(0) && _canAttack)
        {
            //Debug.Log("ckick Đánh nè");
            _isAttack = true;
            if (_isSiting) _isSiting = !_isSiting;
            StartCoroutine(resetAttack());
        }
    }
    public IEnumerator resetAttack()
    {
        yield return new WaitForEndOfFrame();
#if UNITY_EDITOR
        yield return new WaitForEndOfFrame();
#endif
        _isAttack = false;
    }
    public void resetCanAttact()
    {
        PlayerManager.Instance._canMoveAttack = false;
        StartCoroutine(_canAttackk());
    }
    private IEnumerator _canAttackk()
    {
        yield return new WaitForSeconds(_canAttackCooldown);
        _canAttack = true;
    }
    public void setCanAttack() => _canAttack = false;




    // inventory
    private void openInventory()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && PlayerManager.Instance._isAlive)
        {
            Debug.Log("Open Inventory");
            bool isActive = _inventoryMenu.activeSelf;
            _inventoryMenu.SetActive(!isActive);
            _openIventory = !isActive;
            if (!isActive)
            {
                PlayerManager.Instance._knocked = true;
            }
            else
                PlayerManager.Instance._knocked = false;
        }
    }


    // Option
    private void openOption()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool isActive = _Option.activeSelf;
            if (!_openIventory)
            {
                _Option.SetActive(!isActive);

                if (!isActive)
                {
                    PlayerManager.Instance._knocked = true;
                }
                else
                    PlayerManager.Instance._knocked = false;
            }
            else
            {
                _inventoryMenu.SetActive(false);
                _openIventory = false;
                PlayerManager.Instance._knocked = false;
            }
        }
    }
}
