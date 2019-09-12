using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    public Camera grassCam;
    public Camera mainCam;

    private void Start()
    {
        grassCam = GetComponent<Camera>();
        mainCam = transform.parent.GetComponent<Camera>();
    }

    private void Update()
    {
        grassCam.fieldOfView = mainCam.fieldOfView;
    }
}
