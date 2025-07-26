using UnityEngine;
using UnityEngine.UI;

public class ContextMenuController : MonoBehaviour
{
    private RtItem _rtItem;
    private bool _rightClick;

    [SerializeField] private GameObject _panelUse;
    [SerializeField] private Button _buttonUse;
    [SerializeField] private Button _buttonEquie;
    [SerializeField] private Button _buttonRepair;

    [SerializeField] private GameObject _panelEquei;

    public void setRtItem(RtItem item, bool rightClick)
    {
        _rtItem = item;
        _rightClick = rightClick;
    }

    void Start()
    {
        if (_rightClick && _rtItem._itemStatus == ItemStatus.UnEquip)
        {
            _panelUse.SetActive(true);
            if (_rtItem._baseItem._itemType == ItemType.Consumable)
            {
                _buttonUse.enabled = true;
                _buttonEquie.enabled = true;
                _buttonRepair.enabled = false;
            }
            else
            {
                _buttonUse.enabled = false;
                _buttonEquie.enabled = true;
                _buttonRepair.enabled = true;
            }
        }
        else if (_rightClick && _rtItem._itemStatus == ItemStatus.Equip)
        {
            _panelEquei.SetActive(true);
        }
    }

    public void useButton()
    {
        Debug.Log("đã sử dụng " + _rtItem._baseItem._itemName);
        CloseContextMenu();
    }

    public void equipButton()
    {
        //InventoryManager.Instance.Equip(_rtItem);
        Debug.Log("Đã Trang bị " + _rtItem._baseItem._itemName);
        InventoryManager.Instance.Equip(_rtItem);
        CloseContextMenu();
    }

    public void unEquieButton()
    {
        CloseContextMenu();
        Debug.Log("Đã Gỡ Trang bị " + _rtItem._baseItem._itemName);
        InventoryManager.Instance.UnEquip(_rtItem);
    }

    public void reqairButton()
    {
        Debug.Log("Đã sửa Trang bị " + _rtItem._baseItem._itemName);
        CloseContextMenu();
    }

    public void removeButton()
    {
        Debug.Log("Đã xóa Trang bị " + _rtItem._baseItem._itemName);
        CloseContextMenu();
    }

    public void CloseContextMenu()
    {
        Destroy(gameObject);
        transform.parent.gameObject.SetActive(false);
    }
}