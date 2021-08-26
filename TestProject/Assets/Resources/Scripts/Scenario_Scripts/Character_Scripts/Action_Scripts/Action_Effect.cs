using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Class to parse out the effect of an Action from the action list
/// </summary>
[Serializable]
public class Action_Effect
{
    /// <summary>
    /// Variables: 
    /// enum Types - All the various Types of Action effects.
    ///     Orient - Changes the Character(s) orientation.
    ///     Move - Moves Character(s) in the area to a different Tile.
    ///     Damage - Deals damage to Character(s) in the area.
    ///     Heal - Restores Aura to Character(s) in the area.
    ///     Status - Inflicts Condition(s) or Character(s) in the area.
    ///     Elevate - Raises or Lowers the height of Tiles.
    ///     Enable - Enables or Disables certain Actions.
    ///     Pass - End a Character's Turn. 
    /// Type type - The Type of Action
    /// float advantage - The amount of advantage required to start the effect.
    /// Target target - The target of the effect. Either self (will affect character performing action), target (will affect target of action), or path (will affect the action's path).
    /// string[] checks - The checks to perform before activating the effect.
    /// string[] values - A list of strings that detail the effect of the Action.
    /// </summary>
    public enum Affects { Self, Target, Path }

    [JsonProperty]
    public Effect_Types type { get; private set; }
    [JsonProperty]
    public float req_advantage { get; private set; }
    [JsonProperty]
    public Affects affects { get; private set; }
    [JsonProperty]
    public string[] checks { get; private set; }
    [JsonProperty]
    public string[] values { get; private set; }
    [JsonProperty]
    public int min_target_limit { get; private set; }
    [JsonProperty]
    public int max_target_limit { get; private set; }

    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="input">A String of several entries separated by spaces (" ").</param>
    public Action_Effect(string input)
    {
        string[] split_input = input.TrimStart().Split(' ');
        if (split_input.Length >= 1)
        {
            string target_string = split_input[0];
            Array targets = Enum.GetValues(typeof(Affects));
            foreach (Affects targ in targets)
            {
                if (target_string.Contains(targ.ToString()) ||
                    target_string.Contains(targ.ToString().ToLower()))
                {
                    affects = targ;
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
                checks[x] = check_string[x];
                /*if (check_string[x] == "NULL")
                {
                    checks[x] = "CHK_TDST_LTQ_CRNG";
                }
                else
                {
                    checks[x] = check_string[x];
                    //Debug.Log("Check " + x + " " + checks[x]);
                }*/
            }
        }
        if (split_input.Length >= 3)
        {
            string[] target_limit_string = split_input[2].Split(',');
            //Debug.Log(target_limit_string[0]); 
            int min = 0;
            int max = 0;

            int.TryParse(target_limit_string[0], out min);
            int.TryParse(target_limit_string[1], out max);
            min_target_limit = min;
            max_target_limit = max;
        }
        if (split_input.Length >= 4)
        {
            string type_string = split_input[3];
            Array types = Enum.GetValues(typeof(Types));

            foreach (Effect_Types ty in types)
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

        if (split_input.Length >= 5)
        {
            string value_string = split_input[4];
            values = new string[value_string.Split(',').Length];
            for (int x = 0; x < values.Length; x++)
            {
                values[x] = value_string.Split(',')[x];
                //Debug.Log("Values " + x + " " + values[x]);
            }
        }
        else
        {
            values = new string[0];
        }
        //string json = JsonUtility.ToJson(this);
        //Debug.Log("Effect:" + json);
    }

    /// <summary>
    /// Constructor for class, used when converting values from editor.
    /// </summary>
    /// <param name="new_type">An int to determine the Effect_Type.</param>
    /// <param name="new_adv">The advantage required for the effect.</param>
    /// <param name="new_aff">An int to determine what the Effect Affect.</param>
    /// <param name="new_checks">The checks to run before the effect triggers.</param>
    /// <param name="new_values">The values to determine what the effect does.</param>
    /// <param name="new_min">The minimum targets required for the effect to trigger.</param>
    /// <param name="new_max">The maximum amount of targets the effect can affect.</param>
    [JsonConstructor] public Action_Effect(int new_type, float new_adv, int new_aff, 
        string[] new_checks, string[] new_values, int new_min, 
        int new_max)
    {
        type = (Effect_Types)new_type;
        req_advantage = new_adv;
        affects = (Affects)new_aff;
        checks = new_checks;
        values = new_values;
        min_target_limit = new_min;
        max_target_limit = new_max;
    }

    /// <summary>
    /// Resolves the checks for the Action_Effect.
    /// </summary>
    /// <param name="source_obj">The source object of the Action_Effect.</param>
    /// <param name="target_obj">The target object of the Action_Effect.</param>
    /// <param name="target">The Target of the Action_Effect.</param>
    /// <returns></returns>
    public bool Resolve_Checks(GameObject source_obj, GameObject target_obj, Target target)
    {
        return Game_Controller.Parse_Checks(checks, source_obj, target_obj, target);
    }

    public IEnumerator Enact_Effect(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        if (Resolve_Checks(source_obj, target_obj, target))
        {

            if (type == Effect_Types.Orient)
            {
                yield return Enact_Orient(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Move)
            {
                //Debug.Log("target tile: " + target_tile.index[0] + "," + target_tile.index[1]);
                yield return Enact_Move(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Damage)
            {
                yield return Enact_Damage(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Heal)
            {
                yield return Enact_Healing(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Status)
            {
                yield return Enact_Status(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Condition)
            {
                yield return Enact_Condition(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Create)
            {
                yield return Enact_Create(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Elevate)
            {
                yield return Enact_Elevate(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Enable)
            {
                yield return Enact_Enable(act, source_obj, target_obj, target);
            }
            else if (type == Effect_Types.Pass)
            {
                yield return Enact_Pass(act, source_obj, target_obj, target);
            }
        }
        else
        {
            Debug.Log("Check not passed for " + act.name + " " + type.ToString());
        }
    }

    public IEnumerator Enact_Orient(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script chara = null;
        if (affects == Affects.Self)
        {
            chara = Game_Controller.Get_Character_Script(source_obj);
        }
        else if (affects == Affects.Target)
        {
            chara = Game_Controller.Get_Character_Script(target_obj);
        }
        if (chara != null)
        {
            if (values[0] == "SELE")
            {
                yield return chara.StartCoroutine(chara.Choose_Orientation());
                /*while(target_character.state.Peek() == Character_States.Orienting)
                {
                    yield return new WaitForEndOfFrame();
                }*/
            }
            else if (values[0] == "TARG")
            {
                chara.Choose_Orientation(target.center.gameObject);
                while (chara.state.Peek() == Character_States.Orienting)
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            else
            {
                chara.Choose_Orientation((int)Game_Controller.Convert_To_Float(values[0], chara.gameObject, target_obj, target));
            }
        }
    }

    public IEnumerator Enact_Move(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script chara = null;
        if (affects == Affects.Self)
        {
            chara = Game_Controller.Get_Character_Script(source_obj);
        }
        else if (affects == Affects.Target)
        {
            chara = Game_Controller.Get_Character_Script(target_obj);
        }
        if (chara != null)
        {

            int move_type = (int)Game_Controller.Convert_To_Float(values[0], source_obj, target_obj, target);
            int direction = 0;
            float distance = 0;
            List<Tile> move_path = new List<Tile>();
            //Debug.Log(values[1]);
            if (values[1] == "PATH")
            {
                act.Find_Reachable_Tiles(chara, false);
                //Debug.Log(target_tile.obj.name);
                //Debug.Log(target.center);
                move_path = act.Convert_To_Tile_List(this, chara.gameObject, target);
            }
            else if (values[1] == "VECT")
            {
                direction = (int)Game_Controller.Convert_To_Float(values[2], source_obj, target_obj, target);
                //Debug.Log("direction " + direction);
                distance = Game_Controller.Convert_To_Float(values[3], source_obj, target_obj, target);
                move_path = act.Find_Reachable_Tiles(chara, move_type, direction, distance, true);
            }
            //Debug.Log(move_path.Count);
            Tile start = move_path[0];
            Tile end = move_path[move_path.Count - 1];

            //Actually move
            yield return chara.StartCoroutine(chara.Move(act, move_type, move_path));

            Debug.Log("Character " + chara.name + " Moved from: " + start.index[0] + ","
                + start.index[1] + " to: " + end.index[0] + ","
                + end.index[1]);
        }
        else
        {
            Debug.Log("Invalid move type.");
        }
    }

    private IEnumerator Enact_Damage(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        //TODO add damage to Tiles
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Character_Script target_chara = Game_Controller.Get_Character_Script(target_obj);
        int damage = (int)(Game_Controller.Convert_To_Float(values[0], source_obj, target_obj, target) * target.affected_tiles[Game_Controller.Get_Tile_Script(target_obj)][1]);
        if (source_chara)
        {
            source_chara.Increase_Turn_Stats(Character_Turn_Records.Damage_Dealt, damage);
            if (target_chara != null)
            {
                Debug.Log("Character " + source_chara.character_name + " Attacked: " + target_chara.character_name + "; Dealing " + damage + "(" + Game_Controller.Convert_To_Float(values[0], source_obj, target_obj, target) + ") damage");
                target_chara.Take_Damage(damage, source_chara.weapon.pierce);
                Event_Manager.Broadcast(Event_Trigger.ON_DAMAGE, act, values[0], target_obj);
            }
            else
            {
                Debug.Log("Character " + source_chara.character_name + " Attacked: OBJECT" + "; Dealing " + damage + " damage");
                Object_Script target_object = Game_Controller.Get_Object_Script(target_obj);
                target_object.Take_Damage(damage, source_chara.weapon.pierce);
                Event_Manager.Broadcast(Event_Trigger.ON_DAMAGE, act, values[0], target_obj);
            }
        }
        yield return null;
    }

    private IEnumerator Enact_Healing(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Character_Script target_chara = Game_Controller.Get_Character_Script(target_obj);
        if (target_chara && source_chara)
        {
            int healing = (int)(Game_Controller.Convert_To_Float(values[1], source_obj, target_obj, target) * target.affected_tiles[Game_Controller.Get_Tile_Script(target_obj)][1]);
            if (values[0] == Accepted_Float_Shortcuts.AUC.ToString())
            {
                Debug.Log("Character " + source_chara.character_name + " Healed: " + target_chara.character_name + "; for " + healing + " Aura");
                target_chara.Recover_Aura(healing);
                Event_Manager.Broadcast(Event_Trigger.ON_HEALING, act, values[1], target_obj);
            }
            else if (values[0] == Accepted_Float_Shortcuts.MPC.ToString())
            {
                Debug.Log("Character " + source_chara.character_name + " Healed: " + target_chara.character_name + "; for " + healing + " MP");
                target_chara.Recover_Mana(healing);
                Event_Manager.Broadcast(Event_Trigger.ON_HEALING, act, values[1], target_obj);
            }
            else if (values[0] == Accepted_Float_Shortcuts.APC.ToString())
            {
                Debug.Log("Character " + source_chara.character_name + " Healed: " + target_chara.character_name + "; for " + healing + " AP");
                target_chara.Recover_Actions(healing);
                Event_Manager.Broadcast(Event_Trigger.ON_HEALING, act, values[1], target_obj);
            }
            else
            {
                Debug.Log("Invalid Healing prefix.");
            }

        }
        yield return null;
    }

    private IEnumerator Enact_Status(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Character_Script target_chara = Game_Controller.Get_Character_Script(target_obj);
        if (target_chara && source_chara)
        {
            Event_Manager.Broadcast(Event_Trigger.ON_STATUS, act, values[0], target_obj);
        }
        yield return null;
    }

    public IEnumerator Enact_Condition(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Character_Script target_chara = Game_Controller.Get_Character_Script(target_obj);
        if (target_chara && source_chara)
        {
            //First we need to resolve the Condition
            //Check for a power and attribute
            double power = 0;
            string attribute = "";
            //TODO add a way for status to affect Tiles and Objects.
            if (values.Length >= 3 && values[2] != null)
            {
                power = Game_Controller.Convert_To_Float(values[2], source_obj, target_obj, target);
            }
            if (values.Length == 4 && values[3] != null)
            {
                attribute = values[3];
            }
            int duration = (int)Game_Controller.Convert_To_Float(values[1], source_obj, target_obj, target);
            Condition condi = new Condition(values[0], duration, power * target.affected_tiles[Game_Controller.Get_Tile_Script(target_obj)][1], attribute);

            //Now we add the Condition to the target.
            target_chara.Increase_Turn_Stats(Character_Turn_Records.Conditions_Dealt, 1);
            Event_Manager.Broadcast(Event_Trigger.ON_CONDITION, act, values[0], target_obj);
            target_chara.Add_Condition(condi);

            Debug.Log("Character " + source_chara.character_name + " Gave: " + target_chara.character_name +
                " " + condi.type.ToString() + " for " + condi.duration + " turns " + " with " + condi.power +
                " power");
        }
        else
        {
            Debug.Log("No Object on Tile [" + Game_Controller.Get_Tile_Script(target_obj).index[0] + "," + Game_Controller.Get_Tile_Script(target_obj).index[1] + "]");
        }
        yield return null;
    }

    private IEnumerator Enact_Tile_Effect(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Tile target_tile = Game_Controller.Get_Tile_Script(target_obj);
        if (target_tile && source_chara)
        {
            if (target_tile.obj == null && target_tile.traversible)
            {
                for (int i = 3; i < values.Length; i++)
                {
                    if (values[i] != null)
                    {
                        if (values[2] != Effect_Types.Heal.ToString() && values[2] != Effect_Types.Condition.ToString())
                        {
                            values[i - 3] = "" + Game_Controller.Convert_To_Float(values[i], source_obj, target_tile.gameObject, target);
                        }
                        else
                        {
                            if (i == 3)
                            {
                                values[i - 3] = values[i];
                            }
                            else
                            {
                                values[i - 3] = "" + Game_Controller.Convert_To_Float(values[i], source_obj, target_obj, target);
                            }
                        }
                    }
                }
                int duration = (int)Game_Controller.Convert_To_Float(values[1], source_obj, target_obj, target);
                //TODO: FIX THIS LATER.
                // NEED to change this to read object data from a template file based on type and ID. 
                //Game_Controller.Get_Curr_Scenario().Get_Tile_Grid().Create_Hazard(target_tile.gameObject, values[0], new Action_Effect(values[2]), target.affected_tiles[target_tile][0], duration, source_chara);
                Event_Manager.Broadcast(Event_Trigger.ON_EFFECT, act, values[0], target_obj);
                //Debug.Log("Character " + character.character_name + " Created Effect: " + name + " on tile (" + target_tile.index[0] + "," + target_tile.index[1] + "); For " + duration + " and Using " + ap_cost + " AP");
            }
            else
            {
                Debug.Log("Can't spawn Tile_Effect on [" + target_tile.index[0] + "," + target_tile.index[1] + "] because it is occupied.");
            }
        }
        yield return null;
    }

    private IEnumerator Enact_Create(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Tile target_tile = Game_Controller.Get_Tile_Script(target_obj);
        if (target_tile && source_chara)
        {
            if (target_tile.obj == null && target_tile.traversible)
            {
                Event_Manager.Broadcast(Event_Trigger.ON_CREATE, act, values[0], target_obj);
            }
            else
            {
                Debug.Log("Can't spawn Object on [" + target_tile.index[0] + "," + target_tile.index[1] + "] because it is occupied.");
            }
        }
        yield return null;
    }

    private IEnumerator Enact_Elevate(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Tile target_tile = Game_Controller.Get_Tile_Script(target_obj);
        if (target_tile && source_chara)
        {
            int elevation = (int)(Game_Controller.Convert_To_Float(values[0], source_obj, target_obj, target) * target.affected_tiles[target_tile][0]);
            Debug.Log("Character " + source_chara.character_name + " Elevated Tile: (" + target_tile.index[0] + "," + target_tile.index[1] + "); By " + elevation);
            //character.Get_Controller().Get_Curr_Scenario().tile_grid.Elevate(target.game_object.transform, elevation);
            Event_Manager.Broadcast(Event_Trigger.ON_ELEVATE, act, values[0], target_obj);
            yield return Game_Controller.Get_Curr_Scenario().StartCoroutine(Game_Controller.Get_Curr_Scenario().Elevate(target_tile.transform, elevation));
        }
        yield return null;
    }

    private IEnumerator Enact_Enable(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Character_Script target_chara = Game_Controller.Get_Character_Script(target_obj);
        if (target_chara && source_chara)
        {
            foreach (Character_Action action in target_chara.GetComponent<Character_Script>().actions)
            {
                //Debug.Log("act.name: " + act.name + ", eff.values: " + values[0]);
                if (action.name == values[0])
                {
                    Event_Manager.Broadcast(Event_Trigger.ON_ENABLE, act, values[0], target_obj);
                    //Debug.Log("MATCH");
                    if (values[1] == "false" || values[1] == "False" || values[1] == "FALSE")
                    {
                        //Debug.Log("Skill " + act.name + " is disabled.");
                        act.Disable();
                    }
                    if (values[1] == "true" || values[1] == "True" || values[1] == "TRUE")
                    {
                        //Debug.Log("Skill " + act.name + " is enabled.");
                        act.Enable();
                    }
                }
            }
            
        }
        yield return null;
    }

    private IEnumerator Enact_Pass(Character_Action act, GameObject source_obj, GameObject target_obj, Target target)
    {
        Character_Script source_chara = Game_Controller.Get_Character_Script(source_obj);
        Character_Script target_chara = Game_Controller.Get_Character_Script(target_obj);
        if (target_chara && source_chara)
        {
            yield return target_chara.StartCoroutine(target_chara.End_Turn());
        }
        yield return null;
    }

}
