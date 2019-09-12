using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public GameManager.GameLevel m_NextLevel;

    private GameManager gameManager;

    public GameObject flames;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.instance;
        if (flames != null)
        {
            flames.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (flames != null)
        {
            flames.SetActive(true);
        }
        if (other.gameObject.name == "Grandma")
        {
            gameManager.EnterLevel(m_NextLevel);
            gameObject.SetActive(false);
        }
    }
}
