using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    #region Variables
    public float lifeDuration;
    public float speed;
    public int damage;
    public bool applyVigilStrikes;

    [SerializeField] bool destroyOnHit;

    [SerializeField] List<string> tagsToAvoid;

    private List<Stats> hitStats;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        hitStats = new List<Stats>();
        tagsToAvoid = new List<string>();
        tagsToAvoid.Add(gameObject.tag);
        StartCoroutine(SetLifeDuration());
    }

    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            foreach (string tag in tagsToAvoid)
            {
                if (!other.transform.CompareTag(tag))
                {
                    hitStats.Add(other.gameObject.GetComponent<Stats>());
                    other.gameObject.GetComponent<Stats>().ApplyDamage(applyVigilStrikes, damage);

                    if (destroyOnHit)
                        Destroy(gameObject);
                }
            }
        }
    }
    #endregion Unity Methods

    #region Public Methods
    public bool GetVigilStrikedTarget()
    {
        foreach(Stats stat in hitStats)
        {
            if(stat.vigilStrikes >= stat.maxVigilStrikes)
                return true;
            else
                return false;
        }
        return false;
    }
    #endregion Public Methods

    #region Coroutines
    private IEnumerator SetLifeDuration()
    {
        yield return new WaitForSeconds(lifeDuration);
        Destroy(gameObject);
    }
    #endregion Coroutines
}