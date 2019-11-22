using Forge.Factory;
using Forge.Networking.Unity.UI;
using UnityEngine;

namespace Forge.Networking.Unity.Menu
{
	public class HostButton : UIButton<IMenuMediator>
	{
		public override IMenuMediator Mediator { get; set; }

		public override void Invoke()
		{
			if (Mediator.EngineFacade.NetworkMediator == null)
			{
				Debug.Log("Hosting");
				var factory = AbstractFactory.Get<INetworkTypeFactory>();
				Mediator.EngineFacade.NetworkMediator = factory.GetNew<INetworkMediator>();

				Mediator.EngineFacade.NetworkMediator.ChangeEngineProxy(Mediator.EngineFacade);

				//TODO: Catch exception if port is already being used (will not be caught in this function)
				Mediator.EngineFacade.NetworkMediator.StartServer(ushort.Parse(Mediator.PortInput.Text), Mediator.MaxPlayers);
			}
			else
				Debug.LogError("Already (Hosting | Connecting)");
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.H))
				Invoke();
		}
	}
}
