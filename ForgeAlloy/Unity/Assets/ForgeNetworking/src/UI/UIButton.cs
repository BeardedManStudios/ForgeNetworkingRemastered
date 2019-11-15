using System;
using UnityEngine;

namespace Forge.Networking.Unity.UI
{
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
