using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPuzzlePointChecker : MonoBehaviour
{
    public enum CheckPointType { cylinder, floor };
    // [Header("Set Manually")]
    // public Material m_MaterialTouch;
    // public Material m_MaterialNotTouch;
    [Header("Set Dynamically")]
    public int m_CheckPointIndex;
    public CheckPointType m_Type;
    private float m_DelayTime; // hight light color disappear delay time
    private float tmp_m_DelayTime;
    public PatternPuzzleController controller;
    public bool m_StayBright;
    public bool m_Debug;
    public Material m_HighlightColor;
    public Material m_CompletedColor;
    public GameObject rune;
    public GameObject matchRune;
    public ParticleSystem m_RuneSparkles;
    private Renderer rend;
    private Renderer matchRend;
    private Material m_OriginalColor;
    public bool collide;
    private Material[] matTouch;
    private Material matNotTouch;
    private GameObject m_Parent;

    void Awake()
    {
        if (m_Type == CheckPointType.cylinder)
        {
            rend = rune.GetComponent<Renderer>();
            if (matchRune != null)
            {
                matchRend = matchRune.GetComponent<Renderer>();
            }
            m_OriginalColor = rend.material;

            // Looking for particles
            m_RuneSparkles = transform.Find("RuneSparkles").gameObject.GetComponent<ParticleSystem>();
            m_RuneSparkles.Stop();

        //    matTouch = new Material[2];
        //    matTouch[0] = rend.materials[1];
        //    matTouch[1] = rend.materials[0];
        //    matNotTouch = new Material[2];
        //    matNotTouch[0] = rend.materials[0];
        //    matNotTouch[1] = rend.materials[1];
        }
        else if (m_Type == CheckPointType.floor)
        {
            m_Parent = transform.parent.gameObject;
        }

        m_StayBright = false;
    }

    private void Start()
    {
        controller = transform.parent.GetComponent<PatternPuzzleController>();
        if (controller == null)
        {
            controller = transform.parent.parent.GetComponent<PatternPuzzleController>();
        }
        m_Debug = controller.m_Debug;
        m_DelayTime = 0.2f;
        tmp_m_DelayTime = -1;
    }
    void Update()
    {
        if (tmp_m_DelayTime > 0)
        {
            if (m_Type == CheckPointType.cylinder)
            {
                controller.m_CurrentCylinder[m_CheckPointIndex] = true;
            }
            else if (m_Type == CheckPointType.floor)
            {
                controller.m_CurrentFloor[m_CheckPointIndex] = true;
            }
        }
        else
        {
            if (m_Type == CheckPointType.cylinder)
            {
                controller.m_CurrentCylinder[m_CheckPointIndex] = false;
            }
            else if (m_Type == CheckPointType.floor)
            {
                controller.m_CurrentFloor[m_CheckPointIndex] = false;
            }
        }

        if (tmp_m_DelayTime > -1)
        {
            if (!m_StayBright)
            {
                tmp_m_DelayTime -= Time.deltaTime;
            }
        }
    }
    void LateUpdate()
    {
        if (!m_StayBright)
        {
            if (m_Type == CheckPointType.cylinder)
            {
                if (tmp_m_DelayTime < 0)
                {
                    rend.material = m_OriginalColor;
                    if (matchRend != null)
                    {
                        matchRend.material = m_OriginalColor;
                    }
                    collide = false;
                }
                else if (tmp_m_DelayTime > 0)
                {
                    rend.material = m_HighlightColor;
                    if (matchRend != null)
                    {
                        matchRend.material = m_HighlightColor;
                    }
                    collide = true;
                }
            }
        }
    }
    public void Collide()
    {
        tmp_m_DelayTime = m_DelayTime;
        if (m_Debug)
        {
            if (!m_StayBright)
            {
                rend.material = m_HighlightColor;
                if (matchRend != null)
                {
                    matchRend.material = m_HighlightColor;
                }
            }
        }
    }

    public void Touch()
    {
        tmp_m_DelayTime = m_DelayTime;
        if (m_Debug)
        {
            if (!m_StayBright)
            {
                rend.material = m_HighlightColor;
                if (matchRend != null)
                {
                    matchRend.material = m_HighlightColor;
                }
            }
        }
    }

    public void Reset()
    {
        rend.material = m_OriginalColor;
        if (matchRend != null)
        {
            matchRend.material = m_OriginalColor;
        }
        collide = false;
    }

    public void Complete()
    {
        m_StayBright = true;
        rend.material = m_CompletedColor;
        if (matchRend != null)
        {
            matchRend.material = m_CompletedColor;
        }

        // TODO: Play particle effects
        if (m_RuneSparkles)
        {
            m_RuneSparkles.Play();
        }
    }
}
