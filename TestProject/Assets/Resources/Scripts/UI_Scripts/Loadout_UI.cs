using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Loadout_UI : MonoBehaviour {

    private static string LOADOUT_MENU_PREFAB_SRC = "Prefabs/UI_Prefabs/Loadout_Menu";
    private static string CHAR_CREATOR_MENU_PREFAB_SRC = "Prefabs/UI_Prefabs/Character_Creator";
    private int arcade_characters_unlocked = 5;

    // Use this for initialization
    void Start () {
        Setup_Listeners();

        Setup_Scenario_Details(Game_Controller.controller.Get_Curr_Scenario_Data());

        Setup_Character_Details();

        Setup_Buttons();
	}
	
	// Update is called once per frame
	void Update () {
	
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
    /// Sets up the Scenario Details Menu
    /// </summary>
    /// <param name="data">The data to use to fill in the various text fields.</param>
    public void Setup_Scenario_Details(Scenario_Data data)
    {
        Text[] text = gameObject.GetComponentsInChildren<Text>();
        text[3].text = data.scenario_name;
        text[5].text = data.objective.ToString();
        text[7].text = "Short";
        text[9].text = data.description;
        string rewards = "";
        foreach (string s in data.rewards)
        {
            rewards += s + ",";
        }
        text[11].text = rewards;

        Button[] buttons = gameObject.GetComponentsInChildren<Button>();

        if (data.prev_scenario == null)
        {
            buttons[2].transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            buttons[2].transform.localScale = new Vector3(1, 1, 1);
            buttons[2].onClick.AddListener(() => {
                string file_data = File.ReadAllText(data.prev_scenario);
                Scenario_Data new_data = JsonUtility.FromJson<Scenario_Data>(file_data);
                Setup_Scenario_Details(new_data);
            });
        }
        if (data.next_scenario == null)
        {
            buttons[3].transform.localScale = new Vector3(0, 0, 0);
        }
        else
        {
            buttons[3].transform.localScale = new Vector3(1, 1, 1);
            buttons[3].onClick.AddListener(() => {
                string file_data = File.ReadAllText(data.next_scenario);
                Scenario_Data new_data = JsonUtility.FromJson<Scenario_Data>(file_data);
                Setup_Scenario_Details(new_data);
            });
        }
    }

    /// <summary>
    /// Sets up the Character_Details Menus
    /// </summary>
    public void Setup_Character_Details()
    {
        Dropdown[] dropdowns = gameObject.GetComponentsInChildren<Dropdown>();

        dropdowns[0].options.Add(new Dropdown.OptionData() { text = "Select Character" });
        dropdowns[1].options.Add(new Dropdown.OptionData() { text = "Select Character" });
        dropdowns[2].options.Add(new Dropdown.OptionData() { text = "Select Character" });
        dropdowns[3].options.Add(new Dropdown.OptionData() { text = "Select Character" });
        dropdowns[4].options.Add(new Dropdown.OptionData() { text = "Select Character" });
        dropdowns[5].options.Add(new Dropdown.OptionData() { text = "Select Character" });

        //Add existing files to the dropdown box
        //TODO: Make this better, generate a text item at build time.
        Object[] files = Resources.LoadAll("Characters/Arcade_Characters");
        //Debug.Log(files.Length);
        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            dropdowns[0].options.Add(new Dropdown.OptionData() { text = file.name });
            dropdowns[1].options.Add(new Dropdown.OptionData() { text = file.name });
            dropdowns[2].options.Add(new Dropdown.OptionData() { text = file.name });
            dropdowns[3].options.Add(new Dropdown.OptionData() { text = file.name });
            dropdowns[4].options.Add(new Dropdown.OptionData() { text = file.name });
            dropdowns[5].options.Add(new Dropdown.OptionData() { text = file.name });
        }

        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        buttons[4].onClick.AddListener(() => {
            GameObject obj = (GameObject)Instantiate(Resources.Load(CHAR_CREATOR_MENU_PREFAB_SRC));
            obj.transform.parent = GameObject.FindGameObjectWithTag("Main_Canvas").transform;
        });
        buttons[5].onClick.AddListener(() => {
            GameObject obj = (GameObject)Instantiate(Resources.Load(CHAR_CREATOR_MENU_PREFAB_SRC));
            obj.transform.parent = GameObject.FindGameObjectWithTag("Main_Canvas").transform;
        });
        buttons[6].onClick.AddListener(() => {
            GameObject obj = (GameObject)Instantiate(Resources.Load(CHAR_CREATOR_MENU_PREFAB_SRC));
            obj.transform.parent = GameObject.FindGameObjectWithTag("Main_Canvas").transform;
        });
        buttons[7].onClick.AddListener(() => {
            GameObject obj = (GameObject)Instantiate(Resources.Load(CHAR_CREATOR_MENU_PREFAB_SRC));
            obj.transform.parent = GameObject.FindGameObjectWithTag("Main_Canvas").transform;
        });
        buttons[8].onClick.AddListener(() => {
            GameObject obj = (GameObject)Instantiate(Resources.Load(CHAR_CREATOR_MENU_PREFAB_SRC));
            obj.transform.parent = GameObject.FindGameObjectWithTag("Main_Canvas").transform;
        });
        buttons[9].onClick.AddListener(() => {
            GameObject obj = (GameObject)Instantiate(Resources.Load(CHAR_CREATOR_MENU_PREFAB_SRC));
            obj.transform.parent = GameObject.FindGameObjectWithTag("Main_Canvas").transform;
        });
    }

    /// <summary>
    /// Sets up listeners for the menu's buttons.
    /// </summary>
    public void Setup_Buttons()
    {
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        //Set up the Begin button
        buttons[0].onClick.AddListener(() => {
            Game_Controller.controller.Load_Scene(Scenes.Active_Scenario);
        });
        //Set up the Back button
        buttons[1].onClick.AddListener(() => {
            Game_Controller.controller.Load_Prev_Scene();
        });
        
    }

}
