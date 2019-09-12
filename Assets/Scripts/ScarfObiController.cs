using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;

/// <summary>
/// Interacts with obi rope scripts
/// </summary>
public class ScarfObiController : MonoBehaviour
{
    public float speedIncrease = 1f;
    public float speedDecrease = 1f;
    public float minLength = 1f;
    public float maxLength = 12.29f;

    [HideInInspector]
    public bool extendRope = false;
    [HideInInspector]
    public bool shortenRope = false;

    private ObiRopeCursor cursor;
    private ObiRope rope;

    /// <summary>
    /// Initialize
    /// </summary>
    void Start()
    {
        rope = GetComponent<ObiRope>();
        cursor = GetComponent<ObiRopeCursor>();
    }

    /// <summary>
    /// Increase the length of the scarf
    /// </summary>
    public void AddToScarf()
    {
        cursor.ChangeLength(rope.RestLength + speedIncrease * Time.deltaTime);
    }

    /// <summary>
    /// Decrease the length of the scarf
    /// </summary>
    public void SubtractFromScarf()
    {
        if(rope.RestLength > minLength)
        {
            cursor.ChangeLength(rope.RestLength - speedDecrease * Time.deltaTime);
        }
    }
    
    /// <summary>
    /// Pickup upgrades max scarf length
    /// </summary>
    public void IncreaseMaxScarf()
    {
        rope.pooledParticles += 5;
    }

    public void EnableRope()
    {
        rope.enabled = true;
    }

    public void DisableRope()
    {
        rope.enabled = false;
    }
}
