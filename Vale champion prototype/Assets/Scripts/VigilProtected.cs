using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VigilProtected : MonoBehaviour
{
    #region Variables
    public float duration;

    public GameObject vale;
    public Material protectedMat;

    private float timeElapsed;
    private string originalName;

    private List<Material> originalMats;

    private Renderer[] rends;
    private Stats stat;
    private TMP_Text nameText;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        if (GetComponent<ProjectileThrower>())
        {
            GetComponent<ProjectileThrower>().applyVigilStrikes = true;
        }

        stat = GetComponent<Stats>();
        rends = transform.GetComponentsInChildren<Renderer>();
        originalMats = new List<Material>();

        for (int i = 0; i < rends.Length; i++)
        {
            originalMats.Add(rends[i].material);
            rends[i].material = protectedMat;
        }

        nameText = GetComponentInChildren<TMP_Text>();
        originalName = nameText.text;
        nameText.text = "Vigil Protected!";
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed >= duration)
        {
            Destroy(this);
        }

        if(stat.currentHP <= 0)
        {
            ValeAbilities valeAbs = vale.GetComponent<ValeAbilities>();
            Stats valeStats = vale.GetComponent<Stats>();

            float valeCurrHP = valeStats.currentHP;
            float valeFlatHeal = valeAbs.rFlatHeal;

            if(valeCurrHP * valeAbs.rHealPercent > valeFlatHeal)
            {
                Debug.Log("TOTAL HEAL: " + valeStats.currentHP * valeAbs.rHealPercent);
                stat.currentHP = (int)(valeCurrHP * valeAbs.rHealPercent);
                valeStats.Die();
                Destroy(this);
            }
            else
            {
                Debug.Log("FLAT HEAL: " + valeAbs.rFlatHeal);
                stat.currentHP = (int)valeFlatHeal;
                valeStats.Die();
                Destroy(this);
            }
        }
    }

    private void OnDestroy()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = originalMats[i];
        }
        nameText.text = originalName;

        if (GetComponent<ProjectileThrower>())
        {
            GetComponent<ProjectileThrower>().applyVigilStrikes = false;
        }
    }
    #endregion Unity Methods
}