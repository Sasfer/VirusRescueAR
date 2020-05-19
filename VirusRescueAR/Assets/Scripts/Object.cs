using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public GameObject indicator;
    public GameObject objects;
    public GameObject buttons;
    public GameObject data;

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
            GameObject obj = Instantiate(objects, 
                placementIndicator.transform.position, placementIndicator.transform.rotation);
            indicator.SetActive(false);
            buttons.SetActive(true);
            data.SetActive(true);
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
