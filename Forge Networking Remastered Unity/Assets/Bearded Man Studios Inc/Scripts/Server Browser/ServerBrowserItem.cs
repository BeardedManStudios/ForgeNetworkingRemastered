using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace BeardedManStudios.Forge.Networking.Unity
{
    public class ServerBrowserItem : MonoBehaviour
    {
        public Text NameText;
        public Button ConnectButton;

        public void SetData(string name, UnityAction callback)
        {
            NameText.text = name;
            ConnectButton.onClick.AddListener(callback);
        }
    }
}
