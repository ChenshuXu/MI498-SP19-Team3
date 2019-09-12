using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBoxController : MonoBehaviour
{
    public float addWeight = 10f; // Amount of weight to add
    public bool bePicked = false; // Is the box being held?
    public ParticleSystem boxDust; // Particles to play while pushing box

    public InputController playerHolding; // Input cont of the player that is holding the box
    
    private float dist = 1f; // How far the ground is from the game object
    private Rigidbody rb; // Rigidbody on this box
    private int players; // How many players are touching the box

    private AudioManager audioManager; // Play box moving sound


    private void Start()
    {
        dist = transform.localScale.x / 2;
        rb = GetComponent<Rigidbody>();
        audioManager = AudioManager.instance;
    }


    private void Update()
    {
        if((rb.velocity.magnitude > 0.01f && !bePicked && IsGrounded()) 
            || (bePicked && IsGrounded() && playerHolding.moveVector.magnitude > 0.01f))
        {
            // play audio
            audioManager.PlayMoveBox();

            // play particles
            if(!boxDust.isPlaying)
                boxDust.Play();
        }
        else
        {
            // stop audio
            audioManager.StopMoveBox();

            // stop particles
            if(boxDust.isPlaying)
                boxDust.Stop();
        }
    }


    /// <summary>
    /// Is the box currently on the ground?
    /// </summary>
    /// <returns>True or false depending on if box is grounded</returns>
    public bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.BoxCast(transform.position, transform.localScale, Vector3.down, out hit, transform.rotation, dist);
    }


    public bool IsGroundedSmall()
    {
        RaycastHit hit;
        return Physics.BoxCast(transform.position, transform.localScale / 1.5f, Vector3.down, out hit, transform.rotation, dist);
    }


    /// <summary>
    /// Adds weight to the box so that the grandma can't just push it around
    /// </summary>
    /// <param name="other">What collided with the box</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Character" && IsGrounded())
        {
            players++;
            if(players == 1)
            {
                rb.mass += addWeight;
            }
        }
    }


    /// <summary>
    /// Removes weight once the players aren't near it anymore
    /// </summary>
    /// <param name="other">What left the trigger</param>
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Character")
        {
            players--;
            if(players == 0)
            {
                rb.mass -= addWeight;
            }
        }
    }
}
