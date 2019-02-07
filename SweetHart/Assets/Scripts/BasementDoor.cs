using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasementDoor : Door
{
    void Update()
    {
        Quaternion smoothRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(targetAngle, 0, 0), openingSpeed * Time.deltaTime);
        transform.localRotation = smoothRotation;
        if(transform.localEulerAngles.x == targetAngle) {
            isMoving = false;
        }
    }
}
