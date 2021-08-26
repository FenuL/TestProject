using UnityEngine;
using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[Serializable]
public class UI_Controller : MonoBehaviour {

    private static string MAIN_MENU_PREFAB_SRC = "Prefabs/UI_Prefabs/Main_Menu";
    private static string EDITOR_INSPECTOR_PREFAB_SRC = "Prefabs/UI_Prefabs/Inspector";
    private static string ARCADE_MENU_PREFAB_SRC = "Prefabs/UI_Prefabs/Arcade_Menu";
    private static string LOADOUT_MENU_PREFAB_SRC = "Prefabs/UI_Prefabs/Loadout_Menu";

    static UI_Controller controller;
    private GameObject main_menu;
    private GameObject editor_inspector;
    private GameObject arcade_menu;
    private GameObject loadout_menu;

    public static UI_Controller Get_Controller()
    {
        return controller;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Scene currentScene = SceneManager.GetActiveScene();

        // Retrieve the name of this scene.
        string scene_name = currentScene.name;
        if (scene_name == Scenes.Main_Menu.ToString())
        {
            Main_Menu();
        }
        else if (scene_name == Scenes.Editor.ToString())
        {
            Editor();
        }
        else if (scene_name == Scenes.Arcade.ToString())
        {
            Arcade();
        }
        else if (scene_name == Scenes.Loadout.ToString())
        {
            Loadout();
        }
    }

    /// <summary>
    /// Cleans the canvas of any child game objects
    /// </summary>
    public void Clean_Canvas()
    {
        
        main_menu = null;
        editor_inspector = null;
        arcade_menu = null;
        loadout_menu = null;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        //Debug.Log("Cleared");

    }

    /// <summary>
    /// Attempts to find a GameObject via tag and then loads it from a prefab if not found.
    /// </summary>
    /// <param name="tag">The tag to use to check if the GameObject already exists.</param>
    /// <param name="prefab">The prefab to load if the game object does not already exist.</param>
    /// <returns>The prefab that it found.</returns>
    public GameObject Load_From_Prefab(string tag, string prefab)
    {
        GameObject obj = GameObject.FindGameObjectWithTag(tag);
        //Debug.Log("tag " + tag);
        if (obj == null)
        {
            Debug.Log("tag not found");
            GameObject obj_prefab = (GameObject)Resources.Load(prefab);
            obj = (GameObject)Instantiate(obj_prefab);
            //Has to be done this way to preserve scaling
            obj.transform.SetParent(gameObject.transform, false);
        }
        return obj;
    }

    /// <summary>
    /// What to do in the Editor Scene
    /// </summary>
    public void Editor()
    {
        if(editor_inspector == null)
        {
            //Debug.Log("Called");
            editor_inspector = Load_From_Prefab("Editor_Inspector", EDITOR_INSPECTOR_PREFAB_SRC);
        }
    }

    /// <summary>
    /// What to do in the Main_Menu Scene
    /// </summary>
    public void Main_Menu()
    {
        if (main_menu == null)
        {
            main_menu = Load_From_Prefab("Main_Menu", MAIN_MENU_PREFAB_SRC);
        }
    }

    /// <summary>
    /// What to do in the Main_Menu Scene
    /// </summary>
    public void Arcade()
    {
        if (arcade_menu == null)
        {
            arcade_menu = Load_From_Prefab("Arcade_Menu", ARCADE_MENU_PREFAB_SRC);
        }
    }

    /// <summary>
    /// What to do in the Loadout Scene
    /// </summary>
    public void Loadout()
    {
        if (loadout_menu == null)
        {
            loadout_menu = Load_From_Prefab("Loadout_Menu", LOADOUT_MENU_PREFAB_SRC);
        }
    }

    /// <summary>
    /// Starts the UI for a given scene.
    /// </summary>
    /// <param name="scene">The scene whose UI to start.</param>
    /*public void Start(Scenes scene)
    {
        Debug.Log("started ui for " + scene.ToString());
        if (scene == Scenes.Main_Menu)
        {
            if (main_menu == null)
            {
                main_menu = Load_From_Prefab("Main_Menu", MAIN_MENU_PREFAB_SRC);
            }
        }
        else if (scene == Scenes.Editor)
        {
            if (editor_inspector == null)
            {
                editor_inspector = Load_From_Prefab("Editor_Inspector", EDITOR_INSPECTOR_PREFAB_SRC);
            }
        }else if (scene == Scenes.Arcade)
        {
            if (arcade_menu == null)
            {
                arcade_menu = Load_From_Prefab("Arcade_Menu", ARCADE_MENU_PREFAB_SRC);
            }
        }
    }*/

    /// <summary>
    /// Reduce the scale of an object to 0.
    /// </summary>
    /// <param name="obj">The GameObject to minimize.</param>
    public static void Minimize(GameObject obj)
    {
        obj.transform.localScale = new Vector3(0,0,0);
    }

    /// <summary>
    /// Return the scale of an object to 1.
    /// </summary>
    /// <param name="obj">The GameObject to maximize.</param>
    public static void Maximize(GameObject obj)
    {
        obj.transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Ran before Start method. Eliminates any superfluous copies of the UI_Controller. THERE CAN ONLY BE ONE! 
    /// </summary>
    void Awake()
    {
        if (controller == null)
        {
            //DontDestroyOnLoad(gameObject);
            controller = this;
        }
        else if (controller != this)
        {
            Destroy(gameObject);
        }
    }
}
