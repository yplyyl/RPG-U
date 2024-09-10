using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;    //ÿ������ʱ��õ��ٶ���
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }    //��ֹ�ڹ�������н���move
    [Header("Move info")]
    public float moveSpeed = 8f;    //�����ٶȣ���xInput��˿����ٶȵĴ�С
    public float jumpForce;
    public float swordReturnImpact;     //��player������swordReturnImpact��Ϊ���˵Ĳ���

    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private float defaultDashSpeed;

    [Header("Dash info")]
    //[SerializeField] private float dashCooldown;
    //private float dashUsageTimer;     //Ϊdash������ȴʱ�䣬��һ��ʱ���ڲ�������ʹ��
    public float dashSpeed;     //����ٶ�
    public float dashDuration;  //����ʱ��
    public float dashDir { get; private set; }

    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }

    #region ����States
    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerAirState airState { get; private set; }
    public PlayerWallSlideState wallSlide { get; private set; }
    public PlayerWallJumpState wallJump { get; private set; }
    public PlayerDashState dashState { get; private set; }

    public PlayerPrimaryAttackState primaryAttack { get; private set; }
    public PlayerCounterAttackState counterAttack { get; private set; }
    public PlayerAimSwordState aimSword { get; private set; }
    public PlayerCatchSwordState catchSword { get; private set; }
    public PlayerBlackholeState blackHole { get; private set; }
    public PlayerDeadState deadState { get; private set; }
    #endregion

    protected override void Awake()
    {
        //Awake��ʼ��State��Ϊ����State������Զ��еĲ�������animBool�����ж��Ƿ���ô˶�������animatoin�����ɣ�
        base.Awake();

        stateMachine = new PlayerStateMachine();
        //ͨ�����캯�����ڹ���ʱ������Ϣ

        idleState = new PlayerIdleState(this, stateMachine, "Idle");
        moveState = new PlayerMoveState(this, stateMachine, "Move");
        jumpState = new PlayerJumpState(this, stateMachine, "Jump");
        airState = new PlayerAirState(this, stateMachine, "Jump");
        dashState = new PlayerDashState(this, stateMachine, "Dash");
        wallSlide = new PlayerWallSlideState(this, stateMachine, "WallSlide");
        wallJump = new PlayerWallJumpState(this, stateMachine, "Jump");

        primaryAttack = new PlayerPrimaryAttackState(this, stateMachine, "Attack");
        counterAttack = new PlayerCounterAttackState(this, stateMachine, "CounterAttack");

        aimSword = new PlayerAimSwordState(this, stateMachine, "AimSword");
        catchSword = new PlayerCatchSwordState(this, stateMachine, "CatchSword");
        blackHole = new PlayerBlackholeState(this, stateMachine, "Jump");

        deadState = new PlayerDeadState(this, stateMachine, "Die");
    }

    protected override void Start()
    {
        base.Start();

        skill = SkillManager.instance;

        stateMachine.Initialize(idleState);

        defaultMoveSpeed = moveSpeed;
        defaultDashSpeed = dashSpeed;
        defaultJumpForce = jumpForce;
    }

    protected override void Update()
    {
        //��mano��update���Զ�ˢ�µ�����û��mano�Ĳ���ʣ�
        //��Ҫ�����updata�е��������ű��еĺ���stateMachine.currentState.update��ʵ�� stateMachine�е�update

        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();     //��������CurrentState��Update����

        CheckForDashInput();

        if (Input.GetKeyDown(KeyCode.F) && skill.crystal.crystalUnlocked)
        {
            skill.crystal.CanUseSkill();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
            Inventory.instance.Useflask();
    }

    public override void SlowEntityBy(float _slowPercentage, float _slowDuration)
    {
        moveSpeed = moveSpeed * (1 - _slowPercentage);
        dashSpeed = dashSpeed * (1 - _slowPercentage);
        jumpForce = jumpForce * (1 - _slowPercentage);
        anim.speed = anim.speed * (1 - _slowPercentage);

        Invoke("ReturnDefaultSpeed", _slowDuration);
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();

        moveSpeed = defaultMoveSpeed;
        dashSpeed = defaultDashSpeed;
        jumpForce = defaultJumpForce;
    }

    public void AssignNewSword(GameObject _newSword)    //���ִ���swordʵ���ĺ���
    {
        sword = _newSword;
    }

    public void CatchTheSword()     //ͨ��player��CatchTheSword����,��������ʧ��˲�����
    {
        stateMachine.ChangeState(catchSword);

        Destroy(sword);
    }

/*    public void ExitBlackholeAbility()
    {
        stateMachine.ChangeState(airState);
    }*/

    public IEnumerator BusyFor(float _seconds)
    {
        //p39 4.��ֹ�ڹ�������н���move,ͨ������busyֵ����ʹ��ĳЩ״̬ʱ��ʹ��ΪbusyΪtrue���������������state
        //IEnumertor���ʾ��ǽ�һ�������ֿ�ִ�У�ֻ������ĳЩ��������ִ����һ�δ��룬�˺�����StartCoroutine����

        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    //�ӵ�ǰ״̬�õ�AnimationTrigger���е��õĺ���
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (IsWallDetected())   //�޸���wallslide����dash��BUG
            return;

        if (skill.dash.dashUnlocked == false)
            return;

        //dashUsageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            //��DashTimer<0 ���ж� �ĳ�DashSkill����ж�
            //dashUsageTimer = dashCooldown;

            dashDir = Input.GetAxisRaw("Horizontal");   //����һ��ֵ����dash�ķ����Ϊ��Ҫ�ķ�������ǳ���

            if (dashDir == 0)   //�����û�п��Ʒ���ʱʹ��Ĭ�ϳ���
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }   //��Dash�л����ó�һ��������ʹ������������¶���ʹ��

    public override void Die()
    {
        base.Die();

        stateMachine.ChangeState(deadState);
    }

    protected override void SetupZeroKnockbackPower()
    {
        knockbackPower = new Vector2(0, 0);
    }
}
