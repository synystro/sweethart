 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Door : MonoBehaviour
{
    [SerializeField] protected string doorID;
    [SerializeField] protected bool opensInwards;
    [SerializeField] protected float openingSpeed;
    [SerializeField] protected bool isLocked;
    [SerializeField] protected bool isOpen;

    protected float targetAngle;
    protected float closedTimer;
    protected bool isMoving;

    public string DoorID { get { return doorID; } }
    public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
    public bool IsLocked { get { return isLocked; } }

    void Start()
    {
        OpenClose();
    }

    void Update()
    {
        if(isMoving) {
            Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, targetAngle, 0), openingSpeed * Time.deltaTime);
            transform.localRotation = smoothRotation;
            if(transform.localEulerAngles.y == targetAngle) {
                isMoving = false;
            }
        }
    }

    public void Interact()
    {
        isOpen = !isOpen;
        OpenClose();
    }

    public void OpenClose()
    {
        if (isOpen) {
            if (opensInwards) targetAngle = -90f;
            else targetAngle = 90f;
        }
        else {
            targetAngle = 0f;
        }
        isMoving = true;
    }

    public void Locked()
    {
        Debug.Log("This door is locked.");
        // locked sound.
    }
}

