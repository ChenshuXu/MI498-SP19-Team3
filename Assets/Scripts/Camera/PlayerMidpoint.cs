using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMidpoint : MonoBehaviour
{
    public GameObject followCam;
    public GameObject grandma;
    public GameObject baby;
    public float cameraLerpSpeed = 10f;
    public bool following = true;

    private Transform targetOne; // One of the players
    private Transform targetTwo; // Other player
    private PickupController gmaPickupCont;
    private Camera mainCamera;
    private float startDist;
    private CinemachineVirtualCamera cineCam;
    private CinemachineTransposer transposer;
    private GameObject currCam;
    private CinemachineBrain brain;

    void Start()
    {
        gmaPickupCont = grandma.GetComponent<PickupController>();
        mainCamera = Camera.main;
        startDist = Vector3.Distance(transform.position, mainCamera.transform.position);
        cineCam = followCam.GetComponent<CinemachineVirtualCamera>();
        transposer = cineCam.GetCinemachineComponent<CinemachineTransposer>();
        brain = mainCamera.GetComponent<CinemachineBrain>();
    }

    void FixedUpdate()
    {
        targetOne = baby.transform;
        targetTwo = grandma.transform;

        float curDist = Vector3.Distance(transform.position, followCam.transform.position);
        currCam = brain.ActiveVirtualCamera.VirtualCameraGameObject;

        if(currCam == followCam)
        {
            Vector3 t1ScreenPoint = mainCamera.WorldToViewportPoint(targetOne.position);
            Vector3 t2ScreenPoint = mainCamera.WorldToViewportPoint(targetTwo.position);

            if (!(t1ScreenPoint.z > 0 && t1ScreenPoint.x > .2f && t1ScreenPoint.x < .8f && t1ScreenPoint.y > .2f && t1ScreenPoint.y < .8f) || !(t2ScreenPoint.z > 0 && t2ScreenPoint.x > .2f && t2ScreenPoint.x < .8f && t2ScreenPoint.y > .2f && t2ScreenPoint.y < .8f))
            {
                transposer.m_FollowOffset -= transform.worldToLocalMatrix.MultiplyVector(followCam.transform.forward * .1f);
                //Debug.Log("Grow");
            }
            else if ((t1ScreenPoint.z > 0 && t1ScreenPoint.x > .4f && t1ScreenPoint.x < .6f && t1ScreenPoint.y > .4f && t1ScreenPoint.y < .6f) && (t2ScreenPoint.z > 0 && t2ScreenPoint.x > .4f && t2ScreenPoint.x < .6f && t2ScreenPoint.y > .4f && t2ScreenPoint.y < .6f) && curDist > startDist)
            {
                transposer.m_FollowOffset += transform.worldToLocalMatrix.MultiplyVector(followCam.transform.forward * .1f);
                //Debug.Log("Shrink");
            }
        }

        // Changing the midpoint position
        if (gmaPickupCont.pickState == PickupController.PickupState.NotPick)
        {
            // Grandma isn't holding the baby
            Vector3 pos = (targetOne.position + targetTwo.position) / 2;
            if (Vector3.Distance(transform.position, pos) > 0.1)
            {
                // Smoothly switch focus back to between the two characters
                transform.position = Vector3.Lerp(transform.position, pos, Time.fixedDeltaTime * cameraLerpSpeed);
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
                transform.position = Vector3.Lerp(transform.position, pos, Time.fixedDeltaTime * cameraLerpSpeed);
            }
            else
            {
                transform.position = pos;
            }
        }
    }
}
