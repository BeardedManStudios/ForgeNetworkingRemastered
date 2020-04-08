using System.Collections.Generic;
using UnityEngine;

namespace Forge.CharacterControllers
{
	public class VelocityManager : IVelocityManager
	{
		private const float JUMP_Y_THRESHOLD = 0.01f;

		private Rigidbody _rigidbody = null;

		public float Speed { get; set; } = 5.0f;
		public float JumpSpeed { get; set; } = 7.5f;
		public float MaxIncline { get; set; } = 0.5f;
		public float MaxStepHeight { get; set; } = 0.25f;
		public List<Collision> CollisionDeltas { get; set; } = new List<Collision>();

		public void ProcessJump()
		{
			if (CheckStabilityFromVelocity() || CheckStabilityFromCollisions())
				_rigidbody.velocity = _rigidbody.velocity + Vector3.up * JumpSpeed;
		}

		private bool CheckStabilityFromVelocity()
		{
			return Mathf.Abs(_rigidbody.velocity.y) <= JUMP_Y_THRESHOLD;
		}

		private bool CheckStabilityFromCollisions()
		{
			// TODO:  This needs to be figureed out
			return false;
		}

		public void ProcessMovement(Vector3 direction)
		{
			Vector3 dir = direction * Speed;
			Vector3 vel = new Vector3(dir.x, _rigidbody.velocity.y, dir.z);
			foreach (var col in CollisionDeltas)
			{
				foreach (var c in col.contacts)
				{
					if (ContactHigherThanStep(c))
					{
						if (NormalComponentIsBeyondThreshold(c.normal.x, vel.x))
							vel.x = _rigidbody.velocity.x;
						if (NormalComponentIsBeyondThreshold(c.normal.z, vel.z))
							vel.z = _rigidbody.velocity.z;
					}
				}
			}
			_rigidbody.velocity = vel;
		}

		private bool ContactHigherThanStep(ContactPoint c)
		{
			Vector3 delta = c.point - c.thisCollider.transform.position;
			return delta.y <= -MaxStepHeight && delta.y >= -c.thisCollider.bounds.extents.y;
		}

		private bool NormalComponentIsBeyondThreshold(float scale, float velocityScale)
		{
			return Mathf.Abs(scale) > MaxIncline
				&& ((scale > 0.0f && velocityScale < 0.0f)
				|| (scale < 0.0f && velocityScale > 0.0f));
		}

		public void Initialize(Transform transform)
		{
			if (_rigidbody != null)
				throw new System.Exception("TODO:  This is already initialized");
			_rigidbody = transform.GetComponent<Rigidbody>();
			if (_rigidbody == null)
				throw new System.Exception("TODO:  This should have a rigidbody");
		}
	}
}
