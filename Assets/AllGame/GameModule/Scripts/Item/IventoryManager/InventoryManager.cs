using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



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
    [SerializeField] private Transform _inventoryUIWeapon;
    [SerializeField] private Transform _inventoryUIHelmet;
    [SerializeField] private Transform _inventoryUIArmor;
    [SerializeField] private Transform _inventoryUIBoots;
    [SerializeField] private Transform _inventoryUIAccesory;
    [SerializeField] private Transform _inventoryUIConsumahble;
    [SerializeField] private GameObject _itemPrefab;


    [SerializeField] private Transform _EqueiItemUI;
    [SerializeField] private Transform _HelmetPos;
    [SerializeField] private Transform _ArmorPos;
    [SerializeField] private Transform _BootsPos;
    [SerializeField] private Transform _AccessoryPos;
    [SerializeField] private Transform _MeleeWeaponPos;
    [SerializeField] private Transform _RangedWeaponPos;
    [SerializeField] private Transform _ConsumahblePos;
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

    public void AddItemInList(RtItem rtItem)
    {
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
        ApplyItem(item, true);
    }


    public void UnEquip(RtItem item)
    {
        _equipedItem.unEquip(item);
        DisplayInventory();
        DisplayEquipedIetm();
        ApplyItem(item, false);
    }

    private void ApplyItem(RtItem item, bool equip)
    {
        float _physic = item._baseItem._itemDamagePhysical + item._bonusDamagePhysical;
        float _magic = item._baseItem._itemDamgeMagic + item._bonusDamgeMagic;
        float _attackSpeed = item._baseItem._itemSpeedAttack + item._bonusSpeedAttack;
        float _moveSpeed = item._baseItem._itemSpeedMove + item._bonusSpeedMove;
        float _health = item._baseItem._itemHealth + item._bonusHealth;
        float _critPhysic = item._baseItem._itemCritChanceMagic + item._bonusCritChanceMagic;
        float _critMagic = item._baseItem._itemcritChancePhysical + item._bonuscritChancePhysical;
        float _armor = item._baseItem._itemArmor + item._bonusArmor;
        float _physicRes = item._baseItem._itemPhysicRes + item._bonusPhysicRes;
        float _magicRes = item._baseItem._itemMagicResist + item._bonusMagicResist;
        float _cooldown = item._baseItem._itemCooldownReduction + item._bonusCooldownReduction;
        if (equip)
        {
            PlayerManager.Instance.Stats._physicalDamage += _physic;
            PlayerManager.Instance.Stats._magicDamage += _magic;
            PlayerManager.Instance.Stats._runSpeed += _moveSpeed;
            PlayerManager.Instance.Stats._walkSpeed += _moveSpeed;
            PlayerManager.Instance.Stats._attackSpeed += _attackSpeed;
            PlayerManager.Instance.Stats._maxHealth += _health;
            PlayerManager.Instance.Stats._currentHealth += _health;
            PlayerManager.Instance.Stats._critChancePhysical += _critPhysic;
            PlayerManager.Instance.Stats._critChanceMagic += _critMagic;
            PlayerManager.Instance.Stats._armor += _armor;
            PlayerManager.Instance.Stats._physicalDefense += _physicRes;
            PlayerManager.Instance.Stats._magicResist += _magicRes;
            PlayerManager.Instance.Stats._cooldownReduction += _cooldown;
        }
        else
        {
            PlayerManager.Instance.Stats._physicalDamage -= _physic;
            PlayerManager.Instance.Stats._magicDamage -= _magic;
            PlayerManager.Instance.Stats._runSpeed -= _moveSpeed;
            PlayerManager.Instance.Stats._walkSpeed -= _moveSpeed;
            PlayerManager.Instance.Stats._attackSpeed -= _attackSpeed;
            PlayerManager.Instance.Stats._maxHealth -= _health;
            PlayerManager.Instance.Stats._currentHealth -= _health;
            PlayerManager.Instance.Stats._critChancePhysical -= _critPhysic;
            PlayerManager.Instance.Stats._critChanceMagic -= _critMagic;
            PlayerManager.Instance.Stats._armor -= _armor;
            PlayerManager.Instance.Stats._physicalDefense -= _physicRes;
            PlayerManager.Instance.Stats._magicResist -= _magicRes;
            PlayerManager.Instance.Stats._cooldownReduction -= _cooldown;
        }
        PlayerManager.Instance.setAttackSpeed();
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
        if (_inventoryUIWeapon == null)
        {
            Debug.LogError("không có gameobject InventoryUIWeapon.");
            return;
        }
        foreach (Transform child in _inventoryUIWeapon)
        {
            Destroy(child.gameObject);
        }
        if (_inventoryUIHelmet == null)
        {
            Debug.LogError("không có gameobject _inventoryUIHelmet.");
            return;
        }
        foreach (Transform child in _inventoryUIHelmet)
        {
            Destroy(child.gameObject);
        }
        if (_inventoryUIArmor == null)
        {
            Debug.LogError("không có gameobject _inventoryUIArmor.");
            return;
        }
        foreach (Transform child in _inventoryUIArmor)
        {
            Destroy(child.gameObject);
        }
        if (_inventoryUIBoots == null)
        {
            Debug.LogError("không có gameobject _inventoryUIBoots.");
            return;
        }
        foreach (Transform child in _inventoryUIBoots)
        {
            Destroy(child.gameObject);
        }
        if (_inventoryUIAccesory == null)
        {
            Debug.LogError("không có gameobject _inventoryUIAccesory.");
            return;
        }
        foreach (Transform child in _inventoryUIAccesory)
        {
            Destroy(child.gameObject);
        }
        if (_inventoryUIConsumahble == null)
        {
            Debug.LogError("không có gameobject _inventoryUIConsumahble.");
            return;
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

        if (_equipedItem._isHelmet)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _HelmetPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._helmet._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._helmet);
        }
        if (_equipedItem._isArmor)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _ArmorPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._armor._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._armor);
        }
        if (_equipedItem._isBoots)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _BootsPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._boots._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._boots);
        }
        if (_equipedItem._isAccesory)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _AccessoryPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._accessory._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._accessory);
        }
        if (_equipedItem._isMelleWeapon)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _MeleeWeaponPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._MeleeWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._MeleeWeapon);
        }
        if (_equipedItem._isRangedWeapon)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _RangedWeaponPos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._RangedWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._RangedWeapon);
        }
        if (_equipedItem._isConsumahble)
        {
            GameObject obj = Instantiate(_equeiItemPrefab, _ConsumahblePos.position, Quaternion.identity, _EqueiItemUI);
            var _itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            _itemIcon.sprite = _equipedItem._RangedWeapon._baseItem._itemIcon;
            obj.GetComponent<ItemUiController>().setRtItem(_equipedItem._Consumahble);
        }
    }
}
