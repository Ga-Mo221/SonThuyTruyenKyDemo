using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUiController : MonoBehaviour, IPointerClickHandler
{
    public RtItem _rtItem;
    public GameObject _contextMenuPrefab;
    public Transform _displayPos;
    public bool _pickUp = false;

    private GameObject _canvas;
    private Transform _overlay;

    private void Start()
    {
        if (_rtItem._itemStatus == ItemStatus.UnEquip || _rtItem._itemStatus == ItemStatus.Equip)
            _canvas = GameObject.Find("InventoryMenu");
        else if (_rtItem._itemStatus == ItemStatus.Drop)
            _canvas = GameObject.Find("ItemPickUp");
        // in shop
        
        if (_canvas == null)
        {
            Debug.LogError("không tìm thấy _canvas");
        }
        else
        {
            _overlay = _canvas.transform.Find("Overlay");
            if (_overlay == null)
            {
                Debug.LogError("không tìm thấy Overlay trong InventoryMenu");
            }
        }
    }

    public void setRtItem(RtItem item)
    {
        _rtItem = item;
    }

    public void removeUIItem()
    {
        // InventoryManager.Instance._rtItems.Remove(_rtItem);
        // Destroy(gameObject);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    private void OnLeftClick()
    {
        Debug.Log("🖱️ Chuột trái click" + _rtItem._baseItem._itemName);
    }

    private void OnRightClick()
    {
        _overlay.gameObject.SetActive(true);
        Vector3 _pos = _displayPos.position;
        GameObject _inventoryContextMenu = Instantiate(_contextMenuPrefab, _pos, Quaternion.identity, _overlay);
        _overlay.GetComponent<OverlayClick>().SetContextMenu(_inventoryContextMenu.GetComponent<ContextMenuController>());
        _inventoryContextMenu.GetComponent<ContextMenuController>().setRtItem(_rtItem, true, this);
    }
}
