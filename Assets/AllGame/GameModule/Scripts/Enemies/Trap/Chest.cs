using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject F;
    private Animator _anim;

    private bool _canOpen;
    private bool _isOpened = false;

    public bool _Enemy = false;
    private int _enemyDieCount;
    
    public bool _isXeng = false;
    [ShowIf(nameof(_isXeng))]
    [SerializeField] private Transform _coinDropParrent;
    [ShowIf(nameof(_isXeng))]
    [SerializeField] private int _xengCount = 0;

    public bool _isItems = false;
    [ShowIf(nameof(_isItems))]
    [SerializeField] public Transform _itemsDropParent;
    [ShowIf(nameof(_isItems))]
    [SerializeField] private List<Item> _allItem;
    public List<RtItem> _rtItems = new List<RtItem>();
    private List<GameObject> _items = new List<GameObject>();

    public bool _showPrefab = false;
    [ShowIf(nameof(_showPrefab))]
    [SerializeField] private GameObject _tuiItems;
    [ShowIf(nameof(_showPrefab))]
    [SerializeField] private GameObject _xengPrefab;
    [ShowIf(nameof(_showPrefab))]
    [SerializeField] private GameObject _itemPrefab;

    void Awake()
    {
        if (_isItems && _itemsDropParent == null)
        {
            Debug.LogError("hãy gắn profile/ItemPickUp/ScrollView/Viewport/Content vào ");
        }
        if (_isXeng && _coinDropParrent == null)
        {
            Debug.LogError("hãy tạo một gameobj rỗng và gắn vào");
        }
        _anim = GetComponent<Animator>();
        if (_isItems && _allItem.Count > 0)
        {
            foreach (var item in _allItem)
            {
                RtItem _rtItem = new RtItem(item);
                _rtItem.setItemStatus(ItemStatus.Drop);
                _rtItems.Add(_rtItem);
            }
        }
    }

    // void Start()
    // {
    //     if (_Enemy)
    //     {
    //         transform.GetComponent<SpriteRenderer>().enabled = false;
    //         spawnTuiItems(30f);
    //     }
    // }

    void Update()
    {
        openChest();
        if (_isItems && _Enemy)
        {
            if (_enemyDieCount > 0)
                _isItems = false;
        }

        if (_isItems && _items.Count > 0)
        {
            foreach (var item in _items)
            {
                if (item == null) continue;
                ItemUiController _itemUI = item.GetComponent<ItemUiController>();
                if (_itemUI != null)
                {
                    if (_itemUI._pickUp)
                    {
                        _items.Remove(item);
                        _rtItems.Remove(_itemUI._rtItem);
                        displayItems();
                        return;
                    }
                }
                else
                    Debug.LogError("Không tìm thấy ItemUiController");
            }
        }
    }

    public void setEnemyDieCount(int count)
    {
        _enemyDieCount = count;
    }

    public void enemyDie(float DestroyTime)
    {
        dropCoin(); 
        if (_enemyDieCount > 0) return;
        GameObject _tui = Instantiate(_tuiItems, transform.position, Quaternion.identity);
        _tui.transform.Find("Rada").GetComponent<TuiItems>().setTuiItems(this);
        StartCoroutine(destroyTuiItems(DestroyTime, _tui));
    }

    private IEnumerator destroyTuiItems(float DestroyTime, GameObject tuiItems)
    {
        yield return new WaitForSeconds(DestroyTime);
        Destroy(tuiItems);
    }

    private void openChest()
    {
        if (Input.GetKeyDown(KeyCode.F) && _canOpen && !_Enemy)
        {
            F.SetActive(false);
            _anim.SetTrigger("open");
            // linh an o day
            dropCoin();

            if (_isItems && _rtItems.Count > 0)
            {
                displayItems();
                _itemsDropParent.parent.parent.parent.gameObject.SetActive(true);
                PlayerManager.Instance._knocked = true;
            }
            
        }
    }

    public void dropCoin()
    {
        if (_isXeng && !_isOpened)
        {
            int _xengValue = _xengPrefab.GetComponent<CurrencyDisplay>()._XengValue;

            for (int i = 0; i < _xengCount / _xengValue; i++)
            {
                Instantiate(_xengPrefab, transform.position, Quaternion.identity, _coinDropParrent);
            }
        }
        _isOpened = true;
    }

    public void displayItems()
    {
        foreach (Transform child in _itemsDropParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var rtItem in _rtItems)
        {
            GameObject item = Instantiate(_itemPrefab, _itemsDropParent.position, Quaternion.identity, _itemsDropParent);
            ItemUiController itemUiController = item.GetComponent<ItemUiController>();
            if (itemUiController != null)
            {
                rtItem.setItemStatus(ItemStatus.Drop);
                itemUiController.setRtItem(rtItem);
                _items.Add(item);
                var _itemIcon = item.transform.Find("ItemIcon").GetComponent<Image>();
                _itemIcon.sprite = rtItem._baseItem._itemIcon;
            }
            else
            {
                Debug.LogError("Không tìm thấy ItemUiController");
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_Enemy)
        {
            if (_isXeng && !_isOpened)
            {
                _canOpen = true;
                F.SetActive(true);
            }
            if (_isItems && _rtItems.Count > 0)
            {
                _canOpen = true;
                F.SetActive(true);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (_Enemy) return;
        _canOpen = false;
        F.SetActive(false);
    }
}
