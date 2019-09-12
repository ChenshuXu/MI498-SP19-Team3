using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeverSwitch : MonoBehaviour
{
    [Header("Set Manually")]
    public GameManager.GameLevel m_NextLevel; // Level to update reset to

    public float angle = 20f;
    public float snapAngle = 3f;
    public float snapForce = 10000f;
    public bool setCheckpoint = true;
    public bool canFlipBack = false;
    public AudioSource doorSound;
    public AudioSource switchSound;

    private GameManager gameManager;
    private RotateOpen rotateOpen;

    private bool flipped = false;


    private void Start()
    {
        gameManager = GameManager.instance;
        rotateOpen = GetComponent<RotateOpen>();
    }


    private void Update()
    {
        // Add snap force
        if (transform.localEulerAngles.z < angle && transform.localEulerAngles.z > snapAngle && !flipped)
        {
            //Debug.Log("add torque" + transform.localEulerAngles.z);
            GetComponent<Rigidbody>().AddTorque(transform.forward * snapForce);
        }

        // Z direction for rotation
        if(transform.localEulerAngles.z < 40 && transform.localEulerAngles.z > angle && !flipped)
        {
            doorSound.Play();
            switchSound.Play();

            flipped = true;
            if (setCheckpoint)
            {
                gameManager.EnterLevel(m_NextLevel);
            }
        }
        else if(transform.localEulerAngles.z > (300f + angle) && flipped && canFlipBack)
        {
            flipped = false;
        }

        // Open/close door/drawbridge
        if (flipped)
        {
            rotateOpen.Open();
        }
        else if (canFlipBack)
        {
            rotateOpen.Close();
        }
    }
}
