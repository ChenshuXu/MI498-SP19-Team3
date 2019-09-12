using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    // Particles to play on start
    public ParticleSystem dustParticles;
    // How long to wait before going in other direction
    public float waitTime = 0;
    // The start position
    public Transform startPt;
    // Position to move toward
    public Transform endPt;
    // Is it currently activated by a button or switch or runes
    public bool active = false;
    // Is someone standing under the elevator
    public bool sensor = false;

    // Is the elevator moving
    private bool moving = true;
    // Is the elevator moving upwards
    private bool movingUp = false;
    // Is this the first activation of the elevator
    private bool firstMove = true;
    // What position elevator is moving towards
    private Transform currTarget;
    // Audio to play while moving
    private AudioSource elevatorAudio;

    // Start is called before the first frame update
    void Start()
    {
        currTarget = endPt;
        elevatorAudio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active && moving)
        {
            // Check direction of elevator movement
            if(transform.position.y < currTarget.position.y)
            {
                movingUp = true;
            }
            else
            {
                movingUp = false;
            }

            // Stop moving if sensor is triggered and moving downward, continue otherwise
            if (!sensor || (sensor && movingUp))
            {
                // Check if elevator should switch directions
                if (transform.position != currTarget.position)
                {
                    if(firstMove)
                    {
                        dustParticles.Play(true);
                        firstMove = false;
                    }

                    transform.position = Vector3.MoveTowards(transform.position, currTarget.position, .1f);
                    if(!elevatorAudio.isPlaying)
                    {
                        elevatorAudio.Play();
                    }
                }
                else
                {
                    // Pause and then move in other direction
                    moving = false;
                    StartCoroutine(WaitToMove());
                    if (currTarget == endPt)
                    {
                        currTarget = startPt;
                    }
                    else
                    {
                        currTarget = endPt;
                    }
                }
            }
        }
        else if (elevatorAudio.isPlaying)
        {
            elevatorAudio.Stop();
        }
    }

    public IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(waitTime);
        moving = true;
    }
}
