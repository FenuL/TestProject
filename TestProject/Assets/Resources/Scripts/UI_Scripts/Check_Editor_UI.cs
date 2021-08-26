using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class Check_Editor_UI : MonoBehaviour {

    string current_check;
    InputField current_check_input;
    Dropdown output;

    public void Enable(Dropdown drop_out)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        current_check_input.text = "";
        output = drop_out;
        gameObject.SetActive(true);
    }

    public void Enable(Dropdown drop_out, string input)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        output = drop_out;
        current_check_input.text = input;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Create the value and disable the Menu
    /// </summary>
    public void Create()
    {
        if (output != null)
        {
            output.options.Add(new Dropdown.OptionData(current_check_input.text));
        }
        output.RefreshShownValue();
        Disable();
    }

    /// <summary>
    /// Disable the menu
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        current_check_input = gameObject.GetComponentInChildren<InputField>();

        Setup_Dropdowns();

        Disable();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Sets up dropdown boxes for the Checks.
    /// </summary>
    public void Setup_Dropdowns()
    {
        Dropdown[] dropdowns = gameObject.GetComponentsInChildren<Dropdown>();
        List<string> float_options = new List<string>();
        List<string> bool_options = new List<string>();
        foreach (Accepted_Float_Shortcuts sh in Enum.GetValues(typeof(Accepted_Float_Shortcuts)))
        {
            float_options.Add(sh.ToString());
        }
        foreach(Accepted_Bool_Shortcuts sh in Enum.GetValues(typeof(Accepted_Bool_Shortcuts)))
        {
            bool_options.Add(sh.ToString());
        }
        dropdowns[2].AddOptions(float_options);
        dropdowns[5].AddOptions(float_options);
        dropdowns[5].AddOptions(bool_options);

        //set listeners on all dropdowns.
        int i = 0;
        foreach(Dropdown drop in dropdowns)
        {
            drop.onValueChanged.AddListener(delegate {
                Update_Current_Check(drop, i);
            });
            i++;
        }
    }

    public void Update_Current_Check(Dropdown drop, int i)
    {
        Debug.Log(i);
        Debug.Log(drop.value);
        Dropdown[] dropdowns = gameObject.GetComponentsInChildren<Dropdown>();
        if (i == 0)
        {
            if (drop.value == 1)
            {
                current_check = "NOT_" + current_check;
            }
        }
        if (i == 1)
        {

        }
        current_check_input.text = current_check;
    }
}
