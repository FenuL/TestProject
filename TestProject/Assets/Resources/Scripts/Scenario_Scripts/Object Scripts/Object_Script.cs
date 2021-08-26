using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

/// <summary>
/// Script for handling interaction with Non Character Objects on the Tile Grid. 
/// </summary>
[Serializable]
public class Object_Script : MonoBehaviour {
    [JsonProperty] public double obj_id { get; private set; }
    [JsonProperty] public string obj_name { get; private set; }
    [JsonProperty] public string description { get; private set; }
    [JsonProperty] public string spritesheet { get; private set; }
    [JsonProperty] public int sprite { get; private set; }
    [JsonProperty] public float[] object_scale { get; private set; }
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
    [JsonProperty] public int[] size { get; private set; }
    [JsonProperty] public int orientation { get; private set; }
    [JsonProperty] public int height { get; private set; }
    [JsonProperty] public bool solid { get; private set; }
    [JsonProperty] public bool traversible { get; private set; }
    [JsonProperty] public bool destructible { get; private set; }
    [JsonProperty] public bool locked { get; private set; }
    [JsonProperty] public List<Character_Action> reactions { get; private set; }
    [JsonProperty] public List<Conditions> conditions { get; private set; }

    //Non serialized variables
    public Tile curr_tile { get; private set; }
    float height_offset;

    public void Set_Curr_Tile(Tile tile)
    {
        curr_tile = tile;
        tile.Set_Obj(gameObject);
    }

    /// <summary>
    /// Populates all the relevant fields for the script given an existing one. Used to attach a Objecrt_Script to the Object_Prefab.
    /// </summary>
    /// <param name="data">The Object_Script_Data whose information we want to carry over</param>
    public void Instantiate(Object_Script_Data data)
    {
        obj_name = data.obj_name;
        description = data.description;
        object_scale = data.object_scale;
        int[] index = data.curr_tile_index;
        curr_tile = Game_Controller.Get_Curr_Scenario().Get_Tile(index[0], index[1]);
        spritesheet = data.spritesheet;
        sprite = data.sprite;
        aura_max = data.aura_max;
        aura_curr = data.aura_curr;
        mana_max = data.mana_max;
        mana_curr = data.mana_curr;
        reaction_max = data.reaction_max;
        reaction_curr = data.reaction_curr;
        strength = data.strength;
        dexterity = data.dexterity;
        spirit = data.spirit;
        vitality = data.vitality;
        accuracy = data.accuracy;
        resistance = data.resistance;
        lethality = data.lethality;
        finesse = data.finesse;
        default_speed = 0;
        speed = 0;
        weight = data.weight;
        level = data.level;
        size = data.size;
        orientation = data.orientation;
        height = data.height;
        solid = data.solid;
        traversible = data.traversible;
        destructible = data.destructible;
        locked = data.locked;
        reactions = data.reactions;
        conditions = data.conditions;

        string sprite_file = spritesheet.Split('_')[0];
        int sprite_id = 0;
        int.TryParse(sprite_file.Split('_')[1], out sprite_id);
        Sprite[] sprites = Resources.LoadAll<Sprite>(sprite_file);
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[sprite_id];

        //Calculate offsets
        float height_offset = (gameObject.GetComponent<SpriteRenderer>().sprite.rect.height *
            gameObject.transform.localScale.y /
            gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit /
            2);
        //float offset = (height_offset) / 3.5f;
        
        transform.position = new Vector3(transform.position.x, transform.position.y + height_offset, transform.position.z);
    }

    /// <summary>
    /// Deal damage to this Object.
    /// </summary>
    /// <param name="amount"> The amount of damage to take. </param>
    /// <param name="armor_penetration">The amount of armor to ignore. Set to -1 to ignore all armor.</param>
    public void Take_Damage(float amount, float armor_penetration)
    {
        Debug.Log("Object " + name + " takes " + amount + " damage!");
        aura_curr = aura_curr - (int)amount;
        if (aura_curr <= 0)
        {
            Debug.Log("Object is dead.");
        }
        /*else
        {
            if (armor_penetration != -1)
            {
                float damage_negation = armor.armor - armor_penetration;
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
            aura_curr -= (int)amount;
            if (aura_curr < 0)
            {
                aura_curr = 0;
                GetComponent<SpriteRenderer>().color = Color.red;
            }*/
        Game_Controller.Create_Floating_Text(amount.ToString(), transform, Color.red);
        //}
    }

    /// <summary>
    /// Gets the ID of the object in the scenario.
    /// </summary>
    /// <returns>Int ID of the Object</returns>
    public int Get_Scenario_ID()
    {
        return (int)((obj_id % 1) * 10000);
    }

    /// <summary>
    /// Exports this object's data as a Data object that can be used for saving and loading.
    /// </summary>
    /// <returns>An Object_Script_Data object whose fields match these.</returns>
    public Object_Script_Data Export_Data()
    {
        return new Object_Script_Data(this);
    }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start () {
    }
	
	/// <summary>
    /// Called once per Frame to update the Object. 
    /// Makes the Object face the Camera.
    /// </summary>
	void Update () {
        //Change sprite facing to match current camera angle
        transform.eulerAngles = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
