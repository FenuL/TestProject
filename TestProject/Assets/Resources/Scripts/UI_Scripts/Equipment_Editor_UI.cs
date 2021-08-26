using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Equipment_Editor_UI : MonoBehaviour {

    private Dropdown type;
    private InputField name_field;
    private InputField description;
    private InputField modifier;
    private Dropdown action_list;
    private Dropdown actions;
    private Dropdown sprite;
    private Dropdown image;
    private InputField weight;
    private InputField armor;
    private InputField damage;
    private InputField pierce;
    public GameObject action_creator;
    public GameObject area_creator;

    /// <summary>
    /// Opens the area creator to add a modifier string to the editor.
    /// </summary>
    public void Add_Modifier()
    {
        area_creator.GetComponent<Area_Editor_UI>().Enable(modifier);
    }

    /// <summary>
    /// Clears the modifier string
    /// </summary>
    public void Clear_Modifier()
    {
        modifier.text = "";
    }

    /// <summary>
    /// Opens up the Action creation menu to create a new action. 
    /// </summary>
    public void Add_New_Action()
    {
        action_creator.GetComponent<Action_Editor_UI>().Enable();
    }

    public void Add_Action()
    {
        actions.options.Add(new Dropdown.OptionData(action_list.options[action_list.value].text));
        actions.RefreshShownValue();
    }

    public void Remove_Action()
    {
        actions.options.RemoveAt(actions.value);
        actions.RefreshShownValue();
    }

    public void Edit_Action()
    {

    }

    /// <summary>
    /// Creates a new file for storage.
    /// </summary>
    public void Create()
    {
        string[] acts = new string[actions.options.Count];
        for(int x = 0; x< actions.options.Count; x++)
        {
            acts[x] = actions.options[x].text;
        }
        float temp_w;
        float.TryParse(weight.text, out temp_w);
        int temp_d;
        int.TryParse(damage.text, out temp_d);
        float temp_p;
        float.TryParse(pierce.text, out temp_p);
        int temp_a;
        int.TryParse(armor.text, out temp_a);
        Equipment equip = new Weapon(name_field.text, description.text, acts, sprite.options[sprite.value].text, image.options[image.value].text, temp_w, temp_d, temp_p);
        if (type.value == 1)
        {
            equip = new Armor(name_field.text, description.text, acts, sprite.options[sprite.value].text, image.options[image.value].text, temp_w, temp_a);
        }
        else if (type.value == 2)
        {
            equip = new Accessory(name_field.text, description.text, acts, sprite.options[sprite.value].text, image.options[image.value].text, temp_w);
        }

        equip.Export_Data();

        Disable();

    }

    /// <summary>
    /// Disables the Editor and clears the data fields.
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Enables the Editor and clears the fields.
    /// </summary>
    public void Enable()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        name_field.text = "";
        description.text = "";
        weight.text = "0";
        armor.text = "0";
        damage.text = "0";
        pierce.text = "0";
        actions.options.Clear();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Enable the Editor with certain preset settings.
    /// </summary>
    /// <param name="filename"></param>
    public void Enable(string filename)
    {
        transform.position = new Vector3(0, 0, 0);
        Resources.Load(filename);
        gameObject.SetActive(true);
    }

    public void Setup_Dropdowns()
    {
        Object[] files = Resources.LoadAll("Sprites");

        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            sprite.options.Add(new Dropdown.OptionData(file.name));
            image.options.Add(new Dropdown.OptionData(file.name));

        }

        files = Resources.LoadAll("Actions");
        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            action_list.options.Add(new Dropdown.OptionData(file.name));
        }

        action_list.RefreshShownValue();
        sprite.RefreshShownValue();
        image.RefreshShownValue();

    }

	// Use this for initialization
	void Start () {
        Dropdown[] dropdowns = GetComponentsInChildren<Dropdown>();
        type = dropdowns[0];
        action_list = dropdowns[1];
        actions = dropdowns[2];
        sprite = dropdowns[3];
        image = dropdowns[4];

        InputField[] fields = GetComponentsInChildren<InputField>();
        name_field = fields[0];
        description = fields[1];
        modifier = fields[2];
        weight = fields[3];
        armor = fields[4];
        damage = fields[5];
        pierce = fields[6];

        Setup_Dropdowns();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
