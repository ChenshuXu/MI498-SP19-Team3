using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayTutorial : MonoBehaviour
{
    private bool donePlaying = false;
    private bool showText = false;
    public float fadeMusicDuration = 2f;
    public GameObject tutorialBox;
    public Image pauseFill;
    public AudioSource GrandmaSound;
    
    private GameManager gameManager;


    void Start()
    {
        tutorialBox.SetActive(false);
        gameManager = GameManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character" && donePlaying == false)
        {
            // call function in GameManager to get UI displayer position
            //Vector3 displayPos = gameManager.CalculateUIElementPlaceHolderPosition(other.gameObject);

            // find character location on the screen
            //Vector3 screenPos = gameManager.m_Camera.GetComponent<Camera>().WorldToScreenPoint(displayPos);
            //Debug.Log("target is " + screenPos.x + " pixels from the left, " + screenPos.y + " pixels from the button");

            StartTut();
        }
    }

    public void StartTut()
    {
        tutorialBox.SetActive(true);
        donePlaying = true;
        showText = true;
        GrandmaSound.Play();

        // pause game
        gameManager.EnterTutorial(gameObject, pauseFill);
    }

    public void ExitTrigger()
    {
        tutorialBox.SetActive(false);
        showText = false;
    }
    
    /*public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float maxVolume = 1f;

        while (audioSource.volume < 0.99)
        {
            audioSource.volume += maxVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = 1f;
    }

    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float maxVolume = 1f;

        while (audioSource.volume > 0.01)
        {
            audioSource.volume -= maxVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = 1f;
    }*/
}
