using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player drop")]
    [SerializeField] private float chanceToLoseItem;
    [SerializeField] private float chanceToLoseMaterial;

    public override void GenerateDrop()
    {
        Inventory inventory = Inventory.instance;

        //List<InventoryItem> currentEquipment = inventory.GetEquipmentList();
        List<InventoryItem> itemsToUnequip = new List<InventoryItem>();
        List<InventoryItem> materialsToLose = new List<InventoryItem>();

        foreach (InventoryItem item in inventory.GetEquipmentList())
        {
            if (Random.Range(0, 100) <= chanceToLoseItem)
            {
                DropItem(item.data);
                itemsToUnequip.Add(item);
            }
        }

        for (int i = 0; i < itemsToUnequip.Count; i++)
        {
            inventory.UnequipItem(itemsToUnequip[i].data as ItemData_Equipment);
        }

        foreach (InventoryItem item in inventory.GetStashList())
        {
            if (Random.Range(0, 100) <= chanceToLoseMaterial)
            {
                DropItem(item.data);
                materialsToLose.Add(item);
            }
        }

        for (int i = 0; i < materialsToLose.Count; i++)
        {
            inventory.RemoveItem(materialsToLose[i].data);
        }
    }
}
