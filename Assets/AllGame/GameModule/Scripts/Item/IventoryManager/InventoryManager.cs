using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<Item> _allItems = new List<Item>();
    public List<RtItem> _rtItems = new List<RtItem>();
    public EquipedItem _equipedItem = new EquipedItem();

    private SaveItem _saveItem;
    public Transform _inventoryUI;
    public GameObject _itemPrefab;

    public Toggle _enableRemoveItem;

    public Transform _equiepItemUI;
    public Transform _HelmetPos;
    public Transform _ArmorPos;
    public Transform _BootsPos;
    public Transform _AccessoryPos;
    public Transform _MeleeWeaponPos;
    public Transform _RangedWeaponPos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    void Start()
    {
        _saveItem = GetComponent<SaveItem>();
        if (_saveItem == null)
        {
            Debug.LogWarning("không tìm thấy SaveItem component");
            return;
        }
    }


#if UNITY_EDITOR
    [ContextMenu("📦 Import All Items In 'Assets/Items'")]
    private void importAllItemsInFolderItems()
    {
        _allItems.Clear();

        // Tìm tất cả các asset loại Item trong thư mục Assets/Items
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { "Assets/Items" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            if (item != null)
            {
                _allItems.Add(item);
            }
        }

        Debug.Log($"✅ Đã import {_allItems.Count} item từ thư mục Assets/Items.");
    }
#endif


    public void AddItem(Item item)
    {
        RtItem rtItem = new RtItem(item);
        _rtItems.Add(rtItem);
        DisplayInventory();
    }


    public void RemoveItem(RtItem item)
    {
        _rtItems.Remove(item);
        DisplayInventory();
    }


    public void SaveInventory()
    {
        _saveItem.SaveToJson(_rtItems);
    }


    public void ClearInventory()
    {
        _rtItems.Clear();
    }


    public void LoadInventory()
    {
        _saveItem.LoadFromJson(_rtItems);
    }


    public void Equip(RtItem item)
    {
        _equipedItem.equip(item);
        DisplayInventory();
        DisplayEquipedIetm();
    }


    public void UnEquip(RtItem item)
    {
        _equipedItem.unEquip(item);
        DisplayInventory();
        DisplayEquipedIetm();
    }


    public void DisplayInventory()
    {
        foreach (Transform child in _inventoryUI)
        {
            Destroy(child.gameObject);
        }

        foreach (RtItem item in _rtItems)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;

            GameObject obj = Instantiate(_itemPrefab, _inventoryUI);

            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

            _itemIcon.sprite = item._baseItem._itemIcon;

            obj.GetComponent<ItemUiController>().setRtItem(item);
        }

        EnableRemoveButton();
    }

    public void DisplayEquipedIetm()
    {
        foreach (Transform child in _equiepItemUI)
        {
            Destroy(child.gameObject);
        }

        if (_equipedItem._helmet._baseItem != null)
        {
            GameObject obj = Instantiate(_itemPrefab, _HelmetPos.position, Quaternion.identity, _equiepItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._helmet._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._helmet);
        }
        if (_equipedItem._armor._baseItem != null)
        {
            GameObject obj = Instantiate(_itemPrefab, _ArmorPos.position, Quaternion.identity, _equiepItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._armor._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._armor);
        }
        if (_equipedItem._boots._baseItem != null)
        {
            GameObject obj = Instantiate(_itemPrefab, _BootsPos.position, Quaternion.identity, _equiepItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._boots._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._boots);
        }
        if (_equipedItem._accessory._baseItem != null)
        {
            GameObject obj = Instantiate(_itemPrefab, _AccessoryPos.position, Quaternion.identity, _equiepItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._accessory._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._accessory);
        }
        if (_equipedItem._MeleeWeapon._baseItem != null)
        {
            GameObject obj = Instantiate(_itemPrefab, _MeleeWeaponPos.position, Quaternion.identity, _equiepItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._MeleeWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._MeleeWeapon);
        }
        if (_equipedItem._RangedWeapon._baseItem != null)
        {
            GameObject obj = Instantiate(_itemPrefab, _RangedWeaponPos.position, Quaternion.identity, _equiepItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._RangedWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._RangedWeapon);
        }
    }


    public void EnableRemoveButton()
    {
        if (_enableRemoveItem.isOn)
        {
            foreach (Transform child in _inventoryUI)
            {
                child.Find("RemoveItem").gameObject.SetActive(true);
            }
        }
        else
        {
            foreach (Transform child in _inventoryUI)
            {
                child.Find("RemoveItem").gameObject.SetActive(false);
            }
        }
    }
}
