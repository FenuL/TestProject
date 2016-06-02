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
	void Update () {
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
        for (int x = 0; x < controller.curr_player.GetComponent<Character_Script>().actions.Length; x++)
        {
            Transform button = container.GetComponent<RectTransform>().GetChild(x);
            string text = controller.curr_player.GetComponent<Character_Script>().actions[x];
            button.name = text;
            button.FindChild("Text").GetComponent<Text>().text= text;
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            if (text == "Move")
            {
                button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action("Move"); });
            }
            if (text == "Attack")
            {
                button.GetComponent<Button>().onClick.AddListener(() => { controller.curr_player.GetComponent<Character_Script>().Action("Attack"); });// button.GetComponent<Button>().onClick = controller.curr_player.GetComponent<Character_Script>().Action("Attack");
            }
            if (text == "Wait")
            {
                button.GetComponent<Button>().onClick.AddListener(() => { controller.NextPlayer(); }); //; button.GetComponent<Button>().onClick = controller.NextPlayer();
            }
            buttons = x+1;
        }
        //make other buttons invisible;
        if (buttons < container.GetComponent<RectTransform>().childCount)
        {
            for(int x = buttons; x < container.GetComponent<RectTransform>().childCount; x++)
            {
                Transform button = container.GetComponent<RectTransform>().GetChild(x);
                button.GetComponent<Button>().onClick.RemoveAllListeners();
                button.localScale = new Vector3(0,0,0);
            }
        }
    }

}
