using UnityEditor;
using UnityEngine;

namespace BeardedManStudios.Forge.Networking.UnityEditor
{
	public class ForgeNetworkingAboutEditor : EditorWindow
	{
		#region Autocheck
		[InitializeOnLoad]
		private class RuntimeCheck
		{
			static RuntimeCheck()
			{
				ForgeNetworkingAboutEditor.Init();
			}
		}
		#endregion

		#region Constants
		private static ForgeNetworkingAboutEditor _instance;
		private const string EDITOR_PREF_DATE = "FNR_CHECK_DATE";
		private const string EDITOR_PREF_IGNORE = "FNR_IGNORE";
		private const string EDITOR_ASSET_STORE_LINK = "https://assetstore.unity.com/packages/slug/38344";
		private const string EDITOR_GITHUB_LINK = "https://github.com/BeardedManStudios/ForgeNetworkingRemastered";
		private const string EDITOR_DISCORD_LINK = "https://discord.gg/yzZwEYm";
		private const string EDITOR_FORUM_LINK = "https://forum.unity3d.com/threads/no-ccu-limit-forge-networking-superpowered-fully-cross-platform.286900/";
		private string Version { get { return Resources.Load<TextAsset>(ForgeNetworkingEditor.EDITOR_RESOURCES_DIR + "/version").text; } }

		//private static bool ProVersion = false;
		private static bool IgnoreEditorStartup = false;
		#endregion

		#region Private
		private Texture2D _icon;
		private Vector2 _scroll;
		#endregion

		private static void Init()
		{
			IgnoreEditorStartup = EditorPrefs.GetBool(EDITOR_PREF_IGNORE, false);

			if (IgnoreEditorStartup)
				return;

			string lastDateScan = EditorPrefs.GetString(EDITOR_PREF_DATE, null);
			if (!string.IsNullOrEmpty(lastDateScan))
			{
				long lastCheck = long.Parse(lastDateScan);
				System.DateTime lastCheckDate = new System.DateTime(lastCheck);
				System.TimeSpan timespan = System.DateTime.Now - lastCheckDate;
				if (timespan.Days < 1)
					return;
			}

			EditorPrefs.SetString(EDITOR_PREF_DATE, System.DateTime.Now.Ticks.ToString());

			InitializeWindow();
		}

		[MenuItem("Window/Forge Networking/About")]
		private static void InitializeWindow()
		{
			if (_instance == null)
			{
				_instance = GetWindow<ForgeNetworkingAboutEditor>();
				_instance.Setup();
				_instance.Show();
			}
			else
				_instance.Close();
		}

		public void Setup()
		{
			this.titleContent = new GUIContent("Forge About");
			//ProVersion = EditorGUIUtility.isProSkin;
			IgnoreEditorStartup = EditorPrefs.GetBool(EDITOR_PREF_IGNORE);
			_scroll = new Vector2();

			_icon = Resources.Load<Texture2D>("Icon");
			this.minSize = new Vector2(360, 500);
			this.maxSize = new Vector2(360, 500);
		}

		private void OnGUI()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(_icon);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();


			_scroll = EditorGUILayout.BeginScrollView(_scroll);
			if (GUILayout.Button("GitHub", GUILayout.Height(50)))
			{
				Application.OpenURL(EDITOR_GITHUB_LINK);
			}

			if (GUILayout.Button("Join Discord", GUILayout.Height(50)))
			{
				Application.OpenURL(EDITOR_DISCORD_LINK);
			}

			if (GUILayout.Button("Forum Link", GUILayout.Height(50)))
			{
				Application.OpenURL(EDITOR_FORUM_LINK);
			}

			GUILayout.Space(10);

			if (GUILayout.Button("Leave Review", GUILayout.Height(50)))
			{
				Application.OpenURL(EDITOR_ASSET_STORE_LINK);
			}
			EditorGUILayout.EndScrollView();

			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal();
			bool currentSetup = GUILayout.Toggle(IgnoreEditorStartup, "Don't Show On Start");
			if (currentSetup != IgnoreEditorStartup)
			{
				IgnoreEditorStartup = currentSetup;
				EditorPrefs.SetBool(EDITOR_PREF_IGNORE, IgnoreEditorStartup);
			}
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("Version {0}", Version), EditorStyles.boldLabel);
			GUILayout.EndHorizontal();
		}
	}
}
