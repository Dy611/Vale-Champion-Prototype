using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool applyVigilStrikes;
    public float lifeDuration;
    public int damage;

    private void Awake()
    {
        StartCoroutine(SetLifeDuration());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 6)
        {
            collision.gameObject.GetComponent<Stats>().ApplyDamage(applyVigilStrikes, damage);
            Destroy(gameObject);
        }
    }

    private IEnumerator SetLifeDuration()
    {
        yield return new WaitForSeconds(lifeDuration);
        Destroy(gameObject);
    }
}
