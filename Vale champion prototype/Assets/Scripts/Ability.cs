using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool applyVigilStrikes;
    public bool destroyOnHit;
    public float lifeDuration;
    public int damage;

    private void Start()
    {
        StartCoroutine(SetLifeDuration());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 6)
        {
            other.gameObject.GetComponent<Stats>().ApplyDamage(applyVigilStrikes, damage);
            if(destroyOnHit)
                Destroy(gameObject);
        }
    }

    private IEnumerator SetLifeDuration()
    {
        yield return new WaitForSeconds(lifeDuration);
        Destroy(gameObject);
    }
}
