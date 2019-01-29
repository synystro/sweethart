using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hart : Spirit
{
    [SerializeField] private float eyesHeightOffset;

    private Vector3 playerLastKnownLocation;

    NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = this.GetComponent<NavMeshAgent>();

        if(navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent component not attached to " + gameObject.name);
        }



    }
    void Update()
    {
        //base.Float();
        SearchForPlayer();
    }

    private void SearchForPlayer()
    {
        // look for player in chasingRange.
        if (target == null)
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, chasingRange / 2, Vector3.down);
            foreach (RaycastHit hit in hits)
                if (hit.transform.GetComponent<Player>() != null)
                {
                    target = hit.transform.GetComponent<Player>();
                    CheckIfPlayerVisible();
                }
        }
        // check if player isn't behind any obstacle
        if (target != null)
        {
            CheckIfPlayerVisible();
        }
    }

    private void CheckIfPlayerVisible()
    {
        Vector3 sightPosition = new Vector3(
            transform.position.x,
            transform.position.y + eyesHeightOffset,
            transform.position.z            
            );

        Debug.DrawRay(sightPosition, (target.transform.position - transform.position), Color.green);

        RaycastHit hit;
        if (Physics.Raycast(sightPosition, (target.transform.position - transform.position), out hit))
        {
            if (hit.transform == target.transform)
            {
                //Debug.Log("visible");

                target = hit.transform.GetComponent<Player>();
                playerLastKnownLocation = target.transform.position;

                ChasePlayer();
            }
            else
            {
                //Debug.Log("HIDDEN");
            }
        }
    }

    private void ChasePlayer()
    {
            navMeshAgent.SetDestination(playerLastKnownLocation);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("PLAYER KILLED");
        }
    }

}
