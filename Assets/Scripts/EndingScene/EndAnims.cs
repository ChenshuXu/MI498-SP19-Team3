using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndAnims : MonoBehaviour
{
    public bool running = false;
    public int player = 0;

    public Animator animController;
    private AudioManager audioManager;

    private void Start()
    {
        animController = GetComponent<Animator>();
        audioManager = AudioManager.instance;
    }

    private void Update()
    {
        if(running)
        {
            animController.SetBool("isRunning", true);
            audioManager.PlayWalk(player);
        }
        else
        {
            animController.SetBool("isRunning", false);
            audioManager.StopWalk(player);
        }
    }
}
