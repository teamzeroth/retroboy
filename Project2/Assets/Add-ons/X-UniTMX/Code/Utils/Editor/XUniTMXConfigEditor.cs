using UnityEngine;
using UnityEditor;
using System.IO;

namespace X_UniTMX
{
	[CustomEditor(typeof(XUniTMXConfiguration))]
	[InitializeOnLoad]
	public class XUniTMXConfigEditor : Editor
	{
		#region Menu Items
		[MenuItem("X-UniTMX/Configurations", false, 0)]
		public static void Edit()
		{
			Selection.activeObject = XUniTMXConfiguration.Instance;
		}

		[MenuItem("X-UniTMX/Report a Bug", false, 21)]
		public static void ReportBug()
		{
			Application.OpenURL("https://bitbucket.org/Chaoseiro/x-unitmx/issues");
		}

		[MenuItem("X-UniTMX/View Forums", false, 22)]
		public static void OpenForum()
		{
			Application.OpenURL("http://forum.x-unitmx.org/");
		}

		[MenuItem("X-UniTMX/View Trello Board", false, 23)]
		public static void OpenTrello()
		{
			Application.OpenURL("https://trello.com/b/aIFN7aTk/x-unitmx");
		}

		[MenuItem("X-UniTMX/View Tiled Website", false, 35)]
		public static void OpenTiledSite()
		{
			Application.OpenURL("http://www.mapeditor.org/");
		}

		[MenuItem("X-UniTMX/View Tiled Forum", false, 36)]
		public static void OpenTiledForum()
		{
			Application.OpenURL("http://forum.mapeditor.org/");
		}

		[MenuItem("X-UniTMX/Support Tiled Patreon", false, 37)]
		public static void OpenTiledPatreon()
		{
			Application.OpenURL("https://www.patreon.com/bjorn");
		}
		#endregion

		Texture2D imageIcon;
		GUIStyle headerStyle;

		SerializedProperty _anchorPointValue;

		public void OnEnable()
		{
			if (XUniTMXConfiguration.Instance == null)
			{
				XUniTMXConfiguration.Instance = Resources.Load<XUniTMXConfiguration>("X-UniTMX_Configs");
				if (XUniTMXConfiguration.Instance == null)
				{
					XUniTMXConfiguration.Instance = CreateInstance<XUniTMXConfiguration>();
					if (!System.IO.Directory.Exists(System.IO.Path.Combine(Application.dataPath, "X-UniTMX/Resources")))
						AssetDatabase.CreateFolder("Assets/X-UniTMX", "Resources");

					AssetDatabase.CreateAsset(XUniTMXConfiguration.Instance, System.IO.Path.Combine("Assets/X-UniTMX/Resources", "X-UniTMX_Configs.asset"));

				}
			}

			DirectoryInfo rootDir = new DirectoryInfo(Application.dataPath);
			FileInfo[] files = rootDir.GetFiles("GUIHelpers.cs", SearchOption.AllDirectories);
			string editorIconPath = Path.GetDirectoryName(files[0].FullName.Replace("\\", "/").Replace(Application.dataPath, "Assets"));
			editorIconPath = editorIconPath + "/Icons";
			imageIcon = (Texture2D)AssetDatabase.LoadMainAssetAtPath(editorIconPath + "/X-UniTMX Logo Final_32.png");

			headerStyle = new GUIStyle();
			headerStyle.fontStyle = FontStyle.Bold;
			headerStyle.fontSize = 16;
			headerStyle.normal.textColor = Color.white;

			_anchorPointValue = serializedObject.FindProperty("TilePrefabsAnchorPoint");
		}

		protected override void OnHeaderGUI()
		{
			EditorGUI.DrawRect(new Rect(0, 0, Screen.width, 39), Color.grey);
			EditorGUI.DrawTextureTransparent(new Rect(4, 4, 32, 32), imageIcon);
			EditorGUI.LabelField(new Rect(40, 10, Screen.width - 40, 39), "X-UniTMX Configurations", headerStyle);
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Separator();
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();

			XUniTMXConfiguration.Instance.PixelCorrection = EditorGUILayout.Slider(
				new GUIContent("Pixel Correction", "Pixel Correction to be applied to Tile's textures, to avoid gaps/seams between tiles."),
				XUniTMXConfiguration.Instance.PixelCorrection,
				0,
				0.5f);
			
			XUniTMXConfiguration.Instance.EllipsoideColliderApproximationFactor = EditorGUILayout.IntSlider(
				new GUIContent("Ellipse Approximation", "How accurate will be the Colliders that are approximated to an ellipse format (this set the number of vertices of the approximated ellipsoid)."),
				XUniTMXConfiguration.Instance.EllipsoideColliderApproximationFactor,
				4,
				64);			

			XUniTMXConfiguration.Instance.LayerDepthSpacing = EditorGUILayout.IntField(
				new GUIContent("Layer Depth Spacing", "The difference in layer depth (Z position) between layers."),
				XUniTMXConfiguration.Instance.LayerDepthSpacing);

			XUniTMXConfiguration.Instance.TilePrefabAnchor = (Anchor)EditorGUILayout.EnumPopup(
				new GUIContent("Tile Prefabs Anchor", "The Anchor to be used with prefabs generated from TileLayers"),
				XUniTMXConfiguration.Instance.TilePrefabAnchor);

			if (XUniTMXConfiguration.Instance.TilePrefabAnchor.Equals(Anchor.Custom))
			{
				float x = _anchorPointValue.vector2Value.x;
				float y = _anchorPointValue.vector2Value.y;
				x = EditorGUILayout.Slider(
					new GUIContent("Anchor Point X", "The custom anchor point's X value."),
					x,
					0,
					1.0f);
				y = EditorGUILayout.Slider(
					new GUIContent("Anchor Point Y", "The custom anchor point's Y value."),
					y,
					0,
					1.0f);
				_anchorPointValue.vector2Value = new Vector2(x, y);
			}

			XUniTMXConfiguration.Instance.TileSetsFilterMode = (FilterMode)EditorGUILayout.EnumPopup(
				new GUIContent("Texture Filter Mode", "The Filter Mode to be used with TileSet's Textures"),
				XUniTMXConfiguration.Instance.TileSetsFilterMode);

			serializedObject.ApplyModifiedProperties();
			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(XUniTMXConfiguration.Instance);
		}
	}
}