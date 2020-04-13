using System;
using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity.Menu
{
	public class ConnectButton : UIButton<IMenuMediator>
	{
		public override IMenuMediator State { get; set; }

		public override void Invoke()
		{
			if (State.EngineFacade.NetworkMediator != null)
			{
				State.EngineFacade.Logger.LogError("Already (Hosting | Connecting)");
				return;
			}

			if (!string.IsNullOrEmpty(State.AddressInput.Text) && ushort.TryParse(State.PortInput.Text, out ushort port))
			{
				var factory = AbstractFactory.Get<INetworkTypeFactory>();
				State.EngineFacade.NetworkMediator = factory.GetNew<INetworkMediator>();

				State.EngineFacade.NetworkMediator.ChangeEngineProxy(State.EngineFacade);

				try
				{
					State.EngineFacade.NetworkMediator.StartClient(State.AddressInput.Text, port);
				}
				catch (Exception ex)
				{
					State.EngineFacade.Logger.LogException(ex);
				}
			}
			else
				State.EngineFacade.Logger.LogError($"{ (string.IsNullOrEmpty(State.AddressInput.Text) ? "Host Address Not Provided" : "Port Invalid") }");
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.C))
				Invoke();
		}
	}
}
