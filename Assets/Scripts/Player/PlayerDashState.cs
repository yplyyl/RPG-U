using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerState
{
    public PlayerDashState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName) : base(_player, _stateMachine, _animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //SkillManager.instance.clone.CreateClone();
        //player.skill.clone.CreateClone(player.transform,Vector3.zero);
        player.skill.clone.CreateCloneOnDashStart();

        stateTimer = player.dashDuration;
    }

    public override void Exit()
    {
        base.Exit();

        player.skill.clone.CreateCloneOnDashOver();
        player.SetVelocity(0, rb.velocity.y);
    }

    public override void Update()
    {
        base.Update();

        if (!player.IsGroundDetected() && player.IsWallDetected())
            stateMachine.ChangeState(player.wallSlide);

        player.SetVelocity(player.dashDir * player.dashSpeed, 0);

        if (stateTimer < 0)
            stateMachine.ChangeState(player.idleState);
    }
}
