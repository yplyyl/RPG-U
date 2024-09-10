using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal_Skill_Controller : MonoBehaviour
{
    private Player player;
    private Animator anim => GetComponent<Animator>();
    private CircleCollider2D cd => GetComponent<CircleCollider2D>();

    private float crystalExistTimer;

    private bool canMove;
    private bool canExplode;
    private float moveSpeed;

    private bool canGrow;
    private float growSpeed = 5;

    private Transform closestTarget;
    [SerializeField] private LayerMask whatIsEnemy;

    public void SetupCrystal(float _crystalDuration, bool _canExplode, bool _canMove, float _moveSpeed, Transform _closestTarget, Player _player)
    {
        player = _player;
        crystalExistTimer = _crystalDuration;
        canExplode = _canExplode;
        canMove = _canMove;
        moveSpeed = _moveSpeed;
        closestTarget = _closestTarget;
    }

    public void ChooseRandomEnemy()
    {
        float radius = SkillManager.instance.blackhole.GetBlackholeRadius();

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius, whatIsEnemy);

        if (colliders.Length > 0)
            closestTarget = colliders[Random.Range(0, colliders.Length)].transform;
    }

    public void Update()
    {
        crystalExistTimer -= Time.deltaTime;
        if (crystalExistTimer < 0)
        {
            FinishCrystal();
        }

        //可以运动就靠近敌人后爆炸，范围小于1时爆炸，并且爆炸时不能移动
        if (canMove)
        {
            if (closestTarget == null)
                return;

            //修复攻击范围内没有敌人会报错的bug
            if (closestTarget != null)
            {            
                transform.position = Vector2.MoveTowards(transform.position, closestTarget.position, moveSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, closestTarget.position) < 1)
                {
                    FinishCrystal();
                    canMove = false;
                }
            }
            else
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + new Vector3(5, 0, 0), moveSpeed * Time.deltaTime);
            }
        }

        //爆炸瞬间变大
        if (canGrow)
            transform.localScale = Vector2.Lerp(transform.localScale, new Vector2(3, 3), growSpeed * Time.deltaTime);
    }

    //爆炸造成伤害
    private void AnimationExplodeEvent()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, cd.radius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            {
                //hit.GetComponent<Enemy>().DamageEffect();
                hit.GetComponent<Entity>().SetupKnockbackDir(transform);
                player.stats.DoMagicDamage(hit.GetComponent<Characterstats>());

                ItemData_Equipment equipedAmulet = Inventory.instance.GetEquipment(EquipmentType.Amulet);

                if (equipedAmulet != null)
                    equipedAmulet.Effect(hit.transform);
            }
        }
    }

    public void FinishCrystal()
    {
        if (canExplode)
        {
            canGrow = true;
            anim.SetTrigger("Explode");
        }
        else
            SelfDestory();
    }

    public void SelfDestory() => Destroy(gameObject);
}
