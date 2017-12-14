using UnityEngine;
using System.Collections;

/// <summary>
/// Class for Accesory class Equipment. Inherits from Equipment.
/// </summary>
public class Accessory : Equipment
{
    /// <summary>
    /// Constructor for the Class
    /// </summary>
    public Accessory()
    {
        type = Equipment_Type.Accessory;
        durability = 100;
    }

    //TODO ADD OTHER ACCESSORY FUNCTIONALITY
}