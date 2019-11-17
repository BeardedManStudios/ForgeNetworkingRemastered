using System;
using UnityEngine;
using UnityEngine.UI;

namespace Forge.Networking.Unity.UI
{
	[RequireComponent(typeof(Button))]
	public class UIButton : UIElement, IUIButton
	{
		private Action _callback;

		public override bool Visible { get; set; }

		protected override void Awake()
		{
			//Guarenteed because of the RequireComponent attribute
			GetComponent<Button>().onClick.AddListener(Raise);
		}

		public void RegisterCallback(Action callback)
		{
			_callback = callback;
		}

		public void Raise()
		{
			if (_callback != null)
			{
				_callback();
			}
		}
	}
}
