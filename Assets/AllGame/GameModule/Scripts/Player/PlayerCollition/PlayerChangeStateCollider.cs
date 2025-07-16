using UnityEngine;

public class PlayerChangeStateCollider : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _upState;
    [SerializeField] private BoxCollider2D _downState;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerInput _playerInput;

    void Update()
    {
        changeState();
    }

    private void changeState()
    {
        if (_playerController._isDashing || _playerInput._isSiting)
        {
            _upState.enabled = false;
            _downState.enabled = true;
        }
        else
        {
            _upState.enabled = true;
            _downState.enabled = false;
        }
    }
}
