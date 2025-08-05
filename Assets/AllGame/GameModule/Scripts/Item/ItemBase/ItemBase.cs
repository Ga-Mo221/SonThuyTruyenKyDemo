using UnityEngine;

public enum ItemType
{
    Helmet, // nón
    Armor, // giáp
    Weapon, // vũ khí
    Boots, // giày
    Accessory, // phụ kiện
    Consumable // vật phẩm tiêu hao
}

public enum ItemRarity
{
    Common, // thường
    Rare, // quý hiếm
    Legendary // huyền thoại cao cấp
}

// vũ khí cận chiến hay tầm xa
public enum WeaponType
{
    Melee, // cận chiến
    Ranged, // tầm xa
    // không phải vũ khí
    None
}

// equip
public enum ItemStatus
{
    Equip,
    UnEquip,
    InShop,
    Drop
}

[CreateAssetMenu(fileName = "New Item", menuName = "Items/create New Item")]
public class Item : ScriptableObject
{
    public string _itemID;
    public string _itemName;
    public Sprite _itemIcon; // icon của vật phẩm
    public ItemType _itemType;
    public ItemRarity _itemRarity; // độ hiếm
    public WeaponType _weaponType; // loại vũ khí, nếu không phải vũ khí thì None
    public float _itemDamagePhysical;
    public float _itemDamgeMagic;
    public float _itemSpeedAttack;
    public float _itemSpeedMove;
    public float _itemHealth;
    public float _itemcritChancePhysical;
    public float _itemCritChanceMagic;
    public float _itemArmor;
    public float _itemPhysicRes;
    public float _itemMagicResist;
    public float _itemCooldownReduction;
    public int _itemPrice;
    public int _itemSellPrice;
    public float _itemCooldownTime; // thời gian hồi chiêu cua item tieu hao
}

[System.Serializable]
public class RtItem
{
    public string _itemID;
    public string _instanceID;
    public ItemStatus _itemStatus = ItemStatus.UnEquip;
    public float _bonusDamagePhysical = 0f;
    public float _bonusDamgeMagic = 0f;
    public float _bonusSpeedAttack = 0f;
    public float _bonusSpeedMove = 0f;
    public float _bonusHealth = 0f;
    public float _bonuscritChancePhysical = 0f;
    public float _bonusCritChanceMagic = 0f;
    public float _bonusArmor = 0f;
    public float _bonusPhysicRes = 0f;
    public float _bonusMagicResist = 0f;
    public float _bonusCooldownReduction = 0f; // giảm thời gian hồi chiêu

    [System.NonSerialized]
    public Item _baseItem;

    public void Bind(Item item)
    {
        _baseItem = item;
    }


    public RtItem(Item item)
    {
        _baseItem = item;
        _itemID = item._itemID;
        _instanceID = System.Guid.NewGuid().ToString();
        _itemStatus = ItemStatus.UnEquip;
    }

    public void setItemStatus(ItemStatus status)
    {
        _itemStatus = status;
    }

    public string toString()
    {
        return $"{_baseItem._itemName}" +
        $"{_itemID}" +
        $"{_baseItem._itemType}";
    }
}

[System.Serializable]
public class EquipedItem
{
    public RtItem _helmet = null;
    public bool _isHelmet = false;
    public RtItem _armor = null;
    public bool _isArmor = false;
    public RtItem _boots = null;
    public bool _isBoots = false;
    public RtItem _accessory = null;
    public bool _isAccesory = false;
    public RtItem _MeleeWeapon = null;
    public bool _isMelleWeapon = false;
    public RtItem _RangedWeapon = null;
    public bool _isRangedWeapon = false;
    public RtItem _Consumahble = null;
    public bool _isConsumahble = false;

    public void Clear()
    {
        _helmet = null;
        _armor = null;
        _boots = null;
        _accessory = null;
        _MeleeWeapon = null;
        _RangedWeapon = null;
        _Consumahble = null;
    }

    public void equip(RtItem item)
    {
        switch (item._baseItem._itemType)
        {
            case ItemType.Helmet:
                if (_helmet != null) unEquip(item);
                _helmet = item;
                item._itemStatus = ItemStatus.Equip;
                _isHelmet = true;
                break;
            case ItemType.Armor:
                if (_armor != null) unEquip(item);
                _armor = item;
                item._itemStatus = ItemStatus.Equip;
                _isArmor = true;
                break;
            case ItemType.Boots:
                if (_boots != null) unEquip(item);
                _boots = item;
                item._itemStatus = ItemStatus.Equip;
                _isBoots = true;
                break;
            case ItemType.Accessory:
                if (_accessory != null) unEquip(item);
                _accessory = item;
                item._itemStatus = ItemStatus.Equip;
                _isAccesory = true;
                break;
            case ItemType.Weapon:
                if (item._baseItem._weaponType == WeaponType.Melee)
                {
                    if (_MeleeWeapon != null) unEquip(item);
                    _MeleeWeapon = item;
                    _isMelleWeapon = true;
                }
                else if (item._baseItem._weaponType == WeaponType.Ranged)
                {
                    if (_RangedWeapon != null) unEquip(item);
                    _RangedWeapon = item;
                    _isRangedWeapon = true;
                }
                item._itemStatus = ItemStatus.Equip;
                break;
            case ItemType.Consumable:
                if (_Consumahble != null) unEquip(item);
                _Consumahble = item;
                item._itemStatus = ItemStatus.Equip;
                _isConsumahble = true;
                break;
        }
    }

    public void unEquip(RtItem item)
    {
        switch (item._baseItem._itemType)
        {
            case ItemType.Helmet:
                _helmet._itemStatus = ItemStatus.UnEquip;
                _helmet = null;
                _isHelmet = false;
                break;
            case ItemType.Armor:
                _armor._itemStatus = ItemStatus.UnEquip;
                _armor = null;
                _isArmor = false;
                break;
            case ItemType.Boots:
                _boots._itemStatus = ItemStatus.UnEquip;
                _boots = null;
                _isBoots = false;
                break;
            case ItemType.Accessory:
                _accessory._itemStatus = ItemStatus.UnEquip;
                _accessory = null;
                _isAccesory = false;
                break;
            case ItemType.Weapon:
                if (item._baseItem._weaponType == WeaponType.Melee)
                {
                    _MeleeWeapon._itemStatus = ItemStatus.UnEquip;
                    _MeleeWeapon = null;
                    _isMelleWeapon = false;
                }
                else if (item._baseItem._weaponType == WeaponType.Ranged)
                {
                    _RangedWeapon._itemStatus = ItemStatus.UnEquip;
                    _RangedWeapon = null;
                    _isRangedWeapon = false;
                }
                break;
            case ItemType.Consumable:
                _Consumahble._itemStatus = ItemStatus.UnEquip;
                _Consumahble = null;
                _isConsumahble = false;
                break;
        }
    }
}
