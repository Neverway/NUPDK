//=========== Written by Arthur W. Sheldon AKA Lizband_UCC ====================
//
// Purpose: Give player ability to look around
// Applied to:  Player root (The one with the rigidbody attached to it)
// Editor script: 
// Notes: This script was created by following Plai's FPS Movement Tutorial
//  series. (https://www.youtube.com/c/PlaiDev)
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NUPFirstpersonCamera : MonoBehaviour
{
    //=-----------------=
    // Public variables
    //=-----------------=
    [SerializeField] private float sensitivityX;
    [SerializeField] private float sensitivityY;


    //=-----------------=
    // Private variables
    //=-----------------=
    private float lookX;
    private float lookY;
    private float multiplier = 0.01f;
    private float xRotation;
    private float yRotation;


    //=-----------------=
    // Reference variables
    //=-----------------=
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform orietation;
    private NUPFirstpersonWallrun wallrun;



    //=-----------------=
    // Mono Functions
    //=-----------------=
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (GetComponent<NUPFirstpersonWallrun>())
        {
            wallrun = GetComponent<NUPFirstpersonWallrun>();
        }
    }


    private void Update()
    {
        PlayerInput();

        if (wallrun != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, wallrun.tilt);
        }
        else
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        }
        orietation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }



    //=-----------------=
    // Internal Functions
    //=-----------------=
    private void PlayerInput()
    {
        lookX = Input.GetAxisRaw("Mouse X");
        lookY = Input.GetAxisRaw("Mouse Y");

        yRotation += lookX * sensitivityX * multiplier;
        xRotation -= lookY * sensitivityY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Keep the player from breaking their neck when looking too far up or too far down
    }
}
