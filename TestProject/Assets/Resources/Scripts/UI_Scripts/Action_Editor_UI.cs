using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Action_Editor_UI : MonoBehaviour {

    public GameObject effect_creator;
    public GameObject check_creator;
    public GameObject area_creator;

    private Character_Action action;

    public void Add_Area(InputField field)
    {
        area_creator.GetComponent<Area_Editor_UI>().Enable(field);
    }

    public void Add_Effect(Dropdown drop)
    {
        effect_creator.GetComponent<Effect_Editor_UI>().Enable(drop);
    }

    public void Edit_Effect(Dropdown drop)
    {
        effect_creator.GetComponent<Effect_Editor_UI>().Enable(drop, drop.options[drop.value].text);
    }

    public void Add_Check(Dropdown drop)
    {
        check_creator.GetComponent<Check_Editor_UI>().Enable(drop);
    }

    public void Remove_Area(InputField field)
    {
        field.text = "";
    }

    public void Remove_Effect(Dropdown drop)
    {
        drop.options.RemoveAt(drop.value);
        drop.RefreshShownValue();
    }

    public void Remove_Check(Dropdown drop)
    {
        drop.options.RemoveAt(drop.value);
        drop.RefreshShownValue();
    }

    public void Save()
    {
        Disable();
    }

    public void Enable()
    {
        transform.localPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

	// Use this for initialization
	void Start () {
        Disable();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /// <summary>
    /// Updates the action with the relevant fields.
    /// </summary>
    public void Update_Action()
    {

    }
}
