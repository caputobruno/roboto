using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    AudioSource audioS;

    void Start() => audioS = GetComponent<AudioSource>();

    void OnTriggerEnter2D(Collider2D collision) => audioS.Play();
}
