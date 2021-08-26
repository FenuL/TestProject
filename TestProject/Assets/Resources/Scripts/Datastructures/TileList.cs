using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

[Serializable]
public class TileList : IEnumerable
{
    [SerializeField]
    private List<Tile_Data> tiles;
    public int Count { get { return tiles.Count; } private set { } }

    public TileList()
    {
        tiles = new List<Tile_Data>();
        Count = 0;
    }

    public List<Tile_Data> Get_Tiles()
    {
        return tiles;
    }

    public Tile_Data this[int i]
    {
        get { return tiles[i]; }
    }

    public void Add(Tile_Data tile)
    {
        tiles.Add(tile);
        Count += 1;
    }

    // Enumerators are positioned before the first element
    // until the first MoveNext() call.
    public TileList(List<Tile_Data> list)
    {
        tiles = list;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return (IEnumerator)GetEnumerator();
    }

    public TileEnum GetEnumerator()
    {
        return new TileEnum(tiles);
    }

    // When you implement IEnumerable, you must also implement IEnumerator.
    public class TileEnum : IEnumerator
    {
        public List<Tile_Data> tiles;

        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        int position = -1;

        public TileEnum(List<Tile_Data> list)
        {
            tiles = list;
        }

        public bool MoveNext()
        {
            position++;
            return (position < tiles.Count);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public Tile_Data Current
        {
            get
            {
                try
                {
                    return tiles[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
