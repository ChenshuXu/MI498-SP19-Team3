using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Rewired;
using cakeslice;

/// <summary>
/// Game manager controls:
/// Quitting
/// Pausing game
/// Resetting
/// </summary>
public class GameManager : MonoBehaviour
{
    public enum GameLevel
    {
        Level1, Level2, Level3, Level4, Level5, Level6, Level7, Level8, Level9, Level10, Level11, Final
    }

    public static GameManager instance;

    [Header("Set Manually")]
    public GameObject HingeRopePlayersPrefab;
    public GameObject m_BlackCanvas;
    public float m_BlackOutTime = 0.4f;
    public float m_FadeTime = 1.0f;
    public float m_RestartCooldownTime = 3.0f;
    public float m_OutlineShowTimeDelay = 0.2f;
    public float timeHoldUnpause = 1.0f;
    public GameObject m_PuzzleCameras;
    public bool m_EnableLoadingScreen = true;
    public bool m_EnableDebugTool;

    [Header("Set Dynamically")]
    public GameObject m_Camera;
    public GameObject m_HingeRopePlayers;
    public GameLevel m_CurrentLevel;
    public GameObject m_PauseMenu;
    public GameObject level1StartPoint;
    public GameObject level2StartPoint;
    public GameObject level3StartPoint;
    public GameObject level4StartPoint;
    public GameObject level5StartPoint;
    public GameObject level6StartPoint;
    public GameObject level7StartPoint;
    public GameObject level8StartPoint;
    public GameObject level9StartPoint;
    public GameObject level10StartPoint;
    public GameObject level11StartPoint;
    public GameObject m_GrandmaObj;
    public GameObject m_BBcatObj;
    public GameObject m_RopeObj;
    public GameObject m_CameraHolder;
    public GameObject m_UIElementPlaceHolder;
    public Cinemachine.CinemachineTargetGroup targetGroup;
    public bool m_CanRestart = true;
    public GameObject cameraAngle;
    private Player m_Player0; // The Rewired Player0
    private Player m_Player1; // The Rewired Player1

    public bool player0Blocked = false;
    public bool player1Blocked = false;
    public bool tutUnpausing = false;

    private bool m_Paused = false;
    private bool m_TutorialPause = false;
    private GameObject tutorialTriggerObj;
    private float tmp_restartCooldown = 0.0f;
    private float tmp_m_OutlineShowTimeDelay = 0.0f;
    private float timeHolding = 0f;
    private Image pauseHoldImage;

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
            // Destroy old GameManager
            Destroy(instance.gameObject);
            instance = this;
        }

        // Sets this to not be destroyed when reloading scene
        //DontDestroyOnLoad(gameObject);

        if (m_EnableLoadingScreen && m_BlackCanvas)
        {
            Color newColor = m_BlackCanvas.GetComponent<Image>().color;
            newColor.a = 1.0f;
            m_BlackCanvas.GetComponent<Image>().color = newColor;
        }

        m_HingeRopePlayers = GameObject.Find("HingeRope");

        // Initializes an empty game object that tracks the
        // camera Y angle for player movement direction
        cameraAngle = new GameObject("GetsCameraYAngle");
        cameraAngle.AddComponent<CameraAngle>();

        // Set up grandma and baby input controllers
        m_GrandmaObj = GameObject.Find("Grandma");
        m_BBcatObj = GameObject.Find("BbCat");
        m_RopeObj = GameObject.Find("Obi Rope");

        InputController babyInput = m_BBcatObj.GetComponent<InputController>();
        InputController gmaInput = m_GrandmaObj.GetComponent<InputController>();
        PickupController gmaPickup = m_GrandmaObj.GetComponent<PickupController>();

        gmaInput.grandma = babyInput.grandma = gmaInput;
        gmaInput.baby = babyInput.baby = babyInput;
        gmaInput.gmaPickupCont = babyInput.gmaPickupCont = gmaPickup;

        // Get camera
        m_Camera = GameObject.Find("Main Camera");

        // Get camera holder
        m_CameraHolder = GameObject.Find("TargetGroupFollow");

        // Update camera tracking
        targetGroup = m_CameraHolder.GetComponent<Cinemachine.CinemachineTargetGroup>();

        targetGroup.m_Targets[0].target = m_GrandmaObj.transform;
        targetGroup.m_Targets[1].target = m_BBcatObj.transform;

        if(m_PuzzleCameras)
        {
            foreach (Transform child in m_PuzzleCameras.transform)
            {
                child.gameObject.GetComponent<PuzzleCameraCine>().Reset(m_GrandmaObj, m_BBcatObj);
            }
        }
        else
        {
            Debug.Log("NO PUZZLE CAMERAS");
        }

        // Update level start positions
        // TODO: find a better way to get all level start points, instead of get manually
        level1StartPoint = GameObject.Find("Level1 start point");
        level2StartPoint = GameObject.Find("Level2 start point");
        level3StartPoint = GameObject.Find("Level3 start point");
        level4StartPoint = GameObject.Find("Level4 start point");
        level5StartPoint = GameObject.Find("Level5 start point");
        level6StartPoint = GameObject.Find("Level6 start point");
        level7StartPoint = GameObject.Find("Level7 start point");
        level8StartPoint = GameObject.Find("Level8 start point");
        level9StartPoint = GameObject.Find("Level9 start point");
        level10StartPoint = GameObject.Find("Level10 start point");
        level11StartPoint = GameObject.Find("Level11 start point");

        // Update pause menu
        m_PauseMenu = GameObject.Find("PauseMenu");

        Debug.Log("players set");

        if (m_PauseMenu)
        {
            m_PauseMenu.SetActive(false);
        }

        m_Player0 = ReInput.players.GetPlayer(0);
        m_Player1 = ReInput.players.GetPlayer(1);

        // Find UI element
        if (GameObject.Find("UIElementPlaceHolder"))
        {
            m_UIElementPlaceHolder = GameObject.Find("UIElementPlaceHolder");
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
    /// Check if we should quit
    /// </summary>
    private void Update()
    {
        if (!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.

        if (!m_TutorialPause && (Input.GetKeyDown(KeyCode.Escape) || m_Player0.GetButtonDown("Pause") || m_Player1.GetButtonDown("Pause")))
        {
            PauseGame();
        }

        // Remove tutorial after holding down A for a certain amount of time
        if (m_TutorialPause && !tutUnpausing && (m_Player0.GetButton("Jump") || m_Player1.GetButton("Jump")))
        {
            timeHolding += (Time.unscaledDeltaTime / 2f);

            // Fill image
            pauseHoldImage.fillAmount = timeHolding / timeHoldUnpause;

            if (timeHolding > timeHoldUnpause)
            {
                tutUnpausing = true;
                ExitTutorial();
            }
        }
        else if(m_TutorialPause && !tutUnpausing &&
            ((m_Player0.GetButtonUp("Jump") && !m_Player1.GetButton("Jump"))
            || (m_Player1.GetButtonUp("Jump") && !m_Player0.GetButton("Jump"))))
        {
            timeHolding = 0f;
            pauseHoldImage.fillAmount = 0f;
        }

        if (tmp_restartCooldown >= 0.0f)
        {
            tmp_restartCooldown -= Time.deltaTime;
        }

        if (tmp_m_OutlineShowTimeDelay >= 0.0f)
        {
            tmp_m_OutlineShowTimeDelay -= Time.deltaTime;
        }
        CheckBlock();

        // Update UI element place holder direction
        if(m_UIElementPlaceHolder)
            m_UIElementPlaceHolder.transform.eulerAngles = new Vector3(0, Camera.main.transform.eulerAngles.y, 0);
    
        // For debug use
        // Jump to previous/next check point
        // 1 for next, 2 for previous
        if (m_EnableDebugTool)
        {
            tmp_restartCooldown = -1.0f;
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                EnterLevel(m_CurrentLevel+1);
                RestartLevel();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                EnterLevel(m_CurrentLevel-1);
                RestartLevel();
            }
        }
    }

    public void EnterLevel(GameLevel level)
    {
        Debug.Log("Enter level" + level);
        m_CurrentLevel = level;
    }

    public void RestartLevel()
    {
        if (!m_CanRestart || tmp_restartCooldown > 0)
        {
            return;
        }
        Debug.Log("Restart level" + m_CurrentLevel);
        tmp_restartCooldown = m_RestartCooldownTime;
        
        StartCoroutine(FadeToBlack(0.1f, m_BlackCanvas));

        // Then gradually become transparent
        StartCoroutine(FadeToTransparent(1.0f, 1.0f, m_BlackCanvas));
    }

    private GameObject GetStartPosition()
    {
        switch (m_CurrentLevel)
        {
            case GameLevel.Level1:
                return level1StartPoint;
            case GameLevel.Level2:
                return level2StartPoint;
            case GameLevel.Level3:
                return level3StartPoint;
            case GameLevel.Level4:
                return level4StartPoint;
            case GameLevel.Level5:
                return level5StartPoint;
            case GameLevel.Level6:
                return level6StartPoint;
            case GameLevel.Level7:
                return level7StartPoint;
            case GameLevel.Level8:
                return level8StartPoint;
            case GameLevel.Level9:
                return level9StartPoint;
            case GameLevel.Level10:
                return level10StartPoint;
            case GameLevel.Level11:
                return level11StartPoint;
            case GameLevel.Final:
                return level1StartPoint;
        }
        return level1StartPoint;
    }

    public void ResetToPosition(GameObject positionObj)
    {
        // Reset level objects
        if (positionObj.GetComponent<LevelResetter>() != null)
        {
            positionObj.GetComponent<LevelResetter>().Reset();
        }

        // Destroy players
        Destroy(m_HingeRopePlayers);
        m_HingeRopePlayers = Instantiate(HingeRopePlayersPrefab);
        m_HingeRopePlayers.transform.position = positionObj.transform.position;

        // Update gameobject links
        m_GrandmaObj = m_HingeRopePlayers.transform.Find("Grandma").gameObject;
        m_BBcatObj = m_HingeRopePlayers.transform.Find("BbCat").gameObject;
        m_RopeObj = m_HingeRopePlayers.transform.Find("Obi Rope").gameObject;

        InputController babyInput = m_BBcatObj.GetComponent<InputController>();
        InputController gmaInput = m_GrandmaObj.GetComponent<InputController>();
        PickupController gmaPickup = m_GrandmaObj.GetComponent<PickupController>();

        gmaInput.grandma = babyInput.grandma = gmaInput;
        gmaInput.baby = babyInput.baby = babyInput;
        gmaInput.gmaPickupCont = babyInput.gmaPickupCont = gmaPickup;

        // Reset Puzzle cameras
        foreach (Transform child in m_PuzzleCameras.transform)
        {
            child.gameObject.GetComponent<PuzzleCameraCine>().Reset(m_GrandmaObj, m_BBcatObj);
        }

        // Update camera tracking
        targetGroup.m_Targets[0].target = m_GrandmaObj.transform;
        targetGroup.m_Targets[1].target = m_BBcatObj.transform;
    }

    private IEnumerator FadeToBlack(float fadeTime, GameObject obj)
    {
        // Gradually become transparent
        float alpha = obj.GetComponent<Image>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = obj.GetComponent<Image>().color;
            newColor.a = Mathf.Lerp(alpha, 2, t);
            obj.GetComponent<Image>().color = newColor;
            //Debug.Log("newcolor " + newColor);
            yield return null;
        }

        // Reset level objects after black out
        switch (m_CurrentLevel)
        {
            case GameLevel.Level1:
                ResetToPosition(level1StartPoint);
                break;
            case GameLevel.Level2:
                ResetToPosition(level2StartPoint);
                break;
            case GameLevel.Level3:
                ResetToPosition(level3StartPoint);
                break;
            case GameLevel.Level4:
                ResetToPosition(level4StartPoint);
                break;
            case GameLevel.Level5:
                ResetToPosition(level5StartPoint);
                break;
            case GameLevel.Level6:
                ResetToPosition(level6StartPoint);
                break;
            case GameLevel.Level7:
                ResetToPosition(level7StartPoint);
                break;
            case GameLevel.Level8:
                ResetToPosition(level8StartPoint);
                break;
            case GameLevel.Level9:
                ResetToPosition(level9StartPoint);
                break;
            case GameLevel.Level10:
                ResetToPosition(level10StartPoint);
                break;
            case GameLevel.Level11:
                ResetToPosition(level11StartPoint);
                break;
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
    /// Pauses and unpauses the game
    /// </summary>
    public void PauseGame()
    {
        if(!m_Paused)
        {
            // Pause the game
            AudioManager.instance.PlayPause();
            if (!m_TutorialPause)
            {
                //StartCoroutine(CollectableMenu.instance.ShowMenu());
                CollectableMenu.instance.QuickShow(true);
                m_PauseMenu.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

            m_Paused = true;
            Time.timeScale = 0;

            // Select first button for controller
            EventSystem es = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            es.SetSelectedGameObject(null);
            es.SetSelectedGameObject(es.firstSelectedGameObject);

            // Freeze grandma
            m_GrandmaObj.GetComponent<InputController>().enabled = false;
            m_GrandmaObj.GetComponent<Rigidbody>().isKinematic = true;
            m_GrandmaObj.GetComponent<Rigidbody>().useGravity = false;

            // Freeze baby but don't affect rigidbody if held
            if(!m_GrandmaObj.GetComponent<InputController>().haveBbcat)
            {
                m_BBcatObj.GetComponent<Rigidbody>().isKinematic = true;
                m_BBcatObj.GetComponent<Rigidbody>().useGravity = false;
            }
            m_BBcatObj.GetComponent<InputController>().enabled = false;
        }
        else
        {
            // Unpause the game
            AudioManager.instance.PlayPause();
            if (m_PauseMenu && !m_TutorialPause)
            {
                //StartCoroutine(CollectableMenu.instance.HideMenu());
                CollectableMenu.instance.QuickShow(false);
                m_PauseMenu.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            m_Paused = false;
            Time.timeScale = 1;

            // Unfreeze grandma
            m_GrandmaObj.GetComponent<InputController>().enabled = true;
            m_GrandmaObj.GetComponent<Rigidbody>().isKinematic = false;
            m_GrandmaObj.GetComponent<Rigidbody>().useGravity = true;

            // Unfreeze baby but don't affect rigidbody if held
            if (!m_GrandmaObj.GetComponent<InputController>().haveBbcat)
            {
                m_BBcatObj.GetComponent<Rigidbody>().isKinematic = false;
                m_BBcatObj.GetComponent<Rigidbody>().useGravity = true;
            }
            m_BBcatObj.GetComponent<InputController>().enabled = true;
        }
    }

    public Vector3 CalculateUIElementPlaceHolderPosition(GameObject player)
    {
        Vector3 holderPos = new Vector3(0,0,0);
        // Check which player touched the trigger
        
        m_UIElementPlaceHolder.transform.position = player.transform.position;

        // Check which side that player at
        Vector3 LRealPos = m_UIElementPlaceHolder.transform.Find("L").gameObject.transform.position;
        Vector3 RRealPos = m_UIElementPlaceHolder.transform.Find("R").gameObject.transform.position;

        Vector3 grandMaScreenPos = m_Camera.GetComponent<Camera>().WorldToScreenPoint(m_GrandmaObj.transform.position);
        Vector3 BBcatScreenPos = m_Camera.GetComponent<Camera>().WorldToScreenPoint(m_BBcatObj.transform.position);
        if (player == m_GrandmaObj)
        {
            if (grandMaScreenPos.x > BBcatScreenPos.x)
            {
                holderPos = RRealPos;
            }
            else
            {
                holderPos = LRealPos;
            }
        }
        else if (player == m_BBcatObj)
        {
            if (grandMaScreenPos.x > BBcatScreenPos.x)
            {
                holderPos = LRealPos;
            }
            else
            {
                holderPos = RRealPos;
            }
        }
        
        return holderPos;
    }

    /// <summary>
    /// Pauses the game when enter the tutorial area
    /// </summary>
    public void EnterTutorial(GameObject tutorialUIObj, Image fillImage)
    {
        tutorialTriggerObj = tutorialUIObj;
        pauseHoldImage = fillImage;
        m_TutorialPause = true;
        PauseGame();
    }

    /// <summary>
    /// Unpauses the game when press A
    /// </summary>
    public void ExitTutorial()
    {
        tutorialTriggerObj.GetComponent<PlayTutorial>().ExitTrigger();
        PauseGame();
        // Set m_TutorialPause to false after 0.5 seconds to avoid action 
        // conflict with exit tutorial action
        StartCoroutine(WaitToExitPause());
    }

    private IEnumerator WaitToExitPause()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        m_TutorialPause = false;
        tutUnpausing = false;
        timeHolding = 0f;
    }

    public bool GetTutorialPause()
    {
        return m_TutorialPause;
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

    /// <summary>
    /// Loads a level in the game
    /// </summary>
    /// <param name="level">The name of the level</param>
    public void LoadLevel(string level)
    {
        m_Paused = false;
        Time.timeScale = 1;
        if (m_PauseMenu)
        {
            m_PauseMenu.SetActive(false);
        }

        //SceneManager.LoadScene(level);
        StartCoroutine(FadeThenLoadLevel(1.0f, m_BlackCanvas, level));
    }

    /// <summary>
    /// Fade the black canvas to black then load next level
    /// </summary>
    private IEnumerator FadeThenLoadLevel(float fadeTime, GameObject obj, string level)
    {
        // Gradually become transparent
        float alpha = obj.GetComponent<Image>().color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            Color newColor = obj.GetComponent<Image>().color;
            newColor.a = Mathf.Lerp(alpha, 2, t);
            obj.GetComponent<Image>().color = newColor;
            //Debug.Log("newcolor " + newColor);
            yield return null;
        }

        // Load level objects after black out
        SceneManager.LoadScene(level);
        /*
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);
        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        */
    }

    /// <summary>
    /// Check if need to disable outline effect
    /// </summary>
    private void CheckBlock()
    {
        cakeslice.Outline outline = m_RopeObj.GetComponent<cakeslice.Outline>();
        if (outline == null)
        {
            return;
        }
        if (tmp_m_OutlineShowTimeDelay > 0)
        {
            outline.enabled = true;
        }
        else
        {
            outline.enabled = false;
            player0Blocked = false;
            player1Blocked = false;
        }
    }

    public void BlockSetter(int playerId)
    {
        if (playerId == 0)
        {
            player0Blocked = true;
            tmp_m_OutlineShowTimeDelay = m_OutlineShowTimeDelay;
        }
        else if (playerId == 1)
        {
            player1Blocked = true;
            tmp_m_OutlineShowTimeDelay = m_OutlineShowTimeDelay;
        }
    }


    public void ChangeCamTargets(bool holdingBaby)
    {
        if(holdingBaby)
        {
            targetGroup.m_Targets[1].target = null;

            foreach (Transform child in m_PuzzleCameras.transform)
            {
                if(child.gameObject.GetComponent<PuzzleCameraCine>().targetGroup)
                    child.gameObject.GetComponent<PuzzleCameraCine>().targetGroup.m_Targets[1].target = null;
            }
        }
        else
        {
            targetGroup.m_Targets[1].target = m_BBcatObj.transform;

            foreach (Transform child in m_PuzzleCameras.transform)
            {
                if (child.gameObject.GetComponent<PuzzleCameraCine>().targetGroup)
                    child.gameObject.GetComponent<PuzzleCameraCine>().targetGroup.m_Targets[1].target = m_BBcatObj.transform;
            }
        }
    }
}
