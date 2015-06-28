using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using UnityEditor;
using UnityEngine;

// 5 tile autorotated version of 2-edge Wang Tiles. See http://www.cr31.co.uk/stagecast/wang/connect.html
public class PipelineBrush : GridBrush
{
    public Sprite[] m_Sprites;
    public Sprite m_Preview;

    public override void Paint(Grid grid, IntVector2 position, Color color)
    {
        TileMap tilemap = grid.GetComponent<TileMap>();
        UpdateCell(tilemap, position, true);
        UpdateSurroundingCells(tilemap, position);
    }

    public override void Erase(Grid grid, IntVector2 position)
    {
        TileMap tilemap = grid.GetComponent<TileMap>();
        tilemap.SetTile(position, null);
        UpdateSurroundingCells(tilemap, position);
    }

    public override Sprite GetPreviewSprite()
    {
        if (m_Preview != null)
            return m_Preview;
        if (m_Sprites != null && m_Sprites.Length > 0)
            return m_Sprites[0];
        return null;
    }

    private void UpdateSurroundingCells(TileMap tilemap, IntVector2 position)
    {
        UpdateCell(tilemap, position + new IntVector2(0, 1), false);
        UpdateCell(tilemap, position + new IntVector2(1, 0), false);
        UpdateCell(tilemap, position + new IntVector2(0, -1), false);
        UpdateCell(tilemap, position + new IntVector2(-1, 0), false);
    }

    private void UpdateCell(TileMap tilemap, IntVector2 position, bool setAsMySprite)
    {
        int mask = TileValue(tilemap, position + new IntVector2(0, 1)) ? 1 : 0;
        mask += TileValue(tilemap, position + new IntVector2(1, 0)) ? 2 : 0;
        mask += TileValue(tilemap, position + new IntVector2(0, -1)) ? 4 : 0;
        mask += TileValue(tilemap, position + new IntVector2(-1, 0)) ? 8 : 0;

        int index = GetIndex((byte)mask);

        if (index >= 0 && index < m_Sprites.Length && (setAsMySprite || TileValue(tilemap, position)))
        {
			tilemap.SetTile(position, m_Sprites[index]);
			tilemap.SetRotation(position, GetRotation((byte)mask));
        }
    }

    private bool TileValue(TileMap tileMap, IntVector2 position)
    {
        Sprite sprite = tileMap.GetTile(position);
        return sprite != null && m_Sprites.Contains(sprite);
    }

    private int GetIndex(byte mask)
    {
        switch (mask)
        {
            case 0:                                             return 0;
            case 3: case 6: case 9: case 12:                    return 1;
            case 1: case 2: case 4: case 5: case 10: case 8:    return 2;
            case 7: case 11: case 13: case 14:                  return 3;
            case 15:                                            return 4;
        }
        return -1;
    }

    private Quaternion GetRotation(byte mask)
    {
        switch (mask)
        {
            case 9: case 10: case 7: case 2: case 8:
                return Quaternion.Euler(0f, 0f, -90f);
            case 3: case 14:
                return Quaternion.Euler(0f, 0f, -180f);
            case 6: case 13:
		        return Quaternion.Euler(0f, 0f, -270f);
        }
        return Quaternion.identity;
    }

    [MenuItem("Assets/Create/PipelineBrush")]
    public static void CreateBrush()
    {
		string path = EditorUtility.SaveFilePanelInProject("Save Brush", "New Brush", "asset", "Save Brush", "Assets");

		if (path == "")
            return;

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<PipelineBrush>(), path);
    }
}
