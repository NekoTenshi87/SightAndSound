using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SoundEmitterController : MonoBehaviour
{

    public string lookingfor;


    private MovementController movementController;
    private Circle circle;
    private AStarController Astar;

    private bool Is_inside;

    private void Awake()
    {
        movementController = gameObject.GetComponentInParent<MovementController>();
        circle = gameObject.GetComponent<Circle>();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Is_inside)
        {
            
        }
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        Is_inside = true;
    }

    //void OnTriggerStay2D(Collider2D other)
    //{


    //    if (other.gameObject.tag == lookingfor)
    //    {

    //        Is_inside = true;

    //        if (circle.degree != 360)
    //        {
    //            Is_inside = CheckInBetween(other.gameObject);

    //        }
    //    }
    //}

    void OnTriggerExit2D(Collider2D other)
    {
        Is_inside = false;
    }


    bool CheckInBetween(GameObject other)
    {
        Vector3 vec = other.transform.position - gameObject.transform.position;
        float thetaOther = Mathf.Atan2(vec.y, vec.x);

        Vector3 v1;

        v1 = movementController.direction;

        float thetaL, thetaR;
        thetaL = thetaR = Mathf.Atan2(v1.y, v1.x);
        thetaR -= ((float)circle.degree / 2.0f) * Mathf.PI / 180.0f;
        thetaL += ((float)circle.degree / 2.0f) * Mathf.PI / 180.0f;

        if (thetaR < 360 * Mathf.Deg2Rad && thetaL >= 0)
        {
            if (!CheckBetweenAngle(thetaR, thetaL, thetaOther))
                return false;
        }
        else if (thetaL > 360 * Mathf.Deg2Rad)
        {
            if (!CheckBetweenAngle(0, thetaL - 360, thetaOther) || !CheckBetweenAngle(thetaR, 360, thetaOther))
                return false;
        }
        else if (thetaR < 0)
        {
            if (!CheckBetweenAngle(0, thetaL, thetaOther) || !CheckBetweenAngle(360 + thetaR, 360, thetaOther))
                return false;
        }


        return true;
    }


    bool CheckBetweenAngle(float min, float max, float obj)
    {
        if (min <= obj && max >= obj)
            return true;

        return false;
    }
}
