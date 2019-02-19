using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Hart : Spirit {
    [SerializeField] private float eyesHeightOffset;
    [SerializeField] private float interactionDistance;

    private Vector3 playerLastKnownPosition;
    private int movementIndex;
    private bool isPlayerVisible;
    private bool isChasingPlayer;
    private bool caughtPlayer;
    private Door door;
    private NavMeshAgent agent;

    private void Awake() {
        if(this.GetComponent<NavMeshAgent>() != null) {
            agent = this.GetComponent<NavMeshAgent>();
        }
        else { Debug.Log("NavMeshAgent not attached to the following game object: " + this.gameObject.transform.name); }
    }

    void Update() {
        base.Float();
        if(!caughtPlayer) {
            SearchForPlayer();
        }
        else {
            agent.enabled = false;
        }
    }

    private void SearchForPlayer() {
        // look for player in chasingRange.
        if(target == null) {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, chasingRange / 2, Vector3.down);
            foreach(RaycastHit hit in hits)
                if(hit.transform.GetComponent<Player>() != null) {
                    target = hit.transform.GetComponent<Player>();
                    CheckIfPlayerVisible();
                }
        }
        // check if player isn't behind any obstacle.
        if(target != null) {
            CheckIfPlayerVisible();
        }
    }

    private void CheckIfPlayerVisible() {
        // hart eye-sight position.
        Vector3 sightPosition = new Vector3(
            transform.position.x,
            transform.position.y + eyesHeightOffset,
            transform.position.z
            );

        // debug eye-sight position to the player.
        Debug.DrawRay(sightPosition, (target.transform.position - transform.position), Color.green);

        // check if is still seeing the player and chasing it.
        RaycastHit hit;
        if(Physics.Raycast(sightPosition, (target.transform.position - transform.position), out hit)) {
            if(hit.transform.gameObject == target.transform.gameObject) {
                isPlayerVisible = true;
                isChasingPlayer = true;

                target = hit.transform.GetComponent<Player>();
                playerLastKnownPosition = target.transform.position;

                StartCoroutine(ChasePlayer());
            }
            else {
                isPlayerVisible = false;
                StopCoroutine(ToPlayerLastKnownPosition());
                if(playerLastKnownPosition != new Vector3(0, 0, 0) && !caughtPlayer) {
                    StartCoroutine(ToPlayerLastKnownPosition());
                }
            }
        }
    }

    private IEnumerator ChasePlayer() {
        //chase player
        print("chasing player");
        agent.SetDestination(playerLastKnownPosition);
        yield return null;
    }

    private IEnumerator ToPlayerLastKnownPosition() {
        //gotoplayerslastknowlocation
        agent.SetDestination(playerLastKnownPosition);
        yield return null;
    }

    private void OpenDoor(Collider col) {
        Door door = col.transform.GetComponent<Door>();
        if(door.IsLocked) {
            door.Locked();
            // try to find a way around to the player.
        }
        else {
            door.IsOpen = true;
            door.OpenClose();
            agent.enabled = true;
            agent.stoppingDistance = 0;
        }
    }

    private void OnTriggerEnter(Collider col) {
        // player collision
        if(col.gameObject.tag == "Player") {
            if(!caughtPlayer && isPlayerVisible) {
                col.GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>().IsCaught = true;
                caughtPlayer = true;
                print("caught player");
                // freeze hart's rotation.
                GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }

        // door collision
        if(isChasingPlayer) {
            if(col.transform.GetComponent<Door>()) {
                door = col.transform.GetComponent<Door>();
                if(door && !isPlayerVisible) {
                    agent.enabled = false;
                    OpenDoor(col);

                }
            }
        }
    }
}