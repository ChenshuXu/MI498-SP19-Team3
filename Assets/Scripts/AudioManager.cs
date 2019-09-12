using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;

    [Tooltip("The length of time to fade in the music")]
    public float fadeMusicDuration = 2f;
    public float fadeWalkDuration = 0.5f;
    public float fadeRopeDuration = 0.5f;

    public AudioSource backgroundMusic;
    public AudioSource waterSound;

    public AudioSource walkGSound;
    public AudioSource walkBSound;
    public AudioSource jumpGSound;
    public AudioSource jumpBSound;
    public AudioSource throwSound;

    public AudioSource extendSound;
    public AudioSource shortenSound;

    public AudioSource moveBoxSound;
    public AudioSource switchSound;
    public AudioSource gearSound;
    public AudioSource doorSound;
    public AudioSource pickupSound;
    public AudioSource runeSound;
    public AudioSource buttonSound;
    public AudioSource puzzlesolveSound;

    public AudioSource pauseSound;
    public AudioSource babyMeowSound;
    public AudioSource grandmaMeowSound;
    public AudioSource babySplashSound;
    public AudioSource grandmaSplashSound;

    private bool walkingG = false;
    private bool walkingB = false;
    private bool extendG = false;
    private bool shortG = false;
    private bool extendB = false;
    private bool shortB = false;
    private bool freezeRope = false;
    private bool movingBox = false;


    private void Awake()
    {
        // Check if instance already exists
        if (instance == null)
        {
            // if not set it to this
            instance = this;
        }
        // If instance exists and is not this
        else if (instance != this)
        {
            // Destroy this
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Fade in the background music
        if (backgroundMusic)
        {
            StartCoroutine(FadeIn(backgroundMusic, fadeMusicDuration));
        }

        if (waterSound)
        {
            //StartCoroutine(FadeIn(waterSound, fadeMusicDuration));
        }
    }

    private void Update()
    {
        if ((extendB || extendG) && (shortB || shortG) && !freezeRope)
        {
            // Don't play either if both are happening
            freezeRope = true;
            StartCoroutine(FadeOut(extendSound, fadeWalkDuration));
            StartCoroutine(FadeOut(shortenSound, fadeWalkDuration));
        }
        else
        {
            freezeRope = false;
        }
    }


    /* ------------------------ FADING AUDIO ------------------------ */
    public static IEnumerator FadeIn(AudioSource audioSource, float fadeTime)
    {
        float maxVolume = 1f;

        while (audioSource.volume < 0.99)
        {
            audioSource.volume += maxVolume * Time.deltaTime / fadeTime;

            yield return null;
        }

        audioSource.volume = maxVolume;
    }
    
    public static IEnumerator FadeOut(AudioSource audioSource, float fadeTime)
    {
        float maxVolume = audioSource.volume;

        while (audioSource.volume > 0.01)
        {
            audioSource.volume -= maxVolume * Time.deltaTime / fadeTime;

            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = maxVolume;
    }


    /* ------------------------ PLAYER AUDIO ------------------------ */
    public void PlayWalk(int player)
    {
        if (player == 0 && walkGSound && !walkingG)
        {
            walkingG = true;
            walkGSound.Play();
        }
        else if (player == 1 && walkBSound && !walkingB)
        {
            walkingB = true;
            walkBSound.Play();
        }
    }

    public void StopWalk(int player)
    {
        if (player == 0 && walkGSound && walkingG)
        {
            walkingG = false;
            StartCoroutine(FadeOut(walkGSound, fadeWalkDuration));
        }
        else if (player == 1 && walkBSound && walkingB)
        {
            walkingB = false;
            StartCoroutine(FadeOut(walkBSound, fadeWalkDuration));
        }
    }

    public void PlayJump(int player)
    {
        if (player == 0 && jumpGSound)
        {
            jumpGSound.Play();
        }
        else if (player == 1 && jumpBSound)
        {
            jumpGSound.Play();
        }
    }

    public void PlayThrow()
    {
        if (throwSound)
        {
            throwSound.Play();
        }
    }

    /* ------------------------- ROPE AUDIO ------------------------- */
    public void PlayExtend(int player)
    {
        if (extendSound && (!extendB || !extendG) && !freezeRope)
        {
            if (!extendG && !extendB)
            {
                extendSound.Play();
            }

            if (player == 0)
            {
                extendG = true;
            }
            else if (player == 1)
            {
                extendB = true;
            }
        }
    }

    public void StopExtend(int player)
    {
        if (extendSound && (extendG || extendB))
        {
            if (player == 0)
            {
                extendG = false;
            }
            else if (player == 1)
            {
                extendB = false;
            }

            if (!extendB && !extendG)
            {
                StartCoroutine(FadeOut(extendSound, fadeWalkDuration));
            }
        }
    }

    public void PlayShorten(int player)
    {
        if (shortenSound && (!shortB || !shortG) && !freezeRope)
        {
            if (!shortG && !shortB)
            {
                shortenSound.Play();
            }

            if (player == 0)
            {
                shortG = true;
            }
            else if (player == 1)
            {
                shortB = true;
            }
        }
    }

    public void StopShorten(int player)
    {
        if (shortenSound && (shortG || shortB))
        {
            if (player == 0)
            {
                shortG = false;
            }
            else if (player == 1)
            {
                shortB = false;
            }

            if (!shortB && !shortG)
            {
                StartCoroutine(FadeOut(shortenSound, fadeWalkDuration));
            }
        }
    }

    /* ------------------------- BOX AUDIO ------------------------- */
    public void PlayMoveBox()
    {
        if (moveBoxSound && !movingBox)
        {
            movingBox = true;
            moveBoxSound.Play();
        }
    }

    public void StopMoveBox()
    {
        if (moveBoxSound && movingBox)
        {
            movingBox = false;
            moveBoxSound.Stop();
        }
    }


    public void PlaySwitch()
    {
        if(switchSound)
        {
            switchSound.Play();
        }
    }

    public void PlayPuzzle()
    {
        if (puzzlesolveSound)
        {
            puzzlesolveSound.Play();
        }
    }

    public void PlayGear()
    {
        if(gearSound && !gearSound.isPlaying)
        {
            gearSound.Play();
        }
    }
    public void PlayPickup()
    {
        if(pickupSound)
        {
            pickupSound.Play();
        }
    }

    public void PlayButton()
    {
        if (buttonSound)
        {
           buttonSound.Play();
        }
    }

    public void PlayRune()
    {
        if(runeSound && !runeSound.isPlaying)
        {
            runeSound.Play();
        }
    }

    public void PlayDoor()
    {
        if(doorSound && !doorSound.isPlaying)
        {
            doorSound.Play();
        }
    }

    public void PlayPause()
    {
        if (pauseSound)
        {
            pauseSound.Play();
        }
    }

    public void PlayMeow(int playerId)
    {
        if (playerId == 0 && grandmaMeowSound)
        {
            grandmaMeowSound.Play();
        }
        else if (playerId == 1 && babyMeowSound)
        {
            babyMeowSound.Play();
        }
    }

    public void PlaySplash(int playerId)
    {
        if (playerId == 0 && babySplashSound)
        {
            babySplashSound.Play();
        }
        else if (playerId == 1 && grandmaSplashSound)
        {
            grandmaSplashSound.Play();
        }
    }
}
