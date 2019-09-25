using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

[Serializable]
public class MDFloat
{
    [SerializeField] private List<float> values;
    public int Count { get { return values.Count; } private set { } }

    public MDFloat()
    {
        values = new List<float>();
        Count = 0;
    }

    public List<float> get_Values()
    {
        return values;
    }

    public float this[int i]
    {
        get { return values[i]; }
    }

    public void Add(float flo)
    {
        values.Add(flo);
        Count += 1;
    }
}
