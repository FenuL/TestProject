using UnityEngine;
using System.Collections;

/// <summary>
/// Class for Armor class Equipment. Inherits from Equipment. 
/// </summary>
public class Armor : Equipment
{
    /// <summary>
    /// Class Constructor
    /// </summary>
    /// <param name="str">A String for the type of Armor to create. Parses the string to an Armor_Type.</param>
    public Armor(string str)
    {
        type = Equipment_Type.Armor;
        durability = 100;
        switch (str)
        {
            case "Light":
                name = Armor_Types.Light.ToString();
                armor = -1;
                weight = 0;
                effects = new Equip_Effect[2];
                effects[0] = new Equip_Effect(Character_Stats.speed, 1);
                effects[1] = new Equip_Effect(Character_Stats.dexterity, 1);
                actions = new string[6];
                actions[0] = "Blink";
                actions[1] = "Cross";
                actions[2] = "Ring";
                actions[3] = "Cone";
                actions[4] = "Raise";
                actions[5] = "Lower";
                break;
            case "Medium":
                name = Armor_Types.Medium.ToString();
                armor = 2;
                weight = 1;
                effects = new Equip_Effect[2];
                effects[0] = new Equip_Effect(Character_Stats.strength, 1);
                effects[1] = new Equip_Effect(Character_Stats.coordination, 1);
                actions = new string[5];
                actions[0] = "Cross";
                actions[1] = "Ring";
                actions[2] = "Cone";
                actions[3] = "Raise";
                actions[4] = "Lower";
                break;
            case "Heavy":
                name = Armor_Types.Heavy.ToString();
                armor = 5;
                weight = 2;
                effects = new Equip_Effect[2];
                effects[0] = new Equip_Effect(Character_Stats.vitality, 1);
                effects[1] = new Equip_Effect(Character_Stats.spirit, 1);
                actions = new string[6];
                actions[0] = "Channel";
                actions[1] = "Cross";
                actions[2] = "Ring";
                actions[3] = "Cone";
                actions[4] = "Raise";
                actions[5] = "Lower";
                break;
        }
    }

    /// <summary>
    /// Class Constructor Overload. Takes a Type of Armor instead of a String.
    /// </summary>
    /// <param name="ar">Type of Armor to create. </param>
    public Armor(Armor_Types ar)
    {
        type = Equipment_Type.Armor;
        durability = 100;
        switch (ar)
        {
            case Armor_Types.Light:
                name = Armor_Types.Light.ToString();
                armor = -1;
                weight = 0;
                effects = new Equip_Effect[2];
                effects[0] = new Equip_Effect(Character_Stats.speed, 1);
                effects[1] = new Equip_Effect(Character_Stats.dexterity, 1);
                actions = new string[6];
                actions[0] = "Blink";
                actions[1] = "Cross";
                actions[2] = "Ring";
                actions[3] = "Cone";
                actions[4] = "Raise";
                actions[5] = "Lower";
                break;
            case Armor_Types.Medium:
                name = Armor_Types.Medium.ToString();
                armor = 2;
                weight = 1;
                effects = new Equip_Effect[2];
                effects[0] = new Equip_Effect(Character_Stats.strength, 1);
                effects[1] = new Equip_Effect(Character_Stats.coordination, 1);
                actions = new string[5];
                actions[0] = "Cross";
                actions[1] = "Ring";
                actions[2] = "Cone";
                actions[3] = "Raise";
                actions[4] = "Lower";
                break;
            case Armor_Types.Heavy:
                name = Armor_Types.Heavy.ToString();
                armor = 5;
                weight = 2;
                effects = new Equip_Effect[2];
                effects[0] = new Equip_Effect(Character_Stats.vitality, 1);
                effects[1] = new Equip_Effect(Character_Stats.spirit, 1);
                actions = new string[6];
                actions[0] = "Channel";
                actions[1] = "Cross";
                actions[2] = "Ring";
                actions[3] = "Cone";
                actions[4] = "Raise";
                actions[5] = "Lower";
                break;
        }
    }
}
