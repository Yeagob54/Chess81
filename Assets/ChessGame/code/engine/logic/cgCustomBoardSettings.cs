﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class is used for irregular boards( board != 8x8 ), to place pieces accordingly at the top and bottom of the baord.
/// </summary>
public class cgCustomBoardSettings  {

    /// <summary>
    /// All possible moves generated by MoveGenerator, stored by an identifying string using the format (piecetype)+(indexpostion)
    /// </summary>
    public static Dictionary<string, List<sbyte>> PiecePlacements = new Dictionary<string, List<sbyte>>();

    public static void AddPiecePlacement(byte width, byte height,List<sbyte>placements)
    {
        if(!PiecePlacements.ContainsKey(GetKey(width,height)))
            PiecePlacements.Add(GetKey(width, height), placements);
    }
    public static List<sbyte> GetPiecePlacements(byte width, byte height)
    {
        string keyPlacement = GetKey(width, height);
        if (!PiecePlacements.ContainsKey(keyPlacement))
        {
            PiecePlacements.Add(keyPlacement, _GeneratePlacements(width, height));
        }
        return PiecePlacements[keyPlacement];

    }
    public static string GetKey(byte width, byte height)
    {
        return width.ToString() + "x" + height.ToString();
    }
    private static List<sbyte> _GeneratePlacements(byte boardWidth, byte boardHeight)
    {
        List<sbyte> typesFirstRow = new List<sbyte> { 2, 3, 4, 5, 6, 4, 3, 2 };
        List<sbyte> typesSecondRow = new List<sbyte> { 1, 1, 1, 1, 1, 1, 1, 1 };

        int indentation = (int)Math.Round((double)(boardWidth - typesFirstRow.Count) / 2);
        if (indentation < 0) indentation = 0;
        List<sbyte> place = new List<sbyte>();
        for (int col = 0; col < boardHeight; col++)
        {
            for (int row = 0; row < boardWidth; row++)
            {
                if (row < indentation)
                {
                    place.Add(0);
                }
                else if (col == 0 && (row - indentation) < typesFirstRow.Count)
                {
                    place.Add((sbyte)(0 - typesFirstRow[row - indentation]));
                }
                else if (col == 1 && (row - indentation) < typesFirstRow.Count)
                {
                    place.Add((sbyte)(0 - typesSecondRow[row - indentation]));
                }
                else if (col == boardHeight - 1 && (row - indentation) < typesFirstRow.Count)
                {
                    place.Add((sbyte)(typesFirstRow[row - indentation]));
                }
                else if (col == boardHeight - 2 && (row - indentation) < typesFirstRow.Count)
                {
                    place.Add((sbyte)(typesSecondRow[row - indentation]));
                }
                else
                {
                    place.Add(0);
                }
            }
        }
        return place;
    }
}