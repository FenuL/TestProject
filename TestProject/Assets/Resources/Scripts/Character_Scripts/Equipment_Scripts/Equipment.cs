using UnityEngine;
using System.Collections;
using System;

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
    public enum Equipment_Type { Weapon, Armor, Accessory };
    public string name { get; protected set; }
    public Equipment_Type type { get; protected set; }
    public String[] actions { get; protected set; }
    public Equip_Effect[] effects { get; protected set; }
    public int durability { get; protected set; }
    public string description { get; protected set; }
    public float weight { get; protected set; }
    public SpriteRenderer sprite { get; protected set; }

    /// <summary>
    /// Converts the String parameters from the File into a double. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Double</param>
    /// <param name="obj">The Character_Script to use for converting the Accepted_Shortcuts into numbers</param>
    /// <returns></returns>
    public double Convert_To_Double(string input, Character_Script obj)
    {
        double output = 0.0;
        if (double.TryParse(input, out output))
        {
            return output;
        }
        else
        {
            //Remove acronyms from equation
            Array values = Enum.GetValues(typeof(Accepted_Shortcuts));
            foreach (Accepted_Shortcuts val in values)
            {
                if (input.Contains(val.ToString()))
                {
                    if (val.ToString() == "AUM")
                    {
                        input = input.Replace(val.ToString(), "" + obj.aura_max);
                    }
                    else if (val.ToString() == "AUC")
                    {
                        input = input.Replace(val.ToString(), "" + obj.aura_curr);
                    }
                    else if (val.ToString() == "APM")
                    {
                        input = input.Replace(val.ToString(), "" + obj.action_max);
                    }
                    else if (val.ToString() == "APC")
                    {
                        input = input.Replace(val.ToString(), "" + obj.action_curr);
                    }
                    else if (val.ToString() == "MPM")
                    {
                        input = input.Replace(val.ToString(), "" + obj.mana_max);
                    }
                    else if (val.ToString() == "MPC")
                    {
                        input = input.Replace(val.ToString(), "" + obj.mana_curr);
                    }
                    else if (val.ToString() == "CAM")
                    {
                        input = input.Replace(val.ToString(), "" + obj.canister_max);
                    }
                    else if (val.ToString() == "CAC")
                    {
                        input = input.Replace(val.ToString(), "" + obj.canister_curr);
                    }
                    else if (val.ToString() == "SPD")
                    {
                        input = input.Replace(val.ToString(), "" + obj.speed);
                    }
                    else if (val.ToString() == "STR")
                    {
                        input = input.Replace(val.ToString(), "" + obj.strength);
                    }
                    else if (val.ToString() == "DEX")
                    {
                        input = input.Replace(val.ToString(), "" + obj.dexterity);
                    }
                    else if (val.ToString() == "SPT")
                    {
                        input = input.Replace(val.ToString(), "" + obj.spirit);
                    }
                    else if (val.ToString() == "INI")
                    {
                        input = input.Replace(val.ToString(), "" + obj.initiative);
                    }
                    else if (val.ToString() == "VIT")
                    {
                        input = input.Replace(val.ToString(), "" + obj.vitality);
                    }
                    else if (val.ToString() == "LVL")
                    {
                        input = input.Replace(val.ToString(), "" + obj.level);
                    }
                    else if (val.ToString() == "WPR")
                    {
                        input = input.Replace(val.ToString(), "" + obj.weapon.modifier.Length/2);
                    }
                    else if (val.ToString() == "WPD")
                    {
                        input = input.Replace(val.ToString(), "" + obj.weapon.attack);
                    }
                    else if (val.ToString() == "WPN")
                    {
                        input = input.Replace(val.ToString(), "" + obj.weapon.name);
                    }
                    else if (val.ToString() == "ARM")
                    {
                        input = input.Replace(val.ToString(), "" + obj.armor.armor);
                    }
                    else if (val.ToString() == "WGT")
                    {
                        input = input.Replace(val.ToString(), "" + obj.armor.weight);
                    }
                    else if (val.ToString() == "MOC")
                    {
                        input = input.Replace(val.ToString(), "" + obj.action_curr);
                    }
                    else if (val.ToString() == "DST")
                    {
                        input = input.Replace(val.ToString(), "" + obj.aura_max);
                    }
                    else if (val.ToString() == "NUL")
                    {
                        input = input.Replace(val.ToString(), "" + 0.0);
                    }
                }
            }
            //try to convert to double after converting
            if (double.TryParse(input, out output))
            {
                return output;
            }
            else if (input.Contains("+") ||
                input.Contains("/") ||
                input.Contains("*") ||
                input.Contains("-") ||
                input.Contains("^") ||
                input.Contains("(") ||
                input.Contains(")"))
            {
                return Parse_Equation(input);
            }
            return -1.0;
        }

    }

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
}
