using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Hart : Spirit
{
    public GameObject aStar;
    PathFinding pathF;

    [SerializeField] private float eyesHeightOffset;
    [SerializeField] private float interactionDistance;

    private int nextNodePos;
    private const int nextNodePosChase = 1;

    private List<Node> playerLastKnownLocation;
    private int movementIndex;
    private bool isPlayerVisible;
    private bool isChasingPlayer;
    private bool caughtPlayer;
    private Door door;

    void Start()
    {
        pathF = aStar.GetComponent<PathFinding>();
        nextNodePos = 1;
    }

    void Update()
    {
        base.Float();
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
            // check if player isn't behind any obstacle.
            if(target != null) {
                CheckIfPlayerVisible();
            }
        }

    private void CheckIfPlayerVisible() {
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

                playerLastKnownLocation = pathF.FinalPath;
                nextNodePos = 1;

                if(!caughtPlayer) {
                    StopCoroutine(ToPlayerLastKnownPosition());
                    StartCoroutine(ChasePlayer());
                }
                else {
                    //TODO stop hart.
                }
            }
            else {
                isPlayerVisible = false;
                StopCoroutine(ChasePlayer());
                StartCoroutine(ToPlayerLastKnownPosition());
            }
        }
    }

    private IEnumerator ChasePlayer() {

        if(pathF.FinalPath == null) {
            // wait for path to be created.
        }
        else {
            if(playerLastKnownLocation.Count == 1) {
                // get closer and jumpscare player.
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerLastKnownLocation[0].position.x, transform.position.y, playerLastKnownLocation[0].position.z), chasingSpeed * Time.deltaTime);
            }
            else {
                // chase player node after node.
                if(transform.position.x != playerLastKnownLocation.Last().position.x && transform.position.z != playerLastKnownLocation.Last().position.z) {
                    if(transform.position.x != playerLastKnownLocation[nextNodePosChase].position.x && transform.position.z != playerLastKnownLocation[nextNodePosChase].position.z) {
                        transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerLastKnownLocation[nextNodePosChase].position.x, transform.position.y, playerLastKnownLocation[nextNodePosChase].position.z), chasingSpeed * Time.deltaTime);
                    }
                }
            }
        }
        yield return null;
    }

    private IEnumerator ToPlayerLastKnownPosition() {
        if(transform.position.x != playerLastKnownLocation.Last().position.x && transform.position.z != playerLastKnownLocation.Last().position.z) {
                if(transform.position.x != playerLastKnownLocation[nextNodePos].position.x && transform.position.z != playerLastKnownLocation[nextNodePos].position.z) {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(playerLastKnownLocation[nextNodePos].position.x, transform.position.y, playerLastKnownLocation[nextNodePos].position.z), chasingSpeed * Time.deltaTime);
            }
            else {
                if(nextNodePos < playerLastKnownLocation.Count) {
                    nextNodePos++;
                }
            }
        }
        else {
            // player's last known location reached. reset nodePos to 1 for the next time.
            nextNodePos = 1;
        }
        yield return null;
    }

    private void OpenDoor(Collider col) {
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
                // freeze hart's rotation.
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