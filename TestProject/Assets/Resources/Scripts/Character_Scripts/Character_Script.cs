using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Character_Script : MonoBehaviour {

    public class Action
    {
        public static string PLAYER_ACTIONS_FILE = "Assets/Resources/Actions/Action_List.txt";
        public static int HEADINGS = 13;
        public enum Activation_Types { Active, Passive, Reactive }
        public enum Origin_Types { Innate, Soul, Weapon }
        public enum Accepted_Shortcuts { AUM, AUC, APM, APC, MPM, MPC, CAM, CAC, SPD, STR, CRD, SPT, DEX, VIT, LVL, WPR, WPD, ARM, WGT, MOC, DST, NUL }
        
        /// <summary>
        /// AUM = Char Aura MAX
        /// AUC = Char Aura Current
        /// APM = Action Point Max
        /// APC = Action Point Curr
        /// MPM = Mana Point Max
        /// MPC = Mana Point Curr
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
        public String ap_cost;
        //Cost in MP
        public String mp_cost;
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
                        case "ap_cost":
                            act.ap_cost = values.Trim();
                            break;
                        case "mp_cost":
                            act.mp_cost = values.Trim();
                            break;
                        case "range":
                            act.range = values.Trim();
                            break;
                        case "area":
                            act.area = values.TrimStart().TrimEnd();
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

        public static List<Action> Load_Actions()
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

        public double Convert_To_Double(string input, Character_Script obj)
        {
            double output = 0.0;
            if (double.TryParse(input, out output))
            {
                return output;
            }
            else
            {
                //Remove acronyms from equation
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
                        else if (val.ToString() == "APM")
                        {
                            input = input.Replace(val.ToString(), "" + obj.action_max);
                        }
                        else if (val.ToString() == "APC")
                        {
                            input = input.Replace(val.ToString(), "" + obj.action_curr);
                        }
                        else if (val.ToString() == "MPM")
                        {
                            input = input.Replace(val.ToString(), "" + obj.mana_max);
                        }
                        else if (val.ToString() == "MPC")
                        {
                            input = input.Replace(val.ToString(), "" + obj.mana_curr);
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
                }
                //try to convert to double after converting
                if (double.TryParse(input, out output))
                {
                    return output;
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
                return -1.0;
            }

        }

        public double Parse_Equation(string input)
        {
            //Debug.Log("Parsing: " + input);
            //base case, can we convert to double?
            double output;
            if(double.TryParse(input, out output))
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
                    string[] split = input.Split(new char[] { '+' }, 2 );
                    return Parse_Equation(""+(Parse_Equation(split[0]) + Parse_Equation(split[1])));
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
                //resolve exponents
                if (input.Contains("^"))
                {
                    string[] split = input.Split(new char[] { '^' }, 2);
                    return Parse_Equation("" + (Mathf.Pow((float)Parse_Equation(split[0]), (float)Parse_Equation(split[1]))));
                }
            }
            return output;
        }

        public void Select(Character_Script character)
        {

            //Check to see if action is enabled
            //Debug.Log("Name: " + name + " is enabled: " + enabled);
            if (this.enabled)
            {
                //Debug.Log("AP cost: " + (int)Convert_To_Double(cost, character));
                //Check to see if player can afford action:
                if (character.action_curr >= (int)Convert_To_Double(ap_cost, character))
                {
                    //Debug.Log("Enough action points");
                    if (character.mana_curr >= (int)Convert_To_Double(mp_cost, character))
                    {
                        //Debug.Log("Enough mana points");
                        character.curr_action = this;
                        if (target_effect != null)
                        {
                            foreach (Effect eff in target_effect)
                            {
                                if (eff.type.ToString() == Effect.Types.Move.ToString())
                                {
                                    character.state = States.Moving;
                                    character.controller.curr_scenario.FindReachable((int)character.speed, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Status.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                            }
                        }
                        if (self_effect != null)
                        {
                            foreach (Effect eff in self_effect)
                            {
                                if (eff.type.ToString() == Effect.Types.Move.ToString())
                                {
                                    if (Convert_To_Double(eff.value[0], character) != 4)
                                    {
                                        character.state = States.Moving;
                                        //Debug.Log("Speed: " + character.speed);
                                        character.controller.curr_scenario.FindReachable((int)character.speed, (int)Convert_To_Double(range, character));
                                    }
                                    else
                                    {
                                        character.state = States.Blinking;
                                        character.controller.curr_scenario.FindReachable((int)character.speed * 2, (int)Convert_To_Double(range, character));
                                    }
                                }
                                else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));

                                }
                                else if (eff.type.ToString() == Effect.Types.Status.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                                {
                                    character.state = States.Attacking;
                                    character.controller.curr_scenario.FindReachable(character.action_curr, (int)Convert_To_Double(range, character));
                                }
                                else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                                {
                                    character.StartCoroutine(character.End_Turn());
                                }
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
                        else if (but.GetComponent<Image>().color == Color.blue)
                        {
                            but.GetComponent<Image>().color = Color.white;
                        }
                    }
                    character.controller.curr_scenario.CleanReachable();
                    character.controller.curr_scenario.MarkReachable();
                    character.curr_action = this;
                }
                else
                {
                    Debug.Log("NOT Enough Action Points");
                    foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
                    {
                        if (but.name == name)
                        {
                            but.GetComponent<Image>().color = Color.red;
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Action is disabled");
                foreach (Transform but in character.controller.action_menu.GetComponent<Action_Menu_Script>().buttons)
                {
                    if (but.name == name)
                    {
                        but.GetComponent<Image>().color = Color.red;
                    }
                }
            }
        }

        public List<GameObject> Get_Targets(Character_Script character, GameObject target_tile)
        {
            List<GameObject> targets = new List<GameObject>();
            String[] area_effect = area.Split(' ');
            if (area_effect[0] == "Self")
            {
                targets.Add(character.curr_tile.GetComponent<Tile_Data>().node.obj);
            }
            else if (area_effect[0] == "Target")
            {
                if (target_tile.GetComponent<Tile_Data>().node.obj != null)
                {
                    targets.Add(target_tile.GetComponent<Tile_Data>().node.obj);
                }
            }
            else if (area_effect[0] == "Cross")
            {
                int x = 0;
                int y = 0;
                if (area_effect[1] == "Self")
                {
                    x = character.curr_tile.GetComponent<Tile_Data>().x_index;
                    y = character.curr_tile.GetComponent<Tile_Data>().y_index;
                }
                if (area_effect[1] == "Target")
                {
                    x = target_tile.GetComponent<Tile_Data>().x_index;
                    y = target_tile.GetComponent<Tile_Data>().y_index;
                }
                int size = 0;
                int.TryParse(area_effect[2], out size);
                for (int i = -size; i <= size; i++)
                {
                    Transform target = character.controller.curr_scenario.tile_grid.getTile(x + i, y);
                    if (target != null)
                    {
                        if (target.GetComponent<Tile_Data>().node.obj != null)
                        {
                            targets.Add(target.GetComponent<Tile_Data>().node.obj);
                        }
                        //avoid duplicates
                        if (i != 0)
                        {
                            target = character.controller.curr_scenario.tile_grid.getTile(x, y + i);
                            if (target != null)
                            {
                                if (target.GetComponent<Tile_Data>().node.obj != null)
                                {
                                    targets.Add(target.GetComponent<Tile_Data>().node.obj);
                                }
                            }
                        }
                    }
                }

            }
            else if (area_effect[0] == "Ring")
            {
                int x = 0;
                int y = 0;
                if (area_effect[1] == "Self")
                {
                    x = character.curr_tile.GetComponent<Tile_Data>().x_index;
                    y = character.curr_tile.GetComponent<Tile_Data>().y_index;
                }
                if (area_effect[1] == "Target")
                {
                    x = target_tile.GetComponent<Tile_Data>().x_index;
                    y = target_tile.GetComponent<Tile_Data>().y_index;
                }
                int ringsize = 0;
                int gapsize = 0;
                int.TryParse(area_effect[2], out ringsize);
                int.TryParse(area_effect[3], out gapsize);
                for (int i = -ringsize; i <= ringsize; i++)
                {
                    for (int j = -ringsize; j <= ringsize; j++)
                    {
                        if (Math.Abs(i) > gapsize || Math.Abs(j) > gapsize)
                        {
                            Transform target = character.controller.curr_scenario.tile_grid.getTile(x + i, y + j);
                            if (target != null)
                            {
                                if (target.GetComponent<Tile_Data>().node.obj != null)
                                {
                                    targets.Add(target.GetComponent<Tile_Data>().node.obj);
                                }
                            }
                        }
                    }

                }
            }
            if (targets.Count > 0)
            {
                return targets;
            }
            else
            {
                return null;
            }
        }

        public bool Check_Valid(Character_Script character, GameObject target_tile)
        {
            if (target_effect != null)
            {
                foreach (Effect eff in target_effect)
                {
                    if (eff.type.ToString() == Effect.Types.Move.ToString())
                    {

                        //target_character.MoveTo(target_tile.transform);
                    }
                    else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                    {
                        if (Get_Targets(character, target_tile) == null)
                        {
                            Debug.Log("Invalid tile selected");
                            return false;
                        }
                    }
                    else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                    {
                        if (target_tile.GetComponent<Tile_Data>().node.obj == null)
                        {
                            Debug.Log("Invalid tile selected");
                            return false;
                        }
                    }
                    else if (eff.type.ToString() == Effect.Types.Status.ToString())
                    {
                        if (target_tile.GetComponent<Tile_Data>().node.obj == null)
                        {
                            Debug.Log("Invalid tile selected");
                            return false;
                        }
                    }
                    else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                    {
                    }
                    else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                    {
                    }
                    else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                    {
                    }
                }
            }
            if (self_effect != null)
            {
                foreach (Effect eff in self_effect)
                {
                    if (eff.type.ToString() == Effect.Types.Move.ToString())
                    {
                        if (target_tile.GetComponent<Tile_Data>().node.obj != null)
                        {
                            Debug.Log("Invalid tile selected");
                            return false;
                        }
                    }
                    else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                    {
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
                    }
                }
            }
            //Debug.Log("Valid tile selected");
            return true;
        }

        public void Enact(Character_Script character, GameObject target_tile)
        {
            if (Check_Valid(character, target_tile))
            {
                if (target_effect != null)
                {
                    foreach (Effect eff in target_effect)
                    {
                        if (eff.type.ToString() == Effect.Types.Move.ToString())
                        {

                            //target_character.MoveTo(target_tile.transform);
                        }
                        else if (eff.type.ToString() == Effect.Types.Damage.ToString())
                        {
                            List<GameObject> targets = Get_Targets(character, target_tile);
                            foreach (GameObject target in targets)
                            {
                                Character_Script target_character = target.GetComponent<Character_Script>();
                                Debug.Log("Character " + character.character_name + " Attacked: " + target_character.GetComponent<Character_Script>().character_name + "; Dealing " + Calculate_Damage(Convert_To_Double(eff.value[0], character), target_character) + " damage and Using " + ap_cost + " AP");
                                if (target_character.GetComponent<Character_Script>().aura_curr == 0)
                                {
                                    target_character.GetComponent<Character_Script>().Die();
                                }
                                else
                                {
                                    target_character.GetComponent<Character_Script>().aura_curr -= Calculate_Damage(Convert_To_Double(eff.value[0], character), target_character);
                                    if (target_character.GetComponent<Character_Script>().aura_curr < 0)
                                    {
                                        target_character.GetComponent<Character_Script>().aura_curr = 0;
                                        target_character.GetComponent<SpriteRenderer>().color = Color.red;
                                    }
                                }
                            }
                        }
                        else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                        {
                            Character_Script target_character = target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>();
                            if (target_character.GetComponent<Character_Script>().aura_curr == 0)
                            {
                                target_character.GetComponent<SpriteRenderer>().color = Color.white;
                            }
                            target_character.GetComponent<Character_Script>().aura_curr += (int)Convert_To_Double(eff.value[0], character);
                            if (target_character.GetComponent<Character_Script>().aura_curr > target_character.GetComponent<Character_Script>().aura_max)
                            {
                                target_character.GetComponent<Character_Script>().aura_curr = target_character.GetComponent<Character_Script>().aura_max;
                            }
                        }
                        else if (eff.type.ToString() == Effect.Types.Status.ToString())
                        {


                        }
                        else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                        {


                        }
                        else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                        {
                            Character_Script target_character = target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>();
                            foreach (Action act in target_character.GetComponent<Character_Script>().actions)
                            {
                                //Debug.Log("act.name: " + act.name + ", eff.value: " + eff.value[0]);
                                if (act.name == eff.value[0])
                                {
                                    //Debug.Log("MATCH");
                                    if (eff.value[1] == "false")
                                    {
                                        //Debug.Log("Skill " + act.name + " is disabled.");
                                        act.enabled = false;
                                    }
                                    if (eff.value[1] == "true")
                                    {
                                        //Debug.Log("Skill " + act.name + " is enabled.");
                                        act.enabled = true;
                                    }
                                }
                            }
                        }
                        else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                        {
                            Character_Script target_character = target_tile.GetComponent<Tile_Data>().node.obj.GetComponent<Character_Script>();
                            target_character.StartCoroutine(target_character.End_Turn());
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
                            Debug.Log("Character " + character.name + " Moved from: " + character.curr_tile.GetComponent<Tile_Data>().x_index + "," + character.curr_tile.GetComponent<Tile_Data>().y_index + " to: " + target_tile.GetComponent<Tile_Data>().x_index + "," + target_tile.GetComponent<Tile_Data>().y_index + " Using " + target_tile.GetComponent<Tile_Data>().node.weight + " Speed.");
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
                                character.GetComponent<Character_Script>().aura_curr -= Calculate_Damage(Convert_To_Double(eff.value[0], character), character);
                                if (character.GetComponent<Character_Script>().aura_curr < 0)
                                {
                                    character.GetComponent<Character_Script>().aura_curr = 0;
                                    character.GetComponent<SpriteRenderer>().color = Color.red;
                                }
                            }
                        }
                        else if (eff.type.ToString() == Effect.Types.Heal.ToString())
                        {
                            if (character.GetComponent<Character_Script>().aura_curr == 0)
                            {
                                character.GetComponent<SpriteRenderer>().color = Color.white;
                            }
                            character.GetComponent<Character_Script>().aura_curr += (int)Convert_To_Double(eff.value[0], character);
                            if (character.GetComponent<Character_Script>().aura_curr > character.GetComponent<Character_Script>().aura_max)
                            {
                                character.GetComponent<Character_Script>().aura_curr = character.GetComponent<Character_Script>().aura_max;
                            }
                        }
                        else if (eff.type.ToString() == Effect.Types.Status.ToString())
                        {
                        }
                        else if (eff.type.ToString() == Effect.Types.Elevator.ToString())
                        {
                        }
                        else if (eff.type.ToString() == Effect.Types.Enabler.ToString())
                        {
                            foreach (Action act in character.GetComponent<Character_Script>().actions)
                            {
                                //Debug.Log("act.name: " + act.name + ", eff.value: " + eff.value[0]);
                                if (act.name == eff.value[0])
                                {
                                    //Debug.Log("MATCH");
                                    if (eff.value[1] == "false")
                                    {
                                        //Debug.Log("Skill " + act.name + " is disabled.");
                                        act.enabled = false;
                                    }
                                    if (eff.value[1] == "true")
                                    {
                                        //Debug.Log("Skill " + act.name + " is enabled.");
                                        act.enabled = true;
                                    }
                                }
                            }
                        }
                        else if (eff.type.ToString() == Effect.Types.Pass.ToString())
                        {
                            character.StartCoroutine(character.End_Turn());
                        }
                    }
                }
                character.action_curr -= (int)character.curr_action.Convert_To_Double(ap_cost, character);
                character.mana_curr -= (int)character.curr_action.Convert_To_Double(mp_cost, character);
                //character.state = States.Idle;
                character.controller.curr_scenario.CleanReachable();
                character.controller.curr_scenario.ResetReachable();
                if (character.action_curr <= 0)
                {
                    character.StartCoroutine(character.End_Turn());
                }
                else
                {
                    character.controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
                    character.Find_Action("Move").Select(character);
                }
                if (character.action_curr > character.action_max)
                {
                    character.action_curr = character.action_max;
                }
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
            String type_string= input.TrimStart().Split(' ')[0];
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
    public int MP_MULTIPLIER = 5;
    public int MP_RECOVERY = 5;
    public int AP_MAX = 2;
    //public int AP_RECOVERY = 10;
    public int SPEED = 6;
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
    public int mana_max { get; set; }
    public int mana_curr { get; set; }
    public int reaction_max { get; set; }
    public int reaction_curr { get; set; }
    public int canister_max { get; set; }
    public int canister_curr { get; set; }
    public int strength { get; set; }
    public int coordination { get; set; }
    public int spirit { get; set; }
    public int dexterity { get; set; }
    public int vitality { get; set; }
    public double speed { get; set; }
    public int level { get; set; }
    public int orientation { get; set; }
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
                    actions = new String[3];
                    actions[0] = "Blink";
                    actions[1] = "Cross";
                    actions[2] = "Ring";
                    break;
                case "Medium":
                    name = Armors.Medium.ToString();
                    armor = 2;
                    weight = 1;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.strength, 1);
                    effects[1] = new Effect(Character_Stats.coordination, 1);
                    actions = new String[2];
                    actions[0] = "Cross";
                    actions[1] = "Ring";
                    break;
                case "Heavy":
                    name = Armors.Heavy.ToString();
                    armor = 5;
                    weight = 2;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.vitality, 1);
                    effects[1] = new Effect(Character_Stats.spirit, 1);
                    actions = new String[3];
                    actions[0] = "Channel";
                    actions[1] = "Cross";
                    actions[2] = "Ring";
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
                    actions = new String[3];
                    actions[0] = "Blink";
                    actions[1] = "Cross";
                    actions[2] = "Ring";
                    break;
                case Armors.Medium:
                    name = Armors.Medium.ToString();
                    armor = 2;
                    weight = 1;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.strength, 1);
                    effects[1] = new Effect(Character_Stats.coordination, 1);
                    actions = new String[2];
                    actions[0] = "Cross";
                    actions[1] = "Ring";
                    break;
                case Armors.Heavy:
                    name = Armors.Heavy.ToString();
                    armor = 5;
                    weight = 2;
                    effects = new Effect[2];
                    effects[0] = new Effect(Character_Stats.vitality, 1);
                    effects[1] = new Effect(Character_Stats.spirit, 1);
                    actions = new String[3];
                    actions[0] = "Channel";
                    actions[1] = "Cross";
                    actions[2] = "Ring";
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

    public void Orient()
    {
        //Flip sprite based on orientation
        //Debug.Log("Current orientation: " + orientation);
        if (orientation < 2)
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = true;
        }
        else
        {
            this.gameObject.GetComponent<SpriteRenderer>().flipX = false;
        }
        if (orientation == 2 || orientation == 1)
        {
            string object_name = this.gameObject.name;
            this.gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Animations/Controllers/"+object_name+"/"+object_name+ "_Override_S") as AnimatorOverrideController;
        }
        else if (orientation == 3 || orientation == 0)
        {
            string object_name = this.gameObject.name;
            this.gameObject.GetComponent<Animator>().runtimeAnimatorController = Resources.Load("Animations/Controllers/" + object_name + "/" + object_name + "_Override_W") as AnimatorOverrideController;
        }
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
        mana_max = spirit * MP_MULTIPLIER;
        mana_curr = MP_RECOVERY;
        reaction_max = AP_MAX;
        reaction_curr = reaction_max;
        curr_action = null;
        actions = new List<Action>();
        canister_max = can;
        orientation = 2;
        canister_curr = canister_max;
        state = States.Idle;
        all_actions = controller.all_actions;
        actions.Add(Find_Action("Move"));
        actions.Add(Find_Action("Attack"));
        actions.Add(Find_Action("Wait"));
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

    public bool Not_Walking()
    {
        if (state != States.Walking)
        {
            return true;
        }
        return false;
    }

    public IEnumerator End_Turn()
    {
        while(state == States.Walking)
        {
            yield return new WaitForEndOfFrame();
        }
        state = States.Idle;
        Debug.Log("Character " + character_name + " Passed");
        curr_action = null;
        reaction_curr = reaction_max;
        action_curr = action_max;
        mana_curr += MP_RECOVERY;
        if (mana_curr > mana_max)
        {
            mana_curr = mana_max;
        }
        //TODO Fix this later
        foreach (Action act in actions)
        {
            if (!act.enabled)
            {
                act.enabled = true;
            }
        }
        controller.curr_scenario.CleanReachable();
        controller.action_menu.GetComponent<Action_Menu_Script>().resetActions();
        controller.curr_scenario.NextPlayer();
    }

    public Action Find_Action(String name)
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
                        mana_max += eff.effect * MP_MULTIPLIER;
                        //action_curr += eff.effect * MP_MULTIPLIER;
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
                actions.Add(Find_Action(str));
            }
        }
    }

    public static int Calculate_Damage(double damage, Character_Script target)
    {
        //Debug.Log("Damage: " + damage + ", Armor: " + target.armor.armor);
        int dmg = (int)(damage - target.armor.armor);
        if (dmg < 0)
        {
            dmg = 0;
        }
        return dmg;
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
        //reset the tile traversible state and empty the tile
        curr_tile.GetComponent<Tile_Data>().node.traversible = true;
        curr_tile.GetComponent<Tile_Data>().node.obj = null;

        //remove the character from the turn order and character list
        Debug.Log("Character num: " + character_num + " has died");
        //Debug.Log("Characters remaining: " + controller.curr_scenario.characters.Count);
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

    public IEnumerator Move_Over_Time(Tile_Data.Node prev_tile, Tile_Data.Node temp_tile)
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
        state = States.Walking;
        StartCoroutine(Move(clicked_tile));
        //while (state == States.Walking)
        //{
        //    Debug.Log("Walking");
        //}
        //clicked_tile.GetComponent<Tile_Data>().node.obj = this.gameObject;
    }

    public IEnumerator Move(Transform clicked_tile)
    {
        Stack<Tile_Data.Node> path = new Stack<Tile_Data.Node>();
        //path = controller.navmesh.shortestPath(curr_tile.GetComponent<Tile_Data>().node, clicked_tile.GetComponent<Tile_Data>().node, action_curr, SPEED);
        //Tile_Data.Node temp_tile;
        //temp_tile = path.Pop();
        //Tile_Data.Node prev_tile;
        Tile_Data.Node temp_tile = clicked_tile.GetComponent<Tile_Data>().node;
        Tile_Data.Node prev_tile = curr_tile.GetComponent<Tile_Data>().node;
        
        //action_curr -= action_cost;
        //action_cost = 0;

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
                    if(prev_tile.id[0] == temp_tile.id[0] && prev_tile.id[1] > temp_tile.id[1])
                    {
                        orientation = 0;
                    }
                    else if (prev_tile.id[0] < temp_tile.id[0] && prev_tile.id[1] == temp_tile.id[1])
                    {
                        orientation = 1;
                    }
                    else if (prev_tile.id[0] == temp_tile.id[0] && prev_tile.id[1] < temp_tile.id[1])
                    {
                        orientation = 2;
                    }
                    else if (prev_tile.id[0] > temp_tile.id[0] && prev_tile.id[1] == temp_tile.id[1])
                    {
                        orientation = 3;
                    }
                    Orient();
                    transform.position = Vector3.Lerp(new Vector3(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.x-(.1f* controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height), 
                        (float)(controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.y + 0.25f * (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height) + 1.145f),
                        controller.curr_scenario.tile_grid.getTiles()[prev_tile.id[0], prev_tile.id[1]].position.z- (.08f * controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height)), 
                        new Vector3(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.x -(.1f * controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height), 
                        (float)(controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.y + 0.25f* (controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height) + 1.145f),
                        controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].position.z- (.08f * controller.curr_scenario.tile_grid.getTiles()[temp_tile.id[0], temp_tile.id[1]].GetComponent<Tile_Data>().node.height)), 
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
        state = States.Moving;

    }
	
    public Character_Script()
    {

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
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
    }
}
