using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    private Player player => GetComponentInParent<Player>();    //��ø�����ϵ�ʵ�ʴ��ڵ�Player���

    private void AnimationTrigger()
    {
        player.AnimationTrigger();
    }

    private void AttackTrigger()
    {
        AudioManager.instance.PlaySfx(2, null);

        //����һ����ײ���飬��������Ȧ����������ײ��
        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.attackCheck.position, player.attackCheckRadius);

        foreach(var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                EnemyStats _target = hit.GetComponent<EnemyStats>();

                if(_target!=null)
                    player.stats.DoDamage(_target);     //�ṩһ�����Դ�player���õ������˺�������ڣ�ͨ���������˵��˵����

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
