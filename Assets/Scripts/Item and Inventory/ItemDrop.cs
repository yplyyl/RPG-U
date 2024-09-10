using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;      //���ÿ��ܻ���ֵĲ�������
    [SerializeField] private ItemData[] possibleDrop;

    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()      //���������Ʒ����
    {
        for (int i = 0; i < possibleDrop.Length; i++)       //һ���жϿ��ܳ��ֵ�װ��
        {
            if (Random.Range(0,100)<= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        for (int i = 0; i < possibleItemDrop; i++)      //��һ�����ɿ��ܳ��ֵ�װ��
        {

            if (possibleItemDrop <= dropList.Count)
            {
                ItemData randItem = dropList[Random.Range(0, dropList.Count - 1)];
                dropList.Remove(randItem);
                DropItem(randItem);
            }
        }
    }

    protected void DropItem(ItemData _itemData)    //����ʵ������
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
