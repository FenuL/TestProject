using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Character_Script : MonoBehaviour {

    public class Action
    {
        public static string PLAYER_ACTIONS_FILE = "Assets/Resources/Actions/Action_List.txt";
        public static int HEADINGS = 12;
        public enum Activation_Types { Active, Passive, Reactive }
        public enum Origin_Types { Innate, Soul, Weapon }
        public enum Accepted_Shortcuts { AUM, AUC, CAM, CAC, SPD, STR, CRD, SPT, DEX, VIT, LVL, WPR, WPD, ARM, WGT, MOC, DST, NUL }
        /// <summary>
        /// AUM = Char Aura MAX
        /// AUC = Char Aura Current
        /// CAM = Char Canister Max
        /// CAC = Char Canister Current
        /// SPD = Char Speed
        /// STR = Char Strength
        /// CRD = Char Coordination
        /// SPT = Char Spirit
        /// DEX = Char Dexterity
        /// VIT = Char Vitality
        /// LVL = Char Level
        /// WPR = Weapon Range
        /// WPD = Weapon Damage
        /// WPN = Weapon
        /// ARM = Armor Value
        /// WGT = Character Weight
        /// MOC = Movement Cost
        /// DST = Distance between self and target
        /// NUL = Null
        /// </summary>

        //Action name
        public String name;
        //Cost in AP
        public String cost;
        //Affects the number of tiles away the skill can target.
        public String range;
        //Area affected by the skill in number of tiles (1 range is single target)
        public String area;
        //falloff on the skill effect over the area (affects Damage and Elevetaion). 
        public String falloff;
        public List<Effect> self_effect;
        public List<Effect> target_effect;
        public Activation_Types type;
        //what triggers the action if it is a reactive action
        public String trigger;
        //Where the skill originates (for disabling purposes later)
        public Origin_Types origin;
        //if the action is enabled or not
        public bool enabled;
        public String animation;

        public static Action Parse(String[] input)
        {
            Action act = new Action();
            foreach (String s in input)
            {
                if (s != null)
                {
                    String category = s.Split(':')[0];
                    String values = s.Split(':')[1];
                    switch (category)
                    {
                        case "name":
                            act.name = values.TrimStart().TrimEnd();
                            break;
                        case "cost":
                            act.cost = values.Trim();
                            break;
                        case "range":
                            act.range = values.Trim();
                            break;
                        case "area":
                            act.area = values.Trim();
                            break;
                        case "falloff":
                            act.falloff = values.Trim();
                            break;
                        case "self_effect":
                            act.self_effect = new List<Effect>();
                            if (!values.Contains("NUL"))
                            {
                                foreach (String value in values.TrimStart().TrimEnd().Split(';'))
                                {
                                    act.self_effect.Add(new Effect(value));
                                }
                            }
                            break;
                        case "target_effect":
                            act.target_effect = new List<Effect>();
                            if (!values.Contains("NUL"))
                            {
                                foreach (String value in values.TrimStart().TrimEnd().Split(';'))
                                {
                                    act.target_effect.Add(new Effect(value));
                                }
                            }
                            break;
                        case "activation_type":
                            if (values.Trim() == "Reactive" ||
                                values.Trim() == "reactive")
                            {
                                act.type = Activation_Types.Reactive;
                            }
                            else if (values.Trim() == "Passive" ||
                                values.Trim() == "passive")
                            {
                                act.type = Activation_Types.Passive;
                            }
                            else
                            {
                                act.type = Activation_Types.Active;
                            }
                            break;
                        case "origin":
                            if (values.Trim() == "Weapon" ||
                                values.Trim() == "weapon")
                            {
                                act.origin = Origin_Types.Weapon;
                            }
                            else if (values.Trim() == "Soul" ||
                                values.Trim() == "soul")
                            {
                                act.origin = Origin_Types.Soul;
                            }
                            else
                            {
                                act.origin = Origin_Types.Innate;
                            }
                            break;
                        case "trigger":
                            act.trigger = values.Trim();
                            break;
                        case "enabled":
                            if (values.Trim() == "True" ||
                                values.Trim() == "true" ||
                                values.Trim() == "TRUE")
                            {
                                act.enabled = true;
                            }
                            else
                            {
                                act.enabled = false;
                            }
                            break;
                        case "animation":
                            act.animation = values.Trim();
                            break;
                    }
                }
            }
            return act;
        }

        public static List<Action> LoadActions()
        {
            string[] lines = System.IO.File.ReadAllLines(PLAYER_ACTIONS_FILE);
            string[] subset = new String[HEADINGS];
            string line = "";
            List<Action> actions = new List<Action>();
            for (int i = 0; i < lines.Length; i++)
            {
                line = lines[i];
                if(!lines[i].Contains("==="))
                {
                    subset[i % (HEADINGS)] = lines[i];
                }
                else
                {
                    actions.Add(Parse(subset));
                    subset = new String[HEADINGS];
                }

            }
            return actions;
        }

        public double convertToDouble(string input, Character_Script obj)
        {
            double output = 0.0;
            if (double.TryParse(input, out output))
            {
                return output;
            }
            else
            {
                Array values = Enum.GetValues(typeof(Accepted_Shortcuts));

                foreach (Accepted_Shortcuts val in values)
                {
                    if (input.Contains(val.ToString()))
                    {
                        if (val.ToString() == "AUM")
                        {
                            input = input.Replace(val.ToString(), ""+obj.aura_max );
                        }
                        else if (val.ToString() == "AUC")
                        {
                            input = input.Replace(val.ToString(), "" + obj.aura_curr);
                        }
                        else if (val.ToString() == "CAM")
                        {
                            input = input.Replace(val.ToString(), "" + obj.canister_max);
                        }
                        else if (val.ToString() == "CAC")
                        {
                            input = input.Replace(val.ToString(), "" + obj.canister_curr);
                        }
                        else if (val.ToString() == "SPD")
                        {
                            input = input.Replace(val.ToString(), "" + obj.speed);
                        }
                        else if (val.ToString() == "STR")
                        {
                            input = input.Replace(val.ToString(), "" + obj.strength);
                        }
                        else if (val.ToString() == "CRD")
                        {
                            input = input.Replace(val.ToString(), "" + obj.coordination);
                        }
                        else if (val.ToString() == "SPT")
                        {
                            input = input.Replace(val.ToString(), "" + obj.spirit);
                        }
                        else if (val.ToString() == "DEX")
                        {
                            input = input.Replace(val.ToString(), "" + obj.dexterity);
                        }
                        else if (val.ToString() == "VIT")
                        {
                            input = input.Replace(val.ToString(), "" + obj.vitality);
                        }
                        else if (val.ToString() == "LVL")
                        {
                            input = input.Replace(val.ToString(), "" + obj.level);
                        }
                        else if (val.ToString() == "WPR")
                        {
                            input = input.Replace(val.ToString(), "" + obj.weapon.range);
                        }
                        else if (val.ToString() == "WPD")
                        {
                            input = input.Replace(val.ToString(), "" + obj.weapon.attack);
                        }
                        else if (val.ToString() == "WPN")
                        {
                            input = input.Replace(val.ToString(), "" + obj.weapon.name);
                        }
                        else if (val.ToString() == "ARM")
                        {
                            input = input.Replace(val.ToString(), "" + obj.armor.armor);
                        }
                        else if (val.ToString() == "WGT")
                        {
                            input = input.Replace(val.ToString(), "" + obj.armor.weight);
                        }
                        else if (val.ToString() == "MOC")
                        {
                            input = input.Replace(val.ToString(), "" + obj.action_curr);
                        }
                        else if (val.ToString() == "DST")
                        {
                            input = input.Replace(val.ToString(), "" + obj.aura_max);
                        }
                        else if (val.ToString() == "NUL")
                        {
                            input = input.Replace(val.ToString(), "" + 0.0);
                        }
                    }
                    //try to convert to double after converting
                    if (double.TryParse(input, out output))
                    {
                        return output;
                    }

                }
                return -1.0;
            }

        }

        public void Select(Character_Script character)
        {
            character.action_cost = (int)convertToDouble(cost, character);
            if (target_effect != null)
            {
                foreach (Effect eff in target_effect)
                {
                    if (eff.type.ToString() == Effect.Types.Move.ToString())
                    {
                        character.state = States.Moving;
                        
                        character.controller.curr_scenario.FindReachable((int)convertToDouble(cost, character), (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Status.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                }
            }
            if (self_effect != null)
            {
                foreach (Effect eff in self_effect)
                {
                    if (eff.type.ToString() == Effect.Types.Move.ToString())
                    {
                        character.state = States.Moving;
                        character.controller.curr_scenario.FindReachable((int)convertToDouble(cost, character), (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));

                    }
                    else if (eff.type.ToString() == Effect.Types.Status.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                    {
                        character.state = States.Attacking;
                        character.controller.curr_scenario.FindReachable(character.action_curr, (int)convertToDouble(range, character));
                    }
                    else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                    {
                        character.state = States.Idle;
                        character.action_curr -= character.action_cost;
                        print("Character " + character.character_name + " Passed, Recovering " + character.AP_RECOVERY + " AP.");
                        character.action_cost = 0;
                        if (character.action_curr > character.action_max)
                        {
                            character.action_curr = character.action_max;
                        }
                        character.controller.curr_scenario.CleanReachable();
                        character.controller.curr_scenario.NextPlayer();
                        return;
                    }
                }
            }
            //Select the action in the action menu
            foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
            {
                if (but.name == name)
                {
                    but.GetComponent<Image>().color = Color.blue;
                }
                else
                {
                    but.GetComponent<Image>().color = Color.white;
                }
            }
            character.controller.curr_scenario.CleanReachable();
            character.controller.curr_scenario.MarkReachable();
            character.curr_action = this;
        }

        public void Enact(Character_Script character, GameObject target_tile)
        {
            character.action_curr -= character.action_cost;
            character.action_cost = 0;

            if (target_effect != null)
            {
                foreach (Effect eff in target_effect)
                {
                    if(eff.type.ToString() == Effect.Types.Move.ToString())
                    {

                        //target_character.MoveTo(target_tile.transform);
                    }
                    else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                    {
                        if (target_tile.GetComponent<Tile_Data>().node.obj == null)
                        {
                            return;
                        }
                        Character_Script target_character = target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>();
                        print("Character " + character.character_name + " Attacked: " + target_character.GetComponent<Character_Script>().character_name + "; Dealing " + Calculate_Damage(convertToDouble(eff.value[0], character), target_character) + " damage and Using " + cost + " AP");
                        if (target_character.GetComponent<Character_Script>().aura_curr == 0)
                        {
                            target_character.GetComponent<Character_Script>().Die();
                        }
                        else
                        {
                            target_character.GetComponent<Character_Script>().aura_curr -= Calculate_Damage(convertToDouble(eff.value[0], character), target_character);
                            if (target_character.GetComponent<Character_Script>().aura_curr < 0)
                            {
                                target_character.GetComponent<Character_Script>().aura_curr = 0;
                                target_character.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                            }
                        }
                    }
                    else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                    {


                    }
                    else if (eff.type.ToString() == Effect.Types.Status.ToString())
                    {


                    }
                    else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                    {


                    }
                    else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                    {

                    }
                    else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                    {
                        target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().state = States.Idle;
                        target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_curr -= target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_cost;
                        print("Character " + target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().character_name + " Passed, Recovering " + target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().AP_RECOVERY + " AP.");
                        target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_cost = 0;
                        if (target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_curr > target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_max)
                        {
                            target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_curr = target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().action_max;
                        }
                        target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().controller.curr_scenario.CleanReachable();
                        target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>().controller.curr_scenario.NextPlayer();
                    }
                }
            }
            if (self_effect != null)
            {
                foreach (Effect eff in self_effect)
                {
                    if (eff.type.ToString() == Effect.Types.Move.ToString())
                    {
                        //value 0 for movement is movement type 1 is standard movement
                        character.GetComponent<Character_Script>().MoveTo(target_tile.transform);
                        character.curr_tile.GetComponent<Tile_Data>().node.traversible = true;
                        character.curr_tile.GetComponent<Tile_Data>().node.obj = null;
                        character.curr_tile = target_tile.transform;
                        character.curr_tile.GetComponent<Tile_Data>().node.traversible = false;
                        character.curr_tile.GetComponent<Tile_Data>().node.obj = character.gameObject;

                    }
                    else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                    {
                        if (character.GetComponent<Character_Script>().aura_curr == 0)
                        {
                            character.GetComponent<Character_Script>().Die();
                        }
                        else
                        {
                            character.GetComponent<Character_Script>().aura_curr -= Calculate_Damage(convertToDouble(eff.value[0], character), character);
                            if (character.GetComponent<Character_Script>().aura_curr < 0)
                            {
                                character.GetComponent<Character_Script>().aura_curr = 0;
                                character.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                            }
                        }
                    }
                    else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                    {
                    }
                    else if (eff.type.ToString() == Effect.Types.Status.ToString())
                    {
                    }
                    else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                    {
                    }
                    else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                    {
                    }
                    else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                    {
                        character.state = States.Idle;
                        character.action_curr -= character.action_cost;
                        print("Character " + character.character_name + " Waited, Recovering " + character.AP_RECOVERY + " AP.");
                        character.action_cost = 0;
                        if (character.action_curr > character.action_max)
                        {
                            character.action_curr = character.action_max;
                        }
                        character.controller.curr_scenario.CleanReachable();
                        character.controller.curr_scenario.NextPlayer();
                    }
                }
            }
            character.state = States.Idle;
            character.controller.curr_scenario.CleanReachable();
            character.controller.curr_scenario.ResetReachable();
            if (character.action_curr <= 0)
            {
                character.action_curr = 0;
                character.action_curr += character.AP_RECOVERY;

                character.controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
                character.controller.curr_scenario.NextPlayer();
            }
            else
            {
                character.FindAction("Move").Select(character);
            }
            if (character.action_curr > character.action_max)
            {
                character.action_curr = character.action_max;
            }

        }

    }

    public class Effect
    {
        public enum Types { Move, Damage, Heal, Status, Elevator, Enabler, Pass }

        public Types type { get; set; }
        public String[] value { get; set; }

        public Effect(String input)
        {
            String type_string= input.Split(' ')[0];
            value = new String[input.Split(' ').Length];
            Array types = Enum.GetValues(typeof(Types));

            foreach (Types ty in types)
            {
                if (type_string.Contains(ty.ToString()))
                {
                    type = ty;
                }
            }
            int x = 0;
            while (x < input.Split(' ').Length - 1)
            {
                value[x] = input.Split(' ')[x+1];
                x++;
            }
        }
    }

    //Constants
    public int AURA_MULTIPLIER = 10;
    public int AP_MULTIPLIER = 5;
    public int AP_MAX = 20;
    public int AP_RECOVERY = 10;
    public int SPEED = 8;
    public int character_id;
    public enum States { Moving, Attacking, Idle, Dead, Blinking, Walking }
    public enum Weapons { Sword, Rifle, Spear, Sniper, Pistol, Claws, Orb, Hammer }
    public enum Armors { Light, Medium, Heavy }
    public enum Character_Stats { aura_max, action_max, canister_max, strength, coordination, spirit, dexterity, vitality, speed };

    //Variables
    public int character_num { get; set; }
    public string character_name { get; set; }
    public int aura_max { get; set; }
    public int aura_curr { get; set; }
    public int action_max { get; set; }
    public int action_curr { get; set; }
    public int action_cost { get; set; }
    public int canister_max { get; set; }
    public int canister_curr { get; set; }
    public int strength { get; set; }
    public int coordination { get; set; }
    public int spirit { get; set; }
    public int dexterity { get; set; }
    public int vitality { get; set; }
    public double speed { get; set; }
    public int level { get; set; }
    public Weapon weapon { get; set; }
    public Armor armor { get; set; }
    public Accessory[] accessories { get; set; }
    public static List<Action> all_actions { get; set; }
    public List<Action> actions { get; set; }
    public Action curr_action { get; set; }
    public States state { get; set; }
    public Game_Controller controller { get; set; }
    public Transform curr_tile { get; set; }

    public class Equipment
    {
        public string name;
        public enum Equipment_Type { Weapon, Armor, Accessory };
        public Equipment_Type type;
        public String[] actions;
        public Effect[] effects; 
        public int durability;
        public double weight;
        public int armor;
        public SpriteRenderer sprite;

        public class Effect
        {
            public Character_Stats stat;
            public int effect;

            public Effect(Character_Stats st, int eff)
            {
                stat = st;
                effect = eff;
            }
        }
    }

    public class Armor: Equipment
    {
        public Armor(string str)
        {
            type = Equipment_Type.Armor;
            durability = 100;
            switch (str)
            {
                case "Light":
                    name = Armors.Light.ToString();
                    armor = -1;
                    weight = 0;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.speed, 1);
                    effects[1] = new Effect(Character_Stats.dexterity, 1);
                    actions = new String[1];
                    actions[0] = "Blink";
                    //actions[0] = all_actions;
                    break;
                case "Medium":
                    name = Armors.Medium.ToString();
                    armor = 2;
                    weight = 1;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.strength, 1);
                    effects[1] = new Effect(Character_Stats.coordination, 1);
                    break;
                case "Heavy":
                    name = Armors.Heavy.ToString();
                    armor = 5;
                    weight = 2;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.vitality, 1);
                    effects[1] = new Effect(Character_Stats.spirit, 1);
                    actions = new String[1];
                    actions[0] = "Channel";
                    break;
            }
        }

        public Armor(Armors ar)
        {
            type = Equipment_Type.Armor;
            durability = 100;
            switch (ar)
            {
                case Armors.Light:
                    name = Armors.Light.ToString();
                    armor = -1;
                    weight = 0;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.speed, 1);
                    effects[1] = new Effect(Character_Stats.dexterity, 1);
                    actions = new String[1];
                    actions[0] = "Blink";
                    break;
                case Armors.Medium:
                    name = Armors.Medium.ToString();
                    armor = 2;
                    weight = 1;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.strength, 1);
                    effects[1] = new Effect(Character_Stats.coordination, 1);
                    break;
                case Armors.Heavy:
                    name = Armors.Heavy.ToString();
                    armor = 5;
                    weight = 2;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.vitality, 1);
                    effects[1] = new Effect(Character_Stats.spirit, 1);
                    actions = new String[1];
                    actions[0] = "Channel";
                    break;
            }
        }
    }

    public class Accessory : Equipment
    {
        public Accessory()
        {
            type = Equipment_Type.Accessory;
            durability = 100;
        }
    }

    public class Weapon: Equipment{
        public int range;
        public int attack;
        public bool ranged;

        public Weapon(string str)
        {
            type = Equipment_Type.Weapon;
            durability = 100;
            switch (str)
            {
                case "Sword":
                    name = Weapons.Sword.ToString();
                    range = 1;
                    attack = 2;
                    weight = 0.5;
                    ranged = false;
                    break;
                case "Rifle":
                    name = Weapons.Rifle.ToString();
                    range = 4;
                    attack = 3;
                    ranged = true;
                    weight = 1;
                    break;
                case "Spear":
                    name = Weapons.Spear.ToString();
                    range = 2;
                    attack = 2;
                    ranged = false;
                    weight = 1;
                    break;
                case "Sniper":
                    name = Weapons.Sniper.ToString();
                    range = 6;
                    attack = 5;
                    ranged = true;
                    weight = 3;
                    break;
                case "Pistol":
                    name = Weapons.Pistol.ToString();
                    range = 3;
                    attack = 2;
                    ranged = true;
                    weight = 0.5;
                    break;
                case "Claws":
                    name = Weapons.Claws.ToString();
                    range = 1;
                    attack = 10;
                    ranged = false;
                    weight = 4;
                    break;
                default:
                    break;
            }
        }

        public Weapon(Weapons wep)
        {
            type = Equipment_Type.Weapon;
            durability = 100;
            switch (wep)
            {
                case Weapons.Sword:
                    name = Weapons.Sword.ToString();
                    range = 1;
                    attack = 2;
                    weight = 0.5;
                    ranged = false;
                    break;
                case Weapons.Rifle:
                    name = Weapons.Rifle.ToString();
                    range = 4;
                    attack = 3;
                    ranged = true;
                    weight = 1;
                    break;
                case Weapons.Spear:
                    name = Weapons.Spear.ToString();
                    range = 2;
                    attack = 2;
                    ranged = false;
                    weight = 1;
                    break;
                case Weapons.Sniper:
                    name = Weapons.Sniper.ToString();
                    range = 6;
                    attack = 5;
                    ranged = true;
                    weight = 3;
                    break;
                case Weapons.Pistol:
                    name = Weapons.Pistol.ToString();
                    range = 3;
                    attack = 2;
                    ranged = true;
                    weight = 0.5;
                    break;
                case Weapons.Claws:
                    name = Weapons.Claws.ToString();
                    range = 1;
                    attack = 10;
                    ranged = false;
                    weight = 4;
                    break;
                default:
                    break;
            }
        }
    }

    //Methods
	// Use this for initialization
	void Start ()
    {

        //gameObject.SetActive(true);
    }

    public Character_Script(string nm, int lvl, int str, int crd, int spt, int dex, int vit, int spd, int can, string wep, string arm)
    {
        controller = Game_Controller.controller;
        character_name = nm.TrimStart();
        level = lvl;
        strength = str;
        coordination = crd;
        spirit = spt;
        dexterity = dex;
        vitality = vit;
        speed = spd;
        aura_max = vitality * AURA_MULTIPLIER;
        aura_curr = aura_max;
        action_max = AP_MAX;
        action_curr = action_max;
        action_cost = 0;
        actions = new List<Action>();
        canister_max = can;
        canister_curr = canister_max;
        state = States.Idle;
        all_actions = Action.LoadActions();
        actions.Add(FindAction("Move"));
        actions.Add(FindAction("Attack"));
        actions.Add(FindAction("Wait"));
        foreach (Weapons weps in Enum.GetValues(typeof(Weapons)))
        {
            if (wep.TrimStart() == weps.ToString())
            {
                Weapon w = new Weapon(weps);
                Equip(w);
                break;
            }
        }
        foreach (Armors arms in Enum.GetValues(typeof(Armors)))
        {
            if (arm.TrimStart() == arms.ToString())
            {
                Armor a = new Armor(arms);
                Equip(a);
                break;
            }
        }
        //foreach (string s in acc)
        //{
        //    foreach (Weapons weps in Enum.GetValues(typeof(Weapons)))
        //    {
        //        if (wep == weps.ToString())
        //        {
        //            Weapon w = new Weapon(weps);
        //            Equip(w);
        //            break;
        //        }
        //    }
        //}
    }

    public Action FindAction(String name)
    {
        if (all_actions != null)
        {
            //TODO Optimize search to use binary search
            foreach (Action act in all_actions)
            {
                if (act.name == name)
                {
                    return act;
                }
            }
        }
        return null;
    }

    public void Act(Transform target_tile)
    {
        curr_action.Enact(this, target_tile.gameObject);
    }

    public void Equip(Equipment e)
    {
        switch (e.type)
        {
            case Equipment.Equipment_Type.Weapon:
                weapon = (Weapon)e;
                break;
            case Equipment.Equipment_Type.Accessory:
                accessories[0] = (Accessory)e;
                break;
            case Equipment.Equipment_Type.Armor:
                armor = (Armor)e;
                break;
            default:
                break;
        }
        if (e.effects != null)
        {
            foreach (Equipment.Effect eff in e.effects)
            {
                switch (eff.stat)
                {
                    case Character_Stats.action_max:
                        action_max += eff.effect * AP_MULTIPLIER;
                        action_curr += eff.effect * AP_MULTIPLIER;
                        break;
                    case Character_Stats.aura_max:
                        aura_max += eff.effect * AURA_MULTIPLIER;
                        aura_max += eff.effect * AURA_MULTIPLIER;
                        break;
                    case Character_Stats.canister_max:
                        canister_max += eff.effect;
                        break;
                    case Character_Stats.coordination:
                        coordination += eff.effect;
                        break;
                    case Character_Stats.dexterity:
                        dexterity += eff.effect;
                        break;
                    case Character_Stats.speed:
                        speed += eff.effect;
                        break;
                    case Character_Stats.spirit:
                        spirit += eff.effect;
                        action_max += eff.effect * AP_MULTIPLIER;
                        action_curr += eff.effect * AP_MULTIPLIER;
                        break;
                    case Character_Stats.strength:
                        strength += eff.effect;
                        break;
                    case Character_Stats.vitality:
                        vitality += eff.effect;
                        aura_max += eff.effect * AURA_MULTIPLIER;
                        aura_curr += eff.effect * AURA_MULTIPLIER;
                        break;
                }
            }
        }
        /*speed -= e.weight;
        if( speed <= 0)
        {
            speed = 1;
        }*/
        if (e.actions != null)
        {
            foreach(String str in e.actions)
            {
                actions.Add(FindAction(str));
            }
        }
    }

    public static int Calculate_Damage(double damage, Character_Script target)
    {
        return (int)(damage - target.armor.armor);
    }

    public void Randomize(){
        //Randomize stats
		strength = UnityEngine.Random.Range (1,7);
		coordination = UnityEngine.Random.Range (1, 7);
		spirit = UnityEngine.Random.Range (1, 7);
		dexterity = UnityEngine.Random.Range (1, 7);
		vitality = UnityEngine.Random.Range (1, 7);

        //Formulas
        speed = SPEED;
        aura_max = vitality * AURA_MULTIPLIER;
        aura_curr = aura_max;
        action_max = AP_MAX;// spirit * AP_MULTIPLIER;
        action_curr = action_max;
        actions = new List<Action>();
        canister_max = UnityEngine.Random.Range(0, 3);
        canister_curr = canister_max;
		state = States.Idle;

        //Randomize Equipment
        int w = UnityEngine.Random.Range(0, 5);
        Weapon wep;
        if (w == 0)
        {
            wep = new Weapon(Weapons.Sword);
        } else if (w == 1)
        {
            wep = new Weapon(Weapons.Rifle);
        }
        else if (w == 2)
        {
            wep = new Weapon(Weapons.Spear);
        }
        else if (w == 3)
        {
            wep = new Weapon(Weapons.Sniper);
        }
        else if (w == 4)
        {
            wep = new Weapon(Weapons.Pistol);
        }
        else
        {
            wep = new Weapon(Weapons.Claws);
        }
        Equip(wep);
        int a = UnityEngine.Random.Range(0, 3);
        Armor ar;
        if (a == 0)
        {
            ar = new Armor(Armors.Light);
        }
        else if (a == 1)
        {
            ar = new Armor(Armors.Medium);
        }
        else
        {
            ar = new Armor(Armors.Heavy);
        }
        Equip(ar);
		level = 1;
		character_name = "Character " + character_num;
		controller = Game_Controller.controller;
        //FindReachable(controller.tile_grid, weapon.range);
        //FindReachable(controller.GetComponent<Game_Controller>().tile_grid,dexterity);
	}

    public void Die()
    {
        state = States.Dead;
        //reset the tile traversible state
        curr_tile.GetComponent<Tile_Data>().node.traversible = true;

        //remove the character from the turn order and character list
        Debug.Log("Character num: " + character_num + " has died");
        Debug.Log("Characters remaining: " + controller.curr_scenario.characters.Count);
        controller.curr_scenario.characters.Remove(transform.gameObject);
        controller.curr_scenario.turn_order.Remove(transform.gameObject);
        controller.curr_scenario.curr_character_num -= 1;
        if (controller.curr_scenario.curr_character_num < 0)
        {
            controller.curr_scenario.curr_character_num = controller.curr_scenario.characters.Count - 1;
        }

        //remove the character from the board
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        Destroy(this.gameObject);

    }

    public IEnumerator MoveOverTime(Tile_Data.Node prev_tile, Tile_Data.Node temp_tile)
    {
        float elapsedTime = 0;
        float duration = 2;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.x, 
                (float)(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.y + 
                (controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z), 
                new Vector3(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.x, 
                (float)(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.y + 
                (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z), 
                elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    //This is a wrapper script for Move. We can't call Move from Scenario because it is not attached to any object in Unity.
    //Since it was created using New rather than AddComponent() it exists in the C backend but Unity doesn't know it.
    public void MoveTo(Transform clicked_tile)
    {
        StartCoroutine(Move(clicked_tile));
        //clicked_tile.GetComponent<Tile_Data>().node.obj = this.gameObject;
    }

    public IEnumerator Move(Transform clicked_tile)
    {
        state = States.Walking;
        Stack<Tile_Data.Node> path = new Stack<Tile_Data.Node>();
        //path = controller.navmesh.shortestPath(curr_tile.GetComponent<Tile_Data>().node, clicked_tile.GetComponent<Tile_Data>().node, action_curr, SPEED);
        //Tile_Data.Node temp_tile;
        //temp_tile = path.Pop();
        //Tile_Data.Node prev_tile;
        Tile_Data.Node temp_tile = clicked_tile.GetComponent<Tile_Data>().node;
        Tile_Data.Node prev_tile = curr_tile.GetComponent<Tile_Data>().node;
        action_cost = temp_tile.weight;
        //action_cost = (int)clicked_tile.GetComponent<Tile_Data>().node.weight;//Math.Abs(curr_tile.GetComponent<Tile_Data>().x_index - clicked_tile.GetComponent<Tile_Data>().x_index) * (int)(armor.weight + weapon.weight) +Math.Abs(curr_tile.GetComponent<Tile_Data>().y_index - clicked_tile.GetComponent<Tile_Data>().y_index) * (int)(armor.weight + weapon.weight) + (clicked_tile.GetComponent<Tile_Data>().node.height - curr_tile.GetComponent<Tile_Data>().node.height)*2;
        if (action_cost < 1)
        {
            action_cost = 1;
        }
        if (action_cost > action_curr)
        {
            action_cost = action_curr;
        }
        print("Character " + character_name + " Moved from: " + curr_tile.GetComponent<Tile_Data>().x_index + "," + curr_tile.GetComponent<Tile_Data>().y_index + " to: " + clicked_tile.GetComponent<Tile_Data>().x_index + "," + clicked_tile.GetComponent<Tile_Data>().y_index + " Using " + action_cost + " AP");
        action_curr -= action_cost;
        action_cost = 0;

        if (gameObject.CompareTag("Player"))
        {
            //Tile_Data.Node temp_tile = clicked_tile.GetComponent<Tile_Data>().node;
            //Tile_Data.Node prev_tile = curr_tile.GetComponent<Tile_Data>().node;
            //Stack<Tile_Data.Node> path = new Stack<Tile_Data.Node>();

            //Construct a stack that is a path from the clicked tile to the source.
            while(!(temp_tile.id[0] == curr_tile.GetComponent<Tile_Data>().node.id[0] && temp_tile.id[1] == curr_tile.GetComponent<Tile_Data>().node.id[1]))
            {
                path.Push(temp_tile);
                //Look at the parent tile.
                temp_tile = temp_tile.parent;
            }
            
            //distances.Push(distance);
            //Debug.Log("temp_tile.id[0]: " + temp_tile.id[0]);
            //Debug.Log("temp_tile.id[1]: " + temp_tile.id[1]);
            //Debug.Log("curr_tile.id[0]: " + curr_tile.GetComponent<Tile_Data>().node.id[0]);
            //Debug.Log("curr_tile.id[0]: " + curr_tile.GetComponent<Tile_Data>().node.id[1]);
            //Navigate the path by popping tiles out of the stack.
            while (path.Count != 0)
            {

                temp_tile = path.Pop();
                //transform.position = new Vector3(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.x, (float)(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.y + (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
                //
                //yield return new WaitForSeconds(.3f);

                float elapsedTime = 0;
                float duration = .3f;
                //print("duration: " +duration);
                while (elapsedTime < duration)
                {
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.x, 
                        (float)(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.y + 0.25f * (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height) + 1.145f),
                        controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.z), 
                        new Vector3(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.x, 
                        (float)(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.y + 0.25f* (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height) + 1.145f),
                        controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.z), 
                        elapsedTime/duration);
                    //(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f
                    elapsedTime += Time.deltaTime;
                    /*if (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sortingOrder > curr_tile.GetComponent<SpriteRenderer>().sortingOrder)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingOrder = controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingOrder = curr_tile.GetComponent<SpriteRenderer>().sortingOrder+1;
                    }*/
                    yield return new WaitForEndOfFrame();
                }
                //gameObject.GetComponent<SpriteRenderer>().sortingOrder = controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sortingOrder + 1;
                prev_tile = temp_tile;



                //    new Vector3(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.x, (float)(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.y + (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
                //WaitForSeconds(1);
            }
            //transform.position = new Vector3(curr_tile.position.x, (float)(curr_tile.position.y + (curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
        }
        else
        {
            temp_tile = clicked_tile.GetComponent<Tile_Data>().node;
            prev_tile = curr_tile.GetComponent<Tile_Data>().node;
            path = new Stack<Tile_Data.Node>();
            //Construct a stack that is a path from the clicked tile to the source.
            while (!(temp_tile.id[0] == curr_tile.GetComponent<Tile_Data>().node.id[0] && temp_tile.id[1] == curr_tile.GetComponent<Tile_Data>().node.id[1]))
            {
                path.Push(temp_tile);
                temp_tile = temp_tile.parent;
            }
            //Debug.Log("temp_tile.id[0]: " + temp_tile.id[0]);
            //Debug.Log("temp_tile.id[1]: " + temp_tile.id[1]);
            //Debug.Log("curr_tile.id[0]: " + curr_tile.GetComponent<Tile_Data>().node.id[0]);
            //Debug.Log("curr_tile.id[0]: " + curr_tile.GetComponent<Tile_Data>().node.id[1]);
            //Navigate the path by popping tiles out of the stack.
            while (path.Count != 0)
            {

                temp_tile = path.Pop();
                float elapsedTime = 0;
                float duration = .3f;
                //print("duration: " + duration);
                while (elapsedTime < duration)
                {
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.x, (float)(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.y + 0.25f * (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height) + 1.45f), controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.z), new Vector3(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.x, (float)(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.y + 0.25f * (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height) + 1.45f), controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.z), elapsedTime / duration);
                    elapsedTime += Time.deltaTime;
                    /*if (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sortingOrder > curr_tile.GetComponent<SpriteRenderer>().sortingOrder)
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingOrder = controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }
                    else
                    {
                        gameObject.GetComponent<SpriteRenderer>().sortingOrder = curr_tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
                    }*/
                    yield return new WaitForEndOfFrame();
                }
                //gameObject.GetComponent<SpriteRenderer>().sortingOrder = controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<SpriteRenderer>().sortingOrder + 1;
                prev_tile = temp_tile;
            }
            //transform.position = new Vector3(curr_tile.position.x, (float)(curr_tile.position.y + (curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z); 
        }
        //renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();

        //gameObject.GetComponent<SpriteRenderer>().sortingOrder = clicked_tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        /*state = States.Idle;
        controller.curr_scenario.ResetReachable();
        controller.curr_scenario.FindReachable(action_curr, SPEED);
        controller.curr_scenario.MarkReachable();
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.curr_scenario.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();*/

    }

    public void Attack(GameObject character)
    {
        
    }

    public void Blink (Transform clicked_tile)
    {
        action_cost = Math.Abs(curr_tile.GetComponent<Tile_Data>().x_index - clicked_tile.GetComponent<Tile_Data>().x_index) + Math.Abs(curr_tile.GetComponent<Tile_Data>().y_index - clicked_tile.GetComponent<Tile_Data>().y_index);
        if (action_cost < 1)
        {
            action_cost = 1;
        }
        if (action_cost > action_curr)
        {
            action_cost = action_curr;
        }
        print("Character " + character_name + " Blinked from: " + curr_tile.GetComponent<Tile_Data>().x_index + "," + curr_tile.GetComponent<Tile_Data>().y_index + " to: " + clicked_tile.GetComponent<Tile_Data>().x_index + "," + clicked_tile.GetComponent<Tile_Data>().y_index + " Using " + action_cost + " AP");
        action_curr -= action_cost;
        action_cost = 0;
        curr_tile.GetComponent<Tile_Data>().node.traversible = true;
        curr_tile = clicked_tile;
        curr_tile.GetComponent<Tile_Data>().node.traversible = false;
        if (gameObject.CompareTag("Player"))
        {
            transform.position = new Vector3(curr_tile.position.x, (float)(curr_tile.position.y + (curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
        }
        else
        {
            transform.position = new Vector3(curr_tile.position.x, (float)(curr_tile.position.y + (curr_tile.GetComponent<SpriteRenderer>().sprite.rect.height) / 100) + 0.15f, curr_tile.position.z);
        }
        //renderer = (SpriteRenderer)curr_tile.GetComponent<SpriteRenderer> ();
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = clicked_tile.GetComponent<SpriteRenderer>().sortingOrder + 1;
        state = States.Idle;
        controller.curr_scenario.ResetReachable();
        controller.curr_scenario.FindReachable(action_curr, SPEED);
        controller.curr_scenario.MarkReachable();
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.curr_scenario.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
    }

    public void Channel()
    {
        print("Character " + character_name + " Channeled from: " + aura_curr + ", Using " + action_cost + " AP");
        action_curr -= action_cost;
        action_cost = 0;
        aura_curr += 10;
        state = States.Idle;
        if (aura_curr > aura_max)
        {
            aura_curr = aura_max;
        }
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.curr_scenario.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
    }

    public Character_Script(){
	}
	
	// Update is called once per frame
	void Update () {

        if (aura_curr < 0) {
			aura_curr = 0;
		}
        if (aura_curr > aura_max)
        {
            aura_curr = aura_max;
        }
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.curr_scenario.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
    }
}
