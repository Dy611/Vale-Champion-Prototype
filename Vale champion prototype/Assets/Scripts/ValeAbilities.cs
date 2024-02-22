using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;

public class ValeAbilities : MonoBehaviour
{
    #region Variables
    [Header("Requirements")]
    public NavMeshAgent agent;
    public Stats valeStats;
    public Camera cam;
    public Animator valeAnim;
    public CollisionSensor colSensor;
    public GameObject navMeshWithCollision;
    public GameObject navMeshWithoutCollision;
    public Image qFill;
    public Image wFill;
    public Image eFill;
    public Image rFill;
    public TMP_Text qCDText;
    public TMP_Text wCDText;
    public TMP_Text eCDText;
    public TMP_Text rCDText;
    public TMP_Text qChargeText;
    public Color abilityCDFillColorDefault;
    public Color abilityCDFillColorCharges;

    [Header("Q Ability")]
    public Material dashMat;
    public float qCooldown;
    public int qManaCost;
    public int qCurrentCharges;
    public int qMaxCharges;
    public float qNormalDistance;
    public float qDistanceTowardsEnemies;
    public float qSpeed;
    public float qSlowPercent;
    public float qSlowDuration;
    public float qAllyMSIncreasePercent;
    public float qRefundPercent;
    public int qDamage;

    [Header("W Ability")]
    public Material invisMat;
    public Material spellshieldMat;
    public float wCooldown;
    public int wManaCost;
    public float wShieldDuration;
    public float wInvisDuration;
    public float wRefundPercent;
    public float wTargetRange;

    [Header("E Ability")]
    public float eCooldown;
    public int eManaCost;
    public float eSlowPercent;
    public float eSlowDuration;
    public float eRefundPercent;
    public float eQRefundPercent;

    public GameObject eProjectile;
    public Transform eSpawnPoint;
    public float eSpeed;
    public float eLife;

    [Header("R Ability")]
    public Material protectionMat;
    public float rCooldown;
    public int rManaCost;
    public float rDuration;
    public float rHealPercent;
    public float rFlatHeal;
    public float rTargetRange;

    private RaycastHit hit;
    private NavMeshHit navHit;


    private Renderer[] rends;
    private List<Material> originalMats;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        qCurrentCharges = qMaxCharges;

        rends = transform.GetComponentsInChildren<Renderer>();
        originalMats = new List<Material>();

        for (int i = 0; i < rends.Length; i++)
        {
            originalMats.Add(rends[i].material);
        }
    }
    #endregion Unity Methods

    #region Public Methods
    public void OnQAbility(InputValue input)
    {
        if ((qFill.fillAmount == 0 || qCurrentCharges > 0) && valeStats.currentMana >= qManaCost)
        {
            qCurrentCharges--;

            if (qCurrentCharges > 0)
                qChargeText.text = qCurrentCharges.ToString();
            else
                qChargeText.text = "";

            valeStats.currentMana -= qManaCost;
            StartCoroutine(QAbility());
            StartCoroutine(AbilityCooldown(qCooldown, qFill, qCDText, true, qCurrentCharges, qChargeText));
        }
    }
    public void OnWAbility(InputValue input)
    {
        hit = StandardFunctions.GetMousePosition();

        if (hit.transform.gameObject.CompareTag("Ally") && wFill.fillAmount == 0 && valeStats.currentMana >= wManaCost && StandardFunctions.CalculateDistance(hit.transform.position, transform.position) < wTargetRange)
        {
            valeStats.currentMana -= wManaCost;
            agent.SetDestination(transform.position);
            StandardFunctions.RotateObj(gameObject, hit.point);
            valeAnim.SetTrigger("WAbility");
            StartCoroutine(AbilityCooldown(wCooldown, wFill, wCDText, false, 0, null));
        }
    }
    public void OnEAbility(InputValue input)
    {
        if(valeStats.currentMana >= eManaCost && eFill.fillAmount == 0)
        {
            valeStats.currentMana -= eManaCost;
            agent.SetDestination(transform.position);
            hit = StandardFunctions.GetMousePosition();
            StandardFunctions.RotateObj(gameObject, hit.point);
            SimpleMovement.stopMovement = true;
            valeAnim.SetTrigger("EAbility");
            StartCoroutine(AbilityCooldown(eCooldown, eFill, eCDText, false, 0, null));
        }
    }
    public void OnRAbility(InputValue input)
    {
        hit = StandardFunctions.GetMousePosition();

        if (hit.transform.gameObject.CompareTag("Ally") && hit.transform.gameObject != gameObject && rFill.fillAmount == 0 && valeStats.currentMana >= rManaCost && StandardFunctions.CalculateDistance(hit.transform.position, transform.position) < rTargetRange)
        {
            valeStats.currentMana -= rManaCost;
            agent.SetDestination(transform.position);
            StandardFunctions.RotateObj(gameObject, hit.point);
            valeAnim.SetTrigger("RAbility");
            StartCoroutine(AbilityCooldown(rCooldown, rFill, rCDText, false, 0, null));
        }
    }

    public void StartWAbility()
    {
        Spellshield currShield = (Spellshield)hit.transform.gameObject.AddComponent(typeof(Spellshield));
        currShield.tagsToAvoid = new List<string>();
        currShield.tagsToAvoid.Add(gameObject.tag);
        currShield.spellshieldMat = spellshieldMat;
        currShield.invisMat = invisMat;
        currShield.duration = wShieldDuration;
        currShield.invisDuration = wInvisDuration;
    }

    public void StartEAbility()
    {
        StartCoroutine(EAbility());
    }

    public void ESpawnObj()
    {
        StartCoroutine(EAbility());
    }

    public void StartRAbility()
    {
        VigilProtected currProtection = (VigilProtected)hit.transform.gameObject.AddComponent(typeof(VigilProtected));
        currProtection.duration = rDuration;
        currProtection.vale = gameObject;
        currProtection.protectedMat = protectionMat;
    }
    #endregion Public Methods

    #region Coroutines
    private IEnumerator QAbility()
    {
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = dashMat;
        }

        Ability damageVolume = gameObject.AddComponent<Ability>();
        damageVolume.tag = gameObject.tag;
        damageVolume.lifeDuration = 100;
        damageVolume.applyVigilStrikes = true;
        damageVolume.damage = qDamage;

        valeAnim.SetBool("Running", true);
        //Prevent Player From "Breaking" Out Of Ability
        SimpleMovement.stopMovement = true;

        hit = StandardFunctions.GetMousePosition();

        //Rotate Character
        StandardFunctions.RotateObj(gameObject, hit.point);

        //Determine If Ability Is Towards Enemy
        bool towardsEnemy = false;

        colSensor.CleanDeletedReferences();
        foreach (Collider obj in colSensor.cols)
        {
            if (obj != null && obj.transform.CompareTag("Enemy"))
            {
                Vector3 forward = transform.TransformDirection(Vector3.forward);
                Vector3 toOther = obj.transform.position - transform.position;
                if (Vector3.Dot(forward, toOther.normalized) >= 0.8f)
                {
                    towardsEnemy = true;
                }
            }
        }

        //Calculate Point Max Distance Away
        Vector3 targetPos = new Vector3(0, 0, 0);
        if (towardsEnemy)
            targetPos = transform.position + transform.forward * qDistanceTowardsEnemies;
        else
            targetPos = transform.position + transform.forward * qNormalDistance;

        //Determine Closest Point For Potential Travel Through Walls
        NavMesh.SamplePosition(targetPos, out navHit, 100, -1);

        //Disable "Champion" Collision
        agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;

        //Set To Ability Speed
        agent.speed = qSpeed;

        //Disable Standard NavMesh To Allow For Potential Wall Traversal
        navMeshWithCollision.SetActive(false);
        navMeshWithoutCollision.SetActive(true);

        //Move Character
        agent.SetDestination(navHit.position);
        float distance = StandardFunctions.CalculateDistance(navHit.position, transform.position);
        while (distance >= 0.1f)
        {
            distance = StandardFunctions.CalculateDistance(navHit.position, transform.position);
            yield return null;
        }

        //Reset Agent Speed
        agent.speed = 5;

        //Reset NavMeshes
        navMeshWithCollision.SetActive(true);
        navMeshWithoutCollision.SetActive(false);

        //ReEnable Player Movement
        SimpleMovement.stopMovement = false;

        Destroy(damageVolume);
        valeAnim.SetBool("Running", false);
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = originalMats[i];
        }
        yield return null;
    }

    private IEnumerator EAbility()
    {
        GameObject spawnedObj = Instantiate(eProjectile, eSpawnPoint);
        spawnedObj.tag = gameObject.tag;
        spawnedObj.transform.SetParent(null);
        spawnedObj.GetComponent<Ability>().lifeDuration = eLife;
        SimpleMovement.stopMovement = false;

        while (spawnedObj != null)
        {
            spawnedObj.transform.Translate(spawnedObj.transform.forward * eSpeed * Time.deltaTime, Space.World);
            yield return null;
        }
    }
    private IEnumerator AbilityCooldown(float cooldown, Image fillImage, TMP_Text cooldownText, bool hasCharges, int currentCharges, TMP_Text chargeText)
    {
        if (hasCharges)
        {
            //Set the fill color
            if (currentCharges > 0)
            {
                fillImage.color = abilityCDFillColorCharges;
                chargeText.text = currentCharges.ToString();
                cooldownText.enabled = false;
            }
            else
            {
                fillImage.color = abilityCDFillColorDefault;
                chargeText.text = "";
                cooldownText.enabled = true;
            }

            if (fillImage.fillAmount == 0)
            {
                float countDown = cooldown;
                fillImage.fillAmount = 1;
                cooldownText.text = countDown.ToString();
                while (countDown > 0)
                {
                    countDown -= Time.deltaTime;
                    cooldownText.text = Mathf.CeilToInt(countDown).ToString();
                    fillImage.fillAmount = countDown / cooldown;
                    yield return null;
                }
                fillImage.fillAmount = 0;

                //I must figure out how to decouple you!
                qCurrentCharges++;
                if (qCurrentCharges == qMaxCharges)
                {
                    chargeText.text = (currentCharges + 1).ToString();
                    yield return null;
                }
                else
                {
                    StartCoroutine(AbilityCooldown(cooldown, fillImage, cooldownText, true, currentCharges, chargeText));
                }

                yield return null;
            }
        }
        else
        {
            fillImage.color = abilityCDFillColorDefault;
            if (fillImage.fillAmount == 0)
            {
                float countDown = cooldown;
                fillImage.fillAmount = 1;
                cooldownText.text = countDown.ToString();
                while (countDown > 0)
                {
                    countDown -= Time.deltaTime;
                    cooldownText.text = Mathf.CeilToInt(countDown).ToString();
                    fillImage.fillAmount = countDown / cooldown;
                    yield return null;
                }
                fillImage.fillAmount = 0;
                cooldownText.text = "";
                yield return null;
            }
        }
    }
    #endregion Coroutines
}