using Godot;

namespace RushEnemyStates
{
	public class AttackState : RushEnemyState
	{
		private float animLength = float.MaxValue;
		private float timer = 0f;
        private bool startAttack = false;

		public AttackState(RushingEnemy enemy) 
		{
			this.enemy = enemy;
			enemy.Velocity = new Vector2(0, enemy.Velocity.y);	
		}

		public override void _Process(float delta)
		{
            if (!startAttack && timer > 0.049f) {
                enemy.PlayAnimation("Attack");
			    animLength = enemy.AnimPlayer.CurrentAnimationLength;
                startAttack = true;
                timer = 0f;
            }

			if (timer > 0.049f && timer > animLength * 0.7f) {
				enemy.Melee += 1;
				enemy.Attack();
			}

			if (!enemy.AnimPlayer.IsPlaying()) {
				Goblin target = enemy.PlayerDetect();
				if (target != null) {
					ExitState(new ChaseState(enemy, target));
				} else {
					ExitState(new NormalState(enemy));
				}
				return;
			}
		}

		public override void _PhysicsProcess(float delta)
		{
			timer += delta;
		}
	}
}
