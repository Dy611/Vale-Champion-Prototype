using System.Collections;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    #region Variables
    public bool fire = true;

    [SerializeField] float delay;

    [SerializeField] Stats statComp;
    [SerializeField] GameObject projectile;
    [SerializeField] Transform spawnPoint;
    [SerializeField] Animator anim;
    #endregion Variables

    #region Unity Methods
    private void Start()
    {
        StartCoroutine(Fire(delay));
    }
    #endregion Unity Methods

    #region Public Methods
    public void SpawnObj()
    {
        GameObject spawnedObj = Instantiate(projectile, spawnPoint);
        spawnedObj.tag = gameObject.tag;
        spawnedObj.transform.SetParent(null);
        spawnedObj.GetComponent<Ability>().applyVigilStrikes = statComp.applyVigilStrikes;
    }
    #endregion Public Methods

    #region Coroutines
    private IEnumerator Fire(float delay)
    {
        while (fire)
        {
            yield return new WaitForSeconds(delay);
            if(fire)
                anim.SetTrigger("Cast");
        }
    }
    #endregion Coroutines
}