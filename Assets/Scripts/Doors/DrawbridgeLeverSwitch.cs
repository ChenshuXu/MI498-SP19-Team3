using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawbridgeLeverSwitch : MonoBehaviour
{
    [Header("Set Manually")]
    public GameObject lower;
    private bool flipped = false;
    private Quaternion startRot;
    public GameManager.GameLevel m_NextLevel;
    [Header("Set Dynamically")]
    public GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.instance;
        startRot = lower.transform.rotation;
    }

    private void Update()
    {
        if (flipped)
        {
            Debug.Log("flipped");
            lower.transform.rotation = Quaternion.RotateTowards(lower.transform.rotation, Quaternion.Euler(0,0,0), 1f);
        }
        else if (!flipped)
        {
            lower.transform.rotation = Quaternion.RotateTowards(lower.transform.rotation, startRot, 1f);
        }
        // Z direction for rotation
        if (transform.eulerAngles.z < 40 && transform.eulerAngles.z > 27 && !flipped)
        {
            flipped = true;
        }
        else if (transform.eulerAngles.z > 327 && flipped)
        {
            flipped = false;
        }
    }
}
