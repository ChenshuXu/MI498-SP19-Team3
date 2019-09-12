using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Loads the next level when this one is completed
/// </summary>
public class EndLevel : MonoBehaviour
{
    public string nextLevel;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Character")
        {
            // Reached the end of the level, load next level
            gameManager.LoadLevel(nextLevel);
        }
    }
}
