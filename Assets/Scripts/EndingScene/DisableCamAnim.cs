using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCamAnim : MonoBehaviour
{
    public bool disabled = false;
    public GameObject textbox;
    public bool turnOnText = false;
    private bool done = false;
    
    
    void Update()
    {
        // Turn on/off textbox
        if (turnOnText && !textbox.activeSelf)
        {
            textbox.SetActive(true);
        }
        else if (!turnOnText && textbox.activeSelf)
        {
            textbox.SetActive(false);
        }

        // Turn off animator so it doesn't mess with the camera
        if (disabled && !done)
        {
            GetComponent<Animator>().enabled = false;
            done = true;
        }
    }
}
