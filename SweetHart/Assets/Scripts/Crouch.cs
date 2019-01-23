using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController controller;
    private CharacterController characterController;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (Input.GetButton("Crouch"))
        {
            characterController.height = 1f;
            controller.m_WalkSpeed = controller.m_CrouchSpeed;
        }
        else
        {
            characterController.height = 1.8f;
            controller.m_WalkSpeed = controller.default_WalkSpeed;
        }
    }
}
