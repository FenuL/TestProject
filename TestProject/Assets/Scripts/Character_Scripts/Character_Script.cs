using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Character_Script : MonoBehaviour {

    public class Weapon{
        public string name;
        public int range;
        public int attack;
        public bool ranged;
        //public Ability[] abilities;
        //public Perk[] perks;

        public Weapon(Weapons wep)
        {
            switch (wep)
            {
                case Weapons.Sword:
                    name = Weapons.Sword.ToString();
                    range = 1;
                    attack = 2;
                    ranged = false;
                    break;
                case Weapons.Rifle:
                    name = Weapons.Rifle.ToString();
                    range = 4;
                    attack = 3;
                    ranged = true;
                    break;
                case Weapons.Spear:
                    name = Weapons.Spear.ToString();
                    range = 2;
                    attack = 2;
                    ranged = false;
                    break;
                case Weapons.Sniper:
                    name = Weapons.Sniper.ToString();
                    range = 6;
                    attack = 5;
                    ranged = true;
                    break;
                case Weapons.Pistol:
                    name = Weapons.Pistol.ToString();
                    range = 3;
                    attack = 2;
                    ranged = true;
                    break;
                case Weapons.Claws:
                    name = Weapons.Claws.ToString();
                    range = 1;
                    attack = 10;
                    ranged = false;
                    break;
                default:
                    break;
            }
        }
    }

	// Use this for initialization
	void Start () {
	}

    public int calculateDamage()
    {
        if (weapon.ranged)
        {
            return weapon.attack + coordination;
        }else
        {
            return weapon.attack + strength;
        }
    }

    public void Randomize(){
		aura_max = Random.Range (10, 30);
		aura_curr = aura_max;
		canister_max = Random.Range (0,3);
		canister_curr = canister_max;
		armor = Random.Range (0, 5);
		strength = Random.Range (1,7);
		coordination = Random.Range (1, 7);
		spirit = Random.Range (1, 7);
		dexterity = Random.Range (1, 7);
		vitality = Random.Range (1, 7);
		actions = new string[3];
		actions [0] = Actions.Move.ToString();
		actions [1] = Actions.Attack.ToString();
		actions [2] = Actions.Wait.ToString();
		state = States.Idle;
        int w = Random.Range(0, 5);
        if (w == 0)
        {
            weapon = new Weapon(Weapons.Sword);
        } else if (w == 1)
        {
            weapon = new Weapon(Weapons.Rifle);
        }
        else if (w == 2)
        {
            weapon = new Weapon(Weapons.Spear);
        }
        else if (w == 3)
        {
            weapon = new Weapon(Weapons.Sniper);
        }
        else if (w == 4)
        {
            weapon = new Weapon(Weapons.Pistol);
        }
		level = 1;
		character_name = "Character " + character_num;
		controller = Game_Controller.controller;
        //FindReachable(controller.tile_grid, weapon.range);
        //FindReachable(controller.GetComponent<Game_Controller>().tile_grid,dexterity);
	}

    public enum States { Moving, Attacking, Idle, Dead }
    public enum Actions { Move, Attack, Wait }
    public enum Weapons { Sword, Rifle, Spear, Sniper, Pistol, Claws}
	public int character_id { get; set; } 
	public int character_num { get; set; }
	public string character_name { get; set; }
	public int aura_max { get; set; }
	public int aura_curr { get; set; }
	public int canister_max { get; set; }
	public int canister_curr { get; set; }
	public int armor { get; set; }
	public int strength { get; set; }
	public int coordination { get; set; }
	public int spirit { get; set; }
	public int dexterity { get; set; }
	public int vitality { get; set; }
	public int level { get; set; }
    public Weapon weapon { get; set; }
    public string[] actions{ get; set;}
	public States state { get; set; }
	public Equipment[] equipment { get; set; }
	public Game_Controller controller { get; set; }
	public Transform curr_tile { get; set; }
	public List<Transform> reachable_tiles { get; set; }
	
	//public SpriteRenderer renderer;
	public void FindReachable(GameObject grid, int limit){
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
					if( y_index + j >= 0 && y_index + j < grid.GetComponent<Draw_Tile_Grid>().tile_grid.map_height)
					{
						int h = grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j ).GetComponent<Tile_Data>().tile_height-1;
						//print ("distance " + (Mathf.Abs(i)+ Mathf.Abs(j)));
						if (Mathf.Abs (i) + Mathf.Abs (j) <= limit){
							//print ("tile " + (x_index +i) + ","+ (y_index+ j) + " is reachable");
							if (state == States.Moving){
								if (grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j).GetComponent<Tile_Data>().traversible){
									reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j));
								}
							}
							if (state == States.Attacking){
								reachable_tiles.Add(grid.GetComponent<Draw_Tile_Grid>().tile_grid.getTile(x_index + i, y_index + j));
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
		if (act == Actions.Move) {
			state = States.Moving;
			FindReachable(controller.tile_grid, dexterity);
			controller.CleanReachable ();
			controller.MarkReachable ();
		}
		if (act == Actions.Attack) {
			state = States.Attacking;
			FindReachable(controller.tile_grid, weapon.range);
			controller.CleanReachable ();
			controller.MarkReachable ();
		}

	}

    public void Die()
    {
        state = States.Dead;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
    }

    public void Move(Transform clicked_tile)
    {
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
    }

    public void Attack(GameObject character)
    {
        if (character.GetComponent<Character_Script>().aura_curr == 0 )
        {
            character.GetComponent<Character_Script>().Die();
        }else
        {
            character.GetComponent<Character_Script>().aura_curr -= calculateDamage();
            if(character.GetComponent<Character_Script>().aura_curr < 0)
            {
                character.GetComponent<Character_Script>().aura_curr = 0;
                character.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
            }
        }
        state = States.Idle;
    }

    public Character_Script(){
	}
	
	// Update is called once per frame
	void Update () {
		if (aura_curr < 0) {
			aura_curr = 0;
		}
	}
}
