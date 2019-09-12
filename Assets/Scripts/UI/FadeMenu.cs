using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeMenu : MonoBehaviour
{
    public float fadeTimeWait = 0.5f;

    private Image blackImg;
    private Color blackCol;

    private bool fadeOut = false;

    private void Awake()
    {
        blackImg = GetComponent<Image>();
        blackCol = blackImg.color;
        blackCol.a = 1;
        blackImg.color = blackCol;
    }

    private void Start()
    {
        StartCoroutine(FadeWait());
    }

    // Update is called once per frame
    void Update()
    {
        if(fadeOut)
        {
            blackCol.a -= Time.deltaTime;
            blackImg.color = blackCol;
        }
    }

    IEnumerator FadeWait()
    {
        yield return new WaitForSeconds(fadeTimeWait);
        fadeOut = true;
    }
}
