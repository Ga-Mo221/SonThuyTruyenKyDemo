using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemUiController : MonoBehaviour, IPointerClickHandler
{
    public RtItem _rtItem;
    public GameObject _contextMenuPrefab;
    public Transform _displayPos;

    private GameObject _canvas;
    private Transform _overlay;

    private void Start()
    {
        _canvas = GameObject.Find("InventoryMenu");
        if (_canvas == null)
        {
            Debug.LogError("không tìm thấy InventoryMenu");
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
        InventoryManager.Instance._rtItems.Remove(_rtItem);
        Destroy(gameObject);
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
        if (_rtItem._itemStatus == ItemStatus.UnEquip)
        {
            GameObject _inventoryContextMenu = Instantiate(_contextMenuPrefab, _pos, Quaternion.identity, _overlay);
            _overlay.GetComponent<OverlayClick>().SetContextMenu(_inventoryContextMenu.GetComponent<ContextMenuController>());
            _inventoryContextMenu.GetComponent<ContextMenuController>().setRtItem(_rtItem);
        }
    }
}
