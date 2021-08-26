using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

[Serializable]
public class Character_Script_Data
{
    [JsonProperty] public double character_id { get; private set; }
    [JsonProperty] public string character_name { get; private set; }
    [JsonProperty] public string animator_name { get; private set; }
    [JsonProperty] public int aura_max { get; private set; }
    [JsonProperty] public int aura_curr { get; private set; }
    [JsonProperty] public int action_max { get; private set; }
    [JsonProperty] public int action_curr { get; private set; }
    [JsonProperty] public int mana_max { get; private set; }
    [JsonProperty] public int mana_curr { get; private set; }
    [JsonProperty] public int reaction_max { get; private set; }
    [JsonProperty] public int reaction_curr { get; private set; }
    [JsonProperty] public int strength { get; private set; }
    [JsonProperty] public int dexterity { get; private set; }
    [JsonProperty] public int spirit { get; private set; }
    [JsonProperty] public int initiative { get; private set; }
    [JsonProperty] public int vitality { get; private set; }
    [JsonProperty] public float accuracy { get; private set; }
    [JsonProperty] public float resistance { get; private set; }
    [JsonProperty] public float lethality { get; private set; }
    [JsonProperty] public float finesse { get; private set; }
    [JsonProperty] public double default_speed { get; private set; }
    [JsonProperty] public double speed { get; private set; }
    [JsonProperty] public float weight { get; private set; }
    [JsonProperty] public int level { get; private set; }
    [JsonProperty] public int[] size { get; private set; }
    [JsonProperty] public int orientation { get; private set; }
    [JsonProperty] public int[] curr_tile_index { get; private set; }
    [JsonProperty] public Weapon weapon { get; private set; }
    [JsonProperty] public Armor armor { get; private set; }
    [JsonProperty] public Accessory[] accessories { get; private set; }
    [JsonProperty] public List<Character_Action> actions { get; private set; }
    [JsonProperty] public List<Character_Action> reactions { get; private set; }
    [JsonProperty] public List<Character_Action> curr_action { get; private set; }
    [JsonProperty] public List<Character_States> state { get; private set; }
    [JsonProperty] public List<Condition> conditions { get; private set; }
    [JsonProperty] public string description { get; private set; }
    [JsonProperty] public string spritesheet { get; private set; }

    [JsonConstructor]
    public Character_Script_Data(
        double new_character_id,
        string new_character_name,
        string new_animator_name,
        int new_aura_max,
        int new_aura_curr,
        int new_action_max,
        int new_action_curr,
        int new_mana_max,
        int new_mana_curr,
        int new_reaction_max,
        int new_reaction_curr,
        int new_strength,
        int new_dexterity,
        int new_spirit,
        int new_initiative,
        int new_vitality,
        float new_accuracy,
        float new_resistance,
        float new_lethality,
        float new_finesse,
        double new_default_speed,
        double new_speed,
        float new_weight,
        int new_level,
        int[] new_size,
        int new_orientation,
        int[] new_curr_tile_index,
        Weapon new_weapon,
        Armor new_armor,
        Accessory[] new_accessories,
        List<Character_Action> new_actions,
        List<Character_Action> new_reactions,
        List<Character_Action> new_curr_action,
        List<Character_States> new_state,
        List<Condition> new_conditions,
        string new_description,
        string new_spritesheet)
    {
        character_id = new_character_id;
        character_name = new_character_name;
        animator_name = new_animator_name;
        aura_max = new_aura_max;
        aura_curr = new_aura_curr;
        action_max = new_action_max;
        action_curr = new_action_curr;
        mana_max = new_mana_max;
        mana_curr = new_mana_curr;
        reaction_max = new_reaction_max;
        reaction_curr = new_reaction_curr;
        strength = new_strength;
        dexterity = new_dexterity;
        spirit = new_spirit;
        initiative = new_initiative;
        vitality = new_vitality;
        accuracy = new_accuracy;
        resistance = new_resistance;
        lethality = new_lethality;
        finesse = new_finesse;
        default_speed = new_default_speed;
        speed = new_speed;
        weight = new_weight;
        level = new_level;
        size = new_size;
        orientation = new_orientation;
        curr_tile_index = new_curr_tile_index;
        weapon = new_weapon;
        armor = new_armor;
        accessories = new_accessories;
        actions = new_actions;
        reactions = new_reactions;
        curr_action = new_curr_action;
        state = new_state;
        conditions = new_conditions;
        description = new_description;
        spritesheet = new_spritesheet;

    }

    public Character_Script_Data(string new_character_name,
        string new_spritesheet,
        string new_background,
        string new_level,
        string new_action_max,
        string new_reaction_max,
        string new_strength,
        string new_dexterity,
        string new_spirit,
        string new_initiative,
        string new_vitality,
        string new_accuracy,
        string new_resistance,
        string new_lethality,
        string new_finesse,
        string new_speed,
        string new_weight
        )
    {
        character_id = 0;
        character_name = new_character_name;
        spritesheet = new_spritesheet;
        animator_name = new_spritesheet.Split('/')[new_spritesheet.Split('/').Length - 1];
        description = new_background;
        int tmp = 0;
        int.TryParse(new_level, out tmp);
        level = tmp;
        int.TryParse(new_action_max, out tmp);
        action_max = tmp;
        int.TryParse(new_reaction_max, out tmp);
        reaction_max = tmp;
        int.TryParse(new_strength, out tmp);
        strength = tmp;
        int.TryParse(new_dexterity, out tmp);
        dexterity = tmp;
        int.TryParse(new_spirit, out tmp);
        spirit = tmp;
        int.TryParse(new_initiative, out tmp);
        initiative = tmp;
        int.TryParse(new_vitality, out tmp);
        vitality = tmp;
        float tmp2 = 0;
        float.TryParse(new_accuracy, out tmp2);
        accuracy = tmp2;
        float.TryParse(new_resistance, out tmp2);
        resistance = tmp2;
        float.TryParse(new_lethality, out tmp2);
        lethality = tmp2;
        float.TryParse(new_finesse, out tmp2);
        finesse = tmp2;
        double tmp3 = 0;
        double.TryParse(new_speed, out tmp3);
        float.TryParse(new_weight, out tmp2);
        weight = tmp2;
        aura_max = vitality*10;
        aura_curr = aura_max;
        action_curr = action_max;
        mana_max = spirit*5;
        mana_curr = mana_max;
        reaction_curr = reaction_max;
        default_speed = speed;
        orientation = 0;
        weapon = null;
        armor = null;
        accessories = null;
        actions = new List<Character_Action>();
        reactions = new List<Character_Action>();
        curr_action = new List<Character_Action>();
        state = new List<Character_States>();
        conditions = new List<Condition>();
        curr_tile_index = null;
    }

    /// <summary>
    /// Constructor for building a new Character_Script_Data object from an existing character script
    /// </summary>
    /// <param name="chara"></param>
    public Character_Script_Data(Character_Script chara)
    {
        character_id = chara.character_id;
        character_name = chara.character_name;
        animator_name = chara.animator_name;
        aura_max = chara.aura_max;
        aura_curr = chara.aura_curr;
        action_max = chara.action_max;
        action_curr = chara.action_curr;
        mana_max = chara.mana_max;
        mana_curr = chara.mana_curr;
        reaction_max = chara.reaction_max;
        reaction_curr = chara.reaction_curr;
        strength = chara.strength;
        dexterity = chara.dexterity;
        spirit = chara.spirit;
        initiative = chara.initiative;
        vitality = chara.vitality;
        accuracy = chara.accuracy;
        resistance = chara.resistance;
        lethality = chara.lethality;
        finesse = chara.finesse;
        speed = chara.speed;
        weight = chara.weight;
        level = chara.level;
        orientation = chara.orientation;
        weapon = chara.weapon;
        armor = chara.armor;
        accessories = chara.accessories;
        actions = chara.actions;
        reactions = chara.reactions;
        curr_action = new List<Character_Action>();
        Stack<Character_Action> stack = new Stack<Character_Action>();
        while (chara.curr_action.Count > 0)
        {
            Character_Action action = chara.curr_action.Pop();
            curr_action.Add(action);
            stack.Push(action);
        }
        while(stack.Count > 0)
        {
            chara.curr_action.Push(stack.Pop());
        }
        state = new List<Character_States>();
        Stack<Character_States> state_stack = new Stack<Character_States>();
        while (chara.state.Count > 0)
        {
            Character_States s = chara.state.Pop();
            state.Add(s);
            state_stack.Push(s);
        }
        while (state_stack.Count > 0)
        {
            chara.state.Push(state_stack.Pop());
        }
        conditions = new List<Condition>();
        foreach (List<Condition> list in chara.conditions.Values)
        {
            foreach (Condition cond in list)
            {
                conditions.Add(cond);
            }
        }
        curr_tile_index = chara.curr_tile.index;
        description = chara.description;
        spritesheet = chara.spritesheet;
    }



}
