using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Spellshield : MonoBehaviour
{
    #region Variables
    public float duration;
    public float invisDuration;
    public bool invis;

    public List<string> tagsToAvoid;

    public Material invisMat;
    public Material spellshieldMat;

    private float timeElapsed;
    private string originalName;

    private List<Material> originalMats;

    private Renderer[] rends;
    private TMP_Text nameText;
    #endregion Variables

    #region Unity Methods
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

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration && !invis)
        {
            Destroy(this);
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = originalMats[i];
        }
        nameText.text = originalName;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7 && !invis)
        {
            foreach (string tag in tagsToAvoid)
            {
                if (!other.transform.CompareTag(tag))
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
        }
    }
    #endregion Unity Methods

    #region Coroutines
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
    #endregion Coroutines
}