# Pseudocode - Stage 1

```csharp
using System;
using UnityEngine;

1 public class PlayerMovement : Monobehaviour
2
3    DECLARE ALL WALLRUNNING VARIABLES
4
5    VOID WallChecker():
6        isRight = cast ray 1f to the right
7        isLeft = cast ray 1f to the left
8        if NOT isLeft AND NOT isRight:
9            StopRun()
10    END VOID
11
12
13    VOID WallRunIn():
14        IF D is pressed and on right wall THEN StartRun()
15        IF A is pressed and on left wall THEN StartRun()
16    END VOID
17
18
19    VOID StartRun():
20        set RigidBody's (rb) gravity to false
21        Wallrunning is True
22
23        IF the magnitude of the velocity of rb <= max wallrunning speed THEN:
24            Add forward force * Time.deltaTime to rb
25
26            IF isRight THEN:
27                Add right force * Time.deltaTime to rb
28            ELSE:
29                Add left force * Time.deltaTime to rb
30
31            END IF
32
33        END IF
34
35    END VOID
36
37
38    VOID StopRun():
39        set rb gravity = true
40        Wallrunning is False
41    END VOID 
42
43 ----------------------------------------------------------------------------------------------
44
45    //Assingables
46    public Transform playerCam;
47    public Transform orientation;
48
49    //Other
50    private Rigidbody rb;
51
52    //Rotation and look
53    private float xRotation;
54    private float sensitivity = 50f;
55    private float sensMultiplier = 1f;
56
57    //Movement
58    public float moveSpeed = 4500;
59    public float maxSpeed = 20;
60    public bool grounded;
61    public LayerMask whatIsGround;
62
63    public float counterMovement = 0.175f;
64    private float threshold = 0.01f;
65    public float maxSlopeAngle = 35f;
66
67    //Crouch & Slide
68    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
69    private Vector3 playerScale;
70    public float slideForce = 400;
71    public float slideCounterMovement = 0.2f;
72
73    //Jumping
74    private bool readyToJump = true;
75    private float jumpCooldown = 0.25f;
76    public float jumpForce = 550f;
77
78    //Input
79    float x, y;
80    bool jumping, sprinting, crouching;
81
82    //Sliding
83    private Vector3 normalVector = Vector3.up;
84    private Vector3 wallNormalVector;
85
86 ----------------------------------------------------------------------------------------------
87
88    VOID Awake():
89        rb = GetComponent <RigidBody>()
90    END VOID
91
92
93    VOID Start():
94        set playerScale to the localScal of the transformed asset
95        lock the cursor
96        hide the cursor
97    END VOID
98
99
100    VOID FixedUpdate():
101        Movement()
102    END VOID
103
104
105    VOID Update():
106        MyInput()
107        Look()
108        WallChecker()
109        WallRunIn()
110    END VOID
111
112 ----------------------------------------------------------------------------------------------
113
114    VOID MyInput():
115        x = raw input of the horizontal axis
116        y = raw input of the vertical axis
117        jumping = get space bar button status
118        crouching = get LCTRL button status
119
120        IF LCTRL pressed:
121            StartCrouch()
122        IF LCTRL released:
123            StopCrouch()
124        
125        END IF
126
127    END VOID
128
129 ----------------------------------------------------------------------------------------------
130
131    VOID StartCrouch():
132        set the localScale to crouchScale
133        transform.position = new Vector3(
134            transform.position.x, transform.position.y - 0.5f, transform.position.z);
135        IF magnitude of velocity of rb > 0.5f THEN:
136            IF grounded THEN:
137                add forward force * slideForce to rb
138            END IF
139        END IF
140    END VOID
141
142
143    VOID StopCrouch():
144        set the localScale to playerScale
145        transform.position = new Vector3(
146            tranform.position.x, transform.position.y + 0.5f, transform.position.z)
147        )
148    END VOID
149
150 ----------------------------------------------------------------------------------------------
151
152    VOID Movement():
153        // extra gravity for player
154        add (down force * time.deltaTime * 10) to rb
155
156        // Find velocity relative to look
157        create new 2D Vector mag = FindVelRelativeToLook()
158        float xMag = mag.x
159        float yMag = mag.y
160
161        // counter-movement
162        counterMovement(x, y, mag)
163
164        IF readyToJump && jumping THEN:
165            Jump()
166        END IF
167
168        float maxSpeed = this.maxSpeed
169
170        IF player is crouching AND on ground AND ready to jump THEN:
171            add (down force * time.deltaTime * 3000) to rb
172            return
173        END IF
174
175        IF x > 0 and xMag > maxSpeed THEN:
176            x = 0
177        IF x < 0 and xMag < -maxSpeed THEN:
178            x = 0
179        IF y > 0 and yMag > maxSpeed THEN:
180            y = 0
181        IF y < 0 and yMag < -maxspeed THEN:
182            y = 0
183        END IF
184
185        float multiplier = 1f
186        float multiplierV = 1f
187
188        IF player is NOT grounded THEN:
189            multiplier = 0.5f
190            multiplierV = 0.5f
191        END IF
192
193        IF player is grounded AND player is crouching THEN:
194            multiplierV = 0f
195        END IF
196
197        rb.AddForce(
198            orientation.tranform.forward * y * moveSpeed * Time.deltaTime
199             * multiplier * multiplierV
200        )
201
202        rb.AddForce(
203            orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier
204        )
205    END VOID
206
207
208    VOID CounterMovement(parameters: float x ,float y, Vector2 mag):
209        
210        IF player is NOT grounded OR player is jumping THEN:
211            return;
212        END IF
213
214        // SLIDING SLOW-DOWN
215
216        IF player is crouching THEN:
217            add counter-force to player
218        END IF
219        // ACTUAL COUNTER-MOVEMENT
220
221       IF the absoulte value of the magnitude of x > threshold && the absolute value of x < 0.05f || (magnitude of x < -threshold && x > 0) || (magnitude of x > threshold && 222 x < 0) THEN:
223            add (moveSpeed * right force * Time.deltaTime * -magnitude of x * counterMovement) force to rb
224        END IF
225
226        IF the absolute value of the magnitude of y > threshold && the absolute value of y < 0.05f || (magnitude of y < -threshold && y > 0) || (magnitude of y > threshold 227 && y < 0) THEN:
228            add (moveSpeed * right force * time.deltaTime * -magnitude of y * counterMovement) force to rb
229        END IF
230
231        IF SquareRoot of the (velocity of x)^2 + (velocity of z)^2 > maxSpeed THEN:
232            float fallspeed = velocity of rb on the x axis
233            Vector3 n = normalised velocity of rb * maxSpeed
234            rb.velocity = new Vector3(x-axis of n, fallspeed, z axis of n)
235        END IF
236    END VOID
237
238 ----------------------------------------------------------------------------------------------
239
240    VOID Jump():
241        
242        IF player is grounded AND player is ready to jump THEN:
243            readyToJump = False
244
245            // Jump Forces
246
247            rb.AddForce(Vector2.up * jumpForce * 1.5f)
248            rb.AddForce(normalVector * jumpForce * 0.5f)
249
250            // If jumping whilst falling reset y velocity
251            Vector3 vel = rb.velocity
252            IF rb.velocity.y < 0.5f THEN:
253                rb.velocity = new Vector3 (vel.x, 0, vel.z)
254            IF rb.velocity.y < 0 THEN:
255                rb.velocity - new Vector3(vel.x, vel.y / 2, vel.z)
256            END IF
257
258            Invoke nameOf(resetJump), jumpCooldown
259
260        IF player is wallrunning THEN:
261            readyToJump = False
262
263            IF isLeft && D is NOT pressed THEN:
264                add upward force to rb
265            END IF
266
267            IF isRight || isLeft && Input.GetKey(KeyCode.A) || 
268               Input.GetKey(KeyCode.D) THEN:
269                    add downwards force * jumpForce to rb
270            
271            IF isRight && Input.GetKey(KeyCode.A) THEN: // jumping from wall
272                add left * jump force to rb
273            END IF
274
275            IF isLeft && Input.GetKey(KeyCode.D) THEN: // jumping from wall
276                add right * jump force to rb
277            END IF
278
279            add forward force to rb
280
281            rb.velocity = Vector3.zero
282
283            Invoke(NameOf(ResetJump), jumpCooldown)
284        END IF
285    END VOID
286
287
288    VOID ResetJump():
289        player is NOT ready to jump
290    END VOID
291
292 ----------------------------------------------------------------------------------------------
293    
294    private float desiredX
295
296    VOID Look():
297        float mouseX = (get X axis) * sensitivity * Time.fixedDeltaTime
298                       * sensMultiplier
299        float mouseY = (get Y axis) * sensitivity * Time.fixedDeltaTime
300                       * sensMultiplier
301
302        // Current look rotation
303
304        Vector3 rot - playerCam.transform.localRotation.eulerAngles
305        desiredX = rot.y + mouseX
306
307        // rotating but clamping so player cannot look upwards past 3.14c
308
309        xRotation -= mouseY
310        xRotation = Mathf.Clamp(xRotation, -90f, 90f)
311
312        //actual rotation being performed
313
314        playerCam.transform.localRotation = Quarternion.euler(xRotation,
315                                            desiredX, tilt)
316        orientation.tranform.localRotation = Quarternion.euler(0, desiredX, 0)
317
318        // Camera tilt for WALLRUN
319
320        IF the absolute value of tilt < max value for tilt &&
321           player is wallrunning && isRight THEN:
322                increment tilt by Time.deltaTime * maxTilt * 2
323        END IF
324
325        IF the absolute value of tilt < max value for tilt &&
326           player is wallrunning && isRight THEN:
327                decrement tilt by Time.deltaTime * maxTilt * 2
328        END IF
329
330        // resetting tilt
331
332        IF tilt > 0 && NOT isRight && NOT isLeft THEN:
333            decrement tilt by Time.deltaTime * maxTilt * 2
334        END IF
335
336        IF tilt < 0 && NOT isRight && NOT isLeft THEN:
337            increment tilt by Time.deltaTime * maxTilt * 2
338        END IF
339
340    END VOID
341
342
343    VECTOR2 FindVelRelativeToLook():
344        float lookAngle = orientation.transform.eulerAngles.y
345        float moveAngle = quotient value of the tangent of (velocity of x, velocity of z) converted from radians to degrees
346
347        float u = the change in angle (lookAngle, moveAngle)
348        float v = 90 - u
349
350        float magnitude = magnitude of the velocity of rb
351        float yMag = magnitude * cosine value of (u converted to radians)
352        float xMag = magnitude * cosine value of (v converted to radians)
353
354        return new Vector2(xMag, yMag)
355    END FUNCTION
356
357 ----------------------------------------------------------------------------------------------
358
359    BOOL IsFloor(Vector3 v):
360        float angle = Vector3.Angle(upforce of Vector3, v)
361        return angle < maxSlopeAngle
362    END FUNCTION
363
364
365    private bool cancellingGrounded
366
367
368    VOID OnCollisionStay(Collision other):
369        int layer = other.gameObject.layer
370
371        IF WhatisGround NOT equal to (whatIsGround | (1 << layer))) THEN:
372            return
373        END IF
374
375        FOR i = 0, i < other.contactcount, i++:
376            Vector3 normal = other.contact[i].normal
377
378            IF isFloor(normal) THEN:
379                grounded = true
380                cancellingGrounded = false
381                normalVector = normal
382                CancelInvoke(nameof(StopGrounded))
383            END IF
384        END FOR
385
386        float delay = 3f
387
388        IF NOT cancellingGrounded THEN:
389            cancellingGrounded = true
390            Invoke(nameof(StopGrounded), time.deltatime * delay)
391        END IF
392    END VOID
393
394
395    VOID StopGrounded()
396        grouded = false
397    END VOID
```
