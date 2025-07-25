using System.Collections.Generic;
using UnityEngine;

public class SaveItem : MonoBehaviour
{
    public void SaveToJson(List<RtItem> RtItems)
    {
        InventorySaveData saveData = new InventorySaveData();
        saveData.rtItems = RtItems;
        string path = Application.persistentDataPath + "/inventory.json";

        string json = JsonUtility.ToJson(saveData, true); // đẹp và dễ đọc


        System.IO.File.WriteAllText(path, json);
        Debug.Log("✅ Inventory đã được lưu tại: " + path);
    }

    public void LoadFromJson(List<RtItem> RtItems)
    {
        string path = Application.persistentDataPath + "/inventory.json";

        if (!System.IO.File.Exists(path))
        {
            Debug.LogWarning("⚠️ Không tìm thấy file inventory!");
            return;
        }

        string json = System.IO.File.ReadAllText(path);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        RtItems.Clear();

        foreach (var item in saveData.rtItems)
        {
            Item baseItem = InventoryManager.Instance._allItems.Find(x => x._itemID == item._itemID);
            if (baseItem != null)
            {
                item.Bind(baseItem);
                RtItems.Add(item);
            }
            else
            {
                Debug.LogWarning($"⚠️ Không tìm thấy Item với ID: {item._itemID} trong allItems!");
            }
        }

        Debug.Log($"✅ Đã load {RtItems.Count} item từ file JSON.");
    }
}