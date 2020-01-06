using System;
using System.Text;
using Forge.Serialization;
using Forge.ServerRegistry.DataStructures;

namespace Forge.ServerRegistry.Serializers
{
	public class ServerListingEntryArraySerializer : ITypeSerializer
	{
		public object Deserialize(BMSByte buffer)
		{
			int length = buffer.GetBasicType<int>();
			ServerListingEntry[] listing = new ServerListingEntry[length];
			for (int i = 0; i < length; i++)
			{
				listing[i] = new ServerListingEntry
				{
					Address = buffer.GetBasicType<string>(),
					Port = buffer.GetBasicType<ushort>()
				};
			}
			return listing;
		}

		public void Serialize(object val, BMSByte buffer)
		{
			var listing = (ServerListingEntry[])val;
			buffer.Append(BitConverter.GetBytes(listing.Length));
			foreach (var l in listing)
			{
				byte[] strBuf = Encoding.UTF8.GetBytes(l.Address ?? string.Empty);
				buffer.Append(BitConverter.GetBytes(strBuf.Length));
				buffer.Append(strBuf);
				buffer.Append(BitConverter.GetBytes(l.Port));
			}
		}
	}
}
