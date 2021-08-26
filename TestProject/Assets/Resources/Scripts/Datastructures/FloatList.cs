using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

[Serializable]
public class FloatList
{
    [SerializeField] private List<float> values;
    public int Count { get { return values.Count; } private set { } }

    public FloatList()
    {
        values = new List<float>();
        Count = 0;
    }

    /// <summary>
    /// Create a new FloatList from a string input
    /// </summary>
    /// <param name="input">The input string. Should be in format [0.1,0.2,...]</param>
    public FloatList(string input)
    {
        values = new List<float>();
        Count = 0;
        string[] new_values = input.Replace("[", "").Replace("]", "").Split(',');
        foreach (string str in new_values)
        {
            float f;
            if (float.TryParse(str, out f))
            {
                Add(f);
            }else
            {
                Add(0.0f);
            }
        }
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

    public string To_String()
    {
        string output = "[";
        foreach(float f in values)
        {
            output=output + f + ",";
        }
        output = output.Substring(0, output.Length - 1);
        output = output + "]";
        return output;
    }

}
