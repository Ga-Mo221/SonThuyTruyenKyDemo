using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject F;
    private Animator _anim;

    private bool _canOpen;
    private bool _isOpened = false;

    public bool _isXeng = false;
    [ShowIf(nameof(_isXeng))]
    [SerializeField] private int _xengCount = 0;

    public bool _isItems = false;
    [ShowIf(nameof(_isItems))]
    [SerializeField] private List<GameObject> _allItemPrefab;

    public bool _showPrefab = false;
    [ShowIf(nameof(_showPrefab))]
    [SerializeField] private GameObject _xengPrefab;
    [SerializeField] private Transform _parrent;

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

            if (_isXeng)
            {
                int _xengValue = _xengPrefab.GetComponent<CurrencyDisplay>()._XengValue;

                for (int i = 0; i < _xengCount / _xengValue; i++)
                {
                    Instantiate(_xengPrefab, transform.position, Quaternion.identity, _parrent);
                }
            }
            if (_isItems)
            {
                foreach (var item in _allItemPrefab)
                {
                    Instantiate(item, transform.position, Quaternion.identity, _parrent);
                }
            }
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
