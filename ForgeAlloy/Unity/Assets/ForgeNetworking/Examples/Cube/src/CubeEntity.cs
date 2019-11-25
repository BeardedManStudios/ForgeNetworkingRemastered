using Forge.Engine;
using UnityEngine;

namespace Forge.Networking.Unity.Examples
{
	public class CubeEntity : MonoBehaviour, IUnityEntity
	{
		public int Id { get; set; }

		[SerializeField]
		private int _creationId;
		public int CreationId { get { return _creationId; } set { _creationId = value; } }
	}
}
