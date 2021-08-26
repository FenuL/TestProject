using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Editor_UI : MonoBehaviour {

    private static string EDITOR_START_PREFAB_SRC = "Prefabs/UI_Prefabs/Inspector";
    private static string CHARACTER_TEMPLATES = "Templates/Characters/";
    private static string OBJECT_TEMPLATES = "Templates/Objects/";
    private static string HAZARD_TEMPLATES = "Templates/Hazards/";

    private Editor_Controller controller;
    private Dropdown inspector_mode_selector;
    private GameObject curr_inspector;
    private GameObject scenario_inspector;
    public GameObject scenario_mode_selector;
    private GameObject new_scenario_inspector;
    private GameObject edit_scenario_inspector;
    public GameObject tile_inspector;
    public GameObject character_editor;
    public GameObject object_editor;
    public GameObject hazard_editor;
    private bool enable_listeners = true;

    public int Get_Inspector_Mode(){
        return inspector_mode_selector.value;
    }

    // Use this for initialization
    void Start()
    {
        Setup_Listeners();
        controller = GameObject.FindGameObjectWithTag("Editor_Controller").GetComponent<Editor_Controller>();
        Game_Controller.controller.Set_Editor(this);
        inspector_mode_selector = gameObject.transform.GetChild(0).GetChild(0).GetComponent<Dropdown>();
        inspector_mode_selector.onValueChanged.AddListener(delegate
        {
            Change_Inspector_Type(inspector_mode_selector.value);
        });

        Setup_Scenario_Inspector();
        curr_inspector = scenario_inspector;

        Setup_Tile_Inspector();
        tile_inspector.SetActive(false);
        edit_scenario_inspector.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {
	}

    public void Update_Scenario_Dropdown()
    {

    }

    /// <summary>
    /// Brings up the Character_Editor so we can add a new character.
    /// </summary>
    public void Add_New_Character()
    {
        character_editor.GetComponent<Character_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Character_Editor with information on an existing character.
    /// </summary>
    public void Edit_Character()
    {
        character_editor.GetComponent<Character_Editor_UI>().Enable();
    }

    /// <summary>
    /// Clears the Character from the selected tile(s)
    /// </summary>
    public void Clear_Character()
    {

    }

    /// <summary>
    /// Adds a Character to the selected tile(s) from an existing character template
    /// </summary>
    public void Add_Character_From_Template()
    {
        character_editor.GetComponent<Character_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Character Menu with stats from an existing template to edit it.
    /// </summary>
    public void Edit_Character_Template()
    {
        character_editor.GetComponent<Character_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Object_Editor so we can add a new object.
    /// </summary>
    public void Add_New_Object()
    {
        object_editor.GetComponent<Object_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Object_Editor with information on an existing object.
    /// </summary>
    public void Edit_Object()
    {
        object_editor.GetComponent<Object_Editor_UI>().Enable();
    }

    /// <summary>
    /// Clears the Object from the selected tile(s)
    /// </summary>
    public void Clear_Object()
    {

    }

    /// <summary>
    /// Adds a Object to the selected tile(s) from an existing object template
    /// </summary>
    public void Add_Object_From_Template()
    {
        object_editor.GetComponent<Object_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Object Menu with stats from an existing template to edit it.
    /// </summary>
    public void Edit_Object_Template()
    {
        object_editor.GetComponent<Object_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Hazard_Editor so we can add a new hazard.
    /// </summary>
    public void Add_New_Hazard()
    {
        hazard_editor.GetComponent<Hazard_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Hazard_Editor with information on an existing hazard.
    /// </summary>
    public void Edit_Hazard()
    {
        hazard_editor.GetComponent<Hazard_Editor_UI>().Enable();
    }

    /// <summary>
    /// Clears the Hazard from the selected tile(s)
    /// </summary>
    public void Clear_Hazard()
    {

    }

    /// <summary>
    /// Adds a Hazard to the selected tile(s) from an existing hazard template
    /// </summary>
    public void Add_Hazard_From_Template()
    {
        hazard_editor.GetComponent<Hazard_Editor_UI>().Enable();
    }

    /// <summary>
    /// Brings up the Hazard Menu with stats from an existing template to edit it.
    /// </summary>
    public void Edit_Hazard_Template()
    {
        hazard_editor.GetComponent<Hazard_Editor_UI>().Enable();
    }

    public void test()
    {
        Debug.Log("test");
    }

    /// <summary>
    /// Sets up Listeners for determining if the mouse is over the UI.
    /// </summary>
    public void Setup_Listeners()
    {
        EventTrigger trigger = GetComponentInParent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((eventData) => { Game_Controller.controller.Mouse_Enter_UI(); });
        trigger.triggers.Add(entry);

        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;
        entry2.callback = new EventTrigger.TriggerEvent();
        entry2.callback.AddListener((eventData) => { Game_Controller.controller.Mouse_Exit_UI(); });
        trigger.triggers.Add(entry2);
    }

    /// <summary>
    /// Sets up the correct listeners and delegate methods for the scenario inspector.
    /// </summary>
    public void Setup_Scenario_Inspector()
    {
        //Gets the right inspector pieces
        scenario_inspector = GameObject.FindGameObjectWithTag("Scenario_Inspector");
        new_scenario_inspector = scenario_inspector.transform.GetChild(1).gameObject;

        //Input validation for certain fields.
        new_scenario_inspector.transform.GetChild(0).GetChild(0).GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            new_scenario_inspector.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Text>()[1].text = "" + Check_Bound(new_scenario_inspector.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Text>()[1].text, Tile_Grid.MIN_WIDTH, Tile_Grid.MAX_WIDTH);
        });
        new_scenario_inspector.transform.GetChild(1).GetChild(0).GetComponent<InputField>().onEndEdit.AddListener(delegate
        {
            new_scenario_inspector.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Text>()[1].text = "" + Check_Bound(new_scenario_inspector.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Text>()[1].text, Tile_Grid.MIN_LENGTH, Tile_Grid.MAX_LENGTH);
        });
        edit_scenario_inspector = scenario_inspector.transform.GetChild(2).gameObject;

        //Add existing files to the dropdown box
        //TODO: Make this better, generate a text item at build time.
        Object[] files = Resources.LoadAll("Scenarios");
        foreach (Object file in files)
        {
            // file extension check
            scenario_mode_selector.GetComponent<Dropdown>().options.Add(new Dropdown.OptionData() { text = file.name });
            //if (file.name.Contains(".txt"))
            //{
                
            //}

        }

        //sets up the dropdowns correctly
        Setup_Scenario_Inspector_Dropdowns();

        //Sets up the buttons correctly
        Setup_Scenario_Inspector_Buttons();
    }

    /// <summary>
    /// Loads a scenario from a filepath
    /// </summary>
    /// <param name="filepath"></param>
    public void Load_Scenario(Dropdown drop)
    {
        new_scenario_inspector.SetActive(false);
        edit_scenario_inspector.SetActive(true);
        Game_Controller.controller.Set_Scenario(controller.Get_Scenario());
        string filepath = drop.options[drop.value].text;
        Scenario_Data data = Game_Controller.Deserialize_From_File<Scenario_Data>("Scenarios/" + filepath);
        data.Print_Json();
        controller.Get_Scenario().Initialize(data);
    }

    /// <summary>
    /// Creates a new Scenario
    /// </summary>
    public void Create_New_Scenario()
    {
        int width = 0;
        int length = 0;
        if (int.TryParse(new_scenario_inspector.transform.GetChild(0).GetChild(0).GetComponentsInChildren<Text>()[1].text, out width))
        {
            if (int.TryParse(new_scenario_inspector.transform.GetChild(1).GetChild(0).GetComponentsInChildren<Text>()[1].text, out length))
            {
                new_scenario_inspector.SetActive(false);
                edit_scenario_inspector.SetActive(true);
                //UI_Controller.Minimize(new_scenario_inspector);
                //UI_Controller.Maximize(edit_scenario_inspector);

                Game_Controller.controller.Set_Scenario(controller.Get_Scenario());
                controller.Get_Scenario().Initialize(width, length);
            }
        }
    }

    /// <summary>
    /// Exports data on the current Scenario
    /// </summary>
    public void Export_Scenario_Data()
    {
        controller.Update_Fields(inspector_mode_selector.value, curr_inspector);
        controller.Export(inspector_mode_selector.value);
    }

    /// <summary>
    /// Exit the Editor
    /// </summary>
    public void Return_to_Menu()
    {
        Game_Controller.controller.Set_Scenario(null);
        Game_Controller.controller.Load_Scene(Scenes.Main_Menu);
    }

    /// <summary>
    /// Decrease Tile height in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Decrease_Height(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Height(t.height - 1);
        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().height;
        enable_listeners = true;
        controller.Clear_Tiles();
        controller.Mark_Tiles();
    }

    /// <summary>
    /// Increase Tile height in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Increase_Height(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Height(t.height + 1);
        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().height;
        enable_listeners = true;
        controller.Clear_Tiles();
        controller.Mark_Tiles();
    }

    /// <summary>
    /// Decrease Tile rotation in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Decrease_Rotation(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Rotation((int)t.rotation - 90);

        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().rotation;
        enable_listeners = true;
    }

    /// <summary>
    /// Increase Tile rotation in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Increase_Rotation(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Rotation((int)t.rotation + 90);

        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().rotation;
        enable_listeners = true;
    }

    /// <summary>
    /// Decrease Tile type in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Decrease_Type(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Tile_Type((int)t.tile_type - 1);

        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().tile_type;
        enable_listeners = true;
    }

    /// <summary>
    /// Decrease Tile type in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Increase_Type(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Tile_Type((int)t.tile_type + 1);

        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().tile_type;
        enable_listeners = true;
    }

    /// <summary>
    /// Decrease Tile modifier in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Decrease_Modifier(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Modifier((double)t.modifier - 0.05);

        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().modifier;
        enable_listeners = true;
    }

    /// <summary>
    /// Increase Tile modifier in the Tile Editor
    /// </summary>
    /// <param name="field">Field to update</param>
    public void Increase_Modifier(InputField field)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Modifier((double)t.modifier + 0.05);

        }
        enable_listeners = false;
        field.text = "" + controller.Get_Tile().modifier;
        enable_listeners = true;
    }

    /// <summary>
    /// Change the Material of a Tile in the Tile Inspector
    /// </summary>
    /// <param name="drop">The dropdown to get the value to change from</param>
    public void Change_Material(Dropdown drop)
    {
        foreach (Tile t in controller.Get_Tiles())
        {
            t.Set_Material(drop.value);
            drop.RefreshShownValue();
        }
    }

    public void Add_Hazard_From_Template(Dropdown drop)
    {
        foreach (Tile t in controller.Get_Tiles()) {
            string filepath =  HAZARD_TEMPLATES + drop.options[drop.value].text;
            Hazard_Data data = Game_Controller.Deserialize_From_File<Hazard_Data>(filepath);
            controller.Get_Scenario().Add_Hazard(t, data);
        }
    }

    /// <summary>
    /// Sets up the buttons correctly for the scenario inspector.
    /// </summary>
    public void Setup_Scenario_Inspector_Buttons()
    {
    }

    public void Setup_Scenario_Inspector_Dropdowns()
    {
        Dropdown[] dropdowns = scenario_inspector.GetComponentsInChildren<Dropdown>();
        string[] objectives = System.Enum.GetNames(typeof(Scenario_Objectives));
        for (int i = 0; i < objectives.Length; i++)
        {
            dropdowns[1].options.Add(new Dropdown.OptionData() { text = objectives[i] });
        }
        dropdowns[1].value = 0;
        dropdowns[1].RefreshShownValue();
    }

    /// <summary>
    /// Checks if a given value is an int between a minimum and a maximum.
    /// </summary>
    /// <param name="value">The value to test.</param>
    /// <param name="min">The minimum value to check.</param>
    /// <param name="max">The maximum value to check.</param>
    /// <returns>returns an int between min and max values.</returns>
    private int Check_Bound(string value, int min, int max)
    {
        int i = min;
        if (int.TryParse(value, out i))
        {
            if( i < min)
            {
                i = min;
            }
            if( i > max)
            {
                i = max;
            }
        }
        return i;
    }

    /// <summary>
    /// Changes the inspector type depending on the mode we are in.
    /// </summary>
    /// <param name="value">The inspector number to use.</param>
    public void Change_Inspector_Type(int value)
    {
        inspector_mode_selector.value = value;
        curr_inspector.SetActive(false);
        if (value == 0)
        {

            curr_inspector = scenario_inspector;

            //UI_Controller.Maximize(scenario_inspector);
        }
        else if (value == 1)
        {
            curr_inspector = tile_inspector;
            Populate_Tile_Inspector();
        }
        /*if (value != 0)
        {
            curr_inspector = null;
            UI_Controller.Minimize(scenario_inspector);
        }*/
        curr_inspector.SetActive(true);
    }

    public void Setup_Tile_Inspector()
    {
        Dropdown dropdown = tile_inspector.GetComponentInChildren<Dropdown>();
        dropdown.ClearOptions();
        InputField[] fields = tile_inspector.GetComponentsInChildren<InputField>();
        //Height Selector
        fields[0].onValueChanged.AddListener(delegate
        {
            if (enable_listeners)
            {
                int new_height;
                int.TryParse(fields[0].text, out new_height);
                foreach (Tile t in controller.Get_Tiles())
                {
                    t.Set_Height(new_height);
                }
                //controller.Get_Tile().Set_Height(new_height);
                //Debug.Log("Value changed");
                fields[0].text = "" + controller.Get_Tile().height;
                controller.Clear_Tiles();
                controller.Mark_Tiles();
            }
        });
        //Rotation Selector
        fields[1].onEndEdit.AddListener(delegate
        {
            if (enable_listeners)
            {
                int new_rotation;
                int.TryParse(fields[1].text, out new_rotation);
                foreach (Tile t in controller.Get_Tiles())
                {
                    t.Set_Rotation(new_rotation);
                }
                fields[1].text = "" + controller.Get_Tile().rotation;
            }
        });
        //Type selector
        fields[2].onEndEdit.AddListener(delegate
        {
            if (enable_listeners)
            {
                int new_type;
                int.TryParse(fields[2].text, out new_type);
                foreach (Tile t in controller.Get_Tiles())
                {
                    t.Set_Tile_Type(new_type);
                }
                fields[2].text = "" + controller.Get_Tile().tile_type;
            }
        });
        //Modifier Selector
        fields[3].onEndEdit.AddListener(delegate
        {
            if (enable_listeners)
            {
                double new_modifier;
                double.TryParse(fields[4].text, out new_modifier);
                foreach (Tile t in controller.Get_Tiles())
                {
                    t.Set_Modifier(new_modifier);
                }
                fields[4].text = "" + controller.Get_Tile().modifier;
            }
        });

        Setup_Tile_Inspector_Template_Dropdowns();
        //fields[1].text = "" + controller.Get_Tile().index[1];
        //fields[2].text = "" + controller.Get_Tile().height;
        //fields[3].text = "" + controller.Get_Tile().rotation;
        //fields[4].text = "" + controller.Get_Tile().tile_type;
        //fields[5].text = "" + controller.Get_Tile().material;
        //fields[6].text = "" + controller.Get_Tile().modifier;
        //fields[7].text = "" + 100;
        //fields[8].text = "" + 100;
        //fields[9].text = "" + controller.Get_Tile().index[0];

    }

    /// <summary>
    /// Sets up the template dropdowns for the Tile Inspector.
    /// </summary>
    public void Setup_Tile_Inspector_Template_Dropdowns()
    {
        
        // TODO, improve this method because load all is expensive
        Dropdown temp_chara_drop = tile_inspector.GetComponentsInChildren<Dropdown>()[3];
        temp_chara_drop.ClearOptions();
        TextAsset[] files = Resources.LoadAll<TextAsset>("Templates/Characters/");
        foreach (Object file in files)
        {
            temp_chara_drop.options.Add(new Dropdown.OptionData() { text = file.name });
        }
        temp_chara_drop.value = 0;
        temp_chara_drop.RefreshShownValue();

        Dropdown temp_obj_drop = tile_inspector.GetComponentsInChildren<Dropdown>()[5];
        temp_obj_drop.ClearOptions();
        files = Resources.LoadAll<TextAsset>("Templates/Objects/");
        foreach (Object file in files)
        {
            temp_obj_drop.options.Add(new Dropdown.OptionData() { text = file.name });
        }
        temp_obj_drop.value = 0;
        temp_obj_drop.RefreshShownValue();

        Dropdown temp_haz_drop = tile_inspector.GetComponentsInChildren<Dropdown>()[7];
        temp_haz_drop.ClearOptions();
        files = Resources.LoadAll<TextAsset>("Templates/Hazards/");
        foreach (Object file in files)
        {
            temp_haz_drop.options.Add(new Dropdown.OptionData() { text = file.name });
        }
        temp_haz_drop.value = 0;
        temp_haz_drop.RefreshShownValue();
    }

    public void Populate_Tile_Inspector()
    {
        Dropdown tile_dropdown = tile_inspector.GetComponentsInChildren<Dropdown>()[0];
        tile_dropdown.ClearOptions();
        foreach (Tile tile in controller.Get_Tiles())
        {
            tile_dropdown.options.Add(new Dropdown.OptionData() { text = "X: " + tile.index[0] + ", Y: " + tile.index[1] });
        }
        tile_dropdown.value = 0;
        tile_dropdown.RefreshShownValue();

        Dropdown mats_dropdown = tile_inspector.GetComponentsInChildren<Dropdown>()[1];
        mats_dropdown.ClearOptions();
        foreach (Material mat in Resources.LoadAll<Material>(Tile.TILE_MATS))
        {
            mats_dropdown.options.Add(new Dropdown.OptionData() { text = mat.name });
        }

    Dropdown character_dropdown = tile_inspector.GetComponentsInChildren<Dropdown>()[2];
        character_dropdown.ClearOptions();
        foreach (Tile tile in controller.Get_Tiles())
        {
            string name = "None";
            if (tile.Has_Character())
            {
                name = tile.character.GetComponent<Character_Script>().character_name;
            }
            character_dropdown.options.Add(new Dropdown.OptionData() { text = "(" + tile.index[0] + ", " + tile.index[1] + "): " + name});
        }
        character_dropdown.value = 0;
        character_dropdown.RefreshShownValue();

        foreach (Tile tile in controller.Get_Tiles())
        {
            string name = "None";
            if (tile.Has_Character())
            {
                name = tile.character.GetComponent<Character_Script>().character_name;
            }
            character_dropdown.options.Add(new Dropdown.OptionData() { text = "(" + tile.index[0] + ", " + tile.index[1] + "): " + name });
        }

        Dropdown object_dropdown = tile_inspector.GetComponentsInChildren<Dropdown>()[4];
        object_dropdown.ClearOptions();
        foreach (Tile tile in controller.Get_Tiles())
        {
            string name = "None";
            if (tile.Has_Object())
            {
                name = tile.obj.GetComponent<Object_Script>().obj_name;
            }
            object_dropdown.options.Add(new Dropdown.OptionData() { text = "(" + tile.index[0] + ", " + tile.index[1] + "): " + name });
        }
        object_dropdown.value = 0;
        object_dropdown.RefreshShownValue();

        Dropdown hazard_dropdown = tile_inspector.GetComponentsInChildren<Dropdown>()[6];
        hazard_dropdown.ClearOptions();
        foreach (Tile tile in controller.Get_Tiles())
        {
            string name = "None";
            if (tile.Has_Object())
            {
                name = tile.hazard.GetComponent<Hazard>().hazard_name;
            }
            hazard_dropdown.options.Add(new Dropdown.OptionData() { text = "(" + tile.index[0] + ", " + tile.index[1] + "): " + name });
        }
        hazard_dropdown.value = 0;
        hazard_dropdown.RefreshShownValue();



        //dropdown.value = 0;
        //dropdown.itemText = dropdown.options[0];
        InputField[] fields = tile_inspector.GetComponentsInChildren<InputField>();
        //fields[0].text = "" + controller.Get_Tile().index[0];
        //fields[1].text = "" + controller.Get_Tile().index[1];
        fields[0].text = "" + controller.Get_Tile().height;
        fields[1].text = "" + controller.Get_Tile().rotation;
        fields[2].text = "" + controller.Get_Tile().tile_type;
        fields[3].text = "" + controller.Get_Tile().modifier;
        fields[4].text = "" + 100;
        fields[5].text = "" + 100;
    }

    
}
