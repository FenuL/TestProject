using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class for an Action's Target
/// </summary>
public class Target
{
    /// <summary>
    /// Variables:
    /// GameObject game_object - The Object for the Action to Target
    /// float modifier - The AoE modifier for the Action to attach to the Target
    /// </summary>
    public Tile center { get; private set; }
    public Dictionary<Tile, float[]> affected_tiles { get; private set; }
    public Dictionary<Character_Script, float> affected_characters { get; private set; }
    public Dictionary<Object_Script, float> affected_objects { get; private set; }
    public List<Tile> curr_path { get; private set; }

    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="new_target">The GameObject to target.</param>
    /// <param name="new_modifier">The modifier to attach to the Target.</param>
    public Target(Tile new_center, float[,] area)
    {
        center = new_center;
        affected_tiles = new Dictionary<Tile, float[]>();
        affected_characters = new Dictionary<Character_Script, float>();
        affected_objects = new Dictionary<Object_Script, float>();
        curr_path = new List<Tile>();

        //String[] area_effect = center.Split(' ');
        int startX = center.index[0];
        int startY = center.index[1];
        
        //Set the start of the loop
        startX -= (area.GetLength(0) / 2);
        startY -= (area.GetLength(1) / 2);
        //Loop through the area and find valid targets
        for (int x = 0; x < area.GetLength(0); x++)
        {
            for (int y = 0; y < area.GetLength(1); y++)
            {
                if (area[x, y] != 0)
                {
                    //Transform target = character.controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    Tile target = Game_Controller.curr_scenario.tile_grid.getTile(startX + x, startY + y).GetComponent<Tile>();
                    if (target != null)
                    {
                        float[] modifiers = new float[2];
                        modifiers[0] = area[x, y];
                        modifiers[1] = 0;
                        if (target.obj != null)
                        {
                            Character_Script chara = target.obj.GetComponent<Character_Script>();
                            if (chara != null)
                            {
                                affected_characters.Add(chara, area[x,y]);
                            }else
                            {
                                affected_objects.Add(target.obj.GetComponent<Object_Script>(), area [x,y]);
                            }
                            modifiers[1] = area[x, y];
                        }
                        affected_tiles.Add(target, modifiers);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Sets the modifiers for a Tile in affected_tiles. Updates the affected_objects and affected_characters as well if the Tile has one.
    /// </summary>
    /// <param name="Tile">The Tile in affected_tiles to update.</param>
    /// <param name="modifiers">What to change the modifier to.</param>
    public void Set_Modifiers(Tile tile, float[] modifiers)
    {
        if (affected_tiles.ContainsKey(tile)){
            affected_tiles[tile][0] = modifiers[0];
            if (tile.obj != null)
            {
                affected_tiles[tile][1] = modifiers[1];
                Character_Script chara = tile.obj.GetComponent<Character_Script>();
                if (chara != null)
                {
                    affected_characters[chara] = modifiers[1];
                }
                Object_Script obj = tile.obj.GetComponent<Object_Script>();
                if (obj != null)
                {
                    affected_objects[obj] = modifiers[1];
                }
            }
            else
            {
                affected_tiles[tile][1] = 0;
            }
        }
    }

    /// <summary>
    /// Sets the current path to the listed path.
    /// </summary>
    /// <param name="path"></param>
    public void Set_Path(List<Tile> path)
    {
        curr_path = path;
    }

}
