using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndFadeIn : MonoBehaviour
{
    public Image blackImg;
    public float fadeFloat = 3f;

    private void Start()
    {
        StartCoroutine(FadeBlack());
    }

    IEnumerator FadeBlack()
    {
        // Gradually become transparent
        float alpha = blackImg.color.a;
        Color newColor = blackImg.color;
        for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime / fadeFloat)
        {
            newColor = blackImg.color;
            newColor.a = Mathf.Lerp(alpha, 2, t);
            blackImg.color = newColor;
            yield return null;
        }
    }
}
