using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Door : MonoBehaviour
{

    //public UnityStandardAssets.Characters.FirstPerson.FirstPersonController player;

    [SerializeField] private string doorID;
    [SerializeField] private bool opensInwards;
    [SerializeField] private float openingSpeed;
    [SerializeField] private bool isLocked;

    private FirstPersonController player;
    private float targetAngle;
    private bool isOpen;

    public string DoorID { get { return doorID; } }

    void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonController>();
    }

    void Update()
    {
        Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, targetAngle, 0), openingSpeed * Time.deltaTime);
        transform.localRotation = smoothRotation;
    }

    public void Interact()
    {
            isOpen = !isOpen;
            if (isOpen)
            {
                if (opensInwards) targetAngle = -90f;
                else targetAngle = 90f;
            }
            else
            {
                targetAngle = 0f;
            }
    }

    public void Locked()
    {
        Debug.Log("This door is locked.");
        // locked sound.
    }
}

