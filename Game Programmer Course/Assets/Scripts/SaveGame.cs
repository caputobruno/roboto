using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveGame : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.SaveGame();
        Destroy(gameObject);
    }
}
