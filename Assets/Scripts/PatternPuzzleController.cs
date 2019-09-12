using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using Cinemachine;

[RequireComponent(typeof(ObiSolver))]
public class PatternPuzzleController : MonoBehaviour
{
    [Header("Set Manually")]
    public bool m_Debug = true;
    public GameObject textToShow;
    public Elevator elevator;
    public float m_DelayTime = 1.0f;
    private float tmp_DelayTime;
    public string[] correctPatterns;
    [Header("Set Dynamically")]
    public List<GameObject> m_AllCylinders;
    public List<GameObject> m_AllFloors;
    public bool[] m_CorrectPattern;
    public bool[] m_CurrentPattern;
    public bool[] m_CurrentCylinder;
    public bool[] m_CurrentFloor;

    private bool completed = false;
    private AudioManager audioManager;

    void Start()
    {
        tmp_DelayTime = m_DelayTime;

        if (textToShow)
        {
            textToShow.SetActive(false);
        }

        // Initalize index settings
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject cylinder = transform.GetChild(i).gameObject;
            m_AllCylinders.Add(cylinder);
            GameObject floor = cylinder.transform.GetChild(0).gameObject;
            m_AllFloors.Add(floor);
            cylinder.GetComponent<PatternPuzzlePointChecker>().m_CheckPointIndex = i;
            cylinder.GetComponent<PatternPuzzlePointChecker>().m_Type = PatternPuzzlePointChecker.CheckPointType.cylinder;
            floor.GetComponent<PatternPuzzlePointChecker>().m_CheckPointIndex = i;
            floor.GetComponent<PatternPuzzlePointChecker>().m_Type = PatternPuzzlePointChecker.CheckPointType.floor;
        }

        // Initalize parameters
        int cylinderCount = m_AllCylinders.Count;
        m_CorrectPattern = new bool[cylinderCount];
        m_CurrentPattern = new bool[cylinderCount];
        m_CurrentCylinder = new bool[cylinderCount];
        m_CurrentFloor = new bool[cylinderCount];
        for (int i=0; i<cylinderCount; i++)
        {
            m_CurrentCylinder[i] = false;
            m_CurrentFloor[i] = false;
            m_CurrentPattern[i] = false;
        }

        LoadPattern(correctPatterns[0]);

        audioManager = AudioManager.instance;
    }

    void LateUpdate()
    {
        if (CheckPattern())
        {
            tmp_DelayTime -= Time.deltaTime;
        }

        if (tmp_DelayTime <= 0)
        {
            //if (textToShow)
            //{
            //    textToShow.SetActive(true);
            //}
            //StartCoroutine(WaitAndDisappear(4.0f));
            for (int i = 0; i < correctPatterns[0].Length; i++)
            {
                if (correctPatterns[0].Substring(i, 1) == "1")
                {
                    m_AllCylinders[i].GetComponent<PatternPuzzlePointChecker>().Complete();
                }
            }
            elevator.active = true;
            tmp_DelayTime = m_DelayTime;

            if(!completed)
            {
                // Camera shake
                GetComponent<CinemachineImpulseSource>().GenerateImpulse();
                completed = true;
                // Audio
                audioManager.PlayPuzzle();
            }
        }
    }

    private IEnumerator WaitAndDisappear(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if (textToShow)
        {
            textToShow.SetActive(false);
        }
    }

    /// <summary>
    /// Load correct pattern string
    /// String length must be 9
    /// </summary>
    private void LoadPattern(string correctPattern)
    {
        if (correctPattern.Length != 9)
        {
            return;
        }

        for (int i = 0; i < correctPattern.Length; i++)
        {
            if (correctPattern.Substring(i, 1) == "0")
            {
                m_CorrectPattern[i] = false;
            }
            else if (correctPattern.Substring(i, 1) == "1")
            {
                m_CorrectPattern[i] = true;
            }
        }
    }

    /// <summary>
    /// Check if current pattern matches correct pattern
    /// </summary>
    /// <returns></returns>
    public bool CheckPattern()
    {
        for (int i = 0; i < m_CurrentPattern.Length; i++)
        {
            m_CurrentPattern[i] = m_CurrentCylinder[i] || m_CurrentFloor[i];
        }

        for (int i = 0; i < m_CorrectPattern.Length; i++)
        {
            if (m_CorrectPattern[i] != m_CurrentPattern[i])
            {
                return false;
            }
        }
        return true;
    }

    public void Reset()
    {
        Debug.Log("Reset pattern puzzle");
        foreach (GameObject obj in m_AllCylinders)
        {
            obj.GetComponent<PatternPuzzlePointChecker>().Reset();
        }
        
        for (int i = 0; i < m_CurrentPattern.Length; i++)
        {
            m_CurrentPattern[i] = false;
        }

        // TODO: Stop play particle effects
    }
}
