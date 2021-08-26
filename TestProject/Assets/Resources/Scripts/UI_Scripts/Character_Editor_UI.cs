using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class Character_Editor_UI : MonoBehaviour {

    private static string CREATOR_MENU_PREFAB_SRC = "Prefabs/UI_Prefabs/Character_Editor";
    private Dropdown selector;
    private GameObject details_menu;
    private GameObject stats_menu;
    private GameObject equipment_menu;
    private GameObject abilities_menu;

    Character_Script_Data data;

    public Character_Script_Data Get_Data()
    {
        return data;
    }
    public void Set_Data(Character_Script_Data new_data) {
        data = new_data;
    }

    /// <summary>
    /// Disables the Editor
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        Disable();
        //Get_Components();

        //Setup_Main_Menu();

        //Setup_Details();

        //Setup_Stats();

        //Setup_Equipment();

        //Setup_Abilities();
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Enable()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);

    }

    /// <summary>
    /// Gets the various components for easy handling later.
    /// </summary>
    public void Get_Components()
    {
        selector = gameObject.GetComponentsInChildren<Dropdown>()[0];

        //details_menu = transform.Find("Character_Details").gameObject;
        //stats_menu = transform.Find("Character_Stats").gameObject;
        //equipment_menu = transform.Find("Character_Equipment").gameObject;
        //abilities_menu = transform.Find("Character_Abilities").gameObject;
    }

    /// <summary>
    /// Shows a specific Menu and hides the others
    /// </summary>
    public void Refresh_Menu()
    {
        if (selector.value == 0)
        {
            details_menu.SetActive(true);
            stats_menu.SetActive(false);
            equipment_menu.SetActive(false);
            abilities_menu.SetActive(false);
        }
        else if (selector.value == 1)
        {
            details_menu.SetActive(false);
            stats_menu.SetActive(true);
            equipment_menu.SetActive(false);
            abilities_menu.SetActive(false);
        }
        else if (selector.value == 2)
        {
            details_menu.SetActive(false);
            stats_menu.SetActive(false);
            equipment_menu.SetActive(true);
            abilities_menu.SetActive(false);
        }
        else if (selector.value == 3)
        {
            details_menu.SetActive(false);
            stats_menu.SetActive(false);
            equipment_menu.SetActive(false);
            abilities_menu.SetActive(true);
        }

    }

    public void Setup_Main_Menu()
    {
        selector.onValueChanged.AddListener(delegate {
            Refresh_Menu();
        });

        selector.value = 0;

        Button[] buttons = gameObject.GetComponentsInChildren<Button>();

        //Set up the Create button
        buttons[buttons.Length-4].onClick.AddListener(() => {
            Create_Character();
            //GameObject.Destroy(this);
            gameObject.SetActive(false);
        });
        //Set up the Save to Template button
        buttons[buttons.Length - 3].onClick.AddListener(() => {
            //Create_Character();
            //GameObject.Destroy(this);
            //gameObject.SetActive(false);
        });
        //Set up the Delete Template button
        buttons[buttons.Length - 2].onClick.AddListener(() => {
            //Create_Character();
            //GameObject.Destroy(this);
            //gameObject.SetActive(false);
        });
        //Set up the Exit button
        buttons[buttons.Length - 1].onClick.AddListener(() => {
            //GameObject.Destroy(this);
            gameObject.SetActive(false);
        });
    }

    public void Setup_Details()
    {
        Dropdown[] dropdowns = details_menu.GetComponentsInChildren<Dropdown>();

        dropdowns[0].options.Add(new Dropdown.OptionData() { text = "Select Sprite" });

        //Add existing files to the dropdown box
        //TODO: Make this better, generate a text item at build time.
            Object[] files = Resources.LoadAll("Sprites/Character_Sprites/Enemy_Sprites");

        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            if(!file.name.Contains("_")){
                dropdowns[0].options.Add(new Dropdown.OptionData() { text = "Enemy_Sprites/" + file.name });
            }

        }
        files = Resources.LoadAll("Sprites/Character_Sprites/Player_Sprites");
        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            if (!file.name.Contains("_"))
            {
                dropdowns[0].options.Add(new Dropdown.OptionData() { text = "Player_Sprites/" + file.name });
            }

        }
        dropdowns[0].value = 0;
        dropdowns[0].RefreshShownValue();
        dropdowns[0].onValueChanged.AddListener(delegate {
            Change_Sprite_Preview(dropdowns[0]);
        });
    }

    public void Setup_Stats()
    {
        GameObject stats = GameObject.FindGameObjectWithTag("Character_Editor_Stats");
        InputField[] fields = stats.GetComponentsInChildren<InputField>();
        //Level
        fields[0].text = "1";
        //Strength
        fields[1].text = "5";
        //Dexterity
        fields[2].text = "5";
        //Spirit
        fields[3].text = "4";
        //Initiative
        fields[4].text = "10";
        //Vitality
        fields[5].text = "10";
        //Speed
        fields[6].text = "6";
        //Action Max
        fields[7].text = "2";
        //Reaction Max
        fields[8].text = "2";
        //Accuracy
        fields[9].text = "0";
        //Resistance
        fields[10].text = "0";
        //Lethality
        fields[11].text = "1.5";
        //Finesse
        fields[12].text = "1.5";
        //Weight
        fields[13].text = "0";
    }

    public void Setup_Equipment()
    {

    }

    public void Setup_Abilities()
    {

    }

    public void Change_Sprite_Preview(Dropdown drop)
    {

        Sprite sprite = Resources.LoadAll<Sprite>("Sprites/Character_Sprites/" + drop.options[drop.value].text)[0];
        GameObject[] previews = GameObject.FindGameObjectsWithTag("Preview");
        previews[0].GetComponent<Image>().sprite = sprite;
    }

    public void Create_Character()
    {
        //TODO fill this out
        Dropdown[] dropdowns = gameObject.GetComponentsInChildren<Dropdown>();
        InputField[] stats = gameObject.GetComponentsInChildren<InputField>();
        int i = 0;
        foreach (InputField field in stats)
        {
            Debug.Log( i + " " + field.text);
            i++;
        }
        string new_name = stats[0].text;
        string new_background = stats[1].text;
        string new_spritesheet = dropdowns[0].options[dropdowns[0].value].text;
        string new_level = stats[2].text;
        string new_strength = stats[3].text;
        string new_dexterity = stats[4].text;
        string new_spirit = stats[5].text;
        string new_initiative = stats[6].text;
        string new_vitality = stats[7].text;
        string new_speed = stats[8].text;
        string new_action_max = stats[9].text;
        string new_reaction_max = stats[10].text;
        string new_accuracy = stats[11].text;
        string new_resistance = stats[12].text;
        string new_lethality = stats[13].text;
        string new_finesse = stats[14].text;
        string new_weight = stats[15].text;

        Character_Script_Data data = new Character_Script_Data(new_name, 
            new_spritesheet, 
            new_background, 
            new_level, 
            new_action_max, 
            new_reaction_max, 
            new_strength, 
            new_dexterity, 
            new_spirit, 
            new_initiative, 
            new_vitality, 
            new_accuracy, 
            new_resistance, 
            new_lethality, 
            new_finesse, 
            new_speed, 
            new_weight);

        string output = JsonUtility.ToJson(data, true);
        Debug.Log(output);
        string path = "Assets/Resources/Characters/Arcade_Characters/" + new_name + ".txt";
        StreamWriter writer = new StreamWriter(path, false);
        writer.Write(output);
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
    }
}
