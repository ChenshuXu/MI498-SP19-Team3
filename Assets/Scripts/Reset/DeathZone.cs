using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used to restart when the players fall into this area
 */
public class DeathZone : MonoBehaviour
{
    private float waitTime = 0f;
    private bool dying = false;
    private InputController deadPlayer;
    private InputController deadPlayerTwo;
    private AudioManager audioManager;

    private void Start()
    {
        audioManager = AudioManager.instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Character" && !dying)
        {
            dying = true;
            deadPlayer = other.GetComponent<InputController>();
            deadPlayer.deathParticles.Play();
            deadPlayer.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            audioManager.PlaySplash(deadPlayer.playerId);
            waitTime = deadPlayer.deathParticles.main.duration * 0.75f;
            Debug.Log(waitTime);
            StartCoroutine(DelayRestart());
        }
        else if(other.tag == "Character" && dying)
        {
            // just play the particle, other character already fell in water
            deadPlayerTwo = other.GetComponent<InputController>();
            deadPlayerTwo.deathParticles.Play();
            audioManager.PlaySplash(deadPlayerTwo.playerId);
        }
    }

    private IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(waitTime);
        if(deadPlayer)
            deadPlayer.deathParticles.Stop();

        if (deadPlayerTwo)
        {
            deadPlayerTwo.deathParticles.Stop();
            deadPlayerTwo = null;
        }

        GameManager.instance.RestartLevel();
        dying = false;
    }
}
