using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    public int vigilStrikes;
    public int maxHP;
    public int maxMana;
    public int currentHP;
    public int currentMana;
    public float healTickAmount;
    public float healTickRate;
    public float manaTickAmount;
    public float manaTickRate;
    public Image hpBar;
    public Image manaBar;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;

    private void Start()
    {
        StartCoroutine(HealthRegen());
        StartCoroutine(ManaRegen());
        currentHP = maxHP;
        currentMana = maxMana;
    }

    private void Update()
    {
        if(hpBar != null)
        {
            hpBar.fillAmount = (float)currentHP / maxHP;
        }

        if(manaBar != null)
        {
            manaBar.fillAmount = (float)currentMana / maxMana;
        }

        if(hpText != null)
        {
            hpText.text = currentHP + " / " + maxHP;
        }

        if (manaText != null)
        {
            manaText.text = currentMana + " / " + maxMana;
        }
    }

    private IEnumerator HealthRegen()
    {
        while(currentHP <= maxHP + 1)
        {
            yield return new WaitForSeconds(healTickRate);
            currentHP = (int)Mathf.Clamp(currentHP + healTickAmount, 0, maxHP);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator ManaRegen()
    {
        while (currentMana <= maxMana + 1)
        {
            yield return new WaitForSeconds(manaTickRate);
            currentMana = (int)Mathf.Clamp(currentMana + manaTickAmount, 0, maxMana);
            yield return null;
        }

        yield return null;
    }
}
