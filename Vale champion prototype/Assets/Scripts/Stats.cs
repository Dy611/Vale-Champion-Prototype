using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class Stats : MonoBehaviour
{
    #region Variables
    [SerializeField] Texture2D hoverCursor;

    public bool applyVigilStrikes;
    public float attackRange;
    public float attackDelay;
    public int attackDamage;
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
    public GameObject inGameUI;
    public Image hpBar;
    public Image manaBar;
    public Image hpBarHUD;
    public Image manaBarHUD;
    public Image vigilStriked;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI vigilStrikedCount;

    public Component[] disableComps;

    private bool isDead;
    private float decayTimerInitial;
    private float decayTimer;
    #endregion Variables

    #region Unity Methods
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

        if(currentHP <= 0 && !isDead && !GetComponent<VigilProtected>())
        {
            Die();
        }
    }

    void OnMouseEnter()
    {
        if (hoverCursor)
            Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }

    void OnMouseExit()
    {
        // Pass 'null' to the texture parameter to use the default system cursor.
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
    #endregion Unity Methods

    #region Public Methods
    public void Die()
    {
        isDead = true;
        currentHP = 0;
        currentMana = 0;
        GetComponentInChildren<Animator>().SetTrigger("Death");
        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Collider[] cols = GetComponents<Collider>();

        foreach (Collider col in cols)
        {
            col.enabled = false;
        }

        inGameUI.SetActive(false);
        if (GetComponent<Patrol>())
        {
            GetComponent<Patrol>().enabled = false;
        }
        if (GetComponent<SimpleMovement>())
        {
            SimpleMovement.stopMovement = false;
            GetComponent<SimpleMovement>().enabled = false;
        }
        if (GetComponent<ProjectileThrower>())
        {
            GetComponent<ProjectileThrower>().fire = false;
            GetComponent<ProjectileThrower>().enabled = false;
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

        if(GetComponent<Spellshield>() == null || GetComponent<Spellshield>().invis)
            currentHP -= damage;
    }

    public void ApplySlow(float slowPercent, float duration)
    {
        StartCoroutine(Slow(slowPercent, duration));
    }
    #endregion Public Methods

    #region Coroutines
    private IEnumerator HealthRegen()
    {
        while(!isDead)
        {
            yield return new WaitForSeconds(healTickRate);
            if(!isDead)
                currentHP = (int)Mathf.Clamp(currentHP + healTickAmount, 0, maxHP);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator ManaRegen()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(manaTickRate);
            if(!isDead)
                currentMana = (int)Mathf.Clamp(currentMana + manaTickAmount, 0, maxMana);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator Slow(float slowPercent, float duration)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        float originalSpeed = agent.speed;
        agent.speed *= (1 - slowPercent);
        yield return new WaitForSeconds(duration);
        agent.speed = originalSpeed;
    }
    #endregion Coroutines
}