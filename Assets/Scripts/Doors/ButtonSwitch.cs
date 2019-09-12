using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Button that can be pressed by a box or player that opens a door
/// </summary>
public class ButtonSwitch : MonoBehaviour
{
    public bool boxWorks = true;
    public bool playerWorks = true;
    public float pressY;

    private float initY;
    private bool pressedBox = false;
    private int playersOn = 0;
    private bool pressed = false;
    private AudioManager audioManager;

    private RotateOpen rotateOpen;
    private MultiButtonManager mbm;


    private void Start()
    {
        // Get the initial y height for unpressed button
        initY = transform.localPosition.y;
        //rotateOpen = GetComponent<RotateOpen>();
        mbm = this.transform.parent.gameObject.GetComponent<MultiButtonManager>();
        audioManager = AudioManager.instance;
    }


    private void OnTriggerEnter(Collider other)
    {
        // Something is on the button
        if ((other.tag == "Grabable" || other.tag == "FakeBox" || other.tag == "Grab") && boxWorks)
        {
            pressedBox = true;
            PressButton();
         
        }
        else if (other.tag == "Character" && playerWorks)
        {
            playersOn++;
            PressButton();
           
        }
    }


    private void OnTriggerExit(Collider other)
    {
        // Something got off the button
        if ((other.tag == "Grabable" || other.tag == "FakeBox") && boxWorks)
        {
            pressedBox = false;
        }
        else if (other.tag == "Character" && playerWorks)
        {
            playersOn--;
        }

        if (!pressedBox && playersOn == 0)
        {
            // nothing is on the switch
            pressed = false;
            mbm.ButtonUnpressed();
            transform.localPosition = new Vector3(transform.localPosition.x, initY, transform.localPosition.z);
        }
    }


    private void PressButton()
    {
        // Only affect if not yet affected
        if (!pressed)
        {
            Debug.Log("Button pressed.");
            pressed = true;
            mbm.ButtonPressed();
            transform.localPosition = new Vector3(transform.localPosition.x, pressY, transform.localPosition.z);
            audioManager.PlayButton();
        }
    }


    public void ResetButton()
    {
        if(pressed)
        {
            playersOn = 0;
            pressed = false;
            pressedBox = false;
            mbm.ButtonUnpressed();
            transform.localPosition = new Vector3(transform.localPosition.x, initY, transform.localPosition.z);
        }
    }
}
