using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clone_Skill_Controller是绑在Clone体上的也就是Prefah上的
public class Clone_Skill_Controller : MonoBehaviour
{
    private SpriteRenderer sr;
    private Animator anim;
    [SerializeField] private float colorLoosingSpeed;

    private float cloneTimer;
    [SerializeField] private Transform attackCheck;
    [SerializeField] private float attackCheckRadius = .8f;
    private Transform closestEnemy;

    private bool canDupicateClone;
    private float chanceToDupicate;

    private int facingDir = 1;  //这个是控制位置的，产生的克隆体的位置能在敌人外侧

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0) //设置sr消失
        {
            sr.color = new Color(1, 1, 1, sr.color.a - (colorLoosingSpeed * Time.deltaTime));

            if (sr.color.a <= 0)
                Destroy(gameObject);
        }
    }

    public void SetupClone(Transform _newTransform,float _cloneDuration, bool _canAttack, Vector3 _offset, Transform _closestEnemy, bool _canDupicateClone, float _chanceToDupicate)
    {
        if (_canAttack)
            anim.SetInteger("AttackNumber", Random.Range(1, 3));
        //返回[minInclusive..maxInclusive]（范围包括在内）内的随机浮点值。如果minInclusive大于maxInclusive，则数字会自动交换。

        transform.position = _newTransform.position + _offset;  //这个函数实现了将克隆出来的对象的位置与Dash之前的位置重合的效果
        cloneTimer = _cloneDuration;

        closestEnemy = _closestEnemy;
        canDupicateClone = _canDupicateClone;
        chanceToDupicate = _chanceToDupicate;
        FaceClosestTarget();
    }

    private void AnimationTrigger()
    {
        cloneTimer = -.1f;
    }

    private void AttackTrigger()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(attackCheck.position, attackCheckRadius);

        foreach (var hit in colliders)
        {
            if (hit.GetComponent<Enemy>() != null)
            { 
                hit.GetComponent<Enemy>().Damage();

                //使角色克隆体的攻击有概率产生新的克隆体
                if (canDupicateClone)
                { 
                    if (Random.Range(0, 100) < chanceToDupicate)
                    {
                        SkillManager.instance.clone.CreateClone(hit.transform, new Vector3(.5f * facingDir, 0));
                    }
                }
            }
        }
    }

    private void FaceClosestTarget()
    {
        /*      Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);  //创建一个碰撞器组，保存所有圈所碰到的碰撞器

                float closestDistance = Mathf.Infinity;

                foreach(var hit in colliders)
                {
                    if (hit.GetComponent<Enemy>() != null)
                    {
                        float distanceToEnemy = Vector2.Distance(transform.position, hit.transform.position);

                        if (distanceToEnemy < closestDistance)
                        {
                            closestDistance = distanceToEnemy;
                            closestEnemy = hit.transform;
                        }
                    }
                }
        */

        if (closestEnemy != null)
        {
            //敌人在左面，转一圈
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
