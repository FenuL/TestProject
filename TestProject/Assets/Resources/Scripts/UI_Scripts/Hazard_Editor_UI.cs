using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine.UI;

public class Hazard_Editor_UI : MonoBehaviour {
    private static string TEMPLATE_FILEPATH = "Assets/Resources/Templates/Hazards/";

    public GameObject effect_editor;
    public GameObject area_editor;
    public GameObject sprite_preview;

    InputField id_input;
    InputField name_input;
    Dropdown sprite_drop;
    InputField description_input;
    InputField area_input;
    Dropdown effects_drop;
    InputField x_size_input;
    InputField y_size_input;
    InputField x_index_input;
    InputField y_index_input;
    InputField duration_input;
    InputField charges_input;
    InputField owner_input;

    /// <summary>
    /// Enable the menu and populate fields by reading data from a file.
    /// </summary>
    /// <param name="id">The Id of the file you want to open.</param>
    public void Enable(string id)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        if (id != null)
        {
            
        }
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Enable the menu and do not populate fields.
    /// </summary>
    public void Enable()
    {
        transform.localPosition = new Vector3(0, 0, 0);
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
    /// Creates a Hazard on the current Scenario.
    /// </summary>
    public void Create()
    {
        Debug.Log("TODO make this function work");
    }

    /// <summary>
    /// Enables the Area editor.
    /// </summary>
    /// <param name="field">The field to store the input</param>
    public void Add_Area(InputField field)
    {
        area_editor.GetComponent<Area_Editor_UI>().Enable(field);
    }

    /// <summary>
    /// Enables the Action_Effect editor.
    /// </summary>
    /// <param name="drop">The dropdown to put the resulting effect.</param>
    public void Add_Effect(Dropdown drop)
    {
        effect_editor.GetComponent<Effect_Editor_UI>().Enable(drop);
    }

    /// <summary>
    /// Enables the Action_Effect editor to edit an entry.
    /// </summary>
    /// <param name="drop">The dropdown box to store the output.</param>
    public void Edit_Effect(Dropdown drop)
    {
        effect_editor.GetComponent<Effect_Editor_UI>().Enable(drop, drop.options[drop.value].text);
    }

    /// <summary>
    /// Removes an Area
    /// </summary>
    /// <param name="field">The field whose area to remove</param>
    public void Remove_Area(InputField field)
    {
        field.text = "";
    }

    /// <summary>
    /// Removes an Action_Effect from the list.
    /// </summary>
    /// <param name="drop">The dropdown box which entry to remove.</param>
    public void Remove_Effect(Dropdown drop)
    {
        drop.options.RemoveAt(drop.value);
        drop.RefreshShownValue();
    }

    /// <summary>
    /// Sets up the sprite dropdown box.
    /// </summary>
    public void Setup_Sprite_Dropdown()
    {
        sprite_drop.options.Add(new Dropdown.OptionData() { text = "Select Sprite" });

        //Add existing files to the dropdown box
        //TODO: Make this better, generate a text item at build time.
        Object[] files = Resources.LoadAll("Sprites/Hazards/");
        
        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            if (file.name.Contains("_"))
            {
                sprite_drop.options.Add(new Dropdown.OptionData() { text = "Hazards/" + file.name });
            }

        }

        files = Resources.LoadAll("Sprites/Objects/");
        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            if (file.name.Contains("_"))
            {
                sprite_drop.options.Add(new Dropdown.OptionData() { text = "Objects/" + file.name });
            }
        }

        sprite_drop.value = 0;
        sprite_drop.RefreshShownValue();
        sprite_drop.onValueChanged.AddListener(delegate {
            Change_Sprite_Preview(sprite_drop);
        });
    }

    /// <summary>
    /// Alters the sprite preview window based on a dropdown window.
    /// </summary>
    /// <param name="drop">Dropdown to get the sprite name from.</param>
    public void Change_Sprite_Preview(Dropdown drop)
    {
        int index = 0;
        string path = drop.options[drop.value].text;
        if (path.Contains("_"))
        {
            int.TryParse(drop.options[drop.value].text.Split('_')[1], out index);
            path = drop.options[drop.value].text.Split('_')[0];
        }
        Sprite sprite = Resources.LoadAll<Sprite>("Sprites/" + path)[index];
        sprite_preview.GetComponent<Image>().sprite = sprite;
    }

    /// <summary>
    /// Gets the values from all fields and creates a data object.
    /// </summary>
    /// <param name="all_fields">true if you want data from all fields, false if you want a template version.</param>
    /// <returns>Hazard_Data of all the fields.</returns>
    public Hazard_Data Get_Values(bool all_fields)
    {
        Action_Effect[] effects = new Action_Effect[effects_drop.options.Count];
        int x = 0;
        foreach(Dropdown.OptionData option in effects_drop.options)
        {
            effects[x] = JsonConvert.DeserializeObject<Action_Effect>(option.text);
            //Debug.Log(effects[x].req_advantage + ", " + effects[x].type.ToString());
            x++;
        }
        float[,] area = JsonConvert.DeserializeObject<float[,]>(area_input.text);
        double id = 0.00000;
        int x_index = 0;
        int y_index = 0;
        int duration = 0;
        int charges = 0;
        int owner_id = -1;
        int[] size = new int[2];
        int x_size = 0;
        int y_size = 0;
        double.TryParse(id_input.text, out id);
        int.TryParse(x_index_input.text, out x_index);
        int.TryParse(y_index_input.text, out y_index);
        int.TryParse(x_size_input.text, out x_size);
        int.TryParse(y_size_input.text, out y_size);
        int.TryParse(duration_input.text, out duration);
        int.TryParse(charges_input.text, out charges);
        int.TryParse(owner_input.text, out owner_id);
        size[0] = x_size;
        size[1] = y_size;
        if (!all_fields)
        {
            id = (int)id;
            owner_id = -1;
            x_index = 0;
            y_index = 0;
        }

        Hazard_Data data = new Hazard_Data(
            id,
            name_input.text,
            description_input.text,
            "Sprites/" + sprite_drop.options[sprite_drop.value].text,
            effects,
            area,
            size,
            duration, 
            charges, 
            new int[] { x_index, y_index }, 
            owner_id);
        return data;
    }

    /// <summary>
    /// Saves a file to a template. 
    /// </summary>
    public void Save_To_Template()
    {
        Hazard_Data data = Get_Values(false);
        //string json = JsonConvert.SerializeObject(data);
        //Debug.Log(json);
        StartCoroutine(Game_Controller.Serialize_To_File<Hazard_Data>(data, TEMPLATE_FILEPATH + (int)data.id + "-" + data.hazard_name));
    }

    // Use this for initialization
    void Start () {
        InputField[] fields = GetComponentsInChildren<InputField>();
        id_input = fields[0];
        name_input = fields[1];
        description_input = fields[2];
        area_input = fields[3];
        x_size_input = fields[4];
        y_size_input = fields[5];
        x_index_input = fields[6];
        y_index_input = fields[7];
        duration_input = fields[8];
        charges_input = fields[9];
        owner_input = fields[10];
        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();
        sprite_drop = dropdowns[0];
        effects_drop = dropdowns[1];

        Setup_Sprite_Dropdown();
        Disable();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
