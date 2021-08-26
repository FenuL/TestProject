using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

[Serializable]
public class Hazard : MonoBehaviour {
    
    /// <summary>
    /// 
    /// Variables: 
    /// Game_Controller controller - The game controller object.
    /// string name - The name for the Effect. Necessary ebcause sometimes we create the script before the GameObject.
    /// Types type - The Type of the Effect. From the Types enum.
    /// string value - The formula to calculate the Effect's result. Use the Convert_to_Double method to parse this.
    /// float modifier - The modifier for the effect. Passed in when the Effect is created.
    /// int duration - The number of turns the effect is active for. A turn is defined as one Character's turn.
    /// GameObject current_tile - The Tile on which to place the Effect. We use the GameObject so we can get it's positition.
    /// </summary>

    [JsonProperty]
    public double id { get; private set;  }
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
    public List<Tile> current_tiles { get; private set; }
    [JsonProperty]
    public Character_Script owner { get; private set; }

    //Non serialized variables
    float height_offset;
    bool paused;
    bool interrupted;
    Target curr_target;

    /// <summary>
    /// Starts the Hazard with the given correct fields.
    /// </summary>
    /// <param name="data">The Hazard_Data to use to create the Hazard.</param>
    public void Instantiate(Hazard_Data data)
    {
        if (hazard_name == null)
        {
            //Get all the data from the data fields
            id = Game_Controller.Get_Curr_Scenario().Generate_Id(data.id, this.gameObject);
            hazard_name = data.hazard_name;
            description = data.description;
            sprite_name = data.sprite_name;
            effects = data.effects;
            area = data.area;
            duration = data.duration;
            charges = data.charges;
            size = data.size;
            paused = false;
            interrupted = false;
            curr_target = null;

            current_tiles = new List<Tile>();
            for(int x =0; x< size[0]; x++)
            {
                for(int y =0; y< size[1]; y++)
                {
                    current_tiles.Add(Game_Controller.Get_Curr_Scenario().Get_Tile(data.current_tile_index[0]+x, data.current_tile_index[1] +y));
                }
            }
            if (data.owner_id != -1)
            {
                owner = Game_Controller.Get_Curr_Scenario().characters[data.owner_id].GetComponent<Character_Script>();
            }
            string spritesheet = sprite_name.Split('_')[0];
            int sprite_id = 0;
            int.TryParse(data.sprite_name.Split('_')[1], out sprite_id);
            Sprite[] sprites = Resources.LoadAll<Sprite>(spritesheet);
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[sprite_id];

            //Calculate offsets
            height_offset = (gameObject.GetComponent<SpriteRenderer>().sprite.rect.height *
                gameObject.transform.localScale.y /
                gameObject.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit /
                2);
            //offset = (height_offset) / 3.5f;

            //Reset position with height offset
            transform.position = new Vector3(transform.position.x, transform.position.y+ height_offset, transform.position.z);
        }
    }

    /// <summary>
    /// Returns the id of the Hazard in the scenario. Used to find the Hazard in the Hazard dictionary.
    /// </summary>
    /// <returns>Id of the Hazard in the scenario.</returns>
    public int Get_Scenario_ID()
    {
        return (int)((id % 1) * 10000);
    }

    /// <summary>
    /// A Method to check if a Hazard has a particular Effect type.
    /// </summary>
    /// <param name="type"> The Effect_Type to look for</param>
    /// <returns></returns>
    public bool Has_Effect(Effect_Types type)
    {
        foreach (Action_Effect effect in effects)
        {
            if (effect.type == type)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Decrease the duration of the Hazard and returns the updated duration. 
    /// </summary>
    /// <returns>The remaining duration in turns for the Hazard.</returns>
    public int Progress()
    {
        duration -= 1;
        if (duration <= 0)
        {
            gameObject.tag = "Delete";
            foreach(Tile t in current_tiles)
            {
                t.Set_Hazard(null);
            }
        }
        return duration;
    }

    /// <summary>
    /// Returns a data object carrying this information. Used for saving and loading.
    /// </summary>
    /// <returns>A Hazard_Data object whose fields match these.</returns>
    public Hazard_Data Export_Data()
    {
        return new Hazard_Data(this);
    }

    /// <summary>
    /// Coroutine for triggering the Effect.
    /// Calls the various Enact_<>() Functions depending on the Actions's Action_Effects. 
    /// </summary>
    /// <param name="character">Character triggering this Effect.</param>
    /// <param name="trigger">The tile at which the Hazard was triggerd.</param>
    /// <returns>An IEnumerator with the current Coroutine progress. </returns>
    public IEnumerator Enact(Character_Script character, Tile trigger)
    {
        while (paused)
        {
            yield return new WaitForEndOfFrame();
        }
        curr_target = new Target(trigger, area);
        duration -= 1;
        foreach (Tile t in curr_target.affected_tiles.Keys)
        {
            while (paused)
            {
                yield return new WaitForEndOfFrame();
            }
            if (interrupted)
            {
                break;
            }
            foreach (Action_Effect effect in effects)
            {
                Event_Manager.Broadcast(Event_Trigger.ON_TARGET, null, "", t.gameObject);
                while (paused && !interrupted)
                {
                    //TODO add an interrupt here.
                    yield return new WaitForEndOfFrame();
                }
                if (interrupted)
                {
                    break;
                }
                yield return StartCoroutine(effect.Enact_Effect(character.Get_Curr_Action(), owner.gameObject, t.gameObject, curr_target));
            }
        }
        interrupted = false;

        curr_target = null;
        if (duration <= 0)
        {
            gameObject.tag = "Delete";
        }
        charges -= 1;
        if(charges <= 0)
        {
            gameObject.tag = "Delete";
        }
    }

    /// <summary>
    /// Checks if a tile can be a valid target.
    /// </summary>
    /// <returns></returns>
    public bool Valid_Target(Tile target)
    {
        if (target != null)
        {
            if (target.height <= 0)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Called once per Frame to update the Object. 
    /// Makes the Object face the Camera.
    /// </summary>
    void Update()
    {
        //Change sprite facing to match current camera angle
        transform.eulerAngles = new Vector3(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
