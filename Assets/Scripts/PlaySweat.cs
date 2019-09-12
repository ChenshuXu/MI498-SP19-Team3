using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySweat : MonoBehaviour
{
    // The input of the players
    private InputController input;
    public GameObject sweat;
    // Used to control the animations
    public Animator animController;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputController>();
        sweat.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(sweat.GetComponent<ParticleSystem>().playOnAwake == false)
        {
            sweat.GetComponent<ParticleSystem>().playOnAwake = true;
        }
        if (input.haveBox && animController.GetBool("isRunning"))
        {
            sweat.SetActive(true);
        }
        else
        {
            sweat.SetActive(false);
        }
    }
}
