using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ControlAR : MonoBehaviour
{
    //AR
    public ARRaycastManager RaycastManager;
    public ARPlaneManager PlaneManager;
    List<ARRaycastHit> HitResult = new List<ARRaycastHit>();
    public GameObject Helicopter;

    void Start()
    {
        
    }

    void Update()
    {
            UpdateAR();

   }

    void UpdateAR()
    {
        Vector2 positionScreenSpace = Camera.current.ViewportToScreenPoint(new Vector2());
        RaycastManager.Raycast(positionScreenSpace, HitResult, TrackableType.PlaneWithinBounds);
        
        if(HitResult.Count > 0)
        {
            if(PlaneManager.GetPlane(HitResult[0].trackableId).alignment == PlaneAlignment.HorizontalUp)
            {
                Pose pose = HitResult[0].pose;
                Helicopter.transform.position = pose.position;
                Helicopter.SetActive(true);
            }
        }
    }
}
