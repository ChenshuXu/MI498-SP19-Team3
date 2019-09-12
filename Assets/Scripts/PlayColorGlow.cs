using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayColorGlow : MonoBehaviour
{
    // The input of the players
    private InputController input;
    public GameObject colorGlow;
    public GameObject stars;
    // Used to control the animations
    public Animator animController;
    // Start is called before the first frame update
    void Start()
    {
        input = GetComponent<InputController>();
        colorGlow.SetActive(false);
        stars.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        if (animController.GetBool("changeScarfInc") || animController.GetBool("changeScarfDec"))
        {
            //Debug.Log("Got this box");
            colorGlow.SetActive(true);
            stars.SetActive(true);
        }
        else
        {
            colorGlow.SetActive(false);
            stars.SetActive(false);
        }
    }
}
