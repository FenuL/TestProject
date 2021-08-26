using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class Arcade_Menu_UI : MonoBehaviour {

    private static string EDITOR_START_PREFAB_SRC = "Prefabs/UI_Prefabs/Arcade_Menu";
    private Dropdown scenario_selector;

    // Use this for initialization
    void Start()
    {
        Dropdown[] dropdowns = gameObject.GetComponentsInChildren<Dropdown>();
        scenario_selector = dropdowns[0];
        Setup_UI();

    }

    /// <summary>
    /// Sets up the correct listeners and delegate methods for the scenario inspector.
    /// </summary>
    public void Setup_UI()
    {
        //sets up the dropdowns correctly
        Setup_Dropdowns();

        //Sets up the buttons correctly
        Setup_Buttons();
    }

    /// <summary>
    /// Sets up the buttons correctly for the scenario inspector.
    /// </summary>
    public void Setup_Buttons()
    {
        //Sets up buttons for the Inspector
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        //Debug.Log("Found " + buttons.Length);
        buttons[0].onClick.AddListener(() =>
        {
            Game_Controller.controller.Set_Scenario(null);
            string path = "Assets/Resources/Scenarios/" + scenario_selector.options[scenario_selector.value].text + ".txt";
            //Read the text from directly from the test.txt file
            string text = File.ReadAllText(path);
            Scenario_Data data = JsonUtility.FromJson<Scenario_Data>(text);
            Game_Controller.controller.Set_Scenario_Data(data);
            Game_Controller.controller.Load_Scene(Scenes.Loadout);
        });
        buttons[1].onClick.AddListener(() =>
        {
            Game_Controller.controller.Set_Scenario(null);
            Game_Controller.controller.Load_Scene(Scenes.Main_Menu);
        });
    }

    public void Setup_Dropdowns()
    {
        scenario_selector.options.Add(new Dropdown.OptionData() { text = "Select Scenario" });

        //Add existing files to the dropdown box
        //TODO: Make this better, generate a text item at build time.
        Object[] files = Resources.LoadAll("Scenarios");

        foreach (Object file in files)
        {
            // file extension check
            //Debug.Log("Found filename " + file.name);
            scenario_selector.options.Add(new Dropdown.OptionData() { text = file.name });

            if (file.name == ".ext")
            {
            }

        }
        scenario_selector.value = 0;
        scenario_selector.RefreshShownValue();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
