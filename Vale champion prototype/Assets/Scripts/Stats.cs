using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Stats : MonoBehaviour
{
    public int vigilStrikes;
    public int maxVigilStrikes;
    public float vigilStrikesDecayTimeInitial;
    public float vigilStrikesDecayTime;
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
    public Image hpBarHUD;
    public Image manaBarHUD;
    public Image vigilStriked;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI vigilStrikedCount;

    private float decayTimerInitial;
    private float decayTimer;

    private void Start()
    {
        StartCoroutine(HealthRegen());
        StartCoroutine(ManaRegen());
        currentHP = maxHP;
        currentMana = maxMana;
        vigilStriked.enabled = false;
        vigilStrikedCount.text = "";
    }

    private void Update()
    {
        if(hpBarHUD != null)
        {
            hpBarHUD.fillAmount = (float)currentHP / maxHP;
        }

        if(manaBarHUD != null)
        {
            manaBarHUD.fillAmount = (float)currentMana / maxMana;
        }

        if (hpBar != null)
        {
            hpBar.fillAmount = (float)currentHP / maxHP;
        }

        if (manaBar != null)
        {
            manaBar.fillAmount = (float)currentMana / maxMana;
        }

        if (hpText != null)
        {
            hpText.text = currentHP + " / " + maxHP;
        }

        if (manaText != null)
        {
            manaText.text = currentMana + " / " + maxMana;
        }

        if(vigilStrikes > 0)
        {
            vigilStriked.enabled = true;
            vigilStrikedCount.text = vigilStrikes.ToString();

            decayTimerInitial += Time.deltaTime;
            if(decayTimerInitial >= vigilStrikesDecayTimeInitial)
            {
                decayTimer += Time.deltaTime;
                if(decayTimer >= vigilStrikesDecayTime)
                {
                    vigilStrikes--;
                    decayTimer = 0;
                    if (vigilStrikes > 0)
                    {
                        vigilStrikedCount.text = vigilStrikes.ToString();
                    }
                    else
                    {
                        vigilStriked.enabled = false;
                        vigilStrikedCount.text = "";
                        decayTimerInitial = 0;
                        decayTimer = 0;
                    }
                }
            }
        }

        if(currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ApplyDamage(bool applyStrikes, int damage)
    {
        if (applyStrikes)
        {
            vigilStrikes = Mathf.Clamp(vigilStrikes + 1, 0, maxVigilStrikes);
            decayTimerInitial = 0;
            decayTimer = 0;
        }

        if(GetComponent<Spellshield>() == null)
            currentHP -= damage;
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
