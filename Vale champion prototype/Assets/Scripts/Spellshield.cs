using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellshield : MonoBehaviour
{
    public float duration = 1.5f;
    public float invisDuration = 0.75f;

    private float timeElapsed;
    private Renderer[] rends;
    private bool invis;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= duration && !invis)
        {
            Destroy(this);
            Debug.LogWarning("Blocked Nothing");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            invis = true;
            rends = transform.GetComponentsInChildren<Renderer>();
            foreach(Renderer rend in rends)
            {
                Color col = rend.material.color;
                col.a = 0.25f;
                rend.material.color = col;
            }

            StartCoroutine(InvisTime(invisDuration));

        }
    }

    private IEnumerator InvisTime(float length)
    {
        Debug.LogWarning("INVIS");
        yield return new WaitForSeconds(length);
        Debug.LogWarning("VIS");
        foreach (Renderer rend in rends)
        {
            Color col = rend.material.color;
            col.a = 1;
            rend.material.color = col;
        }
        Destroy(GetComponent<Spellshield>());
        yield return null;
    }
}
