using UnityEngine;
using System.Collections;

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
    public int range { get; private set; }
    public int attack { get; private set; }
    public bool ranged { get; private set; }
    public int armor_pierce { get; private set; }

    /// <summary>
    /// Constructor for the class. Takes a String and parses it to a Weapon_Type.
    /// </summary>
    /// <param name="str">A String for the type of Weapon to create</param>
    public Weapon(string str)
    {
        type = Equipment_Type.Weapon;
        durability = 100;
        switch (str)
        {
            case "Sword":
                name = Weapon_Types.Sword.ToString();
                range = 1;
                attack = 2;
                weight = 0.5;
                ranged = false;
                armor_pierce = 0;
                actions = new string[1];
                actions[0] = "Bleed";
                break;
            case "Rifle":
                name = Weapon_Types.Rifle.ToString();
                range = 4;
                attack = 3;
                ranged = true;
                weight = 1;
                armor_pierce = 1;
                actions = new string[1];
                actions[0] = "Stun";
                break;
            case "Spear":
                name = Weapon_Types.Spear.ToString();
                range = 2;
                attack = 2;
                ranged = false;
                weight = 1;
                armor_pierce = 1;
                break;
            case "Sniper":
                name = Weapon_Types.Sniper.ToString();
                range = 6;
                attack = 5;
                ranged = true;
                weight = 3;
                armor_pierce = 2;
                actions = new string[1];
                actions[0] = "Stun";
                break;
            case "Pistol":
                name = Weapon_Types.Pistol.ToString();
                range = 3;
                attack = 2;
                ranged = true;
                weight = 0.5;
                armor_pierce = 0;
                actions = new string[1];
                actions[0] = "Stun";
                break;
            case "Claws":
                name = Weapon_Types.Claws.ToString();
                range = 1;
                attack = 10;
                ranged = false;
                weight = 4;
                armor_pierce = 0;
                actions = new string[1];
                actions[0] = "Bleed";
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Constructor for the class. Takes a Weapon_Type and creates a weapon of that type.
    /// </summary>
    /// <param name="wep">Weapon Type of the weapon to create.</param>
    public Weapon(Weapon_Types wep)
    {
        type = Equipment_Type.Weapon;
        durability = 100;
        switch (wep)
        {
            case Weapon_Types.Sword:
                name = Weapon_Types.Sword.ToString();
                range = 1;
                attack = 2;
                weight = 0.5;
                ranged = false;
                armor_pierce = 0;
                actions = new string[1];
                actions[0] = "Bleed";
                break;
            case Weapon_Types.Rifle:
                name = Weapon_Types.Rifle.ToString();
                range = 4;
                attack = 3;
                ranged = true;
                weight = 1;
                armor_pierce = 1;
                actions = new string[1];
                actions[0] = "Stun";
                break;
            case Weapon_Types.Spear:
                name = Weapon_Types.Spear.ToString();
                range = 2;
                attack = 2;
                ranged = false;
                weight = 1;
                armor_pierce = 1;
                break;
            case Weapon_Types.Sniper:
                name = Weapon_Types.Sniper.ToString();
                range = 6;
                attack = 5;
                ranged = true;
                weight = 3;
                armor_pierce = 2;
                actions = new string[1];
                actions[0] = "Stun";
                break;
            case Weapon_Types.Pistol:
                name = Weapon_Types.Pistol.ToString();
                range = 3;
                attack = 2;
                ranged = true;
                weight = 0.5;
                armor_pierce = 0;
                actions = new string[1];
                actions[0] = "Stun";
                break;
            case Weapon_Types.Claws:
                name = Weapon_Types.Claws.ToString();
                range = 1;
                attack = 10;
                ranged = false;
                weight = 4;
                armor_pierce = 0;
                actions = new string[1];
                actions[0] = "Bleed";
                break;
            default:
                break;
        }
    }
}
