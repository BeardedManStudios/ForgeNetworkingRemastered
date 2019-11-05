using UnityEditor;
using UnityEngine;

namespace BeardedManStudios.MultiplayerMenu.Editor
{
	[CustomEditor(typeof(MultiplayerMenu))]
	public class MultiplayerMenuEditor : UnityEditor.Editor
	{
		private SerializedProperty ipAddress;
		private SerializedProperty portNumber;
		private SerializedProperty settings;
		private SerializedProperty networkManager;
		private SerializedProperty toggleButtons;
		private SerializedProperty DontChangeSceneOnConnect;
		private SerializedProperty masterServerHost;
		private SerializedProperty masterServerPort;
		private SerializedProperty natServerHost;
		private SerializedProperty natServerPort;
		private SerializedProperty connectUsingMatchmaking;
		private SerializedProperty useElo;
		private SerializedProperty myElo;
		private SerializedProperty eloRequired;
		private SerializedProperty useMainThreadManagerForRPCs;
		private SerializedProperty useInlineChat;
		private SerializedProperty getLocalNetworkConnections;
		private SerializedProperty useTCP;

		private void OnEnable()
		{
			ipAddress = serializedObject.FindProperty("ipAddress");
			portNumber = serializedObject.FindProperty("portNumber");

			settings = serializedObject.FindProperty("Settings");
			networkManager = serializedObject.FindProperty("networkManager");
			toggleButtons = serializedObject.FindProperty("ToggledButtons");

			DontChangeSceneOnConnect = serializedObject.FindProperty("DontChangeSceneOnConnect");
			masterServerHost = serializedObject.FindProperty("masterServerHost");
			masterServerPort = serializedObject.FindProperty("masterServerPort");
			natServerHost = serializedObject.FindProperty("natServerHost");
			natServerPort = serializedObject.FindProperty("natServerPort");
			connectUsingMatchmaking = serializedObject.FindProperty("connectUsingMatchmaking");
			useElo = serializedObject.FindProperty("useElo");
			myElo = serializedObject.FindProperty("myElo");
			eloRequired = serializedObject.FindProperty("eloRequired");
			useMainThreadManagerForRPCs = serializedObject.FindProperty("useMainThreadManagerForRPCs");
			useInlineChat = serializedObject.FindProperty("useInlineChat");
			getLocalNetworkConnections = serializedObject.FindProperty("getLocalNetworkConnections");
			useTCP = serializedObject.FindProperty("useTCP");
		}

		public override void OnInspectorGUI()
		{
			//DrawDefaultInspector();

			serializedObject.Update();
			EditorGUILayout.BeginVertical();

			EditorGUILayout.PropertyField(ipAddress);
			EditorGUILayout.PropertyField(portNumber);
			EditorGUILayout.PropertyField(settings);
			EditorGUILayout.PropertyField(networkManager);
			EditorGUILayout.PropertyField(toggleButtons);

			EditorGUILayout.EndVertical();

			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical();
			EditorGUILayout.LabelField("Obsolete Settings", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox(
				"Please use the new ForgeSettings scriptable object. The default settings have been moved to the 'Default Forge Settings' object found in the Scripts folder. These fields will be removed in a future update.",
				MessageType.Warning
			);

			EditorGUILayout.HelpBox(
				"By clicking the 'Update to use ForgeSettings button a new scriptableobject with these settings will be created and linked in the Settings field above.",
				MessageType.Info);

			if (GUILayout.Button("Update to use ForgeSettings"))
			{
				var newSettings = ScriptableObject.CreateInstance<ForgeSettings>();
				newSettings.DontChangeSceneOnConnect = DontChangeSceneOnConnect.boolValue;
				newSettings.masterServerHost = masterServerHost.stringValue;
				newSettings.masterServerPort = (ushort)masterServerPort.intValue;
				newSettings.natServerHost = natServerHost.stringValue;
				newSettings.natServerPort = (ushort)natServerPort.intValue;
				newSettings.connectUsingMatchmaking = connectUsingMatchmaking.boolValue;
				newSettings.useElo = useElo.boolValue;
				newSettings.myElo = myElo.intValue;
				newSettings.eloRequired = eloRequired.intValue;
				newSettings.useMainThreadManagerForRPCs = useMainThreadManagerForRPCs.boolValue;
				newSettings.useInlineChat = useInlineChat.boolValue;
				newSettings.getLocalNetworkConnections = getLocalNetworkConnections.boolValue;
				newSettings.useTCP = useTCP.boolValue;

				AssetDatabase.CreateAsset(newSettings, "Assets/Bearded Man Studios Inc/Scripts/New Forge Settings.asset");
				AssetDatabase.SaveAssets();

				settings.objectReferenceValue = newSettings;
			}

			EditorGUILayout.PropertyField(DontChangeSceneOnConnect);
			EditorGUILayout.PropertyField(masterServerHost);
			EditorGUILayout.PropertyField(masterServerPort);
			EditorGUILayout.PropertyField(natServerHost);
			EditorGUILayout.PropertyField(natServerPort);
			EditorGUILayout.PropertyField(connectUsingMatchmaking);
			EditorGUILayout.PropertyField(useElo);
			EditorGUILayout.PropertyField(myElo);
			EditorGUILayout.PropertyField(eloRequired);
			EditorGUILayout.PropertyField(useMainThreadManagerForRPCs);
			EditorGUILayout.PropertyField(useInlineChat);
			EditorGUILayout.PropertyField(getLocalNetworkConnections);
			EditorGUILayout.PropertyField(useTCP);

			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();
		}
	}
}
