using UnityEngine;
using UnityEngine.UI;

namespace Forge.Networking.Unity.UI
{
	[RequireComponent(typeof(InputField))]
	public class UIInputField : UIElement, IUIInputField
	{
		public override bool Visible { get; set; }
		private InputField _inputField;

		public string Text
		{
			get { return _inputField.text; }
			set
			{
				_inputField.text = value;
			}
		}

		protected override void Awake()
		{
			_inputField = GetComponent<InputField>();
		}
	}
}
