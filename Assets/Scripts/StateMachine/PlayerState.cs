using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerController player;
    protected PlayerStateMachine stateMachine;
    //protected RigidBody playerRigidBody;

    protected float startTime;

    private string animBoolName;

    public PlayerState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
    {
        this.player = player;
        //this.playerRigidBody = playerRigidBody;
        this.stateMachine = stateMachine;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        DoChecks();
        player.animator.SetBool("animBoolName", true);
        startTime = Time.time;
        Debug.Log(animBoolName);
    }

    public virtual void Exit()
    {
        player.animator.SetBool("animBoolName", false);
    }

    public virtual void LogicUpdate()
    {

    }

    public virtual void PhysicsUpdate()
    {
        DoChecks();
    }

    public virtual void DoChecks()
    {

    }
}
