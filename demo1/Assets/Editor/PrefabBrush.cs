using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using UnityEditor;
using UnityEngine;

// Brush to paint tilemap tile + instantiate prefab
public class PrefabBrush : GridBrush
{
    public Sprite m_Sprite;
    public GameObject m_Prefab;

    public override void Paint(Grid grid, IntVector2 position, Color color)
    {
        if (m_Prefab == null)
            return;

        TileMap tilemap = grid.GetComponent<TileMap>();
        tilemap.SetSprite(m_Sprite, position.x, position.y);
        Vector3 newPosition = grid.GetCellCenter(position.x, position.y, 0f);

        GameObject newGameObject;
        if (EditorUtility.IsPersistent(m_Prefab))
            newGameObject = PrefabUtility.InstantiatePrefab(m_Prefab) as GameObject;
        else
            newGameObject = Instantiate(m_Prefab);

        newGameObject.transform.position = newPosition;
        newGameObject.transform.rotation = Quaternion.identity;
        newGameObject.transform.localScale = Vector3.one;
        newGameObject.transform.parent = grid.transform;
    }

    public override Sprite GetPreviewSprite()
    {
        return m_Sprite;
    }

    [MenuItem("Assets/Create/PrefabBrush")]
    public static void CreateBrush()
    {
        string newClipPath = EditorUtility.SaveFilePanelInProject("Save Brush", "New Brush", "asset", "Save Brush", "Assets");

        if (newClipPath == "")
            return;

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PrefabBrush>(), newClipPath);
    }
}
