using Forge.Networking;
using Forge.Networking.Messaging;
using Forge.Networking.Sockets;
using Forge.Networking.Unity;
using Forge.Networking.Unity.Messages;
using UnityEngine;
using UnityEngine.UI;

public class SendButton : MonoBehaviour
{
	[SerializeField]
	private InputField _messageInput = null;
	private string _myName = "";
	private string[] _names = new string[] { "Brent", "Monica", "Brett", "Bob", "Bill", "Thomas", "Cat", "Dog", "Cow", "Man", "Hillboy 321" };

	private IEngineFacade _engine = null;
	private ISocket _mySocket => _engine.NetworkMediator.SocketFacade.ManagedSocket;
	private bool _isServer => _engine.NetworkMediator.SocketFacade is ISocketServerFacade;

	private MessagePool<ChatMessage> _chatMessagePool = new MessagePool<ChatMessage>();

	public void Awake()
	{
		_engine = GameObject.FindObjectOfType<ForgeEngineFacade>();
		_myName = _names[Random.Range(0, _names.Length)];
	}

	public void Send()
	{
		string txt = _messageInput.text.Trim();
		if (string.IsNullOrEmpty(txt))
			return;
		SendNetworkMessage(txt);
	}

	private void SendNetworkMessage(string txt)
	{
		var m = _chatMessagePool.Get();
		m.Name = _myName;
		m.Text = txt;
		if (_isServer)
			SendMessageAsServer(m);
		else
			SendMessageAsClient(m);
		_engine.NetworkMediator.RunMessageLocally(m);
	}

	private void SendMessageAsClient(ChatMessage m)
	{
		_engine.NetworkMediator.SendReliableMessage(m, _mySocket.EndPoint);
	}

	private void SendMessageAsServer(ChatMessage m)
	{
		_engine.NetworkMediator.SendReliableMessage(m);
	}
}
