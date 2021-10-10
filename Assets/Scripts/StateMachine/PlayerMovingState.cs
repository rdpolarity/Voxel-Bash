using RDPolarity.Controllers;

namespace RDPolarity.StateMachine
{
    public class PlayerMovingState : PlayerGroundedState
    {
        public PlayerMovingState(PlayerController playerController, PlayerStateMachine stateMachine, string animBoolName) : base(playerController, stateMachine, animBoolName)
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

            if (PlayerController.checkMoving() == false)
            {
                stateMachine.ChangeState(PlayerController.IdleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            DoChecks();
        }

        public override void DoChecks()
        {
            base.DoChecks();
        }
    }
}
