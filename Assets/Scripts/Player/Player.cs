using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Entity
{
    [Header("Attack details")]
    public Vector2[] attackMovement;    //每个攻击时获得的速度组
    public float counterAttackDuration = .2f;

    public bool isBusy { get; private set; }    //防止在攻击间隔中进入move
    [Header("Move info")]
    public float moveSpeed = 8f;    //定义速度，与xInput相乘控制速度的大小
    public float jumpForce;
    public float swordReturnImpact;     //在player里设置swordReturnImpact作为击退的参数

    private float defaultMoveSpeed;
    private float defaultJumpForce;
    private float defaultDashSpeed;

    [Header("Dash info")]
    //[SerializeField] private float dashCooldown;
    //private float dashUsageTimer;     //为dash设置冷却时间，在一定时间内不能连续使用
    public float dashSpeed;     //冲刺速度
    public float dashDuration;  //持续时间
    public float dashDir { get; private set; }

    public SkillManager skill { get; private set; }
    public GameObject sword { get; private set; }

    #region 定义States
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
        //Awake初始化State，为所有State传入各自独有的参数，及animBool，以判断是否调用此动画（与animatoin配合完成）
        base.Awake();

        stateMachine = new PlayerStateMachine();
        //通过构造函数，在构造时传递信息

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
        //在mano中update会自动刷新但其他没有mano的不会故，
        //需要在这个updata中调用其他脚本中的函数stateMachine.currentState.update以实现 stateMachine中的update

        if (Time.timeScale == 0)
            return;

        base.Update();

        stateMachine.currentState.Update();     //反复调用CurrentState的Update函数

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

    public void AssignNewSword(GameObject _newSword)    //保持创造sword实例的函数
    {
        sword = _newSword;
    }

    public void CatchTheSword()     //通过player的CatchTheSword进入,及当剑消失的瞬间进入
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
        //p39 4.防止在攻击间隔中进入move,通过设置busy值，在使用某些状态时，使其为busy为true，抑制其进入其他state
        //IEnumertor本质就是将一个函数分块执行，只有满足某些条件才能执行下一段代码，此函数有StartCoroutine调用

        isBusy = true;

        yield return new WaitForSeconds(_seconds);
        isBusy = false;
    }

    //从当前状态拿到AnimationTrigger进行调用的函数
    public void AnimationTrigger() => stateMachine.currentState.AnimationFinishTrigger();

    private void CheckForDashInput()
    {
        if (IsWallDetected())   //修复在wallslide可以dash的BUG
            return;

        if (skill.dash.dashUnlocked == false)
            return;

        //dashUsageTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftShift) && SkillManager.instance.dash.CanUseSkill())
        {
            //将DashTimer<0 的判断 改成DashSkill里的判断
            //dashUsageTimer = dashCooldown;

            dashDir = Input.GetAxisRaw("Horizontal");   //设置一个值，将dash的方向改为想要的方向而不是朝向

            if (dashDir == 0)   //当玩家没有控制方向时使用默认朝向
                dashDir = facingDir;

            stateMachine.ChangeState(dashState);
        }
    }   //将Dash切换设置成一个函数，使其在所以情况下都能使用

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
