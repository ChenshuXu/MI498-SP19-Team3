using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayConfetti : MonoBehaviour
{
    // The input of the players
    private InputController input;
    public GameObject confetti;
    // Used to control the animations
    public Animator animController;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputController>();
        confetti.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (animController.GetCurrentAnimatorStateInfo(0).IsName("KittenSticksLanding"))
        {
            //Debug.Log("Got this box!!!!!");
            confetti.SetActive(true);
        }
        else
        {
            confetti.SetActive(false);
        }
    }
}
