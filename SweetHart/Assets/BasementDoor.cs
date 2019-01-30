using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementDoor : Door
{
    void Update()
    {
        Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetAngle, 0, 0), openingSpeed * Time.deltaTime);
        transform.localRotation = smoothRotation;
    }

    public void Interact()
    {
        isOpen = !isOpen;
        OpenClose();
    }

    private void OpenClose()
    {
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
}
