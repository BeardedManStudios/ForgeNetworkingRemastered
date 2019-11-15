using UnityEngine.EventSystems;

namespace Forge.Networking.Unity.UI
{
	public abstract class UIElement : UIBehaviour, IUIElement
	{
		public abstract bool Visible { get; set; }
	}
}
