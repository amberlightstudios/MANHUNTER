using Godot;

namespace RushEnemyStates
{
	public class DeathState : RushEnemyState
	{
		public DeathState(RushingEnemy enemy) 
		{
			this.enemy = enemy;
			enemy.Velocity = new Vector2(0, enemy.Velocity.y);
			enemy.PlayAnimation("Death");
			enemy.BloodGenerator.GenerateBlood(15);
			enemy.GenerateBlood += 15;
			enemy.SetCollisionLayerBit(2, false);
		}

		public override void _Process(float delta)
		{

		}

		public override void _PhysicsProcess(float delta)
		{
			
		}

		public override void ExitState(RushEnemyState newState)
		{
			return;
		}
	}
}
