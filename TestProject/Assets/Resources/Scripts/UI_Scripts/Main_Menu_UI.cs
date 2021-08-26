using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Main_Menu_UI : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Setup_Listeners();
        Setup_Buttons();
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
    /// Sets up button listeners for the main menu
    /// </summary>
    public void Setup_Buttons()
    {
        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        buttons[0].onClick.AddListener(() => { Debug.Log("Story"); });
        buttons[1].onClick.AddListener(() => { Game_Controller.controller.Load_Scene(Scenes.Arcade); });
        buttons[2].onClick.AddListener(() => { Game_Controller.controller.Load_Scene(Scenes.Editor); });
        buttons[3].onClick.AddListener(() => { Debug.Log("Options"); });
        buttons[4].onClick.AddListener(() => { Game_Controller.controller.Print_Credits(); });
    }

	// Update is called once per frame
	void Update () {
	
	}
}
