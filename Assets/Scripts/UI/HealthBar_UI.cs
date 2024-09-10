using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar_UI : MonoBehaviour
{
    private Entity entity;
    private RectTransform mytransform;
    private Characterstats myStats;
    private Slider slider;

    private void Start()
    {
        mytransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<Entity>();
        slider = GetComponentInChildren<Slider>();
        myStats = GetComponentInParent<Characterstats>();

        UpdataHealthUI();

        entity.onFlipped += FlipUI;
        myStats.onHealthChanged += UpdataHealthUI;
        //Debug.Log("health bar ui called");
    }
    private void Update()
    {
        
    }

    private void UpdataHealthUI()   //更新血量条函数，此函数由Event触发
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void FlipUI()   //让UI不随着角色翻转
    {
        //Debug.Log("entity is flipped");
        mytransform.Rotate(0, 180, 0);
    }

    private void OnDisable()
    {
        entity.onFlipped -= FlipUI;
        myStats.onHealthChanged -= UpdataHealthUI;
    }
}
