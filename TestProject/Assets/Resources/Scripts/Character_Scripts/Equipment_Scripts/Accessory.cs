using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class for Accesory class Equipment. Inherits from Equipment.
/// </summary>
[Serializable]
public class Accessory : Equipment
{
    /// <summary>
    /// Constructor for the Class
    /// </summary>
    public Accessory()
    {
        type = Equipment_Type.Accessory;
    }

    //TODO ADD OTHER ACCESSORY FUNCTIONALITY
}