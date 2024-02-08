using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spellshield : MonoBehaviour
{
    public Material invisMat;

    public float duration;
    public float invisDuration;

    private float timeElapsed;
    private Renderer[] rends;
    private List<Material> originalMats;
    private TMP_Text nameText;
    private bool invis;
    private string originalName;

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration && !invis)
        {
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 7)
        {
            invis = true;
            rends = transform.GetComponentsInChildren<Renderer>();
            originalMats = new List<Material>();

            for (int i = 0; i < rends.Length; i++)
            {
                originalMats.Add(rends[i].material);
                rends[i].material = invisMat;
            }

            nameText = GetComponentInChildren<TMP_Text>();
            originalName = nameText.text;
            nameText.text = "Invisible!";
            StartCoroutine(InvisTime(invisDuration));
        }
    }

    private IEnumerator InvisTime(float length)
    {
        yield return new WaitForSeconds(length);

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = originalMats[i];
        }

        nameText.text = originalName;
        Destroy(GetComponent<Spellshield>());
        yield return null;
    }
}
