using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PuzzleCameraCine : MonoBehaviour
{
    public CinemachineVirtualCamera cineCam;
    public CinemachineTargetGroup targetGroup;

    private int playerCount = 0;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Character")
        {
            if (playerCount < 2)
            {
                playerCount++;
                if (cineCam.Priority < 12)
                    cineCam.Priority++;

                if (other.gameObject.GetComponent<InputController>().haveBbcat)
                {
                    // grandma is holding the baby, so add both players
                    playerCount++;
                    if (cineCam.Priority < 12)
                        cineCam.Priority++;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Character")
        {
            if (playerCount > 0)
            {
                if (!other.gameObject.GetComponent<InputController>().bePicked)
                {
                    playerCount--;
                    if (cineCam.Priority > 10)
                        cineCam.Priority--;
                }

                if (other.gameObject.GetComponent<InputController>().haveBbcat)
                {
                    // grandma is holding the baby, so remove both players
                    playerCount = 0;
                    if (cineCam.Priority > 10)
                        cineCam.Priority--;
                }
            }

            if (playerCount == 0)
            {
                // Set the priority back to 10
                cineCam.Priority = 10;
            }
        }
    }

    public void Reset(GameObject gma, GameObject bby)
    {
        playerCount = 0;
        cineCam.Priority = 10;

        if(targetGroup)
        {
            targetGroup.m_Targets[0].target = gma.transform;
            targetGroup.m_Targets[1].target = bby.transform;
        }
    }
}
