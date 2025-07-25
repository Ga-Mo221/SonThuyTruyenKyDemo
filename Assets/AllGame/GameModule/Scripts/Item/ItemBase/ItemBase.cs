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
    InShop
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
    public float _itemMagicResist;
    public float _itemCooldownReduction;
    public int _itemPrice;
    public int _itemSellPrice;
    public float _itemCooldownTime;
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
    public float _bonusMagicResist = 0f;
    public float _bonusCooldownReduction = 0f; // giảm thời gian hồi chiêu
    public float _bonusCooldownTime = 0f; // thời gian hồi chiêu

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
    public RtItem _armor = null;
    public RtItem _boots = null;
    public RtItem _accessory = null;
    public RtItem _MeleeWeapon = null;
    public RtItem _RangedWeapon = null;

    public void Clear()
    {
        _helmet = null;
        _armor = null;
        _boots = null;
        _accessory = null;
        _MeleeWeapon = null;
        _RangedWeapon = null;
    }

    public void equip(RtItem item)
    {
        switch (item._baseItem._itemType)
        {
            case ItemType.Helmet:
                if (_helmet != null) unEquip(item);
                _helmet = item;
                item._itemStatus = ItemStatus.Equip;
                break;
            case ItemType.Armor:
                if (_armor != null) unEquip(item);
                _armor = item;
                item._itemStatus = ItemStatus.Equip;
                break;
            case ItemType.Boots:
                if (_boots != null) unEquip(item);
                _boots = item;
                item._itemStatus = ItemStatus.Equip;
                break;
            case ItemType.Accessory:
                if (_accessory != null) unEquip(item);
                _accessory = item;
                item._itemStatus = ItemStatus.Equip;
                break;
            case ItemType.Weapon:
                if (item._baseItem._weaponType == WeaponType.Melee)
                {
                    if (_MeleeWeapon != null) unEquip(item);
                    _MeleeWeapon = item;
                }
                else if (item._baseItem._weaponType == WeaponType.Ranged)
                {
                    if (_RangedWeapon != null) unEquip(item);
                    _RangedWeapon = item;
                }
                item._itemStatus = ItemStatus.Equip;
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
                break;
            case ItemType.Armor:
                _armor._itemStatus = ItemStatus.UnEquip;
                _armor = null;
                break;
            case ItemType.Boots:
                _boots._itemStatus = ItemStatus.UnEquip;
                _boots = null;
                break;
            case ItemType.Accessory:
                _accessory._itemStatus = ItemStatus.UnEquip;
                _accessory = null;
                break;
            case ItemType.Weapon:
                if (item._baseItem._weaponType == WeaponType.Melee)
                {
                    _MeleeWeapon._itemStatus = ItemStatus.UnEquip;
                    _MeleeWeapon = null;
                }
                else if (item._baseItem._weaponType == WeaponType.Ranged)
                {
                    _RangedWeapon._itemStatus = ItemStatus.UnEquip;
                    _RangedWeapon = null;
                }
                break;
        }
    }
}
