using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spellshield : MonoBehaviour
{
    public float duration = 1.5f;

    private float timeElapsed;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= duration)
        {
            Destroy(this);
            Debug.LogWarning("Blocked Nothing");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            Renderer rend = transform.GetComponent<Renderer>();
            Color invis = rend.material.color;
            invis.a = 0.25f;
            rend.material.color = invis;
            Debug.LogWarning("INVIS");
            Destroy(GetComponent<Spellshield>());
        }
    }
}
