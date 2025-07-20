using UnityEngine;

public class PlayerChangeStateCollider : MonoBehaviour
{
    [SerializeField] private CapsuleCollider2D _upState;
    [SerializeField] private CapsuleCollider2D _downState;
    [SerializeField] private BoxCollider2D _meterial;
    
    private PlayerController _playerController;
    private PlayerInput _playerInput;

    void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _playerInput = GetComponent<PlayerInput>();
    }

    void FixedUpdate()
    {
        changeState();
    }

    private void changeState()
    {
        bool isDown = _playerController._isDashing || _playerInput._isSiting;
        if (_downState.enabled != isDown)
        {
            _downState.enabled = isDown;
            _upState.enabled = !isDown;
            _meterial.enabled = !isDown;
        }
    }
}
