using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour

{
    // Inscribed fields are set in the Unity Inspector pane
    [Header("Inscribed")]

    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;

    [Header("Dynamic")]

    public GameObject launchPoint;
    public Vector3 launchPos;

    public GameObject projectile;

    public bool aimingMode;

    private void Awake()
    {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }
    void OnMouseEnter()
    {
        //print("SlingShot:OnMouseEnter()");
        launchPoint.SetActive(true);
    }

     void OnMouseExit()
    {
        //print("SlingShot:OnMouseExit()");
        launchPoint.SetActive(false);
    }

    void OnMouseDown()
    {
        aimingMode = true;
        // Instantiate a Projectile
        projectile = Instantiate(projectilePrefab as GameObject);
        // Start it at the launchPoint
        projectile.transform.position = launchPos;
        // Set it to isKinematic
        projectile.GetComponent<Rigidbody>().isKinematic = true;
    }

    void Update()
    {
        // If Slinshot is not in aimingMode, don't run this code
        if (!aimingMode) return;

        //Get the current mouse position in 2D screen coordinates
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Find the delta from the launchPos to the mousePos3d
        Vector3 mouseDelta = mousePos3D - launchPos;

        // Limit mouseDelta to the radius of the Slingshot SphereCollider
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude)
        {
            mouseDelta.Normalize();

            mouseDelta *= maxMagnitude;
        }
        // Move the projectile to this new position
        Vector3 projPos = launchPos + mouseDelta;
        projectile.transform.position = projPos;

        if (Input.GetMouseButtonUp(0))
        {
            // The mouse has been realeased
            aimingMode = false;
            Rigidbody projRB = projectile.GetComponent<Rigidbody>();
            projRB.isKinematic = false;
            // Enumeration - creating a hierarchy object set (like base 10, days in a week. sets of things.)  
            projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
            projRB.velocity = -mouseDelta * velocityMult;
            // Switch to slingshot view immediately before setting POI
            FollowCam.SWITCH_VIEW(FollowCam.eView.slingshot);
            FollowCam.POI = projectile;
            // Add a ProjectileLine to the Projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);
            projectile = null;
            MissionDemolition.SHOT_FIRED();
        }
    }
}
