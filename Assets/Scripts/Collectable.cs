using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    // Which UI object will be lit up by picking this up
    public int collectableIndex = 0;


    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Character")
        {
            AudioManager.instance.PlayPickup();
            CollectableMenu.instance.AddCollectable(collectableIndex);
            Destroy(gameObject);
        }
    }
}
