using UnityEditor;
using UnityEngine;
using System.Collections;

public class AnimationBrush : GridBrush
{
	public Sprite[] m_Animation;
	public float m_MinSpeed = 1f;
	public float m_MaxSpeed = 1f;

	public override void Paint(Grid grid, IntVector2 position, Color color)
	{
		TileMap tilemap = grid.GetComponent<TileMap>();
		
		if (tilemap == null || m_Animation == null || m_Animation.Length == 0)
			return;

		tilemap.SetTile(position, m_Animation[0]);
		tilemap.SetAnimation(position, m_Animation, Random.Range(m_MinSpeed, m_MaxSpeed));
	}

	public override Sprite GetPreviewSprite()
	{
		return m_Animation != null && m_Animation.Length > 0 ? m_Animation[0] : null;
	}

	[MenuItem("Assets/Create/AnimationBrush")]
	public static void CreateBrush()
	{
		string path = EditorUtility.SaveFilePanelInProject("Save Brush", "New Brush", "asset", "Save Brush", "Assets");

		if (path == "")
			return;

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<AnimationBrush>(), path);
	}
}
