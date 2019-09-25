using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class for an Action's Target
/// </summary>
public class Target
{
    /// <summary>
    /// Variables:
    /// Tile center - the Center of the Target.
    /// Dictionary<Tile, float[]> affected_tiles - The list of Tiles affected by the target mapped to their modifier.
    /// Dictionary<Character_Script, float> affected_characters - the list of Characters affected by the target mapped to their modifier
    /// Dictionary<Object_Script, float> affected_objects - the list of Objects affected by the target mapped to their modifier.
    /// List<Tile> curr_path - The List of tiles to traverse to reach the center of the target from the character's position.
    /// curr_path_cost - The cost of traversing the Target's curr_path.
    /// </summary>
    public Tile center { get; private set; }
    public Dictionary<Tile, float[]> affected_tiles { get; private set; }
    public Dictionary<Character_Script, float> affected_characters { get; private set; }
    public Dictionary<Object_Script, float> affected_objects { get; private set; }
    public List<Tile> curr_path { get; private set; }
    public float curr_path_cost { get; private set; }

    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="new_target">The GameObject to target.</param>
    /// <param name="new_modifier">The modifier to attach to the Target.</param>
    public Target(Tile new_center, List<MDFloat> area)
    {
        center = new_center;
        affected_tiles = new Dictionary<Tile, float[]>();
        affected_characters = new Dictionary<Character_Script, float>();
        affected_objects = new Dictionary<Object_Script, float>();
        curr_path = new List<Tile>();
        curr_path_cost = 0;

        //String[] area_effect = center.Split(' ');
        int startX = center.index[0];
        int startY = center.index[1];
        
        //Set the start of the loop
        startX -= (area.Count / 2);
        startY -= (area[0].Count / 2);
        //Loop through the area and find valid targets
        for (int x = 0; x < area.Count; x++)
        {
            for (int y = 0; y < area[x].Count; y++)
            {
                if (area[x][y] != 0)
                {
                    //Transform target = character.controller.curr_scenario.tile_grid.getTile(startX + x, startY + y);
                    Tile target = Game_Controller.curr_scenario.tile_grid.getTile(startX + x, startY + y).GetComponent<Tile>();
                    if (target != null)
                    {
                        float[] modifiers = new float[2];
                        modifiers[0] = area[x][y];
                        modifiers[1] = 0;
                        if (target.obj != null)
                        {
                            Character_Script chara = target.obj.GetComponent<Character_Script>();
                            if (chara != null)
                            {
                                affected_characters.Add(chara, area[x][y]);
                            }else
                            {
                                affected_objects.Add(target.obj.GetComponent<Object_Script>(), area [x][y]);
                            }
                            modifiers[1] = area[x][y];
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

    /// <summary>
    /// Sets the path_cost for the target
    /// </summary>
    /// <param name="cost">The cost of the path.</param>
    public void Set_Path_Cost(float cost)
    {
        curr_path_cost = cost;
    }

    /// <summary>
    /// Adds to the path_cost for the target
    /// </summary>
    /// <param name="cost">The amount to add to the cost of the path.</param>
    public void Add_Path_Cost(float cost)
    {
        curr_path_cost += cost;
    }

}
