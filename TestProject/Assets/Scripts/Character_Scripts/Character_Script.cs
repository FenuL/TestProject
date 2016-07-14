﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Character_Script : MonoBehaviour {

    //Constants
    public int AURA_MULTIPLIER = 10;
    public int AP_MULTIPLIER = 5;
    public int AP_RECOVERY = 10;
    public int SPEED = 5;
    public int character_id;
    public enum States { Moving, Attacking, Idle, Dead, Blinking }
    public enum Actions { Move, Attack, Wait, Blink, Channel }
    public enum Weapons { Sword, Rifle, Spear, Sniper, Pistol, Claws }
    public enum Armors { Light, Medium, Heavy }
    public enum Character_Stats { aura_max, action_max, canister_max, strength, coordination, spirit, dexterity, vitality, speed };

    //Variables
    public int character_num { get; set; }
    public string character_name { get; set; }
    public int aura_max { get; set; }
    public int aura_curr { get; set; }
    public int action_max { get; set; }
    public int action_curr { get; set; }
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
    public List<Actions> actions { get; set; }
    public States state { get; set; }
    public Game_Controller controller { get; set; }
    public Transform curr_tile { get; set; }
    public List<Transform> reachable_tiles { get; set; }

    public class Equipment
    {
        public string name;
        public enum Equipment_Type { Weapon, Armor, Accessory };
        public Equipment_Type type;
        public Actions[] actions;
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
                    actions = new Actions[1];
                    actions[0] = Actions.Blink;
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
                    actions = new Actions[1];
                    actions[0] = Actions.Channel;
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
                    actions = new Actions[1];
                    actions[0] = Actions.Blink;
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
                    actions = new Actions[1];
                    actions[0] = Actions.Channel;
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
            actions = new Actions[3];
            actions[0] = Actions.Move;
            actions[1] = Actions.Attack;
            actions[2] = Actions.Wait;
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
            actions = new Actions[3];
            actions[0] = Actions.Move;
            actions[1] = Actions.Attack;
            actions[2] = Actions.Wait;
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
	void Start () {
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
        action_max = spirit * AP_MULTIPLIER;
        action_curr = action_max;
        actions = new List<Actions>();
        canister_max = can;
        canister_curr = canister_max;
        state = States.Idle;
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
        speed -= e.weight;
        if( speed <= 0)
        {
            speed = 1;
        }
        if (e.actions != null)
        {
            foreach(Actions a in e.actions)
            {
                actions.Add(a);
            }
        }
    }

    public int Calculate_Damage(Armor a)
    {
        if (weapon.ranged)
        {
            return weapon.attack + coordination - a.armor;
        }else
        {
            return weapon.attack + strength - a.armor;
        }
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
        action_max = spirit * AP_MULTIPLIER;
        action_curr = action_max;
        actions = new List<Actions>();
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
	
	//public SpriteRenderer renderer;
	public void FindReachable(GameObject grid, double lim){
        int limit = (int)lim;
		reachable_tiles = new List<Transform> ();
		int x_index = curr_tile.GetComponent<Tile_Data> ().x_index;
		int y_index = curr_tile.GetComponent<Tile_Data> ().y_index;
		int i = -limit;
		int j = -limit;
		while ( i <= limit) {
			while (j <= limit) {
				//print ("i " + i);
				//print ("j " + j);
				if (x_index + i  >= 0 && x_index + i < grid.GetComponent<Draw_Tile_Grid>().tile_grid.map_width){
                    if (y_index + j >= 0 && y_index + j < grid.GetComponent<Draw_Tile_Grid>().tile_grid.map_height)
                    {
                        int h = grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().tile_height - 1;
                        //print ("distance " + (Mathf.Abs(i)+ Mathf.Abs(j)));
                        if (Mathf.Abs(i) + Mathf.Abs(j) <= limit) {
                            //print ("tile " + (x_index +i) + ","+ (y_index+ j) + " is reachable");
                            if (state == States.Moving || state == States.Blinking){
								if (grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().traversible){
									reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j));
								}
							}
							if (state == States.Attacking){
                                //print ("scanned x index: " + x_index + i )
                                //Prevent Self-Harm
                                if (i != 0 || j != 0)
                                {
                                    reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j));
                                }
							}

						}
					}
				}
				j += 1;
			}
			j = -limit;
			i += 1;
		}
	}

	public void Action(Actions act){
		if (act == Actions.Move && state != States.Dead) {
			state = States.Moving;
			FindReachable(controller.tile_grid, speed);
			controller.CleanReachable ();
			controller.MarkReachable ();
		}
		if (act == Actions.Attack && state != States.Dead) {
			state = States.Attacking;
			FindReachable(controller.tile_grid, weapon.range);
			controller.CleanReachable ();
			controller.MarkReachable ();
		}
        if (act == Actions.Wait && state != States.Dead)
        {
            state = States.Idle;
            action_curr += AP_RECOVERY;
            if (action_curr > action_max)
            {
                action_curr = action_max;
            }
            print("Character " + character_name + " Waited, Recovering 10AP.");
            controller.CleanReachable();
            controller.NextPlayer();
        }
        if (act == Actions.Blink && state != States.Dead)
        {
            state = States.Blinking;
            FindReachable(controller.tile_grid, weapon.range);
            controller.CleanReachable();
            controller.MarkReachable();
        }
        if (act == Actions.Channel && state != States.Dead)
        {
            Channel();
            controller.CleanReachable();
        }


    }

    public void Die()
    {
        state = States.Dead;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
    }

    public void Move(Transform clicked_tile)
    {
        print("Character " + character_name + " Moved from: " + curr_tile.GetComponent<Tile_Data>().x_index + "," + curr_tile.GetComponent<Tile_Data>().y_index + " to: " + clicked_tile.GetComponent<Tile_Data>().x_index + "," + clicked_tile.GetComponent<Tile_Data>().y_index + " Using 5 AP");
        action_curr -= 5;
        curr_tile.GetComponent<Tile_Data>().traversible = true;
        curr_tile = clicked_tile;
        curr_tile.GetComponent<Tile_Data>().traversible = false;
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
        controller.CleanReachable();
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
    }

    public void Attack(GameObject character)
    {
        print("Character " + character_name + " Attacked: " + character.GetComponent<Character_Script>().character_name + "; Dealing " + Calculate_Damage(character.GetComponent<Character_Script>().armor) + " damage and Using 5 AP");
        action_curr -= 5;
        if (character.GetComponent<Character_Script>().aura_curr == 0 )
        {
            character.GetComponent<Character_Script>().Die();
        }else
        {
            character.GetComponent<Character_Script>().aura_curr -= Calculate_Damage(character.GetComponent<Character_Script>().armor);
            if(character.GetComponent<Character_Script>().aura_curr < 0)
            {
                character.GetComponent<Character_Script>().aura_curr = 0;
                character.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            }
        }
        state = States.Idle;
        controller.CleanReachable();
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
    }

    public void Blink (Transform clicked_tile)
    {
        print("Character " + character_name + " Blinked from: " + curr_tile.GetComponent<Tile_Data>().x_index + "," + curr_tile.GetComponent<Tile_Data>().y_index + " to: " + clicked_tile.GetComponent<Tile_Data>().x_index + "," + clicked_tile.GetComponent<Tile_Data>().y_index + " Using 10 AP");
        action_curr -= 10;
        curr_tile.GetComponent<Tile_Data>().traversible = true;
        curr_tile = clicked_tile;
        curr_tile.GetComponent<Tile_Data>().traversible = false;
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
        controller.CleanReachable();
        if (action_curr <= 0)
        {
            action_curr = 0;
            action_curr += AP_RECOVERY;
            controller.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
    }

    public void Channel()
    {
        print("Character " + character_name + " Channeled from: " + aura_curr + ", Using 10 AP");
        action_curr -= 10;
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
            controller.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
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
            controller.NextPlayer();
        }
        if (action_curr > action_max)
        {
            action_curr = action_max;
        }
    }
}
