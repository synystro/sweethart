using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hart : Spirit
{
    Grid grid;

    [SerializeField] private float eyesHeightOffset;
    [SerializeField] private float interactionDistance;

    private Vector3 playerLastKnownLocation;
    private bool isPlayerVisible;
    private bool isChasingPlayer;
    private bool caughtPlayer;
    private Door door;

    void Start()
    {
        grid = GetComponent<Grid>();
    }

    void Update()
    {
        //base.Float();
        if(!caughtPlayer) {
            SearchForPlayer();
        } else { //TODO: stop hart. }
        }
    }

        private void SearchForPlayer()
        {
            // look for player in chasingRange.
            if(target == null) {
                RaycastHit[] hits = Physics.SphereCastAll(transform.position, chasingRange / 2, Vector3.down);
                foreach(RaycastHit hit in hits)
                    if(hit.transform.GetComponent<Player>() != null) {
                        target = hit.transform.GetComponent<Player>();
                        CheckIfPlayerVisible();
                    }
            }
            // check if player isn't behind any obstacle
            if(target != null) {
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
            if(Physics.Raycast(sightPosition, (target.transform.position - transform.position), out hit)) {
                if(hit.transform.gameObject == target.transform.gameObject) {
                    isPlayerVisible = true;
                    isChasingPlayer = true;

                    target = hit.transform.GetComponent<Player>();
                    playerLastKnownLocation = target.transform.position;

                    if(!caughtPlayer) {
                        ChasePlayer();
                    }
                    else {
                        //TODO stop hart.
                    }
                }
                else {
                    isPlayerVisible = false;
                }
            }
        }

        private void ChasePlayer()
        {
        //transform.position = Vector3.MoveTowards(transform.position, playerLastKnownLocation, chasingSpeed * Time.deltaTime);
            if(transform.position == playerLastKnownLocation) {
                isChasingPlayer = false;
                //TODO: patrol mode.
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
                        //TODO: move hart.
                    }
                    else {
                        //TODO: stop hart.
                    }
                }
            }
        }
    }
}