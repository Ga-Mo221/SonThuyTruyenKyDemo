using UnityEngine;

public class Da : MonoBehaviour
{
    void OnTriggerStay2D(Collider2D collision)
    {
        GameObject parentObject = collision.gameObject.transform.parent?.gameObject;
        PlayerInput _playerInput = parentObject.GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            if (_playerInput._isRunning)
            {
                PlayerManager.Instance.Stats._delay = 80;
            }
            else if (_playerInput._isMoving)
            {
                PlayerManager.Instance.Stats._delay = 90;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PlayerManager.Instance.Stats._delay = 0;
    }
}
