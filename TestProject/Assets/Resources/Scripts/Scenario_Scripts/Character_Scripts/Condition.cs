using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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

    [JsonProperty] public Conditions type { get; private set; }
    [JsonProperty] public int duration { get; private set; }
    [JsonProperty] public double power { get; private set; }
    [JsonProperty] public string attribute { get; private set; }

    //Dictionaries to define maximums for each Condition.
    public static readonly Dictionary<Conditions, int> MAX_STACKS = new Dictionary<Conditions, int>
    {
        { Conditions.Bleed, 5 },
        { Conditions.Boost, 5 },
        { Conditions.Frostbite, 5 },
        { Conditions.Corrupt, 5 },
        { Conditions.Scorch, 5 },
        { Conditions.Blight, 3 },
        { Conditions.Burn, 3 },
        { Conditions.Regen, 3 },
        { Conditions.Hemorrage, 3 },
        { Conditions.Block, 3 },
        { Conditions.Evade, 3 },
        { Conditions.Blind, 1 },
        { Conditions.Clarity, 1 },
        { Conditions.Cleanse, 1 },
        { Conditions.Confuse, 1 },
        { Conditions.Cure, 1 },
        { Conditions.Daze, 1 },
        { Conditions.Drain, 1 },
        { Conditions.Freeze, 1 },
        { Conditions.Haste, 1 },
        { Conditions.Immobilize, 1 },
        { Conditions.Petrify, 1 },
        { Conditions.Poison, 1 },
        { Conditions.Protection, 1 },
        { Conditions.Purify, 1 },
        { Conditions.Restore, 1 },
        { Conditions.Slow, 1 },
        { Conditions.Stun, 1 },
        { Conditions.Vulnerability, 1 },
        { Conditions.Weakness, 1 }
    };
    public static readonly Dictionary<Conditions, int> MAX_DURATION = new Dictionary<Conditions, int>
    {
        { Conditions.Bleed, 5 },
        { Conditions.Burn, 5 },
        { Conditions.Corrupt, 5 },
        { Conditions.Frostbite, 5 },
        { Conditions.Blight, 5 },
        { Conditions.Blind, 5 },
        { Conditions.Block, 5 },
        { Conditions.Boost, 5 },
        { Conditions.Clarity, 5 },
        { Conditions.Cleanse, 5 },
        { Conditions.Confuse, 5 },
        { Conditions.Cure, 5 },
        { Conditions.Daze, 5 },
        { Conditions.Drain, 5 },
        { Conditions.Evade, 5 },
        { Conditions.Freeze, 5 },
        { Conditions.Haste, 5 },
        { Conditions.Immobilize, 5 },
        { Conditions.Petrify, 5 },
        { Conditions.Poison, 5 },
        { Conditions.Protection, 5 },
        { Conditions.Purify, 5 },
        { Conditions.Regen, 5 },
        { Conditions.Restore, 5 },
        { Conditions.Scorch, 5 },
        { Conditions.Slow, 5 },
        { Conditions.Stun, 5 },
        { Conditions.Vulnerability, 5 },
        { Conditions.Weakness, 5 }
    };
    public static readonly Dictionary<Conditions, Conditions> UPGRADE = new Dictionary<Conditions, Conditions>
    {
        { Conditions.Bleed, Conditions.Hemorrage },
        { Conditions.Burn, Conditions.Scorch },
        { Conditions.Corrupt, Conditions.Blight },
        { Conditions.Frostbite, Conditions.Freeze },
        { Conditions.Blight, Conditions.None },
        { Conditions.Blind, Conditions.None },
        { Conditions.Block, Conditions.None },
        { Conditions.Boost, Conditions.None },
        { Conditions.Clarity, Conditions.None },
        { Conditions.Cleanse, Conditions.None },
        { Conditions.Confuse, Conditions.None },
        { Conditions.Cure, Conditions.None },
        { Conditions.Daze, Conditions.None },
        { Conditions.Drain, Conditions.None },
        { Conditions.Evade, Conditions.None },
        { Conditions.Freeze, Conditions.None },
        { Conditions.Haste, Conditions.None },
        { Conditions.Immobilize, Conditions.None },
        { Conditions.Petrify, Conditions.None },
        { Conditions.Poison, Conditions.None },
        { Conditions.Protection, Conditions.None },
        { Conditions.Purify, Conditions.None },
        { Conditions.Regen, Conditions.None },
        { Conditions.Restore, Conditions.None },
        { Conditions.Scorch, Conditions.None },
        { Conditions.Slow, Conditions.None },
        { Conditions.Stun, Conditions.None },
        { Conditions.Vulnerability, Conditions.None },
        { Conditions.Weakness, Conditions.None }
    };

    /// <summary>
    /// Constructor the class used to turn it into JSON and back.
    /// </summary>
    /// <param name="new_type">Condition Type</param>
    /// <param name="new_duration">Duration for the Condition</param>
    /// <param name="new_power">Power of the Condition</param>
    /// <param name="new_attribute">Attribute for the Condition</param>
    [JsonConstructor]
    public Condition(Conditions new_type, int new_duration, double new_power, string new_attribute)
    {
        type = new_type;
        duration = new_duration;
        power = new_power;
        attribute = new_attribute;
        if (type != Conditions.Poison || type != Conditions.Boost)
        {
            attribute = "NA";
        }
    }


    /// <summary>
    /// Constructor for the class.
    /// </summary>
    /// <param name="new_type">string for a Type of Condition. Used as a key for the Character_Script condition Dictionary.</param>
    /// <param name="turn_duration">The number of turns a condition remains active</param>
    /// <param name="new_power">The power of the condition; affects different things depending on the condition. Eg Bleed damage, or Vulnerability damage bonus.</param>
    /// <param name="new_attribute">Determines what attribute the condition affects (only relevant for Poison/Boost)</param>
    public Condition(string new_type, int turn_duration, double new_power, string new_attribute)
    {

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
    /// <param name="condi">A Condition to copy over.</param>
    public Condition(Condition condi)
    {
        type = condi.type;
        duration = condi.duration;
        power = condi.power;
        attribute = condi.attribute;
    }

    /// <summary>
    /// Constructor for class, creates a generic condition of a specific type given a string
    /// </summary>
    /// <param name="name">The name of the Condition to create</param>
    public Condition(string name)
    {
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
    /// Decreases the duration of the condition and returns the new duration;
    /// </summary>
    /// <returns> Returns the duration left for the condition</returns>
    public int Progress()
    {
        duration -= 1;
        return duration;
    }
}
