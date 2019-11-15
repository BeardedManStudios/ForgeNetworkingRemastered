using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Forge.Networking.Unity.UI
{
	[RequireComponent(typeof(InputField))]
	public class UIInputField : UIElement, IUIInputField
	{
		public override bool Visible { get; set; }
		private InputField _inputField;

		private string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				_inputField.text = _text;
			}
		}

		protected override void Awake()
		{
			_inputField = GetComponent<InputField>();
			_text = _inputField.text;
		}
	}
}
