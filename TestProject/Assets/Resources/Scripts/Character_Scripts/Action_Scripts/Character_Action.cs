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
    public enum Operators { EQ, GT, LT, GTQ, LTQ }

    /// <summary>
    /// Variables:
    /// String name - Character_Action name
    /// String ap_cost - Unconverted Cost of the action in Character_Action Points 
    /// String mp_cost - Unconverted Cost of the action in Mana Points
    /// Action_Types type - Used to determine what tiles this Action can affect. Different types have different range selection criteria.
    /// String color - The color to turn the range tiles.
    /// String range - Unconverted number of tiles away the skill can target.
    /// String center - Unconverted where to center the Character_Action. Over a Target or over the Character's Self.
    /// float[,] area - The area of effect that the Character_Action will affect.
    /// String num_targets - The minimum amount of targets required to Enact the skill. Set to 0 for instant action.
    /// List<Target> curr_targets the current list of targets for the ability.
    /// List<Tile> curr_path The current path of the ability. 
    /// bool paused - If the Character_Action is paused or not. 
    /// List<Action_Effect> self_effect - The Effect the Character will have on itself when using this Character_Action
    /// List<Action_Effect> target_effect - the Effect the Character will have on a target when using this Character_Action
    /// Activation_Types action_type - The type of Activation for the Skill. Active skills must be Selected. Passive Skills are always active. Reactive Skills have a Trigger.
    /// Event_Trigger trigger - what event triggers the action if it is a reactive action. Example, ON_DAMAGE
    /// string condition - Under what condition a Reaction can be used after an event is fired. Example, CAUC > CAUM/2
    /// Origin_Types origin - Where the skill originates (for disabling purposes later)
    /// bool enabled - If the Character_Action is enabled or not
    /// String animation - The animation tied to the specific Character_Action.
    /// Character_Script character - The Character performing the Character_Action.
    /// </summary>

    public String name { get; private set; }
    public String ap_cost { get; private set; }
    public String mp_cost { get; private set; }
    public Action_Types action_type { get; private set; }
    public String color { get; private set; }
    public String range { get; private set; }
    public String center { get; private set; }
    public float[,] area { get; private set; }
    public String num_targets { get; private set; }
    public List<string> target_checks { get; private set; }
    public List<Target> curr_targets { get; private set;  }
    public List<Tile> curr_path { get; private set; }
    public bool paused { get; private set; }
    public List<Action_Effect> effects { get; private set; }
    public Activation_Types activation { get; private set; }
    public Event_Trigger trigger { get; private set; }
    public string condition { get; private set; }
    public Origin_Types origin { get; private set; }
    public bool enabled { get; set; }
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
        action_type = Action_Types.Melee;
        range = "";
        center = "";
        area = null;
        num_targets = "";
        target_checks = new List<string>();
        curr_targets = new List<Target>();
        curr_path = new List<Tile>();
        paused = false;
        effects = null;
        activation = Activation_Types.Active;
        trigger = Event_Trigger.ON_DAMAGE;
        condition = "";
        origin = Origin_Types.Innate;
        enabled = true;
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
        action_type = act.action_type;
        color = act.color;
        range = act.range;
        center = act.center;
        area = act.area;
        num_targets = act.num_targets;
        target_checks = act.target_checks;
        curr_targets = new List<Target>();
        curr_path = new List<Tile>();
        paused = act.paused;
        effects = act.effects;
        activation = act.activation;
        trigger = act.trigger;
        condition = act.condition;
        origin = act.origin;
        enabled = act.enabled;
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
            action_type == act.action_type &&
            range == act.range &&
            center == act.center &&
            area == act.area &&
            effects == act.effects &&
            activation == act.activation &&
            trigger == act.trigger &&
            condition == act.condition &&
            origin == act.origin &&
            enabled == act.enabled &&
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
    ///     type - The Action_Type of this Action. Determines what tiles this Action can affect. Eg Path type actions are limited by tile modifiers.
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
    ///         Eg: activation: active
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
        int line_num = 0;
        foreach (String s in input)
        {
            line_num++;
            if (s != null && s.Contains(":"))
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
                    case "type":
                        Array array = Enum.GetValues(typeof(Action_Types));
                        foreach (Action_Types ty in array)
                        {
                            if (values.Trim() == ty.ToString() || values.Trim() == ty.ToString().ToLower())
                            {
                                //Debug.Log("Action " + act.name + " action type: " + act.action_type);
                                act.action_type = ty;
                            }
                        }
                        break;
                    case "color":
                        act.color = values.Trim();
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
                    case "num_targets":
                        act.num_targets = values.TrimStart();
                        break;
                    case "target_checks":
                        foreach (string check in values.TrimStart().TrimEnd().Split(','))
                        {
                            act.target_checks.Add(check);
                        }
                        break;
                    case "effects":
                        int num_effects = 0;
                        int.TryParse(values.TrimStart().TrimEnd(),out num_effects);
                        //Debug.Log("Number of Effects: " + num_effects);
                        act.effects = new List<Action_Effect>();
                        for (int i = 0; i < num_effects; i++)
                        {
                            string str = input[line_num + i];
                            //Debug.Log(str);
                            act.effects.Add(new Action_Effect(str.TrimStart().TrimEnd()));

                        }
                        break;
                    case "activation":
                        if (values.Trim() == "Reactive" ||
                            values.Trim() == "reactive")
                        {
                            act.activation = Activation_Types.Reactive;
                        }
                        else if (values.Trim() == "Passive" ||
                            values.Trim() == "passive")
                        {
                            act.activation = Activation_Types.Passive;
                        }
                        else
                        {
                            act.activation = Activation_Types.Active;
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
                        array = Enum.GetValues(typeof(Event_Trigger));
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
        if(input.Contains("CEIL")){
            string[] start_string = input.Replace("CEIL", "").Split("[".ToCharArray(), 2);
            string[] end_string = start_string[1].Split("]".ToCharArray(), 2);
            /*foreach (string str in start_string)
            {
                Debug.Log("start " + str);
            }
            foreach (string str in end_string)
            {
                Debug.Log("end" + str);
            }*/
            return Mathf.Ceil((float)Convert_To_Double(end_string[0], target));
        }
        if (input.Contains("FLOOR")){
            return Mathf.Floor((float)Convert_To_Double(input.Split('[')[1].Split(']')[0], target));
        }
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
                        if (target.GetComponent<Character_Script>())
                        {
                            source = target.GetComponent<Character_Script>();
                        }
                        else
                        {
                            source = target.GetComponent<Tile>().GetComponent<Character_Script>();
                        }
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
                //Debug.Log("AP cost: " + (int)Convert_To_Double(ap_cost, character.gameObject));
                //Check to see if player can afford action:
                if (Check_Resource())
                {
                    //Debug.Log("Enough action points");
                    if (character.mana_curr >= (int)Convert_To_Double(mp_cost, null))
                    {
                        //Debug.Log("Enough mana points");
                        //character.curr_action.Push(this);
                        
                        if (action_type != Action_Types.None || 
                            Convert_To_Double(num_targets, null) > 0 )
                        {
                            Reset_Targets();
                            Reset_Path();

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

                            if (character.curr_action.Count != 0)
                            {
                                character.curr_action.Pop();
                            }

                            character.curr_action.Push(this);

                            Find_Reachable_Tiles();

                            Game_Controller.curr_scenario.Clean_Reachable();
                            Game_Controller.curr_scenario.Mark_Reachable();
                        }
                        else { 
                            //character.curr_action.Push(this);
                            character.StartCoroutine(character.Act(this, character.curr_tile));
                        }
                    }
                   
                    //character.controller.curr_scenario.Clean_Reachable();
                    //character.controller.Mark_Reachable();


                    //Debug.Log("Character " + character.name + " " + character.character_num + " current action " + character.curr_action.Peek().name);
                    //Debug.Log("Character " + character.name + " current action count " + character.curr_action.Count);
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

    /*
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
                    //Transform target = character.controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    Transform target = Game_Controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    if (target != null)
                    {
                        Target tar = new Target(target.gameObject, Calculate_Total_Modifier(target.gameObject, area[x, y]));
                        if (!curr_targets.Contains(tar))
                        {
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
                    //Transform target = character.controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    Transform target = Game_Controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    if (target != null)
                    {
                        if (target.GetComponent<Tile>().obj != null)
                        {
                            Target tar = new Target(target.GetComponent<Tile>().obj, Calculate_Total_Modifier(target.GetComponent<Tile>().obj, area[x, y]));
                            if (!curr_targets.Contains(tar))
                            {
                                targets.Add(tar);
                            }
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
    */

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
        modifier += Calculate_Weapon_Modifier(target);
        Tile target_tile;
        if (target.GetComponent<Tile>()) {
            target_tile = target.GetComponent<Tile>();
            if (target_tile.obj != null)
            {
                modifier += Calculate_Combo_Modifier(target);
                modifier += Calclulate_Orientation_Modifier(target);
                Character_Script target_character = target_tile.obj.GetComponent<Character_Script>();
                if (target_character != null)
                {
                    modifier -= target_character.resistance;
                }
                //Debug.Log("Target Resistance is: " + target_character.resistance);
            }
        }
        else
        {
            modifier += Calculate_Combo_Modifier(target);
            modifier += Calclulate_Orientation_Modifier(target);
            Character_Script target_character = target.GetComponent<Character_Script>();
            if (target_character != null)
            {
                modifier -= target_character.resistance;
            }
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
        Tile target_tile = target.GetComponent<Tile>();
        Character_Script target_character = target.GetComponent<Character_Script>();
        if (target_character != null)
        {
            target_tile = target_character.curr_tile.GetComponent<Tile>();
        }
        if (target_tile != null)
        {
            modifier = 0.125f * (character.curr_tile.GetComponent<Tile>().height - target_tile.height);
            if (modifier > 0.25f)
            {
                modifier = 0.25f;
            }
            else if (modifier < -0.25f)
            {
                modifier = -0.25f;
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
        Tile target_tile = target.GetComponent<Tile>();
        if (target_tile == null)
        {
            Character_Script target_character = target.GetComponent<Character_Script>();
            if (target_character != null)
            {
                target_tile = target_character.curr_tile.GetComponent<Tile>();
            }
            else
            {
                target_tile = target.GetComponent<Object_Script>().curr_tile.GetComponent<Tile>();
            }
        }
        Tile char_tile = character.curr_tile.GetComponent<Tile>();
        //Debug.Log(character.weapon.name + " range is " + character.weapon.modifier.GetLength(0)/2);
        int range = character.weapon.modifier.GetLength(0) / 2;
        int diff_x = Math.Abs(char_tile.index[0] - target_tile.index[0]);
        int diff_y = Math.Abs(char_tile.index[1] - target_tile.index[1]);
        if (diff_x + range < character.weapon.modifier.GetLength(0) &&
            diff_x >= 0 &&
            diff_y + range < character.weapon.modifier.GetLength(0) &&
            diff_y >= 0)
        {
            modifier = character.weapon.modifier[diff_x + range, diff_y + range];
        }
        //Debug.Log("Weapon Modifier is: " + modifier);
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

    /*
    /// <summary>
    /// Checks if the an Character_Action can be used on a specific Tile. 
    /// For instance can't use a Damage Character_Action on a space with no valid Targets.
    /// </summary>
    /// <param name="target_tile">The Tile around which the Character_Action is occurring</param>
    /// <returns>True if using the Character_Action on the current target tile is Valid. False otherwise.</returns>
    public bool Check_Valid(GameObject target_tile)
    {
        bool valid = false;
        if (effects != null)
        {
            foreach (Action_Effect eff in effects)
            {
                if (eff.target == Action_Effect.Target.target)
                {
                    if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                    {

                        //target_character.MoveTo(target_tile.transform);
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                    {
                        if (Get_Targets(target_tile) != null)
                        {
                            valid = true;
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                    {
                        if (target_tile.GetComponent<Tile>().obj != null)
                        {
                            valid = true;
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                    {
                        if (target_tile.GetComponent<Tile>().obj != null)
                        {
                            valid = true;
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
                else
                {
                    if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                    {
                        if (target_tile.GetComponent<Tile>().obj == null && 
                            target_tile.GetComponent<Tile>().traversible)
                        {
                            valid = true;
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
        }
        Debug.Log("Tile is: " + valid);
        return valid;
    }
    */

    public bool Check_Valid_Target(GameObject target)
    {
        Tile target_tile = target.GetComponent<Tile>();
        if (target_tile != null)
        {
            foreach(string check in target_checks)
            {
                if(!Parse_Check(check, target))
                {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Function to check the criteria for an Action_Effect is met. 
    /// </summary>
    /// <param name="checks">The check list for the Effect being invoked.</param>
    /// <param name="targ">The GameObject who will be affected by the Effect if the checks succeed.</param>
    /// <returns>True if the Effect is valid, False otherwise.</returns>
    public bool Check_Criteria(string[] checks, GameObject targ)
    {
        bool valid = true;
        foreach (string str in checks)
        {
            valid = Parse_Check(str, targ);
            if (!valid)
            {
                return valid;
            }
        }
        return valid;
    }

    /// <summary>
    /// Function to parse a Check into an actual condition. 
    /// EG: CHK_CCND_EQ_BLD returns true if the character has the condition Bleeding
    /// EG: CHK_NOT_TAUC_GT_CAUC returns true if the target's current aura points are not greater than the character's current aura points.
    /// </summary>
    /// <param name="input">The string to parse</param>
    /// <param name="targ">The GameObject affected by the Effect.</param>
    /// <returns>True if all checks have passed, false otherwise.</returns>
    public bool Parse_Check(string input, GameObject targ)
    {
        bool valid = true;
        bool not_flag = false;
        Operators ope;
        //Debug.Log("Check is " + input);
        string[] conditionals = input.Split('_');
        int i = 1;
        if (input == "NULL")
        {
            return true;
        }
        if (conditionals[i] == "NOT")
        {
            not_flag = true;
            i++;
        }
        if (conditionals[i] == "EMPTY")
        {
            //Debug.Log("Check if the target is empty");
            if (targ.GetComponent<Tile>())
            {
                if (targ.GetComponent<Tile>().obj != null)
                {
                    valid = false;
                }
                else
                {
                    valid = true;
                }
            }else
            {
                valid = false;
            }
        }
        if (conditionals[i] == "CHAR")
        {
            //Debug.Log("Check if the target contains a character");
            if (targ.GetComponent<Tile>())
            {
                if (targ.GetComponent<Tile>().obj != null)
                {
                    if (targ.GetComponent<Tile>().obj.GetComponent<Character_Script>())
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                    }
                }
                else
                {
                    valid = false;
                }
            }
            else
            {
                valid = false;
            }
        }
        if (conditionals[i] == "CCND")
        {
            //Debug.Log("Check if the character has condition");
            valid = character.Has_Condition(new Condition(conditionals[i + 2]).type);

        }
        if (conditionals[i] == "TCND")
        {
            Character_Script chara;
            if (targ.GetComponent<Character_Script>())
            {
                chara = targ.GetComponent<Character_Script>();
            }
            else
            {
                chara = targ.GetComponent<Tile>().obj.GetComponent<Character_Script>();
            }

            if (chara != null)
            {
                valid = chara.Has_Condition(new Condition(conditionals[i + 2]).type);
            }
            else
            {
                valid = false;
            }
            //Debug.Log("Check if the target has condition");
        }
        if (conditionals[i] == "IN" && conditionals[i+1] == "RANGE")
        {
            //Debug.Log("Check if the target is in range");
            valid = Check_In_Range(targ);
        }
        if (not_flag)
        {
            valid = !valid;
        }
        //Debug.Log("Condition is " + valid);
        return valid;
    }

    /// <summary>
    /// Check if a target is still in range
    /// </summary>
    /// <param name="target">The target to check</param>
    /// <returns>True if the target is still in the reachable tiles. False otherwise.</returns>
    public bool Check_In_Range(GameObject target)
    {
        bool in_range = false;
        //Determine if the target is a tile.
        Tile tile = target.GetComponent<Tile>();
        if (tile != null) {
            //Check if the target is in one of our Reachable tiles
            /*foreach (Tile ti in reachable_tiles)
            {
                if (tile.Equals(ti))
                {
                    return true;
                }
            }*/
        }

        //Determine if the target is a character
        Character_Script chara = target.GetComponent<Character_Script>();
        if (chara != null)
        {
            //Check if the target is in one of our Reachable tiles
            /*foreach (Tile ti in reachable_tiles)
            {
                if (chara.Equals(ti.obj))
                {
                    return true;
                }
            }*/
        }

        //Determine if the target is a character
        Object_Script obj = target.GetComponent<Object_Script>();
        if (obj != null)
        {
            //Check if the target is in one of our Reachable tiles
            /*foreach (Tile ti in reachable_tiles)
            {
                if (obj.Equals(ti.obj))
                {
                    return true;
                }
            }*/
        }
        return in_range;
    }

    /// <summary>
    /// Finds a path from the last tile in the curr_path for the Action to the given target Tile. Then updates curr_path with that path;
    /// </summary>
    /// <param name="target">The target. </param>
    public void Find_Path(GameObject target)
    {
        Stack<Tile> path = new Stack<Tile>();
        //Character_Script chara = target.GetComponent<Character_Script>();
        Tile target_tile = target.GetComponent<Tile>();
        //Debug.Log("Finding Path");
        if (target_tile != null)
        {
            if(action_type == Action_Types.Path)
            {
                //Debug.Log("Finding Path with Path rules");
                path = Game_Controller.curr_scenario.tile_grid.navmesh.FindPath(curr_path[curr_path.Count - 1], target_tile);
            }
            else if (action_type == Action_Types.Instant)
            {
                path.Push(target_tile);
            }
            else if (action_type == Action_Types.Ranged)
            {
                path.Push(target_tile);
            }
            else if (action_type == Action_Types.Melee)
            {
                path.Push(target_tile);
            }
            else if (action_type == Action_Types.Unrestricted_Path)
            {
                path = Game_Controller.curr_scenario.tile_grid.navmesh.FindPath(curr_path[curr_path.Count - 1], target_tile);
            }
            else if (action_type == Action_Types.Projectile)
            {
                path.Push(target_tile);
            }
            /*if (move_type == 4)
            {
                path.Push(clicked_tile.gameObject.GetComponent<Tile>());
            }else
            {
                path = Game_Controller.curr_scenario.tile_grid.navmesh.FindPath(curr_tile.GetComponent<Tile>(), clicked_tile.gameObject.GetComponent<Tile>());
            }*/
        }

        //Debug.Log("Path size " + path.Count);
        //Add the Tiles in the stack to the current path.
        while (path.Count !=0)
        {
            Tile path_tile = path.Pop();
            //Debug.Log("Tile index: [" + path_tile.index[0] + "," + path_tile.index[1] + "]");
            //character.curr_action.Peek().curr_path.Add(curr_scenario.clicked_tile.GetComponent<Tile>());
            curr_path.Add(path_tile);
        }
    }

    public List<Tile> Find_Reachable_Tiles()
    {
        List<Tile> tiles = new List<Tile>();
        if (action_type == Action_Types.Path)
        {
            //character.controller.curr_scenario.Find_Reachable((int)character.speed, (int)Convert_To_Double(range, null), 1);
            Game_Controller.curr_scenario.Find_Reachable((int)Convert_To_Double(range, null), (int)Convert_To_Double(range, null), 1);
        }
        else if (action_type == Action_Types.Unrestricted_Path)
        {
            //character.controller.curr_scenario.Find_Reachable((int)character.speed * 2, (int)Convert_To_Double(range, null), 2);
            Game_Controller.curr_scenario.Find_Reachable((int)character.speed * 2, (int)Convert_To_Double(range, null), 2);
        }
        else if (action_type == Action_Types.Ranged)
        {
            //character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null), 3);
            Game_Controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null), 3);
        }
        else if (action_type == Action_Types.Melee)
        {
            //character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null), 3);
            Game_Controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null), 3);
        }
        else if (action_type == Action_Types.Projectile)
        {
            //character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null), 3);
            Game_Controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, null), 3);
        }
        else if (action_type == Action_Types.Instant)
        {
            //character.controller.curr_scenario.Find_Reachable((int)character.speed * 2, (int)Convert_To_Double(range, null), 2);
            Game_Controller.curr_scenario.Find_Reachable((int)Convert_To_Double(range, null), (int)Convert_To_Double(range, null), 2);
        }
        else if (action_type == Action_Types.None)
        {
            //character.controller.curr_scenario.Find_Reachable((int)character.speed * 2, (int)Convert_To_Double(range, null), 2);
            Game_Controller.curr_scenario.Find_Reachable((int)Convert_To_Double(range, null), (int)Convert_To_Double(range, null), 3);
        }
        return tiles;
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
                Character_Script target_chara = null;
                if (obj.GetComponent<Character_Script>())
                {
                    target_chara = obj.GetComponent<Character_Script>();
                }
                else
                {
                    target_chara = obj.GetComponent<Tile>().obj.GetComponent<Character_Script>();
                }
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

    /// <summary>
    /// Check if the character has enough Action Points or Reaction points to perform the action.
    /// </summary>
    /// <returns>True if the character has enough AP/RP. False otherwise.</returns>
    public bool Check_Resource()
    {
        //Debug.Log("type " + type + " character actions " + character.action_curr);

        if ((activation == Activation_Types.Active && character.action_curr >= (int)Convert_To_Double(ap_cost, null)) || 
            (activation == Activation_Types.Reactive && character.reaction_curr >= (int)Convert_To_Double(ap_cost, null)))
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
            //character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().Pause();
            Game_Controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().Pause();
            //Debug.Log("Pausing Character " + character.controller.curr_scenario.curr_player.Peek().name + "'s current action " + character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().name);
            //Debug.Log("Character action count " + character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count);
            //character.controller.curr_scenario.curr_player.Push(character.gameObject);
            Game_Controller.curr_scenario.curr_player.Push(character.gameObject);
            //character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Push(this);
            Add_Target(act.character.curr_tile.gameObject);
            Game_Controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Push(this);
            Character_Script target_character = target.GetComponent<Character_Script>();
            character.StartCoroutine(character.Act(this, act.character.curr_tile));
        }

    }

    public void End_Reaction()
    {
        //Debug.Log("Ending Reaction of " + character.name);
        //character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Pop();
        Game_Controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Pop();
        //character.controller.curr_scenario.curr_player.Pop();
        Game_Controller.curr_scenario.curr_player.Pop();
        //Debug.Log("Current player: " + character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count);
        //if (character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count > 0)
        Reset_Targets();
        Reset_Path();
        if (Game_Controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Count > 0)
        {
            //character.controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().Resume();
            Game_Controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().curr_action.Peek().Resume();
        }
    }

    /// <summary>
    /// Adds a target tile to the list of targets if it is a Valid target.
    /// <param name="target_tile">The target to add to the list of the Action's current Targets.</param>
    /// </summary>
    public void Add_Target(GameObject target_tile)
    {
        Tile tile = character.curr_tile.GetComponent<Tile>();
        if (center == "Target") {
            tile = target_tile.GetComponent<Tile>();
        }
        if (tile != null)
        {
            if (Check_Valid_Target(target_tile))
            {
                Add_Waypoint(target_tile);
                Target target = new Target(tile, area);
                float[] modifiers = new float[2];
                foreach (Tile affected_tile in target.affected_tiles.Keys)
                {
                    modifiers[0] = Calculate_Total_Modifier(affected_tile.gameObject, target.affected_tiles[affected_tile][0]);
                    if (affected_tile.obj != null)
                    {
                        modifiers[1] = Calculate_Total_Modifier(affected_tile.obj, target.affected_tiles[affected_tile][1]);
                    }
                    target.Set_Modifiers(affected_tile, modifiers);
                }
                //Find_Path(target_tile);
                //Debug.Log("curr path " + curr_path.Count);
                target.Set_Path(curr_path);
                //Debug.Log("target path " + target.curr_path.Count);
                //Reset_Path();
                curr_targets.Add(target);
            }
        }
    }

    /// <summary>
    /// Adds a step in the current path for the Action
    /// </summary>
    /// <param name="obj">The target that would be added.</param>
    /// <returns>True if the waypoint was added successfully, False otherwise.</returns>
    public bool Add_Waypoint(GameObject obj)
    {
        bool added = false;
        Tile target_tile = obj.GetComponent<Tile>();
        if (target_tile != null)
        {
            if(action_type == Action_Types.Path)
            {
                if(target_tile.traversible)
                {
                    Find_Path(obj);
                    added = true;
                }
            }
            else
            {
                Find_Path(obj);
                added = true;
            }
        }
        return added;
    }

    /// <summary>
    /// Returns a bool depending on whether or not an Action has enough Targets to perform its Action.
    /// </summary>
    /// <returns>True if the Action has a number of valid targets between its minimum and maximum, False otherwise.</returns>
    public bool Has_Valid_Targets()
    {
        bool valid = false;
        if (curr_targets.Count >= Convert_To_Double(num_targets, null))
        {
            valid = true;
        }
        return valid;
    }

    /// <summary>
    /// Resets the action's current list of targets
    /// </summary>
    public void Reset_Targets()
    {
        curr_targets = new List<Target>();
    }

    /// <summary>
    /// Resets the action's current path
    /// </summary>
    public void Reset_Path()
    {
        curr_path = new List<Tile>();
        curr_path.Add(character.curr_tile.GetComponent<Tile>());
    }

    /// <summary>
    /// Coroutine for actually using the Character_Action.
    /// Character_Action must be Validated before Enacting (Check_Valid()):
    /// Calls the various Enact_<>() Functions depending on the Character_Actions's Action_Effects. 
    /// </summary>
    /// <param name="target_tile">The Tile on which to Enact the Character_Action. </param>
    /// <returns>An IEnumerator with the current Coroutine progress. </returns>
    public IEnumerator Enact()
    {
        float duration = 1;
        AnimationClip anim_clip = new AnimationClip();
        if (activation == Activation_Types.Active)
        {
            anim_clip = character.GetComponent<Animator>().runtimeAnimatorController.animationClips[anim_num - 99];
            duration = anim_clip.length;
        }
        else if (activation == Activation_Types.Reactive)
        {
            anim_clip = character.GetComponent<Animator>().runtimeAnimatorController.animationClips[anim_num - 194];
            duration = anim_clip.length;
            //Debug.Log(anim_clip.name);
        }
        //Debug.Log("Animation playing: " + anim_clip.name+ " looping? " + anim_clip.isLooping);
        float elapsedTime = 0;

        //Debug.Log("Duration " + duration);
        //Debug.Log("Animation loops " + character.GetComponent<Animator>().runtimeAnimatorController.animationClips[anim_num - 99].isLooping);
        if (effects != null)
        {
            //Stack<Action> Functions = new Stack<Action>();

            //TODO find a way to carry over self modifier
            float[,] self_mod = new float[1, 1];
            self_mod[0, 0] = Calculate_Total_Modifier(character.gameObject, 1);
            Target self = new Target(character.curr_tile.GetComponent<Tile>(), self_mod);



            //Functions.Push(Enact_Damage("", targets[0]));
            if (curr_targets.Count > 0)
            {
                foreach (Target target in curr_targets)
                {
                    //TODO eventually add combo mods to Tile scripts as well.
                    foreach (Character_Script chara in target.affected_characters.Keys)
                    {
                        chara.Increase_Combo_Mod();
                    }
                }
            }

            //Begin animation
            character.GetComponent<Animator>().SetInteger("Anim_Num", anim_num);
            proc_effect_num = 1;
            character.GetComponent<Animator>().SetTrigger("Act");
            int target_index = 0;
            foreach (Target target in curr_targets)
            {
                foreach (Action_Effect eff in effects)
                {
                    if (eff.target_limit[0] <= target_index &&
                       eff.target_limit[1] >= target_index)
                    {
                        //Debug.Log("Taking effect type: " + eff.type);
                        elapsedTime = 0;

                        List<Target> temp_targets = new List<Target>();
                        if (eff.target == Action_Effect.Target.target)
                        {
                            temp_targets.Add(target);
                        }
                        else if (eff.target == Action_Effect.Target.self)
                        {
                            temp_targets.Add(self);
                        }
                        else if (eff.target == Action_Effect.Target.path)
                        {
                            List<Target> path_targets = new List<Target>();
                            foreach (Tile tile in target.curr_path)
                            {
                                //find a way to specify path modifier.
                                float[,] path_mod = new float[1, 1];
                                path_mod[0, 0] = Calculate_Total_Modifier(tile.gameObject, 0.5f);
                                path_targets.Add(new Target(tile, path_mod));
                            }

                            temp_targets = path_targets;
                        }
                        //Debug.Log(character.name + " " + name + "Num of target effects: " + effects.Count);
                        //Debug.Log(character.name + " " + name + "effect_num " + proc_effect_num);
                        //Debug.Log(character.name + " " + name + "effect type " + eff.type);
                        while (proc_effect_num == 0 || paused)
                        {
                            //Escape if too much time has passed.
                            elapsedTime += Time.deltaTime;
                            if (elapsedTime > duration && !anim_clip.isLooping)
                            {
                                //Debug.Log(character.name + " " + name + " Escaping");
                                elapsedTime = 0;
                                proc_effect_num = -1;
                            }
                            //Debug.Log(character.name + " " + name + "Paused? " + paused);
                            //Debug.Log(character.name + " " + name + "Proc num " + proc_effect_num);
                            yield return new WaitForEndOfFrame();
                        }

                        //stop procing if Character is Orienting.
                        while (character.state == Character_States.Walking ||
                            character.state == Character_States.Orienting)
                        {
                            yield return new WaitForEndOfFrame();
                        }

                        if (eff.type == Action_Effect.Types.Orient)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Orient(eff, effect_target, target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Move)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Move(eff, effect_target, target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Damage)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Damage(eff, effect_target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Heal)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Healing(eff, effect_target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Status)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Status(eff, effect_target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Effect)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Effect(eff, effect_target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Elevate)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Elevate(eff, effect_target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Enable)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;

                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Enable(eff, effect_target));
                            }
                        }
                        else if (eff.type == Action_Effect.Types.Pass)
                        {
                            //Proc the effect
                            proc_effect_num -= 1;
                            foreach (Target effect_target in temp_targets)
                            {
                                character.StartCoroutine(Enact_Pass(eff.checks, effect_target));
                            }
                        }
                    }
                }
                target_index += 1;
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

        while (character.state == Character_States.Walking || 
            character.state == Character_States.Orienting)
        {
            //Debug.Log("State " + character.state.ToString());
            yield return new WaitForEndOfFrame();
        }

        //Return the character to idle state.
        character.state = Character_States.Idle;
        character.gameObject.GetComponent<Animator>().SetTrigger("Done_Acting");

        proc_effect_num = 0;

        //Reset the action's targets and Path.
        Reset_Targets();
        Reset_Path();

        if (activation == Activation_Types.Reactive)
        {
            End_Reaction();
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
    /// Function to Enact an Orient type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The effect to perform.</param>
    /// <param name="to_orient">The GameObject to orient.</param>
    /// <param name="target">The currently selected Target. May be necessary to figure out where to Orient.</param>
    public IEnumerator Enact_Orient(Action_Effect effect, Target to_orient, Target target)
    {
        foreach (Tile target_tile in to_orient.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                if (target_tile.obj.GetComponent<Character_Script>())
                {
                    Character_Script target_character = target_tile.obj.GetComponent<Character_Script>();
                    //Debug.Log(effect.values[0]);
                    if(effect.values[0] == "Select")
                    {
                        target_character.StartCoroutine(target_character.Choose_Orientation());
                        while(target_character.state == Character_States.Orienting)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    else if (effect.values[0] == "Target")
                    {
                        target_character.Choose_Orientation(target.center.gameObject);
                        while (target_character.state == Character_States.Orienting)
                        {
                            yield return new WaitForEndOfFrame();
                        }
                    }
                    else
                    {
                        target_character.Choose_Orientation((int)Convert_To_Double(effect.values[0], target_tile.obj));
                    }
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }

        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Move Type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The effect to perform. </param>
    /// <param name="effect">The GameObject to move. </param>
    /// <param name="target">The currently selected Target. May be necessary for pathing purposes.</param>
    public IEnumerator Enact_Move(Action_Effect effect, Target to_move, Target target)
    {
        //mover types
        // 1 = standard move
        // 2 = push/pull
        // 3 = fly
        // 4 = warp
        foreach (Tile target_tile in to_move.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                if (target_tile.obj != null)
                {
                    Character_Script chara = target_tile.obj.GetComponent<Character_Script>();
                    //Check for a valid object to move.
                    if (chara != null)
                    {
                        int move_type = (int)Convert_To_Double(effect.values[0], target_tile.obj);
                        Tile start = chara.curr_tile.GetComponent<Tile>();
                        //Check for valid move type.

                        if (effect.values[1] == "TARG_PATH")
                        {
                            target.Set_Path(curr_path);
                        }

                        //Actually move
                        chara.MoveTo(move_type, target.curr_path);

                        Debug.Log("Character " + chara.name + " Moved from: " + start.index[0] + ","
                            + start.index[1] + " to: " + target.center.index[0] + ","
                            + target.center.index[1]);
                    }
                    else
                    {
                        Debug.Log("Invalid move type.");
                    }
                }
                else
                {
                    Debug.Log("Invalid move Object.");
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }
        
    }

    /// <summary>
    /// Function to Enact a Damage type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to perform.</param>
    /// <param name="target">The Target on which to act. </param>
    public IEnumerator Enact_Damage(Action_Effect effect, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                //TODO add damage to Tiles

                if (target_tile.obj != null)
                {
                    Character_Script chara = target_tile.obj.GetComponent<Character_Script>();
                    if (chara != null)
                    {
                        int damage = (int)(Convert_To_Double(effect.values[0], chara.gameObject) * target.affected_tiles[target_tile][1]);
                        Debug.Log("Character " + character.character_name + " Attacked: " + chara.character_name + "; Dealing " + damage + "(" + Convert_To_Double(effect.values[0], chara.gameObject) + ") damage and Using " + ap_cost + " AP");
                        chara.Take_Damage(damage, character.weapon.pierce);
                        Event_Manager.Broadcast(Event_Trigger.ON_DAMAGE, this, effect.values[0], target_tile.gameObject);
                    }
                    else
                    {
                        int damage = (int)(Convert_To_Double(effect.values[0], target_tile.obj) * target.affected_tiles[target_tile][1]);
                        Debug.Log("Character " + character.character_name + " Attacked: OBJECT" + "; Dealing " + damage + " damage and Using " + ap_cost + " AP");
                        Object_Script target_object = target_tile.obj.GetComponent<Object_Script>();
                        target_object.Take_Damage(damage, character.weapon.pierce);
                        Event_Manager.Broadcast(Event_Trigger.ON_DAMAGE, this, effect.values[0], target_tile.gameObject);
                    }
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }

        
    }

    /// <summary>
    /// Function to Enact a Healing type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to instantiate.</param>
    /// <param name="target">The Target being Healed.</param>
    public IEnumerator Enact_Healing(Action_Effect effect, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                if (target_tile.obj.GetComponent<Character_Script>())
                {
                    Character_Script target_character = target_tile.obj.GetComponent<Character_Script>();
                    int healing = (int)(Convert_To_Double(effect.values[1], target_tile.obj) * target.affected_tiles[target_tile][1]);
                    if (effect.values[0] == Accepted_Shortcuts.CAUC.ToString() || effect.values[0] == Accepted_Shortcuts.TAUC.ToString())
                    {
                        Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " Aura, Using " + ap_cost + " AP");
                        target_character.Recover_Aura(healing);
                    }
                    else if (effect.values[0] == Accepted_Shortcuts.CMPC.ToString() || effect.values[0] == Accepted_Shortcuts.TMPC.ToString())
                    {
                        Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " MP, Using " + ap_cost + " AP");
                        target_character.Recover_Mana(healing);
                    }
                    else if (effect.values[0] == Accepted_Shortcuts.CAPC.ToString() || effect.values[0] == Accepted_Shortcuts.TAPC.ToString())
                    {
                        Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " AP, Using " + ap_cost + " AP");
                        target_character.Recover_Actions(healing);
                    }
                    else
                    {
                        Debug.Log("Invalid Healing prefix.");
                    }

                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }
        
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Status type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to instantiate.</param>
    /// <param name="target">The Target affected by the Character_Action. </param>
    public IEnumerator Enact_Status(Action_Effect effect, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                //First we need to resolve the Condition
                //Check for a power and attribute
                double power = 0;
                string attribute = "";
                //TODO add a way for status to affect Tiles and Objects.
                if (target_tile.obj != null)
                {
                    if (effect.values.Length >= 3 && effect.values[2] != null)
                    {
                        power = Convert_To_Double(effect.values[2], target_tile.obj);
                    }
                    if (effect.values.Length == 4 && effect.values[3] != null)
                    {
                        attribute = effect.values[3];
                    }
                    int duration = (int)Convert_To_Double(effect.values[1], target_tile.obj);
                    Condition condi = new Condition(effect.values[0], duration, power * target.affected_tiles[target_tile][1], attribute);

                    //Now we add the Condition to the target.
                    Character_Script target_character = target_tile.obj.GetComponent<Character_Script>();
                    target_character.Add_Condition(condi);

                    Debug.Log("Character " + character.character_name + " Gave: " + target_character.character_name +
                        " " + condi.type.ToString() + " for " + condi.duration + " turns " + " with " + condi.power +
                        " power, for " + ap_cost + "AP");
                }else
                {
                    Debug.Log("No Object on Tile [" + target_tile.index[0] + "," + target_tile.index[1] + "]");
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }
        

        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Effect type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to isntantiate.</param>
    /// <param name="target">The Target tile to affect. </param>
    public IEnumerator Enact_Effect(Action_Effect effect, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                if (target_tile.obj == null && target_tile.traversible)
                {
                    string[] values = new string[effect.values.Length - 3];
                    for (int i = 3; i < effect.values.Length; i++)
                    {
                        if (effect.values[i] != null)
                        {
                            if (effect.values[2] != Action_Effect.Types.Heal.ToString() && effect.values[2] != Action_Effect.Types.Status.ToString())
                            {
                                values[i - 3] = "" + Convert_To_Double(effect.values[i], target_tile.gameObject);
                            }
                            else
                            {
                                if (i == 3)
                                {
                                    values[i - 3] = effect.values[i];
                                }
                                else
                                {
                                    values[i - 3] = "" + Convert_To_Double(effect.values[i], target_tile.gameObject);
                                }
                            }
                        }
                    }
                    int duration = (int)Convert_To_Double(effect.values[1], target_tile.gameObject);
                    Tile_Effect tile_effect = new Tile_Effect(effect.values[0], effect.values[2], duration, values, target.affected_tiles[target_tile][0], target_tile.gameObject);
                    tile_effect.Instantiate();
                    //Debug.Log("Character " + character.character_name + " Created Effect: " + name + " on tile (" + target_tile.gameObject.GetComponent<Tile>().index[0] + "," + target_tile.gameObject.GetComponent<Tile>().index[1] + "); For " + duration + " and Using " + ap_cost + " AP");
                }
                else
                {
                    Debug.Log("Can't spawn Tile_Effect on [" + target_tile.index[0] + "," + target_tile.index[1] + "] because it is occupied.");
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }
        
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Elevate type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to enact.</param>
    /// <param name="target">The Target tile to affect. </param>
    public IEnumerator Enact_Elevate(Action_Effect effect, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                int elevation = (int)(Convert_To_Double(effect.values[0], target_tile.gameObject) * target.affected_tiles[target_tile][0]);
                Debug.Log("Character " + character.character_name + " Elevated Tile: (" + target_tile.index[0] + "," + target_tile.index[1] + "); By " + elevation + " and Using " + ap_cost + " AP");
                //character.controller.curr_scenario.tile_grid.Elevate(target.game_object.transform, elevation);
                Game_Controller.curr_scenario.tile_grid.Elevate(target_tile.transform, elevation);
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }
        
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Enable type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to enact.</param>
    /// <param name="target">The Target whose ability is being Enabled/Disabled.</param>
    public IEnumerator Enact_Enable(Action_Effect effect, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(effect.checks, target_tile.gameObject))
            {
                if (target_tile.obj.GetComponent<Character_Script>())
                {
                    Character_Script target_character = target_tile.obj.GetComponent<Character_Script>();
                    foreach (Character_Action act in target_character.GetComponent<Character_Script>().actions)
                    {
                        //Debug.Log("act.name: " + act.name + ", eff.effect.values: " + effect.values[0]);
                        if (act.name == effect.values[0])
                        {
                            //Debug.Log("MATCH");
                            if (effect.values[1] == "false" || effect.values[1] == "False" || effect.values[1] == "FALSE")
                            {
                                //Debug.Log("Skill " + act.name + " is disabled.");
                                act.enabled = false;
                            }
                            if (effect.values[1] == "true" || effect.values[1] == "True" || effect.values[1] == "TRUE")
                            {
                                //Debug.Log("Skill " + act.name + " is enabled.");
                                act.enabled = true;
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }
        
        //Reset character state when actions are done
        //character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Pass type Character_Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="effect">The Action_Effect to be enacted.</param>
    /// <param name="target">The Target for the Character_Action.</param>
    public IEnumerator Enact_Pass(string[] checks, Target target)
    {
        foreach (Tile target_tile in target.affected_tiles.Keys)
        {
            while (paused)
            {
                //TODO add an interrupt here.
                yield return new WaitForEndOfFrame();
            }
            if (Check_Criteria(checks, target_tile.gameObject))
            {
                if (target_tile.obj.GetComponent<Character_Script>())
                {
                    Character_Script target_character = target_tile.obj.GetComponent<Character_Script>();
                    target_character.StartCoroutine(target_character.End_Turn());
                }
            }
            else
            {
                Debug.Log("Condition not met");
            }
        }

    }

}
