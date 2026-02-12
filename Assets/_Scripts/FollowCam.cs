using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour{
    static public GameObject POI;
    static public FollowCam Instance;

    [Header("Inscribed")]
    public float easing = 0.05f;
    public float castleLookTime = 0.75f;
    public float castleEasing = 0.2f;
    public Transform slingshotFocus;
    public Transform castleFocus;

    public Vector2 minXY = Vector2.zero;

    [Header("Dynamic")]
    public float camZ;
    private float castleFocusUntil;
    private Rigidbody watchedProjectileRB;

    void Awake(){
        Instance = this;
        camZ = this. transform.position.z;
        RefreshFocusTargets();
    }

    void RefreshFocusTargets(){
        if (slingshotFocus == null){
            Slingshot slingshot = FindObjectOfType<Slingshot>();
            if (slingshot != null){
                slingshotFocus = slingshot.transform;
            }
        }

        if (castleFocus == null){
            Goal goal = FindObjectOfType<Goal>();
            if (goal != null){
                castleFocus = goal.transform;
            }
        }
    }

    public static void FocusOnCastleQuick(){
        if (Instance == null) return;
        Instance.castleFocusUntil = Time.time + Instance.castleLookTime;
    }

    public static void FocusOnCastleUntilProjectileStops(GameObject projectile){
        if (Instance == null) return;

        if (projectile == null){
            Instance.watchedProjectileRB = null;
            Instance.castleFocusUntil = 0f;
            return;
        }

        Instance.watchedProjectileRB = projectile.GetComponent<Rigidbody>();
        Instance.castleFocusUntil = 0f;
    }

    public static void FocusOnSlingshot(){
        if (Instance == null) return;
        Instance.watchedProjectileRB = null;
        Instance.castleFocusUntil = 0f;
    }

    void FixedUpdate(){
        RefreshFocusTargets();

        Vector3 destination = Vector3.zero;
        bool focusingCastle = Time.time < castleFocusUntil;

        if (watchedProjectileRB != null){
            if (watchedProjectileRB.IsSleeping()){
                watchedProjectileRB = null;
            } else {
                focusingCastle = true;
            }
        }

        if(POI!= null){

            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping()){
                POI = null;
            }
        }

        if (focusingCastle && castleFocus != null){
            destination = castleFocus.position;
        } else if(POI != null){
            destination = POI.transform.position;
        } else if (slingshotFocus != null){
            destination = slingshotFocus.position;
        }
    //    if(POI == null) return;

    //    Vector3 destination = POI.transform.position;
    
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        float currentEasing = focusingCastle ? castleEasing : easing;
        destination = Vector3.Lerp(transform.position, destination, currentEasing);

        destination.z = camZ;

        transform.position = destination;

        Camera.main.orthographicSize = destination.y + 10;
    }
}
