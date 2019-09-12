using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupRune : MonoBehaviour
{
    public GameObject slashes;
    public GameObject elevatorRune;
    public ParticleSystem pickupParticle;

    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Character")
        {
            // Make a sound and put the rune where it should be
            // pickupParticle.Play();
            audioManager.PlayRune();
            elevatorRune.SetActive(true);
            slashes.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
