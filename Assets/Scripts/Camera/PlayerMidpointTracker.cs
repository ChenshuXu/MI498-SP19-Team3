using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes camera position and zoom based on player positions
/// </summary>
public class PlayerMidpointTracker : MonoBehaviour
{
    public GameObject grandma;
    public GameObject baby;
    public Transform cameraPosition;
    public float cameraLerpSpeed = 10f;
    public float speedUp = 0.16f;
    public bool cameraFollow = true;

    private Transform targetOne; // One of the players
    private Transform targetTwo; // Other player
    private PickupController gmaPickupCont;
    private Camera mainCamera;
    private float startDist;

    private Vector3 origPos;

    private bool changingCamera = false;

    void Start()
    {
        mainCamera = Camera.main;
        startDist = Vector3.Distance(transform.position, mainCamera.transform.position);
        
        gmaPickupCont = grandma.GetComponent<PickupController>();
        origPos = transform.position - mainCamera.transform.position;
    }
    
    void FixedUpdate()
    {
        if (cameraFollow && !changingCamera)
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, cameraPosition.position, .3f);
            mainCamera.transform.rotation = Quaternion.RotateTowards(mainCamera.transform.rotation, cameraPosition.rotation, .9f);
        }
        targetOne = baby.transform;
        targetTwo = grandma.transform;
        float curDist = Vector3.Distance(transform.position, cameraPosition.transform.position);
        
        Vector3 t1ScreenPoint = mainCamera.WorldToViewportPoint(targetOne.position);
        Vector3 t2ScreenPoint = mainCamera.WorldToViewportPoint(targetTwo.position);

        if(changingCamera)
        {
            cameraPosition.transform.position = transform.position - origPos;
        }

        if (!(t1ScreenPoint.z > 0 && t1ScreenPoint.x > .2f && t1ScreenPoint.x < .8f && t1ScreenPoint.y > .2f && t1ScreenPoint.y < .8f) || !(t2ScreenPoint.z > 0 && t2ScreenPoint.x > .2f && t2ScreenPoint.x < .8f && t2ScreenPoint.y > .2f && t2ScreenPoint.y < .8f))
        {
            cameraPosition.transform.localPosition -= transform.worldToLocalMatrix.MultiplyVector(cameraPosition.transform.forward * .1f);
            //Debug.Log("Grow");
        }
        else if ((t1ScreenPoint.z > 0 && t1ScreenPoint.x > .4f && t1ScreenPoint.x < .6f && t1ScreenPoint.y > .4f && t1ScreenPoint.y < .6f) && (t2ScreenPoint.z > 0 && t2ScreenPoint.x > .4f && t2ScreenPoint.x < .6f && t2ScreenPoint.y > .4f && t2ScreenPoint.y < .6f) && curDist > startDist)
        {
            cameraPosition.transform.localPosition += transform.worldToLocalMatrix.MultiplyVector(cameraPosition.transform.forward * .1f);
            //Debug.Log("Shrink");
        }

        // Changing the midpoint position
        if (gmaPickupCont.pickState == PickupController.PickupState.NotPick)
        {
            // Grandma isn't holding the baby
            Vector3 pos = (targetOne.position + targetTwo.position) / 2;
            if (Vector3.Distance(transform.position, pos) > 0.1)
            {
                // Smoothly switch focus back to between the two characters
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * cameraLerpSpeed);
            }
            else
            {
                transform.position = pos;
            }
        }
        else
        {
            // Grandma is holding the baby, focus on her
            Vector3 pos = targetTwo.position;
            if (Vector3.Distance(transform.position, targetTwo.position) > 0.1)
            {
                // Smoothly switch focus to the grandma only
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * cameraLerpSpeed);
            }
            else
            {
                transform.position = pos;
            }
        }
    }

    
    public void ResetCameraPosition()
    {
        cameraFollow = true;
        Vector3 pos = (baby.transform.position + grandma.transform.position) / 2;
        mainCamera.transform.position = pos;
    }


    public void SetCameraFollow(bool following)
    {
        cameraFollow = following;
        if(following)
        {
            float dist = Vector3.Distance(mainCamera.transform.position, cameraPosition.position) * speedUp;
            StartCoroutine(LerpFromTo(mainCamera.transform.position, cameraPosition.position, mainCamera.transform.rotation, cameraPosition.rotation, dist));
        }
        else
        {
            StopAllCoroutines();
        }
    }


    IEnumerator LerpFromTo(Vector3 pos1, Vector3 pos2, Quaternion rot1, Quaternion rot2, float duration)
    {
        changingCamera = true;
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            pos2 = cameraPosition.position;
            mainCamera.transform.position = Vector3.Lerp(pos1, pos2, t / duration);
            mainCamera.transform.rotation = Quaternion.Slerp(rot1, rot2, t / duration);
            yield return 0;
        }
        mainCamera.transform.position = pos2;
        mainCamera.transform.rotation = rot2;
        changingCamera = false;
    }
}
