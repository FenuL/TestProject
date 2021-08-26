using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class Tile_Grid_Data
{
    [SerializeField] List<TileList> tiles;

    public List<TileList> Get_Tiles()
    {
        return tiles;
    }

    public Tile_Grid_Data()
    {
        tiles = new List<TileList>();
    }

    public Tile_Grid_Data(Tile_Grid grid)
    {
        tiles = new List<TileList>();
        for (int x = 0; x < grid.Get_Width(); x++)
        {
            tiles.Add(new TileList());
            for (int y = 0; y < grid.Get_Length(); y++)
            {
                tiles[x].Add(grid.Get_Tile(x, y).Export_Data());
            }
        }
    }
}
