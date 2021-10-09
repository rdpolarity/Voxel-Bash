using RDPolarity.Controllers;

namespace RDPolarity.StateMachine
{
    public class PlayerFallingState : PlayerState
    {

        public PlayerFallingState(PlayerController playerController, PlayerStateMachine stateMachine, string animBoolName) : base(playerController, stateMachine, animBoolName)
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
            if(PlayerController.checkDashing())
            {
                stateMachine.ChangeState(PlayerController.DashState);
            }
            else if(PlayerController.checkGrounded())
            {
                stateMachine.ChangeState(PlayerController.MoveState); // add landing state?
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
