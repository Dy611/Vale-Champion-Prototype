using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class SimpleMovement : MonoBehaviour
{
    public Camera cam;
    public NavMeshAgent agent;
    public static bool stopMovement;
    public Animator valeAnim;
    public float rotSpeed;

    private NavMeshHit myNavHit;

    // Update is called once per frame
    void Update()
    {
        //Update Animation When Walking
        if(agent.velocity.x != 0 || agent.velocity.z != 0)
            valeAnim.SetBool("Walking", true);
        else
            valeAnim.SetBool("Walking", false);

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);

            if (NavMesh.SamplePosition(hit.point, out myNavHit, 100, -1))
            {
                Vector3 lookDir = myNavHit.position - transform.position;
                float RotateSpeed = Time.deltaTime * rotSpeed;
                lookDir.y = 0;

                Vector3 NewDirectionToLook = Vector3.RotateTowards(transform.forward, lookDir, RotateSpeed, 0.0f);
                

                StartCoroutine(StoreClick(myNavHit.position, NewDirectionToLook));
            }
        }
    }

    private IEnumerator StoreClick(Vector3 targetPos, Vector3 rot)
    {
        while (stopMovement)
        {
            yield return null;
        }
        transform.rotation = Quaternion.LookRotation(rot);
        agent.SetDestination(targetPos);
        yield return null;
    }
}
