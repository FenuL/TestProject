using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

[Serializable]
public class Hazard_Data{

    [JsonProperty]
    public double id { get; private set; }
    [JsonProperty]
    public string hazard_name { get; private set; }
    [JsonProperty]
    public string description { get; private set; }
    [JsonProperty]
    public string sprite_name { get; private set; }
    [JsonProperty]
    public Action_Effect[] effects { get; private set; }
    [JsonProperty]
    public float[,] area { get; private set; }
    [JsonProperty]
    public int[] size { get; private set; }
    [JsonProperty]
    public int duration { get; private set; }
    [JsonProperty]
    public int charges { get; private set; }
    [JsonProperty]
    public int[] current_tile_index { get; private set; }
    [JsonProperty]
    public int owner_id { get; private set; }

    /// <summary>
    /// Json Constructor for the class. Used to turn Json into a relevant Hazard_Data object.
    /// </summary>
    /// <param name="new_id">ID for the Hazard_Data</param>
    /// <param name="new_name">Name for the Hazard_Data</param>
    /// <param name="new_description">Description for the Hazard_Data</param>
    /// <param name="new_sprite_name">Sprite name for the Hazard_Data</param>
    /// <param name="new_effects">Effects for the Hazard_Data</param>
    /// <param name="new_area">Area for the Hazard_Data</param>
    /// <param name="new_size">Size for the Hazard_Data</param>
    /// <param name="new_duration">Duration in turns for the Hazard_Data</param>
    /// <param name="new_charges">Duration in times triggered for the Hazard_Data</param>
    /// <param name="new_tile_index">Index for the Tile for the Hazard_Data</param>
    /// <param name="new_owner_id">ID for the character who owns the effect.</param>
    [JsonConstructor]
    public Hazard_Data(
        double new_id, 
        string new_name, 
        string new_description, 
        string new_sprite_name, 
        Action_Effect[] new_effects, 
        float[,] new_area, 
        int[] new_size, 
        int new_duration, 
        int new_charges, 
        int[] new_tile_index,
        int new_owner_id)
    {
        id = new_id;
        hazard_name = new_name;
        description = new_description;
        sprite_name = new_sprite_name;
        effects = new_effects;
        area = new_area;
        size = new_size;
        duration = new_duration;
        charges = new_charges;
        current_tile_index = new_tile_index;
        owner_id = new_owner_id;
    }

    /// <summary>
    /// Constructor for turning a Hazard into its relevant data class for storage.
    /// </summary>
    /// <param name="e">The Hazard to convert into data.</param>
    public Hazard_Data(Hazard e)
    {
        id = e.id;
        hazard_name = e.name;
        description = e.description;
        sprite_name = e.sprite_name;
        effects = e.effects;
        area = e.area;
        size = e.size;
        duration = e.duration;
        charges = e.charges;
        current_tile_index = e.current_tiles[0].index;
        owner_id = -1;
        if (e.owner != null)
        {
            owner_id = e.owner.Get_Scenario_ID();
        }
    }

}
