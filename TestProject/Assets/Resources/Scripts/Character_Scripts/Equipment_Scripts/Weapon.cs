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
    /// </summary>
    public int range;
    public int attack;
    public bool ranged;

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
                break;
            case "Rifle":
                name = Weapon_Types.Rifle.ToString();
                range = 4;
                attack = 3;
                ranged = true;
                weight = 1;
                break;
            case "Spear":
                name = Weapon_Types.Spear.ToString();
                range = 2;
                attack = 2;
                ranged = false;
                weight = 1;
                break;
            case "Sniper":
                name = Weapon_Types.Sniper.ToString();
                range = 6;
                attack = 5;
                ranged = true;
                weight = 3;
                break;
            case "Pistol":
                name = Weapon_Types.Pistol.ToString();
                range = 3;
                attack = 2;
                ranged = true;
                weight = 0.5;
                break;
            case "Claws":
                name = Weapon_Types.Claws.ToString();
                range = 1;
                attack = 10;
                ranged = false;
                weight = 4;
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
                break;
            case Weapon_Types.Rifle:
                name = Weapon_Types.Rifle.ToString();
                range = 4;
                attack = 3;
                ranged = true;
                weight = 1;
                break;
            case Weapon_Types.Spear:
                name = Weapon_Types.Spear.ToString();
                range = 2;
                attack = 2;
                ranged = false;
                weight = 1;
                break;
            case Weapon_Types.Sniper:
                name = Weapon_Types.Sniper.ToString();
                range = 6;
                attack = 5;
                ranged = true;
                weight = 3;
                break;
            case Weapon_Types.Pistol:
                name = Weapon_Types.Pistol.ToString();
                range = 3;
                attack = 2;
                ranged = true;
                weight = 0.5;
                break;
            case Weapon_Types.Claws:
                name = Weapon_Types.Claws.ToString();
                range = 1;
                attack = 10;
                ranged = false;
                weight = 4;
                break;
            default:
                break;
        }
    }
}
