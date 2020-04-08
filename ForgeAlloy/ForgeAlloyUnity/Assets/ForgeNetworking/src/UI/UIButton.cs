using UnityEngine;
using UnityEngine.UI;

namespace Forge.Networking.Unity.UI
{
	[RequireComponent(typeof(Button))]
	public abstract class UIButton<T> : UIElement, IUIButton<T>
	{
		public abstract T State { get; set; }
		public override bool Visible { get; set; } = true;

		public abstract void Invoke();

		protected override void Awake()
		{
			// Guarenteed because of the RequireComponent attribute
			GetComponent<Button>().onClick.AddListener(Invoke);
		}
	}
}
