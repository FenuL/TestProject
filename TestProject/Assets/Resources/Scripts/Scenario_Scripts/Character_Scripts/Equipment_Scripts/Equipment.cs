using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
using System.IO;
using Newtonsoft.Json;

/// <summary>
/// Class for storing information of Character Equipment
/// </summary>
[Serializable]
public class Equipment
{
    /// <summary>
    /// enum Equipment_Type - Lists all possible Equipment types.
    /// enum Armor_Types - Lists all possible Armor types.
    /// enum Weapon_Types - Lists all possible weapon types.
    /// string name - The name of the piece of Equipment
    /// Equipment_Type type - The type of Equipment
    /// String[] actions - The actions given by the Equipment
    /// Equip_Effect[] effects - The effects of equipping this piece of Equipment
    /// int durability - The durability of this piece of Equipment
    /// string description - The description for this piece of Equipment
    /// double weight - The weight of the Equipment. Affects Character speed
    /// int armor - The armor given by this Equipment. Affects Character damage taken.
    /// SpriteRenderer sprite - The sprite for the this piece of Equipment
    /// </summary>
    /// 
    private static string WEAPON_FILEPATH = "Assets/Resources/Equipment/Weapons/";
    private static string ARMOR_FILEPATH = "Assets/Resources/Equipment/Armors/";
    private static string ACCESSORY_FILEPATH = "Assets/Resources/Equipment/Accessories/";
    public enum Equipment_Type { Weapon, Armor, Accessory };
    [JsonProperty] public string equip_name { get; protected set; }
    [JsonProperty] public Equipment_Type type { get; protected set; }
    [JsonProperty] public string[] action_names { get; protected set; }
    [JsonProperty] public string image { get; protected set; }
    [JsonProperty] public string description { get; protected set; }
    [JsonProperty] public float weight { get; protected set; }
    [JsonProperty] public string sprite { get; protected set; }

    /// <summary>
    /// Parses a String Equation and computes it. Used by the Convert_To_Double Function to fully solve equations.
    /// </summary>
    /// <param name="input">An Equation to parse out.</param>
    /// <returns></returns>
    public double Parse_Equation(string input)
    {
        //Debug.Log("Parsing: " + input);
        //base case, can we convert to double?
        double output;
        if (double.TryParse(input, out output))
        {
            return output;
        }
        //otherwise, we have work to do
        else
        {
            //Order of operations is reversed because it's a stack.
            //resolve parentheses
            if (input.Contains("(") || input.Contains(")"))
            {
                string[] split = input.Split(new char[] { '(' }, 2);
                string[] split2 = split[1].Split(new char[] { ')' }, 2);
                double result = Parse_Equation(split2[0]);
                return Parse_Equation("" + split[0] + result + split2[1]);
            }
            //resolve addition
            if (input.Contains("+"))
            {
                string[] split = input.Split(new char[] { '+' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) + Parse_Equation(split[1])));
            }
            //resolve subtraction
            if (input.Contains("-"))
            {
                string[] split = input.Split(new char[] { '-' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) - Parse_Equation(split[1])));
            }
            //resolve multiplication
            if (input.Contains("*"))
            {
                string[] split = input.Split(new char[] { '*' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) * Parse_Equation(split[1])));
            }
            //resolve division
            if (input.Contains("/"))
            {
                string[] split = input.Split(new char[] { '/' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) / Parse_Equation(split[1])));
            }
            //resolve exponents
            if (input.Contains("^"))
            {
                string[] split = input.Split(new char[] { '^' }, 2);
                return Parse_Equation("" + (Mathf.Pow((float)Parse_Equation(split[0]), (float)Parse_Equation(split[1]))));
            }
        }
        return output;
    }

    /// <summary>
    /// A class for handling how Equipment gives different Effects when Equipped by a Character.
    /// </summary>
    public class Equip_Effect
    {
        /// <summary>
        /// Character_Script.Character_Stats stat - the Character stat that will be modified
        /// int effect - The amount by which to affect the Character stat
        /// </summary>
        public Character_Stats stat { get; private set; }
        public int effect { get; private set; }

        /// <summary>
        /// Constructor for the class
        /// </summary>
        /// <param name="st">The Character stat to affect</param>
        /// <param name="eff">The integer by which to affect the Character stat</param>
        public Equip_Effect(Character_Stats st, int eff)
        {
            stat = st;
            effect = eff;
        }
    }

    /// <summary>
    /// Creates a file with the appropriate data in the correct place depending on location type.
    /// </summary>
    public void Export_Data()
    {
        string output = JsonUtility.ToJson(this, true);
        Debug.Log(output);
        Weapon data2 = JsonUtility.FromJson<Weapon>(output);
        //Debug.Log("character " + data2.Get_Tile_Grid().Get_Tiles()[0][0].Get_Character());
        if (data2.description != null)
        {
            Debug.Log("NOT NULL");
        }

        string path = "";
        if (type == Equipment_Type.Weapon)
        { 
            path = WEAPON_FILEPATH + equip_name + ".txt";
        }else if (type == Equipment_Type.Armor)
        {
            path = ARMOR_FILEPATH + equip_name + ".txt";
        }
        else if (type == Equipment_Type.Accessory)
        {
            path = ACCESSORY_FILEPATH + equip_name + ".txt";
        }

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(output);
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
    }
}
