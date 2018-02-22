using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/// <summary>
/// Class to define Character Actions. 
/// </summary>
public class Action
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
    /// String name - Action name
    /// String ap_cost - Unconverted Cost of the action in Action Points 
    /// String mp_cost - Unconverted Cost of the action in Mana Points
    /// String range - Unconverted number of tiles away the skill can target.
    /// String center - Unconverted where to center the Action. Over a Target or over the Character's Self.
    /// float[,] area - The area of effect that the Action will affect.
    /// List<Action_Effect> self_effect - The Effect the Character will have on itself when using this Action
    /// List<Action_Effect> target_effect - the Effect the Character will have on a target when using this Action
    /// Activation_Types type - The type of Activation for the Skill. Active skills must be Selected. Passive Skills are always active. Reactive Skills have a Trigger.
    /// String trigger - what triggers the action if it is a reactive action
    /// Origin_Types origin - Where the skill originates (for disabling purposes later)
    /// bool enabled - If the Action is enabled or not
    /// string orient - Whether the ability lets you select orientation. 
    /// String animation - The animation tied to the specific Action.
    /// </summary>

    public String name { get; private set; }
    public String ap_cost { get; private set; }
    public String mp_cost { get; private set; }
    public String range { get; private set; }
    public String center { get; private set; }
    public float[,] area { get; private set; }
    public List<Action_Effect> self_effect { get; private set; }
    public List<Action_Effect> target_effect { get; private set; }
    public Activation_Types type { get; private set; }
    public String trigger { get; private set; }
    public Origin_Types origin { get; private set; }
    public bool enabled { get; set; }
    public string orient { get; private set; }
    public String animation { get; private set; }

    /// <summary>
    /// Takes a List of strings and returns a usable Action object.
    /// </summary>
    /// <param name="input">A List of strings in the following format: HEADING: VALUES 
    /// Valid Headings:
    ///     name - The name of the Action. 
    ///         Accepts one value.
    ///     ap_cost - The cost of the Action in AP. 
    ///         Accepts one value, can use Accepted_Shortcuts.
    ///         Eg: ap_cost: 1
    ///     mp_cost - The cost of the Action in MP. 
    ///         Accepts one value, can use Accepted_Shortcuts.
    ///         Eg: mp_cost: MPC
    ///     range -  The range of the Action. Accepts one value, can use Accepted_Shortcuts.
    ///     center - The center of the Action. Either "Self" or "Target" followed by Size of AoE, eg "center: Target 3x3". 
    ///     area - The area affected by the Action and multiplier to apply to the Target. Accepts multiple doubles separated by spaces. 
    ///          Multiple area headings are accepted, but number must match amount specified in "center" heading.
    ///          Eg: "area: 0 1 0" 
    ///              "area: 0.5 0 0.5"
    ///              "area: 0 1 0"
    ///     self_effect - The effect to apply to the Character using this Action. 
    ///         Accepts Multiple Entries separated by semicolons (";"). 
    ///         Values for each effect are separated by spaces " ".
    ///         Eg: self_effect: Heal 10; Pass
    ///     target_effect - The effect to apply to the Characters affected by the Target. 
    ///         Accepts Multiple Entries separated by semicolons (";").
    ///         Eg: target_effect: Elevate 2; Damage WPD+3
    ///     activation_type - The Activation type for the Action. From the Activation_Type enum. Active, Passive or Reactive.
    ///         Eg: activation_type: active
    ///     origin - The Origin of the Action, from the Origin enum. What is used to complete the Action. Used for Disables.
    ///         Eg: origin: weapon
    ///     trigger - The Trigger for the Action if it is Reactive. NUL for no Trigger.
    ///         Eg: trigger: NUL
    ///     orient - If the Action lets the player choose an orientation after its use, before or looks at a specified target.
    ///         Eg: orient: target
    ///     enabled - If the Action is enabled or not. 
    ///         Eg: enabled: true
    ///     animation - The animation attached to the Action</param>
    ///         Eg: animation: NUL
    /// <returns>A completely constructed Action</returns>
    public static Action Parse(string[] input)
    {
        Action act = new Action();
        int area_x_index = 0;
        int area_y_index = 0;
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
                        act.trigger = values.Trim();
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
        return act;
    }

    /// <summary>
    /// Reads in the Action_List file and collects String inputs to pass to the Parse() Function. 
    /// Returns a complete List of Actions created from the File. 
    /// Used to generate a List of all available Actions.
    /// </summary>
    /// <returns>A List of all Actions available</returns>
    public static Dictionary<string, Action> Load_Actions()
    {
        Dictionary<string, Action> actions = new Dictionary<string, Action>();
        foreach (string file in System.IO.Directory.GetFiles("Assets/Resources/Actions"))
        {
            string[] lines = System.IO.File.ReadAllLines(file);
            Action action = Parse(lines);
            //Debug.Log("NAME: " + action.name);
            //Debug.Log("COST: "+ action.ap_cost);
            if (action != null && action.name != null)
            {
                actions.Add(action.name, action);
            }
        }
        return actions;
    }

    /// <summary>
    /// Converts the String parameters from the Action into a double. 
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
                        input = input.Replace(val.ToString(), "" + obj.weapon.modifier.GetLength(0)/2);
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
    /// What to do when an Action is selected from the Action Menu:
    /// Prerequisites for Selecting an action:
    ///     The Action is enabled.
    ///     The Character has enough Action Points.
    ///     The Character has enough Mana Points.
    ///     There are no Conditions blocking the Action.
    /// </summary>
    /// <param name="character">The Character performing the Action, from which to derive its stats.</param>
    public void Select(Character_Script character)
    {
        //Check to see if action is enabled
        //Debug.Log("Name: " + name + " is enabled: " + enabled);
        if (this.enabled)
        {
            //Debug.Log("AP cost: " + (int)Convert_To_Double(cost, character));
            //Check to see if player can afford action:
            if (character.action_curr >= (int)Convert_To_Double(ap_cost, character))
            {
                //Debug.Log("Enough action points");
                if (character.mana_curr >= (int)Convert_To_Double(mp_cost, character))
                {
                    //Debug.Log("Enough mana points");
                    character.curr_action = this;
                    if (target_effect != null)
                    {
                        foreach (Action_Effect eff in target_effect)
                        {
                            if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                            {
                                character.state = Character_States.Moving;
                                character.controller.curr_scenario.Find_Reachable((int)character.speed, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Elevate.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Enable.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                        }
                    }
                    if (self_effect != null)
                    {
                        foreach (Action_Effect eff in self_effect)
                        {
                            if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                            {
                                if (Convert_To_Double(eff.value[0], character) != 4)
                                {
                                    character.state = Character_States.Moving;
                                    //Debug.Log("Speed: " + character.speed);
                                    character.controller.curr_scenario.Find_Reachable((int)character.speed, (int)Convert_To_Double(range, character));
                                }
                                else
                                {
                                    character.state = Character_States.Blinking;
                                    character.controller.curr_scenario.Find_Reachable((int)character.speed * 2, (int)Convert_To_Double(range, character));
                                }
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Elevate.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Enable.ToString())
                            {
                                character.state = Character_States.Attacking;
                                character.controller.curr_scenario.Find_Reachable(character.action_curr, (int)Convert_To_Double(range, character));
                            }
                            else if (eff.type.ToString() == Action_Effect.Types.Pass.ToString())
                            {
                                //character.curr_action = this;
                                //character.StartCoroutine(character.Act(null));
                                //Debug.Log("TEST 2");
                                //TODO Change this so the Action actually goes off before ending the turn.
                                //Enact(character, null);
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
                character.curr_action = this;
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
            Debug.Log("Action is disabled");
            foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
            {
                if (but.name == name)
                {
                    but.GetComponent<Image>().color = Color.red;
                }
            }
        }
    }

    /// <summary>
    /// Gets the Tiles in the Area of Effect for the current Action.
    /// </summary>
    /// <param name="character">The Character performing the Action.</param>
    /// <param name="target_tile">The tile around which the Action is being used. </param>
    /// <returns>A List of Targets, which contain a GameObject(Tile Data) and the action's Area Modifier.</returns>
    public List<Target> Get_Target_Tiles(Character_Script character, GameObject target_tile)
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
    /// Gets the Character_Scripts in the Area of Effect for the current Action.
    /// </summary>
    /// <param name="character">The Character performing the Action.</param>
    /// <param name="target_tile">The tile around which the Action is being used. </param>
    /// <returns>A List of Targets, which contain a GameObject(Character_Script) and the action's Area Modifier.</returns>
    public List<Target> Get_Targets(Character_Script character, GameObject target_tile)
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
                            Target tar = new Target(target.GetComponent<Tile>().obj, Calculate_Total_Modifier(character, target.GetComponent<Tile>().obj, area[x, y]));
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
    /// <param name="character">The character performing the action</param>
    /// <param name="target">The target of the action. Could be a Character_Script or an Object.</param>
    /// <param name="action_mod">The modifier for the Action being used in that space. </param>
    /// <returns></returns>
    public float Calculate_Total_Modifier(Character_Script character, GameObject target, float action_mod)
    {
        float modifier = 0;
        modifier += action_mod;
        //Debug.Log("Action modifier is: " + action_mod);
        modifier += character.accuracy;
        //Debug.Log("Character Accuracy is: " + character.accuracy);
        modifier += Calculate_Height_Modifier(character, target);
        modifier += Calclulate_Orientation_Modifier(character, target);
        modifier += Calculate_Weapon_Modifier(character, target);
        modifier += Calculate_Combo_Modifier(character, target);
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
    /// <param name="character">The Character performing the Action.</param>
    /// <param name="target">The target of the Action.</param>
    /// <returns>The Height Modifier.</returns>
    public float Calculate_Height_Modifier(Character_Script character, GameObject target)
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
    /// <param name="character">The Character performing the Action</param>
    /// <param name="target">The target of the Action</param>
    /// <returns>The Orientation modifier.</returns>
    public float Calclulate_Orientation_Modifier(Character_Script character, GameObject target)
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
    /// <param name="character">The character performing the Action.</param>
    /// <param name="target">The target of the Action</param>
    /// <returns>The weapon modifier.</returns>
    public float Calculate_Weapon_Modifier(Character_Script character, GameObject target)
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
    /// <param name="character">The character performing the Action.</param>
    /// <param name="target">The target of the Action</param>
    /// <returns>the combo modifier</returns>
    public float Calculate_Combo_Modifier(Character_Script character, GameObject target)
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
    /// Checks if the an Action can be used on a specific Tile. 
    /// For instance can't use a Damage Action on a space with no valid Targets.
    /// </summary>
    /// <param name="character">The Character performing the Action</param>
    /// <param name="target_tile">The Tile around which the Action is occurring</param>
    /// <returns>True if using the Action on the current target tile is Valid. False otherwise.</returns>
    public bool Check_Valid(Character_Script character, GameObject target_tile)
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
                    if (Get_Targets(character, target_tile) == null)
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
    /// Coroutine for actually using the Action.
    /// Action must be Validated before Enacting (Check_Valid()):
    /// Calls the various Enact_<>() Functions depending on the Actions's Action_Effects. 
    /// </summary>
    /// <param name="character">Character using this Action.</param>
    /// <param name="target_tile">The Tile on which to Enact the Action. </param>
    /// <returns>An IEnumerator with the current Coroutine progress. </returns>
    public IEnumerator Enact(Character_Script character, GameObject target_tile)
    {
        if (Check_Valid(character, target_tile))
        {
            if (target_effect != null)
            {
                //Find targets in AoE
                List<Target> targets = Get_Targets(character, target_tile);
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
                foreach (Action_Effect eff in target_effect)
                {
                    if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                    {
                        // find the target to move towards
                        int centerX = character.curr_tile.GetComponent<Tile>().index[0];
                        int centerY = character.curr_tile.GetComponent<Tile>().index[1];
                        //Target tile = new Target(character.curr_tile.gameObject, 0);
                        if(center == "target")
                        {
                            centerX = target_tile.GetComponent<Tile>().index[0] + (area.GetLength(0) / 2);
                            centerY = target_tile.GetComponent<Tile>().index[1] + (area.GetLength(1) / 2);
                        }

                        foreach (Target target in targets)
                        {
                            int tile_x = 0;
                            int tile_y = 0; 
                            if (target.game_object.GetComponent<Character_Script>()) {
                                tile_x = (int)(centerX +
                                        (target.game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile>().index[0] - centerX) *
                                        target.modifier *
                                        Convert_To_Double(eff.value[1], character));
                                tile_y = (int)(centerY +
                                        (target.game_object.GetComponent<Character_Script>().curr_tile.GetComponent<Tile>().index[1] - centerY) *
                                        target.modifier *
                                        Convert_To_Double(eff.value[1], character));
                                if(tile_x < 0)
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
                                Target tile = new Target(character.controller.curr_scenario.tile_grid.getTile(tile_x,tile_y).gameObject, target.modifier);
                                Enact_Move(target.game_object.GetComponent<Character_Script>(), eff.value[0], tile);
                            }
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                    {
                        foreach (Target target in targets)
                        {
                            Enact_Damage(character, eff.value[0], target);
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                    {
                        foreach (Target target in targets)
                        {
                            Enact_Healing(character, eff.value[0], target);
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                    {
                        foreach (Target target in targets)
                        {
                            Enact_Status(character, eff.value, target);
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Elevate.ToString())
                    {
                        foreach (Target target in targets)
                        {
                            Enact_Elevate(character, eff.value[0], target);
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Enable.ToString())
                    {
                        foreach (Target target in targets)
                        {
                            Enact_Enable(character, eff.value, target);
                        }
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Pass.ToString())
                    {
                        List<Target> target_tiles = Get_Target_Tiles(character, target_tile);
                        foreach (Target target in target_tiles)
                        {
                            Enact_Pass(character, target);
                        }
                    }
                }
            }
            if (self_effect != null)
            {
                foreach (Action_Effect eff in self_effect)
                {
                    //TODO FIND A WAY TO CARRY FORWARD MODIFIER FOR SELF EFFECTS
                    Target target = new Target(character.gameObject,area[0,0]);
                    if (eff.type.ToString() == Action_Effect.Types.Move.ToString())
                    {
                        Enact_Move(character, eff.value[0], new Target(target_tile, area[0,0]));
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Damage.ToString())
                    {
                        Enact_Damage(character, eff.value[0], target);
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Heal.ToString())
                    {
                        Enact_Healing(character, eff.value[0], target);
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Status.ToString())
                    {
                        Enact_Status(character, eff.value, target);
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Elevate.ToString())
                    {
                        Enact_Elevate(character, eff.value[0], new Target(character.curr_tile.gameObject, 0));
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Enable.ToString())
                    {
                        Enact_Enable(character, eff.value, target);
                    }
                    else if (eff.type.ToString() == Action_Effect.Types.Pass.ToString())
                    {
                        Enact_Pass(character, target);
                    }
                }
            }

            while (character.state != Character_States.Idle)
            {
                //Debug.Log("State " + character.state.ToString());
                yield return new WaitForEndOfFrame();
            }
        }
    }

    /// <summary>
    /// What to do when a player Selects a Damage type Action from the Action Menu.
    /// TODO ADD OTHER SELECT METHODS
    /// </summary>
    public void Select_Damage()
    {

    }

    /// <summary>
    /// Function to Enact a Move Type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Move Action</param>
    /// <param name="movetype">The Type of Movement the Character is performing. </param>
    /// <param name="target">The Target destination for the Move.</param>
    public void Enact_Move(Character_Script character, String movetype, Target target)
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
    /// Function to Enact a Damage type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action</param>
    /// <param name="value">The String with the damage equation.</param>
    /// <param name="target">The Target being dealt damage. </param>
    public void Enact_Damage(Character_Script character, String value, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            int damage = (int)(Convert_To_Double(value, character) * target.modifier);
            Debug.Log("original damage: " + Convert_To_Double(value, character));
            Debug.Log("Character " + character.character_name + " Attacked: " + target_character.character_name + "; Dealing " + damage + " damage and Using " + ap_cost + " AP");
            target_character.Take_Damage(damage, character.weapon.pierce);
        }
        else
        {
            int damage = (int)(Convert_To_Double(value, character) * target.modifier);
            Debug.Log("Character " + character.character_name + " Attacked: OBJECT" + "; Dealing " + damage + " damage and Using " + ap_cost + " AP");
        }
        //Reset character state when actions are done
        character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Healing type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action</param>
    /// <param name="value">The String with the healing equation.</param>
    /// <param name="target">The Target being Healed.</param>
    public void Enact_Healing(Character_Script character, String value, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            int healing = (int)(Convert_To_Double(value, character) * target.modifier);
            Debug.Log("Character " + character.character_name + " Healed: " + target_character.character_name + "; for " + healing + " and Using " + ap_cost + " AP");
            target_character.Recover_Damage(healing);
        }
        //Reset character state when actions are done
        character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Status type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action</param>
    /// <param name="value">The equation for what Status to apply. </param>
    /// <param name="target">The Target affected by the Action. </param>
    public void Enact_Status(Character_Script character, String[] values, Target target)
    {
        //First we need to resolve the Condition
        //Check for a power and attribute
        double power = 0;
        string attribute = "";
        if (values.Length >= 3 && values[2] != null)
        {
            power = Convert_To_Double(values[2],character);
        }
        if (values.Length == 4 && values[3] != null)
        {
            attribute = values[3];
        }
        int duration = (int)Convert_To_Double(values[1], character);
        Condition condi = new Condition(values[0], duration, power*target.modifier, attribute);

        //Now we add the Condition to the target.
        Character_Script target_character = target.game_object.GetComponent<Character_Script>();
        target_character.Add_Condition(condi);

        Debug.Log("Character " + character.character_name + " Gave: " + target_character.character_name + 
            " " + condi.type.ToString() + " for " + condi.duration + " turns " + " with " + condi.power + 
            " power, for " + ap_cost + "AP") ;

        //Reset character state when actions are done
        character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Elevate type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action. </param>
    /// <param name="value">The String with the equation for how much to Elevate.</param>
    /// <param name="target">The Target tile to affect. </param>
    public void Enact_Elevate(Character_Script character, String value, Target target)
    {
        if (target.game_object.GetComponent<Tile>())
        {
            int elevation = (int)(Convert_To_Double(value, character) * target.modifier);
            Debug.Log("Character " + character.character_name + " Elevated Tile: (" + target.game_object.GetComponent<Tile>().index[0] + "," + target.game_object.GetComponent<Tile>().index[1] + "); By " + elevation + " and Using " + ap_cost + " AP");
            character.controller.curr_scenario.tile_grid.Elevate(target.game_object.transform, elevation);
        }
        else
        {
            Debug.Log("Invalid target for Elevate.");
        }
        //Reset character state when actions are done
        character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact an Enable type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action. </param>
    /// <param name="value">The String with the euqation for what to do. Should be an action name and a bool pair.</param>
    /// <param name="target">The Target whose ability is being Enabled/Disabled.</param>
    public void Enact_Enable(Character_Script character, String[] value, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            foreach (Action act in target_character.GetComponent<Character_Script>().actions)
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
        character.state = Character_States.Idle;
    }

    /// <summary>
    /// Function to Enact a Pass type Action. Used in the Enact() Function.
    /// </summary>
    /// <param name="character">The Character performing the Action.</param>
    /// <param name="target">The Target for the Action.</param>
    public void Enact_Pass(Character_Script character, Target target)
    {
        if (target.game_object.GetComponent<Character_Script>())
        {
            Character_Script target_character = target.game_object.GetComponent<Character_Script>();
            target_character.StartCoroutine(target_character.End_Turn());
        }
    }

}
