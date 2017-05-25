using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class Action_Menu_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public RectTransform container;
    public Game_Controller controller { get; set; }
    public List<Transform> buttons;
    public bool isOpen;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isOpen = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isOpen = false;
    }

    // Use this for initialization
    void Start () {
        controller = Game_Controller.controller;
        //container = transform.FindChild("Action Menu").GetComponent<RectTransform>();
        //resetActions();
        isOpen = false;
	}
	
	// Update is called once per frame
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

    public void resetActions()
    {
        int button_num  = 0;
        int x = 0;
        buttons = new List<Transform>();
        Transform button;
        double ap_cost;
        double mp_cost;
        foreach (Character_Script.Action a in controller.curr_scenario.curr_player.GetComponent<Character_Script>().actions)
        {
            button = container.GetComponent<RectTransform>().GetChild(x);
            buttons.Add(button);
            button.localScale = new Vector3(1, 1, 1);
            button.GetComponent<Image>().color = Color.white;
            //TODO FIX THIS SHIT
            //set the button name and text correctly
            button.name = a.name;
            button.FindChild("Text").GetComponent<Text>().text = a.name;
            button.GetComponent<Button>().onClick.RemoveAllListeners();

            //check if the cost of the action is too high
            ap_cost = a.Convert_To_Double(a.ap_cost, controller.curr_scenario.curr_player.GetComponent<Character_Script>());
            mp_cost = a.Convert_To_Double(a.mp_cost, controller.curr_scenario.curr_player.GetComponent<Character_Script>());
            if (ap_cost > controller.curr_scenario.curr_player.GetComponent<Character_Script>().action_curr ||
                mp_cost > controller.curr_scenario.curr_player.GetComponent<Character_Script>().mana_curr)
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
