using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectableMenu : MonoBehaviour
{
    public static CollectableMenu instance;

    public Sprite goldFeather;
    public float timeKeepUp = 3f;
    public float timeMove = 1f;
    public float moveSpeed = 1f;
    public Vector3 posMoveTo;

    public Image[] feathers;

    public bool playTutorial = false;
    
    private RectTransform rectTrans;
    private Vector3 origPos;
    private bool isMoving = false;
    private bool needToAdd = false;
    private int spotToAdd = 0;


    /// <summary>
    /// Initialize the instance if not created
    /// </summary>
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
            // Destroy it
            Destroy(instance.gameObject);
            instance = this;
        }

        rectTrans = GetComponent<RectTransform>();
        origPos = rectTrans.anchoredPosition;
    }

    public void AddCollectable(int index)
    {
        if(playTutorial)
        {
            playTutorial = false;
            if(GetComponent<PlayTutorial>())
                GetComponent<PlayTutorial>().StartTut();
        }

        needToAdd = true;
        spotToAdd = index;
        StartCoroutine(ShowMenuAdd(index));
    }

    public void QuickShow(bool pause)
    {
        if(pause)
        {
            rectTrans.anchoredPosition = posMoveTo;
            StopAllCoroutines();
            if(needToAdd)
            {
                feathers[spotToAdd].sprite = goldFeather;
            }
        }
        else
        {
            rectTrans.anchoredPosition = origPos;
            StopAllCoroutines();
        }
    }

    private IEnumerator ShowMenuAdd(int ind)
    {
        //Make sure there is only one instance of this function running
        if (isMoving)
        {
            yield break; ///exit if this is still running
        }
        isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Vector3 startPos = rectTrans.anchoredPosition;

        while (counter < timeMove)
        {
            if (Time.timeScale > 0)
            {
                counter += Time.fixedDeltaTime * 2;
                rectTrans.anchoredPosition = Vector3.Lerp(startPos, posMoveTo, counter / timeMove);
            }
            yield return null;
        }

        isMoving = false;

        feathers[ind].sprite = goldFeather;
        needToAdd = false;

        yield return new WaitForSeconds(timeKeepUp);

        StartCoroutine(HideMenu());
    }

    public IEnumerator ShowMenu()
    {
        //Make sure there is only one instance of this function running
        if (isMoving)
        {
            yield break; ///exit if this is still running
        }
        isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Vector3 startPos = rectTrans.anchoredPosition;

        while (counter < timeMove)
        {
            if(Time.timeScale > 0)
            {
                counter += Time.fixedDeltaTime * 2;
                rectTrans.anchoredPosition = Vector3.Lerp(startPos, posMoveTo, counter / timeMove);
            }
            
            yield return null;
        }

        isMoving = false;
    }

    public IEnumerator HideMenu()
    {
        //Make sure there is only one instance of this function running
        if (isMoving)
        {
            yield break; ///exit if this is still running
        }
        isMoving = true;

        float counter = 0;

        //Get the current position of the object to be moved
        Vector3 startPos = rectTrans.anchoredPosition;

        while (counter < timeMove)
        {
            counter += Time.fixedDeltaTime * 2;
            rectTrans.anchoredPosition = Vector3.Lerp(startPos, origPos, counter / timeMove);
            yield return null;
        }

        isMoving = false;
    }
}
