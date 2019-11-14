using Forge;
using Forge.Factory;
using Forge.Networking;
using Forge.Networking.Players;
using Forge.ServerRegistry.Messaging.Interpreters;
using ForgeServerRegistryService.Engine;
using ForgeServerRegistryService.Messaging.Interpreters;
using ForgeServerRegistryService.Networking.Players;

namespace ForgeServerRegistryService
{
	public class ForgeServerRegistryService
	{
		private static void Main(string[] args)
		{
			ForgeRegistration.Initialize();
			var factory = AbstractFactory.Get<INetworkTypeFactory>();
			factory.Register<IRegisterAsServerInterpreter, RegisterAsServerInterpreter>();
			factory.Register<IGetServerRegistryInterpreter, GetServerRegistryInterpreter>();
			factory.Replace<INetPlayer, NetworkPlayer>();

			var networkMediator = factory.GetNew<INetworkMediator>();
			networkMediator.ChangeEngineFacade(new ServerRegistryEngine());
			networkMediator.StartServer(15940, 100);

			while (!networkMediator.SocketFacade.CancellationSource.IsCancellationRequested)
			{
				// TODO:  Listen for exit command and call cancel
			}
		}
	}
}
