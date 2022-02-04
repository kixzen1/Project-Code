using UnityEngine;

public class GrapplingGun : Monobehaviour

  /// <summary>
  /// Assign all variables
  /// </summary>
  
  private LineRenderer lr;
  private Vector3 grapplePoint;
  public LayerMask whatIsGrappleable;
  public transform gunTip, camera, player;
  private float maxDistance = 100f;
  private SpringJoint joint;
  
  VOID Awake():
    lr = GetComponent<LineRenderer>()
  END VOID
  
  
  VOID Update():
    IF mouse1 pressed THEN:
      StartGrapple()
    ELIF mouse1 released THEN:
      StopGrapple()
    END IF
  END VOID
  
  
  // Called after the update funtion
  VOID LateUpdate():
    DrawRope()
  END VOID
  
  
  // Call whenever grapple is started
  VOID StartGrapple():
    RaycastHit hit;
    IF Physics.Raycast(camera.position, camera.forwards, 
                       out hit, maxdistance, whatIsGrappleable) THEN:
      grapplePoint = hit.point
      joint = Addcomponent<SpringJoint> to player.gameObject
      set the auto connected anchoring of joint to false
      set the connected anchor of joint to grapplePoint
  
      joint.spring = 4.5f
      joint.damper = 7f
      joint.massScale = 4.5f
  
      set the position count of lr to 2
      set the current grappling position to the position of guntip
     END IF
  END VOID
  
  
  VOID StopGrapple():
    set the position count of lr to 0
    destroy the joint
  END VOID
  
  private Vector3 currentGrapplePosition
  
  VOID DrawRope():
    IF NOT grappling THEN:
      do not draw the rope
    END IF
    
    current grapple position = Vector3.Lerp(current grapple position,
                                            grapplePoint, Time.deltaTime * 8f)
    set position 0 of lr to position of gunTip
    set position 1 of lr to currentGrapplePosition
 END VOID
 
 
  BOOL isGrappling:
    return joint != null
  END FUCNTION
    
    
  VECTOR3 GetGrapplePoint():
    return position of grapplePoint
  END VOID
END CLASS
  
  
  
  
  



  
  
  
  
  
  
