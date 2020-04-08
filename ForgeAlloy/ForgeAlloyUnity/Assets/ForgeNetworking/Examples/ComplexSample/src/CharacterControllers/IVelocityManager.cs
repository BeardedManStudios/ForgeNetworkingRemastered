using System.Collections.Generic;
using UnityEngine;

namespace Forge.CharacterControllers
{
	public interface IVelocityManager
	{
		float Speed { get; set; }
		float JumpSpeed { get; set; }
		float MaxIncline { get; set; }
		float MaxStepHeight { get; set; }
		List<Collision> CollisionDeltas { get; set; }
		void Initialize(Transform transform);
		void ProcessMovement(Vector3 direction);
		void ProcessJump();
	}
}