using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Script for handling the Action Menu for Characters.
/// </summary>
public class Action_Menu_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    /// <summary>
    /// RectTransform container - The Transform containing the Action Menu
    /// Game_Controller controller - The Game Controller with the information about the game. 
    /// List<Transform> buttons - The list of buttons in the Action Menu
    /// bool isOpen - If the Action Menu is open or not. 
    /// bool toggle_open - if the Action menu is toggled open or not.
    /// </summary>
    public RectTransform container;
    public Text text;
    public List<Transform> buttons;
    public bool is_open { get; private set; }
    public bool toggle_open { get; private set; }

    /// <summary>
    /// Opens the Action Menu when the Mouse enters it. 
    /// </summary>
    /// <param name="eventData">The position of the Mouse.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!toggle_open) { 
            is_open = true;
        }
    }

    /// <summary>
    /// Closes the Action Menu when the Mouse exits it. 
    /// </summary>
    /// <param name="eventData">The position of the Mouse</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!toggle_open)
        {
            is_open = false;
        }
    }

    /// <summary>
    /// Used by the Game Controller to Start the Action Menu early.
    /// </summary>
    public void Initialize()
    {
        Start();
    }

    // Use this for initialization
    void Start () {
        //container = transform.FindChild("Action Menu").GetComponent<RectTransform>();
        //resetActions();
        is_open = false;
        toggle_open = false;
        this.GetComponent<RectTransform>().position = new Vector3(Screen.width/2,20,0);
	}

    /// <summary>
    /// Update is called once per frame.
    /// Checks if the menu is Open and adjusts the scale of the Menu accordingly. 
    /// </summary>
    void Update ()
    {
        //transform.position = new Vector3(controller.curr_player.transform.position.x, controller.curr_player.transform.position.y, controller.curr_player.transform.position.z);

        if (is_open)
        {
            Vector3 scale = container.localScale;
            scale.y = Mathf.Lerp(scale.y, 1, Time.deltaTime*12);
            container.localScale = scale;
        }
        else
        {
            Vector3 scale = container.localScale;
            scale.y = Mathf.Lerp(scale.y, 0, Time.deltaTime * 12);
            container.localScale = scale;
        }
    }

    /// <summary>
    /// Resets the Action Menu. 
    /// Called when the player switches and we need to load a new set of Actions in the menu.
    /// </summary>
    public void resetActions()
    {
        int button_num  = 0;
        int x = 0;
        buttons = new List<Transform>();
        Transform button;
        //Center the action menu based on available actions
        //container.GetComponent<RectTransform>().position = new Vector3(Screen.width/2+200 - 50f* controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().actions.Count/2, this.GetComponent<RectTransform>().position.y+15, 0);
        container.GetComponent<RectTransform>().position = new Vector3(Screen.width / 2 + 200 - 50f * Game_Controller.Get_Curr_Scenario().Get_Curr_Character().Get_Curr_Actions().Count / 2, this.GetComponent<RectTransform>().position.y + 15, 0);
        //foreach (Character_Action a in controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().actions)
        foreach (Character_Action a in Game_Controller.Get_Curr_Scenario().Get_Curr_Character().Get_Curr_Actions())
        {
            button = container.GetComponent<RectTransform>().GetChild(x);
            buttons.Add(button);
            button.localScale = new Vector3(1, 1, 1);
            button.GetComponent<Image>().color = Color.white;
            //TODO FIX THIS SHIT
            //set the button name and text correctly
            button.name = a.name;
            button.FindChild("Text").GetComponent<Text>().text =  "\n"+a.name + "\n"+ (button_num + 1)+ ".";
            button.GetComponent<Button>().onClick.RemoveAllListeners();

            //check if the cost of the action is too high
            if (a.Check_Resource() ||
                !a.enabled || 
                a.activation != Character_Action.Activation_Types.Active)
            {
                button.GetComponent<Image>().color = Color.red;
            }else
            {
                int index = x;

                //button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_scenario.curr_player.Peek().GetComponent<Character_Script>().actions[index].Select(); });
                button.GetComponent<Button>().onClick.AddListener(() => { Game_Controller.Get_Curr_Scenario().Get_Curr_Character().actions[index].Select(); });
            }
            button_num = x + 1;
            x++;
        }
        
        //make other buttons invisible;
        if (button_num < container.GetComponent<RectTransform>().childCount)
        {
            for(int y = button_num; y < container.GetComponent<RectTransform>().childCount; y++)
            {
                button = container.GetComponent<RectTransform>().GetChild(y);
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                button.localScale = new Vector3(0,0,0);
            }
        }
    }

    /// <summary>
    /// Toggles the action menu to keep it open. Called when the Action Menu is clicked.
    /// </summary>
    public void Toggle()
    {
        if (toggle_open)
        {
            toggle_open = false;
            is_open = false;
        }
        else
        {
            toggle_open = true;
            is_open = true;
        }
    }

    /// <summary>
    /// Sets the text description for the Action Menu
    /// </summary>
    /// <param name="text"></param>
    public void Set_Text(string new_text)
    {
        text.text = "Action Menu - " + new_text;
    }
}
