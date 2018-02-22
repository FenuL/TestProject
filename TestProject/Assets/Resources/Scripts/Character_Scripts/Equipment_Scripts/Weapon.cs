using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class for Weapon class Equipment. Inherits from Equipment.
/// </summary>
public class Weapon : Equipment
{
    /// <summary>
    /// Variables
    /// int range - The range in number of tiles for the Weapon.
    /// int attack - The attack value for the Weapon.
    /// bool ranged - Whether the Weapon is Ranged or Melee.
    /// int armor_pierce - How much armor the Weapon ignores when dealing damage.
    /// </summary>
    public string category { get; private set;  }
    public string scaling { get; private set; }
    public float[,] modifier { get; private set; }
    public float attack { get; private set; }
    public float pierce { get; private set; }
    public string passive { get; private set; }
    public string reaction { get; private set; }
    public string active1 { get; private set; }
    public string active2 { get; private set; }
    public string active3 { get; private set; }

    /// <summary>
    /// Takes a List of strings and returns a usable Weapon object.
    /// </summary>
    /// <param name="input">The list of weapon stats to create the Weapon.</param>
    /// <returns>The Weapon object.</returns>
    public static Weapon Parse(string[] input)
    {
        Weapon weapon = new Weapon();
        int mod_x_index = 0;
        int mod_y_index = 0;
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
                        weapon.name = values.TrimStart().TrimEnd();
                        category = weapon.name;
                        break;
                    case "scaling":
                        weapon.scaling = values.TrimStart().TrimEnd();
                        break;
                    case "area":
                        int x;
                        int y;
                        int.TryParse(values.Split('x')[0], out x);
                        int.TryParse(values.Split('x')[1], out y);
                        weapon.modifier = new float[x, y];
                        break;
                    case "mod":
                        foreach (string num in values.TrimStart().TrimEnd().Split(' '))
                        {
                            number = 0;
                            float.TryParse(num, out number);
                            weapon.modifier[mod_x_index, mod_y_index] = number;
                            mod_y_index++;
                        }
                        mod_y_index = 0;
                        mod_x_index++;
                        break;
                    case "attack":
                        number = 0;
                        float.TryParse(values.TrimStart().TrimEnd(), out number);
                        weapon.attack = number;
                        break;
                    case "weight":
                        number = 0;
                        float.TryParse(values.TrimStart().TrimEnd(), out number);
                        weapon.weight = number;
                        break;
                    case "pierce":
                        number = 0;
                        float.TryParse(values.TrimStart().TrimEnd(), out number);
                        weapon.pierce = number;
                        break;
                    case "Passive":
                        weapon.passive = values.TrimStart().TrimEnd();
                        break;
                    case "Reaction":
                        weapon.reaction = values.TrimStart().TrimEnd();
                        break;
                    case "Active1":
                        weapon.active1 = values.TrimStart().TrimEnd();
                        break;
                    case "Active2":
                        weapon.active2 = values.TrimStart().TrimEnd();
                        break;
                    case "Active3":
                        weapon.active3 = values.TrimStart().TrimEnd();
                        break;
                }
            }
        }
        weapon.actions[0] = weapon.active1;
        weapon.actions[1] = weapon.active2;
        weapon.actions[2] = weapon.active3;
        return weapon;
    }

    /// <summary>
    /// Used in conjustion with the Parse method to create a Dictionary of all known weapon types.
    /// </summary>
    /// <returns>A constructed Dictionary containing all known weapon types, using their names as the key.</returns>
    public static Dictionary<string, Weapon> Load_Weapons()
    {
        Dictionary<string, Weapon> weapon_types = new Dictionary<string, Weapon>();
        foreach (string file in System.IO.Directory.GetFiles("Assets/Resources/Equipment/Weapons/"))
        {
            string[] lines = System.IO.File.ReadAllLines(file);
            Weapon weapon = Parse(lines);
            if (weapon != null && weapon.name != "")
            {
                //Debug.Log("Added " + weapon.name);
                weapon_types.Add(weapon.name, weapon);
            }
        }
        return weapon_types;
    }

    /// <summary>
    /// An empty Constructor used to instantiate an epty Weapon object.
    /// </summary>
    public Weapon()
    {
        type = Equipment_Type.Weapon;
        name = "";
        attack = 0;
        pierce = 0;
        passive = "";
        reaction = "";
        active1 = "";
        active2 = "";
        active3 = "";
        actions = new string[3];  
}

    /// <summary>
    /// Constructor for the class. Takes a String and parses it to a Weapon_Type.
    /// </summary>
    /// <param name="str">A String for the type of Weapon to create</param>
    public Weapon(string str)
    {
        type = Equipment_Type.Weapon;
        durability = 100;
    }

    /// <summary>
    /// Constructor for the class. Takes a Weapon_Type and creates a weapon of that type.
    /// </summary>
    /// <param name="wep">Weapon Type of the weapon to create.</param>
    public Weapon(Weapon_Types wep)
    {
        type = Equipment_Type.Weapon;
        durability = 100;
        
    }
}
