using UnityEngine;
using UnityEngine.EventSystems;

public class OverlayClick : MonoBehaviour, IPointerClickHandler
{
    public ContextMenuController _contextMenu;
    public GameObject Overlay;

    public void SetContextMenu(ContextMenuController contextMenu)
    {
        _contextMenu = contextMenu;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_contextMenu != null)
        {
            _contextMenu.CloseContextMenu();
        }
        else
        {
            Debug.LogError("❌ contextMenu chưa được gán!");
        }
        //transform.gameObject.SetActive(false);
    }
}
