using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Effect_Editor_UI : MonoBehaviour {

    Dropdown type;
    Dropdown affects;
    Dropdown checks;
    InputField values;
    InputField min_targets;
    InputField max_targets;
    InputField advantage_required;
    public GameObject value_creator;
    public GameObject check_creator;
    Dropdown output;
    string[] default_checks = new string[1] { "CHK_CC_ADV_GTE_CC_ADR" };

    /// <summary>
    /// Adds a check to the list. Opens the Check Editor.
    /// </summary>
    public void Add_Check()
    {
        check_creator.GetComponent<Check_Editor_UI>().Enable(checks);
    }

    /// <summary>
    /// Adds a value to the list. Opens up the Value Editor.
    /// </summary>
    public void Add_Value()
    {
        value_creator.GetComponent<Value_Editor_UI>().Enable(type,values);
    }

    /// <summary>
    /// Removes a check from the check list.
    /// </summary>
    public void Remove_Check()
    {
        checks.options.RemoveAt(checks.value);
        checks.RefreshShownValue();
    }

    /// <summary>
    /// Removes a value from the value list.
    /// </summary>
    public void Remove_Value()
    {
        values.text = "";
    }

    public void Edit_Check()
    {
        check_creator.GetComponent<Check_Editor_UI>().Enable(checks, checks.options[checks.value].text);
    }

    /// <summary>
    /// Opens the Value Editor with a specific value to Edit. 
    /// </summary>
    public void Edit_Value()
    {
        value_creator.GetComponent<Value_Editor_UI>().Enable(type,values,values.text);
    }

    /// <summary>
    /// Create the value and disable the Menu
    /// </summary>
    public void Create()
    {
        Action_Effect action = Validate();
        if (action != null) {
            string json = JsonConvert.SerializeObject(action);
            //Action_Effect action2 = JsonConvert.DeserializeObject<Action_Effect>(json);
            output.options.Add(new Dropdown.OptionData(json));
            output.RefreshShownValue();
            Disable();
        }else
        {
            Game_Controller.Create_Error_Message("Something went wrong with Serializing this Effect!");
        }
    }

    /// <summary>
    /// Validates all fields and returns an Action_Effect if all fields can be read.
    /// </summary>
    /// <returns></returns>
    public Action_Effect Validate()
    {
        Action_Effect action = null;
        float adv;
        int min_targ;
        int max_targ;
        if (float.TryParse(advantage_required.text, out adv))
        {
            if (int.TryParse(min_targets.text, out min_targ))
            {
                if (int.TryParse(max_targets.text, out max_targ))
                {
                    string[] new_checks = new string[checks.options.Count];
                    int i = 0;
                    foreach( Dropdown.OptionData data in checks.options)
                    {
                        new_checks[i] = data.text;
                        i++;
                    }
                    action = new Action_Effect(type.value, adv, affects.value, new_checks, JsonConvert.DeserializeObject<string[]>(values.text) , min_targ, max_targ);
                }
                else
                {
                    Game_Controller.Create_Error_Message("Could not parse max targets! Input a valid entry for max targets!");
                }
            }
            else
            {
                Game_Controller.Create_Error_Message("Could not parse min targets! Input a valid entry for min targets!");
            }
        }else
        {
            Game_Controller.Create_Error_Message("Could not parse advantage required! Input a valid entry for advantage required!");
        }
        return action;
    }

    /// <summary>
    /// Enable the menu.
    /// </summary>
    /// <param name="drop_out">The output dropdown box to put the output.</param>
    public void Enable(Dropdown drop_out)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        output = drop_out;
        type.value = 0;
        type.RefreshShownValue();
        affects.value = 0;
        affects.RefreshShownValue();
        values.text = "";
        checks.ClearOptions();
        foreach (string str in default_checks)
        {
            checks.options.Add(new Dropdown.OptionData(str));
        }
        checks.RefreshShownValue();
        min_targets.text = "0";
        max_targets.text = "0";
        advantage_required.text = "0";
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Enable the menu with preset values.
    /// </summary>
    /// <param name="drop_out">The dropdown to put the output.</param>
    /// <param name="json">A json representation of an Action_Effect</param>
    public void Enable(Dropdown drop_out, string json)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        output = drop_out;
        Action_Effect action = JsonConvert.DeserializeObject<Action_Effect>(json);
        type.value = (int)action.type;
        affects.value = (int)action.affects;
        values.text = JsonConvert.SerializeObject(action.values);
        checks.ClearOptions();
        foreach (string str in action.checks)
        {
            checks.options.Add(new Dropdown.OptionData(str));
        }
        checks.RefreshShownValue();
        advantage_required.text = "" + action.req_advantage;
        min_targets.text = ""+action.min_target_limit;
        max_targets.text = "" + action.max_target_limit;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disable the menu
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Populates the dropdown boxes with the correct fields.
    /// </summary>
    public void Setup_Dropdowns()
    {
        foreach (Effect_Types types in Effect_Types.GetValues(typeof(Effect_Types)))
        {
            string value = types.ToString();
            if (types == Effect_Types.Orient)
            {
                value += " - Changes a Target's orientation";
            }
            else if (types == Effect_Types.Damage)
            {
                value += " - Damages a Target's resources (HP/MP/AP/RP)";
            }
            else if (types == Effect_Types.Heal)
            {
                value += " - Restores a Target's resources (HP/MP/AP/RP)";
            }
            else if (types == Effect_Types.Move)
            {
                value += " - Move a Target to a different Tile";
            }
            else if (types == Effect_Types.Status)
            {
                value += " - Changes a Target's Stats (STR/DEX/INI/ETC) for an amount of turns";
            }
            else if (types == Effect_Types.Condition)
            {
                value += " - Gives a Condition (BLD/BRN/VULN/ETC) to a Target";
            }
            else if (types == Effect_Types.Create)
            {
                value += " - Create a Character/Object/Tile_Effect of a Tile";
            }
            else if (types == Effect_Types.Elevate)
            {
                value += " - Changes a Target Tile's Height";
            }
            else if (types == Effect_Types.Enable)
            {
                value += " - Enable/Disable one of the Target's Actions";
            }
            else if (types == Effect_Types.Pass)
            {
                value += " - End the Target's Turn";
            }


            type.options.Add(new Dropdown.OptionData(value));
        }
        
        type.RefreshShownValue();
    }

    // Use this for initialization
    void Start () {
        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();
        type = dropdowns[0];
        affects = dropdowns[1];
        checks = dropdowns[2];
        InputField[] fields = GetComponentsInChildren<InputField>();
        values = fields[0];
        min_targets = fields[1];
        max_targets = fields[2];
        advantage_required = fields[3];

        Setup_Dropdowns();

        Disable();

        //Set in the editor
        //check_creator = GameObject.Find("Check_Editor");
        //value_creator = GameObject.Find("Value_Editor");
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
