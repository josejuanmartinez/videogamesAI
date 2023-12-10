using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class HexTranslator
{
    static bool initialized = false;
    static int minX, minY, maxX, maxY, sizeX, sizeY;
    public static void Initialize()
    {
        Tilemap tilemap = GameObject.Find("CardTypeTilemap").GetComponent<Tilemap>();
        minX = tilemap.cellBounds.xMin;
        minY = tilemap.cellBounds.yMin;
        maxX = tilemap.cellBounds.xMax;
        maxY = tilemap.cellBounds.yMax;

        sizeX = tilemap.cellBounds.size.x;
        sizeY = tilemap.cellBounds.size.y;
    }

    public static Vector3Int GetNormalizedCellPos(Vector3Int cellPos)
    {
        if (!initialized)
            Initialize();

        if(cellPos.x < minX || cellPos.x >= maxX || cellPos.y < minY || cellPos.y >= maxY)
        {
            return Vector3Int.back;
        }
        int displacementX = cellPos.x - minX;
        int displacementY = sizeY - (cellPos.y - minY);

        return new Vector3Int(displacementX, displacementY, 0);
    }

    public static string GetCellPosString(Vector3Int cellPos)
    {
        return "(" + cellPos.x + "," + cellPos.y + ")";
    }
    public static string GetNormalizedCellPosString(Vector3Int cellPos)
    {
        return GetNormalizedCellPosString(cellPos, false);
    }

    public static string GetNormalizedCellPosString(Vector2Int cellPos)
    {
        return GetNormalizedCellPosString(new Vector3Int(cellPos.x, cellPos.y, 0), false);
    }

    public static string GetNormalizedCellPosString(Vector3Int cellPos, bool debug)
    {
        Vector3Int res = GetNormalizedCellPos(cellPos);
        if (res == Vector3Int.back)
        {
            return "(--,--)";
        }            
        else
        {
            if (debug)
            {
                return "*" + cellPos.x + "," + cellPos.y + "*";
            }
            else
            {
                return "(" + res.x.ToString("00") + "," + res.y.ToString("00") + ")";
            }
        }

            
    }

    public static int GetNormalizedCellPosInt(Vector3Int cellPos)
    {
        Vector3Int res = GetNormalizedCellPos(cellPos);
        return (res.x * sizeX) + res.y;
    }

    public static Vector3Int UnnormalizeCellPosInt(Vector3Int cellPos)
    {
        if (!initialized)
            Initialize();

        int newX = cellPos.x + minX;
        int newY = cellPos.y + minY - sizeY;


        return new Vector3Int(newX, newY, 0);
    }

    public static string GetDebugTileInfo(Vector3Int cellPos)
    {
        return "*" + cellPos.x + "," + cellPos.y + " * ";
    }

    public static string GetDebugTileInfo(Vector2Int cellPos)
    {
        return "*" + cellPos.x + "," + cellPos.y + " * ";
    }
    public static List<Vector3Int> GetSurroundings(Vector2Int cell)
    {
        return GetSurroundings(new Vector3Int(cell.x, cell.y, 0));
    }
    public static List<Vector3Int> GetSurroundings(Vector3Int cell)
    {
        List<Vector3Int> result = new List<Vector3Int>() { cell };
        bool even = (cell.y % 2 == 0);
        Vector3Int[] directions = even ? MovementManager.directionsEvenY : MovementManager.directionsUnevenY;
        foreach(Vector3Int direction in directions)
        {
            Vector3Int res = cell + direction;
            res.z = 0;
            result.Add(res);
        }
        return result;
    }
}
