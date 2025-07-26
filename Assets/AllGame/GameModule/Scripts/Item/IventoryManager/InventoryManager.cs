using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public List<Item> _allItems = new List<Item>();
    public List<RtItem> _rtItemsWeapon = new List<RtItem>(); // gioi hang 10
    public List<RtItem> _rtItemsHelmet = new List<RtItem>(); // gioi hang 15
    public List<RtItem> _rtItemsArmor = new List<RtItem>(); // gioi hang 15
    public List<RtItem> _rtItemsBoots = new List<RtItem>(); // gioi hang 15
    public List<RtItem> _rtItemsAccesory = new List<RtItem>(); // gioi hang 15
    public List<RtItem> _rtItemsConsumahble = new List<RtItem>(); // gioi hang 60
    public EquipedItem _equipedItem = new EquipedItem();

    private SaveItem _saveItem;
    [SerializeField] private Transform _inventoryGOJ;
    private Transform _inventoryUIWeapon;
    private Transform _inventoryUIHelmet;
    private Transform _inventoryUIArmor;
    private Transform _inventoryUIBoots;
    private Transform _inventoryUIAccesory;
    private Transform _inventoryUIConsumahble;
    [SerializeField] private GameObject _itemPrefab;


    private Transform _EqueiItemUI;
    private Transform _HelmetPos;
    private Transform _ArmorPos;
    private Transform _BootsPos;
    private Transform _AccessoryPos;
    private Transform _MeleeWeaponPos;
    private Transform _RangedWeaponPos;
    private Transform _ConsumahblePos;
    [SerializeField] private GameObject _equeiItemPrefab;

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

        if (_inventoryGOJ == null)
            Debug.LogError("không có gameobject Profile. hãy thêm vào GameManager/InventoryManager/inventory GOJ");
        else
        {
            _inventoryUIWeapon = _inventoryGOJ.Find("InventoryMenu/Nen/Menu/Panel/Weapon/Content");
            _inventoryUIHelmet = _inventoryGOJ.Find("InventoryMenu/Nen/Menu/Panel/Helmet/Content");
            _inventoryUIArmor = _inventoryGOJ.Find("InventoryMenu/Nen/Menu/Panel/Armor/Content");
            _inventoryUIBoots = _inventoryGOJ.Find("InventoryMenu/Nen/Menu/Panel/Boots/Content");
            _inventoryUIAccesory = _inventoryGOJ.Find("InventoryMenu/Nen/Menu/Panel/Accessory/Content");
            _inventoryUIConsumahble = _inventoryGOJ.Find("InventoryMenu/Nen/Menu/Panel/Consumahble/Viewport/Content");

            _HelmetPos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/HelmetPos");
            _ArmorPos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/ArmorPos");
            _BootsPos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/BootsPos");
            _AccessoryPos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/AccessoryPos");
            _MeleeWeaponPos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/WeapoolMeleePos");
            _RangedWeaponPos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/WeapooRangedPos");
            _ConsumahblePos = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/ConsumahblePos");
            _EqueiItemUI = _inventoryGOJ.Find("InventoryMenu/Nen/Equie/EquieItemUI");
        }
    }


#if UNITY_EDITOR
    [ContextMenu("📦 Import All Items In 'Assets/AllGame/GameModule/Items'")]
    private void importAllItemsInFolderItems()
    {
        _allItems.Clear();

        // Tìm tất cả các asset loại Item trong thư mục Assets/AllGame/GameModule/Items
        string[] guids = AssetDatabase.FindAssets("t:Item", new[] { "Assets/AllGame/GameModule/Items" });

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Item item = AssetDatabase.LoadAssetAtPath<Item>(path);
            if (item != null)
            {
                _allItems.Add(item);
            }
        }

        Debug.Log($"✅ Đã import {_allItems.Count} item từ thư mục Assets/AllGame/GameModule/Items.");
    }
#endif


    public void AddItemInList(Item item)
    {
        RtItem rtItem = new RtItem(item);
        switch (rtItem._baseItem._itemType)
        {
            case ItemType.Weapon:
            {
                if (_rtItemsWeapon.Count < 10)
                    _rtItemsWeapon.Add(rtItem);
                else
                    Debug.Log("Túi đã đầy");
                break;
            }
            case ItemType.Helmet:
            {
                if (_rtItemsHelmet.Count < 15)
                    _rtItemsHelmet.Add(rtItem);
                else
                    Debug.Log("Túi đã đầy");
                break;
            }
            case ItemType.Armor:
            {
                if (_rtItemsArmor.Count < 15)
                    _rtItemsArmor.Add(rtItem);
                else
                    Debug.Log("Túi đã đầy");
                break;
            }
            case ItemType.Boots:
            {
                if (_rtItemsBoots.Count < 15)
                    _rtItemsBoots.Add(rtItem);
                else
                    Debug.Log("Túi đã đầy");
                break;
            }
            case ItemType.Accessory:
            {
                if (_rtItemsAccesory.Count < 15)
                    _rtItemsAccesory.Add(rtItem);
                else
                    Debug.Log("Túi đã đầy");
                break;
            }
            case ItemType.Consumable:
            {
                if (_rtItemsConsumahble.Count < 60)
                    _rtItemsConsumahble.Add(rtItem);
                else
                    Debug.Log("Túi đã đầy");
                break;
            }
        }
        DisplayInventory();
    }


    public void RemoveItemInList(RtItem item)
    {
        switch (item._baseItem._itemType)
        {
            case ItemType.Weapon:
            {
                _rtItemsWeapon.Remove(item);
                break;
            }
            case ItemType.Helmet:
            {
                _rtItemsHelmet.Remove(item);
                Debug.Log("Đã xóa Item ra khỏi túi");
                break;
            }
            case ItemType.Armor:
            {
                _rtItemsArmor.Remove(item);
                Debug.Log("Đã xóa Item ra khỏi túi");
                break;
            }
            case ItemType.Boots:
            {
                _rtItemsBoots.Remove(item);
                Debug.Log("Đã xóa Item ra khỏi túi");
                break;
            }
            case ItemType.Accessory:
            {
                _rtItemsAccesory.Remove(item);
                Debug.Log("Đã xóa Item ra khỏi túi");
                break;
            }
            case ItemType.Consumable:
            {
                _rtItemsConsumahble.Remove(item);
                Debug.Log("Đã xóa Item ra khỏi túi");
                break;
            }
        }
        DisplayInventory();
    }


    public void SaveInventory()
    {
        //_saveItem.SaveToJson(_rtItems);
    }


    public void ClearInventory()
    {
        _rtItemsWeapon.Clear();
        _rtItemsHelmet.Clear();
        _rtItemsArmor.Clear();
        _rtItemsBoots.Clear();
        _rtItemsAccesory.Clear();
        _rtItemsConsumahble.Clear();
    }


    public void LoadInventory()
    {
        //_saveItem.LoadFromJson(_rtItems);
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
        cleanInventory();

        foreach (RtItem item in _rtItemsWeapon)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;
            addItemInInventory(item, _inventoryUIWeapon);
        }
        foreach (RtItem item in _rtItemsHelmet)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;
            addItemInInventory(item, _inventoryUIHelmet);
        }
        foreach (RtItem item in _rtItemsArmor)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;
            addItemInInventory(item, _inventoryUIArmor);
        }
        foreach (RtItem item in _rtItemsBoots)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;
            addItemInInventory(item, _inventoryUIBoots);
        }
        foreach (RtItem item in _rtItemsAccesory)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;
            addItemInInventory(item, _inventoryUIAccesory);
        }
        foreach (RtItem item in _rtItemsConsumahble)
        {
            if (item._itemStatus != ItemStatus.UnEquip) continue;
            addItemInInventory(item, _inventoryUIConsumahble);
        }
    }

    private void cleanInventory()
    {
        foreach (Transform child in _inventoryUIWeapon)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _inventoryUIHelmet)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _inventoryUIArmor)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _inventoryUIBoots)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _inventoryUIAccesory)
        {
            Destroy(child.gameObject);
        }
        foreach (Transform child in _inventoryUIConsumahble)
        {
            Destroy(child.gameObject);
        }
    }

    private void addItemInInventory(RtItem item, Transform parent)
    {
        GameObject obj = Instantiate(_itemPrefab, parent);

        var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();

        _itemIcon.sprite = item._baseItem._itemIcon;

        obj.GetComponent<ItemUiController>().setRtItem(item);
    }

    public void DisplayEquipedIetm()
    {
        foreach (Transform child in _EqueiItemUI)
        {
            Destroy(child.gameObject);
        }

        if (_equipedItem._helmet != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _HelmetPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._helmet._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._helmet);
        }
        if (_equipedItem._armor != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _ArmorPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._armor._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._armor);
        }
        if (_equipedItem._boots != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _BootsPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._boots._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._boots);
        }
        if (_equipedItem._accessory != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _AccessoryPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._accessory._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._accessory);
        }
        if (_equipedItem._MeleeWeapon != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _MeleeWeaponPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._MeleeWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._MeleeWeapon);
        }
        if (_equipedItem._RangedWeapon != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _RangedWeaponPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._RangedWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._RangedWeapon);
        }
        if (_equipedItem._Consumahble != null)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _ConsumahblePos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._RangedWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._Consumahble);
        }
    }
}
