using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{
    protected override void Start()
    {
        base.Start();

    }

    public void SetupCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null)
            return;

        item.data = _data;
        itemImage.sprite = _data.icon;
        itemText.text = _data.itemName;

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * .8f;
        else
            itemText.fontSize = 28;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        //ItemData_Equipment craftdata = item.data as ItemData_Equipment;
        //Inventory.instance.CanCraft(craftdata, craftdata.craftingMaterial);

        ui.craftWindow.SetupCraftWindow(item.data as ItemData_Equipment);
    }
}
