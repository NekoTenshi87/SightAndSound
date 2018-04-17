using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightEmitterController : MonoBehaviour
{
    
    public string lookingfor;

    private Circle circle;
    private MovementController movementController;
    private AStarController Astar;
    private GridController grid;
    private SightListener lis;

    private GameObject Target = null;

    private void Awake()
    {
        circle = gameObject.GetComponent<Circle>();
        movementController = gameObject.GetComponentInParent<MovementController>();
        Astar = gameObject.GetComponent<AStarController>();
        grid = GameObject.Find("Grid").GetComponent<GridController>();
        lis = gameObject.transform.parent.GetComponentInChildren<SightListener>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!movementController.isPaused())
        {
            if (Target)
            {
                Vector2Int pos_grid = grid.GetRowColumn(gameObject.transform.position);
                Vector2Int tpos_grid = grid.GetRowColumn(Target.transform.position);

                if (grid.IsClearPath(pos_grid, tpos_grid, Target.transform.lossyScale.x / 2.0f))
                {
                    Vector3 pos = Target.transform.position;
                    pos.z = 0.0f;

                    Astar.ComputePath(pos, float.MaxValue, true);

                    if (lis)
                    {
                        lis.UpdateWaypoints(Astar.GetWaypoints());
                    }
                }
            }
        }
    }
 
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == lookingfor)
        {
            if (circle.degree != 360)
            {
                if (CheckInBetween(other.gameObject))
                {
                    if (Target == null)
                    {
                        Target = other.gameObject;
                    }
                }
                else
                {
                    Target = null;
                }
            }
            else
            {
                if (Target == null)
                {
                    Target = other.gameObject;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == Target)
        {
            Target = null;
        }
    }


    bool CheckInBetween(GameObject other)
    {
        Vector3 vec = other.transform.position - gameObject.transform.position;
        float thetaOther = Mathf.Atan2(vec.y, vec.x);

        Vector3 v1;

        v1 = movementController.direction;

        float thetaL, thetaR;
        thetaL = thetaR = Mathf.Atan2(v1.y, v1.x);
        thetaR -= ((float)circle.degree / 2.0f) * Mathf.Deg2Rad;
        thetaL += ((float)circle.degree / 2.0f) * Mathf.Deg2Rad;

        if (thetaOther < 0.0f)
        {
            thetaOther += 2.0f * Mathf.PI;
        }

        if (thetaL < 0.0f)
        {
            thetaL += 2.0f * Mathf.PI;
        }

        if (thetaR < 0.0f)
        {
            thetaR += 2.0f * Mathf.PI;
        }

        if (thetaR < thetaL)
        {
            if (CheckBetweenAngle(thetaR, thetaL, thetaOther))
                return true;
        }
        else
        {
            if (CheckBetweenAngle(0.0f, thetaL, thetaOther) || CheckBetweenAngle(thetaR, 2.0f * Mathf.PI, thetaOther))
                return true;
        }

        return false;
    }


    bool CheckBetweenAngle(float min, float max, float obj)
    {
        if (min <= obj && max >= obj)
            return true;

        return false;
    }
}
