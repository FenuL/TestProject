using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class Tile_Data{
    [JsonProperty]
    public int[] index { get; private set; }
    [JsonProperty]
    public int tile_type { get; private set; }
    [JsonProperty]
    public float rotation { get; private set; }
    [JsonProperty]
    public int height { get; private set; }
    [JsonProperty]
    public int material { get; private set; }
    [JsonProperty]
    public double modifier { get; private set; }
    [JsonProperty]
    public Character_Script_Data character { get; private set; }
    [JsonProperty]
    public Object_Script_Data obj { get; private set; }
    [JsonProperty]
    public Hazard_Data hazard { get; private set; }
    [JsonProperty]
    public bool traversible { get; private set; }

    /// <summary>
    /// Constructor for the JSon Parser. Populates the fileds from primitive types.
    /// </summary>
    /// <param name="new_index">The index of the Tile_Data in the Tile_Grid</param>
    /// <param name="new_type">The type of Tile object (it's mesh).</param>
    /// <param name="new_rot">The rotation of the Tile object (in y-axis)</param>
    /// <param name="new_height">The height of the Tile object.</param>
    /// <param name="new_material">The material used to store the Tile object.</param>
    /// <param name="new_modifier">The modifier used for the tile.</param>
    /// <param name="new_char">The Character on the Tile, if any.</param>
    /// <param name="new_obj">The Object on the Tile, if any</param>
    /// <param name="new_haz">The Hazard on the Tile, if any</param>
    /// <param name="new_trav">If the tile is traversible</param>
    [JsonConstructor]
    public Tile_Data(int[] new_index, int new_type, float new_rot, 
        int new_height, int new_material, double new_modifier, Character_Script_Data new_char, 
        Object_Script_Data new_obj, Hazard_Data new_haz, bool new_trav)
    {
        index = new_index;
        tile_type = new_type;
        rotation = new_rot;
        height = new_height;
        character = new_char;
        obj = new_obj;
        hazard = new_haz;
        traversible = new_trav;
        modifier = new_modifier;
        material = new_material;
    }

    /// <summary>
    /// Create a Tile_Data object from a Tile object. 
    /// </summary>
    /// <param name="tile">The Tile object to convert to Tile_Data</param>
    public Tile_Data(Tile tile)
    {
        index = tile.index;
        tile_type = tile.tile_type;
        rotation = tile.rotation;
        height = tile.height;
        material = tile.material;
        modifier = tile.modifier;
        character = null;
        if (tile.Has_Character())
        {
            character = tile.character.GetComponent<Character_Script>().Export_Data();
        }
        obj = null;
        if (tile.Has_Object())
        {
            obj = tile.obj.GetComponent<Object_Script>().Export_Data();
        }
        hazard = null;
        if (tile.Has_Hazard())
        {
            hazard = tile.hazard.GetComponent<Hazard>().Export_Data();
        }
        traversible = tile.traversible;
    }
}
