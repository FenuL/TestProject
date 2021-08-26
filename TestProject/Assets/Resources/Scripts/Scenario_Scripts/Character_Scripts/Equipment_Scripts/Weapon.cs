using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

/// <summary>
/// Class for Weapon class Equipment. Inherits from Equipment.
/// </summary>
[Serializable]
public class Weapon : Equipment
{
    /// <summary>
    /// Variables
    /// int range - The range in number of tiles for the Weapon.
    /// int attack - The attack value for the Weapon.
    /// bool ranged - Whether the Weapon is Ranged or Melee.
    /// int armor_pierce - How much armor the Weapon ignores when dealing damage.
    /// </summary>
    [JsonProperty] public float[,] modifier { get; protected set; }
    [JsonProperty] public int damage { get; protected set; }
    [JsonProperty] public float pierce { get; protected set; }

    /// <summary>
    /// JSON Constructor for Weapons. Used to convert to JSON to a Weapon class.
    /// </summary>
    /// <param name="new_equip_name"></param>
    /// <param name="new_type"></param>
    /// <param name="new_action_names"></param>
    /// <param name="new_image"></param>
    /// <param name="new_description"></param>
    /// <param name="new_weight"></param>
    /// <param name="new_sprite"></param>
    /// <param name="new_modifier"></param>
    /// <param name="new_damage"></param>
    /// <param name="new_pierce"></param>
    [JsonConstructor]
    public Weapon(
        string new_equip_name,
        Equipment_Type new_type,
        string[] new_action_names,
        string new_image,
        string new_description,
        float new_weight,
        string new_sprite,
        float[,] new_modifier,
        int new_damage,
        float new_pierce)
    {
        equip_name = new_equip_name;
        type = new_type;
        action_names = new_action_names;
        image = new_image;
        description = new_description;
        weight = new_weight;
        sprite = new_sprite;
        modifier = new_modifier;
        damage = new_damage;
        pierce = new_pierce;
    }

    public static Weapon ParseJSON(string json)
    {
        Weapon weapon = JsonUtility.FromJson<Weapon>(json);
        return weapon;
    }

    /// <summary>
    /// Takes a List of strings and returns a usable Weapon object.
    /// </summary>
    /// <param name="input">The list of weapon stats to create the Weapon.</param>
    /// <returns>The Weapon object.</returns>
/*    public static Weapon Parse(string[] input)
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
                        weapon.equip_name = values.TrimStart().TrimEnd();
                        category = weapon.equip_name;
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
        weapon.actions[3] = weapon.reaction;
        return weapon;
    }*/

    /// <summary>
    /// Used in conjustion with the Parse method to create a Dictionary of all known weapon types.
    /// </summary>
    /// <returns>A constructed Dictionary containing all known weapon types, using their names as the key.</returns>
    public static Dictionary<string, Weapon> Load_Weapons()
    {
        Dictionary<string, Weapon> weapon_types = new Dictionary<string, Weapon>();
        foreach (string file in System.IO.Directory.GetFiles("Assets/Resources/Equipment/Weapons/"))
        {
            //string[] lines = System.IO.File.ReadAllLines(file);
            Weapon weapon = null;
            if (file.EndsWith(".json"))
            {
                weapon = ParseJSON(System.IO.File.ReadAllText(file));
                //string json = JsonUtility.ToJson(weapon);
                //Debug.Log("Weapon:" + json);
            }
            if (weapon != null && weapon.equip_name != "")
            {
                //Debug.Log("Added " + weapon.equip_name);
                if (!weapon_types.ContainsKey(weapon.equip_name))
                {
                    weapon_types.Add(weapon.equip_name, weapon);
                }
                else
                {
                    Debug.Log(weapon.equip_name + " already exists!");
                }
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
        equip_name = "";
        damage = 0;
        pierce = 0;
        action_names = new string[4];  
}

    /// <summary>
    /// Constructor for the class. Takes a String and parses it to a Weapon_Type.
    /// </summary>
    /// <param name="str">A String for the type of Weapon to create</param>
    public Weapon(string str)
    {
        type = Equipment_Type.Weapon;
    }

    /// <summary>
    /// Constructor for the class. Takes a Weapon_Type and creates a weapon of that type.
    /// </summary>
    /// <param name="wep">Weapon Type of the weapon to create.</param>
    public Weapon(Weapon_Types wep)
    {
        type = Equipment_Type.Weapon;
        
    }


    /// <summary>
    /// Constructor for the class. Used for creating weapon data files.
    /// </summary>
    /// <param name="nam">Name of the weapon.</param>
    /// <param name="desc">Description of the weapon.</param>
    /// <param name="acts">Actions granted by the weapon</param>
    /// <param name="spri">The spritesheet for the Weapon.</param>
    /// <param name="img">The image for the Weapon.</param>
    /// <param name="wei">The Weapon's innate weight characteristic.</param>
    /// <param name="dam">The Weapon's innate damage modifier.</param>
    /// <param name="pier">The Weapon's innate pierce modifier.</param>
    public Weapon(string nam, string desc, string[] acts, string spri, string img, float wei, int dam, float pier)
    {
        equip_name = nam;
        description = desc;
        action_names = acts;
        sprite = spri;
        image = img;
        modifier = new float[,] { 
            { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f },
            { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, 
            { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, 
            { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f }, 
            { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f } };
        weight = wei;
        damage = dam;
        pierce = pier;
        type = Equipment_Type.Weapon;
    }


}
