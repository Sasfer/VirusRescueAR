using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Object : MonoBehaviour
{
    public GameObject objects;

    private PlacementIndicator placementIndicator;


    void Start()
    {
        placementIndicator = FindObjectOfType<PlacementIndicator>();

    }

    void Update()
    {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began)
        {
            GameObject obj = Instantiate(objects, 
                placementIndicator.transform.position, placementIndicator.transform.rotation);
           
        }
        
    }   
}
