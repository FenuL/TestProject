using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tile_Effect : MonoBehaviour {

    /// <summary>
    /// Constants:
    /// static string EFFECT_PREFAB_FILE - The location of the effect prefab used to create additional Tile_Effects.
    /// 
    /// Variables: 
    /// Game_Controller controller - The game controller object.
    /// enum Types - The possible types of Effects.
    /// string name - The name for the Effect. Necessary ebcause sometimes we create the script before the GameObject.
    /// Types type - The Type of the Effect. From the Types enum.
    /// string value - The formula to calculate the Effect's result. Use the Convert_to_Double method to parse this.
    /// float modifier - The modifier for the effect. Passed in when the Effect is created.
    /// int duration - The number of turns the effect is active for. A turn is defined as one Character's turn.
    /// GameObject current_tile - The Tile on which to place the Effect. We use the GameObject so we can get it's positition.
    /// </summary>

    private static string EFFECT_PREFAB_FILE = "Prefabs/Scenario Prefabs/Effect Prefabs/effect_prefab";

    public Game_Controller controller { get; private set; }
    public enum Types { Move, Damage, Heal, Status, Effect, Elevate, Enable, Pass }
    public string name { get; private set; }
    public Types type { get; private set; }
    public string[] value { get; private set; }
    public float modifier { get; private set; }
    public int duration { get; private set; }
    public GameObject current_tile { get; private set; }

    /// <summary>
    /// Constructor for the class taking in an existing Tile_Effect script.
    /// </summary>
    /// <param name="old_effect">The old Tile_Effect tp base this one out of.</param>
    public Tile_Effect(Tile_Effect old_effect)
    {
        name = old_effect.name;
        type = old_effect.type;
        value = old_effect.value;
        duration = old_effect.duration;
        modifier = old_effect.modifier;
        current_tile = old_effect.current_tile;
    }

    /// Constructor for the class
    /// </summary>
    /// <param name="new_name">Name for the Tile_Effect</param>
    /// <param name="new_type">The Type of Effect</param>
    /// <param name="new_value">The value for the Effect</param>
    /// <param name="new_duration">The number of character turns the Effect remains in play</param>
    /// <param name="new_modifier">The Effect's modifier.</param>
    /// <param name="new_tile">The tile on which to place the effect</param>
    public Tile_Effect(string new_name, string new_type, int new_duration, string[] new_value, float new_modifier, GameObject new_tile){
        controller = Game_Controller.controller;
        name = new_name;
        Array types = Enum.GetValues(typeof(Types));
        foreach (Types ty in types)
        {
            //Debug.Log(new_type + " and " + ty.ToString());
            if (new_type.Contains(ty.ToString()))
            {
                type = ty;
                //Debug.Log("type:" + type);
            }
        }
        value = new_value;
        duration = new_duration;
        modifier = new_modifier;
        current_tile = new_tile;
    }

    /// <summary>
    /// Used to create an actual GameObject and pass in the script's variables to it. 
    /// </summary>
    public void Instantiate()
    {
        GameObject tile_effect_prefab = Resources.Load(EFFECT_PREFAB_FILE, typeof(GameObject)) as GameObject;
        GameObject tile_effect_obj = ((GameObject)Instantiate(tile_effect_prefab, current_tile.transform.position + new Vector3(0, .5f * (current_tile.GetComponent<Tile>().height+1), 0), Quaternion.identity));
        Tile_Effect tile_effect_script = tile_effect_obj.GetComponent<Tile_Effect>();
        tile_effect_obj.name = name;
        tile_effect_script.type = type;
        //Debug.Log(type.ToString());
        if (type == Types.Heal)
        {
            tile_effect_obj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Object Sprites/potions_transparent")[0];
        }
        if (type == Types.Status)
        {
            tile_effect_obj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Object Sprites/potions_transparent")[1];
        }
        if (type == Types.Damage)
        {
            tile_effect_obj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Object Sprites/potions_transparent")[2];
        }
        if (type == Types.Heal && value[0] == Accepted_Shortcuts.MPC.ToString())
        {
            tile_effect_obj.GetComponent<SpriteRenderer>().sprite = Resources.LoadAll<Sprite>("Sprites/Object Sprites/potions_transparent")[3];
        }
        tile_effect_script.value = value;
        tile_effect_script.modifier = modifier;
        tile_effect_script.duration = duration;
        tile_effect_script.current_tile = current_tile;
        controller.curr_scenario.tile_effects.Add(tile_effect_obj);
        current_tile.GetComponent<Tile>().effect = tile_effect_obj;
}

    /// <summary>
    /// Decrease the duration of the Tile_Effect and returns the updated duration. 
    /// </summary>
    /// <returns>The remaining duration in turns for the Tile_Effect.</returns>
    public int Progress()
    {
        duration -= 1;
        if (duration <= 0)
        {
            gameObject.tag = "Delete";
            current_tile.GetComponent<Tile>().effect = null;
        }
        return duration;
    }

    /// <summary>
    /// Coroutine for triggering the Effect.
    /// Calls the various Enact_<>() Functions depending on the Actions's Action_Effects. 
    /// </summary>
    /// <param name="character">Character triggering this Effect.</param>
    /// <returns>An IEnumerator with the current Coroutine progress. </returns>
    public IEnumerator Enact(Character_Script character)
    {
        while (character.curr_action.paused)
        {
            yield return new WaitForEndOfFrame();
        }
        if (type == Types.Move)
        {
            //Enact_Move(character, value[0]);
        }
        else if (type == Types.Damage)
        {
            Enact_Damage(character, value[0]);
        }
        else if (type == Types.Heal)
        {
            Enact_Healing(character, value);
        }
        else if (type == Types.Status)
        {
            Enact_Status(character, value);
        }
        else if (type == Types.Enable)
        {
            Enact_Enable(character, value);
        }
        else if (type == Types.Elevate)
        {
            Enact_Elevate(character, value[0]);
        }
        else if (type == Types.Pass)
        {
            Enact_Pass(character);
        }
        character.curr_action.Resume();
        gameObject.tag = "Delete";
    }

    /// <summary>
    /// Function to Enact a Move Type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Move Action</param>
    /// <param name="movetype">The Type of Movement the Character is performing. </param>
    /// <param name="target">The Target destination for the Move.</param>
    public void Enact_Move(Character_Script character, string movetype, Target target)
    {
        //mover types
        // 1 = standard move
        // 2 = push/pull
        // 3 = fly
        // 4 = warp

        if (target.game_object.GetComponent<Tile>())
        {
            Tile target_tile = target.game_object.GetComponent<Tile>();

            //Actually move
            character.GetComponent<Character_Script>().MoveTo(target.game_object.transform);

            //reset current tile information
            character.curr_tile.GetComponent<Tile>().traversible = true;
            character.curr_tile.GetComponent<Tile>().obj = null;

            //set new tile information
            character.curr_tile = target_tile.transform;
            character.curr_tile.GetComponent<Tile>().traversible = false;
            character.curr_tile.GetComponent<Tile>().obj = character.gameObject;
        }
        else
        {
            Debug.Log("Invalid target for move.");
        }
    }

    /// <summary>
    /// Converts the String parameters from the Effect into a double. 
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
                    else if (val.ToString() == "CRD")
                    {
                        input = input.Replace(val.ToString(), "" + obj.coordination);
                    }
                    else if (val.ToString() == "SPT")
                    {
                        input = input.Replace(val.ToString(), "" + obj.spirit);
                    }
                    else if (val.ToString() == "DEX")
                    {
                        input = input.Replace(val.ToString(), "" + obj.dexterity);
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
                        input = input.Replace(val.ToString(), "" + obj.weapon.modifier.GetLength(0) / 2);
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
    /// Function to Enact a Damage type Effect. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character triggering the Effect</param>
    /// <param name="value">The String with the damage equation.</param>
    public void Enact_Damage(Character_Script character, String value)
    {
        int damage = (int)(Convert_To_Double(value, character) * modifier);
        character.Take_Damage(damage, character.weapon.pierce);
    }

    /// <summary>
    /// Function to Enact a Healing type Effect. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character triggering the Effect</param>
    /// <param name="value">The String with the healing equation.</param>
    public void Enact_Healing(Character_Script character, String[] value)
    {
        int healing = (int)(Convert_To_Double(value[1], character) * modifier);
        if (value[0] == Accepted_Shortcuts.AUC.ToString())
        {
            Debug.Log("Character " + character.character_name + " Triggered: " + name + "; for " + healing + " Aura");
            character.Recover_Aura(healing);
        }
        else if (value[0] == Accepted_Shortcuts.MPC.ToString())
        {
            Debug.Log("Character " + character.character_name + " Triggered: " + name + "; for " + healing + " MP");
            character.Recover_Mana(healing);
        }
        else if (value[0] == Accepted_Shortcuts.APC.ToString())
        {
            Debug.Log("Character " + character.character_name + " Triggered: " + name + "; for " + healing + " AP");
            character.Recover_Actions(healing);
        }
        else
        {
            Debug.Log("Invalid Healing prefix.");
        }
    }

    /// <summary>
    /// Function to Enact a Status type Effect. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character triggering the Effect</param>
    /// <param name="value">The equation for what Status to apply. </param>
    public void Enact_Status(Character_Script character, String[] values)
    {
        //First we need to resolve the Condition
        //Check for a power and attribute
        double power = 0;
        string attribute = "";
        if (values.Length >= 3 && values[2] != null)
        {
            power = Convert_To_Double(values[2], character);
        }
        if (values.Length == 4 && values[3] != null)
        {
            attribute = values[3];
        }
        int duration = (int)Convert_To_Double(values[1], character);
        Condition condi = new Condition(values[0], duration, power * modifier, attribute);

        //Now we add the Condition to the target.
        character.Add_Condition(condi);
    }

    /// <summary>
    /// Function to Enact an Elevate type Effect. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character triggering the Effect </param>
    /// <param name="value">The String with the equation for how much to Elevate.</param>
    public void Enact_Elevate(Character_Script character, String value)
    {
        if (current_tile.GetComponent<Tile>())
        {
            int elevation = (int)(Convert_To_Double(value, character) * modifier);
            character.controller.curr_scenario.tile_grid.Elevate(current_tile.transform, elevation);
        }
        else
        {
            Debug.Log("Invalid target for Elevate.");
        }
    }

    /// <summary>
    /// Function to Enact an Enable type Effect. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action. </param>
    /// <param name="value">The String with the euqation for what to do. Should be an action name and a bool pair.</param>
    public void Enact_Enable(Character_Script character, String[] value)
    {
        foreach (Action act in character.GetComponent<Character_Script>().actions)
        {
            //Debug.Log("act.name: " + act.name + ", eff.value: " + eff.value[0]);
            if (act.name == value[0])
            {
                //Debug.Log("MATCH");
                if (value[1] == "false" || value[1] == "False" || value[1] == "FALSE")
                {
                    //Debug.Log("Skill " + act.name + " is disabled.");
                    act.enabled = false;
                }
                if (value[1] == "true" || value[1] == "True" || value[1] == "TRUE")
                {
                    //Debug.Log("Skill " + act.name + " is enabled.");
                    act.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Function to Enact a Pass type Effect. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action.</param>
    public void Enact_Pass(Character_Script character)
    {
        character.StartCoroutine(character.End_Turn());
    }

    // Use this for initialization
    void Start () {
	
	}

    /// <summary>
    /// Called once per Frame to update the Object. 
    /// Makes the Object face the Camera.
    /// </summary>
    void Update()
    {
        //Change sprite facing to match current camera angle
        transform.eulerAngles = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
