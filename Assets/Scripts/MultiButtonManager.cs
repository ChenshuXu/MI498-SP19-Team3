using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiButtonManager : MonoBehaviour
{
    public enum WhatToActivate { door, elevator };

    public int totalButtons = 1;
    public WhatToActivate toActivate;
    public Elevator elevator;

    private int pressedButtons = 0;
    private RotateOpen rotateOpen;
    private bool active = false;


    void Start()
    {
        rotateOpen = GetComponent<RotateOpen>();
    }


    void Update()
    {
        if (active)
        {
            if (toActivate == WhatToActivate.door)
            {
                rotateOpen.Open();
            }
            else if (toActivate == WhatToActivate.elevator && elevator != null)
            {
                elevator.active = true;
            }
        }
        else
        {
            if (toActivate == WhatToActivate.door)
            {
                rotateOpen.Close();
            }
            else if (toActivate == WhatToActivate.elevator && elevator != null)
            {
                elevator.active = false;
            }
        }

    }


    public void ButtonPressed() {
        pressedButtons += 1;
        if(pressedButtons == totalButtons - 1 || (totalButtons == 1 && pressedButtons == 1))
        {
            active = true;
        }
    }


    public void ButtonUnpressed()
    {
        if (pressedButtons == totalButtons - 1 || (totalButtons == 1 && pressedButtons == 1))
        {
            active = false;
        }
        pressedButtons -= 1;
    }
}
