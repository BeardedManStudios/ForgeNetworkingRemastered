namespace Forge.Networking.Unity.UI
{
	public interface IUIButton<T> : IUIElement
	{
		T Mediator { get; set; }
		void Invoke();
	}
}
