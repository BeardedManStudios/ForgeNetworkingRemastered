using System.Collections.Generic;
using System.Net;
using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.ServerRegistry.DataStructures;
using Forge.ServerRegistry.Messaging.Interpreters;
using Forge.ServerRegistry.Messaging.Messages;
using ForgeServerRegistryService.Networking.Players;

namespace ForgeServerRegistryService.Messaging.Interpreters
{
	public class GetServerRegistryInterpreter : IGetServerRegistryInterpreter
	{
		public bool ValidOnClient => false;
		public bool ValidOnServer => true;

		public void Interpret(INetworkMediator netContainer, EndPoint sender, IMessage message)
		{
			var registryMessage = new ForgeServerRegistryMessage();
			registryMessage.Entries = GetEntriesArrayFromCurrentPlayers(netContainer);
			netContainer.MessageBus.SendReliableMessage(registryMessage,
				netContainer.SocketFacade.ManagedSocket, sender);
		}

		private static ServerListingEntry[] GetEntriesArrayFromCurrentPlayers(INetworkMediator netContainer)
		{
			var entries = new List<ServerListingEntry>();
			if (netContainer.PlayerRepository.Count > 0)
			{
				using (var e = netContainer.PlayerRepository.GetEnumerator())
				{
					while (e.MoveNext())
					{
						var player = (NetworkPlayer)e.Current;
						if (player.IsRegisteredServer)
						{
							var ep = (IPEndPoint)player.EndPoint;
							entries.Add(new ServerListingEntry
							{
								Address = ep.Address.ToString(),
								Port = (ushort)ep.Port
							});
						}
					}
				}
			}
			return entries.ToArray();
		}
	}
}
