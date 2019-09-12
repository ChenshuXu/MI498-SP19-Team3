using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Make things blocking views transparent
/// </summary>
public class TransparentObstacle : MonoBehaviour
{
    public enum Trans
    {
        transparent, notTransparent
    }
    [Header("Set Manually")]
    public float transparentPercent = 0.4f;
    public float delay = 0.2f;
    [Header("Set Dynamically")]
    public Trans trans;
    public MeshRenderer _rend;
    public Material[] mat;
    private float tmpdelay;
    public bool rendInChild = false;
    void Start()
    {
        if (rendInChild)
        {
            _rend = gameObject.GetComponentInChildren<MeshRenderer>();
        }
        else
        {
            _rend = gameObject.GetComponent<MeshRenderer>();
        }
        
        mat = _rend.materials;
        tmpdelay = delay;
        trans = Trans.notTransparent;
    }

    void Update()
    {
        if (tmpdelay > 0.0f)
        {
            tmpdelay -= Time.deltaTime;
        }
        else
        {
            SetTrans(Trans.notTransparent);
        }

    }

    public void SetTransparent()
    {
        SetTrans(Trans.transparent);
    }

    private void SetTrans(Trans t)
    {
        switch (trans)
        {
            case Trans.transparent:
                foreach (Material m in mat)
                {
                    StartCoroutine(FadeTo(1, 0.2f, m));
                }
                break;
        }

        trans = t;

        switch (trans)
        {
            case Trans.transparent:
                tmpdelay = delay;
                foreach (Material m in mat)
                {
                    StartCoroutine(FadeTo(transparentPercent, 0.2f, m));
                }
                break;
        }
        
    }

    IEnumerator FadeTo(float aValue, float aTime, Material material)
    {
        float alpha = material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = material.color;
            newColor.a = Mathf.Lerp(alpha, aValue, t);
            material.color = newColor;
            yield return null;
        }
    }
}
