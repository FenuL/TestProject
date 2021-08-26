using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

/// <summary>
/// Class to define Character Actions. 
/// </summary>
[Serializable]
public class Character_Action
{
    /// <summary>
    /// enum Activation_Types - The types of actions for abilities
    /// enum Origin_Types - The types of origin for abilities.
    /// enum Accepted_Shortcuts - Shortcuts used to read the actions from the action list.
    /// </summary>
    public enum Activation_Types{ Active, Passive, Reactive }
    public enum Trigger_Target_Types { Source, Target, None }
    public enum Origin_Types { Innate, Soul, Weapon }
    public enum Center_Types { Self, Target }

    /// <summary>
    /// Variables:
    /// String name - Character_Action name
    /// String ap_cost - Unconverted Cost of the action in Character_Action Points 
    /// String mp_cost - Unconverted Cost of the action in Mana Points
    /// Path_Types type - Used to determine what tiles this Action can affect. Different types have different range selection criteria.
    /// String color - The color to turn the range tiles.
    /// String range - Unconverted number of tiles away the skill can target.
    /// String center - Unconverted where to center the Character_Action. Over a Target or over the Character's Self.
    /// float[,] area - The area of effect that the Character_Action will affect.
    /// String num_targets - The minimum amount of targets required to Enact the skill. Set to 0 for instant action.
    /// List<Target> curr_targets the current list of targets for the ability.
    /// List<Tile> curr_path The current path of the ability. 
    /// bool paused - If the Character_Action is paused or not. 
    /// bool interrupt - If the Character_Action can interrupt another Action.
    /// List<Action_Effect> self_effect - The Effect the Character will have on itself when using this Character_Action
    /// List<Action_Effect> target_effect - the Effect the Character will have on a target when using this Character_Action
    /// Activation_Types path_type - The type of Activation for the Skill. Active skills must be Selected. Passive Skills are always active. Reactive Skills have a Trigger.
    /// Event_Trigger trigger - what event triggers the action if it is a reactive action. Example, ON_DAMAGE
    /// string condition - Under what condition a Reaction can be used after an event is fired. Example, CAUC > CAUM/2
    /// Origin_Types origin - Where the skill originates (for disabling purposes later)
    /// bool enabled - If the Character_Action is enabled or not
    /// String animation - The animation tied to the specific Character_Action.
    /// Character_Script character - The Character performing the Character_Action.
    /// </summary>

    //Serialized values
    [JsonProperty] public String name { get; private set; }
    [JsonProperty] public String ap_cost { get; private set; }
    [JsonProperty] public String mp_cost { get; private set; }
    [JsonProperty] public Path_Types path_type { get; private set; }
    [JsonProperty] public String tile_color { get; private set; }
    [JsonProperty] public String range { get; private set; }
    [JsonProperty] public Center_Types center { get; private set; }
    [JsonProperty] public float[,] area { get; private set; }
    [JsonProperty] public String num_targets { get; private set; }
    [JsonProperty] public string[] target_checks { get; private set; }
    [JsonProperty] public bool interrupt { get; private set; }
    [JsonProperty] public List<Action_Effect> effects { get; private set; }
    [JsonProperty] public Activation_Types activation { get; private set; }
    [JsonProperty] public Event_Trigger trigger { get; private set; }
    [JsonProperty] public Trigger_Target_Types trigger_target { get; private set; }
    [JsonProperty] public string[] trigger_checks { get; private set; }
    [JsonProperty] public Origin_Types origin { get; private set; }
    [JsonProperty] public bool enabled { get; private set; }
    [JsonProperty] public String animation { get; private set; }

    //Non serialized variables
    public bool paused { get; private set; }
    public bool interrupted { get; private set; }
    public List<Target> curr_targets { get; private set; }
    public List<Tile> curr_path { get; private set; }
    public float curr_path_cost { get; private set; }
    public Character_Script character { get; private set; }
    public int proc_effect_num { get; private set; }
    public int anim_num { get; private set; }

    //setters
    /// <summary>
    /// Sets the proc effect num to the number specified.
    /// </summary>
    /// <param name="i">What to set the proc effect num to.</param>
    public void Set_Proc_Effect_Num(int i)
    {
        proc_effect_num = i;
    }

    /// <summary>
    /// Create an empty Character_Action.
    /// </summary>
    public Character_Action()
    {
        name = "";
        ap_cost = "";
        mp_cost = "";
        path_type = Path_Types.Melee;
        range = "";
        center = Center_Types.Self;
        area = null;
        num_targets = "";
        target_checks = new string[0];
        curr_targets = new List<Target>();
        curr_path = new List<Tile>();
        curr_path_cost = 0;
        paused = false;
        interrupted = false;
        effects = null;
        interrupt = false;
        activation = Activation_Types.Active;
        trigger = Event_Trigger.ON_DAMAGE;
        trigger_target = Trigger_Target_Types.None;
        trigger_checks = new string[0];
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
        path_type = act.path_type;
        tile_color = act.tile_color;
        range = act.range;
        center = act.center;
        area = act.area;
        num_targets = act.num_targets;
        target_checks = act.target_checks;
        curr_targets = new List<Target>();
        curr_path = new List<Tile>();
        curr_path_cost = 0;
        paused = act.paused;
        interrupted = act.interrupted;
        interrupt = act.interrupt;
        effects = act.effects;
        activation = act.activation;
        trigger = act.trigger;
        trigger_target = act.trigger_target;
        trigger_checks = act.trigger_checks;
        origin = act.origin;
        enabled = act.enabled;
        animation = act.animation;
        character = chara;
        proc_effect_num = act.proc_effect_num;
        //string json = JsonUtility.ToJson(this);
        //Debug.Log(json);
    }

    /// <summary>
    /// JSON constructor for the class. USed to convert JSON to the relevant fields.
    /// </summary>
    /// <param name="new_name">Name of the Character_Action</param>
    /// <param name="new_ap_cost">Cost in AP of the Character_Action</param>
    /// <param name="new_mp_cost">Cost in mp of the Character_Action</param>
    /// <param name="new_path_type">Path type of the Character_Action</param>
    /// <param name="new_tile_color">Tile Color of the Character_Action</param>
    /// <param name="new_range">Range in Tiles of the Character_Action</param>
    /// <param name="new_center">Center of the Character_Action</param>
    /// <param name="new_area">Area of the Character_Action</param>
    /// <param name="new_num_targets">Number of targets of the Character_Action</param>
    /// <param name="new_target_checks">Checks for a valid Target</param>
    /// <param name="new_interrupt">If the Character_Action interrupts</param>
    /// <param name="new_effects">Effects of the Character_Action</param>
    /// <param name="new_activation">What activates the Character_Action</param>
    /// <param name="new_trigger">What triggers the Character_Action</param>
    /// <param name="new_trigger_target">What </param>
    /// <param name="new_trigger_checks">What the triggers checks for</param>
    /// <param name="new_origin">The source of the Character_Action</param>
    /// <param name="new_enabled">Is the Character_Action enabled</param>
    /// <param name="new_animation">The animation to use for th Character_Action</param>
    [JsonConstructor]
    public Character_Action(
        String new_name,
        String new_ap_cost,
        String new_mp_cost,
        Path_Types new_path_type,
        String new_tile_color,
        String new_range,
        Center_Types new_center,
        float[,] new_area,
        String new_num_targets,
        string[] new_target_checks,
        bool new_interrupt,
        List<Action_Effect> new_effects,
        Activation_Types new_activation,
        Event_Trigger new_trigger,
        Trigger_Target_Types new_trigger_target,
        string[] new_trigger_checks,
        Origin_Types new_origin,
        bool new_enabled,
        String new_animation)
    {
        name = new_name;
        ap_cost = new_ap_cost;
        mp_cost = new_mp_cost;
        path_type = new_path_type;
        tile_color = new_tile_color;
        range = new_range;
        center = new_center;
        area = new_area;
        num_targets = new_num_targets;
        target_checks = new_target_checks;
        curr_targets = new List<Target>();
        curr_path = new List<Tile>();
        curr_path_cost = 0;
        paused = false;
        interrupted = false;
        interrupt = new_interrupt;
        effects = new_effects;
        activation = new_activation;
        trigger = new_trigger;
        trigger_target = new_trigger_target;
        trigger_checks = new_trigger_checks;
        origin = new_origin;
        enabled = new_enabled;
        animation = new_animation;
        character = null;
        proc_effect_num = 0;
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
            path_type == act.path_type &&
            range == act.range &&
            center == act.center &&
            area == act.area &&
            effects == act.effects &&
            activation == act.activation &&
            trigger == act.trigger &&
            trigger_checks == act.trigger_checks &&
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
    /// Reads a JSON string and returns a Character_Action class.
    /// </summary>
    /// <param name="json">The JSON string to parse.</param>
    /// <returns>The Character_Action the JSON represents, or null if the json couldn't be parsed.</returns>
    public static Character_Action ParseJson(string json)
    {
        Character_Action act = JsonUtility.FromJson<Character_Action>(json);
        //Debug.Log("act name " + act.name);
        if (act.name != null && act.name != "")
        {
            act.interrupted = false;
            act.paused = false;
            //Debug.Log("Json parsed");
            act.curr_targets = new List<Target>();
            act.curr_path = new List<Tile>();
            act.curr_path_cost = 0;
            act.character = null;
            act.proc_effect_num = 0;
            act.anim_num = 0;
            return act;
        }
        return null;
        
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
        foreach (string file in System.IO.Directory.GetFiles("Assets/Resources/Actions","*.*", System.IO.SearchOption.AllDirectories))
        {
            Character_Action action = null;
            if (file.EndsWith(".json"))
            {
                //Debug.Log("Found " + file);
                action = ParseJson(System.IO.File.ReadAllText(file));
                //string json = JsonUtility.ToJson(action);
                //Debug.Log("Action:" + json);
                //Debug.Log("Fields:" + action2.name);
            }
            if (action != null && action.name != null && action.name != "")
            {
                actions.Add(action.name, action);
            }

        }
        return actions;
    }

    /// <summary>
    /// Method to convert a string to a specific list of Tiles. Used to determine movement path.
    /// </summary>
    /// <param name="input">String formula for where to find path. Example: CT_PATH returns the path of the current target</param>
    /// <param name="target_obj">The object being targeted by the action.</param>
    /// <param name="target">The current target of the action.</param>
    /// <returns></returns>
    public List<Tile> Convert_To_Tile_List(Action_Effect effect, GameObject target_obj, Target target)
    {
        List<Tile> move_path = new List<Tile>();
        if (effect.values[1] == "PATH")
        {
            if (effect.values[2] == "TA_PATH")
            {
                move_path = target.curr_path;
            }
            else if (effect.values[2].Contains("TN_"))
            {
                string input = effect.values[2];
                int start = input.IndexOf("TN_") + 3;
                int length = input.Substring(start).IndexOf("_");
                int target_index;
                if (int.TryParse(input.Substring(start, length), out target_index))
                {
                    if (target_index < curr_targets.Count)
                    {
                        move_path = curr_targets[target_index].curr_path;
                    }
                }
            }
        }
        else if (effect.values[1] == "VECT")
        {
            float direction = Game_Controller.Convert_To_Float(effect.values[2], character.gameObject, target_obj, target);
            float distance = Game_Controller.Convert_To_Float(effect.values[3], character.gameObject, target_obj, target);
            Tile start = target_obj.GetComponent<Character_Script>().curr_tile;
            Tile end = target_obj.GetComponent<Tile>();
            Debug.Log("direction: " + direction);
            Debug.Log("distance: " + distance);
            Debug.Log("start: [" + start.index[0] + "," + start.index[1] + "]");
            if (direction > 3)
            {
                direction = direction % 4;
            }
            if (direction >= 0)
            {
                if (direction == 0)
                {
                    float end_index = start.index[1] - distance;
                    if (end_index < 0)
                    {
                        end_index = 0;
                    }
                    else if (end_index >= Game_Controller.Get_Curr_Scenario().Get_Width())
                    {
                        end_index = Game_Controller.Get_Curr_Scenario().Get_Width() - 1;
                    }
                    end = Game_Controller.Get_Curr_Scenario().Get_Tile(start.index[0], (int)end_index);
                }
                else if (direction == 1)
                {
                    float end_index = start.index[0] + distance;
                    if (end_index < 0)
                    {
                        end_index = 0;
                    }
                    else if (end_index >= Game_Controller.Get_Curr_Scenario().Get_Length())
                    {
                        end_index = Game_Controller.Get_Curr_Scenario().Get_Length() - 1;
                    }
                    end = Game_Controller.Get_Curr_Scenario().Get_Tile((int)end_index, start.index[1]);
                }
                else if (direction == 2)
                {
                    float end_index = start.index[1] + distance;
                    if (end_index < 0)
                    {
                        end_index = 0;
                    }
                    else if (end_index >= Game_Controller.Get_Curr_Scenario().Get_Width())
                    {
                        end_index = Game_Controller.Get_Curr_Scenario().Get_Width() - 1;
                    }
                    end = Game_Controller.Get_Curr_Scenario().Get_Tile(start.index[0], (int)end_index);
                }
                else if (direction == 3)
                {
                    float end_index = start.index[0] - distance;
                    if (end_index < 0)
                    {
                        end_index = 0;
                    }
                    else if (end_index >= Game_Controller.Get_Curr_Scenario().Get_Length())
                    {
                        end_index = Game_Controller.Get_Curr_Scenario().Get_Length() - 1;
                    }
                    end = Game_Controller.Get_Curr_Scenario().Get_Tile((int)end_index, start.index[1]);
                }

                Debug.Log(" desired end: [" + end.index[0] + "," + end.index[1] + "]");

                if (!Game_Controller.Get_Curr_Scenario().reachable_tiles.Contains(end))
                {
                    Stack<Tile> stack = Find_Path(start, end);
                    while (stack.Count != 0)
                    {
                        Tile path_tile = stack.Pop();
                        //Debug.Log("Tile index: [" + path_tile.index[0] + "," + path_tile.index[1] + "]");
                        //character.Get_Curr_Action().Peek().curr_path.Add(curr_scenario.clicked_tile);
                        move_path.Add(path_tile);
                    }
                }else
                {
                    Debug.Log("End out of target range");
                }


            }
        }
        return move_path;
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
                //Debug.Log("AP cost: " + (int)Game_Controller.Convert_To_Float(ap_cost, character.gameObject));
                //Check to see if player can afford action:
                if (Check_Resource())
                {
                    //character.Get_Curr_Action().Push(this);

                    if (path_type != Path_Types.None ||
                        Game_Controller.Convert_To_Float(num_targets, character.gameObject, null, null) > 0)
                    {
                        Reset_Targets();
                        Reset_Path();

                        //Select the action in the action menu
                        foreach (Transform but in Game_Controller.controller.Get_Action_Menu().GetComponent<Action_Menu_Script>().buttons)
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

                        if (character.Get_Curr_Actions().Count != 0)
                        {
                            character.Pop_Curr_Action();
                        }

                        character.Push_Curr_Action(this);

                        //Show the number of targets for the selected Action.
                        Game_Controller.controller.Get_Action_Menu().GetComponent<Action_Menu_Script>().Set_Text("Using " + name + " - Select " + num_targets + " Target(s)");

                        Find_Reachable_Tiles(character, true);

                        Game_Controller.Get_Curr_Scenario().Clean_Reachable();
                        Game_Controller.Get_Curr_Scenario().Mark_Reachable();
                    }
                    else
                    {
                        //character.Get_Curr_Action().Push(this);
                        character.StartCoroutine(character.Act(this, character.curr_tile));

                    }


                    //Game_Controller.controller.Get_Curr_Scenario().Clean_Reachable();
                    //Game_Controller.controller.Mark_Reachable();


                    //Debug.Log("Character " + character.name + " " + character.character_num + " current action " + character.Get_Curr_Action().Peek().name);
                    //Debug.Log("Character " + character.name + " current action count " + character.Get_Curr_Action().Count);
                    //Debug.Log(character.character_num);

                }
                else
                {
                    Debug.Log("NOT Enough Action Points");
                    foreach (Transform but in Game_Controller.controller.Get_Action_Menu().GetComponent<Action_Menu_Script>().buttons)
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
                foreach (Transform but in Game_Controller.controller.Get_Action_Menu().GetComponent<Action_Menu_Script>().buttons)
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
            startX = character.curr_tile.index[0];
            startY = character.curr_tile.index[1];
        }
        else if (center == "Target")
        {
            if (target_tile != null)
            {
                startX = target_tile.index[0];
                startY = target_tile.index[1];
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
                    //Transform target = Game_Controller.controller.Get_Curr_Scenario().Get_Tile_Grid().getTile(startX + x, startY + y);
                    Transform target = Game_Controller.Get_Curr_Scenario().Get_Tile_Grid().getTile(startX + x, startY + y);
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
            startX = character.curr_tile.index[0];
            startY = character.curr_tile.index[1];
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
                    //Transform target = Game_Controller.controller.Get_Curr_Scenario().Get_Tile_Grid().getTile(startX + x, startY + y);
                    Transform target = Game_Controller.Get_Curr_Scenario().Get_Tile_Grid().getTile(startX + x, startY + y);
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
            target_tile = target_character.curr_tile;
        }
        if (target_tile != null)
        {
            modifier = 0.125f * (character.curr_tile.height - target_tile.height);
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
                target_tile = target_character.curr_tile;
            }
            else
            {
                target_tile = target.GetComponent<Object_Script>().curr_tile.GetComponent<Tile>();
            }
        }
        Tile char_tile = character.curr_tile;
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
            modifier = target_character.turn_stats[Character_Turn_Records.Combo_Modifier];
        }
        return modifier;
    }

    /// <summary>
    /// Finds a path from the last tile in the curr_path for the Action to the given target Tile. Then updates curr_path with that path;
    /// </summary>
    /// <param name="target">The target. </param>
    public Stack<Tile> Find_Path(Tile start, Tile end)
    {
        Stack<Tile> path = new Stack<Tile>();
        //Character_Script chara = target.GetComponent<Character_Script>();
        Tile target_tile = end;
        //Debug.Log("Finding Path");
        if (target_tile != null)
        {
            if(path_type == Path_Types.Path)
            {
                //Debug.Log("Finding Path with Path rules");
                path = Game_Controller.Get_Curr_Scenario().Find_Path(start, target_tile);
            }
            else if (path_type == Path_Types.Instant)
            {
                path.Push(target_tile);
            }
            else if (path_type == Path_Types.Ranged)
            {
                path.Push(target_tile);
            }
            else if (path_type == Path_Types.Melee)
            {
                //Debug.Log("PATHFINDING start " + start.index[0] + "," + start.index[1] + " end " + target_tile.index[0] + "," + target_tile.index[1]);
                path = Game_Controller.Get_Curr_Scenario().Find_Path(start, target_tile);
            }
            else if (path_type == Path_Types.Unrestricted_Path)
            {
                path = Game_Controller.Get_Curr_Scenario().Find_Path(start, target_tile);
            }
            else if (path_type == Path_Types.Projectile)
            {
                path.Push(target_tile);
            }
            /*if (move_type == 4)
            {
                path.Push(clicked_tile.gameObject.GetComponent<Tile>());
            }else
            {
                path = Game_Controller.Get_Curr_Scenario().Get_Tile_Grid().navmesh.FindPath(curr_tile.GetComponent<Tile>(), clicked_tile.gameObject.GetComponent<Tile>());
            }*/
        }

        //Debug.Log("Path size " + path.Count);
        return path;
    }

    /// <summary>
    /// Function for finding tiles in range of an ability based on Action_Type.
    /// </summary>
    /// <param name="chara">The character whose range we are checking.</param>
    /// <param name="update">Whether or not to update the scenario's reachable tiles.</param>
    /// <returns>The tiles that are reachable</returns>
    public List<Tile> Find_Reachable_Tiles(Character_Script chara, bool update)
    {
        List<Tile> tiles = new List<Tile>();
        
        if (path_type == Path_Types.Path)
        {
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable((int)character.speed, (int)Game_Controller.Convert_To_Float(range, character.gameObject), 1);
            //Debug.Log((int)(Game_Controller.Convert_To_Float(range, character.gameObject, null) - curr_path_cost));
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara,(int)(Game_Controller.Convert_To_Float(range, character.gameObject)-curr_path_cost), (int)Game_Controller.Convert_To_Float(range, character.gameObject), 1,update);
        }
        else if (path_type == Path_Types.Unrestricted_Path)
        {
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable((int)character.speed * 2, (int)Game_Controller.Convert_To_Float(range, character.gameObject, null, curr_targets), 2);
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara,(int)chara.speed * 2, (int)Game_Controller.Convert_To_Float(range, character.gameObject), 2, update);
        }
        else if (path_type == Path_Types.Ranged)
        {
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable(character.action_curr, (int)Game_Controller.Convert_To_Float(range, character.gameObject, null, curr_targets), 3);
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara, chara.action_curr, (int)Game_Controller.Convert_To_Float(range, character.gameObject), 3, update);
        }
        else if (path_type == Path_Types.Melee)
        {
            //Debug.Log("Range " + (int)Game_Controller.Convert_To_Float(range, character.gameObject, null, curr_targets));
            //Debug.Log("character name: " + chara.name);
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable(character.action_curr, (int)Game_Controller.Convert_To_Float(range, character.gameObject, null, curr_targets), 3);
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara, chara.action_curr, (int)Game_Controller.Convert_To_Float(range, character.gameObject), 3, update);
            /*foreach (Tile tile in tiles)
            {
                Debug.Log(tile.name);
            }*/
        }
        else if (path_type == Path_Types.Projectile)
        {
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable(character.action_curr, (int)Game_Controller.Convert_To_Float(range, null, curr_targets), 3);
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara, chara.action_curr, (int)Game_Controller.Convert_To_Float(range, character.gameObject), 3, update);
        }
        else if (path_type == Path_Types.Instant)
        {
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable((int)character.speed * 2, (int)Game_Controller.Convert_To_Float(range, character.gameObject, null, curr_targets), 2);
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara, (int)Game_Controller.Convert_To_Float(range, character.gameObject), (int)Game_Controller.Convert_To_Float(range, character.gameObject), 2, update);
        }
        else if (path_type == Path_Types.None)
        {
            //Game_Controller.controller.Get_Curr_Scenario().Find_Reachable((int)character.speed * 2, (int)Game_Controller.Convert_To_Float(range, character.gameObject,null, curr_targets), 2);
            tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara, (int)Game_Controller.Convert_To_Float(range, character.gameObject), (int)Game_Controller.Convert_To_Float(range, character.gameObject), 3, update);
        }
        return tiles;
    }

    /// <summary>
    /// Function for finding reachable tiles based on movement type  and distance rather than Action_Type. 
    /// </summary>
    /// <param name="chara"> The Character performing the Action</param>
    /// <param name="move_type">The type of movement (1-4)</param>
    /// <param name="direction">The direction of the movement (0-3)</param>
    /// <param name="distance">The distance to travel</param>
    /// <param name="update">Whether or not to update the reachable tiles array for the scenario.</param>
    /// <returns></returns>
    public List<Tile> Find_Reachable_Tiles(Character_Script chara, int move_type, int direction, float distance, bool update)
    {
        List<Tile> tiles = new List<Tile>();
        if (move_type >= 0)
        {
            if (move_type == 1)
            {
                //TODO, add something here
            }
            else if (move_type == 2)
            {

                tiles = Game_Controller.Get_Curr_Scenario().Find_Reachable(chara, direction, distance, (int)distance, 5, update);
                //Debug.Log(tiles.Count);
            }
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
    /// Check to see if a character is in Range of the ability.
    /// </summary>
    /// <param name="chara">The Character whose position we need to check</param>
    /// <returns>True if in range, False otherwise</returns>
    public bool Check_Range(Character_Script chara)
    {
        //Debug.Log(character.name + " is using " + name + " on " + chara.name);
        if (Find_Reachable_Tiles(character, false).Contains(chara.curr_tile))
        {
            //TODO add more detailed check.
            return true;
        }
        return false;
    }

    /// <summary>
    /// Check if the character has enough Action Points or Reaction and Mana points to perform the action.
    /// </summary>
    /// <returns>True if the character has enough AP/RP and MP. False otherwise.</returns>
    public bool Check_Resource()
    {
        //Debug.Log("type " + type + " character actions " + character.action_curr);
        if (character != null)
        {
            if (((activation == Activation_Types.Active && character.action_curr >= (int)Game_Controller.Convert_To_Float(ap_cost, character.gameObject)) ||
                (activation == Activation_Types.Reactive && character.reaction_curr >= (int)Game_Controller.Convert_To_Float(ap_cost, character.gameObject))) &&
                character.mana_curr >= (int)Game_Controller.Convert_To_Float(mp_cost, character.gameObject))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Trigger a Reaction type action.
    /// </summary>
    /// <param name="act">The action that triggered this reaction</param>
    /// <param name="value">The value of the action being taken</param>
    /// <param name="target">The target of the action</param>
    public void React(Character_Action act, string value, GameObject target)
    {
        if (Game_Controller.Parse_Checks(trigger_checks, character.gameObject, target) && Check_Resource())
        {
            GameObject true_target = act.character.curr_tile.gameObject;
            if (trigger_target == Trigger_Target_Types.Target)
            {
                true_target = target;
            }
            Debug.Log("Character " + character.name + " is reacting with " + name + " targeting" + true_target.name);
            //Debug.Log("Current character " + Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().name);
            //Debug.Log("Current action " + Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Peek().name);
            //Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Peek().Pause();

            //Pause current Action.
            Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Pause();
            if (interrupt)
            {
                Debug.Log("Interrupting " + Game_Controller.Get_Curr_Scenario().Get_Curr_Character_Action().name);
                Game_Controller.Get_Curr_Scenario().Get_Curr_Character_Action().interrupted = true;
                //Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Interrupt();
            }
            //Debug.Log("Pausing Character " + Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().name + "'s current action " + Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Peek().name);
            //Debug.Log("Character action count " + Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Count);
            //Game_Controller.controller.Get_Curr_Scenario().curr_player.Push(character.gameObject);

            //Make this character the current character.
            Game_Controller.Get_Curr_Scenario().Push_Curr_Character(character.gameObject);

            //Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Push(this);

            //Add a Target for this Action.
            Add_Target(true_target);

            //Make this Action the current Action.
            Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Push_Curr_Action(this);

            //Start the Action
            character.StartCoroutine(character.Act(this, true_target.GetComponent<Tile>()));
        }

    }

    public void End_Reaction()
    {
        //Debug.Log("Ending Reaction of " + character.name);
        //Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Pop();

        //Remove the current Reaction from the Action stack.
        Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Pop_Curr_Action();

        //Game_Controller.controller.Get_Curr_Scenario().Pop_Curr_Character();

        //Remove the current Character from the Action stack.
        Game_Controller.Get_Curr_Scenario().Pop_Curr_Character();

        //Debug.Log("Current player: " + Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Count);
        //if (Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Count > 0)

        //Reset the Targets and Path for this Action.
        Reset_Targets();
        Reset_Path();

        //If there was a previous character in the stack, resume their Action.
        if (Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Actions().Count > 0)
        {
            Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Resume();
            if (!interrupt)
            {
                //Game_Controller.controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Peek().Resume();
                
                //Debug.Log("Resumed");
            }else
            {
                //Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().Pop();
            }
        }
    }

    /// <summary>
    /// Adds a target tile to the list of targets if it is a Valid target.
    /// <param name="target_tile">The target to add to the list of the Action's current Targets.</param>
    /// </summary>
    public void Add_Target(GameObject target_tile)
    {
        Tile tile = character.curr_tile;
        if (center == Center_Types.Target) {
            tile = target_tile.GetComponent<Tile>();
        }
        if (tile != null)
        {
            if (Game_Controller.Parse_Checks(target_checks, character.gameObject, target_tile))
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
                target.Set_Path_Cost(curr_path_cost);
                //Debug.Log("Path cost " + curr_path_cost);
                //Debug.Log("target path " + target.curr_path.Count);
                //Reset_Path();
                curr_targets.Add(target);
                Game_Controller.controller.Get_Action_Menu().GetComponent<Action_Menu_Script>().Set_Text("Using " + name + " - Select " + (Game_Controller.Convert_To_Float(num_targets, character.gameObject, target_tile)-curr_targets.Count) + " Target(s)");
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
            if(path_type == Path_Types.Path)
            {
                if(target_tile.traversible)
                {
                    curr_path_cost += (float)target_tile.weight;
                    Stack<Tile> path = Find_Path(curr_path[curr_path.Count - 1], target_tile);
                    //Add the Tiles in the stack to the current path.
                    while (path.Count != 0)
                    {
                        Tile path_tile = path.Pop();
                        //Debug.Log("Tile index: [" + path_tile.index[0] + "," + path_tile.index[1] + "]");
                        //character.Get_Curr_Action().Peek().curr_path.Add(curr_scenario.clicked_tile);
                        curr_path.Add(path_tile);
                    }
                    added = true;
                }
            }
            else
            {
                if (curr_path.Count > 0)
                {
                    Stack<Tile> path = Find_Path(curr_path[curr_path.Count - 1], target_tile);
                    //Add the Tiles in the stack to the current path.
                    while (path.Count != 0)
                    {
                        Tile path_tile = path.Pop();
                        //Debug.Log("Tile index: [" + path_tile.index[0] + "," + path_tile.index[1] + "]");
                        //character.Get_Curr_Action().Peek().curr_path.Add(curr_scenario.clicked_tile);
                        curr_path.Add(path_tile);
                    }
                    added = true;
                }
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
        if (curr_targets.Count >= Game_Controller.Convert_To_Float(num_targets, character.gameObject))
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
        curr_path.Add(character.curr_tile);
        curr_path_cost = 0;
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
        //character.state.Push(Character_States.Enacting);
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
            self_mod[0,0] = Calculate_Total_Modifier(character.gameObject, 1);
            Target self = new Target(character.curr_tile, self_mod);

            if (curr_targets.Count > 0)
            {
                foreach (Target target in curr_targets)
                {
                    //TODO eventually add combo mods to Tile scripts as well.
                    foreach (Character_Script chara in target.affected_characters.Keys)
                    {
                        chara.Increase_Turn_Stats(Character_Turn_Records.Combo_Modifier,1);
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
                if (interrupted)
                {
                    break;
                }
                foreach (Action_Effect eff in effects)
                {
                    if (interrupted)
                    {
                        break;
                    }
                    //Debug.Log("Using " + eff.Get_Type() + " on " + target.center.index[0] + "," + target.center.index[1]);
                    //Debug.Log("lower limit " + eff.target_limit[0] + " higher limit " + eff.target_limit[1]);
                    //Debug.Log("target_index " + target_index);
                    if (eff.min_target_limit <= target_index &&
                       eff.max_target_limit >= target_index)
                    {
                        //Debug.Log("Taking effect type: " + eff.Get_Type());
                        elapsedTime = 0;

                        List<Target> temp_targets = new List<Target>();
                        if (eff.affects == Action_Effect.Affects.Target)
                        {
                            temp_targets.Add(target);
                        }
                        else if (eff.affects == Action_Effect.Affects.Self)
                        {
                            temp_targets.Add(self);
                        }
                        else if (eff.affects == Action_Effect.Affects.Path)
                        {
                            List<Target> path_targets = new List<Target>();
                            foreach (Tile tile in target.curr_path)
                            {
                                //find a way to specify path modifier.
                                float[,] path_mod = new float[1, 1];
                                //List<FloatList> path_mod = new List<FloatList>();
                                //path_mod.Add(new FloatList());
                                path_mod[0,0] = Calculate_Total_Modifier(tile.gameObject, 0.5f);
                                path_targets.Add(new Target(tile, path_mod));
                            }

                            temp_targets = path_targets;
                        }
                        //Debug.Log(character.name + " " + name + "Num of target effects: " + effects.Count);
                        //Debug.Log(character.name + " " + name + "effect_num " + proc_effect_num);
                        //Debug.Log(character.name + " " + name + "effect type " + eff.Get_Type());
                        while (proc_effect_num == 0 || paused && !interrupted)
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

                        if (interrupted)
                        {
                            break;
                        }
                        //stop procing if Character is current Effect has not been resolved.
                        while (character.state.Peek() != Character_States.Enacting || !Is_Current_Action())
                        {
                            Debug.Log("character "  + character.name + " performing " + name + " is " + character.state.Peek());
                            Debug.Log("Current Action is " + Game_Controller.Get_Curr_Scenario().Get_Curr_Character().GetComponent<Character_Script>().Get_Curr_Action().name);
                            yield return new WaitForEndOfFrame();
                        }
                        foreach (Target effect_target in temp_targets)
                        {
                            while (paused && !interrupted)
                            {
                                //TODO add an interrupt here.
                                yield return new WaitForEndOfFrame();
                            }
                            if (interrupted)
                            {
                                break;
                            }
                            foreach (Tile target_tile in effect_target.affected_tiles.Keys)
                            {
                                Event_Manager.Broadcast(Event_Trigger.ON_TARGET, this, "", target_tile.gameObject);
                                while (paused && !interrupted)
                                {
                                    //TODO add an interrupt here.
                                    yield return new WaitForEndOfFrame();
                                }
                                if (interrupted)
                                {
                                    break;
                                }
                                //Proc the effect

                                proc_effect_num -= 1;
                                yield return character.StartCoroutine(eff.Enact_Effect(this, character.gameObject, target_tile.gameObject, target));
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

        //Wait for the effects to be completed.
        while (character.state.Peek() != Character_States.Enacting || 
            !Is_Current_Action())
        {
            //Debug.Log("State " + character.state.ToString());
            yield return new WaitForEndOfFrame();
        }

        //Return the character to Acting state.
        //Debug.Log(character.name + " is " + character.state.Peek());
        //character.state.Pop();
        //Debug.Log(character.name + " is " + character.state.Peek());
        character.gameObject.GetComponent<Animator>().SetTrigger("Done_Acting");

        proc_effect_num = 0;

        //Reset the action's targets and Path.
        Reset_Targets();
        Reset_Path();
        interrupted = false;

        //Debug.Log("Finished Enacting " + name);
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
    /// Checks to see if this action is currently on top of the action stack for the character currently on top of the player stack.
    /// </summary>
    /// <returns>True if this action's character is on top of the current player stack and this action is on top of their action stack.</returns>
    public bool Is_Current_Action()
    {
        if (character.Is_Current_Character())
        {
            if(character.Has_Current_Action() && 
               this == character.Get_Curr_Action())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Sets the enabled flag to true;
    /// </summary>
    public void Enable()
    {
        enabled = true;
    }

    /// <summary>
    /// Sets the enabled flag to false;
    /// </summary>
    public void Disable()
    {
        enabled = false;
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

}
