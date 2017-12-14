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
    /// </summary>
    public RectTransform container;
    public List<Transform> buttons;
    public Game_Controller controller { get; private set; }
    public bool isOpen { get; private set; }

    /// <summary>
    /// Opens the Action Menu when the Mouse enters it. 
    /// </summary>
    /// <param name="eventData">The position of the Mouse.</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
    }

    /// <summary>
    /// Closes the Action Menu when the Mouse exits it. 
    /// </summary>
    /// <param name="eventData">The position of the Mouse</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
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
        controller = Game_Controller.controller;
        //container = transform.FindChild("Action Menu").GetComponent<RectTransform>();
        //resetActions();
        isOpen = false;
        this.GetComponent<RectTransform>().position = new Vector3(Screen.width/2,20,0);
	}

    /// <summary>
    /// Update is called once per frame.
    /// Checks if the menu is Open and adjusts the scale of the Menu accordingly. 
    /// </summary>
    void Update ()
    {
        //transform.position = new Vector3(controller.curr_player.transform.position.x, controller.curr_player.transform.position.y, controller.curr_player.transform.position.z);

        if (isOpen)
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
        double ap_cost;
        double mp_cost;
        //Center the action menu based on available actions
        container.GetComponent<RectTransform>().position = new Vector3(Screen.width/2+200 - 50f* controller.curr_scenario.curr_player.GetComponent<Character_Script>().actions.Count/2, this.GetComponent<RectTransform>().position.y+15, 0);

        foreach (Action a in controller.curr_scenario.curr_player.GetComponent<Character_Script>().actions)
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
            ap_cost = a.Convert_To_Double(a.ap_cost, controller.curr_scenario.curr_player.GetComponent<Character_Script>());
            mp_cost = a.Convert_To_Double(a.mp_cost, controller.curr_scenario.curr_player.GetComponent<Character_Script>());
            if (ap_cost > controller.curr_scenario.curr_player.GetComponent<Character_Script>().action_curr ||
                mp_cost > controller.curr_scenario.curr_player.GetComponent<Character_Script>().mana_curr ||
                !a.enabled)
            {
                button.GetComponent<Image>().color = Color.red;
            }else
            {
                int index = x;
                button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_scenario.curr_player.GetComponent<Character_Script>().actions[index].Select(controller.curr_scenario.curr_player.GetComponent<Character_Script>()); });
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

}
