# Pseudocode - Stage 1

```csharp
using System;
using UnityEngine;

public class PlayerMovement : Monobehaviour

    DECLARE ALL WALLRUNNING VARIABLES

    VOID WallChecker():
        isRight = cast ray 1f to the right
        isLeft = cast ray 1f to the left
        if NOT isLeft AND NOT isRight:
            StopRun()
    END VOID

    VOID WallRunIn():
        IF D is pressed and on right wall THEN StartRun()
        IF A is pressed and on left wall THEN StartRun()
    END VOID

    VOID StartRun():
        set RigidBody's (rb) gravity to false
        Wallrunning is True

        IF the magnitude of the velocity of rb <= max wallrunning speed THEN:
            Add forward force * Time.deltaTime to rb

            IF isRight THEN:
                Add right force * Time.deltaTime to rb
            ELSE:
                Add left force * Time.deltaTime to rb

            END IF

        END IF

    END VOID

    VOID StopRun():
        set rb gravity = true
        Wallrunning is False
    END VOID 

----------------------------------------------------------------------------------------------

    //Assingables
    public Transform playerCam;
    public Transform orientation;

    //Other
    private Rigidbody rb;

    //Rotation and look
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;

    //Movement
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public bool grounded;
    public LayerMask whatIsGround;

    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;

    //Crouch & Slide
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    //Jumping
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;

    //Input
    float x, y;
    bool jumping, sprinting, crouching;

    //Sliding
    private Vector3 normalVector = Vector3.up;
    private Vector3 wallNormalVector;

----------------------------------------------------------------------------------------------

    VOID Awake():
        rb = GetComponent <RigidBody>()
    END VOID

    VOID Start():
        set playerScale to the localScal of the transformed asset
        lock the cursor
        hide the cursor
    END VOID

    VOID FixedUpdate():
        Movement()
    END VOID

    VOID Update():
        MyInput()
        Look()
        WallChecker()
        WallRunIn()
    END VOID

----------------------------------------------------------------------------------------------

    VOID MyInput():
        x = raw input of the horizontal axis
        y = raw input of the vertical axis
        jumping = get space bar button status
        crouching = get LCTRL button status

        IF LCTRL pressed:
            StartCrouch()
        IF LCTRL released:
            StopCrouch()
        
        END IF

    END VOID

----------------------------------------------------------------------------------------------

    VOID StartCrouch():
        set the localScale to crouchScale
        transform.position = new Vector3(
            transform.position.x, transform.position.y - 0.5f, transform.position.z);
        IF magnitude of velocity of rb > 0.5f THEN:
            IF grounded THEN:
                add forward force * slideForce to rb
            END IF
        END IF
    END VOID

    VOID StopCrouch():
        set the localScale to playerScale
        transform.position = new Vector3(
            tranform.position.x, transform.position.y + 0.5f, transform.position.z)
        )
    END VOID

----------------------------------------------------------------------------------------------

    VOID Movement():
        // extra gravity for player
        add (down force * time.deltaTime * 10) to rb

        // Find velocity relative to look
        create new 2D Vector mag = FindVelRelativeToLook()
        float xMag = mag.x
        float yMag = mag.y

        // counter-movement
        counterMovement(x, y, mag)

        IF readyToJump && jumping THEN:
            Jump()
        END IF

        float maxSpeed = this.maxSpeed

        IF player is crouching AND on ground AND ready to jump THEN:
            add (down force * time.deltaTime * 3000) to rb
            return
        END IF

        IF x > 0 and xMag > maxSpeed THEN:
            x = 0
        IF x < 0 and xMag < -maxSpeed THEN:
            x = 0
        IF y > 0 and yMag > maxSpeed THEN:
            y = 0
        IF y < 0 and yMag < -maxspeed THEN:
            y = 0
        END IF

        float multiplier = 1f
        float multiplierV = 1f

        IF player is NOT grounded THEN:
            multiplier = 0.5f
            multiplierV = 0.5f
        END IF

        IF player is grounded AND player is crouching THEN:
            multiplierV = 0f
        END IF

        rb.AddForce(
            orientation.tranform.forward * y * moveSpeed * Time.deltaTime
             * multiplier * multiplierV
        )

        rb.AddForce(
            orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier
        )
    END VOID

    VOID CounterMovement(parameters: float x ,float y, Vector2 mag):
        
        IF player is NOT grounded OR player is jumping THEN:
            return;
        END IF

        // SLIDING SLOW-DOWN

        IF player is crouching THEN:
            add counter-force to player
        END IF
        // ACTUAL COUNTER-MOVEMENT

        IF the absoulte value of the magnitude of x > threshold && the absolute value of x < 0.05f || (magnitude of x < -threshold && x > 0) || (magnitude of x > threshold && x < 0) THEN:
            add (moveSpeed * right force * Time.deltaTime * -magnitude of x * counterMovement) force to rb
        END IF

        IF the absolute value of the magnitude of y > threshold && the absolute value of y < 0.05f || (magnitude of y < -threshold && y > 0) || (magnitude of y > threshold && y < 0) THEN:
            add (moveSpeed * right force * time.deltaTime * -magnitude of y * counterMovement) force to rb
        END IF

        IF SquareRoot of the (velocity of x)^2 + (velocity of z)^2 > maxSpeed THEN:
            float fallspeed = velocity of rb on the x axis
            Vector3 n = normalised velocity of rb * maxSpeed
            rb.velocity = new Vector3(x-axis of n, fallspeed, z axis of n)
        END IF
    END VOID

----------------------------------------------------------------------------------------------

    VOID Jump():
        
        IF player is grounded AND player is ready to jump THEN:
            readyToJump = False

            // Jump Forces

            rb.AddForce(Vector2.up * jumpForce * 1.5f)
            rb.AddForce(normalVector * jumpForce * 0.5f)

            // If jumping whilst falling reset y velocity
            Vector3 vel = rb.velocity
            IF rb.velocity.y < 0.5f THEN:
                rb.velocity = new Vector3 (vel.x, 0, vel.z)
            IF rb.velocity.y < 0 THEN:
                rb.velocity - new Vector3(vel.x, vel.y / 2, vel.z)
            END IF

            Invoke nameOf(resetJump), jumpCooldown

        IF player is wallrunning THEN:
            readyToJump = False

            IF isLeft && D is NOT pressed THEN:
                add upward force to rb
            END IF

            IF isRight || isLeft && Input.GetKey(KeyCode.A) || 
               Input.GetKey(KeyCode.D) THEN:
                    add downwards force * jumpForce to rb
            
            IF isRight && Input.GetKey(KeyCode.A) THEN: // jumping from wall
                add left * jump force to rb
            END IF

            IF isLeft && Input.GetKey(KeyCode.D) THEN: // jumping from wall
                add right * jump force to rb
            END IF

            add forward force to rb

            rb.velocity = Vector3.zero

            Invoke(NameOf(ResetJump), jumpCooldown)
        END IF
    END VOID

    VOID ResetJump():
        player is NOT ready to jump
    END VOID

----------------------------------------------------------------------------------------------
    
    private float desiredX

    VOID Look():
        float mouseX = (get X axis) * sensitivity * Time.fixedDeltaTime
                       * sensMultiplier
        float mouseY = (get Y axis) * sensitivity * Time.fixedDeltaTime
                       * sensMultiplier

        // Current look rotation

        Vector3 rot - playerCam.transform.localRotation.eulerAngles
        desiredX = rot.y + mouseX

        // rotating but clamping so player cannot look upwards past 3.14c

        xRotation -= mouseY
        xRotation = Mathf.Clamp(xRotation, -90f, 90f)

        //actual rotation being performed

        playerCam.transform.localRotation = Quarternion.euler(xRotation,
                                            desiredX, tilt)
        orientation.tranform.localRotation = Quarternion.euler(0, desiredX, 0)

        // Camera tilt for WALLRUN

        IF the absolute value of tilt < max value for tilt &&
           player is wallrunning && isRight THEN:
                increment tilt by Time.deltaTime * maxTilt * 2
        END IF

        IF the absolute value of tilt < max value for tilt &&
           player is wallrunning && isRight THEN:
                decrement tilt by Time.deltaTime * maxTilt * 2
        END IF

        // resetting tilt

        IF tilt > 0 && NOT isRight && NOT isLeft THEN:
            decrement tilt by Time.deltaTime * maxTilt * 2
        END IF

        IF tilt < 0 && NOT isRight && NOT isLeft THEN:
            increment tilt by Time.deltaTime * maxTilt * 2
        END IF

    END VOID

    VECTOR2 FindVelRelativeToLook():
        float lookAngle = orientation.transform.eulerAngles.y
        float moveAngle = quotient value of the tangent of (velocity of x, velocity of z) converted from radians to degrees

        float u = the change in angle (lookAngle, moveAngle)
        float v = 90 - u

        float magnitude = magnitude of the velocity of rb
        float yMag = magnitude * cosine value of (u converted to radians)
        float xMag = magnitude * cosine value of (v converted to radians)

        return new Vector2(xMag, yMag)
    END FUNCTION

    BOOL IsFloor(Vector3 v):
        float angle = Vector3.Angle(upforce of Vector3, v)
        return angle < maxSlopeAngle
    END FUNCTION

    private bool cancellingGrounded

    VOID OnCollisionStay(Collision other):
        int layer = other.gameObject.layer

        IF WhatisGround NOT equal to (whatIsGround | (1 << layer))) THEN:
            return
        END IF

        FOR i = 0, i < other.contactcount, i++:
            Vector3 normal = other.contact[i].normal

            IF isFloor(normal) THEN:
                grounded = true
                cancellingGrounded = false
                normalVector = normal
                CancelInvoke(nameof(StopGrounded))
            END IF
        END FOR

        float delay = 3f

        IF NOT cancellingGrounded THEN:
            cancellingGrounded = true
            Invoke(nameof(StopGrounded), time.deltatime * delay)
        END IF
    END VOID

    VOID StopGrounded()
        grouded = false
    END VOID
```
