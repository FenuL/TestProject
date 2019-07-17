using UnityEngine;
using System.Collections;

/// <summary>
/// Script for handling interaction with Non Character Objects on the Tile Grid. 
/// </summary>
public class Object_Script : MonoBehaviour {
    public Transform curr_tile { get; set; }

    /// <summary>
    /// Deal damage to this Object.
    /// </summary>
    /// <param name="amount"> The amount of damage to take. </param>
    /// <param name="armor_penetration">The amount of armor to ignore. Set to -1 to ignore all armor.</param>
    public void Take_Damage(float amount, float armor_penetration)
    {
        Debug.Log("Object " + name + " takes " + amount + " damage!");
        /*if (aura_curr == 0)
        {
            Die();
        }
        else
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
    /// Game_Controller controller - The game controller object. 
    /// </summary>
    public Game_Controller controller { get; set; }

    /// <summary>
    /// Used for initialization
    /// </summary>
    void Start () {
        controller = Game_Controller.controller;
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
