using RDPolarity.Controllers;

namespace RDPolarity.StateMachine
{
    public class PlayerGroundedState : PlayerState
    {
        public PlayerGroundedState(PlayerController playerController, PlayerStateMachine stateMachine, string animBoolName) : base(playerController, stateMachine, animBoolName)
        {

        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();
            if (PlayerController.checkDashing())
            {
                stateMachine.ChangeState(PlayerController.DashState);
            }
            else if (PlayerController.checkGrounded() == false)
            {
                stateMachine.ChangeState(PlayerController.FallState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public override void DoChecks()
        {
            base.DoChecks();
        }
    }
}
