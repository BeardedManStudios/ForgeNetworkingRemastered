using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity.Menu
{
	public class HostButton : UIButton<IMenuMediator>
	{
		public override IMenuMediator State { get; set; }

		public override void Invoke()
		{
			if (State.EngineFacade.NetworkMediator == null)
			{
				Debug.Log("Hosting");
				var factory = AbstractFactory.Get<INetworkTypeFactory>();
				State.EngineFacade.NetworkMediator = factory.GetNew<INetworkMediator>();
				State.EngineFacade.NetworkMediator.PlayerRepository.onPlayerAddedSubscription += onPlayerAdded;

				State.EngineFacade.NetworkMediator.ChangeEngineProxy(State.EngineFacade);

				// TODO:  Catch exception if port is already being used (will not be caught in this function)
				State.EngineFacade.NetworkMediator.StartServer(ushort.Parse(State.PortInput.Text), State.MaxPlayers);
			}
			else
				Debug.LogError("Already (Hosting | Connecting)");
		}

		private void onPlayerAdded(Players.INetPlayer player)
		{
			Debug.Log($"Player connected: { player.EndPoint }");
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.H))
				Invoke();
		}
	}
}
