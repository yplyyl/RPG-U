using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();    //获得父组件上的实际存在的Player组件

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySfx(2, null);

        //创建一个碰撞器组，保存所有圈所碰到的碰撞器
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if(_target!=null)
                    player.stats.DoDamage(_target);     //提供一个可以从player调用敌人受伤函数的入口，通过传入受伤敌人的组件

                //hit.GetComponent<Enemy>().Damage();
                //hit.GetComponent<Characterstats>().TakeDamage(player.stats.damage.GetValue());
                //Debug.Log(player.stats.damage.GetValue());

                ItemData_Equipment weaponDate = Inventory.instance.GetEquipment(EquipmentType.Weapon);

                if (weaponDate != null)
                    weaponDate.Effect(_target.transform);

            }
        }
    }

    private void ThrowSword()
    {
        SkillManager.instance.sword.CreatSword();
    }
}
