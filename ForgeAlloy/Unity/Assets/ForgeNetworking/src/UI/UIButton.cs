using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Forge.Networking.Unity.UI
{
	[RequireComponent(typeof(Button))]
	public class UIButton : UIElement, IUIButton
	{
		private Action _callback;

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

		public override bool Visible { get; set; }
	}
}
