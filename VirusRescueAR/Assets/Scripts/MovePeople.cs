using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePeople : MonoBehaviour
{
	public GameObject person;
	public bool ejeX;

	private float move;
	private bool direction;

    void Start()
    {
        move = 0.0f;
        direction = true;
    }

    void Update()
    {
        if(ejeX == true)
        {
        	if(direction == true )
        	{
        		Vector3 temp = new Vector3(0.001f, 0.0f , 0.0f);
        		person.transform.position += temp;
        		move = move + 0.001f;
        		if(move >= 0.1f)
        			direction = false;
        	}
        	else
        	{
        		Vector3 temp = new Vector3(0.001f, 0.0f , 0.0f);
        		person.transform.position -= temp;
        		move = move - 0.001f;
        		if(move <= 0.0f)
        			direction = true;
        	}
        }
        else
        {
        	if(direction == true )
        	{
        		Vector3 temp = new Vector3(0.0f, 0.0f , 0.001f);
        		person.transform.position += temp;
        		move = move + 0.001f;
        		if(move >= 0.1f)
        			direction = false;
        	}
        	else
        	{
        		Vector3 temp = new Vector3(0.0f, 0.0f , 0.001f);
        		person.transform.position -= temp;
        		move = move - 0.001f;
        		if(move <= 0.0f)
        			direction = true;
        	}
        }
    }
}
