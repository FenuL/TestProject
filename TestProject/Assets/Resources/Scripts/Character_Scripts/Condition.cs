using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Class to store conditions that can affect characters
/// </summary>
public class Condition
{
    /// <summary>
    /// Conditions - The enum storing all known types of conditions
    /// Conditions type - The type of condition of this particular condition. Used as key for Character condition dictionary.
    /// int duration - The number of turns a condition remains active
    /// int MAX_STACKS - A Dictionary of the maximum number of Conditions that can affect one target. For all Conditions.
    /// int MAX_DURATION - A Dictionary of the maximum duration of all Conditions. 
    /// Conditions UPGRADE - A Dictionary of what to upgrade the Condition to if stacks are exceeded.
    /// int power - The power of the condition; affects different things depending on the condition. Eg Bleed damage, or Vulnerability damage bonus.
    /// string attribute - Determines what attribute the condition affects (only relevant for Poison/Boost)
    /// </summary>

    public Conditions type { get; private set; }
    public int duration { get; private set; }
    public Dictionary<Conditions, int> MAX_STACKS { get; private set; }
    public Dictionary<Conditions, int> MAX_DURATION { get; private set; }
    public Dictionary<Conditions, Conditions> UPGRADE { get; private set; }
    public double power { get; private set; }
    public string attribute { get; private set; }

    /// <summary>
    /// Constructor for the class.
    /// </summary>
    /// <param name="new_type">string for a Type of Condition. Used as a key for the Character_Script condition Dictionary.</param>
    /// <param name="turn_duration">The number of turns a condition remains active</param>
    /// <param name="new_power">The power of the condition; affects different things depending on the condition. Eg Bleed damage, or Vulnerability damage bonus.</param>
    /// <param name="new_attribute">Determines what attribute the condition affects (only relevant for Poison/Boost)</param>
    public Condition(string new_type, int turn_duration, double new_power, string new_attribute)
    {
        Create_Dictionaries();

        if (new_type == Conditions.Bleed.ToString())
        {
            type = Conditions.Bleed;
            attribute = "NA";
        }
        else if (new_type == Conditions.Blight.ToString())
        {
            type = Conditions.Blight;
            attribute = "NA";
        }
        else if (new_type == Conditions.Blind.ToString())
        {
            type = Conditions.Blind;
            attribute = "NA";
        }
        else if (new_type == Conditions.Block.ToString())
        {
            type = Conditions.Block;
            attribute = "NA";
        }
        else if (new_type == Conditions.Boost.ToString())
        {
            type = Conditions.Boost;
            attribute = new_attribute;
        }
        else if (new_type == Conditions.Burn.ToString())
        {
            type = Conditions.Burn;
            attribute = "NA";
        }
        else if (new_type == Conditions.Clarity.ToString())
        {
            type = Conditions.Clarity;
            attribute = "NA";
        }
        else if (new_type == Conditions.Cleanse.ToString())
        {
            type = Conditions.Cleanse;
            attribute = "NA";
        }
        else if (new_type == Conditions.Confuse.ToString())
        {
            type = Conditions.Confuse;
            attribute = "NA";
        }
        else if (new_type == Conditions.Corrupt.ToString())
        {
            type = Conditions.Corrupt;
            attribute = "NA";
        }
        else if (new_type == Conditions.Cure.ToString())
        {
            type = Conditions.Cure;
            attribute = "NA";
        }
        else if (new_type == Conditions.Daze.ToString())
        {
            type = Conditions.Daze;
            attribute = "NA";
        }
        else if (new_type == Conditions.Drain.ToString())
        {
            type = Conditions.Drain;
            attribute = "NA";
        }
        else if (new_type == Conditions.Evade.ToString())
        {
            type = Conditions.Evade;
            attribute = "NA";
        }
        else if (new_type == Conditions.Freeze.ToString())
        {
            type = Conditions.Freeze;
            attribute = "NA";
        }
        else if (new_type == Conditions.Frostbite.ToString())
        {
            type = Conditions.Frostbite;
            attribute = "NA";
        }
        else if (new_type == Conditions.Haste.ToString())
        {
            type = Conditions.Haste;
            attribute = "NA";
        }
        else if (new_type == Conditions.Hemorrage.ToString())
        {
            type = Conditions.Hemorrage;
            attribute = "NA";
        }
        else if (new_type == Conditions.Immobilize.ToString())
        {
            type = Conditions.Immobilize;
            attribute = "NA";
        }
        else if (new_type == Conditions.Petrify.ToString())
        {
            type = Conditions.Petrify;
            attribute = "NA";
        }
        else if (new_type == Conditions.Poison.ToString())
        {
            type = Conditions.Poison;
            attribute = new_attribute;
        }
        else if (new_type == Conditions.Protection.ToString())
        {
            type = Conditions.Protection;
            attribute = "NA";
        }
        else if (new_type == Conditions.Purify.ToString())
        {
            type = Conditions.Purify;
            attribute = "NA";
        }
        else if (new_type == Conditions.Regen.ToString())
        {
            type = Conditions.Regen;
            attribute = "NA";
        }
        else if (new_type == Conditions.Restore.ToString())
        {
            type = Conditions.Restore;
            attribute = "NA";
        }
        else if (new_type == Conditions.Scorch.ToString())
        {
            type = Conditions.Scorch;
            attribute = "NA";
        }
        else if (new_type == Conditions.Slow.ToString())
        {
            type = Conditions.Slow;
            attribute = "NA";
        }
        else if (new_type == Conditions.Stun.ToString())
        {
            type = Conditions.Stun;
            attribute = "NA";
        }
        else if (new_type == Conditions.Vulnerability.ToString())
        {
            type = Conditions.Vulnerability;
            attribute = "NA";
        }
        else if (new_type == Conditions.Weakness.ToString())
        {
            type = Conditions.Weakness;
            attribute = "NA";
        }
        else
        {
            type = Conditions.None;
            attribute = "NA";
        }
        duration = turn_duration;
        power = new_power;
    }

    /// <summary>
    /// Constructor for the class.
    /// </summary>
    /// <param name="new_type">string for a Type of Condition. Used as a key for the Character_Script condition Dictionary.</param>
    /// <param name="turn_duration">The number of turns a condition remains active</param>
    /// <param name="new_power">The power of the condition; affects different things depending on the condition. Eg Bleed damage, or Vulnerability damage bonus.</param>
    /// <param name="new_attribute">Determines what attribute the condition affects (only relevant for Poison/Boost)</param>
    public Condition(Conditions new_type, int turn_duration, double new_power, string new_attribute)
    {
        Create_Dictionaries();
        type = new_type;
        duration = turn_duration;
        power = new_power;
        attribute = new_attribute;
    }

    /// <summary>
    /// Constructor for the class.
    /// </summary>
    /// <param name="condi">A Condition to copy over.</param>
    public Condition(Condition condi)
    {
        type = condi.type;
        duration = condi.duration;
        power = condi.power;
        attribute = condi.attribute;
        MAX_DURATION = condi.MAX_DURATION;
        MAX_STACKS = condi.MAX_STACKS;
        UPGRADE = condi.UPGRADE;
    }

    /// <summary>
    /// Constructor for class, creates a generic condition of a specific type given a string
    /// </summary>
    /// <param name="name">The name of the Condition to create</param>
    public Condition(string name)
    {
        Create_Dictionaries();
        Array array = Enum.GetValues(typeof(Condition_Shortcuts));
        Array array2 = Enum.GetValues(typeof(Conditions));
        foreach (Condition_Shortcuts condi in array)
        {
            if (condi.ToString() == name)
            {
                int num = (int)condi;
                type = (Conditions)array2.GetValue(num);
                power = 0;
                duration = 0;
                attribute = "NA";
                break;
            }
        }
        if (attribute != "NA") {
            
            foreach (Conditions condi in array2)
            {
                if (condi.ToString() == name)
                {
                    type = condi;
                    power = 0;
                    duration = 0;
                    attribute = "NA";
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Creates Dictionaries for the MAX_STACKS, MAX_DURATION and UPGRADE for all conditions.
    /// </summary>
    public void Create_Dictionaries()
    {
        MAX_STACKS = new Dictionary<Conditions, int>();
        MAX_DURATION = new Dictionary<Conditions, int>();
        UPGRADE = new Dictionary<Conditions, Conditions>();
        foreach (Conditions condi in Conditions.GetValues(typeof(Conditions)))
        {

            if (condi == Conditions.Bleed)
            {
                UPGRADE.Add(condi, Conditions.Hemorrage);
            }
            else if (condi == Conditions.Burn)
            {
                UPGRADE.Add(condi, Conditions.Scorch);
            }
            else if (condi == Conditions.Corrupt)
            {
                UPGRADE.Add(condi, Conditions.Blight);
            }
            else if (condi == Conditions.Frostbite)
            {
                UPGRADE.Add(condi, Conditions.Freeze);
            }
            else
            {
                UPGRADE.Add(condi, Conditions.None);
            }

            if (condi == Conditions.Bleed ||
                condi == Conditions.Boost ||
                condi == Conditions.Frostbite ||
                condi == Conditions.Corrupt ||
                condi == Conditions.Scorch)
            {
                MAX_STACKS.Add(condi, 5);
            }
            else if (condi == Conditions.Blight ||
                condi == Conditions.Burn ||
                condi == Conditions.Regen ||
                condi == Conditions.Hemorrage)
            {
                MAX_STACKS.Add(condi, 3);
            }
            else
            {
                MAX_STACKS.Add(condi, 1);
            }
            MAX_DURATION.Add(condi, 5);
        }
    }

    /// <summary>
    /// Decreases the duration of the condition and returns the new duration;
    /// </summary>
    /// <returns> Returns the duration left for the condition</returns>
    public int Progress()
    {
        duration -= 1;
        return duration;
    }
}
