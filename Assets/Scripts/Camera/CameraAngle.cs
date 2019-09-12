using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to get the y rotation only of the camera to give to the controllers
/// </summary>
public class CameraAngle : MonoBehaviour
{
    void Start()
    {
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }

    void Update()
    {
        transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    }
}
