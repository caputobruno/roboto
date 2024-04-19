using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] TMP_Text _scoreText;
    [SerializeField] Image[] _hearts;
    [SerializeField] Image _flashImage;

    Player _player;

    public void Bind(Player player)
    {
        _player = player;

        _player.CoinsChanged += UpdateCoins;
        UpdateCoins();

        _player.HealthChanged += UpdateHealth;
        UpdateHealth();
    }

    void UpdateCoins()
    {
        _scoreText.SetText(_player.Coins.ToString());
    }

    void UpdateHealth()
    {
        for(int i = 0; i < _hearts.Length; i++)
        {
            Image heart = _hearts[i];
            heart.enabled = i < _player.Health;
        }

        StartCoroutine(Flash(0));
        StartCoroutine(Flash(0.4f));
    }

    IEnumerator Flash(float time)
    {
        yield return new WaitForSeconds(time);
        _flashImage.enabled = true;
        yield return new WaitForSeconds(0.2f);
        _flashImage.enabled = false;
    }
}
