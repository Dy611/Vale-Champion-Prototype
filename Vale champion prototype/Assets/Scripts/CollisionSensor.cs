﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class To Detect Collision Based On Layers
/// </summary>
public class CollisionSensor : MonoBehaviour
{
    #region Variables
    [Tooltip("The Layers This Sensor Detects")]
    [SerializeField] LayerMask layers;

    [Tooltip("The Tags This Sensor Uses")]
    [SerializeField] string[] tags;

    [SerializeField] Color standardColor;
    [SerializeField] Color detectionColor;

    /// <summary>
    /// The Amount Of Colliders This Object Is Around
    /// </summary>
    private int colCount = 0;

    /// <summary>
    /// How Long This Sensor Should Ignore Collision
    /// </summary>
    private float disableTimer;

    /// <summary>
    /// The Colliders This Sensor Is Currently Touching
    /// </summary>
    [HideInInspector] public List<Collider> cols;
    #endregion Variables

    #region Unity Methods
    private void OnEnable()
    {
        colCount = 0;
    }

    void Update()
    {
        disableTimer -= Time.deltaTime;
        if (colCount > 0)
            GetComponent<Renderer>().material.color = detectionColor;
        else
            GetComponent<Renderer>().material.color = standardColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            if (tags.Length > 0)
            {
                foreach (string tag in tags)
                {
                    if (!other.CompareTag(tag))
                    {
                        colCount++;
                        cols.Add(other);
                    }
                }
            }
            else
            {
                colCount++;
                cols.Add(other);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            if (tags.Length > 0)
            {
                foreach (string tag in tags)
                {
                    if (!other.CompareTag(tag))
                    {
                        colCount--;
                        cols.Remove(other);
                    }
                }
            }
            else
            {
                colCount--;
                cols.Remove(other);
            }
        }
    }
    #endregion Unity Methods

    #region Public Methods
    public bool IsCollided()
    {
        if (disableTimer > 0)
            return false;
        return colCount > 0;
    }

    public void Disable(float duration)
    {
        disableTimer = duration;
    }

    public void CleanDeletedReferences()
    {
        for(int i = 0; i < cols.Count; i++)
        {
            if(cols[i] == null)
            {
                cols.Remove(cols[i]);
                colCount--;
            }
        }
    }
    #endregion Public Methods
}