using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using UnityEditor;
using UnityEngine;

// 15 tile autorotated version of 2-edge + 2-corner Wang Tiles. See http://www.cr31.co.uk/stagecast/wang/blob.html
public class TerrainBrush : GridBrush
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
        UpdateCell(tilemap, position + new IntVector2(1, 1), false);
        UpdateCell(tilemap, position + new IntVector2(1, 0), false);
        UpdateCell(tilemap, position + new IntVector2(1, -1), false);
        UpdateCell(tilemap, position + new IntVector2(0, -1), false);
        UpdateCell(tilemap, position + new IntVector2(-1, -1), false);
        UpdateCell(tilemap, position + new IntVector2(-1, 0), false);
        UpdateCell(tilemap, position + new IntVector2(-1, 1), false);
    }

    private void UpdateCell(TileMap tilemap, IntVector2 position, bool setAsMySprite)
    {
        int mask = TileValue(tilemap, position + new IntVector2(0, 1)) ? 1 : 0;
        mask += TileValue(tilemap, position + new IntVector2(1, 1)) ? 2 : 0;
        mask += TileValue(tilemap, position + new IntVector2(1, 0)) ? 4 : 0;
        mask += TileValue(tilemap, position + new IntVector2(1, -1)) ? 8 : 0;
        mask += TileValue(tilemap, position + new IntVector2(0, -1)) ? 16 : 0;
        mask += TileValue(tilemap, position + new IntVector2(-1, -1)) ? 32 : 0;
        mask += TileValue(tilemap, position + new IntVector2(-1, 0)) ? 64 : 0;
        mask += TileValue(tilemap, position + new IntVector2(-1, 1)) ? 128 : 0;

        byte original = (byte)mask;
        if ((original | 254) < 255) { mask = mask & 125; }
        if ((original | 251) < 255) { mask = mask & 245; }
        if ((original | 239) < 255) { mask = mask & 215; }
        if ((original | 191) < 255) { mask = mask & 95; }

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
            case 0:                                 return 0;
            case 1: case 4: case 16: case 64:       return 1;
            case 5: case 20: case 80: case 65:      return 2;
            case 7: case 28: case 112: case 193:    return 3;
            case 17: case 68:                       return 4;
            case 21: case 84: case 81: case 69:     return 5;
            case 23: case 92: case 113: case 197:   return 6;
            case 29: case 116: case 209: case 71:   return 7;
            case 31: case 124: case 241: case 199:  return 8;
            case 85:                                return 9;
            case 87: case 93: case 117: case 213:   return 10;
            case 95: case 125: case 245: case 215:  return 11;
            case 119: case 221:                     return 12;
            case 127: case 253: case 247: case 223: return 13;
            case 255:                               return 14;
        }
        return -1;
    }

    private Quaternion GetRotation(byte mask)
    {
        switch (mask)
        {
            case 4: case 20: case 28: case 68: case 84: case 92: case 116: case 124: case 93: case 125: case 221: case 253:
                return Quaternion.Euler(0f, 0f, -90f);
            case 16: case 80: case 112: case 81: case 113: case 209: case 241: case 117: case 245: case 247:
                return Quaternion.Euler(0f, 0f, -180f);
            case 64: case 65: case 193: case 69: case 197: case 71: case 199: case 213: case 215: case 223:
                return Quaternion.Euler(0f, 0f, -270f);
        }
        return Quaternion.identity;
    }

    [MenuItem("Assets/Create/TerrainBrush")]
    public static void CreateBrush()
    {
		string path = EditorUtility.SaveFilePanelInProject("Save Brush", "New Brush", "asset", "Save Brush", "Assets");

		if (path == "")
            return;

		AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerrainBrush>(), path);
    }
}
