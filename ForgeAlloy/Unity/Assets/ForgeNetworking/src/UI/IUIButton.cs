using System;

namespace Forge.Networking.Unity.UI
{
	public interface IUIButton : IUIElement
	{
		void RegisterCallback(Action callback);
	}
}
