using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject F;
    private Animator _anim;

    private bool _canOpen;
    private bool _isOpened = false;

    void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        openChest();
    }

    private void openChest()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canOpen && !_isOpened)
        {
            _isOpened = true;
            F.SetActive(false);
            _anim.SetTrigger("open");
            PlayerManager.Instance.Stats._xeng += 100;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _canOpen = true;
            if (!_isOpened) F.SetActive(true);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        _canOpen = false;
        F.SetActive(false);
    }
}
