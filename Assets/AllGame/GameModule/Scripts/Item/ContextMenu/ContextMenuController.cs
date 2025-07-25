using UnityEngine;

public class ContextMenuController : MonoBehaviour
{
    public RtItem _rtItem;

    public void setRtItem(RtItem item)
    {
        _rtItem = item;
    }

    public void useButton()
    {
        Debug.Log("đã sử dụng " + _rtItem._baseItem._itemName);
        CloseContextMenu();
    }

    public void equipButton()
    {
        InventoryManager.Instance.Equip(_rtItem);
        CloseContextMenu();
    }

    public void CloseContextMenu()
    {
        Destroy(gameObject);
        transform.parent.gameObject.SetActive(false);
    }
}