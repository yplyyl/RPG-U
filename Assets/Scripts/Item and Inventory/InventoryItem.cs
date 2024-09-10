using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData data;       //保存实打实的Item数据
    public int stackSize;       //记录相同Item的数量

    public InventoryItem(ItemData _newitemData)     //创建时就传入要保存的Item
    {
        data = _newitemData;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
