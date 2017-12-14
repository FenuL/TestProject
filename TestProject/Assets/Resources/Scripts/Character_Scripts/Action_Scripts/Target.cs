using UnityEngine;
using System.Collections;

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
    public GameObject game_object { get; private set; }
    public float modifier { get; private set; }

    /// <summary>
    /// Constructor for the class
    /// </summary>
    /// <param name="new_target">The GameObject to target.</param>
    /// <param name="new_modifier">The modifier to attach to the Target.</param>
    public Target(GameObject new_target, float new_modifier)
    {
        game_object = new_target;
        modifier = new_modifier;
    }
}
