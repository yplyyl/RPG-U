using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;


public class Entity : MonoBehaviour
{
    #region Components ����Unity���
    public Animator anim { get; private set; }      //����õ����ϵ�animator�Ŀ���Ȩ
    public Rigidbody2D rb { get; private set; }     //����õ����ϵ�Rigidbody2D�������Ȩ
    public EntityFX fx { get; private set; }
    public SpriteRenderer sr { get; private set; }

    public Characterstats stats { get; private set; }
    public CapsuleCollider2D cd { get; private set; }
    #endregion

    [Header("Knockback info")]
    [SerializeField] protected Vector2 knockbackPower;  //���������ٶ�
    [SerializeField] protected float knockbackDuration;     //�������ʱ��
    protected bool isKnocked;   //��ֵͨ����סSetVelocity�����ķ�ʽ������ֹ��һ����ɫ������ʱ�����Ҷ������

    [Header("Collision info")]
    public Transform attackCheck;       //transform�࣬�����ʱ�����λ��,�������ƹ�������λ��
    public float attackCheckRadius;     //���뾶
    [SerializeField] protected Transform groundCheck;   //transform�࣬�����ʱ�����λ��,���������λ�������λ��
    [SerializeField] protected float groundCheckDistance;
    [SerializeField] protected Transform wallCheck;
    [SerializeField] protected float wallCheckDistance;
    [SerializeField] protected LayerMask whatIsGround;

    public int knockbackDir { get; private set; }
    public int facingDir { get; private set; } = 1;     //�ж��Ƿ���
    protected bool facingRight = true;

    public System.Action onFlipped;     //������д������ֻ�ǽ��������������������ǵĺ���

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();      //�õ��Լ���������ϵ�animator�Ŀ���Ȩ
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        fx = GetComponent<EntityFX>();
        stats = GetComponent<Characterstats>();
        cd = GetComponent<CapsuleCollider2D>();
    }

    protected virtual void Update()
    {
        
    }

    public virtual void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {

    }

    protected virtual void ReturnDefaultSpeed()
    {
        anim.speed = 1;
    }

    public virtual void DamageImpact()
    {
        //fx.StartCoroutine("FlashFX");
        StartCoroutine("HitKnockback");
        //Debug.Log(gameObject.name + "was damaged");
    }

    public virtual void SetupKnockbackDir(Transform _damageDirection)
    {
        if (_damageDirection.position.x > transform.position.x)
            knockbackDir = -1;
        else if (_damageDirection.position.x < transform.position.x)
            knockbackDir = 1;
    }

    public void SetupKnockbackPower(Vector2 _knockbackpower) => knockbackPower = _knockbackpower;

    public virtual IEnumerator HitKnockback()
    {
        isKnocked = true;

        rb.velocity = new Vector2(knockbackPower.x * knockbackDir, knockbackPower.y);

        yield return new WaitForSeconds(knockbackDuration);
        isKnocked = false;
        SetupZeroKnockbackPower();
    }

    protected virtual void SetupZeroKnockbackPower()
    {

    }

    #region Velocity
    public void SetZeroVelocity()
    {
        if (isKnocked)
            return;
         
        rb.velocity = new Vector2(0, 0);
    }
    public void SetVelocity(float _xVelocity, float _yVelocity)
    {
        if (isKnocked)
            return;

        rb.velocity = new Vector2(_xVelocity, _yVelocity);
        FlipController(_xVelocity);
    }
    #endregion

    #region Collision
    public bool IsGroundDetected() => Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, whatIsGround);
    public bool IsWallDetected() => Physics2D.Raycast(wallCheck.position, Vector2.right * facingDir, wallCheckDistance, whatIsGround);

    protected virtual void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector3(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
        Gizmos.DrawWireSphere(attackCheck.position, attackCheckRadius); 
    }
    #endregion

    #region Flip
    public virtual void Flip()
    {
        facingDir = facingDir * -1;
        facingRight = !facingRight;
        transform.Rotate(0, 180, 0);

        if(onFlipped!=null)
            onFlipped();
    }

    public virtual void FlipController(float _x)
    {
        if (_x > 0 && !facingRight)
            Flip();
        else if (_x < 0 && facingRight)
            Flip();
    }
    #endregion

    public virtual void Die()
    {

    }
}
