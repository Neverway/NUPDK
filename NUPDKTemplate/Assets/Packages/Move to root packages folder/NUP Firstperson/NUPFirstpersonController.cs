//=========== Written by Arthur W. Sheldon AKA Lizband_UCC ====================
//
// Purpose: Give player ability to move, sprint, jump, crouch, etc.
// Applied to: Player root (The one with the rigidbody attached to it)
// Editor script: 
// Notes: This script was created by following Plai's FPS Movement Tutorial
//  series. (https://www.youtube.com/c/PlaiDev)
//  & the crouching was created by following Comp-3 Interactive's
//  crouching tutorial. (https://www.youtube.com/c/COMP3Interactive)
//
//=============================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NUPFirstpersonController : MonoBehaviour
{
    //=-----------------=
    // Public variables
    //=-----------------=
    public float playerHeight; // Used for getting the distance to the player models feet during a ground check

    [Header("Options")]
    public bool canMove = true;
    public bool canSprint = true;
    public bool canCrouch = true;
    public bool canSlide = true;
    public bool canJump = true;

    [Header("Movement")]
    public float movementSpeed = 6f;
    public float movementMultiplier = 6f; // Used to counteract the physics drag
    public float airMultiplier = 0.4f; // Used to counteract the physics drag

    [Header("Sprinting")]
    public bool sprinting;
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    public float acceleration = 10f;
    
    [Header("Crouching")]
    public bool crouching;
    public float crouchSpeed = 0.3f;
    public float standHeight = 2.0f;
    public float crouchHeight = 1.25f;
    public float crouchOffsetModifer = -0.5f;
    public GameObject viewpoint;
    public CapsuleCollider playerCollider;
    public bool headClear;
    public float headDistance = 0.25f;

    [Header("Crouch Sliding")]
    public bool sliding; 
    public float slideForce = 0.1f; 
    public float slideDuration = 0.1f;
    public float slideDeceleration = 0.05f;

    [Header("Jumping")]
    public float jumpForce = 10f;
    public LayerMask groundMask;

    [Header("Drag")]
    public float groundDrag = 6f;
    public float crouchDrag = 12f;
    public float slideDrag = 1f;
    public float airDrag = 2f;


    //=-----------------=
    // Private variables
    //=-----------------=
    private float horizontalMovement;
    private float verticalMovement;
    private Vector3 moveDirection;

    private Vector3 slopeMoveDirection;
    private RaycastHit slopeHit;
    private float groundDistance = 0.4f;
    private bool isGrounded;

    private bool decreaseSlide = false;
    private bool initalSlide = false; 
    private bool slideStopped = false;
    private float currentSlideDrag; 


    //=-----------------=
    // Reference variables
    //=-----------------=
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform headCheck;
    [SerializeField] private Transform orietation;
    private Rigidbody playerRigidbody;


    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }



    //=-----------------=
    // Mono Functions
    //=-----------------=
    IEnumerator slideTime()
    {
        yield return new WaitForSeconds(slideDuration);     // The delay until it is accepting input again
        decreaseSlide = true;
    }

    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true; // Keep the physics player from just falling over
        currentSlideDrag = slideDrag;
    }

    private void Update()
    {
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);
        headClear = !Physics.Raycast(headCheck.transform.position, Vector3.up, headDistance, groundMask);


        if (canMove) PlayerInput();
        if (canSprint) Sprint();
        if (canCrouch) Crouch();
        if (canSlide) Slide();
        if (canJump) Jump();
        PhysicsDrag();
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        var desiredHeight = crouching ? crouchHeight : standHeight*-1;
        if (playerCollider.height != desiredHeight)
        {
            AdjustHeight(desiredHeight);

            //var viewpointPosition = viewpoint.transform.position;
            //viewpointPosition.y = playerCollider.height;

            //viewpoint.transform.position = viewpointPosition;
        }
    }



    //=-----------------=
    // Internal Functions
    //=-----------------=
    private void PlayerInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orietation.forward * verticalMovement + orietation.right * horizontalMovement; // I don't understand how this converts to a Vector3 (Please explain here later)
    }

    private void PlayerMovement()
    {
        if (isGrounded && !OnSlope())
        {
            playerRigidbody.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier, ForceMode.Acceleration); // moveDirection.normalized is used to fix diagonal acceleration (Up+Left=Fast boi)
        }
        else if (isGrounded && OnSlope())
        {
            playerRigidbody.AddForce(slopeMoveDirection.normalized * movementSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            playerRigidbody.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
            //playerRigidbody.AddForce(moveDirection.normalized * movementSpeed * airMultiplier, ForceMode.Acceleration);
        }
    }

    private void Crouch()
    {
        if (Input.GetKey(KeyCode.LeftControl) && !sprinting)
        {
            crouching = true;
        }
        else if (!Input.GetKey(KeyCode.LeftControl) && !sliding && headClear)
        {
            crouching = false;
        }
    }

    private void Slide()
    {
        if (Input.GetKey(KeyCode.LeftControl) && sprinting && isGrounded && !slideStopped)
        {
            sliding = true;
            crouching = true;
            if (!initalSlide)
            {
                playerRigidbody.AddForce(moveDirection.normalized * movementSpeed * movementMultiplier * slideForce, ForceMode.Acceleration);
                StartCoroutine("slideTime");
                initalSlide = true;
            }
        }
        else
        {
            sliding = false;
            initalSlide = false;
            decreaseSlide = false;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            slideStopped = false;
        }

        if(decreaseSlide && currentSlideDrag < crouchDrag+5)
        {
            currentSlideDrag += slideDeceleration;
        }
        else if (decreaseSlide && currentSlideDrag >= crouchDrag+5)
        {
            sliding = false;
            initalSlide = false;
            decreaseSlide = false;
            slideStopped = true;
            if (headClear) { crouching = false; }
        }
        else if (!decreaseSlide && currentSlideDrag > slideDrag)
        {
            currentSlideDrag -= slideDeceleration * 1.5f;
        }
    }

    private void AdjustHeight(float height)
    {
        float center = height / 2;

        playerCollider.height = Mathf.Lerp(playerCollider.height, height, crouchSpeed);
        if (crouching)
        {
            playerCollider.center = Vector3.Lerp(playerCollider.center, new Vector3(0, center+crouchOffsetModifer, 0), crouchSpeed);
        }
        else
        {
            playerCollider.center = Vector3.Lerp(playerCollider.center, new Vector3(0, center+1, 0), crouchSpeed);
        }
    }

    private void Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift) && isGrounded)
        {
            sprinting = true;
            movementSpeed = Mathf.Lerp(sprintSpeed, walkSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            sprinting = false;
            movementSpeed = Mathf.Lerp(walkSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
    }
    
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, 0, playerRigidbody.velocity.z);
            playerRigidbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);   
        }
    }

    private void PhysicsDrag()
    {
        if (isGrounded && !crouching)
        {
            playerRigidbody.drag = groundDrag;
        }
        else if (isGrounded && crouching && !sliding)
        {
            playerRigidbody.drag = crouchDrag;
        }
        else if (isGrounded && sliding)
        {
            playerRigidbody.drag = currentSlideDrag;
        }
        else if (!isGrounded)
        {
            playerRigidbody.drag = airDrag;
        }
    }

}
