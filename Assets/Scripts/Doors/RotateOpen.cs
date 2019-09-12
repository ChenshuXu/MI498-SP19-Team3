using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


/// <summary>
/// Lower/raise a drawbridge or open/close a door
/// </summary>
public class RotateOpen : MonoBehaviour
{
    // List of objects to rotate
    public GameObject[] objToRotate;
    // List of objects to animate
    public GameObject[] objToPlayAnim;
    // How fast to rotate
    public float rotSpeed = 1f;
    // Angle of open door/drawbridge
    public Quaternion openRot;
    
    // Is it currently rotating
    public bool rotating = false;

    // Opening
    private bool startedOpening = false;
    // Closing
    private bool startedClosing = false;

    // Angle started at
    private Quaternion startRot;
    // Used in Slerp to rotate
    private float timeCount = 0.0f;


    private void Start()
    {
        // Get initial start rotation
        for (int i = 0; i < objToRotate.Length; i++)
        {
            startRot = objToRotate[i].transform.localRotation;
        }
    }

    public void Open()
    {
        // Used to reset timeCount
        if(!startedOpening)
        {
            timeCount = 0;
            startedOpening = true;
            startedClosing = false;
        }

        // Rotate open
        for (int i = 0; i < objToRotate.Length; i++)
        {
            objToRotate[i].transform.rotation = Quaternion.Slerp(objToRotate[i].transform.rotation, openRot, timeCount);
        }

        // Play open animation
        for (int i = 0; i < objToPlayAnim.Length; i++)
        {
            if (objToPlayAnim[i].GetComponent<PlayableDirector>())
            {
                objToPlayAnim[i].GetComponent<PlayableDirector>().Play();
            }
        }

        // Add to timecount for slerp
        timeCount = timeCount + Time.deltaTime / 3f;
    }

    public void Close()
    {
        // Used to reset timeCount
        if (!startedClosing)
        {
            timeCount = 0;
            startedOpening = false;
            startedClosing = true;
        }

        // Rotate closed
        for (int i = 0; i < objToRotate.Length; i++)
        {
            objToRotate[i].transform.rotation = Quaternion.Slerp(objToRotate[i].transform.rotation, startRot, timeCount);
        }

        // Add to timecount for slerp
        timeCount = timeCount + Time.deltaTime / 3f;
    }
}
