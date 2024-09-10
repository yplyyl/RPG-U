using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    //private SpriteRenderer sr;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Rigidbody2D rb;
    //[SerializeField] private Vector2 velocity;

    private void SetupVisuals()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item Object - " + itemData.itemName;
    }

/*    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            rb.velocity = velocity;
    }*/

    public void SetupItem(ItemData _itemData, Vector2 _velocity)    //设置实例函数
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisuals();
    }

    public void PickupItem()    //拾取函数
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            return;
        }

        AudioManager.instance.PlaySfx(18, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }

/*    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        sr.sprite = itemData.icon;
    }*/
}
