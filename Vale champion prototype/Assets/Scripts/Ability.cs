using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool applyVigilStrikes;
    public bool destroyOnHit;
    public float lifeDuration;
    public int damage;
    public List<string> tagsToAvoid;

    private List<Stats> hitStats;

    public bool GetVigilStrikedTarget()
    {
        foreach(Stats stat in hitStats)
        {
            if(stat.vigilStrikes >= stat.maxVigilStrikes)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    private void Start()
    {
        hitStats = new List<Stats>();
        tagsToAvoid = new List<string>();
        tagsToAvoid.Add(gameObject.tag);
        StartCoroutine(SetLifeDuration());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            foreach(string tag in tagsToAvoid)
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

    private IEnumerator SetLifeDuration()
    {
        yield return new WaitForSeconds(lifeDuration);
        Destroy(gameObject);
    }
}
