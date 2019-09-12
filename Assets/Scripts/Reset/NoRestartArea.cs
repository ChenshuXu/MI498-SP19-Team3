using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoRestartArea : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Character")
        {
            GameManager.instance.m_CanRestart = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Character")
        {
            GameManager.instance.m_CanRestart = true;
        }
    }
}
