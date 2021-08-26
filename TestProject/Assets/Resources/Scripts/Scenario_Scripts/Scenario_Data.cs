using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

[Serializable]
public class Scenario_Data
{
    [JsonProperty] public string scenario_id { get; private set; }
    [JsonProperty] public string scenario_sector { get; private set; }
    [JsonProperty] public string scenario_name { get; private set; }
    [JsonProperty] public string description { get; private set; }
    [JsonProperty] public Scenario_Objectives objective { get; private set; }
    [JsonProperty] public List<string> rewards { get; private set; }
    [JsonProperty] public string bonus_objective { get; private set; }
    [JsonProperty] public List<string> bonus_rewards { get; private set; }
    [JsonProperty] public List<int> unlocks { get; private set; }
    [JsonProperty] public List<int> unlocks_on_loss { get; private set; }
    [JsonProperty] public List<int> unlocks_on_win { get; private set; }
    [JsonProperty] public List<int> unlocks_on_bonus { get; private set; }
    [JsonProperty] public List<int> curr_character_ids { get; private set; }
    [JsonProperty] public int curr_character_num { get; private set; }
    [JsonProperty] public List<int> turn_order_ids { get; private set; }
    [JsonProperty] public Tile_Data[,] tiles { get; private set; }
    [JsonProperty] public int curr_round { get; private set; }
    [JsonProperty] public string prev_scenario { get; private set; }
    [JsonProperty] public string next_scenario { get; private set; }

    /// <summary>
    /// JSON Constructor used to turn JSON into Scenario_Data
    /// </summary>
    /// <param name="new_scenario_id">ID of the current scenario</param>
    /// <param name="new_scenario_sector">Sector in which the current scenario can be played.</param>
    /// <param name="new_scenario_name">Name of the scenario.</param>
    /// <param name="new_description">Description of the scenario.</param>
    /// <param name="new_objective">Objective of the scenario.</param>
    /// <param name="new_rewards">Rewards for the scenario.</param>
    /// <param name="new_bonus_objective">Bonus objective for the scenario.</param>
    /// <param name="new_bonus_rewards">Rewards if the bonus objective is achieved.</param>
    /// <param name="new_unlocks">Scenarios Ids unlocked by completing this scenario.</param>
    /// <param name="new_unlocks_on_loss">Scenarios Ids unlocked by failing this scenario.</param>
    /// <param name="new_unlocks_on_win">Scenarios Ids unlocked by winning this scenario.</param>
    /// <param name="new_unlocks_on_bonus">Scenario Ids unlocked by completing the bonus of this scenario.</param>
    /// <param name="new_curr_character_ids">Ids of the current characters in the current characters stack.</param>
    /// <param name="new_curr_character_num"></param>
    /// <param name="new_turn_order_ids">Ids of the current turn order.</param>
    /// <param name="new_tiles">Tile data for all tiles of the scenario</param>
    /// <param name="new_curr_round">Number of the current round.</param>
    /// <param name="new_prev_scenario">The Id of the scenario before this one.</param>
    /// <param name="new_next_scenario">The Next scenario after this one.</param>
    [JsonConstructor]
    public Scenario_Data(
        string new_scenario_id,
        string new_scenario_sector,
        string new_scenario_name,
        string new_description,
        Scenario_Objectives new_objective,
        List<string> new_rewards,
        string new_bonus_objective,
        List<string> new_bonus_rewards,
        List<int> new_unlocks,
        List<int> new_unlocks_on_loss,
        List<int> new_unlocks_on_win,
        List<int> new_unlocks_on_bonus,
        List<int> new_curr_character_ids,
        int new_curr_character_num,
        List<int> new_turn_order_ids,
        Tile_Data[,] new_tiles,
        int new_curr_round,
        string new_prev_scenario,
        string new_next_scenario)
    {
        scenario_id = new_scenario_id;
        scenario_sector = new_scenario_sector;
        scenario_name = new_scenario_name;
        description = new_description;
        objective = new_objective;
        rewards = new_rewards;
        bonus_objective = new_bonus_objective;
        bonus_rewards = new_bonus_rewards;
        unlocks = new_unlocks;
        unlocks_on_loss = new_unlocks_on_loss;
        unlocks_on_win = new_unlocks_on_win;
        unlocks_on_bonus = new_unlocks_on_bonus;
        curr_character_ids = new_curr_character_ids;
        curr_character_num = new_curr_character_num;
        turn_order_ids = new_turn_order_ids;
        tiles = new_tiles;
        curr_round = new_curr_round;
        prev_scenario = new_prev_scenario;
        next_scenario = new_next_scenario;
    }

    /// <summary>
    /// Constructor to turn Scenarios into Scenario_Data for storage.
    /// </summary>
    /// <param name="s">Scenario to turn into data.</param>
    public Scenario_Data(Scenario s)
    {
        scenario_id = s.scenario_id;
        scenario_sector = s.scenario_sector;
        scenario_name = s.scenario_name;
        description = s.description;
        objective = s.objective;
        rewards = s.rewards;
        bonus_objective = s.bonus_objective;
        bonus_rewards = s.bonus_rewards;
        unlocks = s.unlocks;
        unlocks_on_loss = s.unlocks_on_loss;
        unlocks_on_win = s.unlocks_on_win;
        unlocks_on_bonus = s.unlocks_on_loss;
        curr_character_ids = new List<int>();
        List<Character_Script> curr_character_scripts = new List<Character_Script>();
        if (s.curr_character != null)
        {
            while (s.curr_character.Count > 0)
            {
                Character_Script chara = s.curr_character.Pop().GetComponent<Character_Script>();
                if (chara)
                {
                    curr_character_scripts.Add(chara);
                    curr_character_ids.Add(chara.Get_Scenario_ID());
                }
            }
            //Restore the Scenario's stack
            foreach (Character_Script chars in curr_character_scripts)
            {
                s.curr_character.Push(chars.gameObject);
            }
        }
        curr_character_num = s.curr_character_num;
        turn_order_ids = new List<int>();
        if (s.turn_order != null)
        {
            foreach (GameObject obj in s.turn_order)
            {
                Character_Script chara = obj.GetComponent<Character_Script>();
                if (chara)
                {
                    turn_order_ids.Add(chara.Get_Scenario_ID());
                }
            }
        }
        curr_round = s.curr_round;
        prev_scenario = s.prev_scenario;
        next_scenario = s.next_scenario;
        tiles = new Tile_Data[s.tiles.GetLength(0), s.tiles.GetLength(1)];
        foreach(Tile tile in s.tiles)
        {
            tiles[tile.index[0], tile.index[1]] = tile.Export_Data();
        }
    }

    /// <summary>
    /// Prints Json for the file.
    /// </summary>
    public void Print_Json()
    {
        string json_data = JsonConvert.SerializeObject(this);
        Debug.Log(json_data);
    }
}
