using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewGameButton : MonoBehaviour
{
    private void Start() => GetComponent<Button>().onClick.AddListener(CreateNewGame);
    public void CreateNewGame() => GameManager.Instance.NewGame();
}
