using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class to parse out the effect of an Action from the action list
/// </summary>
public class Action_Effect
{
    /// <summary>
    /// Variables: 
    /// enum Types - All the various Types of Action effects.
    ///     Move - Moves Character(s) in the area to a different Tile.
    ///     Damage - Deals damage to Character(s) in the area.
    ///     Heal - Restores Aura to Character(s) in the area.
    ///     Status - Inflicts Condition(s) or Character(s) in the area.
    ///     Elevate - Raises or Lowers the height of Tiles.
    ///     Enable - Enables or Disables certain Actions.
    ///     Pass - End a Character's Turn. 
    /// Type type - The Type of Action
    /// string[] value - A list of strings that detail the effect of the Action.
    /// </summary>
    public enum Types { Move, Damage, Heal, Status, Elevate, Enable, Pass }

    public Types type { get; private set; }
    public string[] value { get; private set; }

    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="input">A String of several entries separated by spaces (" ").</param>
    public Action_Effect(string input)
    {
        string type_string = input.TrimStart().Split(' ')[0];
        value = new string[input.Split(' ').Length];
        Array types = Enum.GetValues(typeof(Types));

        foreach (Types ty in types)
        {
            //Debug.Log(type_string + " and " + ty.ToString());
            if (type_string.Contains(ty.ToString()))
            {
                type = ty;
                //Debug.Log("type:" + type);
            }
        }
        int x = 0;
        while (x < input.Split(' ').Length - 1)
        {
            value[x] = input.Split(' ')[x + 1];
            x++;
        }
    }
}
