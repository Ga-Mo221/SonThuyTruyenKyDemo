using UnityEngine;

public class NextMiniMap : MonoBehaviour
{
    public bool _next = true;
    [SerializeField] private GameObject _Map;
    [SerializeField] private GameObject _Camera;
    private float _direction;

    void Start()
    {
        _direction = _next ? 1 : -1;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.localScale.x == _direction)
        {
            _Map.SetActive(true);
        }
        else
        {
            _Camera.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.localScale.x == -_direction)
        {
            _Map.SetActive(false);
            _Camera.SetActive(false);
        }
        else
        {
            _Camera.SetActive(true);
        }
    }
}
