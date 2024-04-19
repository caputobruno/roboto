using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool CinematicPlaying { get; private set; }
    public static bool IsLoading { get; private set; }

    public List<string> AllGameNames = new List<string>();

    [SerializeField] GameData _gameData;
    PlayerInputManager _playerInputManager;

    public void ToggleCinematic(bool cinematicPlaying) => CinematicPlaying = cinematicPlaying;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        _playerInputManager = GetComponent<PlayerInputManager>();
        _playerInputManager.onPlayerJoined += HandlePlayerJoined;

        SceneManager.sceneLoaded += HandleSceneLoaded;

        string commaSeparatedList = PlayerPrefs.GetString("AllGameNames");
        AllGameNames = commaSeparatedList.Split(",").ToList();
        AllGameNames.Remove("");
    }

    private void HandleSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Menu")
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        else
        {
            _gameData.SceneName = arg0.name;
            _playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;

            var levelData = _gameData.LevelDatas.FirstOrDefault(t=> t.LevelName == arg0.name);
            if(levelData == null)
            {
                levelData = new LevelData() { LevelName = arg0.name };
                _gameData.LevelDatas.Add(levelData);
            }

            Bind<Coin, CoinData>(levelData.CoinDatas);
            Bind<PlayerInventory, PlayerData>(_gameData.PlayerDatas);

            var allPlayers = FindObjectsOfType<Player>();
            foreach(var player in allPlayers)
            {
                var playerInput = player.GetComponent<PlayerInput>();
                var data = GetPlayerData(playerInput.playerIndex);
                player.Bind(data);

                if (GameManager.IsLoading)
                {
                    player.RestorePositionAndVelocity();
                    IsLoading = false;
                }

            }
        }
            //SaveGame();

    }

    void Bind<T, D>(List<D> datas) where T : MonoBehaviour, IBind<D> where D : INamed, new()
    {
        var instances = FindObjectsOfType<T>();
        foreach(var instance in instances)
        {
            var data = datas.FirstOrDefault(t => t.Name == instance.name);
            if (data == null)
            {
                data = new D() { Name = instance.name };
                datas.Add(data);
            }
            instance.Bind(data);
        }
    }

    public void SaveGame()
    {
        if(string.IsNullOrWhiteSpace(_gameData.GameName))
            _gameData.GameName = "Game " + AllGameNames.Count;

        string text = JsonUtility.ToJson(_gameData);
        Debug.Log(text);

        PlayerPrefs.SetString(_gameData.GameName, text);

        if (AllGameNames.Contains(_gameData.GameName) == false)
            AllGameNames.Add(_gameData.GameName);

        string commaSeparatedGameNames = string.Join(",", AllGameNames);
        PlayerPrefs.SetString("AllGameNames", commaSeparatedGameNames);
        PlayerPrefs.Save();
    }

    public void LoadGame(string gameName)
    {
        IsLoading = true;
        string text = PlayerPrefs.GetString(gameName);
        _gameData = JsonUtility.FromJson<GameData>(text);
        if(string.IsNullOrWhiteSpace(_gameData.SceneName))
            _gameData.SceneName = "Level 1";
        SceneManager.LoadScene(_gameData.SceneName);
    }

    public void ReloadGame()
    {
        LoadGame(_gameData.GameName);
    }

    void HandlePlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("HandlePlayerJoined" + playerInput);
        PlayerData playerData = GetPlayerData(playerInput.playerIndex);

        Player player = playerInput.GetComponent<Player>();
        player.Bind(playerData);
    }

    PlayerData GetPlayerData(int playerIndex)
    {
        if(_gameData.PlayerDatas.Count <= playerIndex)
        {
            var playerData = new PlayerData();
            _gameData.PlayerDatas.Add(playerData);
        }
        return _gameData.PlayerDatas[playerIndex];
    }

    internal void NewGame()
    {
        Debug.Log("New Game Called");
        _gameData = new GameData();
        _gameData.GameName = DateTime.Now.ToString("G");
        SceneManager.LoadScene("Level 1");
    }

    internal void DeleteGame(string gameName)
    {
        PlayerPrefs.DeleteKey(gameName);
        AllGameNames.Remove(gameName);

        string commaSeparatedGameNames = string.Join(",", AllGameNames);
        PlayerPrefs.SetString("AllGameNames", commaSeparatedGameNames);
        PlayerPrefs.Save();
    }

    internal Item GetItem(string itemName)
    {
        int index = itemName.IndexOf("_");
        if(index < 0)
        {
            Debug.LogError($"Separator '_' not found in item name: {itemName} ");
            return null;
        }

        string prefabName = itemName.Substring(0, itemName.IndexOf("_"));
        var prefab = _allItems.FirstOrDefault(t => t.name == prefabName);
        if(prefab == null)
        {
            Debug.LogError($"Unable to find item {itemName}");
            return null;
        }
        var newInstance = Instantiate(prefab);
        newInstance.name = prefabName;
        return newInstance;
    }
    public List<Item> _allItems;
}

public interface INamed
{
    string Name { get; set; }
}