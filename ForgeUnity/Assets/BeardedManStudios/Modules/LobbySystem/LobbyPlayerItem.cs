using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace BeardedManStudios.Forge.Networking.Unity.Lobby
{
	public class LobbyPlayerItem : MonoBehaviour
	{
		public Color[] TeamColors;
		public Color[] AvatarColors;
		public GameObject KickButton;
		public Image AvatarBG;
		public Text AvatarID;
		public InputField PlayerName;
		public Text PlayerTeamID;

        public Button[] Buttons;

		[HideInInspector]
		public Transform ThisTransform;

		[HideInInspector]
		public GameObject ThisGameObject;

		public LobbyPlayer AssociatedPlayer { get; private set; }
		private LobbyManager _manager;

		public void Init(LobbyManager manager)
		{
			ThisGameObject = gameObject;
			ThisTransform = transform;
			_manager = manager;
		}

		public void Setup(LobbyPlayer associatedPlayer, bool interactableValue)
		{
			ToggleInteractables(interactableValue);
			AssociatedPlayer = associatedPlayer;
            ChangeAvatarID(associatedPlayer.AvatarID);
            ChangeName(associatedPlayer.Name);
            ChangeTeam(associatedPlayer.TeamID);
		}

		public void SetParent(Transform parent)
		{
			ThisTransform.SetParent(parent);
			ThisTransform.localPosition = Vector3.zero;
			ThisTransform.localScale = Vector3.one;
		}

		public void KickPlayer()
		{
			_manager.KickPlayer(this);
		}
		
		public void RequestChangeTeam()
		{
			int nextID = AssociatedPlayer.TeamID + 1;
			if (nextID >= TeamColors.Length)
				nextID = 0;

			_manager.ChangeTeam(this, nextID);
		}

		public void RequestChangeAvatarID()
		{
			int nextID = AssociatedPlayer.AvatarID + 1;
			if (nextID >= AvatarColors.Length)
				nextID = 0;

			_manager.ChangeAvatarID(this, nextID);
		}

		public void RequestChangeName()
		{
			_manager.ChangeName(this, PlayerName.text);
		}

		public void ChangeAvatarID(int id)
		{
			Color avatarColor = Color.white;

			//Note: This is just an example, you are free to make your own team colors and
			// change this to however you see fit
			if (TeamColors.Length > id && id >= 0)
				avatarColor = AvatarColors[id];

			AvatarID.text = id.ToString();
			AvatarBG.color = avatarColor;
		}

		public void ChangeName(string name)
		{
			PlayerName.text = name;
		}

		public void ChangeTeam(int id)
		{
			PlayerTeamID.text = string.Format("Team {0}", id);
		}

		public void ToggleInteractables(bool value)
		{
            for (int i = 0; i < Buttons.Length; ++i)
                Buttons[i].interactable = value;

            AvatarBG.raycastTarget = value;
			PlayerTeamID.raycastTarget = value;
			PlayerName.interactable = value;
		}

		public void ToggleObject(bool value)
		{
			ThisGameObject.SetActive(value);
		}
	}
}