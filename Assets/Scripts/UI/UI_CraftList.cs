using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;         //便于寻找将要删除的craftSlot的父组件
    [SerializeField] private GameObject craftSlotPerfab;        //创建craftSlot的预制体

    [SerializeField] private List<ItemData_Equipment> craftEquipment;       //一个将要导进craftList的data类型组
    //[SerializeField] private List<UI_CraftSlot> craftSlots;     //暂时保存将要删除的craftSlot

    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
        SetupDefaultCraftWindow();
        //AssingCraftSlot();
    }
/*    private void AssingCraftSlot()      //获得当前所有的craftSlot函数//主要用来为清空做准备
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            craftSlots.Add(craftSlotParent.GetChild(i).GetComponent<UI_CraftSlot>());
        }
    }*/

    public void SetupCraftList()        //将所有保存在其中的装备类型实例化craftslot并存入CraftList的函数
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)  //删除所有原来存在于其list里的slot
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }
        //craftSlots = new List<UI_CraftSlot>();      //清空此槽，方便下一次装其他的装备

        for (int i = 0; i < craftEquipment.Count; i++)      //创建的实例往craftPartent里塞
        {
            GameObject newSlot = Instantiate(craftSlotPerfab, craftSlotParent);     
            newSlot.GetComponent<UI_CraftSlot>().SetupCraftSlot(craftEquipment[i]);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SetupCraftList();
    }

    public void SetupDefaultCraftWindow()
    {
        if (craftEquipment[0] != null)
            GetComponentInParent<UI>().craftWindow.SetupCraftWindow(craftEquipment[0]);
    }
}
