using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Error_Editor_UI : MonoBehaviour {

    private static int WIDTH = 560;
    private static int HEIGHT = 200;
    private Text message;
    public delegate void InputDelegate(bool b);
    private InputDelegate del;

    /// <summary>
    /// Enable the menu
    /// </summary>
    /// <param name="input">The string to use as the error message.</param>
    public void Enable(string input)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        message.text = input;
        del = null;
    }

    /// <summary>
    /// Enable the menu
    /// </summary>
    /// <param name="input">The string to use as the error message.</param>
    public void Enable(string input, InputDelegate new_del)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        gameObject.SetActive(true);
        message.text = input;
        del = new_del;
    }

    /// <summary>
    /// Disable the menu and execute the delegate method with the correct parameter.
    /// </summary>
    /// <param name="b">True or false will be set by the Ok and Cancel buttons.</param>
    public void Disable(bool b)
    {
        message.text = "";
        if(del != null)
        {
            del(b);
            del = null;
        }
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Disable the menu
    /// </summary>
    public void Disable()
    {
        Disable(false);
    }

    // Use this for initialization
    void Start () {
        message = gameObject.GetComponentsInChildren<Text>()[1];

        Button[] buttons = gameObject.GetComponentsInChildren<Button>();
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => { Disable(true); });
        buttons[0].onClick.RemoveAllListeners();
        buttons[0].onClick.AddListener(() => { Disable(false); });

        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(WIDTH, HEIGHT);

        //Needs to be active on start to be found by game controller
        //Disable();
	}
}
