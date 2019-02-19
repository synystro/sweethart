using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spirit : MonoBehaviour
{

    [Header("Chasing")]
    [SerializeField] protected float chasingRange;

    [Header("Flying")]
    [SerializeField] private float distanceFromFloor;

    protected Rigidbody enemyRigidbody;
    protected Player target;

    void Awake()
    {
        enemyRigidbody = transform.GetComponent<Rigidbody>();
    }
    void Update()
    {

    }

    public void Float()
    {
        // make the spirit float
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Vector3 targetPosition = hit.point;

            // move the spirit up.
            targetPosition = new Vector3(
                targetPosition.x,
                targetPosition.y + distanceFromFloor,
                targetPosition.z
                );
            transform.position = targetPosition;
        }
    }

}
