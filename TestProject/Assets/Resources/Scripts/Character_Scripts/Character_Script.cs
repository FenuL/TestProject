using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

/// <summary>
/// Script for handling Character interaction. Should be attached to every Character object. 
/// </summary>
public class Character_Script : MonoBehaviour {
    //Constants
    private int AURA_MULTIPLIER = 10;
    private int MP_MULTIPLIER = 5;
    private int MP_RECOVERY = 5;
    private int AP_MAX = 2;
    //public int AP_RECOVERY = 10;
    private int SPEED = 6;

    /// <summary>
    /// Constants:
    /// int AURA_MULTIPLIER - Used to calculate Maximum Aura.
    /// MP_MULTIPLIER - Used to calculate Maximum Mana.
    /// MP_RECOVERY - Amount of MP gained each turn.
    /// AP_MAX - The maximum amount of Action Points for the Character.
    /// SPEED - The default speed for Characters.
    /// 
    /// Variables
    /// int character_id - The ID of the Character for saving/loading purposes and for lookup in the player/monster_stats array.
    /// int character_num - The number of the Character for turn order in the current Scenario.
    /// string character_name - The name of the Character
    /// string animator_name - The name of the Animator attached to this Character.
    /// float[] character_scale - The scale of the current Character.
    /// int aura_max - The maximum Aura for the Character. Essentially HP.
    /// int aura_curr - The current Aura for the Character. If the Character hits 0 he/she is killable.
    /// int action_max - The maximum Action Points for the Character.
    /// int action_curr - The current number of Action Points for the Character. If this hits 0, the Turn ends.
    /// int mana_max - The maximum amount of Mana for the Character. Used to cast powerful Actions.
    /// int mana_curr - The current amount of Mana for the Character. Character can't use Actions that cost more than his/her current_mana.
    /// int reaction_max - The maximum amount of Reactions for the Character. 
    /// int reaction_curr - The current number of Reactions for the Character. If this hits 0 the Character can't use Reaction type Actions.
    /// int canister_max - The maximum amount of Mana Canisters that the Character can carry.
    /// int canister_curr - The current number of Mana Canisters for the Character. If this hits 0 the Character won't be able to rapidly recover Mana.
    /// int strength  - The Character's Strength stat. Influences damage with Melee Weapons.
    /// int coordination - The Character's Coordination stat. Influences damage with Ranged Weapons.
    /// int spirit - The Character's Spirit stat. Influences Maximum MP.
    /// int dexterity - The Character's Dexterity stat. Used for determining turn order.
    /// int vitality - The Character's Vitality stat. Used to determine Maximum Aura. 
    /// float accuracy - The Character's Accuracy. Used to determine their total Ability Modifier.
    /// float resistance - The Character's Resistance. Used to determine the total Ability Modifier of Abilities used against them.
    /// float lethality - The Character's Lethality. Used to determine their maximum Ability Modifier.
    /// float finesse - The Character's Finesse. Used to determine the Ability Modifier threshold for a Critical Hit.
    /// double speed - The distance a Character can traverse with a Move.
    /// int level - The level for the Character. Leveling can raise your other stats.
    /// int orientation - The direction the Character sprite is looking. Influences the Object's Animator.
    /// int camera_orientation_offset - The direction of the Camera looking at the Character can affect their Orientation.
    /// Vector3 camera_position_offset - The offset of the Character Sprite so that it looks right for the Camera.
    /// float height_offset - The offset for the Character Sprite so that is looks right on top of Tiles.
    /// bool rotate - If the Character needs to rotate. TODO FIND A DIFFERENT WORKAROUND.
    /// Weapon weapon - The Character's Equipped Weapon.
    /// Armor armor - The Character's Equipped Armor.
    /// Accessory[] accessories - The Character's Equipped Accessories.
    /// static List<Action> all_actions - The List of all possible Actions.
    /// List<Action> actions - The list of all the Character's Actions. A subset of all_actions derived from the Character's Equipment.
    /// Action curr_action - The Character's currently Selected Action.
    /// Character_States state - The Character's current State. Typically altered by their Action.
    /// Dictionary<Conditions., List<Condition>> conditions - The List of Conditions currently afflicting the Character.
    /// Game_Controller controller - The Game Controller that handles overarching game processes.
    /// Transform curr_tile - The current Tile Object the Character is on top of.
    /// bool ending_turn - If the Character's turn is ending. Used to prevent the screen from scrolling until other Coroutines are complete. TODO, FIND A DIFFERENT WORKAROUND
    /// </summary>
    /// TODO: Change Scenario Character Loading so we can set these to private.
    public int character_id { get; set; }
    public int character_num { get; private set; }
    public string character_name { get; private set; }
    public string animator_name { get; private set; }
    public float[] character_scale { get; private set; }
    public int aura_max { get; private set; }
    public int aura_curr { get; private set; }
    public int action_max { get; private set; }
    public int action_curr { get; private set; }
    public int mana_max { get; private set; }
    public int mana_curr { get; private set; }
    public int reaction_max { get; private set; }
    public int reaction_curr { get; private set; }
    public int canister_max { get; private set; }
    public int canister_curr { get; private set; }
    public int strength { get; private set; }
    public int coordination { get; private set; }
    public int spirit { get; private set; }
    public int dexterity { get; private set; }
    public int vitality { get; private set; }
    public float accuracy { get; private set; }
    public float resistance { get; private set; }
    public float lethality { get; private set; }
    public float finesse { get; private set; }
    public double speed { get; private set; }
    public int level { get; private set; }
    public int orientation { get; private set; }
    public int camera_orientation_offset { get; private set; }
    public Vector3 camera_position_offset { get; private set; }
    public float height_offset { get; private set; }
    public bool rotate { get; set; }
    public Weapon weapon { get; private set; }
    public Armor armor { get; private set; }
    public Accessory[] accessories { get; private set; }
    public static List<Action> all_actions { get; private set; }
    public List<Action> actions { get; private set; }
    public Action curr_action { get; set; }
    public Character_States state { get; set; }
    public Dictionary<Conditions, List<Condition>> conditions { get; private set; }
    public Game_Controller controller { get; set; }
    public Transform curr_tile { get; set; }
    public bool ending_turn = false;

    //Methods
    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="nm">Character name</param>
    /// <param name="lvl">Character level</param>
    /// <param name="str">Character Strength</param>
    /// <param name="crd">Character Coordination</param>
    /// <param name="spt">Character Spirit</param>
    /// <param name="dex">Character Dexterity</param>
    /// <param name="vit">Character Vitality</param>
    /// <param name="spd">Character Speed</param>
    /// <param name="can">Character Canisters</param>
    /// <param name="wep">Character Weapon</param>
    /// <param name="arm">Character Armor</param>
    public Character_Script(string nm, int lvl, int str, int crd, int spt, int dex, int vit, int spd, int can, string wep, string arm, string animator, float[] scale)
    {
        character_scale = scale;
        controller = Game_Controller.controller;
        character_name = nm.TrimStart();
        level = lvl;
        strength = str;
        coordination = crd;
        spirit = spt;
        dexterity = dex;
        vitality = vit;
        speed = spd;
        accuracy = 0;
        resistance = 0;
        lethality = 2;
        finesse = 1.75f;
        aura_max = vitality * AURA_MULTIPLIER;
        aura_curr = aura_max;
        action_max = AP_MAX;
        action_curr = action_max;
        mana_max = spirit * MP_MULTIPLIER;
        mana_curr = MP_RECOVERY;
        reaction_max = AP_MAX;
        reaction_curr = reaction_max;
        curr_action = null;
        actions = new List<Action>();
        canister_max = can;
        animator_name = animator;
        orientation = 2;
        canister_curr = canister_max;
        conditions = new Dictionary<Conditions, List<Condition>>();
        state = Character_States.Idle;
        all_actions = controller.all_actions;
        actions.Add(Find_Action("Move"));
        actions.Add(Find_Action("Attack"));
        foreach (Equipment.Weapon_Types weps in Enum.GetValues(typeof(Equipment.Weapon_Types)))
        {
            if (wep.TrimStart() == weps.ToString())
            {
                Weapon w = new Weapon(weps);
                Equip(w);
                break;
            }
        }
        foreach (Equipment.Armor_Types arms in Enum.GetValues(typeof(Equipment.Armor_Types)))
        {
            if (arm.TrimStart() == arms.ToString())
            {
                Armor a = new Armor(arms);
                Equip(a);
                break;
            }
        }
        actions.Add(Find_Action("Wait"));
        //foreach (string s in acc)
        //{
        //    foreach (Equipment.Weapon_Types weps in Enum.GetValues(typeof(Equipment.Weapon_Types)))
        //    {
        //        if (wep == weps.ToString())
        //        {
        //            Weapon w = new Weapon(weps);
        //            Equip(w);
        //            break;
        //        }
        //    }
        //}
    }

    /// <summary>
    /// Default Class Constructor for Inheritance purposes.
    /// </summary>
    public Character_Script()
    {

    }

    /// <summary>
    /// Populates all the relevant fields for the script given an existing one. Used to attach a Character_Script to the Character_Prefab.
    /// </summary>
    /// <param name="data">The Character_Script whose information we want to carry over</param>
    /// <param name="char_id">The Id of the character in the Character_Data pool.</param>
    /// <param name="char_num">The number of the character in the current Scenario.</param>
    public void Instantiate(Character_Script data, int char_id, int char_num, int char_orient)
    {
        name = data.character_name;
        transform.localScale = new Vector3(data.character_scale[0], data.character_scale[1], data.character_scale[2]);
        animator_name = data.animator_name;
        character_id = char_id;
        character_num = char_num;
        height_offset = (this.GetComponent<SpriteRenderer>().sprite.rect.height *
            this.transform.localScale.y /
            this.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit /
            2);
        float offset = (height_offset) / 3.5f;
        transform.position = new Vector3(transform.position.x - offset, 
            transform.position.y + height_offset, 
            transform.position.z - offset);
        camera_position_offset = new Vector3(-offset, 0.00f, -offset);
        character_name = data.character_name;
        level = data.level;
        strength = data.strength;
        coordination = data.coordination;
        spirit = data.spirit;
        dexterity = data.dexterity;
        vitality = data.vitality;
        accuracy = 0;
        resistance = 0;
        lethality = 2;
        finesse = 1.75f;
        speed = (int)data.speed;
        canister_max = data.canister_max;
        weapon = data.weapon;
        armor = data.armor;
        aura_max = data.aura_max;
        aura_curr = data.aura_curr;
        action_max = data.action_max;
        action_curr = data.action_max;
        mana_max = data.mana_max;
        mana_curr = data.mana_curr;
        actions = data.actions;
        canister_curr = data.canister_curr;
        state = data.state;
        conditions = new Dictionary<Conditions, List<Condition>>();
        controller = data.controller;
        orientation = char_orient;
        Orient();
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    void Start ()
    {
        //gameObject.SetActive(true);
    }

    /// <summary>
    /// Start the Character Turn:
    ///     Selects Move as the default action
    ///     Updates Conditions.
    /// </summary>
    public void Start_Turn()
    {
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
        Find_Action("Move").Select(this);

        //Center camera on player
        Camera.main.GetComponent<Camera_Controller>().PanTo(transform.position - Camera.main.transform.forward * 35);

        Update_Conditions();

    }

    /// <summary>
    /// Coroutine to End the Character's Turn: 
    ///     Resets and Cleans reachable_tiles; 
    ///     Resets current Action; 
    ///     Increases AP and MP; 
    ///     Increases reaction points; 
    ///     Progresses Conditions; 
    ///     Sets Character state to Idle; 
    ///     Moves to the Next_Player(); 
    /// </summary>
    /// <returns></returns>
    public IEnumerator End_Turn()
    {
        ending_turn = true;
        if (curr_action.orient == "select")
        {
            StartCoroutine(Choose_Orientation());
        }
        controller.curr_scenario.Reset_Reachable();
        controller.curr_scenario.Clean_Reachable();
        while (state == Character_States.Walking || state == Character_States.Orienting)
        {
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Character " + character_name + " Passed");
        curr_action = null;
        reaction_curr = reaction_max;
        action_curr = action_max;
        mana_curr += MP_RECOVERY;
        if (mana_curr > mana_max)
        {
            mana_curr = mana_max;
        }
        //TODO Fix this later
        foreach (Action act in actions)
        {
            if (!act.enabled)
            {
                act.enabled = true;
            }
        }
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
        Progress_Conditions();
        ending_turn = false;
        state = Character_States.Idle;
        controller.curr_scenario.Next_Player();
    }

    /// <summary>
    /// Add a new condition to the conditions list
    /// If it is a cleanser we trigger it immediately
    /// </summary>
    /// <param name="condition">The new Condition to add.</param>
    public void Add_Condition(Condition condition)
    {
        Color color = Color.red;
        //Check if the condition type is a cleanser
        if (condition.type == Conditions.Clarity)
        {
            Remove_Condition(Conditions.Confuse);
            Remove_Condition(Conditions.Blind);
            color = Color.green;
        }
        else if (condition.type == Conditions.Cleanse)
        {
            Remove_Condition(Conditions.Immobilize);
            Remove_Condition(Conditions.Daze);
            Remove_Condition(Conditions.Stun);
            Remove_Condition(Conditions.Freeze);
            Remove_Condition(Conditions.Petrify);
            color = Color.green;
        }
        else if (condition.type == Conditions.Cure)
        {
            Remove_Condition(Conditions.Bleed);
            Remove_Condition(Conditions.Burn);
            Remove_Condition(Conditions.Corrupt);
            Remove_Condition(Conditions.Frostbite);
            Remove_Condition(Conditions.Drain);
            color = Color.green;
        }
        else if (condition.type == Conditions.Restore)
        {
            Remove_Condition(Conditions.Weakness);
            Remove_Condition(Conditions.Vulnerability);
            Remove_Condition(Conditions.Slow);
            Remove_Condition(Conditions.Poison);
            color = Color.green;
        }
        else if (condition.type == Conditions.Purify)
        {
            Remove_Condition(Conditions.Bleed);
            Remove_Condition(Conditions.Burn);
            Remove_Condition(Conditions.Corrupt);
            Remove_Condition(Conditions.Frostbite);
            Remove_Condition(Conditions.Drain);
            Remove_Condition(Conditions.Immobilize);
            Remove_Condition(Conditions.Daze);
            Remove_Condition(Conditions.Stun);
            Remove_Condition(Conditions.Confuse);
            Remove_Condition(Conditions.Blind);
            Remove_Condition(Conditions.Freeze);
            Remove_Condition(Conditions.Petrify);
            Remove_Condition(Conditions.Weakness);
            Remove_Condition(Conditions.Vulnerability);
            Remove_Condition(Conditions.Slow);
            Remove_Condition(Conditions.Poison);
            Remove_Condition(Conditions.Hemorrage);
            Remove_Condition(Conditions.Blight);
            Remove_Condition(Conditions.Scorch);
            color = Color.green;
        }
        else if (condition.type == Conditions.None)
        {
            Debug.Log("ERROR: Condition Type not recognized.");
        }
        //If the condition is not a cleanser, we add it to the list
        else
        {
            //Check if we already have a condition of this type
            //if we do, get the current list and add a new stack to it.
            //If we don't create a new list and add it to the condition dictionary;
            List<Condition> condi_list;
            if (conditions.TryGetValue(condition.type, out condi_list))
            {
                //If we already have a condition we need to check the maximum stack size.
                if (condi_list.Count < condition.MAX_STACKS[condition.type])
                {
                    condi_list.Add(condition);
                }else
                {
                    //Check if our condition is the type to Upgrade
                    if (condition.UPGRADE[condition.type] != Conditions.None)
                    {
                        //TODO: ADD SCALING WHEN UPGRADING CONDITIONS
                        Add_Condition(new Condition(condition.UPGRADE[condition.type], condition.duration, condition.power*2, condition.attribute));
                    }else
                    {
                        //TODO write code to replace old/worse conditions when not upgrading them.
                    }
                }
            }
            else
            {
                condi_list = new List<Condition>();
                condi_list.Add(condition);
                conditions.Add(condition.type, condi_list);
            }
        }
        Game_Controller.Create_Floating_Text(condition.type.ToString(), transform, color);

    }

    /// <summary>
    /// Remove all stacks of a specified condition
    /// </summary>
    /// <param name="condition">The Condition type to remove.</param>
    public void Remove_Condition(Conditions condition)
    {
        conditions.Remove(condition);
        /*List<Condition> condi_list;
        if (conditions.TryGetValue(condition, out condi_list))
        {
            conditions[condition] = null;
        }*/
    }

    /// <summary>
    /// Check if a Character has a certain Condition
    /// </summary>
    /// <param name="condition">The Condition to check for</param>
    /// <returns>True if the Character has at least one stack of the Condition, False otherwise</returns>
    public bool Has_Condition(Conditions condition)
    {
        List<Condition> condi_list;
        if (conditions.TryGetValue(condition, out condi_list)){
            return true;
        }else
        {
            return false;
        }
    }

    /// <summary>
    /// Trigger condition effects. 
    /// Used at the start of a turn.
    /// TODO FINISH IMPLEMENTATION
    /// </summary>
    public void Update_Conditions()
    {
        //DoT Conditions
        if (Has_Condition(Conditions.Bleed))
        {
            foreach (Condition condi in conditions[Conditions.Bleed])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        if (Has_Condition(Conditions.Burn))
        {
            foreach (Condition condi in conditions[Conditions.Burn])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        if (Has_Condition(Conditions.Corrupt))
        {
            foreach (Condition condi in conditions[Conditions.Corrupt])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        if (Has_Condition(Conditions.Frostbite))
        {
            foreach (Condition condi in conditions[Conditions.Frostbite])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        if (Has_Condition(Conditions.Hemorrage))
        {
            foreach (Condition condi in conditions[Conditions.Hemorrage])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        if (Has_Condition(Conditions.Scorch))
        {
            foreach (Condition condi in conditions[Conditions.Scorch])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        if (Has_Condition(Conditions.Blight))
        {
            foreach (Condition condi in conditions[Conditions.Blight])
            {
                Take_Damage((int)condi.power, -1);
            }
        }
        //CC
        if (Has_Condition(Conditions.Stun) ||
            Has_Condition(Conditions.Freeze) ||
            Has_Condition(Conditions.Petrify))
        {
            StartCoroutine(End_Turn());
        }
        //TODO ADD effects for conditions.
    }

    /// <summary>
    /// Decrease Condition timer and remove them if the duration hits 0;
    /// Called at the end of a player's Turn.
    /// </summary>
    public void Progress_Conditions()
    {
        //TODO can't remove elements while iterating through with foreach loop.
        Dictionary<Conditions, List<Condition>> temp_conditions = new Dictionary<Conditions, List<Condition>>(conditions);
        List<Condition> temp_condi_list;
        foreach (List<Condition> condi_list in conditions.Values)
        {
            temp_condi_list = new List<Condition>(condi_list); 
            foreach (Condition condi in condi_list)
            {
                Condition temp_condi = new Condition(condi);
                //Decrease the duration and remove the condition if it hits 0
                if (temp_condi.Progress() <= 0)
                {
                    temp_condi_list.Remove(condi);
                    Debug.Log("Condition stack expired: " + temp_condi.type);
                }
                else
                {
                    temp_condi_list.Remove(condi);
                    temp_condi_list.Add(temp_condi);
                }

                temp_conditions[condi.type] = temp_condi_list;

                //If there are no more stacks we remove the condi list from the dictionary
                if (temp_condi_list.Count == 0)
                {
                    temp_conditions.Remove(condi.type);
                    Debug.Log("Condition type ended: " + temp_condi.type);
                }

            }
        }
        conditions = temp_conditions;

    }

    /// <summary>
    /// Coroutine for letting the player select the character orientation
    /// </summary>
    /// <returns>The status of the Coroutine.</returns>
    public IEnumerator Choose_Orientation()
    {
        Character_States prev_state = state;
        while (!Input.GetMouseButton(0))
        {
            state = Character_States.Orienting;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            //deprecated 2d raycast physics
            //hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (Physics.Raycast(ray, out hit, 100))
            {
                Tile tile_data = hit.transform.GetComponent<Tile>();
                if (tile_data != null)
                {
                    if (tile_data.index[0] >= curr_tile.GetComponent<Tile>().index[0] &&
                        tile_data.index[1] < curr_tile.GetComponent<Tile>().index[1])
                    {
                        orientation = 0 + camera_orientation_offset;
                    }
                    if (tile_data.index[0] > curr_tile.GetComponent<Tile>().index[0] &&
                        tile_data.index[1] >= curr_tile.GetComponent<Tile>().index[1])
                    {
                        orientation = 1 + camera_orientation_offset;
                    }
                    if (tile_data.index[0] <= curr_tile.GetComponent<Tile>().index[0] &&
                        tile_data.index[1] > curr_tile.GetComponent<Tile>().index[1])
                    {
                        orientation = 2 + camera_orientation_offset;
                    }
                    if (tile_data.index[0] < curr_tile.GetComponent<Tile>().index[0] &&
                        tile_data.index[1] <= curr_tile.GetComponent<Tile>().index[1])
                    {
                        orientation = 3 + camera_orientation_offset;
                    }
                    if (orientation > 3)
                    {
                        orientation = orientation - 4;
                    }
                    if (orientation < 0)
                    {
                        orientation = 4-orientation;
                    }
                }
                //orientation = (orientation + camera_orientation_offset) % 4;
            }
            Orient();
            yield return new WaitForEndOfFrame();
        }
        state = prev_state;
    }

    /// <summary>
    /// Points character to a specific target
    /// </summary>
    /// <param name="target">The Target for the Character to Orient towards.</param>
    public void Choose_Orientation(GameObject target)
    {
        Character_States prev_state = state;
        state = Character_States.Orienting;
        Tile tile_data = target.GetComponent<Tile>();
        if (tile_data != null)
        {
            if (tile_data.index[0] >= curr_tile.GetComponent<Tile>().index[0] &&
                tile_data.index[1] < curr_tile.GetComponent<Tile>().index[1])
            {
                orientation = 0;
            }
            if (tile_data.index[0] > curr_tile.GetComponent<Tile>().index[0] &&
                tile_data.index[1] >= curr_tile.GetComponent<Tile>().index[1])
            {
                orientation = 1;
            }
            if (tile_data.index[0] <= curr_tile.GetComponent<Tile>().index[0] &&
                tile_data.index[1] > curr_tile.GetComponent<Tile>().index[1])
            {
                orientation = 2;
            }
            if (tile_data.index[0] < curr_tile.GetComponent<Tile>().index[0] &&
                tile_data.index[1] <= curr_tile.GetComponent<Tile>().index[1])
            {
                orientation = 3;
            }
            orientation = (orientation + camera_orientation_offset) % 4;
        }
        Orient();
        state = prev_state;
    }

    /// <summary>
    /// Function that actually updates character's sprite based on their Orientation
    /// </summary>
    public void Orient()
    {
        //Reset orientation if it's above or below bound
        if (orientation > 3 )
        {
            orientation = 0;
        }
        if (orientation < 0)
        {
            orientation = 3;
        }
        if (camera_orientation_offset > 3)
        {
            camera_orientation_offset = 0;
        }
        if (camera_orientation_offset < 0)
        {
            camera_orientation_offset = 3;
        }
        /*Debug.Log("Name: " + name + 
            " , Height: " + this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height + 
            ", Scale: " + this.gameObject.transform.localScale.x + 
            " , Pixels: " + this.gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit);*/
        float offset = (height_offset) / 3.5f;

        //Update the location modifier based on camera offset
        if (camera_orientation_offset == 0)
        {
            
            //camera_position_offset = new Vector3(this.gameObject.GetComponent<SpriteRenderer>().sprite.rect.height/ this.gameObject.transform.localScale.x);
            
            camera_position_offset = new Vector3(-offset, 0.00f, -offset);
        }
        else if (camera_orientation_offset == 1)
        {
            camera_position_offset = new Vector3(offset, 0.00f, -offset);
        }
        else if (camera_orientation_offset == 2)
        {
            camera_position_offset = new Vector3(offset, 0.00f, offset);
        }
        else if (camera_orientation_offset == 3)
        {
            camera_position_offset = new Vector3(-offset, 0.00f, offset);
        }
        

        //Flip sprite based on orientation
        //Debug.Log("Current orientation: " + orientation);
        if (orientation < 2)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        if (orientation == 2 || orientation == 1)
        {
            string object_name = this.gameObject.name;
            string controller_name = "Animations/Controllers/" + animator_name + "/" + animator_name + "_Override_S";
            this.gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load(controller_name) as AnimatorOverrideController;
        }
        else if (orientation == 3 || orientation == 0)
        {
            string object_name = this.gameObject.name;
            string controller_name = "Animations/Controllers/" + animator_name + "/" + animator_name + "_Override_W";
            this.gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load(controller_name) as AnimatorOverrideController;
        }
    }

    /// <summary>
    /// Coroutine to run when the screen turns to update the character angle
    /// </summary>
    /// <returns></returns>
    public IEnumerator Turn()
    {
        float elapsedTime = 0;
        float duration = .3f;
        Vector3 start = transform.position;
        Vector3 target = curr_tile.position + 
            camera_position_offset + 
            new Vector3(0, height_offset + Tile_Grid.TILE_SCALE * (curr_tile.GetComponent<Tile>().height), 0);
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(start,
                target,
                elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Check to see if the character is not walking. Used for delaying the end of turn.
    /// </summary>
    /// <returns>True if the Character's state is Walking. False otherwise.</returns>
    public bool Not_Walking()
    {
        if (state != Character_States.Walking)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Find a specific Action from the character's list of actions
    /// </summary>
    /// <param name="name">The name of the Action to look for.</param>
    /// <returns>The Action if it is found, null otherwise.</returns>
    public Action Find_Action(String name)
    {
        if (all_actions != null)
        {
            //TODO Optimize search to use binary search
            foreach (Action act in all_actions)
            {
                if (act.name == name)
                {
                    return act;
                }
            }
        }
        return null;
    }

    /// <summary>
    /// Coroutine to perform an Action.
    /// </summary>
    /// <param name="target_tile">The target for the Action.</param>
    /// <returns>The current status of the Coroutine.</returns>
    public IEnumerator Act(Action action, Transform target_tile)
    {
        if (curr_action != null)
        {
            StartCoroutine(action.Enact(this, target_tile.gameObject));

            while (state != Character_States.Idle)
            {
                yield return new WaitForEndOfFrame();
            }

            //update AP and MP
            action_curr -= (int)action.Convert_To_Double(action.ap_cost, this);
            mana_curr -= (int)action.Convert_To_Double(action.mp_cost, this);
            //character.state = Character_Script.States.Idle;

            //Update reachable tiles
            controller.curr_scenario.Clean_Reachable();
            controller.curr_scenario.Reset_Reachable();

            if (action.orient == "target")
            {
                Choose_Orientation(target_tile.gameObject);
            }
            if (action_curr > action_max)
            {
                action_curr = action_max;
            }

            if (action_curr <= 0 && controller.curr_scenario.curr_player.GetComponent<Character_Script>().character_num == character_num)
            {
                StartCoroutine(End_Turn());
            }
            else
            {
                controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
                Find_Action("Move").Select(this);
            }
        }
        //curr_action.Enact(this, target_tile.gameObject);

    }

    /// <summary>
    /// Equip a piece of Equipment to trigger it's Equipment_Effects
    /// </summary>
    /// <param name="e">The Equipment to Equip</param>
    public void Equip(Equipment e)
    {
        switch (e.type)
        {
            case Equipment.Equipment_Type.Weapon:
                weapon = (Weapon)e;
                break;
            case Equipment.Equipment_Type.Accessory:
                accessories[0] = (Accessory)e;
                break;
            case Equipment.Equipment_Type.Armor:
                armor = (Armor)e;
                break;
            default:
                break;
        }
        if (e.effects != null)
        {
            foreach (Equipment.Equip_Effect eff in e.effects)
            {
                switch (eff.stat)
                {
                    case Character_Stats.aura_max:
                        aura_max += eff.effect * AURA_MULTIPLIER;
                        aura_max += eff.effect * AURA_MULTIPLIER;
                        break;
                    case Character_Stats.canister_max:
                        canister_max += eff.effect;
                        break;
                    case Character_Stats.coordination:
                        coordination += eff.effect;
                        break;
                    case Character_Stats.dexterity:
                        dexterity += eff.effect;
                        break;
                    case Character_Stats.speed:
                        speed += eff.effect;
                        break;
                    case Character_Stats.spirit:
                        spirit += eff.effect;
                        mana_max += eff.effect * MP_MULTIPLIER;
                        //action_curr += eff.effect * MP_MULTIPLIER;
                        break;
                    case Character_Stats.strength:
                        strength += eff.effect;
                        break;
                    case Character_Stats.vitality:
                        vitality += eff.effect;
                        aura_max += eff.effect * AURA_MULTIPLIER;
                        aura_curr += eff.effect * AURA_MULTIPLIER;
                        break;
                }
            }
        }
        /*speed -= e.weight;
        if( speed <= 0)
        {
            speed = 1;
        }*/
        if (e.actions != null)
        {
            foreach(String str in e.actions)
            {
                actions.Add(Find_Action(str));
            }
        }
    }

    /// <summary>
    /// Create random character stats
    /// DEPRECATED
    /// </summary>
    public void Randomize(){
        //Randomize stats
		strength = UnityEngine.Random.Range (1,7);
		coordination = UnityEngine.Random.Range (1, 7);
		spirit = UnityEngine.Random.Range (1, 7);
		dexterity = UnityEngine.Random.Range (1, 7);
		vitality = UnityEngine.Random.Range (1, 7);

        //Formulas
        speed = SPEED;
        aura_max = vitality * AURA_MULTIPLIER;
        aura_curr = aura_max;
        action_max = AP_MAX;// spirit * AP_MULTIPLIER;
        action_curr = action_max;
        actions = new List<Action>();
        canister_max = UnityEngine.Random.Range(0, 3);
        canister_curr = canister_max;
		state = Character_States.Idle;

        //Randomize Equipment
        int w = UnityEngine.Random.Range(0, 5);
        Weapon wep;
        if (w == 0)
        {
            wep = new Weapon(Equipment.Weapon_Types.Sword);
        } else if (w == 1)
        {
            wep = new Weapon(Equipment.Weapon_Types.Rifle);
        }
        else if (w == 2)
        {
            wep = new Weapon(Equipment.Weapon_Types.Spear);
        }
        else if (w == 3)
        {
            wep = new Weapon(Equipment.Weapon_Types.Sniper);
        }
        else if (w == 4)
        {
            wep = new Weapon(Equipment.Weapon_Types.Pistol);
        }
        else
        {
            wep = new Weapon(Equipment.Weapon_Types.Claws);
        }
        Equip(wep);
        int a = UnityEngine.Random.Range(0, 3);
        Armor ar;
        if (a == 0)
        {
            ar = new Armor(Equipment.Armor_Types.Light);
        }
        else if (a == 1)
        {
            ar = new Armor(Equipment.Armor_Types.Medium);
        }
        else
        {
            ar = new Armor(Equipment.Armor_Types.Heavy);
        }
        Equip(ar);
		level = 1;
		character_name = "Character " + character_num;
		controller = Game_Controller.controller;
        //FindReachable(controller.tile_grid, weapon.range);
        //FindReachable(controller.GetComponent<Game_Controller>().tile_grid,dexterity);
	}

    /// <summary>
    /// Deal damage to this character.
    /// </summary>
    /// <param name="amount"> The amount of damage to take. </param>
    /// <param name="armor_penetration">The amount of armor to ignore. Set to -1 to ignore all armor.</param>
    public void Take_Damage(int amount, int armor_penetration)
    {
        Debug.Log("Character " + character_name + " takes " + amount + " damage!");
        if (aura_curr == 0)
        {
            Die();
        }
        else
        {
            if (armor_penetration != -1)
            {
                int damage_negation = armor.armor - armor_penetration;
                if (damage_negation < 0)
                {
                    damage_negation = 0;
                }
                amount = amount - damage_negation;
                if (amount < 0)
                {
                    amount = 0;
                }
            }
            aura_curr -= amount;
            if (aura_curr < 0)
            {
                aura_curr = 0;
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            Game_Controller.Create_Floating_Text(amount.ToString(), transform, Color.red);
        }
    }

    /// <summary>
    /// Estimates how much Damage would be taken by an attack. 
    /// </summary>
    /// <returns>The Damage the Character would take. </returns>
    public int Estimate_Damage(double amount, int armor_penetration )
    {
        if (armor_penetration != -1)
        {
            int damage_negation = armor.armor - armor_penetration;
            if (damage_negation < 0)
            {
                damage_negation = 0;
            }
            amount = amount - damage_negation;
            if (amount < 0)
            {
                amount = 0;
            }
        }
        if (aura_curr - amount < 0)
        {
            amount = aura_curr;
        }
        return (int)amount;
    }

    /// <summary>
    /// Remove Damage from this Character.
    /// </summary>
    /// <param name="amount">The amount of Damage to remove.</param>
    public void Recover_Damage(int amount)
    {
        //Set the character to unwounded if they are wounded
        if (aura_curr == 0)
        {
            GetComponent<SpriteRenderer>().color = Color.white;
        }
        aura_curr += (int)amount;
        //Cap Aura gain to the max
        if (aura_curr > aura_max)
        {
            aura_curr = aura_max;
        }
        Game_Controller.Create_Floating_Text(amount.ToString(), transform, Color.green);
    }

    /// <summary>
    /// What happens when a character Dies
    /// TODO fix turn order glitches
    /// </summary>
    public void Die()
    {
        state = Character_States.Dead;
        //reset the tile traversible state and empty the tile
        curr_tile.GetComponent<Tile>().traversible = true;
        curr_tile.GetComponent<Tile>().obj = null;

        //remove the character from the turn order and character list
        Debug.Log("Character num: " + character_num + " has died");
        //Debug.Log("Characters remaining: " + controller.curr_scenario.characters.Count);
        controller.curr_scenario.characters.Remove(transform.gameObject);
        controller.curr_scenario.turn_order.Remove(transform.gameObject);
        if (character_num == controller.curr_scenario.curr_player.GetComponent<Character_Script>().character_num)
        {
            controller.curr_scenario.Next_Player();
        }
        else
        {
            /*controller.curr_scenario.curr_character_num -= 1;
            if (controller.curr_scenario.curr_character_num < 0)
            {
                controller.curr_scenario.curr_character_num = controller.curr_scenario.characters.Count - 1;
            }*/
        }

        //remove the character from the board
        //gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        Destroy(this.gameObject);

    }

    /// <summary>
    /// This is a wrapper script for the Move Coroutine. 
    /// We can't call Move from Scenario because it is not attached to any object in Unity.
    /// Since it was created using New rather than AddComponent() it exists in the C backend but Unity doesn't know it.
    /// TODO ADD CHARACTER_SCRIPT TO SCENARIO.
    /// </summary>
    /// <param name="clicked_tile"></param>
    public void MoveTo(Transform clicked_tile)
    {
        state = Character_States.Walking;
        StartCoroutine(Move(clicked_tile));
        //while (state == Character_States.Walking)
        //{
        //    Debug.Log("Walking");
        //}
        //clicked_tile.GetComponent<Tile>().obj = this.gameObject;
    }

    /// <summary>
    /// Coroutine to perform a Move Action. 
    /// </summary>
    /// <param name="clicked_tile">The Tile for the Character to Move to.</param>
    /// <returns>The current status of the Coroutine</returns>
    public IEnumerator Move(Transform clicked_tile)
    {
        state = Character_States.Walking;
        Stack<Tile> path = new Stack<Tile>();
        //path = controller.navmesh.shortestPath(curr_tile.GetComponent<Tile>().node, clicked_tile.GetComponent<Tile>().node, action_curr, SPEED);
        //Tile.Node temp_tile;
        //temp_tile = path.Pop();
        //Tile.Node prev_tile;
        Tile temp_tile = clicked_tile.GetComponent<Tile>();
        Tile prev_tile = curr_tile.GetComponent<Tile>();
        
        //action_curr -= action_cost;
        //action_cost = 0;

        
        //Tile.Node temp_tile = clicked_tile.GetComponent<Tile>().node;
        //Tile.Node prev_tile = curr_tile.GetComponent<Tile>().node;
        //Stack<Tile.Node> path = new Stack<Tile.Node>();

        //Construct a stack that is a path from the clicked tile to the source.
        while(!(temp_tile.index[0] == curr_tile.GetComponent<Tile>().index[0] && temp_tile.index[1] == curr_tile.GetComponent<Tile>().index[1]))
        {
            path.Push(temp_tile);
            //Look at the parent tile.
            temp_tile = temp_tile.parent;
        }
            
        //distances.Push(distance);
        //Debug.Log("temp_tile.index[0]: " + temp_tile.index[0]);
        //Debug.Log("temp_tile.index[1]: " + temp_tile.index[1]);
        //Debug.Log("curr_tile.index[0]: " + curr_tile.GetComponent<Tile>().index[0]);
        //Debug.Log("curr_tile.index[0]: " + curr_tile.GetComponent<Tile>().index[1]);
        //Navigate the path by popping tiles out of the stack.
        while (path.Count != 0)
        {

            temp_tile = path.Pop();
            //transform.position = new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x, (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
            //
            //yield return new WaitForSeconds(.3f);
            float elapsedTime = 0;
            float duration = .3f;
            //print("duration: " +duration);
            Vector3 start = new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (prev_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset;
            Vector3 end = new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) +camera_position_offset;

            while (elapsedTime < duration)
            {
                if(prev_tile.index[0] == temp_tile.index[0] && prev_tile.index[1] > temp_tile.index[1])
                {
                    orientation = (0 + camera_orientation_offset) %4;
                }
                else if (prev_tile.index[0] < temp_tile.index[0] && prev_tile.index[1] == temp_tile.index[1])
                {
                    orientation = (1 + camera_orientation_offset) %4;
                }
                else if (prev_tile.index[0] == temp_tile.index[0] && prev_tile.index[1] < temp_tile.index[1])
                {
                    orientation = (2 + camera_orientation_offset) %4;
                }
                else if (prev_tile.index[0] > temp_tile.index[0] && prev_tile.index[1] == temp_tile.index[1])
                {
                    orientation = (3 + camera_orientation_offset) %4;
                }
                Orient();
                if (prev_tile.height - 2 >= temp_tile.height)
                {
                    //Debug.Log("Fall");
                    Vector3 center = (start + end) * 0.5F;
                    center -= new Vector3(0, 0.5f, 0);
                    Vector3 riseRelCenter = start - center;
                    Vector3 setRelCenter = end - center;
                    transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, elapsedTime / duration);
                    transform.position += center;
                    /*transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (prev_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                            new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                            elapsedTime / duration);*/
                }
                else if (prev_tile.height + 2 <= temp_tile.height)
                {
                    //Debug.Log("Jump");
                    Vector3 center = (start + end) * 0.5F;
                    center -= new Vector3(0, 0.5f, 0);
                    Vector3 riseRelCenter = start - center;
                    Vector3 setRelCenter = end - center;
                    transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, elapsedTime/duration);
                    transform.position += center;
                    /*transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (prev_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                            new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                            elapsedTime / duration);
                            */
                }
                else if (prev_tile.height < temp_tile.height)
                {
                    Vector3 center = (start + end) * 0.5F;
                    center -= new Vector3(0, 0.5f, 0);
                    Vector3 riseRelCenter = start - center;
                    Vector3 setRelCenter = end - center;
                    transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, elapsedTime / duration);
                    transform.position += center;
                    /*transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                            new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                            (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                            controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                            elapsedTime / duration);*/
                }
                else if (prev_tile.height > temp_tile.height)
                {
                    Vector3 center = (start + end) * 0.5F;
                    center -= new Vector3(0, 0.5f, 0);
                    Vector3 riseRelCenter = start - center;
                    Vector3 setRelCenter = end - center;
                    transform.position = Vector3.Slerp(riseRelCenter, setRelCenter, elapsedTime / duration);
                    transform.position += center;
                    /*transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (prev_tile.height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                        new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                        elapsedTime / duration);*/
                }
                else
                {
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (prev_tile.height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                        new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (temp_tile.height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                        elapsedTime / duration);
                }
                /*if (CompareTag("Player"))
                {
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                        new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                        elapsedTime / duration);
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x - (.1f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height),
                        (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + 1.145f),
                        controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z - (.08f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height)),
                        new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x - (.1f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height),
                        (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + 1.145f),
                        controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z - (.08f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height)),
                        elapsedTime / duration);
                    
                }
                if (CompareTag("Monster"))
                {
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z) + camera_position_offset,
                        new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x,
                        (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + height_offset),
                        controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z) + camera_position_offset,
                        elapsedTime / duration);
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.x - (.1f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height),
                        (float)(controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + 0.7f),
                        controller.curr_scenario.tile_grid.tiles[prev_tile.index[0], prev_tile.index[1]].position.z - (.08f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height)),
                        new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x - (.1f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height),
                        (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + Tile_Grid.TILE_SCALE * (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height) + 0.7f),
                        controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.z - (.08f * controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<Tile>().height)),
                        elapsedTime / duration);
                        
                }*/
                //(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f
                elapsedTime += Time.deltaTime;
                /*if (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<SpriteRenderer>().sortingOrder > curr_tile.GetComponent<SpriteRenderer>().sortingOrder)
                {
                   gameObject.GetComponent<SpriteRenderer>().sortingOrder = controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<SpriteRenderer>().sortingOrder + 1;
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().sortingOrder = curr_tile.GetComponent<SpriteRenderer>().sortingOrder+1;
                }*/
                yield return new WaitForEndOfFrame();
            }
            //gameObject.GetComponent<SpriteRenderer>().sortingOrder = controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<SpriteRenderer>().sortingOrder + 1;
            prev_tile = temp_tile;



            //    new Vector3(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.x, (float)(controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].position.y + (controller.curr_scenario.tile_grid.tiles[temp_tile.index[0], temp_tile.index[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
            //WaitForSeconds(1);
        }
            //transform.position = new Vector3(curr_tile.position.x, (float)(curr_tile.position.y + (curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
            
        state = Character_States.Idle;

    }

    /// <summary>
    /// Update is called once per frame:
    ///     Checks if the Aura, AP and MP are within acceptable bounds.
    ///     Turns the Character towards the Camera.
    /// </summary>
    public void Update () {

        if (aura_curr < 0) {
			aura_curr = 0;
		}
        if (aura_curr > aura_max)
        {
            aura_curr = aura_max;
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
        float rotation = Camera.main.GetComponent<Camera_Controller>().rotationAmount;
        //Change sprite facing to match current camera angle

        //flip the orientation if rotation hits a certain angle

        if (rotate && rotation < 50 && rotation > 0) {
            camera_orientation_offset -= 1;
            orientation -= 1;
            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            Orient();
            StartCoroutine(Turn());
            rotate = false;
            
        }
        else if (rotate && rotation > -40 && rotation < 0 )
        {
            camera_orientation_offset += 1;
            orientation += 1;
            Orient();
            StartCoroutine(Turn());
            rotate = false;
            
        }
        float rot = rotation * 4 * Time.deltaTime;
        if (curr_tile != null)
        {
            transform.RotateAround(curr_tile.position, Vector3.up, rot);
            transform.eulerAngles = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        }
        //rotationAmount -= rotationAmount * CAMERA_TURN_SPEED * Time.deltaTime;
    }
}
