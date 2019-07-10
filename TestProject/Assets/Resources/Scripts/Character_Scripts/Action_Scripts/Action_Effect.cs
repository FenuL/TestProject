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
    /// Target target - The target of the effect. Either self (will affect character performing action), target (will affect target of action), or path (will affect the action's path).
    /// string[] checks - The checks to perform before activating the effect.
    /// string[] values - A list of strings that detail the effect of the Action.
    /// </summary>
    public enum Types { Move, Damage, Heal, Status, Effect, Elevate, Enable, Pass }
    public enum Target { self, target, path }

    public Types type { get; private set; }
    public Target target { get; private set; }
    public string[] checks { get; private set; }
    public string[] values { get; private set; }

    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="input">A String of several entries separated by spaces (" ").</param>
    public Action_Effect(string input)
    {
        string[] split_input = input.TrimStart().Split(' ');
        if (split_input.Length >= 1) {
            string target_string = split_input[0];
            Array targets = Enum.GetValues(typeof(Target));
            foreach (Target targ in targets)
            {
                if (target_string.Contains(targ.ToString()) || 
                    target_string.Contains(targ.ToString().ToLower()))
                {
                    target = targ;
                }
            }
        }

        if (split_input.Length >= 2)
        {
            string[] check_string = split_input[1].Split(',');
            checks = new string[check_string.Length];
            for (int x = 0; x < checks.Length; x++)
            {
                //Debug.Log(check_string[x]);
                if (check_string[x] == "NUL")
                {
                    checks[x] = "CHK_TDST_LTQ_CRNG";
                }
                else
                {
                    checks[x] = check_string[x];
                    //Debug.Log("Check " + x + " " + checks[x]);
                }
            }
        }

        if (split_input.Length >= 3)
        {
            string type_string = split_input[2];
            Array types = Enum.GetValues(typeof(Types));

            foreach (Types ty in types)
            {
                //Debug.Log(type_string + " and " + ty.ToString());
                if (type_string.Contains(ty.ToString()) || 
                    type_string.Contains(ty.ToString().ToLower()))
                {
                    type = ty;
                    //Debug.Log("type:" + type);
                }
            }
        }

        if (split_input.Length >= 4)
        {
            string value_string = split_input[3];
            values = new string[value_string.Split(',').Length];
            for (int x = 0; x < values.Length; x++)
            {
                values[x] = value_string.Split(',')[x];
                //Debug.Log("Values " + x + " " + values[x]);
            }
        }else
        {
            values = new string[0];
        }
    }
}
