using UnityEngine;
using UnityEngine.UI;

public class ChatListener : MonoBehaviour
{
	[SerializeField]
	private GameObject _textPrefab = null;

	public void PrintMessage(string name, string messageText)
	{
		var go = GameObject.Instantiate(_textPrefab, transform);
		go.GetComponent<Text>().text = $"{name}: {messageText}";
	}
}
