using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Serialization;

/// <summary>
/// Game Controller class. Handles overarching Game processes. 
/// </summary>
[Serializable]
public class Game_Controller : MonoBehaviour {
    /// <summary>
    /// string STARTING_SCENARIO - the Scenario to load at the start of the game.
    /// Game_Controller controller - Controller. Makes sure there is only one copy of this.
    /// GameObject canvas - The Canvas to draw the UI on.
    /// Scenario curr_scenario - The current Scenario that is Loaded.
    /// ArrayList avail_Scenarios - The Array of currently available Scenarios.
    /// GameObject main_camera - The Main camera object.
    /// List<Character_Actions> all_actions - The list of all possible Character_Actions created by parsing the Character_Action List File. 
    /// Transform action_menu - the Action Menu used to select different Character Actions. 
    /// </summary>
    static string STARTING_SCENARIO = "Assets/Resources/Maps/tile_map.txt";
    static string ERROR_PREFAB_SRC = "Prefabs/UI_Prefabs/Error_Editor";
    public enum Comparison_Operators { EQ, GT, LT, GTQ, LTQ }
    //public static string STARTING_SCENARIO = "Assets/Resources/Maps/falls_map.txt";

    public static Game_Controller controller { get; private set; }
    public static bool wait_for_input { get; private set; }
    public static bool overwrite_file { get; private set; }
    Editor_UI editor;
    Editor_Controller editor_controller;
    static FloatingText popup;
    static GameObject canvas;
    static Scenario curr_scenario;
    Scenario_Data curr_scenario_data;
    ArrayList avail_scenarios;
    GameObject main_camera;
    Dictionary<string, Character_Action> all_actions;
    Dictionary<string, Weapon> all_weapons;
    Dictionary<string, Armor> all_armors;
    static Dictionary<Accepted_Float_Shortcuts, string> stat_descriptions;
    static Dictionary<Accepted_Bool_Shortcuts, string> bool_descriptions;
    static Dictionary<Condition_Shortcuts, string> condi_descriptions;
    Transform action_menu;
    Dictionary<Controlls, KeyCode[]> controlls;
    string prev_scene;
    Character_Script_Data[] loadout_data;
    bool mouse_over_ui;
    static GameObject error_ui;

    public static FloatingText Get_Popup()
    {
        return popup;
    }
    public static GameObject Get_Canvas()
    {
        return canvas;
    }
    public static Scenario Get_Curr_Scenario()
    {
        return curr_scenario;
    }
    public Scenario_Data Get_Curr_Scenario_Data()
    {
        return curr_scenario_data;
    }
    public ArrayList Get_Avail_Scenarios()
    {
        return avail_scenarios;
    }
    public GameObject Get_Main_Camera()
    {
        return main_camera;
    }
    public Dictionary<string, Character_Action> Get_All_Actions()
    {
        return all_actions;
    }
    public Dictionary<string, Weapon> Get_All_Weapons()
    {
        return all_weapons;
    }
    public Dictionary<string, Armor> Get_All_Armors()
    {
        return all_armors;
    }
    public Transform Get_Action_Menu()
    {
        return action_menu;
    }
    public Dictionary<Controlls, KeyCode[]> Get_Controlls()
    {
        return controlls;
    }
    public Character_Script_Data[] Get_Loadout_Data()
    {
        return loadout_data;
    }
    public static Dictionary<Accepted_Float_Shortcuts, string> Get_Stat_Descriptions()
    {
        return stat_descriptions;
    }
    public static Dictionary<Accepted_Bool_Shortcuts, string> Get_Bool_Descriptions()
    {
        return bool_descriptions;
    }
    public static Dictionary<Condition_Shortcuts, string> Get_Condition_Descriptions()
    {
        return condi_descriptions;
    }
    public void Set_Loadout_Data(Character_Script_Data[] data)
    {
        loadout_data = data;
    }
    public void Set_Scenario(Scenario script)
    {
        curr_scenario = script;
    }
    public void Set_Scenario_Data(Scenario_Data data)
    {
        curr_scenario_data = data;
    }
    public void Set_Editor(Editor_UI edit)
    {
        editor = edit;
    }
    public void Set_Editor_Controller(Editor_Controller control)
    {
        editor_controller = control;
    }
    public void Print_Credits()
    {
        Debug.Log("Lead Programmer: Luciano Fenu");
        Debug.Log("Lead Writer/Composer: Nathan Keith");
        Debug.Log("Lead Artist: Patrick Holloway");
    }

    public void Delete_Obj(GameObject obj)
    {
        GameObject.Destroy(obj);
    }

    /// <summary>
    /// Save the Current Game Status.
    /// TODO: UPDATE THIS TO CURRENT STANDARDS
    /// </summary>
	public void Save(){
		BinaryFormatter formatter = new BinaryFormatter ();
		FileStream file = File.Create (Application.persistentDataPath + "/game_data.dat");

		GameData data = new GameData();
		/*data.curr_map = scenariocurr_map;
		data.curr_character_num = curr_character_num;
		data.characters = characters;
		data.curr_player = curr_player;
		data.cursor = cursor;
		data.tile_grid = tile_grid;
		data.tile_data = tile_data;
		data.tiles = tiles;
		data.clicked_tile = clicked_tile;
		data.selected_tile = selected_tile;
		data.initialized = initialized;
		data.curr_round = curr_round;*/
		formatter.Serialize (file, data);
		file.Close ();

	}

    /// <summary>
    /// Load an existing game Save.
    /// TODO: UPDATE THIS TO CURRENT STANDARDS
    /// </summary>
	public void Load(){
		if (File.Exists (Application.persistentDataPath + "/game_data.dat")) {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream file = File.Open (Application.persistentDataPath + "/game_data.dat", FileMode.Open);
			GameData data = (GameData) formatter.Deserialize(file);
			/*curr_map = data.curr_map;
			curr_character_num = data.curr_character_num;
			characters = data.characters;
			curr_player = data.curr_player;
			cursor = data.cursor;
			tile_grid = data.tile_grid;
			tile_data = data.tile_data;
			tiles = data.tiles;
			clicked_tile = data.clicked_tile;
			selected_tile = data.selected_tile;
			initialized = data.initialized;
			curr_round = data.curr_round;*/
			formatter.Serialize (file, data);

			file.Close();
		}
	}

    /// <summary>
    /// Sets the Overwrite File flag to the input and stops waiting for input.
    /// </summary>
    /// <param name="input">True to overwrite the file, False to not.</param>
    private static void Overwrite_File(bool input)
    {
        wait_for_input = false;
        overwrite_file = input;
    }

    /// <summary>
    /// Serialize object data to a file using BSon
    /// </summary>
    /// <typeparam name="T">Type of the object to serialize</typeparam>
    /// <param name="obj">Object to serialize</param>
    /// <param name="path">Path of the file to store.</param>
    /// <param name="settings">Settings for the serializer (defaults to null)</param>
    public static IEnumerator Serialize_To_File<T>(T obj, string path, JsonSerializerSettings settings = null)
    {
        using (var mem_stream = new MemoryStream())
        using (var bson_writer = new BsonWriter(mem_stream)) // BsonDataWriter in Json.NET 10.0.1 and later
        {
            JsonSerializer.CreateDefault(settings).Serialize(bson_writer, obj);
            string data = Convert.ToBase64String(mem_stream.ToArray());
            char[] splitters = { '/' };
            TextAsset file = Resources.Load<TextAsset>(path.Split( splitters ,3)[2]);
            if (file != null)
            {
                wait_for_input = true;
                overwrite_file = false;
                Create_Error_Message("Warning! File: " + path + " already exists! Overwrite?", Overwrite_File);
                while (wait_for_input)
                {
                    yield return null;
                }
                if (overwrite_file)
                {
                    StreamWriter file_writer = new StreamWriter(path + ".txt", false);
                    file_writer.WriteLine(data);
                    file_writer.Close();
                    //Re-import the file to update the reference in the editor
                    AssetDatabase.ImportAsset(path+".txt");
                    AssetDatabase.Refresh();
                }
            }
            else
            {
                StreamWriter file_writer = new StreamWriter(path + ".txt", true);
                file_writer.WriteLine(data);
                file_writer.Close();
                //Re-import the file to update the reference in the editor
                AssetDatabase.ImportAsset(path+".txt");
                AssetDatabase.Refresh();
            }
        }
    }

    /// <summary>
    /// Deserialize a file written in BSON.
    /// </summary>
    /// <typeparam name="T">Type of object to deserialize.</typeparam>
    /// <param name="path">Path to the file to deserialize</param>
    /// <param name="settings">Settings for the deserializer. Defaults to null.</param>
    /// <returns>Object data.</returns>
    public static T Deserialize_From_File<T>(string path, JsonSerializerSettings settings = null)
    {
        TextAsset asset = Resources.Load<TextAsset>(path);
        //StreamReader file_reader = new StreamReader(path);
        //string contents = file_reader.ReadToEnd();
        //file_reader.Close();
        

        byte[] data = Convert.FromBase64String(asset.text);

        using (var stream = new MemoryStream(data))
        using (var reader = new BsonReader(stream)) // BsonDataReader in Json.NET 10.0.1 and later
        {
            var serializer = JsonSerializer.CreateDefault(settings);
            //https://www.newtonsoft.com/json/help/html/DeserializeFromBsonCollection.html
            if (serializer.ContractResolver.ResolveContract(typeof(T)) is JsonArrayContract)
                reader.ReadRootValueAsArray = true;
            return serializer.Deserialize<T>(reader);
        }
    }

    /// <summary>
    /// Searches a GameObject for a Character_Script.
    /// </summary>
    /// <param name="source_obj">The GameObject to search.</param>
    /// <returns>A Character_Script if it found one, null otherwise.</returns>
    public static Character_Script Get_Character_Script(GameObject source_obj)
    {
        Character_Script script = null;
        if (source_obj != null)
        {
            if (source_obj.GetComponent<Character_Script>())
            {
                script = source_obj.GetComponent<Character_Script>();
            }
            else if (source_obj.GetComponent<Tile>())
            {
                script = source_obj.GetComponent<Tile>().obj.GetComponent<Character_Script>();
            }
            else if (source_obj.GetComponent<Hazard>())
            {
                script = source_obj.GetComponent<Hazard>().owner;
            }
        }
        return script;
    }

    /// <summary>
    /// Searches a GameObject for a Tile script.
    /// </summary>
    /// <param name="source_obj">The object to search.</param>
    /// <returns>A Tile script if it found one, null otherwise.</returns>
    public static Tile Get_Tile_Script(GameObject source_obj)
    {
        Tile script = null;
        if (source_obj != null)
        {
            if (source_obj.GetComponent<Tile>())
            {
                script = source_obj.GetComponent<Tile>();
            }
            else if (source_obj.GetComponent<Character_Script>())
            {
                script = source_obj.GetComponent<Character_Script>().curr_tile;
            }
            else if (source_obj.GetComponent<Hazard>())
            {
                script = source_obj.GetComponent<Hazard>().current_tiles[0];
            }
        }
        return script;
    }

    /// <summary>
    /// Searches a GameObject for a Object_Script.
    /// </summary>
    /// <param name="source_obj">The GameObject to search</param>
    /// <returns>An Object_Script if it found one, null otherwise.</returns>
    public static Object_Script Get_Object_Script(GameObject source_obj)
    {
        Object_Script script = null;
        if (source_obj != null)
        {
            if (source_obj.GetComponent<Object_Script>())
            {
                script = source_obj.GetComponent<Object_Script>();
            }
            else if (source_obj.GetComponent<Tile>())
            {
                script = source_obj.GetComponent<Tile>().obj.GetComponent<Object_Script>();
            }
        }
        return script;
    }

    /// <summary>
    /// Searches a GameObject for a Hazard script.
    /// </summary>
    /// <param name="source_obj">The GameObject to search</param>
    /// <returns>An Hazard if it found one, null otherwise.</returns>
    public static Hazard Get_Hazard_Script(GameObject source_obj)
    {
        Hazard script = null;
        if (source_obj != null)
        {
            if (source_obj.GetComponent<Hazard>())
            {
                script = source_obj.GetComponent<Hazard>();
            }
            else if (source_obj.GetComponent<Tile>())
            {
                script = source_obj.GetComponent<Tile>().hazard.GetComponent<Hazard>();
            }
        }
        return script;
    }

    /// <summary>
    /// Converts the String parameters from the Character_Action into a float. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Float</param>
    /// <param name="source_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CC.</param>
    /// <param name="target_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CT.</param>
    /// <param name="targets">The list of curr_targets used to parse ACCEPTED_SHORTCUTS beginning with TN.</param>
    /// <param name="escape">The current parse attempt. Used to escape an infinite loop. </param>
    /// <returns>A float value of the equation given in the input</returns>
    public static float Convert_To_Float(string input, GameObject source_obj, GameObject target_obj, Target target, int escape)
    {
        //Debug.Log("Action: " + name + "; " + input);
        //Base case 
        if (escape > 5)
        {
            //Debug.Log(target_obj.name);
            Debug.Log("Could not parse " + input);
            return -10000;
        }
        Character_Script source= null;
        Tile source_tile = null;
        string prefix = "CC_";
        float output = 0;
        Target target_source = null;
        if (target != null)
        {
            target_source = target;
        }
        if (source_obj != null)
        {
            source = Get_Character_Script(source_obj);
            prefix = "CC_";
        }
        else if (input.Contains("TC_"))
        {
            if (target_obj != null)
            {
                source = Get_Character_Script(target_obj);
                prefix = "TC";
            }
        }
        else if (input.Contains("ST_"))
        {
            source_tile = Get_Tile_Script(source_obj);
            prefix = "ST_";
        }
        else if (input.Contains("TT_"))
        {
            if (target_obj != null)
            {
                source_tile = Get_Tile_Script(target_obj);
                prefix = "TT_";
            }
        }
        else if (input.Contains("TA_"))
        {
            prefix = "TA_";

            /*int start = input.IndexOf("TN_") + 3;
            int length = input.Substring(start).IndexOf("_");
            prefix = "TN_" + input.Substring(start, length) + "_";
            int target_index;
            if (int.TryParse(input.Substring(start, length), out target_index))
            {
                if (target_index < targets.Count)
                {
                    target_source = targets[target_index];
                }
            }
            if (target_source != null)
            {
                if (target_source.center.obj != null)
                {
                    if (target_source.center.obj.GetComponent<Character_Script>())
                    {
                        source = target_source.center.obj.GetComponent<Character_Script>();
                    }
                }
            }*/
        }

        if (input.Contains("CEIL"))
        {
            string[] start_string = input.Replace("CEIL", "").Split("[".ToCharArray(), 2);
            string[] end_string = start_string[1].Split("]".ToCharArray(), 2);
            return Mathf.Ceil(Convert_To_Float(end_string[0], source_obj, target_obj, target, escape + 1));
        }
        if (input.Contains("FLOOR"))
        {
            string[] start_string = input.Replace("FLOOR", "").Split("[".ToCharArray(), 2);
            string[] end_string = start_string[1].Split("]".ToCharArray(), 2);
            return Mathf.Floor(Convert_To_Float(end_string[0], source_obj, target_obj, target, escape + 1));
        }
        if (input.Contains("ABSL"))
        {
            string[] start_string = input.Replace("ABSL", "").Split("[".ToCharArray(), 2);
            string[] end_string = start_string[1].Split("]".ToCharArray(), 2);
            return Mathf.Abs(Convert_To_Float(end_string[0], source_obj, target_obj, target, escape + 1));
        }
        if (float.TryParse(input, out output))
        {
            return output;
        }
        else
        {
            //Remove acronyms from equation
            Array values = Enum.GetValues(typeof(Accepted_Float_Shortcuts));
            foreach (Accepted_Float_Shortcuts val in values)
            {
                //Debug.Log("Input " + input);
                if (source != null && !source.Equals(null))
                {
                    if (input.Contains(prefix))
                    {
                        if (input.Contains(val.ToString()))
                        {
                            if (val.ToString().Contains("AUM"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.aura_max);
                            }
                            else if (val.ToString().Contains("AUC"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.aura_curr);
                            }
                            else if (val.ToString().Contains("APM"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.action_max);
                            }
                            else if (val.ToString().Contains("APC"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.action_curr);
                            }
                            else if (val.ToString().Contains("MPM"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.mana_max);
                            }
                            else if (val.ToString().Contains("MPC"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.mana_curr);
                            }
                            /*else if (val.ToString().Contains("CAM"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.Get_Canister_Max());
                            }
                            else if (val.ToString().Contains("CAC"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.Get_Canister_Curr());
                            }*/
                            else if (val.ToString().Contains("SPD"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.speed);
                            }
                            else if (val.ToString().Contains("STR"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.strength);
                            }
                            else if (val.ToString().Contains("DEX"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.dexterity);
                            }
                            else if (val.ToString().Contains("SPT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.spirit);
                            }
                            else if (val.ToString().Contains("INI"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.initiative);
                            }
                            else if (val.ToString().Contains("VIT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.vitality);
                            }
                            else if (val.ToString().Contains("LVL"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.level);
                            }
                            else if (val.ToString().Contains("WPR"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.weapon.modifier.GetLength(0) / 2);
                            }
                            else if (val.ToString().Contains("WPD"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.weapon.damage);
                            }
                            else if (val.ToString().Contains("WPN"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.weapon.equip_name);
                            }
                            else if (val.ToString().Contains("ARM"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.armor.armor);
                            }
                            else if (val.ToString().Contains("WGT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.weight);
                            }
                            else if (val.ToString().Contains("MOC"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.action_curr);
                            }
                            else if (val.ToString().Contains("ORI"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.orientation);
                                //Debug.Log("Orientation: " + source.orientation);
                            }
                            else if (val.ToString().Contains("COO"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.camera_orientation_offset);
                                //Debug.Log("Camera offset: " + source.camera_orientation_offset);
                            }
                            else if (val.ToString().Contains("CMB"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Combo_Modifier]);
                            }
                            else if (val.ToString().Contains("DTT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Damage_Taken]);
                            }
                            else if (val.ToString().Contains("DDT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Damage_Dealt]);
                            }
                            else if (val.ToString().Contains("WTT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Wounds_Taken]);
                            }
                            else if (val.ToString().Contains("TMT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Tiles_Moved]);
                            }
                            else if (val.ToString().Contains("ATT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Actions_Taken]);
                            }
                            else if (val.ToString().Contains("RTT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Reactions_Taken]);
                            }
                            else if (val.ToString().Contains("KTT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Kills]);
                            }
                            else if (val.ToString().Contains("CDT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Conditions_Dealt]);
                            }
                            else if (val.ToString().Contains("CTT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.turn_stats[Character_Turn_Records.Conditions_Taken]);
                            }
                            else if (val.ToString().Contains("DST"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source.Get_Distance(source_tile));
                            }
                            else if (val.ToString().Contains("CST"))
                            {
                                if (target_source != null)
                                {
                                    input = input.Replace(prefix + val.ToString(), "" + target_source.curr_path_cost);
                                }
                                else
                                {
                                    input = input.Replace(prefix + val.ToString(), "" + 0);
                                }
                            }
                            else if (val.ToString().Contains("HGT"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source_tile.height);
                            }
                            else if (val.ToString().Contains("INX"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source_tile.index[0]);
                            }
                            else if (val.ToString().Contains("INY"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source_tile.index[1]);
                            }
                            else if (val.ToString().Contains("MOD"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + source_tile.modifier);
                            }
                            else if (val.ToString().Contains("NUL"))
                            {
                                input = input.Replace(prefix + val.ToString(), "" + 0.0);
                            }
                        }
                    }
                    else
                    {
                        //try to convert to double after converting
                        if (float.TryParse(input, out output))
                        {
                            return output;
                        }
                        else if (input.Contains("SC_") ||
                            input.Contains("TA_") ||
                            input.Contains("ST_") ||
                            input.Contains("TC_") ||
                            input.Contains("TT_"))
                        {
                            return Convert_To_Float(input, source_obj, target_obj, target, escape + 1);
                        }
                        else if (input.Contains("+") ||
                            input.Contains("/") ||
                            input.Contains("*") ||
                            input.Contains("-") ||
                            input.Contains("^") ||
                            input.Contains("(") ||
                            input.Contains(")"))
                        {
                            return Parse_Equation(input);
                        }
                    }
                }
            }
        }
        return Convert_To_Float(input, source_obj, target_obj, target, escape + 1);
    }

    /// <summary>
    /// Converts the String parameters from the Character_Action into a float. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Double</param>
    /// <param name="source_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CC.</param>
    /// <param name="target_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CT.</param>
    /// <param name="target">The Target used to parse ACCEPTED_SHORTCUTS beginning with CT.</param>
    /// <returns>A float value of the equation given in the input</returns>
    public static float Convert_To_Float(string input, GameObject source_obj, GameObject target_obj, Target target)
    {
        if (input.Contains("TA_") && target == null)
        {
            Debug.Log("WARNING: string " + input + " contains a reference to a Target field but a null Target was provided.");
        }
        return Convert_To_Float(input, source_obj, target_obj, target, 0);
    }

    /// <summary>
    /// Converts the String parameters from the Character_Action into a double. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Double</param>
    /// <param name="source_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CC.</param>
    /// <param name="target_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CT.</param>
    /// <returns>A float value of the equation given in the input</returns>
    public static float Convert_To_Float(string input, GameObject source_obj, GameObject target_obj)
    {
        if (input.Contains("TA_"))
        {
            Debug.Log("WARNING: string " + input + " contains a reference to a Target field but no Target was provided.");
        }
        if ((input.Contains("TC_") || input.Contains("TT_")) && ! target_obj)
        {
            Debug.Log("WARNING: string " + input + " contains a reference to a target value but a null target was provided.");
        }
        return Convert_To_Float(input, source_obj, target_obj, null);
    }

    /// <summary>
    /// Converts the String parameters from the Character_Action into a double. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Double</param>
    /// <param name="source_obj">The GameObject used to parse ACCEPTED_SHORTCUTS beginning with CC.</param>
    /// <returns>A float value of the equation given in the input</returns>
    public static float Convert_To_Float(string input, GameObject source_obj)
    {
        if (input.Contains("TC_") || input.Contains("TT_"))
        {
            Debug.Log("WARNING: string " + input + " contains a reference to a target value but no target was provided.");
        }
        if ((input.Contains("SC_") || input.Contains("ST_")) && !source_obj)
        {
            Debug.Log("WARNING: string " + input + " contains a reference to a source value but a null source was provided.");
        }
        return Convert_To_Float(input, source_obj, null);
    }

    /// <summary>
    /// Converts the String parameters from the Character_Action into a double. 
    /// Parses Accepted_Shortcuts in the String based on stats from the provided Character_Script.
    /// </summary>
    /// <param name="input">The String to convert to a Double</param>
    /// <returns>A float value of the equation given in the input</returns>
    public static float Convert_To_Float(string input)
    {
        if (input.Contains("SC_") || input.Contains("ST_"))
        {
            Debug.Log("WARNING: string " + input + " contains a reference to a source value but no source was provided.");
        }
        return Convert_To_Float(input, null);
    }

    /// <summary>
    /// Takes a string input that is a valid check and returns a bool for that check.
    /// Checks should be in valid check format:
    ///  - CHK_[NOT]_<SOURCE>_<SHORTCUT>_<OPERATOR>_<SOURCE>_<SHORTCUT> for numerical checks
    ///  - CHK_[NOT]_<SOURCE>_<OPERATOR>_<SHORTCUT> for boolean checks
    /// Valid Sources:
    ///  - SC: Source Character
    ///  - ST: Source Tile
    ///  - TC: Target Character
    ///  - TT: Target Tile
    /// Valid Operators:
    ///  - EQ: Equals (Used for numerical checks)
    ///  - GT: Greater than (Used for numerical checks)
    ///  - LT: Less than (Used for numerical checks)
    ///  - GTQ: Greater than or equal (Used for numerical checks)
    ///  - LT: Less than or equal (Used for numerical checks)
    ///  - IS: Used for boolean checks.
    /// Valid Shortcuts:
    ///  - See Accepted Bool and Float Shortcuts.
    /// Example: CHK_CC_AUC_GT_TC_AUC checks if the current character's current Aura is greater than the target character's.
    /// Example: CHK_NOT_TT_IS_HOBJ checks if the target tile does not have an object on it. 
    /// Example: CHK_TC_IS_FRND checks if the target character is friendly (based on their tag).
    /// Example: CHK_TT_IS_HOBJ checks if the target tile has an object. 
    /// </summary>
    /// <param name="input">The check to check</param>
    /// <param name="source_obj">The object invoking the check. USed to expand checks with CC or CT parameters.</param>
    /// <param name="target_obj">The target of the check. Used to expand checks TC or TT parameters.</param>
    /// <param name="target">The target for the check. Used to expand checks containing TA parameter.</param>
    /// <returns>True if the check is True, False otherwise or if the check is not paresable.</returns>
    public static bool Convert_To_Bool(string input, GameObject source_obj, GameObject target_obj, Target target)
    {
        bool output = false;
        Character_Script source = null;
        Tile source_tile = null;
        string[] conditionals = input.Split('_');
        int i = 1;
        if (conditionals[i] == "NOT")
        {
            i++;
        }
        //get source
        if (conditionals[i] == "CC")
        {
            if (target_obj != null)
            {
                source = Get_Character_Script(source_obj);
            }
        }
        else if (conditionals[i] == "TC")
        {
            if (target_obj != null)
            {
                source = Get_Character_Script(target_obj);
            }
        }
        else if (input.Contains("CT"))
        {
            source_tile = Get_Tile_Script(source_obj);
        }
        else if (input.Contains("TT"))
        {
            if (target_obj != null)
            {
                source_tile = Get_Tile_Script(target_obj);
            }
        }

        if (conditionals[i + 2] == Accepted_Bool_Shortcuts.SELF.ToString())
        {
            if (source != null)
            {
                output = source.Equals(Get_Character_Script(source_obj));
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.FRND.ToString())
        {
            if (source != null)
            {
                output = source.Check_Tag("Character (Friend)");
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.ENMY.ToString())
        {
            if (source != null)
            {
                output = source.Check_Tag("Character (Enemy)");
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.HOBJ.ToString())
        {
            if (source_tile != null)
            {
                output = source_tile.Has_Object();
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.HHAZ.ToString())
        {
            if (source_tile != null)
            {
                output = source_tile.Has_Hazard();
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.TRAV.ToString())
        {
            if (source_tile != null)
            {
                output = source_tile.traversible;
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.IRNG.ToString())
        {
            if (source != null)
            {
                output = Get_Character_Script(source_obj).Get_Curr_Action().Check_Range(source);
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        else if (conditionals[i + 2] == Accepted_Bool_Shortcuts.COND.ToString())
        {
            if (source != null)
            {
                output = source.Has_Condition(new Condition(conditionals[i + 3]).type);
            }
            else
            {
                Debug.Log("Invalid source: " + input);
                return false;
            }
        }
        return output;
    }

    /// <summary>
    /// Takes a list of checks and returns whether the sum of the checks pass or not. 
    /// Checks should be split by && or || operators for logical purposes.
    /// Checks should be in valid check format:
    ///  - CHK_[NOT]_<SOURCE>_<SHORTCUT>_<OPERATOR>_<SOURCE>_<SHORTCUT> for numerical checks
    ///  - CHK_[NOT]_<SOURCE>_<OPERATOR>_<SHORTCUT> for boolean checks
    /// Valid Sources:
    ///  - SC: Source Character
    ///  - ST: Source Tile
    ///  - TC: Target Character
    ///  - TT: Target Tile
    /// Valid Operators:
    ///  - EQ: Equals (Used for numerical checks)
    ///  - GT: Greater than (Used for numerical checks)
    ///  - LT: Less than (Used for numerical checks)
    ///  - GTQ: Greater than or equal (Used for numerical checks)
    ///  - LT: Less than or equal (Used for numerical checks)
    ///  - IS: Used for boolean checks.
    /// Valid Shortcuts:
    ///  - See Accepted Bool and Float Shortcuts.
    /// Example: CHK_CC_AUC_GT_TC_AUC checks if the current character's current Aura is greater than the target character's.
    /// Example: CHK_NOT_TT_IS_HOBJ checks if the target tile does not have an object on it. 
    /// </summary>
    /// <param name="checks">The string array of checks to check.</param>
    /// <param name="source_obj">The source triggering the checks </param>
    /// <param name="target_obj">The target of the ability for the checks</param>
    /// <param name="target">The target of the ability. </param>
    /// <param name="curr_check">The number of the current check in the array (method is recursive)</param>
    /// <returns>True if the checks pass, False otherwise of if the checks cannot be parsed.</returns>
    public static bool Parse_Checks(string[] checks, GameObject source_obj, GameObject target_obj, Target target, int curr_check)
    {
        bool output = false;
        if (checks == null || checks[0] == "null")
        {
            return true;
        }
        for (int i = curr_check; i < checks.Length; i++)
        {
            string check = checks[i];
            Debug.Log("Check " + i + ": " + check);
            if (check == "&")
            {
                return output && Parse_Checks(checks, source_obj, target_obj, target, i + 1);
            }
            else if (check == "|")
            {
                return output || Parse_Checks(checks, source_obj, target_obj, target, i + 1);
            }
            else
            {
                output = Parse_Check(check, source_obj, target_obj, target);
            }
        }
        return output;
    }

    /// <summary>
    /// Takes a list of checks and returns whether the sum of the checks pass or not. Starts parsing at check 0.
    /// Checks should be split by && or || operators for logical purposes.
    /// Checks should be in valid check format:
    ///  - CHK_[NOT]_<SOURCE>_<SHORTCUT>_<OPERATOR>_<SOURCE>_<SHORTCUT> for numerical checks
    ///  - CHK_[NOT]_<SOURCE>_<OPERATOR>_<SHORTCUT> for boolean checks
    /// Valid Sources:
    ///  - SC: Source Character
    ///  - ST: Source Tile
    ///  - TC: Target Character
    ///  - TT: Target Tile
    /// Valid Operators:
    ///  - EQ: Equals (Used for numerical checks)
    ///  - GT: Greater than (Used for numerical checks)
    ///  - LT: Less than (Used for numerical checks)
    ///  - GTQ: Greater than or equal (Used for numerical checks)
    ///  - LT: Less than or equal (Used for numerical checks)
    ///  - IS: Used for boolean checks.
    /// Valid Shortcuts:
    ///  - See Accepted Bool and Float Shortcuts.
    /// Example: CHK_CC_AUC_GT_TC_AUC checks if the current character's current Aura is greater than the target character's.
    /// Example: CHK_NOT_TT_IS_HOBJ checks if the target tile does not have an object on it. 
    /// </summary>
    /// <param name="checks">The string array of checks to check.</param>
    /// <param name="source_obj">The source triggering the checks </param>
    /// <param name="target_obj">The target of the ability for the checks</param>
    /// <param name="target">The target of the ability. </param>
    /// <returns>True if the checks pass, False otherwise of if the checks cannot be parsed.</returns>
    public static bool Parse_Checks(string[] checks, GameObject source_obj, GameObject target_obj, Target target)
    {
        return Parse_Checks(checks, source_obj, target_obj, target, 0);
    }

    /// <summary>
    /// Takes a list of checks and returns whether the sum of the checks pass or not. Starts parsing at check 0.
    /// Checks should be split by && or || operators for logical purposes.
    /// Checks should be in valid check format:
    ///  - CHK_[NOT]_<SOURCE>_<SHORTCUT>_<OPERATOR>_<SOURCE>_<SHORTCUT> for numerical checks
    ///  - CHK_[NOT]_<SOURCE>_<OPERATOR>_<SHORTCUT> for boolean checks
    /// Valid Sources:
    ///  - SC: Source Character
    ///  - ST: Source Tile
    ///  - TC: Target Character
    ///  - TT: Target Tile
    /// Valid Operators:
    ///  - EQ: Equals (Used for numerical checks)
    ///  - GT: Greater than (Used for numerical checks)
    ///  - LT: Less than (Used for numerical checks)
    ///  - GTQ: Greater than or equal (Used for numerical checks)
    ///  - LT: Less than or equal (Used for numerical checks)
    ///  - IS: Used for boolean checks.
    /// Valid Shortcuts:
    ///  - See Accepted Bool and Float Shortcuts.
    /// Example: CHK_CC_AUC_GT_TC_AUC checks if the current character's current Aura is greater than the target character's.
    /// Example: CHK_NOT_TT_IS_HOBJ checks if the target tile does not have an object on it. 
    /// </summary>
    /// <param name="checks">The string array of checks to check.</param>
    /// <param name="source_obj">The source triggering the checks </param>
    /// <param name="target_obj">The target of the ability for the checks</param>
    /// <param name="target">The target of the ability. </param>
    /// <returns>True if the checks pass, False otherwise of if the checks cannot be parsed.</returns>
    public static bool Parse_Checks(string[] checks, GameObject source_obj, GameObject target_obj)
    {
        return Parse_Checks(checks, source_obj, target_obj, null, 0);
    }

    /// <summary>
    /// Function to parse a Check into an actual condition. 
    /// Checks should be in valid check format:
    ///  - CHK_[NOT]_<SOURCE>_<SHORTCUT>_<OPERATOR>_<SOURCE>_<SHORTCUT> for numerical checks
    ///  - CHK_[NOT]_<SOURCE>_<OPERATOR>_<SHORTCUT> for boolean checks
    /// Valid Sources:
    ///  - SC: Source Character
    ///  - ST: Source Tile
    ///  - TC: Target Character
    ///  - TT: Target Tile
    /// Valid Operators:
    ///  - EQ: Equals (Used for numerical checks)
    ///  - GT: Greater than (Used for numerical checks)
    ///  - LT: Less than (Used for numerical checks)
    ///  - GTQ: Greater than or equal (Used for numerical checks)
    ///  - LT: Less than or equal (Used for numerical checks)
    ///  - IS: Used for boolean checks.
    /// Valid Shortcuts:
    ///  - See Accepted Bool and Float Shortcuts.
    /// Example: CHK_CC_IS_COND_BLD returns true if the character has the condition Bleeding
    /// Example: CHK_NOT_SC_AUC_GT_TC_AUC returns true if the source's current aura points are not greater than the target's current aura points.
    /// </summary>
    /// <param name="input">The string to parse</param>
    /// <param name="source_obj">The object which is the source</param>
    /// <param name="target_obj">The object which is the target.</param>
    /// <param name="target">The Target which is the target.</param>
    /// <returns>True if all checks have passed, false otherwise.</returns>
    public static bool Parse_Check(string input, GameObject source_obj, GameObject target_obj, Target target)
    {
        if (input == null || input == "" || input == "NULL")
        {
            return true;
        }
        bool output = true;
        bool not_flag = false;
        //Debug.Log("Check is " + input);
        string[] conditionals = input.Split('_');
        int i = 1;
        if(conditionals[0] != "CHK")
        {
            Debug.Log("Invalid check " + input);
            return false;
        }
        if (conditionals[i] == "NOT")
        {
            not_flag = true;
            i++;
        }  

        //Check if it is a boolean check
        if (conditionals[i + 1] == "IS")
        {
            output = Convert_To_Bool(input, source_obj, target_obj, target);
        }
        else
        {
            //Check if it is a numerical comparison
            Array values = Enum.GetValues(typeof(Comparison_Operators));
            bool comparison = false;
            foreach (Comparison_Operators val in values)
            {
                if (conditionals[i + 2] == val.ToString())
                {
                    comparison = true;
                    break;
                }
            }
            if (comparison)
            {
                string temp = conditionals[i] + "_" + conditionals[i + 1];
                float value1 = Convert_To_Float(temp, source_obj, target_obj, target);
                if (conditionals[i + 2] == Comparison_Operators.EQ.ToString())
                {
                    temp = conditionals[i + 3] + "_" + conditionals[i + 4];
                    output = (value1 == Convert_To_Float(temp, source_obj, target_obj, target));
                }
                else if (conditionals[i + 2] == Comparison_Operators.GT.ToString())
                {
                    temp = conditionals[i + 3] + "_" + conditionals[i + 4];
                    output = (value1 > Convert_To_Float(temp, source_obj, target_obj, target));
                }
                else if (conditionals[i + 2] == Comparison_Operators.GTQ.ToString())
                {
                    temp = conditionals[i + 3] + "_" + conditionals[i + 4];
                    output = (value1 >= Convert_To_Float(temp, source_obj, target_obj, target));
                }
                else if (conditionals[i + 2] == Comparison_Operators.LT.ToString())
                {
                    temp = conditionals[i + 3] + "_" + conditionals[i + 4];
                    output = (value1 < Convert_To_Float(temp, source_obj, target_obj, target));
                }
                else if (conditionals[i + 2] == Comparison_Operators.LTQ.ToString())
                {
                    temp = conditionals[i + 3] + "_" + conditionals[i + 4];
                    output = (value1 <= Convert_To_Float(temp, source_obj, target_obj, target));
                }
            }
            else
            {
                return false;
            }
        }
        
        if (not_flag)
        {
            output = !output;
        }
        return output;
    }

    /// <summary>
    /// Parses a String Equation and computes it. Used by the Convert_To_Float Function to fully solve equations.
    /// </summary>
    /// <param name="input">An Equation to parse out.</param>
    /// <returns></returns>
    public static float Parse_Equation(string input)
    {
        //Debug.Log("Parsing: " + input);
        //base case, can we convert to double?
        float output;
        if (float.TryParse(input, out output))
        {
            return output;
        }
        //otherwise, we have work to do
        else
        {
            //Order of operations is reversed because it's a stack.
            //resolve parentheses
            if (input.Contains("(") || input.Contains(")"))
            {
                string[] split = input.Split(new char[] { '(' }, 2);
                string[] split2 = split[1].Split(new char[] { ')' }, 2);
                double result = Parse_Equation(split2[0]);
                return Parse_Equation("" + split[0] + result + split2[1]);
            }
            //resolve addition
            if (input.Contains("+"))
            {
                string[] split = input.Split(new char[] { '+' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) + Parse_Equation(split[1])));
            }
            //resolve subtraction
            if (input.Contains("-"))
            {
                string[] split = input.Split(new char[] { '-' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) - Parse_Equation(split[1])));
            }
            //resolve multiplication
            if (input.Contains("*"))
            {
                string[] split = input.Split(new char[] { '*' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) * Parse_Equation(split[1])));
            }
            //resolve division
            if (input.Contains("/"))
            {
                string[] split = input.Split(new char[] { '/' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) / Parse_Equation(split[1])));
            }
            //resolve mod
            if (input.Contains("%"))
            {
                string[] split = input.Split(new char[] { '%' }, 2);
                return Parse_Equation("" + (Parse_Equation(split[0]) % Parse_Equation(split[1])));
            }
            //resolve exponents
            if (input.Contains("^"))
            {
                string[] split = input.Split(new char[] { '^' }, 2);
                return Parse_Equation("" + (Mathf.Pow((float)Parse_Equation(split[0]), (float)Parse_Equation(split[1]))));
            }
        }
        return output;
    }

    /// <summary>
    /// Ran before a Start Method. Ensures there is only ever one Game_Controller.
    /// </summary>
	void Awake (){
		if (controller == null) {
			DontDestroyOnLoad (gameObject);
			controller = this;
		} else if (controller != this) {
			Destroy(gameObject);
		}
	}

    /// <summary>
    /// Used for initialization.
    /// Finds the Canvas to draw the UI text on.
    /// Finds the Main Camera.
    /// Sets up the Character_Action Menu
    /// Loads the first Scenario.
    /// </summary>
	void Start() {
        mouse_over_ui = false;
        controlls = createControlls();
        canvas = GameObject.Find("Canvas");
        /*if (SceneManager.GetActiveScene().name == "Main_Menu") {
            UI_Controller.Get_Controller().Start(Scenes.Main_Menu);
        }*/
        popup = Resources.Load<FloatingText>("Prefabs/Scenario Prefabs/Object Prefabs/PopupTextParent");
        main_camera = GameObject.FindGameObjectWithTag("MainCamera");
        all_actions = Character_Action.Load_Actions();
        all_weapons = Weapon.Load_Weapons();
        all_armors = Armor.Load_Armors();

        //Initialize the dictionary of character stats
        if (stat_descriptions == null)
        {
            Load_Stat_Descriptions();
        }

        //curr_scenario = GameObject.Find("Scenario").GetComponent<Scenario>();
        //curr_scenario.Start(STARTING_SCENARIO);
        avail_scenarios = new ArrayList();
        prev_scene = SceneManager.GetActiveScene().name;
        loadout_data = new Character_Script_Data[6];
        //avail_scenarios.Add(curr_scenario);
        //curr_scenario.Load_Scenario();
        //action_menu.GetComponent<Action_Menu_Script>().Initialize();
        //action_menu.GetComponent<Action_Menu_Script>().resetActions();
        //MarkReachable ();

        if (SceneManager.GetActiveScene().name == Scenes.Editor.ToString())
        {
            editor = GameObject.FindGameObjectWithTag("Editor_Inspector").GetComponent<Editor_UI>();
            editor_controller = GameObject.FindGameObjectWithTag("Editor_Controller").GetComponent<Editor_Controller>();
        }

        //Set up error messages
        error_ui = GameObject.FindGameObjectWithTag("Error_Editor");
        if(error_ui == null)
        {
            GameObject prefab = Resources.Load(ERROR_PREFAB_SRC) as GameObject;
            GameObject obj = Instantiate(prefab);
            obj.transform.parent = canvas.transform;
            error_ui = obj;
        }
        error_ui.GetComponent<Error_Editor_UI>().Disable();
    }

    /// <summary>
    /// Method to create default Controlls.
    /// </summary>
    /// <returns>A dictionary with the default controlls mapped to the default keys.</returns>
    public Dictionary<Controlls, KeyCode[]> createControlls()
    {
        Dictionary<Controlls, KeyCode[]> controlls = new Dictionary<Controlls, KeyCode[]>();
        foreach (Controlls ctrl in Controlls.GetValues(typeof(Controlls)))
        {
            KeyCode[] keys = new KeyCode[2];
            if (ctrl == Controlls.Ability_Hotkey_0)
            {
                keys[0] = KeyCode.Alpha1;
                keys[1] = KeyCode.Keypad1;
            }
            else if (ctrl == Controlls.Ability_Hotkey_1)
            {
                keys[0] = KeyCode.Alpha2;
                keys[1] = KeyCode.Keypad2;
            }
            else if (ctrl == Controlls.Ability_Hotkey_2)
            {
                keys[0] = KeyCode.Alpha3;
                keys[1] = KeyCode.Keypad3;
            }
            else if (ctrl == Controlls.Ability_Hotkey_3)
            {
                keys[0] = KeyCode.Alpha4;
                keys[1] = KeyCode.Keypad4;
            }
            else if (ctrl == Controlls.Ability_Hotkey_4)
            {
                keys[0] = KeyCode.Alpha5;
                keys[1] = KeyCode.Keypad5;
            }
            else if (ctrl == Controlls.Ability_Hotkey_5)
            {
                keys[0] = KeyCode.Alpha6;
                keys[1] = KeyCode.Keypad6;
            }
            else if (ctrl == Controlls.Ability_Hotkey_6)
            {
                keys[0] = KeyCode.Alpha7;
                keys[1] = KeyCode.Keypad7;
            }
            else if (ctrl == Controlls.Ability_Hotkey_7)
            {
                keys[0] = KeyCode.Alpha8;
                keys[1] = KeyCode.Keypad8;
            }
            else if (ctrl == Controlls.Ability_Hotkey_8)
            {
                keys[0] = KeyCode.Alpha9;
                keys[1] = KeyCode.Keypad9;
            }
            else if (ctrl == Controlls.Ability_Hotkey_9)
            {
                keys[0] = KeyCode.Alpha0;
                keys[1] = KeyCode.Keypad0;
            }
            else if (ctrl == Controlls.Camera_Scroll_Up)
            {
                keys[0] = KeyCode.W;
                keys[1] = KeyCode.UpArrow;
            }
            else if (ctrl == Controlls.Camera_Scroll_Down)
            {
                keys[0] = KeyCode.S;
                keys[1] = KeyCode.DownArrow;
            }
            else if (ctrl == Controlls.Camera_Scroll_Left)
            {
                keys[0] = KeyCode.A;
                keys[1] = KeyCode.LeftArrow;
            }
            else if (ctrl == Controlls.Camera_Scroll_Right)
            {
                keys[0] = KeyCode.D;
                keys[1] = KeyCode.RightArrow;
            }
            else if (ctrl == Controlls.Camera_Turn_Left)
            {
                keys[0] = KeyCode.Q;
                keys[1] = KeyCode.LeftBracket;
            }
            else if (ctrl == Controlls.Camera_Turn_Right)
            {
                keys[0] = KeyCode.E;
                keys[1] = KeyCode.RightBracket;
            }
            else if (ctrl == Controlls.Next_Player)
            {
                keys[0] = KeyCode.Equals;
                keys[1] = KeyCode.KeypadPlus;
            }
            else if (ctrl == Controlls.Previous_Player)
            {
                keys[0] = KeyCode.Minus;
                keys[1] = KeyCode.KeypadMinus;
            }
            else if (ctrl == Controlls.Pause)
            {
                keys[0] = KeyCode.P;
                keys[1] = KeyCode.P;
            }
            controlls.Add(ctrl, keys);
        }

        return controlls;
    }

    /// <summary>
    /// Loads the description of each stat in the dictionary. This is used for populating menus later.
    /// </summary>
    public static void Load_Stat_Descriptions()
    {
        stat_descriptions = new Dictionary<Accepted_Float_Shortcuts, string>();
        TextAsset textFile = Resources.Load("Config/Descriptions/Float_Stats/Stat_Descriptions") as TextAsset;
        string text = textFile.text;
        string[] lines = text.Split('\n');
        foreach(string line in lines)
        {
            string[] content = line.Split('-');
            foreach (Accepted_Float_Shortcuts sh in Enum.GetValues(typeof(Accepted_Float_Shortcuts)))
            {
                if(sh.ToString() == content[0])
                {
                    stat_descriptions[sh] = content[1];
                }
            }
        }
        bool_descriptions = new Dictionary<Accepted_Bool_Shortcuts, string>();
        textFile = Resources.Load("Config/Descriptions/Bool_Stats/Stat_Descriptions") as TextAsset;
        text = textFile.text;
        lines = text.Split('\n');
        foreach (string line in lines)
        {
            string[] content = line.Split('-');
            foreach (Accepted_Bool_Shortcuts sh in Enum.GetValues(typeof(Accepted_Bool_Shortcuts)))
            {
                if (sh.ToString() == content[0])
                {
                    bool_descriptions[sh] = content[1];
                }
            }
        }
        condi_descriptions = new Dictionary<Condition_Shortcuts, string>();
        textFile = Resources.Load("Config/Descriptions/Conditions/Condition_Descriptions") as TextAsset;
        text = textFile.text;
        lines = text.Split('\n');
        foreach (string line in lines)
        {
            string[] content = line.Split('-');
            foreach (Condition_Shortcuts sh in Enum.GetValues(typeof(Condition_Shortcuts)))
            {
                if (sh.ToString() == content[0])
                {
                    condi_descriptions[sh] = content[1];
                }
            }
        }
    }

    /// <summary>
    /// Creates Floating Text from the FloatingText prefab at a specified location.
    /// </summary>
    /// <param name="text">The text to display. </param>
    /// <param name="location">The location to display the text. </param>
    /// <param name="color">The color of the text. </param>
    public static void Create_Floating_Text(string text, Transform location, Color color)
    {
        FloatingText instance = Instantiate(popup);
        Vector2 screen_position = Camera.main.WorldToScreenPoint(new Vector3(location.position.x + UnityEngine.Random.Range(-.5f, .5f),
            location.position.y + UnityEngine.Random.Range(1.5f,1.7f), location.position.z));
        instance.transform.SetParent(canvas.transform, false);
        instance.transform.position = screen_position;
        instance.Set_Text(text);
        instance.Set_Color(color);
    }

    public static void Create_Error_Message(string message)
    {
        error_ui.GetComponent<Error_Editor_UI>().Enable(message);
    }

    public static void Create_Error_Message(string message, Error_Editor_UI.InputDelegate del)
    {
        error_ui.GetComponent<Error_Editor_UI>().Enable(message, del);
    }

    /// <summary>
    /// Called once per frame to update the Game.
    /// Updates the Scenario.
    /// Checks for Input
    /// Checks Mouse Poistion
    /// </summary>
    void Update () {
        //curr_scenario.Update();
        Read_Input();
        checkMousePos();
    }

    /// <summary>
    /// Loads a specific scene.
    /// </summary>
    /// <param name="scene">The scene to load.</param>
    public void Load_Scene(Scenes scene)
    {
        prev_scene = SceneManager.GetActiveScene().name;
        //UI_Controller.Get_Controller().Clean_Canvas();
        SceneManager.LoadScene(scene.ToString());
        /*if (scene == Scenes.Main_Menu)
        {
            UI_Controller.Get_Controller().Start(Scenes.Main_Menu);
        }*/
    }

    /// <summary>
    /// Loads the previous scene.
    /// </summary>
    public void Load_Prev_Scene()
    {
        //UI_Controller.Get_Controller().Clean_Canvas();
        SceneManager.LoadScene(prev_scene);
        /*if (scene == Scenes.Main_Menu)
        {
            UI_Controller.Get_Controller().Start(Scenes.Main_Menu);
        }*/
    }

    public void Mouse_Enter_UI()
    {
        mouse_over_ui = true;
    }

    public void Mouse_Exit_UI()
    {
        mouse_over_ui = false;
    }

    /// <summary>
    /// Reads Player Input.
    /// </summary>
    void Read_Input()
    {
        //map selection
        //Deprecated
        /*if (Input.GetKeyDown("space"))
        {
            if (curr_scenario.scenario_file == "Assets/Resources/Maps/falls_map.txt")
            {
                curr_scenario.Unload_Scenario();
                curr_scenario = new Scenario("Assets/Maps/tile_map.txt");
                curr_scenario.Load_Scenario();
            }
            else if (curr_scenario.scenario_file == "Assets/Resources/Maps/tile_map.txt")
            {
                curr_scenario.Unload_Scenario();
                curr_scenario = new Scenario("Assets/Maps/falls_map.txt");
                curr_scenario.Load_Scenario();
            }
        }*/

        //check for mouse clicks
        GraphicRaycaster caster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        PointerEventData data = new PointerEventData(null);
        data.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        if (!mouse_over_ui && Input.GetMouseButtonDown(0))
        {
            //Create the PointerEventData
            data = new PointerEventData(null);
            //Look at mouse position
            data.position = Input.mousePosition;
            //Create list to receive results
            results = new List<RaycastResult>();
            //Raycast
            caster.Raycast(data, results);
            if (curr_scenario != null)
            {
                curr_scenario.Reset_Cursor();
                if (results.Count == 0)
                {
                    curr_scenario.clicked_tile = curr_scenario.selected_tile;
                }
                if (SceneManager.GetActiveScene().name != "Editor")
                {
                    Character_Script character = curr_scenario.Get_Curr_Character().GetComponent<Character_Script>();
                    if ((character.state.Peek() != Character_States.Idle ||
                    character.state.Peek() != Character_States.Dead))
                    {
                        foreach (Tile tile in curr_scenario.reachable_tiles)
                        {
                            if (tile.Equals(curr_scenario.clicked_tile))
                            {
                                //Debug.Log(character.name + " num of curr_action " + character.curr_action.Count);
                                //Add selected tile to list of targets. If the right number of targets is met, trigger the Action.
                                if (character.Get_Curr_Action().path_type == Path_Types.Path && Input.GetKey(KeyCode.LeftShift))
                                {
                                    //Get the current path to the clicked tile and save it under the current path for the Action.
                                    if (character.Get_Curr_Action().Add_Waypoint(curr_scenario.clicked_tile.gameObject))
                                    {
                                        //character.Get_Curr_Action().Find_Path(curr_scenario.clicked_tile.gameObject);
                                        curr_scenario.Reset_Reachable();
                                        character.Get_Curr_Action().Find_Reachable_Tiles(character, true);
                                        curr_scenario.Mark_Reachable();
                                    }

                                }
                                else
                                {
                                    character.Get_Curr_Action().Add_Target(curr_scenario.clicked_tile.gameObject);
                                    if (character.Get_Curr_Action().Has_Valid_Targets() && character.state.Peek() == Character_States.Idle)
                                    {
                                        //Debug.Log("Acting");

                                        character.StartCoroutine(character.Act(character.Get_Curr_Action(), curr_scenario.clicked_tile));
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (editor != null && editor_controller != null)
                    {
                        editor_controller.Reset_Tiles();
                        editor_controller.Add_Tile(curr_scenario.selected_tile);
                        editor.Change_Inspector_Type(1);
                    }
                }
                curr_scenario.clicked_tile = null;
            }
        }
        if (!mouse_over_ui && Input.GetMouseButton(0))
        {
            //Create the PointerEventData
            data = new PointerEventData(null);
            //Look at mouse position
            data.position = Input.mousePosition;
            //Create list to receive results
            results = new List<RaycastResult>();
            //Raycast
            caster.Raycast(data, results);
            if (curr_scenario != null)
            {
                if (results.Count == 0)
                {
                    curr_scenario.clicked_tile = curr_scenario.selected_tile;
                }
                editor_controller.Add_Tile(curr_scenario.selected_tile);
                if (editor.Get_Inspector_Mode() == 1)
                {
                    editor.Populate_Tile_Inspector();
                }
            }
        }
        //Action menu hotkeys
        if (curr_scenario != null && SceneManager.GetActiveScene().name != "Editor") {
            Character_Script character = curr_scenario.Get_Curr_Character().GetComponent<Character_Script>();
            if (!character.ending_turn)
            {
                if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_0][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_0][1]))
                {
                    if (character.actions.Count >= 1)
                    {
                        character.actions[0].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_1][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_1][1]))
                {
                    if (character.actions.Count >= 2)
                    {
                        character.actions[1].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_2][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_2][1]))
                {
                    if (character.actions.Count >= 3)
                    {
                        character.actions[2].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_3][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_3][1]))
                {
                    if (character.actions.Count >= 4)
                    {
                        character.actions[3].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_4][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_4][1]))
                {
                    if (character.actions.Count >= 5)
                    {
                        character.actions[4].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_5][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_5][1]))
                {
                    if (character.actions.Count >= 6)
                    {
                        character.actions[5].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_6][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_6][1]))
                {
                    if (character.actions.Count >= 7)
                    {
                        character.actions[6].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_7][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_7][1]))
                {
                    if (character.actions.Count >= 8)
                    {
                        character.actions[7].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_8][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_8][1]))
                {
                    if (character.actions.Count >= 9)
                    {
                        character.actions[8].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_9][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Ability_Hotkey_9][1]))
                {
                    if (character.actions.Count >= 10)
                    {
                        character.actions[9].Select();
                    }
                }
                else if (Input.GetKeyDown(controlls[Controlls.Pause][0]) ||
                    Input.GetKeyDown(controlls[Controlls.Pause][1]))
                {
                    if (character.Get_Curr_Action() != null)
                    {
                        if (character.Get_Curr_Action().paused)
                        {
                            character.Get_Curr_Action().Resume();
                        }
                        else
                        {
                            character.Get_Curr_Action().Pause();
                        }
                    }
                }
                if (Input.GetKeyDown("k"))
                {
                    character.Die();
                    curr_scenario.Next_Player();
                }
            }

            //Next player button
            if (Input.GetKeyDown(controlls[Controlls.Next_Player][0]) ||
                Input.GetKeyDown(controlls[Controlls.Next_Player][1]))
            {
                curr_scenario.Next_Player();
            }

            //Prev player button
            if (Input.GetKeyDown(controlls[Controlls.Previous_Player][0]) ||
                Input.GetKeyDown(controlls[Controlls.Previous_Player][1]))
            {
                curr_scenario.Prev_Player();
            }
        }

        //check for mouse button up
        if (Input.GetMouseButtonUp(0))
        {
            //cursor.GetComponent<Animator>().SetBool("Clicked", false);
        }

        //Camera Turning
        //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().state);
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            if ((Input.GetKeyDown(controlls[Controlls.Camera_Turn_Right][0]) ||
                Input.GetKeyDown(controlls[Controlls.Camera_Turn_Right][1])) &&
                (!curr_scenario.Has_Current_Character() ||
                curr_scenario.Get_Curr_Character().state.Peek() != Character_States.Walking))
            {
                //Debug.Log("x:" + main_camera.transform.rotation.x + ", y:" + main_camera.transform.rotation.y + "z:" + main_camera.transform.rotation.z + ", w:" + main_camera.transform.rotation.w);

                //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                //transform.RotateAround(transform.position, Vector3.up, -90f);
                //main_camera.transform.RotateAround(curr_scenario.selected_tile.transform.position, Vector3.up, 90* Time.deltaTime);
                //main_camera.GetComponent<Camera_Controller>().targetRotation *= Quaternion.AngleAxis(90, main_camera.transform.forward);
                if (!main_camera.GetComponent<Camera_Controller>().rotating)
                {
                    main_camera.GetComponent<Camera_Controller>().rotating = true;
                    main_camera.GetComponent<Camera_Controller>().rotationAmount -= 90;
                    foreach (GameObject chara in curr_scenario.characters.Values)
                    {
                        chara.GetComponent<Character_Script>().rotate = true;
                        //chara.GetComponent<Character_Script>().camera_offset += 1;
                        //chara.GetComponent<Character_Script>().orientation += 1;
                        //chara.GetComponent<Character_Script>().Orient();
                    }
                }

                //main_camera.GetComponent<Camera_Controller>().rotationAmount -= 90;
                //update orientation based on camera
                //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().orientation);

            }
            if ((Input.GetKeyDown(controlls[Controlls.Camera_Turn_Left][0]) ||
                Input.GetKeyDown(controlls[Controlls.Camera_Turn_Left][1])) &&
                (!curr_scenario.Has_Current_Character() || 
                curr_scenario.Get_Curr_Character().state.Peek() != Character_States.Walking))
            {
                //Debug.Log("x:" + main_camera.transform.rotation.x + ", y:" + main_camera.transform.rotation.y + "z:" + main_camera.transform.rotation.z + ", w:" + main_camera.transform.rotation.w);
                //transform.rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                //transform.RotateAround(transform.position, Vector3.up, 90f);
                if (!main_camera.GetComponent<Camera_Controller>().rotating)
                {
                    main_camera.GetComponent<Camera_Controller>().rotating = true;
                    main_camera.GetComponent<Camera_Controller>().rotationAmount += 90;
                    foreach (GameObject chara in curr_scenario.characters.Values)
                    {
                        chara.GetComponent<Character_Script>().rotate = true;
                        //chara.GetComponent<Character_Script>().camera_offset -= 1;
                        //chara.GetComponent<Character_Script>().orientation -= 1;
                        //chara.GetComponent<Character_Script>().Orient();
                    }
                }

                //main_camera.GetComponent<Camera_Controller>().rotationAmount += 90;
                //Debug.Log(curr_scenario.curr_player.GetComponent<Character_Script>().orientation);

            }
        }
    }

    /// <summary>
    /// Checks the Mouse position and gives a Tile if the mouse is hovering over it.
    /// </summary>
    public void checkMousePos()
    {
        //create a ray at the mouse position, point it towards the grid. See if it hits a collider.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        //deprecated 2d raycast physics
        //hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Physics.Raycast(ray, out hit, 100))
        {
            Tile tile_data = hit.transform.GetComponent<Tile>();
            if (tile_data != null)
            {
                //update cursor location
                curr_scenario.selected_tile = tile_data;
                curr_scenario.Update_Cursor(curr_scenario.selected_tile);
            }
        }
    }

    /// <summary>
    /// Used to write content to a file.
    /// </summary>
    /// <param name="filename">The name of the file to write to.</param>
    /// <param name="content">The content to put in the file.</param>
    public static void Write_File(string filename, string content)
    {
        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(filename, true);
        writer.Write(content);
        writer.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(filename);

    }

}

/// <summary>
/// Used for Saving and Loading the Game. 
/// </summary>
[Serializable]
class GameData
{
	public string curr_map;
	public int curr_character_num;
	public GameObject[] characters;
	public GameObject curr_player;
	public GameObject cursor;
	public GameObject tile_grid;
	public Tile tile_data;
	public Transform[,] tiles;
	public Transform clicked_tile;
	public Transform selected_tile;
	public bool initialized;
	public int curr_round;
}
