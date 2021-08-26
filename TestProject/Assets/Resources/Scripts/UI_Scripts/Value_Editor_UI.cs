using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json;
using System.Collections.Generic;

public class Value_Editor_UI : MonoBehaviour {

    Text type;
    Text description;
    Dropdown target_shortcuts;
    Dropdown value_selector;
    Dropdown float_shortcuts;
    Dropdown bool_shortcuts;
    Dropdown condi_shortcuts;
    GameObject[] values;
    GameObject[] value_inputs;
    GameObject[] value_dropdowns;
    InputField output;

    /// <summary>
    /// Enable the menu and prepare it for a specific value type.
    /// </summary>
    /// <param name="type">The dropdown to get the input type from.</param>
    /// <param name="output_field">The InputField to put the output.</param>
    public void Enable(Dropdown drop_in, InputField output_field)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        output = output_field;
        Setup_Text(drop_in.value, null);
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Enable the menu and prepare it for a specific value type.
    /// </summary>
    /// <param name="drop_in">The dropdown to get the input type from.</param>
    /// <param name="output_field">The inputfield to get the output type from.</param>
    /// <param name="input">What the current value to display is.</param>
    public void Enable(Dropdown drop_in, InputField output_field, string input)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        output = output_field;
        Setup_Text(drop_in.value, input);
        gameObject.SetActive(true);
    }

    public void Setup_Text(int input, string json)
    {
        if(input == 0)
        {
            type.text = "Orient";
            description.text = "Changes a character's orientation.\n";
            description.text += "Expected value format: \n";
            description.text += "- 2 Values [SELE/TARG/FORM, formula]\n";
            description.text += "  - \"SELE\" = lets the select an orientation.\n";
            description.text += "  - \"TARG\" = sets the orientation to the action's center.\n";
            description.text += "  - \"FORM\" = sets the orientation to a specific direction.\n";
            description.text += "  - formula = determins the direction if value 1 is FORM.\n";
            description.text += "\n";
            description.text += "Examples:\n";
            description.text += "- [1,0] - Sets the target orientation to be the skill target.\n";
            description.text += "- [2,CC_ORI] - Sets the target orientation to be same as the current character's orientation.\n";
        }
        else if (input == 1)
        {
            type.text = "Move";
            description.text = "Move a character to a different tile.";
            description.text += "Expected value format: \n";
            description.text += "- 5 Values [Move Type, PATH/VECT, Direction, Force, Modifier]\n";
            description.text += "  - Move Type = 0 to 4. Determines what happens during movement.\n";
            description.text += "  - \"PATH\" = Character will move along the path for the Action.\n";
            description.text += "  - \"VECT\" = Character will move in a straight line in a particular direction.\n";
            description.text += "  - Direction = Formula determining the path the chracter will take.\n";
            description.text += "  - Force = Formula determining the amount of tiles the chracter will travel.\n";
            description.text += "  - Modifier = Formula determining how Advantage affects the Force value.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,0,CT_PATH,CC_SPD,0] = Character will WALK up to their SPEED along the Action's Path.\n";
            description.text += "- [2,1,CC_ORI,4,CC_ADV/2] = Character will be PUSHED 4 tiles +1 for every 2 Advantage.\n";
        }
        else if (input == 2)
        {
            type.text = "Damage";
            description.text = "Deal an amount of damage to one of the target's resources.\n";
            description.text += "Expected value format: \n";
            description.text += "- 4 Values: [Damage Type, Resource, Damage, Modifier]\n";
            description.text += "  - Damage Type - Dropdown box determines damage type. Used to determine Advantage.\n";
            description.text += "  - Resource - Dropdown box determines what is taking damage. Typically Aura.\n";
            description.text += "  - Damage - Formula determines how much damage is taken.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects damage taken.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,0,10,CC_ADV/2] = Character will deal 10 (+1 for every 2 Advantage) Kinetic Damage.\n";
            description.text += "- [1,1,CC_WPD+CC_STR,0] = Character will deal Warp Damage based on their strength and \n";
            description.text += "weapon to the target's mana.";
        }
        else if (input == 3)
        {
            type.text = "Heal";
            description.text = "Heal one of the target's resources for an amount.\n";
            description.text += "Expected value format: \n";
            description.text += "- 3 Values: [Resource, Force, Modifier]\n";
            description.text += "  - Resource - Dropdown box determines what is being healed. Typically Aura.\n";
            description.text += "  - Force - Formula determines how much healing is taken.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects healing taken.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,CC_AUM/2,0] = Character will heal half of their maximum Aura, unaffected by Advantage.\n";
            description.text += "- [1,10,CC_ADV] = Character will heal their mana by 10 (+1 per Advantage point).\n";
        }
        else if (input == 4)
        {
            type.text = "Status";
            description.text = "Alter one of the target's stats for a certain amount of turns.\n";
            description.text += "Expected value format: \n";
            description.text += "- 5 Values: [Stat, Force, Modifier, Duration, Modifier]\n";
            description.text += "  - Stat - Dropdown box determines what stat is affected.\n";
            description.text += "  - Force - Formula determines how much the stat is affected.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects the Force.\n";
            description.text += "  - Duration - Formula determines how many turns the the stat is affected.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects the Duration.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,2,0,2,0] = Character's stat will be boosted by 2 for 2 turns, unaffected by Advantage.\n";
        }
        else if (input == 5)
        {
            type.text = "Condition";
            description.text = "Adds a Condition to the target for a certain amount of turns.\n";
            description.text += "Expected value format: \n";
            description.text += "- 5 Values: [Condition, Force, Modifier, Duration, Modifier]\n";
            description.text += "  - Stat - Dropdown box determines what Condition is applied.\n";
            description.text += "  - Force - Formula determines how many of the Condition is applied.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects the Force.\n";
            description.text += "  - Duration - Formula determines how many turns the the Condition lasts.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects the Duration.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,2,0,2,0] = Character's will receive 2 stacks of Bleed for 2 turns, unaffected by Advantage.\n";
        }
        else if (input == 6)
        {
            type.text = "Create";
            description.text = "Creates an Object/Character/Tile_Effect on the target tile.\n";
            description.text += "Expected value format: \n";
            description.text += "- 5 Values: [Type, ID, Delay, Force, Modifier]\n";
            description.text += "  - Type - Dropdown box determines what if the created item is an Object/Character/Effect.\n";
            description.text += "  - ID - Formula for the ID number of the object/item to be created.\n";
            description.text += "  - Delay - Formula determines how many rounds before the item is created.\n";
            description.text += "  - Force - Formula determines how item's characteristics.\n";
            description.text += "  - Modifier - Formula determines how Advantage affects the Force.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,0,0,0,0] = Will spawn a Character with ID 0 (0-99 are loadout characters) with 0 delay.\n";
            description.text += "- [1,100,0,CC_DEX,0] = Will spawn an Object with ID 100 with 0 delay, based on the Char's Dex.\n";
        }
        else if (input == 7)
        {
            type.text = "Elevate";
            description.text = "Raises/Lowers a Tile by the specified amount.\n";
            description.text += "Expected value format: \n";
            description.text += "- 2 Values: [Force, Modifier]\n";
            description.text += "  - Force - Formula to determine how much to raise/lower tile (can be negative).\n";
            description.text += "  - Modifier - Formula determines how Advantage affects the Force.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [CC_STR+2,CC_ADV/2] = Will Raise a Tile by STR +2 + 1 per 2 Advantage.\n";
            description.text += "- [-3,0] = Will Lower a tile by 3, ignoring Advantage.\n";
        }
        else if (input == 8)
        {
            type.text = "Enable";
            description.text = "Enable/Disable a Character's specific Action.\n";
            description.text += "Expected value format: \n";
            description.text += "- 2 Values: [Enable, Name]\n";
            description.text += "  - Enable - Dropdown to enable or disable action.\n";
            description.text += "  - Name - Name of the Action to affect.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0,Charge] = Will Enable an action called \"Charge\".\n";
            description.text += "- [1,Heal] = Will Disable an action called \"Heal\".\n";
        }
        else if (input == 9)
        {
            type.text = "Pass";
            description.text = "End the Character's turn.\n";
            description.text += "Expected value format: \n";
            description.text += "- 1 Values: [Placeholder]\n";
            description.text += "  - Placeholder - Not sure what to do with this yet.\n";
            description.text += "\n";
            description.text += "Examples: \n";
            description.text += "- [0] = End the target's turn. \"Charge\".\n";
        }
        Setup_Values(input, json);
    }

    /// <summary>
    /// Create the value and disable the Menu
    /// </summary>
    public void Create()
    {

        if (output != null)
        {
            string json = "";
            if (type.text == Effect_Types.Orient.ToString())
            {
                json = Get_Values(new int[2] { 0, 1 });
            }
            else if (type.text == Effect_Types.Move.ToString())
            {
                json = Get_Values(new int[5] { 0, 0, 1, 1, 1 });
            }
            else if (type.text == Effect_Types.Damage.ToString())
            {
                json = Get_Values(new int[4] { 0, 0, 1, 1 });
            }
            else if (type.text == Effect_Types.Heal.ToString())
            {
                json = Get_Values(new int[3] { 0, 1, 1});
            }
            else if (type.text == Effect_Types.Status.ToString())
            {
                json = Get_Values(new int[5] { 0, 1,1,1,1 });
            }
            else if (type.text == Effect_Types.Condition.ToString())
            {
                json = Get_Values(new int[5] { 0, 1,1,1,1 });
            }
            else if (type.text == Effect_Types.Create.ToString())
            {
                json = Get_Values(new int[5] { 0, 1,1,1,1 });
            }
            else if (type.text == Effect_Types.Elevate.ToString())
            {
                json = Get_Values(new int[2] { 1, 1 });
            }
            else if (type.text == Effect_Types.Enable.ToString())
            {
                json = Get_Values(new int[2] { 0, 1 });
            }
            else if (type.text == Effect_Types.Pass.ToString())
            {
                json = Get_Values(new int[1] { 1 });
            }
            //
            if (json != null)
            {
                output.text = json;
                Disable();
            }
        }

    }

    /// <summary>
    /// Disable the menu
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds the value from the dropdown to the current value
    /// </summary>
    /// <param name="input"></param>
    public void Add_to_Value(Dropdown input)
    {
        int value_num = value_selector.value;
        if (value_inputs[value_num].activeSelf)
        {
            InputField curr_value = value_inputs[value_selector.value].GetComponent<InputField>();
            curr_value.text = curr_value.text + 
                target_shortcuts.options[target_shortcuts.value].text.Split('-')[0].TrimEnd() + 
                "_" + 
                input.options[input.value].text.Split('-')[0].TrimEnd();
        }
        else
        {
            Game_Controller.Create_Error_Message("Selected Value is invalid for this effect type! Select a different value to modify.");
        }
    }

    /// <summary>
    /// Populates the dropdown boxes with the correct fields.
    /// </summary>
    public void Setup_Dropdowns()
    {
        foreach (Accepted_Float_Shortcuts shortcut in Accepted_Float_Shortcuts.GetValues(typeof(Accepted_Float_Shortcuts)))
        {
            if (Game_Controller.Get_Stat_Descriptions() == null)
            {
                Game_Controller.Load_Stat_Descriptions();
            }
            string value = shortcut.ToString() + " - " + Game_Controller.Get_Stat_Descriptions()[shortcut];
            float_shortcuts.options.Add(new Dropdown.OptionData(value));
        }
        foreach (Accepted_Bool_Shortcuts shortcut in Accepted_Bool_Shortcuts.GetValues(typeof(Accepted_Bool_Shortcuts)))
        {
            string value = shortcut.ToString() + " - " + Game_Controller.Get_Bool_Descriptions()[shortcut];
            bool_shortcuts.options.Add(new Dropdown.OptionData(value));
        }
        foreach (Condition_Shortcuts shortcut in Condition_Shortcuts.GetValues(typeof(Condition_Shortcuts)))
        {
            string value = shortcut.ToString() + " - " + Game_Controller.Get_Condition_Descriptions()[shortcut];
            condi_shortcuts.options.Add(new Dropdown.OptionData(value));
        }

        float_shortcuts.RefreshShownValue();
        bool_shortcuts.RefreshShownValue();
        condi_shortcuts.RefreshShownValue();
    }

    /// <summary>
    /// Sets up the values by type.
    /// </summary>
    /// <param name="input">The Effect type to setup for.</param>
    /// <param name="input">The json string to set the base values. Can be null.</param>
    public void Setup_Values(int input, string json)
    {
        if (input == 0)
        {
            //Orient
            Enable_Values(new int[] { 0, 1 });
            int i = 0;
            foreach (Orient_Types type in Orient_Types.GetValues(typeof(Orient_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 1 }, json);
        }
        else if (input == 1)
        {
            //Move
            Enable_Values(new int[] { 0, 0, 1, 1, 1 });
            int i = 0;
            foreach (Movement_Types type in Movement_Types.GetValues(typeof(Movement_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            i = 0;
            foreach (Pathing_Types type in Pathing_Types.GetValues(typeof(Pathing_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[1].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[1].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 0, 1, 1, 1 }, json);
        }
        else if (input == 2)
        {
            //Damage
            Enable_Values(new int[] { 0, 0, 1, 1 });
            int i = 0;
            foreach (Damage_Types type in Damage_Types.GetValues(typeof(Damage_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            i = 0;
            foreach (Character_Resource_Types type in Character_Resource_Types.GetValues(typeof(Character_Resource_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[1].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[1].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 0, 1, 1 }, json);
        }
        else if (input == 3)
        {
            //Heal
            Enable_Values(new int[] { 0, 1, 1 });
            int i = 0;
            foreach (Character_Resource_Types type in Character_Resource_Types.GetValues(typeof(Character_Resource_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 1, 1 }, json);
        }
        else if (input == 4)
        {
            //Status
            Enable_Values(new int[] { 0, 1, 1, 1, 1 });
            int i = 0;
            foreach (Character_Stats type in Character_Stats.GetValues(typeof(Character_Stats)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 1, 1, 1, 1 }, json);
        }
        else if (input == 5)
        {
            //Condition
            Enable_Values(new int[] { 0, 1, 1, 1, 1 });
            int i = 0;
            foreach (Conditions type in Conditions.GetValues(typeof(Conditions)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 1, 1, 1, 1 }, json);
        }
        else if (input == 6)
        {
            //Create
            Enable_Values(new int[] { 0, 1, 1, 1, 1 });
            int i = 0;
            foreach (Create_Types type in Create_Types.GetValues(typeof(Create_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 1, 1, 1, 1 }, json);
        }
        else if (input == 7)
        {
            //Elevate
            Enable_Values(new int[] { 1, 1 });
            Set_Values(new int[] { 1, 1 }, json);
        }
        else if (input == 8)
        {
            //Enable
            Enable_Values(new int[] { 0, 1});
            int i = 0;
            foreach (Enable_Types type in Enable_Types.GetValues(typeof(Enable_Types)))
            {
                string value = i + " - " + type.ToString();
                value_dropdowns[0].GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(value));
                i++;
            }
            value_dropdowns[0].GetComponent<Dropdown>().RefreshShownValue();
            Set_Values(new int[] { 0, 1}, json);
        }
        else if (input == 9)
        {
            //Pass
            Enable_Values(new int[] { 1 });
            Set_Values(new int[] { 1 }, json);
        }
    }

    /// <summary>
    /// Sets up the values for the editor given an array of values.
    /// </summary>
    /// <param name="vals">The input array. Should be length 6, filled with 0, 1 and 2.</param>
    public void Enable_Values(int[] vals)
    {
        int i = 0;
        foreach(int val in vals)
        {
            if(val == 0)
            {
                values[i].GetComponentsInChildren<Text>()[1].text = "";
                value_inputs[i].SetActive(false);
                value_dropdowns[i].SetActive(true);
                value_dropdowns[i].GetComponent<Dropdown>().ClearOptions();
            }
            if(val == 1)
            {
                values[i].GetComponentsInChildren<Text>()[1].text = "";
                value_inputs[i].SetActive(true);
                value_inputs[i].GetComponent<InputField>().text = "";
                value_dropdowns[i].SetActive(false);
            }
            i++;
        }
        for(int x =i;  x<6; x++)
        {
            values[x].GetComponentsInChildren<Text>()[1].text = "N/A";
            value_inputs[x].SetActive(false);
            value_dropdowns[x].SetActive(false);
        }
    }

    /// <summary>
    /// Sets the values for the dropdowns and input fields to be what was passed in.
    /// </summary>
    /// <param name="array">An array of ints to tell which value is a dropdown and which is an input field.</param>
    /// <param name="json">A json string to be deserialized and passed into the values.</param>
    public void Set_Values(int[] array, string json)
    {
        if(json != null)
        {
            string[] values = JsonConvert.DeserializeObject<string[]>(json);
            int i = 0;
            foreach (int num in array)
            {
                if(num == 0)
                {
                    int x = 0;
                    if(int.TryParse(values[i], out x))
                    {
                        value_dropdowns[i].GetComponent<Dropdown>().value = x;
                    }
                    value_dropdowns[i].GetComponent<Dropdown>().RefreshShownValue();
                }
                else if (num == 1)
                {
                    value_inputs[i].GetComponent<InputField>().text = values[i];
                }
                i++;
            }
        }
    }

    /// <summary>
    /// Gets the values stored in the editor and returns them as a json string.
    /// </summary>
    /// <param name="values">An int[] used to determine which values to get. 0 gets the dropdown value, 1 gets the field value.</param>
    /// <returns></returns>
    public string Get_Values(int[] values)
    {
        string[] output = new string[values.Length];
        int i = 0;
        foreach(int val in values)
        {
            if(val == 0)
            {
                output[i] = "" + value_dropdowns[i].GetComponent<Dropdown>().value;
            }
            else if (val == 1)
            {
                if (Validate(value_inputs[i].GetComponent<InputField>().text))
                {
                    output[i] = value_inputs[i].GetComponent<InputField>().text;
                }else
                {
                    Game_Controller.Create_Error_Message("ERROR! Value " + (i+1) + " could not be parsed. Input a valid entry for value " + (i+1) +".");
                    return null;
                }
            }
            i++;
        }
        return JsonConvert.SerializeObject(output);
    }

    /// <summary>
    /// Reset the values in the dropdown field.
    /// </summary>
    public void Clear_Values()
    {

    }

    /// <summary>
    /// Validates input to see if a given formula is valid.
    /// </summary>
    /// <param name="input">The input to process.</param>
    /// <returns></returns>
    public bool Validate(string input)
    {
        if (input == "")
        {
            return false;
        }
        float f = 0;// Game_Controller.Convert_To_Float(input);
        if(f == -10000)
        {
            return false;
        }
        return true;
    }

	// Use this for initialization
	void Start () {
        Text[] all_text = GetComponentsInChildren<Text>();
        type = all_text[2];
        description = all_text[4];
        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();
        target_shortcuts = dropdowns[0];
        value_selector = dropdowns[1];
        float_shortcuts = dropdowns[2];
        bool_shortcuts = dropdowns[3];
        condi_shortcuts = dropdowns[4];
        InputField[] fields = GetComponentsInChildren<InputField>();
        values = new GameObject[6];
        values[0] = all_text[21].gameObject;
        values[1] = all_text[26].gameObject;
        values[2] = all_text[31].gameObject;
        values[3] = all_text[36].gameObject;
        values[4] = all_text[41].gameObject;
        values[5] = all_text[46].gameObject;
        value_inputs = new GameObject[6];
        value_inputs[0] = fields[0].gameObject;
        value_inputs[1] = fields[1].gameObject;
        value_inputs[2] = fields[2].gameObject;
        value_inputs[3] = fields[3].gameObject;
        value_inputs[4] = fields[4].gameObject;
        value_inputs[5] = fields[5].gameObject;
        value_dropdowns = new GameObject[6];
        value_dropdowns[0] = dropdowns[5].gameObject;
        value_dropdowns[1] = dropdowns[6].gameObject; ;
        value_dropdowns[2] = dropdowns[7].gameObject; ;
        value_dropdowns[3] = dropdowns[8].gameObject; ;
        value_dropdowns[4] = dropdowns[9].gameObject; ;
        value_dropdowns[5] = dropdowns[10].gameObject; ;

        Setup_Text(0, null);
        Setup_Dropdowns();
        Disable();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
