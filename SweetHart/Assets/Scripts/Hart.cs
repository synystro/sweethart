using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hart : Spirit
{
    [SerializeField] private float eyesHeightOffset;
    [SerializeField] private float interactionDistance;

    private Vector3 playerLastKnownLocation;
    private bool isPlayerVisible;
    private bool isChasingPlayer;
    private bool caughtPlayer;
    private Door door;

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
        if(!caughtPlayer) {
            SearchForPlayer();
        } else { navMeshAgent.isStopped = true; }
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
            if (hit.transform.gameObject == target.transform.gameObject)
            {
                isPlayerVisible = true;
                isChasingPlayer = true;

                target = hit.transform.GetComponent<Player>();
                playerLastKnownLocation = target.transform.position;

                if(!caughtPlayer) {
                    navMeshAgent.isStopped = false;
                    ChasePlayer();
                }
                else {
                    navMeshAgent.isStopped = true;
                }
            }
            else
            {
                isPlayerVisible = false;
            }
        }
    }

    private void ChasePlayer()
    {
        navMeshAgent.SetDestination(playerLastKnownLocation);
        if(transform.position == playerLastKnownLocation) {
            isChasingPlayer = false;
        }
    }

    private void OpenDoor(Collider col)
    {
            Door door = col.transform.GetComponent<Door>();
            if(door.IsLocked) { door.Locked(); } else { door.IsOpen = true; door.OpenClose(); }
    }

    private void OnTriggerEnter(Collider col)
    {
        // player collision
        if(col.gameObject.tag == "Player") {
            if(!caughtPlayer && isPlayerVisible) {
                col.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().IsCaught = true;
                caughtPlayer = true;
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        // door collision
        if(isChasingPlayer) {
            if(col.transform.GetComponent<Door>()) {
                door = col.transform.GetComponent<Door>();
                if(door && !isPlayerVisible) {

                    OpenDoor(col);

                    if(!door.IsCompletelyOpen) {
                        navMeshAgent.isStopped = true;
                    } else {
                        navMeshAgent.isStopped = false;
                    }
                }
            }
        }
    }
}
