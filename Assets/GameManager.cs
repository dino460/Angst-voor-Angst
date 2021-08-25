using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private AudioManager audioManager;

    void Start()
    {
        audioManager.Play("Ambience");
    }
}
