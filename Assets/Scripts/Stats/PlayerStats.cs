using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Characterstats
{
    private Player player;

    public override void DoDamage(Characterstats _targetStats)
    {
        base.DoDamage(_targetStats);
    }

    protected override void Start()
    {
        base.Start();

        player = GetComponent<Player>();
    }

    public override void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);

        //player.DamageEffect();
    }

    public override void Die()
    {
        base.Die();
        player.Die();

        GameManager.instance.lostCurrencyAmount = PlayerManager.instance.currency;
        PlayerManager.instance.currency = 0;

        GetComponent<PlayerItemDrop>()?.GenerateDrop();
    }

    protected override void DecreaseHealthBy(int _damage)
    {
        base.DecreaseHealthBy(_damage);

        if (_damage > GetMaxHealthValue() * .3f)
        {
            player.SetupKnockbackPower(new Vector2(10, 6));

            int random = Random.Range(34, 35);
            AudioManager.instance.PlaySfx(random, null);
            Debug.Log("high damage");
        }

        ItemData_Equipment currentArmor = Inventory.instance.GetEquipment(EquipmentType.Armor);

        if (currentArmor != null)
            currentArmor.Effect(player.transform);
    }

    public override void OnEvasion()
    {
        Debug.Log("player avoid attack");
        player.skill.dodge.CreateMirageOnDodge();
    }

    public void CloneDamage(Characterstats _targetStats, float _multiplier)
    {
        if (TargetCanAvoidAttack(_targetStats))  //…Ë÷√…¡±‹
            return;

        int totalDamage = damage.GetValue() + strength.GetValue();

        if (_multiplier > 0)
            totalDamage = Mathf.RoundToInt(totalDamage * _multiplier);

        if (CanCrit())      //±¨…À…Ë÷√
        {
            totalDamage = CalculateCriticalDamage(totalDamage);
            //Debug.Log("toatl Crit damage " + totalDamage);
        }

        totalDamage = CheckTargetArmor(_targetStats, totalDamage);      //…Ë÷√∑¿”˘
        _targetStats.TakeDamage(totalDamage);

        DoMagicDamage(_targetStats); //remove if 
    }
}
