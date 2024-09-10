using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData data;       //����ʵ��ʵ��Item����
    public int stackSize;       //��¼��ͬItem������

    public InventoryItem(ItemData _newitemData)     //����ʱ�ʹ���Ҫ�����Item
    {
        data = _newitemData;
        AddStack();
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
