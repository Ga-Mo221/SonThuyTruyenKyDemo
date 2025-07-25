using UnityEngine;

public class NextMiniMap : MonoBehaviour
{
    public bool _next = true;
    [SerializeField] private GameObject _Map;
    [SerializeField] private GameObject _Camera;

    void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerInput _playerInput = collision.GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            float x = _playerInput._moveInput;

            if (_next)
            {
                if (x > 0.1)
                {
                    _Map.SetActive(true);
                    //Debug.Log("Map B mở " + x);
                }
                else
                {
                    _Camera.SetActive(false);
                    //Debug.Log("Cam B Đóng " + x);
                }
            }
            else
            {
                if (x > 0.1)
                {
                    _Camera.SetActive(false);
                    //Debug.Log("Cam A Đóng " + x);
                }
                else
                {
                    _Map.SetActive(true);
                    //Debug.Log("Map A mở " + x);
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        PlayerInput _playerInput = collision.GetComponent<PlayerInput>();
        if (_playerInput != null)
        {
            float x = _playerInput._moveInput;

            if (_next)
            {
                if (x < -0.1)
                {
                    _Map.SetActive(false);
                    //Debug.Log("Map B Đóng " + x);
                }
                else
                {
                    _Camera.SetActive(true);
                    //Debug.Log("Cam B mở " + x);
                }
            }
            else
            {
                if (x > 0.1)
                {
                    _Map.SetActive(false);
                    //Debug.Log("Map A đóng " + x);
                }
                else
                {
                    _Camera.SetActive(true);
                    //Debug.Log("Cam A mở " + x);
                }
            }
        }
    }
}
