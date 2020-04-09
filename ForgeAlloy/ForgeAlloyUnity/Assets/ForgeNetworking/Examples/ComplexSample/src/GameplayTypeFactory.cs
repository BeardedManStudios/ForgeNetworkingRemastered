using Forge.CharacterControllers;
using Puzzle.SemaphoreMarkers;

namespace Forge.Factory
{
	public class GameplayTypeFactory : TypeFactory, IGameplayTypeFactory
	{
		public override void PrimeRegistry()
		{
			// Input control
			Register<IMovementInputDevice, MovementKeyboardInput>();
			Register<IVelocityManager, VelocityManager>();
			Register<ICameraInputDevice, CameraMouseInput>();
			Register<IRotationManager, RotationManager>();
			Register<INetworkTransformManager, NetworkTransformManager>();
			Register<IFriendPreviewManager, FriendPreviewManager>();

			// Visual semaphore marker system
			Register<IVisualSemaphoreMarker, SemaphoreMarker>();
		}
	}
}
