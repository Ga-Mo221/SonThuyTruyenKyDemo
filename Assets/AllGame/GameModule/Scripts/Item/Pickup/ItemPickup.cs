using UnityEngine;


public class ItemPickup : MonoBehaviour
{
    public Item item;

    private void Pickup()
    {
        if (item == null)
        {
            Debug.LogWarning("không có item nào được gắn");
            return;
        }

        InventoryManager.Instance.AddItem(item);
    }

    void OnMouseDown()
    {
        Pickup();
        //Destroy(gameObject);
    }
}
