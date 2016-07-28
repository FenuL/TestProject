using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System;

public class Action_Menu_Script : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    public RectTransform container;
    public Game_Controller controller { get; set; }
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
        int buttons = 0;
        int x = 0;
        foreach (Character_Script.Actions a in controller.curr_player.GetComponent<Character_Script>().actions)
        {
            Transform button = container.GetComponent<RectTransform>().GetChild(x);
            button.localScale = new Vector3(1, 1, 1);
            button.GetComponent<Image>().color = Color.white;
            string text="";
            if ((int)a != 0)
            {
                if ((int)a > 0)
                {
                    text = a.ToString() + ": -" + (int)a;
                }
                else
                {
                    text = a.ToString() + ": -x";
                }
            }else
            {
                text = a.ToString() + ": +10";
            }
            button.name = text;
            button.FindChild("Text").GetComponent<Text>().text = text;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            if (text.Contains(Character_Script.Actions.Move.ToString()))
            {
                if ((int)a < controller.curr_player.GetComponent<Character_Script>().action_curr)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Move); });
                }
                else
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                
            }
            if (text.Contains(Character_Script.Actions.Attack.ToString()))
            {
                if ((int)a < controller.curr_player.GetComponent<Character_Script>().action_curr)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Attack); });// button.GetComponent<Button>().onClick = controller.curr_player.GetComponent<Character_Script>().Action("Attack");
                }
                else
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                
            }
            if (text.Contains(Character_Script.Actions.Wait.ToString()))
            {
                if ((int)a < controller.curr_player.GetComponent<Character_Script>().action_curr)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Wait); }); //; button.GetComponent<Button>().onClick = controller.NextPlayer();
                }
                else
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                
            }
            if (text.Contains(Character_Script.Actions.Blink.ToString()))
            {
                if ((int)a < controller.curr_player.GetComponent<Character_Script>().action_curr)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Blink); }); //; button.GetComponent<Button>().onClick = controller.NextPlayer();
                }
                else
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                
            }
            if (text.Contains(Character_Script.Actions.Channel.ToString()))
            {
                if ((int)a < controller.curr_player.GetComponent<Character_Script>().action_curr)
                {
                    button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Channel); });// button.GetComponent<Button>().onClick = controller.NextPlayer();
                }
                else
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                
            }
            buttons = x + 1;
            x++;
        }
        /*for (int x = 0; x < controller.curr_player.GetComponent<Character_Script>().actions.Length; x++)
        {
            Transform button = container.GetComponent<RectTransform>().GetChild(x);
            string text = controller.curr_player.GetComponent<Character_Script>().actions[x];
            button.name = text;
            button.FindChild("Text").GetComponent<Text>().text= text;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            if (text == Character_Script.Actions.Move.ToString())
            {
                button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Move); });
            }
            if (text == Character_Script.Actions.Attack.ToString())
            {
                button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action(Character_Script.Actions.Attack); });// button.GetComponent<Button>().onClick = controller.curr_player.GetComponent<Character_Script>().Action("Attack");
            }
            if (text == Character_Script.Actions.Wait.ToString())
            {
                button.GetComponent<Button>().onClick.AddListener(() => { controller.NextPlayer(); }); //; button.GetComponent<Button>().onClick = controller.NextPlayer();
            }
            buttons = x+1;
        }*/
        //make other buttons invisible;
        if (buttons < container.GetComponent<RectTransform>().childCount)
        {
            for(int y = buttons; y < container.GetComponent<RectTransform>().childCount; y++)
            {
                Transform button = container.GetComponent<RectTransform>().GetChild(y);
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                button.localScale = new Vector3(0,0,0);
            }
        }
    }

}
