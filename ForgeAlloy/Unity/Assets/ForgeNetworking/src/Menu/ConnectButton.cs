using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity.Menu
{
	public class ConnectButton : UIButton<IMenuMediator>
	{
		public override IMenuMediator Mediator { get; set; }

		public override void Invoke()
		{
			if (Mediator.EngineFacade.NetworkMediator != null)
			{
				Debug.LogError("Already (Hosting | Connecting)");
				return;
			}

			if (!string.IsNullOrEmpty(Mediator.AddressInput.Text) && ushort.TryParse(Mediator.PortInput.Text, out ushort port))
			{
				var factory = AbstractFactory.Get<INetworkTypeFactory>();
				Mediator.EngineFacade.NetworkMediator = factory.GetNew<INetworkMediator>();

				Mediator.EngineFacade.NetworkMediator.ChangeEngineProxy(Mediator.EngineFacade);

				//TODO: Catch exception if connection fails
				Mediator.EngineFacade.NetworkMediator.StartClient(Mediator.AddressInput.Text, port);
			}
			else
				Debug.LogError($"{ (string.IsNullOrEmpty(Mediator.AddressInput.Text) ? "Host Address Not Provided" : "Port Invalid") }");
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
				Invoke();
		}
	}
}
