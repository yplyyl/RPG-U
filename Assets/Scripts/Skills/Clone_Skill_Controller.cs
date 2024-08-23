using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Clone_Skill_Controller�ǰ���Clone���ϵ�Ҳ����Prefah�ϵ�
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

    private int facingDir = 1;  //����ǿ���λ�õģ������Ŀ�¡���λ�����ڵ������

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        cloneTimer -= Time.deltaTime;

        if (cloneTimer < 0) //����sr��ʧ
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
        //����[minInclusive..maxInclusive]����Χ�������ڣ��ڵ��������ֵ�����minInclusive����maxInclusive�������ֻ��Զ�������

        transform.position = _newTransform.position + _offset;  //�������ʵ���˽���¡�����Ķ����λ����Dash֮ǰ��λ���غϵ�Ч��
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

                //ʹ��ɫ��¡��Ĺ����и��ʲ����µĿ�¡��
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
        /*      Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25);  //����һ����ײ���飬��������Ȧ����������ײ��

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
            //���������棬תһȦ
            if (transform.position.x > closestEnemy.position.x)
            {
                facingDir = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
}
