using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour{

    [Header("Inscribed")]
    public GameObject projectilePrefab;
    public float velocityMult = 10f;
    public GameObject projLinePrefab;
    public float aimLineLength = 5f;

    [Header("Dynamic")]
    public GameObject launchPoint;
    public Vector3 launchPos;
    public GameObject projectile;
    public bool aimingMode;
    public LineRenderer aimLine;

    void Awake(){
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;

        GameObject aimLineObj = new GameObject("AimLine");
        aimLineObj.transform.SetParent(transform, false);
        aimLine = aimLineObj.AddComponent<LineRenderer>();
        aimLine.positionCount = 2;
        aimLine.startWidth = 0.08f;
        aimLine.endWidth = 0.08f;
        aimLine.material = new Material(Shader.Find("Sprites/Default"));
        aimLine.startColor = Color.red;
        aimLine.endColor = Color.red;
        aimLine.enabled = false;
    }
 
 void OnMouseEnter(){
    //print("Slingshot:OnMouseEnter()");
    launchPoint.SetActive(true);
 }

 void OnMouseExit(){
    //print("Slingshot:OnMouseExit()");
    launchPoint.SetActive(false);
 }

 void OnMouseDown(){

    aimingMode = true;

    projectile = Instantiate(projectilePrefab) as GameObject;

    projectile.transform.position = launchPos;

    projectile.GetComponent<Rigidbody>().isKinematic = true;
    aimLine.enabled = true;
 }
 void Update(){

    if (!aimingMode) return;

    Vector3 mousePos2D = Input.mousePosition;
    mousePos2D.z = -Camera.main.transform.position.z;
    Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

    Vector3 mouseDelta = mousePos3D -launchPos;

    float maxMagnitude = this.GetComponent<SphereCollider>().radius;
    if (mouseDelta.magnitude > maxMagnitude){
        mouseDelta.Normalize();
        mouseDelta*= maxMagnitude;
    }

    Vector3 projPos = launchPos + mouseDelta;
    projectile.transform.position = projPos;
    UpdateAimLine(projPos, mouseDelta);

    if(Input.GetMouseButtonUp(0)){

        aimingMode = false;
        aimLine.enabled = false;
        Rigidbody projRB = projectile.GetComponent<Rigidbody>();
        projRB.isKinematic = false;
        projRB.collisionDetectionMode = CollisionDetectionMode.Continuous;
        projRB.linearVelocity = -mouseDelta * velocityMult;
        FollowCam.FocusOnCastleUntilProjectileStops(projectile);
        Instantiate<GameObject>(projLinePrefab, projectile.transform);
        projectile = null;
    }
 }

 void UpdateAimLine(Vector3 projPos, Vector3 mouseDelta){
    if (aimLine == null) return;

    Vector3 launchDir = (-mouseDelta).normalized;
    Vector3 endPos = projPos + (launchDir * aimLineLength);

    aimLine.SetPosition(0, projPos);
    aimLine.SetPosition(1, endPos);
 }
}
