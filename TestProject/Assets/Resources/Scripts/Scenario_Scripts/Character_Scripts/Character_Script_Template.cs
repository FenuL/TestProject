using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Character_Script_Template : MonoBehaviour{
    public int character_num;
    public string character_name;
    public string animator_name;
    public float[] character_scale;
    public int[] aura_max;
    public int[] aura_curr;
    public int[] action_max;
    public int[] action_curr;
    public int[] mana_max;
    public int[] mana_curr;
    public int[] reaction_max;
    public int[] reaction_curr;
    public int[] strength;
    public int[] dexterity;
    public int[] spirit;
    public int[] initiative;
    public int[] vitality;
    public float[] accuracy;
    public float[] resistance;
    public float[] lethality;
    public float[] finesse;
    public double[] default_speed;
    public double[] speed;
    public float[] weight;
    public int[] level;
    public int[] orientation;
    public int camera_orientation_offset;
    public Vector3 camera_position_offset;
    public float height_offset;
    public Tile curr_tile;
    public string weapon_name;
    public string armor_name;
    public string[] accessory_names;

    
}
