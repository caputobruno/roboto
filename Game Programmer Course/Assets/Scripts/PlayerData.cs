using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData : INamed
{
    [field: SerializeField] public string Name { get; set; }
    public int Coins;
    public int Health = 6;
    public int maxHealth = 6;
    public Vector2 Position;
    public Vector2 Velocity;
    public List<string> Items = new List<string>();
}

[Serializable]
public class GameData
{
    public List<PlayerData> PlayerDatas = new List<PlayerData>();
    public string GameName;
    public string SceneName;
    public List<LevelData> LevelDatas = new List<LevelData>();
}


[Serializable]
public class LevelData
{
    public string LevelName;
    public List<CoinData> CoinDatas = new List<CoinData>();
}

[Serializable]
public class CoinData : INamed
{
    public bool IsCollected;
    [field: SerializeField] public string Name { get; set; }
}