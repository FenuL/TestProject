using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/// <summary>
/// Class to define Character Actions. 
/// </summary>
public class Character_Action
{
    /// <summary>
    /// string PLAYER_ACTIONS_FILE - The actions file where all actions are stored
    /// int HEADINGS - The number of headings in each action
    /// enum Activation_Types - The types of actions for abilities
    /// enum Origin_Types - The types of origin for abilities.
    /// enum Accepted_Shortcuts - Shortcuts used to read the actions from the action list.
    /// </summary>
    private static string PLAYER_ACTIONS_FILE = "Assets/Resources/Actions/Action_List.txt";
    private static int HEADINGS = 14;
    public enum Activation_Types { Active, Passive, Reactive }
    public enum Origin_Types { Innate, Soul, Weapon }

    /// <summary>
    /// Variables:
    /// String name - Character_Action name
    /// String ap_cost - Unconverted Cost of the action in Character_Action Points 
    /// String mp_cost - Unconverted Cost of the action in Mana Points
    /// String range - Unconverted number of tiles away the skill can target.
    /// String center - Unconverted where to center the Character_Action. Over a Target or over the Character's Self.
    /// float[,] area - The area of effect that the Character_Action will affect.
    /// bool paused - If the Character_Action is paused or not. 
    /// List<Action_Effect> self_effect - The Effect the Character will have on itself when using this Character_Action
    /// List<Action_Effect> target_effect - the Effect the Character will have on a target when using this Character_Action
    /// Activation_Types type - The type of Activation for the Skill. Active skills must be Selected. Passive Skills are always active. Reactive Skills have a Trigger.
    /// Event_Trigger trigger - what event triggers the action if it is a reactive action. Example, ON_DAMAGE
    /// string condition - Under what condition a Reaction can be used after an event is fired. Example, CAUC > CAUM/2
    /// Origin_Types origin - Where the skill originates (for disabling purposes later)
    /// bool enabled - If the Character_Action is enabled or not
    /// string orient - Whether the ability lets you select orientation. 
    /// String animation - The animation tied to the specific Character_Action.
    /// Character_Script character - The Character performing the Character_Action.
    /// </summary>

    public String name { get; private set; }
    public String ap_cost { get; private set; }
    public String mp_cost { get; private set; }
    public String range { get; private set; }
    public String center { get; private set; }
    public float[,] area { get; private set; }
    public bool paused { get; private set; }
    public List<Action_Effect> self_effect { get; private set; }
    public List<Action_Effect> target_effect { get; private set; }
    public Activation_Types type { get; private set; }
    public Event_Trigger trigger { get; private set; }
    public string condition { get; private set; }
    public Origin_Types origin { get; private set; }
    public bool enabled { get; set; }
    public string orient { get; private set; }
    public String animation { get; private set; }
    public Character_Script character { get; private set; }
    public int proc_effect_num { get; set; }
    public int anim_num { get; private set; }

    /// <summary>
    /// Create an empty Character_Action.
    /// </summary>
    public Character_Action()
    {
        name = "";
        ap_cost = "";
        mp_cost = "";
        range = "";
        center = "";
        area = null;
        paused = false;
        self_effect = null;
        target_effect = null;
        type = Activation_Types.Active;
        trigger = Event_Trigger.ON_DAMAGE;
        condition = "";
        origin = Origin_Types.Innate;
        enabled = true;
        orient = "";
        animation = "";
        character = null;
        proc_effect_num = 0;
    }

    /// <summary>
    /// Constructor used to assign an Action to a character.
    /// </summary>
    /// <param name="act">The Action from which to inherit the other action fields</param>
    /// <param name="chara">The Character_Script that owns the Action</param>
    public Character_Action(Character_Action act, Character_Script chara)
    {
        name = act.name;
        ap_cost = act.ap_cost;
        mp_cost = act.mp_cost;
        range = act.range;
        center = act.center;
        area = act.area;
        paused = act.paused;
        self_effect = act.self_effect;
        target_effect = act.target_effect;
        type = act.type;
        trigger = act.trigger;
        condition = act.condition;
        origin = act.origin;
        enabled = act.enabled;
        orient = act.orient;
        animation = act.animation;
        character = chara;
        proc_effect_num = act.proc_effect_num;
    }

    /// <summary>
    /// Checks to see if one Character_Action is equal to another. Does not Check the Action's Character_Script.
    /// </summary>
    /// <param name="act">The Character_Action to compare to. </param>
    /// <returns>True if they are Equal, False otherwise. </returns>
    public bool Equals(Character_Action act)
    {
        bool eql = false;
        if (name == act.name &&
            ap_cost == act.ap_cost &&
            mp_cost == act.mp_cost &&
            range == act.range &&
            center == act.center &&
            area == act.area &&
            self_effect == act.self_effect &&
            target_effect == act.target_effect &&
            type == act.type &&
            trigger == act.trigger &&
            condition == act.condition &&
            origin == act.origin &&
            enabled == act.enabled &&
            orient == act.orient &&
            animation == act.animation)
        {
            eql = true;
        }
        return eql;
    }

    /// <summary>
    /// Set the Character for the Action.
    /// </summary>
    /// <param name="chara">The Character to set for the Action</param>
    public void Set_Character(Character_Script chara)
    {
        character = chara;
    }

    /// <summary>
    /// Takes a List of strings and returns a usable Character_Action object.
    /// </summary>
    /// <param name="input">A List of strings in the following format: HEADING: VALUES 
    /// Valid Headings:
    ///     name - The name of the Character_Action. 
    ///         Accepts one value.
    ///     ap_cost - The cost of the Character_Action in AP. 
    ///         Accepts one value, can use Accepted_Shortcuts.
    ///         Eg: ap_cost: 1
    ///     mp_cost - The cost of the Character_Action in MP. 
    ///         Accepts one value, can use Accepted_Shortcuts.
    ///         Eg: mp_cost: MPC
    ///     range -  The range of the Character_Action. Accepts one value, can use Accepted_Shortcuts.
    ///     center - The center of the Character_Action. Either "Self" or "Target" followed by Size of AoE, eg "center: Target 3x3". 
    ///     area - The area affected by the Character_Action and multiplier to apply to the Target. Accepts multiple doubles separated by spaces. 
    ///          Multiple area headings are accepted, but number must match amount specified in "center" heading.
    ///          Eg: "area: 0 1 0" 
    ///              "area: 0.5 0 0.5"
    ///              "area: 0 1 0"
    ///     self_effect - The effect to apply to the Character using this Character_Action. 
    ///         Accepts Multiple Entries separated by semicolons (";"). 
    ///         Values for each effect are separated by spaces " ".
    ///         Eg: self_effect: Heal 10; Pass
    ///     target_effect - The effect to apply to the Characters affected by the Target. 
    ///         Accepts Multiple Entries separated by semicolons (";").
    ///         Eg: target_effect: Elevate 2; Damage WPD+3
    ///     activation_type - The Activation type for the Character_Action. From the Activation_Type enum. Active, Passive or Reactive.
    ///         Eg: activation_type: active
    ///     origin - The Origin of the Character_Action, from the Origin enum. What is used to complete the Character_Action. Used for Disables.
    ///         Eg: origin: weapon
    ///     trigger - The Trigger for the Character_Action if it is Reactive. NUL for no Trigger.
    ///         Eg: trigger: NUL
    ///     orient - If the Character_Action lets the player choose an orientation after its use, before or looks at a specified target.
    ///         Eg: orient: target
    ///     enabled - If the Character_Action is enabled or not. 
    ///         Eg: enabled: true
    ///     animation - The animation attached to the Character_Action</param>
    ///         Eg: animation: NUL
    /// <returns>A completely constructed Character_Action</returns>
    public static Character_Action Parse(string[] input)
    {
        Character_Action act = new Character_Action();
        int area_x_index = 0;
        int area_y_index = 0;
        act.paused = false;
        foreach (String s in input)
        {
            if (s != null)
            {
                //Split the line into its category and its values
                string[] split = s.Split(':');
                if (split.Length < 2)
                {
                    return null;
                } 
                String category = split[0];
                String values = split[1];
                //Debug.Log("category: " + category);
                //Debug.Log("value 0: " + values.TrimStart().TrimEnd());
                //read the inputs based on their categories
                switch (category)
                {
                    case "name":
                        act.name = values.TrimStart().TrimEnd();
                        break;
                    case "ap_cost":
                        act.ap_cost = values.Trim();
                        break;
                    case "mp_cost":
                        act.mp_cost = values.Trim();
                        break;
                    case "range":
                        act.range = values.Trim();
                        break;
                    case "center":
                        act.center = values.TrimStart().Split(' ')[0];
                        int x;
                        int y;
                        int.TryParse(values.Split(' ')[2].Split('x')[0], out x);
                        int.TryParse(values.Split(' ')[2].Split('x')[1], out y);
                        act.area = new float[x, y];
                        break;
                    case "area":
                        foreach (String num in values.TrimStart().TrimEnd().Split(' '))
                        {
                            float number = 0;
                            float.TryParse(num, out number);
                            act.area[area_x_index, area_y_index] = number;
                            area_y_index++;
                        }
                        area_y_index = 0;
                        area_x_index++;
                        break;
                    case "self_effect":
                        act.self_effect = new List<Action_Effect>();
                        if (!values.Contains("NUL"))
                        {
                            foreach (String value in values.TrimStart().TrimEnd().Split(';'))
                            {
                                act.self_effect.Add(new Action_Effect(value));
                            }
                        }
                        break;
                    case "target_effect":
                        act.target_effect = new List<Action_Effect>();
                        if (!values.Contains("NUL"))
                        {
                            foreach (String value in values.TrimStart().TrimEnd().Split(';'))
                            {
                                act.target_effect.Add(new Action_Effect(value));
                            }
                        }
                        break;
                    case "activation_type":
                        if (values.Trim() == "Reactive" ||
                            values.Trim() == "reactive")
                        {
                            act.type = Activation_Types.Reactive;
                        }
                        else if (values.Trim() == "Passive" ||
                            values.Trim() == "passive")
                        {
                            act.type = Activation_Types.Passive;
                        }
                        else
                        {
                            act.type = Activation_Types.Active;
                        }
                        break;
                    case "origin":
                        if (values.Trim() == "Weapon" ||
                            values.Trim() == "weapon")
                        {
                            act.origin = Origin_Types.Weapon;
                        }
                        else if (values.Trim() == "Soul" ||
                            values.Trim() == "soul")
                        {
                            act.origin = Origin_Types.Soul;
                        }
                        else
                        {
                            act.origin = Origin_Types.Innate;
                        }
                        break;
                    case "trigger":
                        Array array = Enum.GetValues(typeof(Event_Trigger));
                        foreach (Event_Trigger tri in array)
                        {
                            if (values.Trim() == tri.ToString())
                            {
                                act.trigger = tri;
                            }
                        }
                        break;
                    case "condition":
                        //TODO Expand on this
                        act.condition = values.Trim();
                        break;
                    case "orient":
                        act.orient = values.Trim();
                        break;
                    case "enabled":
                        if (values.Trim() == "True" ||
                            values.Trim() == "true" ||
                            values.Trim() == "TRUE")
                        {
                            act.enabled = true;
                        }
                        else
                        {
                            act.enabled = false;
                        }
                        break;
                    case "animation":
                        act.animation = values.Trim();
                        break;
                }
            }
        }
        act.character = null;
        return act;
    }

    /// <summary>
    /// Reads in the Action_List file and collects String inputs to pass to the Parse() Function. 
    /// Returns a complete List of Character_Actions created from the File. 
    /// Used to generate a List of all available Character_Actions.
    /// </summary>
    /// <returns>A List of all Character_Actions available</returns>
    public static Dictionary<string, Character_Action> Load_Actions()
    {
        Dictionary<string, Character_Action> actions = new Dictionary<string, Character_Action>();
        foreach (string file in System.IO.Directory.GetFiles("Assets/Resources/Actions"))
        {
            string[] lines = System.IO.File.ReadAllLines(file);
            Character_Action action = Parse(lines);
            //Debug.Log("NAME: " + action.name);
            //Debug.Log("COST: "+ action.ap_cost);
            if (action != null && action.name != null && action.name != "")
            {
                actions.Add(action.name, action);
            }
        }
        return actions;
    }

    /// <summary>
    /// Converts the String parameters from the Character_Action into a double. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Double</param>
    /// <param name="target">The target receiving the action. Used to parse ACCEPTED_SHORTCUTS beginning with T.</param>
    /// <returns></returns>
    public double Convert_To_Double(string input, GameObject target)
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
                Character_Script source = character;

                if (val.ToString()[0] == 'T')
                {
                    if (target != null)
                    {
                        source = target.GetComponent<Character_Script>();
                    }
                }
                //Debug.Log("Input " + input);
                if (source != null && !source.Equals(null))
                {
                    if (input.Contains(val.ToString()))
                    {
                        if (val.ToString().Contains("AUM"))
                        {
                            input = input.Replace(val.ToString(), "" + source.aura_max);
                        }
                        else if (val.ToString().Contains("AUC"))
                        {
                            input = input.Replace(val.ToString(), "" + source.aura_curr);
                        }
                        else if (val.ToString().Contains("APM"))
                        {
                            input = input.Replace(val.ToString(), "" + source.action_max);
                        }
                        else if (val.ToString().Contains("APC"))
                        {
                            input = input.Replace(val.ToString(), "" + source.action_curr);
                        }
                        else if (val.ToString().Contains("MPM"))
                        {
                            input = input.Replace(val.ToString(), "" + source.mana_max);
                        }
                        else if (val.ToString().Contains("MPC"))
                        {
                            input = input.Replace(val.ToString(), "" + source.mana_curr);
                        }
                        else if (val.ToString().Contains("CAM"))
                        {
                            input = input.Replace(val.ToString(), "" + source.canister_max);
                        }
                        else if (val.ToString().Contains("CAC"))
                        {
                            input = input.Replace(val.ToString(), "" + source.canister_curr);
                        }
                        else if (val.ToString().Contains("SPD"))
                        {
                            input = input.Replace(val.ToString(), "" + source.speed);
                        }
                        else if (val.ToString().Contains("STR"))
                        {
                            input = input.Replace(val.ToString(), "" + source.strength);
                        }
                        else if (val.ToString().Contains("CRD"))
                        {
                            input = input.Replace(val.ToString(), "" + source.coordination);
                        }
                        else if (val.ToString().Contains("SPT"))
                        {
                            input = input.Replace(val.ToString(), "" + source.spirit);
                        }
                        else if (val.ToString().Contains("DEX"))
                        {
                            input = input.Replace(val.ToString(), "" + source.dexterity);
                        }
                        else if (val.ToString().Contains("VIT"))
                        {
                            input = input.Replace(val.ToString(), "" + source.vitality);
                        }
                        else if (val.ToString().Contains("LVL"))
                        {
                            input = input.Replace(val.ToString(), "" + source.level);
                        }
                        else if (val.ToString().Contains("WPR"))
                        {
                            input = input.Replace(val.ToString(), "" + source.weapon.modifier.GetLength(0) / 2);
                        }
                        else if (val.ToString().Contains("WPD"))
                        {
                            input = input.Replace(val.ToString(), "" + source.weapon.attack);
                        }
                        else if (val.ToString().Contains("WPN"))
                        {
                            input = input.Replace(val.ToString(), "" + source.weapon.name);
                        }
                        else if (val.ToString().Contains("ARM"))
                        {
                            input = input.Replace(val.ToString(), "" + source.armor.armor);
                        }
                        else if (val.ToString().Contains("WGT"))
                        {
                            input = input.Replace(val.ToString(), "" + source.armor.weight);
                        }
                        else if (val.ToString().Contains("MOC"))
                        {
                            input = input.Replace(val.ToString(), "" + source.action_curr);
                        }
                        else if (val.ToString().Contains("DST"))
                        {
                            input = input.Replace(val.ToString(), "" + source.aura_max);
                        }
                        else if (val.ToString().Contains("NUL"))
                        {
                            input = input.Replace(val.ToString(), "" + 0.0);
                        }
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
    /// What to do when an Character_Action is selected from the Character_Action Menu:
    /// Prerequisites for Selecting an action:
    ///     The Character_Action is enabled.
    ///     The Character has enough Character_Action Points.
    ///     The Character has enough Mana Points.
    ///     There are no Conditions blocking the Character_Action.
    /// </summary>
    public void Select()
    {
        if (character.Is_Idle())
        {
            //Debug.Log("Selecting Action");
            //Check to see if action is enabled
            //Debug.Log("Name: " + name + " is enabled: " + enabled);
            if (this.enabled)
            {
                //Debug.Log("AP cost: " + (int)Convert_To_Double(cost, character));
                //Check to see if player can afford action:
                if (Check_Resource())
                {
                    //Debug.Log("Enough action points");
                    if (character.mana_curr >= (int)Convert_To_Double(mp_cost, null))
                    {
                        //Debug.Log("Enough mana points");
                        //character.curr_action.Push(this);
                        if (target_effect != null)
                        {
                            foreach (Action_Effect eff in target_effect)
                            {
                                if (eff.type == Action_Effect.Types.Move)
                                {
                                    //character.state = Character_States.Moving;
                                    character.controller.curr_scenario.Find_Reachable((int)character.speed, (int)Convert_To_Double(range, null),1);
                                }
                                else if (eff.type == Action_Effect.Types.Damage)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Heal)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Status)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Elevate)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Enable)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                            }
                        }
                        if (self_effect != null)
                        {
                            foreach (Action_Effect eff in self_effect)
                            {
                                if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                                {
                                    if (Convert_To_Double(eff.value[0], null) != 4)
                                    {
                                        //character.state = Character_States.Moving;
                                        //Debug.Log("Speed: " + character.speed);
                                        character.controller.curr_scenario.Find_Reachable((int)character.speed, (int)Convert_To_Double(range, null),1);
                                    }
                                    else
                                    {
                                        //character.state = Character_States.Blinking;
                                        character.controller.curr_scenario.Find_Reachable((int)character.speed * 2, (int)Convert_To_Double(range, null),2);
                                    }
                                }
                                else if (eff.type == Action_Effect.Types.Damage)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Heal)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Status)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Elevate)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Enable)
                                {
                                    //character.state = Character_States.Attacking;
                                    character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null),3);
                                }
                                else if (eff.type == Action_Effect.Types.Pass)
                                {
                                    //character.curr_action = this;
                                    //character.StartCoroutine(character.Act(null));
                                    //Debug.Log("TEST 2");
                                    //TODO Change this so the Character_Action actually goes off before ending the turn.
                                    //Enact(character, null);
                                    character.curr_action.Push(this);
                                    character.StartCoroutine(character.End_Turn());
                                }
                            }
                        }
                    }
                    //Select the action in the action menu
                    foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
                    {
                        if (but.name == name)
                        {
                            but.GetComponent<Image>().color = Color.blue;
                        }
                        else if (but.GetComponent<Image>().color == Color.blue)
                        {
                            but.GetComponent<Image>().color = Color.white;
                        }
                    }
                    character.controller.curr_scenario.Clean_Reachable();
                    character.controller.curr_scenario.Mark_Reachable();
                    if (character.curr_action.Count != 0)
                    {
                        character.curr_action.Pop();
                    }
                    character.curr_action.Push(this);
                    //Debug.Log("Character " + character.name + " current action " + character.curr_action.Peek().name);
                    //Debug.Log(character.character_num);

                }
                else
                {
                    Debug.Log("NOT Enough Action Points");
                    foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
                    {
                        if (but.name == name)
                        {
                            but.GetComponent<Image>().color = Color.red;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Character_Action is disabled");
                foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
                {
                    if (but.name == name)
                    {
                        but.GetComponent<Image>().color = Color.red;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets the Tiles in the Area of Effect for the current Character_Action.
    /// </summary>
    /// <param name="target_tile">The tile around which the Character_Action is being used. </param>
    /// <returns>A List of Targets, which contain a GameObject(Tile Data) and the action's Area Modifier.</returns>
    public List<Target> Get_Target_Tiles(GameObject target_tile)
    {
        List<Target> targets = new List<Target>();
        //String[] area_effect = center.Split(' ');
        int startX = 0;
        int startY = 0;
        //Set the center of the ability
        if (center == "Self")
        {
            startX = character.curr_tile.GetComponent<Tile>().index[0];
            startY = character.curr_tile.GetComponent<Tile>().index[1];
        }
        else if (center == "Target")
        {
            if (target_tile != null)
            {
                startX = target_tile.GetComponent<Tile>().index[0];
                startY = target_tile.GetComponent<Tile>().index[1];
            }
        }
        //Set the start of the loop
        startX -= (area.GetLength(0) / 2);
        startY -= (area.GetLength(1) / 2);
        //Loop through the area and find valid targets
        for (int x = 0; x < area.GetLength(0); x++)
        {
            for (int y = 0; y < area.GetLength(1); y++)
            {
                if (area[x, y] != 0)
                {
                    Transform target = character.controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    if (target != null)
                    {
                        Target tar = new Target(target.gameObject, area[x, y]);
                        targets.Add(tar);
                    }
                }
            }
        }
        if (targets.Count > 0)
        {
            return targets;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Gets the Character_Scripts in the Area of Effect for the current Character_Action.
    /// </summary>
    /// <param name="target_tile">The tile around which the Character_Action is being used. </param>
    /// <returns>A List of Targets, which contain a GameObject(Character_Script) and the action's Area Modifier.</returns>
    public List<Target> Get_Targets(GameObject target_tile)
    {
        List<Target> targets = new List<Target>();
        int startX = 0;
        int startY = 0;
        //Set the center of the ability
        if (center == "Self")
        {
            startX = character.curr_tile.GetComponent<Tile>().index[0];
            startY = character.curr_tile.GetComponent<Tile>().index[1];
        }
        else if (center == "Target")
        {
            if (target_tile != null)
            {
                startX = target_tile.GetComponent<Tile>().index[0];
                startY = target_tile.GetComponent<Tile>().index[1];
            }
        }
        //Set the start of the loop
        startX -= area.GetLength(0) / 2;
        startY -= area.GetLength(1) / 2;

        //Loop through the area and find valid targets
        for (int x = 0; x < area.GetLength(0); x++)
        {
            for (int y = 0; y < area.GetLength(1); y++)
            {
                if (area[x, y] != 0)
                {
                    Transform target = character.controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    if (target != null)
                    {
                        if (target.GetComponent<Tile>().obj != null)
                        {
                            Target tar = new Target(target.GetComponent<Tile>().obj, Calculate_Total_Modifier(target.GetComponent<Tile>().obj, area[x, y]));
                            targets.Add(tar);
                        }
                    }
                }
            }
        }
        if (targets.Count > 0)
        {
            return targets;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Used to calculate the total modifier to be applied to the ability. 
    /// </summary>
    /// <param name="target">The target of the action. Could be a Character_Script or an Object.</param>
    /// <param name="action_mod">The modifier for the Character_Action being used in that space. </param>
    /// <returns></returns>
    public float Calculate_Total_Modifier(GameObject target, float action_mod)
    {
        float modifier = 0;
        modifier += action_mod;
        //Debug.Log("Character_Action modifier is: " + action_mod);
        modifier += character.accuracy;
        //Debug.Log("Character Accuracy is: " + character.accuracy);
        modifier += Calculate_Height_Modifier(target);
        modifier += Calclulate_Orientation_Modifier(target);
        modifier += Calculate_Weapon_Modifier(target);
        modifier += Calculate_Combo_Modifier(target);
        Character_Script target_character = target.GetComponent<Character_Script>();
        if (target_character != null)
        {
            modifier -= target_character.resistance;
            //Debug.Log("Target Resistance is: " + target_character.resistance);
        }

        //Debug.Log("Character Lethality is: " + character.lethality);
        if (modifier > character.lethality)
        {
            modifier = character.lethality;
        }

        //Debug.Log("Character Finesse is: " + character.finesse);
        if(modifier >= character.finesse)
        {
            //Debug.Log("Critical Hit!");
        }

        //Debug.Log("Total Modifier is: " + modifier);
        return modifier;
    }

    /// <summary>
    /// Calculate the portion of the Ability Modifier derived from the character's and target's Tile Elevation.
    /// </summary>
    /// <param name="target">The target of the Character_Action.</param>
    /// <returns>The Height Modifier.</returns>
    public float Calculate_Height_Modifier(GameObject target)
    {
        float modifier = 0;
        Character_Script target_character = target.GetComponent<Character_Script>();
        if (target_character == null)
        {
            Tile tile = target.GetComponent<Tile>();
            if (tile != null)
            {
                GameObject obj = tile.obj;
                if (obj != null)
                {
                    target_character = obj.GetComponent<Character_Script>();
                }
            }
        }
        if (target_character != null)
        {
            if(target_character.curr_tile != null &&
                character.curr_tile != null)
            {
                modifier = 0.125f * (character.curr_tile.GetComponent<Tile>().height - target_character.curr_tile.GetComponent<Tile>().height);
                if (modifier > 0.25f)
                {
                    modifier = 0.25f;
                }
                else if (modifier < -0.25f)
                {
                    modifier = -0.25f;
                }
            }
        }
        //Debug.Log("Height Modifier is: " + modifier);
        return modifier;
    }

    /// <summary>
    /// Calculate the portion of the Ability Modifier derived from the character's and target's Orientation.
    /// </summary>
    /// <param name="target">The target of the Character_Action</param>
    /// <returns>The Orientation modifier.</returns>
    public float Calclulate_Orientation_Modifier(GameObject target)
    {
        float modifier = 0;
        Character_Script target_character = target.GetComponent<Character_Script>();
        if (target_character == null)
        {
            Tile tile = target.GetComponent<Tile>();
            if (tile != null)
            {
                GameObject obj = tile.obj;
                if (obj != null)
                {
                    target_character = obj.GetComponent<Character_Script>();
                }
            }
        }
        if (target_character != null)
        {
            Tile target_tile_data = target_character.curr_tile.GetComponent<Tile>();
            Tile char_tile_data = character.curr_tile.GetComponent<Tile>();
            if (char_tile_data != null && 
                target_tile_data != null)
            {
                if (target_character.orientation == 0)
                {
                    //Striking from the sides
                    if ((char_tile_data.index[0] > target_tile_data.index[0] &&
                        Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0])) ||
                        (char_tile_data.index[0] < target_tile_data.index[0] &&
                        Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0])))
                    {
                        modifier = .125f;
                    }
                    //Striking from behind
                    if (char_tile_data.index[1] > target_tile_data.index[1] &&
                        Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]) <= Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]))
                    {
                        modifier = .25f;
                    }
                    //Otherwise we are striking from the front and don't update the modifier.
                }
                else if (target_character.orientation == 1)
                {
                    //Striking from the sides
                    if ((char_tile_data.index[1] > target_tile_data.index[1] &&
                        Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]) <= Math.Abs(char_tile_data.index[1] - target_tile_data.index[1])) ||
                        (char_tile_data.index[1] < target_tile_data.index[1] &&
                        Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]) <= Math.Abs(char_tile_data.index[1] - target_tile_data.index[1])))
                    {
                        modifier = .125f;
                    }
                    //Striking from behind
                    if (char_tile_data.index[0] < target_tile_data.index[0] &&
                        Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]))
                    {
                        modifier = .25f;
                    }
                    //Otherwise we are striking from the front and don't update the modifier.
                }
                else if (target_character.orientation == 2)
                {
                    //Striking from the sides
                    if ((char_tile_data.index[0] > target_tile_data.index[0] &&
                        Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0])) ||
                        (char_tile_data.index[0] < target_tile_data.index[0] &&
                        Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0])))
                    {
                        modifier = .125f;
                    }
                    //Striking from behind
                    if (char_tile_data.index[1] < target_tile_data.index[1] &&
                        Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]))
                    {
                        modifier = .25f;
                    }
                    //Otherwise we are striking from the front and don't update the modifier.
                }
                else if (target_character.orientation == 3)
                {
                    //Striking from the sides
                    if ((char_tile_data.index[1] > target_tile_data.index[1] &&
                        Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]) <= Math.Abs(char_tile_data.index[1] - target_tile_data.index[1])) ||
                        (char_tile_data.index[1] < target_tile_data.index[1] &&
                        Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]) <= Math.Abs(char_tile_data.index[1] - target_tile_data.index[1])))
                    {
                        modifier = .125f;
                    }
                    //Striking from behind
                    if (char_tile_data.index[0] > target_tile_data.index[0] &&
                        Math.Abs(char_tile_data.index[1] - target_tile_data.index[1]) <= Math.Abs(char_tile_data.index[0] - target_tile_data.index[0]))
                    {
                        modifier = .25f;
                    }
                    //Otherwise we are striking from the front and don't update the modifier.
                }
            }
        }
        //Debug.Log("Facing Modifier is: " + modifier);
        return modifier;
    }

    /// <summary>
    /// Calculate the portion of the Ability Modifier derived from the character's Weapon Preferred spaces.
    /// </summary>
    /// <param name="target">The target of the Character_Action</param>
    /// <returns>The weapon modifier.</returns>
    public float Calculate_Weapon_Modifier(GameObject target)
    {
        float modifier = 0;
        Character_Script target_character = target.GetComponent<Character_Script>();
        if (target_character == null)
        {
            Tile tile = target.GetComponent<Tile>();
            if (tile != null)
            {
                GameObject obj = tile.obj;
                if (obj != null)
                {
                    target_character = obj.GetComponent<Character_Script>();
                }
            }
        }
        if (target_character != null)
        {
            Tile char_tile = character.curr_tile.GetComponent<Tile>();
            Tile tar_tile = target_character.curr_tile.GetComponent<Tile>();
            //Debug.Log(character.weapon.name + " range is " + character.weapon.modifier.GetLength(0)/2);
            int range = character.weapon.modifier.GetLength(0)/2;
            int diff_x = Math.Abs(char_tile.index[0] - tar_tile.index[0]);
            int diff_y = Math.Abs(char_tile.index[1] - tar_tile.index[1]);
            if (diff_x + range < character.weapon.modifier.GetLength(0) && 
                diff_x >= 0 && 
                diff_y + range < character.weapon.modifier.GetLength(0) && 
                diff_y >= 0)
            {
                modifier = character.weapon.modifier[diff_x + range , diff_y + range];
            }
            //Debug.Log("Weapon Modifier is: " + modifier);
        }
        return modifier;
    }

    /// <summary>
    /// Calculate the portion of the Ability Modifier derived from the target's Combo Modifier
    /// </summary>
    /// <param name="target">The target of the Character_Action</param>
    /// <returns>the combo modifier</returns>
    public float Calculate_Combo_Modifier(GameObject target)
    {
        float modifier = 0;
        Character_Script target_character = target.GetComponent<Character_Script>();
        Tile tile = target.GetComponent<Tile>();
        if (tile != null)
        {
            GameObject obj = tile.obj;
            if (obj != null)
            {
                target_character = obj.GetComponent<Character_Script>();
            }
        }
        if (target_character != null)
        {
            modifier = target_character.combo_mod;
        }
        return modifier;
    }

    /// <summary>
    /// Checks if the an Character_Action can be used on a specific Tile. 
    /// For instance can't use a Damage Character_Action on a space with no valid Targets.
    /// </summary>
    /// <param name="target_tile">The Tile around which the Character_Action is occurring</param>
    /// <returns>True if using the Character_Action on the current target tile is Valid. False otherwise.</returns>
    public bool Check_Valid(GameObject target_tile)
    {
        if (target_effect != null)
        {
            foreach (Action_Effect eff in target_effect)
            {
                if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                {

                    //target_character.MoveTo(target_tile.transform);
                }
                else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                {
                    if (Get_Targets(target_tile) == null)
                    {
                        Debug.Log("Invalid tile selected");
                        return false;
                    }
                }
                else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                {
                    if (target_tile.GetComponent<Tile>().obj == null)
                    {
                        Debug.Log("Invalid tile selected");
                        return false;
                    }
                }
                else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                {
                    if (target_tile.GetComponent<Tile>().obj == null)
                    {
                        Debug.Log("Invalid tile selected");
                        return false;
                    }
                }
                else if (eff.type.ToString() == Action_Effect.Types.Elevate.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Enable.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Pass.ToString())
                {
                }
            }
        }
        if (self_effect != null)
        {
            foreach (Action_Effect eff in self_effect)
            {
                if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                {
                    if (target_tile.GetComponent<Tile>().obj != null)
                    {
                        Debug.Log("Invalid tile selected");
                        return false;
                    }
                }
                else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Elevate.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Enable.ToString())
                {
                }
                else if (eff.type.ToString() == Action_Effect.Types.Pass.ToString())
                {
                }
            }
        }
        //Debug.Log("Valid tile selected");
        return true;
    }

    /// <summary>
    /// Add the Reaction to the Event_Manager list. This makes the reaction triggerable.
    /// </summary>
    public void Enable_Reaction()
    {
        Event_Manager.AddHandler(trigger, React);
    }

    /// <summary>
    /// Disable the Reaction in the Event_Manager list. This makes the reaction untriggerable.
    /// </summary>
    public void Disable_Reaction()
    {
        Event_Manager.RemoveHandler(trigger, React);
    }

    /// <summary>
    /// Check if the condition for completing the Character_Action is valid. 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public bool Evaluate_Conditional(string condition, Character_Action act, string value, GameObject obj)
    {
        bool result = true;
        string[] values = condition.Split(' ');
        string substring = "";
        for (int x =0; x<values.Length; x++)
        {
            string str = values[x];
            if (str == "&")
            {
                for (int y = x+1; y< values.Length; y++)
                {
                    substring += values[y] + " ";
                }
                return (result && Evaluate_Conditional(substring, act, value, obj));
            }else if (str == "|")
            {
                for (int y = x + 1; y < values.Length; y++)
                {
                    substring += values[y] + " ";
                }
                return (result || Evaluate_Conditional(substring, act, value, obj));
            }
            else
            {
                Character_Script target_chara = obj.GetComponent<Character_Script>();
                if (str.Contains(Accepted_Tests.CHK_SRC_ENMY.ToString()))
                {
                    if (act.character != null)
                    {
                        result = act.character.Check_Tag("Character (Enemy)");
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_SRC_FRND.ToString()))
                {
                    if (act.character != null)
                    {
                        result = act.character.Check_Tag("Character (Friend)");
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_SRC_RANG.ToString()))
                {
                    if (act.character != null)
                    {
                        result = Check_Range(act.character);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_SRC_SELF.ToString()))
                {
                    if (act.character != null)
                    {
                        result = act.character.Equals(character);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_TARG_ENMY.ToString()))
                {
                    if (target_chara != null)
                    {
                        result = target_chara.Check_Tag("Character (Enemy)");
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_TARG_FRND.ToString()))
                {
                    
                    if (target_chara != null)
                    {
                        result = target_chara.Check_Tag("Character (Friend)");
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_TARG_RANG.ToString()))
                {
                    if (target_chara != null)
                    {
                        result = Check_Range(target_chara);
                    }
                    else
                    {
                        result = false;
                    }
                }
                else if (str.Contains(Accepted_Tests.CHK_TARG_SELF.ToString()))
                {

                    if (target_chara != null)
                    {
                        result = target_chara.Equals(character);
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Check to see if a character is in Range of the ability.
    /// </summary>
    /// <param name="chara">The Character whose position we need to check</param>
    /// <returns>True if in range, False otherwise</returns>
    public bool Check_Range(Character_Script chara)
    {
        //TODO add more detailed check.
        return true;
    }

    public bool Check_Resource()
    {
        //Debug.Log("type " + type + " character actions " + character.action_curr);

        if ((type == Activation_Types.Active && character.action_curr >= (int)Convert_To_Double(ap_cost, null)) || 
            (type == Activation_Types.Reactive && character.reaction_curr >= (int)Convert_To_Double(ap_cost, null)))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Trigger a Reaction type action.
    /// </summary>
    /// <param name="act">The action being taken</param>
    /// <param name="value">The value of the action being taken</param>
    /// <param name="target">The target of the action</param>
    public void React(Character_Action act, string value, GameObject target)
    {
        if (Evaluate_Conditional(condition, act, value, target) && Check_Resource())
        {
            //Debug.Log("Character " + character.name + " is reacting with " + name);
            character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().Pause();
            //Debug.Log("Pausing Character " + character.controller.curr_scenario.curr_player.Peek().name + "'s current action " + character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().name);
            //Debug.Log("Character action count " + character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count);
            character.controller.curr_scenario.curr_player.Push(character.gameObject);
            character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Push(this);
            Character_Script target_character = target.GetComponent<Character_Script>();
            character.StartCoroutine(character.Act(this, act.character.curr_tile));
        }

    }

    public void End_Reaction()
    {
        //Debug.Log("Ending Reaction of " + character.name);
        character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Pop();
        character.controller.curr_scenario.curr_player.Pop();
        //Debug.Log("Current player: " + character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count);
        if (character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count > 0)
        {
            character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().Resume();
        }
    }

    /// <summary>
    /// Coroutine for actually using the Character_Action.
    /// Character_Action must be Validated before Enacting (Check_Valid()):
    /// Calls the various Enact_<>() Functions depending on the Character_Actions's Action_Effects. 
    /// </summary>
    /// <param name="target_tile">The Tile on which to Enact the Character_Action. </param>
    /// <returns>An IEnumerator with the current Coroutine progress. </returns>
    public IEnumerator Enact(GameObject target_tile)
    {
        if (Check_Valid(target_tile))
        {
            float duration = 1;
            AnimationClip anim_clip = new AnimationClip();
            if (type == Activation_Types.Active)
            {
                anim_clip = character.GetComponent<Animator>().runtimeAnimatorController.animationClips[anim_num - 99];
                duration = anim_clip.length;
            }
            else if (type == Activation_Types.Reactive)
            {
                anim_clip = character.GetComponent<Animator>().runtimeAnimatorController.animationClips[anim_num - 194];
                duration = anim_clip.length;
                //Debug.Log(anim_clip.name);
            }
            //Debug.Log("Animation playing: " + anim_clip.name+ " looping? " + anim_clip.isLooping);
            float elapsedTime = 0;
            
            //Debug.Log("Duration " + duration);
            //Debug.Log("Animation loops " + character.GetComponent<Animator>().runtimeAnimatorController.animationClips[anim_num - 99].isLooping);
            if (target_effect != null)
            {
                //Stack<Action> Functions = new Stack<Action>();

                //Find targets in AoE
                List<Target> targets = Get_Targets(target_tile);
                //Functions.Push(Enact_Damage("", targets[0]));
                if (targets != null)
                {
                    foreach (Target target in targets)
                    {
                        if (target.game_object.GetComponent<Character_Script>())
                        {
                            target.game_object.GetComponent<Character_Script>().Increase_Combo_Mod();
                        }
                    }
                }

                //Begin animation
                character.GetComponent<Animator>().SetInteger("Anim_Num", anim_num);
                character.GetComponent<Animator>().SetTrigger("Act");

                foreach (Action_Effect eff in target_effect)
                {
                    elapsedTime = 0;
                    //Debug.Log(character.name + " " + name + "Num of target effects: " + target_effect.Count);
                    //Debug.Log(character.name + " " + name + "effect_num " + proc_effect_num);
                    //Debug.Log(character.name + " " + name + "effect type " + eff.type);
                    while (proc_effect_num == 0 || paused)
                    {
                        //Escape if too much time has passed.
                        elapsedTime += Time.deltaTime;
                        if (elapsedTime > duration && ! anim_clip.isLooping)
                        {
                            //Debug.Log(character.name + " " + name + " Escaping");
                            elapsedTime = 0;
                            proc_effect_num = -1;
                        }
                        //Debug.Log(character.name + " " + name + "Paused? " + paused);
                        //Debug.Log(character.name + " " + name + "Proc num " + proc_effect_num);
                        yield return new WaitForEndOfFrame();
                    }
                    
                    if (eff.type == Action_Effect.Types.Move)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;

                        // find the target to move towards
                        int centerX = character.curr_tile.GetComponent<Tile>().index[0];
                        int centerY = character.curr_tile.GetComponent<Tile>().index[1];
                        //Target tile = new Target(character.curr_tile.gameObject, 0);
                        if (center == "target")
                        {
                            centerX = target_tile.GetComponent<Tile>().index[0] + (area.GetLength(0) / 2);
                            centerY = target_tile.GetComponent<Tile>().index[1] + (area.GetLength(1) / 2);
                        }

                        foreach (Target target in targets)
                        {
                            while ( paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            int tile_x = 0;
                            int tile_y = 0;
                            if (target.game_object.GetComponent<Character_Script>())
                            {
                                tile_x = (int)(centerX +
                                        (target.game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile>().index[0] - centerX) *
                                        target.modifier *
                                        Convert_To_Double(eff.value[1], target.game_object));
                                tile_y = (int)(centerY +
                                        (target.game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile>().index[1] - centerY) *
                                        target.modifier *
                                        Convert_To_Double(eff.value[1], target.game_object));
                                if (tile_x < 0)
                                {
                                    tile_x = 0;
                                }
                                if (tile_y < 0)
                                {
                                    tile_y = 0;
                                }
                                if (tile_x > character.controller.curr_scenario.tile_grid.grid_length)
                                {
                                    tile_x = character.controller.curr_scenario.tile_grid.grid_length;
                                }
                                if (tile_y > character.controller.curr_scenario.tile_grid.grid_width)
                                {
                                    tile_y = character.controller.curr_scenario.tile_grid.grid_width;
                                }
                                Target tile = new Target(character.controller.curr_scenario.tile_grid.getTile(tile_x, tile_y).gameObject, target.modifier);
                                //TODO: Add a way to move a target to a specific tile.
                                //Enact_Move(target.game_object.GetComponent<Character_Script>(), eff.value[0], tile);
                            }
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Damage)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        foreach (Target target in targets)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Damage(eff.value[0], target);
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Heal)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        foreach (Target target in targets)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Healing(eff.value, target);
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Status)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        foreach (Target target in targets)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Status(eff.value, target);
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Effect)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        List<Target> target_tiles = Get_Target_Tiles(target_tile);
                        foreach (Target target in target_tiles)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Effect(eff.value, target);
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Elevate)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        foreach (Target target in targets)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Elevate(eff.value[0], target);
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Enable)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        foreach (Target target in targets)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Enable(eff.value, target);
                        }
                    }
                    else if (eff.type == Action_Effect.Types.Pass)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        List<Target> target_tiles = Get_Target_Tiles(target_tile);
                        foreach (Target target in target_tiles)
                        {
                            while (paused)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            Enact_Pass(target);
                        }
                    }
                }
            }
            if (self_effect != null)
            {
                foreach (Action_Effect eff in self_effect)
                {
                    elapsedTime = 0;
                    //Debug.Log(character.name + " " + name + "Num of self effects: " + self_effect.Count);
                    //Debug.Log(character.name + " " + name + "Effect num " + proc_effect_num);
                    //Debug.Log(character.name + " " + name + "Effect type " + eff.type);
                    while (proc_effect_num == 0 || paused)
                    {
                        //Escape if too much time has passed.
                        elapsedTime += Time.deltaTime;
                        if (elapsedTime > duration && !anim_clip.isLooping)
                        {
                            //Debug.Log(character.name + " " + name + "Escaping");
                            elapsedTime = 0;
                            proc_effect_num = -1;
                        }
                        //Debug.Log(character.name + " " + name + "Paused? " + paused);
                        //Debug.Log(character.name + " " + name + "Proc num " + proc_effect_num);
                        yield return new WaitForEndOfFrame();
                    }
                    //TODO FIND A WAY TO CARRY FORWARD MODIFIER FOR SELF EFFECTS
                    Target target = new Target(character.gameObject, area[0, 0]);
                    if (eff.type == Action_Effect.Types.Move)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Move(eff.value[0], new Target(target_tile, area[0, 0]));
                    }
                    else if (eff.type == Action_Effect.Types.Damage)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Damage(eff.value[0], target);
                    }
                    else if (eff.type == Action_Effect.Types.Heal)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Healing(eff.value, target);
                    }
                    else if (eff.type == Action_Effect.Types.Status)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Status(eff.value, target);
                    }
                    else if (eff.type == Action_Effect.Types.Effect)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Effect(eff.value, target);
                    }
                    else if (eff.type == Action_Effect.Types.Elevate)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Elevate(eff.value[0], new Target(character.curr_tile.gameObject, 0));
                    }
                    else if (eff.type == Action_Effect.Types.Enable)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Enable(eff.value, target);
                    }
                    else if (eff.type == Action_Effect.Types.Pass)
                    {
                        //Proc the effect
                        proc_effect_num -= 1;
                        while (paused)
                        {
                            //TODO add an interrupt here.
                            yield return new WaitForEndOfFrame();
                        }
                        Enact_Pass(target);
                    }

                }
            }

            //Debug.Log("State " + character.state);
            /*if(character.state != Character_States.Walking)
            {
                //Reset character state when actions are done
                character.state = Character_States.Idle;
            }*/

            //Wait for animation and effects to end to reset the character state.
            //while(!character.GetComponent<Animator>(). GetCurrentAnimatorClipInfo(0) .animationClips[2])

            while (character.state == Character_States.Walking)
            {
                //Debug.Log("State " + character.state.ToString());
                yield return new WaitForEndOfFrame();
            }

            //Return the character to idle state.
            character.state = Character_States.Idle;
            character.gameObject.GetComponent<Animator>().SetTrigger("Done_Acting");

            proc_effect_num = 0;

            if (type == Activation_Types.Reactive)
            {
                End_Reaction();
            }

        }
    }

    /// <summary>
    /// Sets the number of the animation for this Action in the Animator State Machine
    /// </summary>
    /// <param name="num"></param>
    public void Set_Anim_Num(int num)
    {
        anim_num = num;
        //Debug.Log("Set Animation Number to " + num);
    }

    

    /// <summary>
    /// Pause the current action
    /// </summary>
    public void Pause()
    {
        paused = true;
    }

    /// <summary>
    /// Resume the current Character_Action
    /// </summary>
    public void Resume()
    {
        paused = false;
    }

    /// <summary>
    /// What to do when a player Selects a Damage type Character_Action from the Action Menu.
    /// TODO ADD OTHER SELECT METHODS
    /// </summary>
    public void Select_Damage()
    {

    }

    /// <summary>
    /// Function to Enact a Move Type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="movetype">The Type of Movement the Character is performing. </param>
    /// <param name="target">The Target destination for the Move.</param>
    public void Enact_Move(String movetype, Target target)
    {
        //mover types
        // 1 = standard move
        // 2 = push/pull
        // 3 = fly
        // 4 = warp

        if (target.game_object.GetComponent<Tile>())
        {
            Tile target_tile = target.game_object.GetComponent<Tile>();
            Debug.Log("Character " + character.name + " Moved from: " + character.curr_tile.GetComponent<Tile>().index[0] + ","
                + character.curr_tile.GetComponent<Tile>().index[1] + " to: " + target.game_object.GetComponent<Tile>().index[0] + "," 
                + target_tile.index[1] + " Using " + target_tile.weight + " Speed.");
            
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
    /// Function to Enact a Damage type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="value">The String with the damage equation.</param>
    /// <param name="target">The Target being dealt damage. </param>
    public void Enact_Damage(String value, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            int damage = (int)(Convert_To_Double(value, target.game_object) * target.modifier);
            Debug.Log("original damage: " + Convert_To_Double(value, target.game_object));
            Debug.Log("Character " + character.character_name + " Attacked: " + target_character.character_name + "; Dealing " + damage + " damage and Using " + ap_cost + " AP");
            target_character.Take_Damage(damage, character.weapon.pierce);
            Event_Manager.Broadcast(Event_Trigger.ON_DAMAGE, this, value, target.game_object);
        }
        else
        {
            int damage = (int)(Convert_To_Double(value, target.game_object) * target.modifier);
            Debug.Log("Character " + character.character_name + " Attacked: OBJECT" + "; Dealing " + damage + " damage and Using " + ap_cost + " AP");
            Object_Script target_object = target.game_object.GetComponent<Object_Script>();
            target_object.Take_Damage(damage, character.weapon.pierce);
            Event_Manager.Broadcast(Event_Trigger.ON_DAMAGE, this, value, target.game_object);
        }
    }

    /// <summary>
    /// Function to Enact a Healing type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="value">The String with the healing equation.</param>
    /// <param name="target">The Target being Healed.</param>
    public void Enact_Healing(String[] value, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            int healing = (int)(Convert_To_Double(value[1], target.game_object) * target.modifier);
            if (value[0] == Accepted_Shortcuts.CAUC.ToString() || value[0] == Accepted_Shortcuts.TAUC.ToString())
            {
                Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " Aura, Using " + ap_cost + " AP");
                target_character.Recover_Aura(healing);
            }
            else if (value[0] == Accepted_Shortcuts.CMPC.ToString() || value[0] == Accepted_Shortcuts.TMPC.ToString())
            {
                Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " MP, Using " + ap_cost + " AP");
                target_character.Recover_Mana(healing);
            }
            else if (value[0] == Accepted_Shortcuts.CAPC.ToString() || value[0] == Accepted_Shortcuts.TAPC.ToString())
            {
                Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " AP, Using " + ap_cost + " AP");
                target_character.Recover_Actions(healing);
            }
            else
            {
                Debug.Log("Invalid Healing prefix.");
            }

        }
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Status type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="value">The equation for what Status to apply. </param>
    /// <param name="target">The Target affected by the Character_Action. </param>
    public void Enact_Status(String[] values, Target target)
    {
        //First we need to resolve the Condition
        //Check for a power and attribute
        double power = 0;
        string attribute = "";
        if (values.Length >= 3 && values[2] != null)
        {
            power = Convert_To_Double(values[2], target.game_object);
        }
        if (values.Length == 4 && values[3] != null)
        {
            attribute = values[3];
        }
        int duration = (int)Convert_To_Double(values[1], target.game_object);
        Condition condi = new Condition(values[0], duration, power*target.modifier, attribute);

        //Now we add the Condition to the target.
        Character_Script target_character = target.game_object.GetComponent<Character_Script>();
        target_character.Add_Condition(condi);

        Debug.Log("Character " + character.character_name + " Gave: " + target_character.character_name + 
            " " + condi.type.ToString() + " for " + condi.duration + " turns " + " with " + condi.power + 
            " power, for " + ap_cost + "AP") ;

        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Effect type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="value">The String with the equation for how much to Elevate.</param>
    /// <param name="target">The Target tile to affect. </param>
    public void Enact_Effect(String[] value, Target target)
    {
        if (target.game_object.GetComponent<Tile>())
        {
            if (target.game_object.GetComponent<Tile>().obj == null && target.game_object.GetComponent<Tile>().traversible)
            {
                string[] values = new string[value.Length - 3];
                for (int i = 3; i < value.Length; i++)
                {
                    if (value[i] != null)
                    {
                        if (value[2] != Action_Effect.Types.Heal.ToString() && value[2] != Action_Effect.Types.Status.ToString())
                        {
                            values[i - 3] = "" + Convert_To_Double(value[i], target.game_object);
                        }else
                        {
                            if (i == 3)
                            {
                                values[i - 3] = value[i];
                            }else
                            {
                                values[i - 3] = "" + Convert_To_Double(value[i], target.game_object);
                            }
                        }
                    }
                }
                int duration = (int)Convert_To_Double(value[1], target.game_object);
                Tile_Effect effect = new Tile_Effect(value[0], value[2], duration, values, target.modifier, target.game_object);
                effect.Instantiate();
                Debug.Log("Character " + character.character_name + " Created Effect: " + name + " on tile (" + target.game_object.GetComponent<Tile>().index[0] + "," + target.game_object.GetComponent<Tile>().index[1] + "); For " + duration + " and Using " + ap_cost + " AP");
            }
        }
        else
        {
            Debug.Log("Invalid target for Effect.");
        }
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Elevate type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="value">The String with the equation for how much to Elevate.</param>
    /// <param name="target">The Target tile to affect. </param>
    public void Enact_Elevate(String value, Target target)
    {
        if (target.game_object.GetComponent<Tile>())
        {
            int elevation = (int)(Convert_To_Double(value, target.game_object) * target.modifier);
            Debug.Log("Character " + character.character_name + " Elevated Tile: (" + target.game_object.GetComponent<Tile>().index[0] + "," + target.game_object.GetComponent<Tile>().index[1] + "); By " + elevation + " and Using " + ap_cost + " AP");
            character.controller.curr_scenario.tile_grid.Elevate(target.game_object.transform, elevation);
        }
        else
        {
            Debug.Log("Invalid target for Elevate.");
        }
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Enable type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="value">The String with the euqation for what to do. Should be an action name and a bool pair.</param>
    /// <param name="target">The Target whose ability is being Enabled/Disabled.</param>
    public void Enact_Enable(String[] value, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            foreach (Character_Action act in target_character.GetComponent<Character_Script>().actions)
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
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Pass type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="target">The Target for the Character_Action.</param>
    public void Enact_Pass(Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            target_character.StartCoroutine(target_character.End_Turn());
        }
    }

}
