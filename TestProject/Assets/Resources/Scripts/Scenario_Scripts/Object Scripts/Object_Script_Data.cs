using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class Object_Script_Data{
    [JsonProperty] public double obj_id { get; private set; }
    [JsonProperty] public string obj_name { get; private set; }
    [JsonProperty] public string description { get; private set; }
    [JsonProperty] public int[] curr_tile_index { get; private set; }
    [JsonProperty] public float[] object_scale { get; private set; }
    [JsonProperty] public string spritesheet { get; private set; }
    [JsonProperty] public int sprite { get; private set; }
    [JsonProperty] public int aura_max { get; private set; }
    [JsonProperty] public int aura_curr { get; private set; }
    [JsonProperty] public int mana_max { get; private set; }
    [JsonProperty] public int mana_curr { get; private set; }
    [JsonProperty] public int reaction_max { get; private set; }
    [JsonProperty] public int reaction_curr { get; private set; }
    [JsonProperty] public int strength { get; private set; }
    [JsonProperty] public int dexterity { get; private set; }
    [JsonProperty] public int spirit { get; private set; }
    [JsonProperty] public int vitality { get; private set; }
    [JsonProperty] public float accuracy { get; private set; }
    [JsonProperty] public float resistance { get; private set; }
    [JsonProperty] public float lethality { get; private set; }
    [JsonProperty] public float finesse { get; private set; }
    [JsonProperty] public double default_speed { get; private set; }
    [JsonProperty] public double speed { get; private set; }
    [JsonProperty] public float weight { get; private set; }
    [JsonProperty] public int level { get; private set; }
    [JsonProperty] public int orientation { get; private set; }
    [JsonProperty] public int[] size  { get; private set; }
    [JsonProperty] public int height { get; private set; }
    [JsonProperty] public bool solid { get; private set; }
    [JsonProperty] public bool traversible { get; private set; }
    [JsonProperty] public bool destructible { get; private set; }
    [JsonProperty] public bool locked { get; private set; }
    [JsonProperty] public List<Character_Action> reactions { get; private set; }
    [JsonProperty] public List<Conditions> conditions { get; private set; }

    [JsonConstructor]
    public Object_Script_Data(
        double new_obj_id,
        string new_obj_name, 
        string new_description,
        int[] new_curr_tile_index,
        float[] new_object_scale,
        string new_spritesheet,
        int new_sprite,
        int new_aura_max,
        int new_aura_curr,
        int new_mana_max,
        int new_mana_curr,
        int new_reaction_max,
        int new_reaction_curr,
        int new_strength,
        int new_dexterity,
        int new_spirit,
        int new_vitality,
        float new_accuracy,
        float new_resistance,
        float new_lethality,
        float new_finesse,
        double new_default_speed,
        double new_speed,
        float new_weight,
        int new_level,
        int new_orientation,
        int[] new_size,
        int new_height,
        bool new_solid,
        bool new_traversible,
        bool new_destructible,
        bool new_locked,
        List<Character_Action> new_reactions,
        List<Conditions> new_conditions)
    {
        obj_id = new_obj_id;
        obj_name = new_obj_name;
        description = new_description;
        curr_tile_index = new_curr_tile_index;
        object_scale = new_object_scale;
        spritesheet = new_spritesheet;
        sprite = new_sprite;
        aura_max = new_aura_max;
        aura_curr = new_aura_curr;
        mana_max = new_mana_max;
        mana_curr = new_mana_curr;
        reaction_max = new_reaction_max;
        reaction_curr = new_reaction_curr;
        strength = new_strength;
        dexterity = new_dexterity;
        spirit = new_spirit;
        vitality = new_vitality;
        accuracy = new_accuracy;
        resistance = new_resistance;
        lethality = new_lethality;
        finesse = new_finesse;
        default_speed = new_default_speed;
        speed = new_speed;
        weight = new_weight;
        level = new_level;
        orientation = new_orientation;
        size = new_size;
        height = new_height;
        solid = new_solid;
        traversible = new_traversible;
        destructible = new_destructible;
        locked = new_locked;
        reactions = new_reactions;
        conditions = new_conditions;
    }

    /// <summary>
    /// Constructor for building Object_Script_Data from an Object_Script
    /// </summary>
    /// <param name="obj"></param>
    public Object_Script_Data(Object_Script obj)
    {
        obj_name = obj.obj_name;
        description = obj.description;
        object_scale = obj.object_scale;
        curr_tile_index = obj.curr_tile.index;
        spritesheet = obj.spritesheet;
        sprite = obj.sprite;
        aura_max = obj.aura_max;
        aura_curr = obj.aura_curr;
        mana_max = obj.mana_max;
        mana_curr = obj.mana_curr;
        reaction_max = obj.reaction_max;
        reaction_curr = obj.reaction_curr;
        strength = obj.strength;
        dexterity = obj.dexterity;
        spirit = obj.spirit;
        vitality = obj.vitality;
        accuracy = obj.accuracy;
        resistance = obj.resistance;
        lethality = obj.lethality;
        finesse = obj.finesse;
        default_speed = obj.default_speed;
        speed = obj.speed;
        weight = obj.weight;
        level = obj.level;
        size = obj.size;
        orientation = obj.orientation;
        height = obj.height;
        solid = obj.solid;
        traversible = obj.traversible;
        destructible = obj.destructible;
        locked = obj.locked;
        reactions = obj.reactions;
        conditions = obj.conditions;
    }
}
