using BeardedManStudios.Forge.Networking;
using BeardedManStudios.Forge.Networking.Generated;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : ChatManagerBehavior
{
	public Transform contentTransform;
	public GameObject messageLabel;
	public GameObject eventSystem;

	public InputField messageInput;

	private List<Text> messageLabels = new List<Text>();
	public int maxMessages = 100;

	private void Awake()
	{
		Instantiate(eventSystem);
	}

	public override void SendMessage(RpcArgs args)
	{
		string username = args.GetNext<string>();
		string message = args.GetNext<string>();

		Text label = null;
		if (messageLabels.Count == maxMessages)
		{
			label = messageLabels[0];
			messageLabels.RemoveAt(0);
			label.transform.SetAsLastSibling();
		}
		else
			label = (Instantiate(messageLabel, contentTransform) as GameObject).GetComponent<Text>();

		messageLabels.Add(label);
		label.text = username + ": " + message;
	}

	public void SendMessage()
	{
		string message = messageInput.text.Trim();
		if (string.IsNullOrEmpty(message))
			return;

		string name = networkObject.Networker.Me.Name;

		if (string.IsNullOrEmpty(name))
			name = NetWorker.InstanceGuid.ToString().Substring(0, 5);

		networkObject.SendRpc(RPC_SEND_MESSAGE, Receivers.All, name, message);
		messageInput.text = "";
		messageInput.Select();
	}
}