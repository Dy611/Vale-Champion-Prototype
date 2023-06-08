using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Class To Detect Collision Based On Layers
/// </summary>
public class CollisionSensor : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The Layers This Sensor Detects
    /// </summary>
    [SerializeField] LayerMask layers;

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
        if(colCount > 0)
        {
            GetComponent<Renderer>().material.color = detectionColor;
        }
        else
        {
            GetComponent<Renderer>().material.color = standardColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            colCount++;
            cols.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & layers) != 0)
        {
            colCount--;
            cols.Remove(other);
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
    #endregion Public Methods
}