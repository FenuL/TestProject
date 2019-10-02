using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Class for Armor class Equipment. Inherits from Equipment. 
/// </summary>
[Serializable]
public class Armor : Equipment
{
    [SerializeField]
    public int armor;

    public static Armor ParseJSON(string json)
    {
        Armor armor = JsonUtility.FromJson<Armor>(json);
        return armor;
    }

    /// <summary>
    /// Takes a List of strings and returns a usable Armor object.
    /// </summary>
    /// <param name="input">The list of armor stats to create the Armor.</param>
    /// <returns>The Armor object.</returns>
    /*public static Armor Parse(string[] input)
    {
        Armor armor = new Armor();
        float number = 0;
        foreach (string s in input)
        {
            if (s != null)
            {
                //Split the line into its category and its values
                string[] split = s.Split(':');
                if (split.Length < 2)
                {
                    return null;
                }
                string category = split[0];
                string values = split[1];
                //read the inputs based on their categories
                switch (category)
                {
                    case "name":
                        armor.name = values.TrimStart().TrimEnd();
                        category = armor.name;
                        break;
                    case "armor":
                        number = 0;
                        float.TryParse(values.TrimStart().TrimEnd(), out number);
                        armor.armor = (int)number;
                        break;
                    case "weight":
                        number = 0;
                        float.TryParse(values.TrimStart().TrimEnd(), out number);
                        armor.weight = number;
                        break;
                    case "Passive":
                        armor.passive = values.TrimStart().TrimEnd();
                        break;
                    case "Reaction":
                        armor.reaction = values.TrimStart().TrimEnd();
                        break;
                    case "Active1":
                        armor.active1 = values.TrimStart().TrimEnd();
                        break;
                    case "Active2":
                        armor.active2 = values.TrimStart().TrimEnd();
                        break;
                }
            }
        }
        armor.actions[0] = armor.active1;
        armor.actions[1] = armor.active2;
        armor.actions[2] = armor.reaction;
        return armor;
    }*/

    /// <summary>
    /// Used in conjustion with the Parse method to create a Dictionary of all known armor types.
    /// </summary>
    /// <returns>A constructed Dictionary containing all known armor types, using their names as the key.</returns>
    public static Dictionary<string, Armor> Load_Armors()
    {
        Dictionary<string, Armor> armor_types = new Dictionary<string, Armor>();
        foreach (string file in System.IO.Directory.GetFiles("Assets/Resources/Equipment/Armors/"))
        {
            //string[] lines = System.IO.File.ReadAllLines(file);
            //Armor armor = Parse(lines);
            Armor armor = null;
            if (file.EndsWith(".json"))
            {
                armor = ParseJSON(System.IO.File.ReadAllText(file));
                //string json = JsonUtility.ToJson(armor);
                //Debug.Log("Armor:" + json);
            }
            if (armor != null && armor.name != "")
            {
                if (!armor_types.ContainsKey(armor.name))
                {
                    armor_types.Add(armor.name, armor);
                }else
                {
                    Debug.Log(armor.name + " already exists!");
                }
            }
        }
        return armor_types;
    }

    /// <summary>
    /// An empty Constructor used to instantiate an empty Armor object.
    /// </summary>
    public Armor()
    {
        type = Equipment_Type.Armor;
        name = "";
        action_names = new string[3];
    }

    /// <summary>
    /// Class Constructor
    /// </summary>
    /// <param name="str">A String for the type of Armor to create. Parses the string to an Armor_Type.</param>
    public Armor(string str)
    {
        type = Equipment_Type.Armor;
    }

    /// <summary>
    /// Class Constructor Overload. Takes a Type of Armor instead of a String.
    /// </summary>
    /// <param name="ar">Type of Armor to create. </param>
    public Armor(Armor_Types ar)
    {
        type = Equipment_Type.Armor;
    }
}
