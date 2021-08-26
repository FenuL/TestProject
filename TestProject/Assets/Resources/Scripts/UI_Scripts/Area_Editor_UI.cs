using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;

public class Area_Editor_UI : MonoBehaviour {

    private static int START_X = 35;
    private static int PREFAB_WIDTH = 85;
    private static int START_Y = -20;
    private static int PREFAB_HEIGHT = 35;
    private static string INPUT_PREFAB_SRC = "Prefabs/UI_Prefabs/Area_Input";
    [SerializeField] private List<FloatList> modifiers;
    private GameObject input_prefab;
    public GameObject content;
    GameObject[,] inputs;
    InputField output;
    InputField width;
    InputField length;

    public void Setup_Grid()
    {
        int w = 0;
        int l = 0;
        int.TryParse(width.text, out w);
        int.TryParse(length.text, out l);

        //clear out old list
        foreach(GameObject obj in inputs)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        inputs = new GameObject[w, l];

        for (int i =0; i< w; i++)
        {
            for (int j =0; j< l; j++)
            {
                GameObject obj = ((GameObject)Instantiate(input_prefab,
                    new Vector3(0,0,0),
                    Quaternion.identity));
                obj.transform.SetParent(content.transform);
                obj.transform.position = new Vector3((float)(START_X + PREFAB_WIDTH * j + content.transform.position.x),
                        (float)(START_Y - PREFAB_HEIGHT * i + content.transform.position.y),
                        (float)(0));
                inputs[i, j] = obj;
                obj.GetComponent<Dropdown>().value=2;
            }
        }
    }

    /// <summary>
    /// Create the value and disable the Menu
    /// </summary>
    public void Create()
    {
        int w = 0;
        int l = 0;
        int.TryParse(width.text, out w);
        int.TryParse(length.text, out l);
        Dictionary<int, float> conversion = new Dictionary<int, float>();
        conversion[0] = -0.5f;
        conversion[1] = -0.25f;
        conversion[2] = 0.0f;
        conversion[3] = 0.25f;
        conversion[4] = 0.5f;

        float[,] modifiers = new float[w,l];

        for(int x =0; x< inputs.GetLength(0); x++)
        {
            for(int y=0; y< inputs.GetLength(1); y++)
            {
                modifiers[x,y]=conversion[inputs[x, y].GetComponent<Dropdown>().value];
            }
        }
        
        output.text = JsonConvert.SerializeObject(modifiers);
        Disable();
    }

    /// <summary>
    /// Enable the menu.
    /// </summary>
    /// <param name="drop_out">The output InputField box to put the output.</param>
    public void Enable(InputField drop_out)
    {
        transform.localPosition = new Vector3(0, 0, 0);
        output = drop_out;
        width.text = "3";
        length.text = "3";
        Setup_Grid();
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disable the menu
    /// </summary>
    public void Disable()
    {
        gameObject.SetActive(false);
    }

    // Use this for initialization
    void Start () {
        input_prefab = Resources.Load(INPUT_PREFAB_SRC, typeof(GameObject)) as GameObject;

        InputField[] fields = GetComponentsInChildren<InputField>();
        width = fields[0];
        length = fields[1];

        inputs = new GameObject[3, 3];
        width.text = "3";
        length.text = "3";
        Setup_Grid();

        Disable();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
