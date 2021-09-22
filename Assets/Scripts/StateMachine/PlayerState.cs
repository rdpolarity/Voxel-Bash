using RDPolarity.Controllers;
using UnityEngine;

namespace RDPolarity.StateMachine
{
    public class PlayerState
    {
        protected PlayerController PlayerController;
        protected PlayerStateMachine stateMachine;
        //protected RigidBody playerRigidBody;

        protected float startTime;

        private string animBoolName;

        public PlayerState(PlayerController playerController, PlayerStateMachine stateMachine, string animBoolName)
        {
            this.PlayerController = playerController;
            //this.playerRigidBody = playerRigidBody;
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
        }

        public virtual void Enter()
        {
            DoChecks();
            PlayerController.animator.SetBool("animBoolName", true);
            startTime = Time.time;
            Debug.Log(animBoolName);
        }

        public virtual void Exit()
        {
            PlayerController.animator.SetBool("animBoolName", false);
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
}
