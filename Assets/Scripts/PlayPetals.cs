using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayPetals : MonoBehaviour
{
    public ParticleSystem p; 
    // Start is called before the first frame update
    void Start()
    {
        p.Pause();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character")
        {
            Debug.Log("PETALS");
            p.Clear();
            p.Play();
        }

    }
}
