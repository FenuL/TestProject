using UnityEngine;
using System.Collections;
using System;
using Newtonsoft.Json;

/// <summary>
/// Class for Accesory class Equipment. Inherits from Equipment.
/// </summary>
[Serializable]
public class Accessory : Equipment
{

    [JsonProperty]
    public int uses { get; protected set; }

    [JsonConstructor]
    public Accessory(
        string new_equip_name,
        Equipment_Type new_type,
        string[] new_action_names,
        string new_image,
        string new_description,
        float new_weight,
        string new_sprite,
        int new_uses)
    {
        equip_name = new_equip_name;
        type = new_type;
        action_names = new_action_names;
        image = new_image;
        description = new_description;
        weight = new_weight;
        sprite = new_sprite;
        uses = new_uses;
    }

    /// <summary>
    /// Constructor for the Class
    /// </summary>
    public Accessory()
    {
        type = Equipment_Type.Accessory;
    }

    //TODO ADD OTHER ACCESSORY FUNCTIONALITY

    /// <summary>
    /// Constructor for the class. Used for creating Accessory data files.
    /// </summary>
    /// <param name="nam">Name of the Accessory.</param>
    /// <param name="desc">Description of the Accessory.</param>
    /// <param name="acts">Actions granted by the Accessory</param>
    /// <param name="spri">The spritesheet for the Accessory.</param>
    /// <param name="img">The image for the Accessory.</param>
    /// <param name="wei">The Accessory's innate weight characteristic.</param>
    /// <param name="arm">The Weapon's innate armor modifier.</param>
    public Accessory(string nam, string desc, string[] acts, string spri, string img, float wei)
    {
        equip_name = nam;
        description = desc;
        action_names = acts;
        sprite = spri;
        image = img;
        weight = wei;
        type = Equipment_Type.Accessory;
    }
}