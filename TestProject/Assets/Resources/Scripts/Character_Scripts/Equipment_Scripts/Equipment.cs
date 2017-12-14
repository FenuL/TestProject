using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class for storing information of Character Equipment
/// </summary>
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
    public enum Armor_Types { Light, Medium, Heavy };
    public enum Weapon_Types { Sword, Rifle, Spear, Sniper, Pistol, Claws };
    public string name { get; protected set; }
    public Equipment_Type type { get; protected set; }
    public String[] actions { get; protected set; }
    public Equip_Effect[] effects { get; protected set; }
    public int durability { get; protected set; }
    public string description { get; protected set; }
    public double weight { get; protected set; }
    public int armor { get; protected set; }
    public SpriteRenderer sprite { get; protected set; }

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
