namespace Forge.Networking.Unity.UI
{
	public interface IUIButton<T> : IUIElement
	{
		T State { get; set; }
		void Invoke();
	}
}
