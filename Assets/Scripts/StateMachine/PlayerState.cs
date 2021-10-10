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

        private Animator animator;
        private string animBoolName;

        public PlayerState(PlayerController playerController, PlayerStateMachine stateMachine, string animBoolName)
        {
            this.PlayerController = playerController;
            this.stateMachine = stateMachine;
            this.animBoolName = animBoolName;
        }

        public virtual void Init()
        {
            animator = PlayerController.animator;
        }

        public virtual void Enter()
        {
            DoChecks();
            if(animator != null)
            {
                animator.SetBool(animBoolName, true);
            }
            else
            {
                animator = PlayerController.animator;
                animator.SetBool(animBoolName, true);
            }
            startTime = Time.time;
            Debug.Log(animBoolName);
        }

        public virtual void Exit()
        {
            if (animator != null)
            {
                animator.SetBool(animBoolName, false);
            }
            else
            {
                animator = PlayerController.animator;
                animator.SetBool(animBoolName, false);
            }
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

        public string CheckState()
        {
            return animBoolName;
        }
        public void SetAnim(Animator aAnimator)
        {
            animator = aAnimator;
        }
    }
}
