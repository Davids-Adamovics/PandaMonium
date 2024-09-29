using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
            StartGrapple();
        else if (Input.GetMouseButtonUp(1))
            StopGrapple();
    }

    void LateUpdate() // Called after Update()
    {
        if (joint != null)
        {
            DrawRope();
        }
        else
        {
            lr.enabled = false;
        }
    }

    void StartGrapple()
    {
        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, maxDistance))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            lr.enabled = true;
        }
    }

    void DrawRope()
    {
        if (!joint) return;

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, grapplePoint);
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }

    public bool IsGrappling() // Changed method name to avoid conflict
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }
}
