using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorSensor : MonoBehaviour
{
    // Elevator this sensor is connected to
    private Elevator elevator;
    // How many players are in the sensor area
    private int playerCount = 0;

    private void Start()
    {
        // Get the elevator from the parent
        elevator = transform.parent.gameObject.GetComponent<Elevator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Player enters area
        if (other.gameObject.tag == "Character")
        {
            if (playerCount < 2)
            {
                playerCount++;
                elevator.sensor = true;

                if (other.gameObject.GetComponent<InputController>().haveBbcat)
                {
                    // grandma is holding the baby, so add both players
                    playerCount++;
                    elevator.sensor = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Player exits area
        if (other.gameObject.tag == "Character")
        {
            if (playerCount > 0)
            {
                if (!other.gameObject.GetComponent<InputController>().bePicked)
                    playerCount--;

                if (other.gameObject.GetComponent<InputController>().haveBbcat)
                {
                    // grandma is holding the baby, so remove both players
                    playerCount = 0;
                }
            }

            if (playerCount == 0)
            {
                // let elevator move again
                elevator.sensor = false;
            }
        }
    }
}
