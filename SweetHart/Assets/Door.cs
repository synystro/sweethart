﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool opensInwards;
    [SerializeField] private float openingSpeed;

    private float targetAngle;
    private bool isOpen;

    void Start()
    {

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
        } else {
            targetAngle = 0f;
        }
    }
}
