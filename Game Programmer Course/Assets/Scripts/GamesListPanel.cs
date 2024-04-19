using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamesListPanel : MonoBehaviour
{
    [SerializeField] LoadGameButton _buttonPrefab;

    private void Start()
    {
        UpdateLoadButtonList();
    }

    public void UpdateLoadButtonList()
    {
        int i = 0;
        foreach (var gameName in GameManager.Instance.AllGameNames)
        {
            i++;
            var button = Instantiate(_buttonPrefab, transform);
            button.SetGameName("Game " + i);
        }
    }
}
