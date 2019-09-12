using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndTextBox : MonoBehaviour
{
    public string levelToLoad = "MainMenu";
    public GameObject textbox;
    public Image blackImg;
    public Text continueText;
    public bool turnOn = false;
    public bool startFading = false;
    public float fadeFloat = 0.5f;

    private bool startedFade = false;

    private void Update()
    {
        // Turn on/off textbox
        if (turnOn && !textbox.activeSelf)
        {
            textbox.SetActive(true);
        }
        else if (!turnOn && textbox.activeSelf)
        {
            textbox.SetActive(false);
        }

        // Start fading to black
        if(!startedFade && startFading)
        {
            startedFade = true;
            StartCoroutine(FadeBlack());
        }
    }

    IEnumerator FadeBlack()
    {
        // Gradually fade to black
        float alpha = blackImg.color.a;
        Color newColor = blackImg.color;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeFloat)
        {
            newColor = blackImg.color;
            newColor.a = Mathf.Lerp(alpha, 2, t);
            blackImg.color = newColor;
            yield return null;
        }

        StartCoroutine(FadeText());
    }

    IEnumerator FadeText()
    {
        // Gradually fade in text
        float alpha = continueText.color.a;
        Color newColor = continueText.color;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeFloat)
        {
            newColor = continueText.color;
            newColor.a = Mathf.Lerp(alpha, 2, t);
            continueText.color = newColor;
            yield return null;
        }

        StartCoroutine(FadeOutText());
    }

    IEnumerator FadeOutText()
    {
        // Gradually fade in text
        float alpha = continueText.color.a;
        Color newColor = continueText.color;
        for (float t = 1.0f; t > 0.0f; t -= Time.deltaTime / fadeFloat)
        {
            newColor = continueText.color;
            newColor.a = Mathf.Lerp(alpha, 2, t);
            continueText.color = newColor;
            yield return null;
        }

        SceneManager.LoadScene(levelToLoad);
    }
}
