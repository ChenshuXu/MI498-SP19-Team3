using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuOptions : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditsMenu;
    public GameObject controlsMenu;
    public GameObject citationMenu;
    public GameObject m_BlackCanvas;
    public bool m_EnableLoadingScreen;

    void Awake()
    {
        if (m_EnableLoadingScreen && m_BlackCanvas)
        {
            Color newColor = m_BlackCanvas.GetComponent<Image>().color;
            newColor.a = 1.0f;
            m_BlackCanvas.GetComponent<Image>().color = newColor;
        }
    }

    void OnEnable()
    {
        Debug.Log("OnEnable called");
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        Debug.Log("OnDisable");
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded: " + scene.name);
        Debug.Log(mode);
        if (m_EnableLoadingScreen && m_BlackCanvas)
        {
            // Then gradually become transparent
            StartCoroutine(FadeToTransparent(1.5f, 1.0f, m_BlackCanvas));
        }
    }

    /// <summary>
    /// Black out the screen for few seconds
    /// Change the alpha color of the black canvas gradually to 0
    /// </summary>
    private IEnumerator FadeToTransparent(float blackOutTime, float fadeTime, GameObject obj)
    {
        // Become black for seconds
        yield return new WaitForSecondsRealtime(blackOutTime);

        // Gradually become transparent
        float alpha = obj.GetComponent<Image>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = obj.GetComponent<Image>().color;
            newColor.a = Mathf.Lerp(alpha, 0, t);
            obj.GetComponent<Image>().color = newColor;
            //Debug.Log("newcolor " + newColor);
            yield return null;
        }
    }

    /// <summary>
    /// Loads a level in the game
    /// </summary>
    /// <param name="level">The name of the level</param>
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
        //StartCoroutine(LoadYourAsyncScene(level));
    }

    /// <summary>
    /// Loads a level in the game
    /// </summary>
    /// <param name="level">The name of the level</param>
    IEnumerator LoadYourAsyncScene(string level)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }


    /// <summary>
    /// Switches to a different menu view
    /// </summary>
    /// <param name="menu">The name of the menu</param>
    public void ShowMenu(string menu)
    {
        if (menu == "Main")
        {
            creditsMenu.SetActive(false);
            controlsMenu.SetActive(false);
            mainMenu.SetActive(true);
            citationMenu.SetActive(false);
        }
        else if (menu == "Credits")
        {
            mainMenu.SetActive(false);
            controlsMenu.SetActive(false);
            creditsMenu.SetActive(true);
            citationMenu.SetActive(false);
        }
        else if (menu == "Controls")
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(false);
            controlsMenu.SetActive(true);
            citationMenu.SetActive(false);

        }
        else if (menu == "Citations")
        {
            mainMenu.SetActive(false);
            creditsMenu.SetActive(false);
            controlsMenu.SetActive(false);
            citationMenu.SetActive(true);
        }
    }


    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }
}
