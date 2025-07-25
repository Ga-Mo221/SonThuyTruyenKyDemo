using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventorySaveData
{
    public List<RtItem> rtItems = new List<RtItem>();
    public EquipedItem equipedItem = new EquipedItem();
}
