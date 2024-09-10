using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;

public class UI_CraftList : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Transform craftSlotParent;         //����Ѱ�ҽ�Ҫɾ����craftSlot�ĸ����
    [SerializeField] private GameObject craftSlotPerfab;        //����craftSlot��Ԥ����

    [SerializeField] private List<ItemData_Equipment> craftEquipment;       //һ����Ҫ����craftList��data������
    //[SerializeField] private List<UI_CraftSlot> craftSlots;     //��ʱ���潫Ҫɾ����craftSlot

    void Start()
    {
        transform.parent.GetChild(0).GetComponent<UI_CraftList>().SetupCraftList();
        SetupDefaultCraftWindow();
        //AssingCraftSlot();
    }
/*    private void AssingCraftSlot()      //��õ�ǰ���е�craftSlot����//��Ҫ����Ϊ�����׼��
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)
        {
            craftSlots.Add(craftSlotParent.GetChild(i).GetComponent<UI_CraftSlot>());
        }
    }*/

    public void SetupCraftList()        //�����б��������е�װ������ʵ����craftslot������CraftList�ĺ���
    {
        for (int i = 0; i < craftSlotParent.childCount; i++)  //ɾ������ԭ����������list���slot
        {
            Destroy(craftSlotParent.GetChild(i).gameObject);
        }
        //craftSlots = new List<UI_CraftSlot>();      //��մ˲ۣ�������һ��װ������װ��

        for (int i = 0; i < craftEquipment.Count; i++)      //������ʵ����craftPartent����
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
