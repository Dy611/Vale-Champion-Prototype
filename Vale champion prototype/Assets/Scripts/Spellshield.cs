using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spellshield : MonoBehaviour
{
    public Material invisMat;
    public Material spellshieldMat;

    public float duration;
    public float invisDuration;

    private float timeElapsed;
    private Renderer[] rends;
    private List<Material> originalMats;
    private TMP_Text nameText;
    private bool invis;
    private string originalName;

    private void Start()
    {
        rends = transform.GetComponentsInChildren<Renderer>();
        originalMats = new List<Material>();

        for (int i = 0; i < rends.Length; i++)
        {
            originalMats.Add(rends[i].material);
            rends[i].material = spellshieldMat;
        }

        nameText = GetComponentInChildren<TMP_Text>();
        originalName = nameText.text;
        nameText.text = "SpellShield!";
    }

    private void OnDestroy()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = originalMats[i];
        }
        nameText.text = originalName;
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration && !invis)
        {
            Destroy(this);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            invis = true;

            for (int i = 0; i < rends.Length; i++)
            {
                rends[i].material = invisMat;
            }

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
        Destroy(this);
        yield return null;
    }
}
