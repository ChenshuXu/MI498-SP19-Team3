using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    // Used to control the animations
    public Animator animController;

    // The input of the players
    private InputController input;
    // Used to play certain sounds
    private AudioManager audioManager;
    

    private void Start()
    {
        input = GetComponent<InputController>();
        audioManager = AudioManager.instance;
    }
    
    private void Update()
    {
        CheckRun();
        CheckJump();
        CheckGrabBox();
        CheckGrabBaby();
        CheckIsThrown();
        CheckScarfChange();
        // GrandmaController will probably have to call something in this script to throw baby
    }


    /// <summary>
    /// Used to check if player is moving, idle, or falling
    /// </summary>
    void CheckRun()
    {
        float xMove = input.moveVector.x;
        float yMove = input.moveVector.y;
        float zMove = input.moveVector.z;

        // Run
        if (input.grounded && input.canMove && (xMove != 0 || zMove != 0))
        {
            //Debug.Log("Walking");
            animController.SetBool("isRunning", true);
            audioManager.PlayWalk(input.playerId);
        }
        else if (input.grounded && input.canMove)
        {
            //Debug.Log("Not Walking");
            animController.SetBool("isRunning", false);
            audioManager.StopWalk(input.playerId);
            // Idle animation?
        }
        else if (yMove < 0 && input.canMove)
        {
            //Debug.Log("Falling");
        }
    }


    /// <summary>
    /// Used to check if the player is jumping
    /// </summary>
    void CheckJump()
    {
        // Jump
        if (input.midJump)
        {
            //Debug.Log("JUMP");
            animController.SetBool("jumpUp", true);
        }
        else
        {
            animController.SetBool("jumpUp", false);
        }
    }


    /// <summary>
    /// Used to check if the player is holding the box, and if they're pushing or pulling
    /// </summary>
    void CheckGrabBox()
    {
        if (input.haveBox)
        {
            //Debug.Log("Grabbing Box");
            animController.SetBool("boxGrab", true);

            if(input.playerId == 0)
            {
                // Get direction towards the box and direction of movement
                Vector3 dirToBox = (input.pickupObject.transform.position - transform.position).normalized;
                Vector3 dirMovement = input.moveVector.normalized;

                // Only pushing or pulling if moving
                if (dirMovement.magnitude > 0.01)
                {
                    // Whichever dot product is larger tells us the direction we're moving
                    float dotProdPush = Vector3.Dot(dirToBox, dirMovement);
                    float dotProdPull = Vector3.Dot(-dirToBox, dirMovement);

                    if (dotProdPush > dotProdPull)
                    {
                        //Debug.Log("Pushing Box");
                        animController.SetBool("goingForward", true);
                    }
                    else
                    {
                        //Debug.Log("Pulling Box");
                        animController.SetBool("goingForward", false);
                    }
                }
                else
                {
                    // Just holding the box, not pushing or pulling
                }
            }
        }
        else
        {
            // Not holding the box
            animController.SetBool("boxGrab", false);
        }
    }


    /// <summary>
    /// Used to check if the grandma is holding the baby
    /// </summary>
    void CheckGrabBaby()
    {
        // Grandma is holding baby
        if (input.haveBbcat && input.playerId == 0)
        {
            animController.SetBool("kittenGrab", true);
        }
        else if (input.playerId == 0)
        {
            animController.SetBool("kittenGrab", false);
        }

        if(input.bePicked && input.playerId == 1)
        {
            animController.SetBool("kittenHeld", true);
        }
        else if (input.playerId == 1)
        {
            animController.SetBool("kittenHeld", false);
        }
    }


    /// <summary>
    /// Check if the baby is thrown and hasn't landed yet
    /// </summary>
    void CheckIsThrown()
    {
        // Grandma throw baby
        // GrandmaController will probably have to call something in this script?

        // Baby being thrown

        if (input.thrown && input.playerId == 1)
        {
            animController.SetBool("kitBeingThrown", true);
        }
        else if (input.playerId == 1)
        {
            animController.SetBool("kitBeingThrown", false);
        }
    }

    void CheckScarfChange()
    {
        if (input.changeScarfInc)
        {
            animController.SetBool("changeScarfInc", true);
        }
        else
        {
            animController.SetBool("changeScarfInc", false);
        }
        if (input.changeScarfDec)
        {
            animController.SetBool("changeScarfDec", true);
        }
        else
        {
            animController.SetBool("changeScarfDec", false);
        }
    }

    public void ThrowBaby()
    {
        // Grandma script calls this to play the throw animation
    }

    
}



    