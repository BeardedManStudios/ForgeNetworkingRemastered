namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	/// <summary>
	/// This is the class identity of this object
	/// </summary>
	public class ForgeClassIdentity
	{
		public string IdentityName;
		public int IdentityID;

		public ForgeClassIdentity()
		{
			IdentityName = string.Empty;
			IdentityID = -1;
		}

		public ForgeClassIdentity(string name, int id)
		{
			this.IdentityName = name;
			this.IdentityID = id;
		}
	}
}