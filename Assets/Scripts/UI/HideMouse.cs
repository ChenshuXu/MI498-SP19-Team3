using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HideMouse : MonoBehaviour
{
    public bool startHidden = true;

    private EventSystem eventSystem;
    private GameObject currSelected;

    private void Start()
    {
        eventSystem = EventSystem.current;

        //Set cursor to not be visible
        if(startHidden)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        currSelected = eventSystem.currentSelectedGameObject;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            // Make sure that clicking the mouse doesn't change anything on the menus
            eventSystem.SetSelectedGameObject(currSelected);
        }
        currSelected = eventSystem.currentSelectedGameObject;
    }
}
