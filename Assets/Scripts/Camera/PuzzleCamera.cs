using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleCamera : MonoBehaviour
{
    public Camera mainCamera;
    public PlayerMidpointTracker cameraHolder;
    public Transform puzzleCameraPosition;
    public int playersNeeded = 2;

    public float speedUp = 0.16f;

    private int playerCount = 0;
    

    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, Quaternion rot1, Quaternion rot2, float duration)
    {
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            mainCamera.transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            mainCamera.transform.rotation = Quaternion.Slerp(rot1, rot2, t / duration);
            yield return 0;
        }
        mainCamera.transform.position = pos2;
        mainCamera.transform.rotation = rot2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Character")
        {
            if (playerCount < 2)
                playerCount++;

            if (playerCount == playersNeeded)
            {
                cameraHolder.SetCameraFollow(false);
                cameraHolder.StopAllCoroutines();
                float dist = Vector3.Distance(mainCamera.transform.position, puzzleCameraPosition.position) * speedUp;
                StartCoroutine(LerpFromTo(mainCamera.transform.position, puzzleCameraPosition.position, mainCamera.transform.rotation, puzzleCameraPosition.rotation, dist));

                if (other.gameObject.GetComponent<InputController>().haveBbcat)
                    playerCount++;
            }
            else if (other.gameObject.GetComponent<InputController>().haveBbcat)
            {
                // grandma is holding the baby, so add both players
                playerCount++;

                if (playerCount == playersNeeded)
                {
                    cameraHolder.SetCameraFollow(false);
                    cameraHolder.StopAllCoroutines();
                    float dist = Vector3.Distance(mainCamera.transform.position, puzzleCameraPosition.position) * speedUp;
                    StartCoroutine(LerpFromTo(mainCamera.transform.position, puzzleCameraPosition.position, mainCamera.transform.rotation, puzzleCameraPosition.rotation, dist));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Character")
        {
            if (playerCount > 0 && !other.gameObject.GetComponent<InputController>().bePicked)
                playerCount--;

            if (other.gameObject.GetComponent<InputController>().haveBbcat)
            {
                // grandma is holding the baby, so add remove both players
                playerCount--;
            }

            if (playerCount == 0)
            {
                cameraHolder.SetCameraFollow(true);
                StopAllCoroutines();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Character" && cameraHolder.cameraFollow && playerCount >= playersNeeded)
        {
            cameraHolder.SetCameraFollow(false);
            cameraHolder.StopAllCoroutines();
            float dist = Vector3.Distance(mainCamera.transform.position, puzzleCameraPosition.position) * speedUp;
            StartCoroutine(LerpFromTo(mainCamera.transform.position, puzzleCameraPosition.position, mainCamera.transform.rotation, puzzleCameraPosition.rotation, dist));
        }
    }

    public void Reset()
    {
        playerCount = 0;
    }
}
