using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;      //设置可能会出现的材料数量
    [SerializeField] private ItemData[] possibleDrop;

    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;

    public virtual void GenerateDrop()      //随机生成物品函数
    {
        for (int i = 0; i < possibleDrop.Length; i++)       //一个判断可能出现的装备
        {
            if (Random.Range(0,100)<= possibleDrop[i].dropChance)
            {
                dropList.Add(possibleDrop[i]);
            }
        }

        for (int i = 0; i < possibleItemDrop; i++)      //另一个生成可能出现的装备
        {

            if (possibleItemDrop <= dropList.Count)
            {
                ItemData randItem = dropList[Random.Range(0, dropList.Count - 1)];
                dropList.Remove(randItem);
                DropItem(randItem);
            }
        }
    }

    protected void DropItem(ItemData _itemData)    //创建实例函数
    {
        GameObject newDrop = Instantiate(dropPrefab, transform.position, Quaternion.identity);

        Vector2 randomVelocity = new Vector2(Random.Range(-5, 5), Random.Range(15, 20));

        newDrop.GetComponent<ItemObject>().SetupItem(_itemData, randomVelocity);
    }
}
