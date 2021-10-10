using RDPolarity.Controllers;

namespace RDPolarity.StateMachine
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerController playerController, PlayerStateMachine stateMachine, string animBoolName) : base(playerController, stateMachine, animBoolName)
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

            if(PlayerController.checkMoving())
            {
                stateMachine.ChangeState(PlayerController.MoveState);
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
