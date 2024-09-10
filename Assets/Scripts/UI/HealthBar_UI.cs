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

    private void UpdataHealthUI()   //����Ѫ�����������˺�����Event����
    {
        slider.maxValue = myStats.GetMaxHealthValue();
        slider.value = myStats.currentHealth;
    }

    private void FlipUI()   //��UI�����Ž�ɫ��ת
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
