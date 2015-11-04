using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

namespace X_UniTMX
{
	public class XUniTMXConfiguration : ScriptableObject
	{

		//public static XUniTMXConfiguration Instance = null;
		#region Singleton
		private static XUniTMXConfiguration _instance;

		public static XUniTMXConfiguration Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load<XUniTMXConfiguration>("X-UniTMX_Configs");

					if (_instance == null)
					{
						_instance = CreateInstance<XUniTMXConfiguration>();
#if UNITY_EDITOR
						if (!System.IO.Directory.Exists(System.IO.Path.Combine(Application.dataPath, "X-UniTMX/Resources")))
							AssetDatabase.CreateFolder("Assets/X-UniTMX", "Resources");

						AssetDatabase.CreateAsset(_instance, System.IO.Path.Combine("Assets/X-UniTMX/Resources", "X-UniTMX_Configs.asset"));
#endif
					}
				}

				return _instance;
			}
			set
			{
				_instance = value;
			}
		}
		#endregion

		/// <summary>
		/// Pixel Correction to be applied to Tile's textures, to avoid gaps/seams between tiles.
		/// </summary>
		/// <remarks>
		/// Usually the default value is enough, but you might need to adjust it for your project. 
		/// If you are using Spacing between tiles, you can set this to 0.
		/// </remarks>
		public float PixelCorrection = 0.01f;

		/// <summary>
		/// How accurate will be the colliders that are approximated to an ellipse format.
		/// </summary>
		/// <remarks>
		/// Increasing this value will generate higher-quality ellipsoides, at the cost of more vertices.
		/// This number is the number of generated vertices the ellipsoide will have.
		/// </remarks>
		public int EllipsoideColliderApproximationFactor = 16;

		/// <summary>
		/// The difference in layer depth between layers.
		/// </summary>
		/// <remarks>
		/// The algorithm for creating the LayerDepth for each layer when enumerating from
		/// back to front is:
		/// float layerDepth = 1f - (LayerDepthSpacing * x);</remarks>
		public int LayerDepthSpacing = 1;

		/// <summary>
		/// The Anchor to be used with prefabs generated from TileLayers
		/// </summary>
		public Anchor TilePrefabAnchor = Anchor.Center;

		/// <summary>
		/// The actual TilePrefabAnchor point value
		/// </summary>
		[SerializeField]
		protected Vector2 TilePrefabsAnchorPoint = Vector2.zero;

		/// <summary>
		/// The Filter mode to be applied to the TileSets
		/// </summary>
		public FilterMode TileSetsFilterMode = FilterMode.Point;

		public Vector2 GetTilePrefabsAnchorPoint()
		{
			if (TilePrefabAnchor.Equals(Anchor.Custom))
				return TilePrefabsAnchorPoint;

			return ObjectExtensions.GetAnchorPointValue(TilePrefabAnchor);
		}
	}
}
