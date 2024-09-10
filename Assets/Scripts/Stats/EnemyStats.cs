using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : Characterstats
{
    private Enemy enemy;
    private ItemDrop myDropSystem;
    public Stat soulsDropAmount;

    [Header("Level details")]
    [SerializeField] private int level = 1;

    [Range(0f, 1f)]
    [SerializeField] private float percentageModifier = .4f;

    public override void DoDamage(Characterstats _targetStats)
    {
        base.DoDamage(_targetStats);
    }

    protected override void Start()
    {
        soulsDropAmount.SetDefaultValue(100);
        ApplyLevelModifier();
        base.Start();

        enemy = GetComponent<Enemy>();
        myDropSystem = GetComponent<ItemDrop>();
    }

    private void ApplyLevelModifier()
    {
        Modifier(strength);
        Modifier(agility);
        Modifier(intelligence);
        Modifier(vitality);

        Modifier(damage);
        Modifier(critChance);
        Modifier(critPower);

        Modifier(maxHealth);
        Modifier(armor);
        Modifier(evasion);
        Modifier(magiceResistance);

        Modifier(fireDamage);
        Modifier(iceDamage);
        Modifier(lightingDamage);

        Modifier(soulsDropAmount);
    }

    private void Modifier(Stat _stat)
    {
        for (int i = 0; i < level; i++)
        {
            float modifier = _stat.GetValue() * percentageModifier;
            _stat.AddModifier(Mathf.RoundToInt(modifier));
        }
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        //enemy.DamageEffect();
    }

    public override void Die()
    {
        base.Die();
        enemy.Die();

        PlayerManager.instance.currency += soulsDropAmount.GetValue();
        myDropSystem.GenerateDrop();

        Destroy(gameObject, 5f);
    }
}
