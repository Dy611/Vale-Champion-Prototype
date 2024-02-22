using System.Collections;
using UnityEngine;

public class ProjectileThrower : MonoBehaviour
{
    #region Variables
    [SerializeField] bool fire = true;
    [SerializeField] float delay;

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
    }
    #endregion Public Methods

    #region Coroutines
    private IEnumerator Fire(float delay)
    {
        while (fire)
        {
            yield return new WaitForSeconds(delay);
            anim.SetTrigger("Cast");
        }
    }
    #endregion Coroutines
}