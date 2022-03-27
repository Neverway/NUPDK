//=========== Written by Arthur W. Sheldon AKA Lizband_UCC ====================
//
// Purpose: 
// Applied to: 
// Editor script: 
// Notes: 
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NUPFirstpersonWallrun : MonoBehaviour
{
    //=-----------------=
    // Public variables
    //=-----------------=
    [Header("Wallrunning")]
    public LayerMask wallrunableMask;
    public float wallDistance = 0.55f;
    public float minimumJumpHeight = 0.5f;
    public float wallrunGravity = 2f;
    public float wallrunJumpForce = 3f;
    public Vector3 wallrunExitAngleModifer = new Vector3(0, 0.25f, 0);
    public float forwardKickoff = 0.25f;

    [Header("Camera Effects")]
    public Camera playerCamera;
    public float wallrunFovModifier = 5f;
    public float wallrunFovTime = 10f;
    public float cameraTilt = 5f;
    public float cameraTiltTime = 10f;

    public float tilt { get; private set; }


    //=-----------------=
    // Private variables
    //=-----------------=
    private bool wallLeft = false;
    private bool wallRight = false;

    private float fov;
    private float wallrunFov;


    //=-----------------=
    // Reference variables
    //=-----------------=
    public Transform orietation;
    private Rigidbody playerRigidbody;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;


    private bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minimumJumpHeight);
    }



    //=-----------------=
    // Mono Functions
    //=-----------------=
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        fov = playerCamera.fieldOfView;
        wallrunFov = fov+wallrunFovModifier;
    }

    private void Update()
    {
        CheckWalls();

        if (CanWallRun())
        {
            if (wallLeft || wallRight)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }
        }
        else
        {
            StopWallRun();
        }
    }
    
    
    
    //=-----------------=
    // Internal Functions
    //=-----------------=
    private void CheckWalls()
    {
        wallLeft = Physics.Raycast(transform.position, -orietation.right, out leftWallHit, wallDistance, wallrunableMask);
        wallRight = Physics.Raycast(transform.position, orietation.right, out rightWallHit, wallDistance, wallrunableMask);
    }

    private void StartWallRun()
    {
        playerRigidbody.useGravity = false;
        playerRigidbody.AddForce((Vector3.down * wallrunGravity), ForceMode.Force);
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, wallrunFov, wallrunFovTime * Time.deltaTime);

        if (wallLeft) { tilt = Mathf.Lerp(tilt, -cameraTilt, cameraTiltTime * Time.deltaTime); }
        if (wallRight) { tilt = Mathf.Lerp(tilt, cameraTilt, cameraTiltTime * Time.deltaTime); }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallLeft)
            {
                Vector3 wallrunJumpDirection = transform.up + leftWallHit.normal + wallrunExitAngleModifer + (orietation.transform.forward * forwardKickoff);
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                playerRigidbody.AddForce(wallrunJumpDirection * wallrunJumpForce * 100, ForceMode.Force);
            }
            else if (wallRight)
            {
                Vector3 wallrunJumpDirection = transform.up + rightWallHit.normal + wallrunExitAngleModifer + (orietation.transform.forward * forwardKickoff);
                playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
                playerRigidbody.AddForce(wallrunJumpDirection * wallrunJumpForce * 100, ForceMode.Force);
            }
        }
    }

    private void StopWallRun()
    {
        playerRigidbody.useGravity = true;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, fov, wallrunFovTime * Time.deltaTime);
        tilt = Mathf.Lerp(tilt, 0, cameraTiltTime * Time.deltaTime);
    }
    
    
    
    //=-----------------=
    // External Functions
    //=-----------------=
}
