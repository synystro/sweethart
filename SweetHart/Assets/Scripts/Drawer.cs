using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drawer : MonoBehaviour
{
    [SerializeField] private float openingSpeed;
    [SerializeField] private float openingDistance;
    [SerializeField] private bool isLocked;
    [SerializeField] private bool isOpen;

    public bool IsLocked { get { return isLocked; } }

    private Vector3 originalPosition;
    private Vector3 openedPosition;

    void Start()
    {
        originalPosition = transform.position;
        openedPosition = transform.position + (Vector3.forward * openingDistance);
    }
    void Update()
    {
        if (isOpen)
        {
            transform.position = Vector3.Lerp(transform.position, openedPosition, openingSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, openingSpeed * Time.deltaTime);
        }
    }
    public void Interact()
    {
        isOpen = !isOpen;
    }
}
