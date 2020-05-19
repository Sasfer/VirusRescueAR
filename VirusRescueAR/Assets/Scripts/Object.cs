using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class Object : MonoBehaviour
{
    public GameObject indicator;
    public GameObject objects;
    public GameObject buttons;
    public GameObject data;
    public GameObject pause;

    private PlacementIndicator placementIndicator;
    private bool setObject = false;


    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();

    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began && setObject == false)
        {
           //GameObject obj = Instantiate(objects, 
            //    placementIndicator.transform.position, placementIndicator.transform.rotation);

            objects.SetActive(true);
            objects.transform.position = placementIndicator.transform.position;
            objects.transform.rotation = placementIndicator.transform.rotation;
            indicator.SetActive(false);
            pause.SetActive(true);
            buttons.SetActive(true);
            data.SetActive(true);
            /*foreach (var plane in planeManager.trackables)
            {
                plane.gameObject.SetActive(false);
            }*/
            setObject = true;
        }
        
    }   

    public void ControlHide()
    {
        if(setObject == true)
        {
            buttons.SetActive(false);
        }
    } 

    public void ControlShow()
    {
        if(setObject == true)
        {
            buttons.SetActive(true);
        }
    }
}
