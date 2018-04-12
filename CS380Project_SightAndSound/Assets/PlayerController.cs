using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    enum MOVE_TYPE
    {
        DIRECTIONAL,
        WAYPOINT
    };

    MOVE_TYPE move_type = MOVE_TYPE.WAYPOINT;

    [ExecuteInEditMode]
    public void ResizePlayer(float size)
    {
        Vector3 scale_vect = Vector3.one;
        scale_vect *= size * 0.8f;

        transform.localScale = scale_vect;
    }

    private void MovePlayer ()
    { 
        Vector3 vect = Vector3.zero;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            vect.y += 1.0f;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            vect.x += 1.0f;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            vect.y -= 1.0f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            vect.x -= 1.0f;
        }

        vect.Normalize();

        MovementController mov = gameObject.GetComponent<MovementController>();

        if (Input.GetKey(KeyCode.Space))
        {
            mov.SetMoveJog();
        }
        else
        {
            mov.SetMoveRun();
        }

        if (vect != Vector3.zero)
        {
            move_type = MOVE_TYPE.DIRECTIONAL;
        }
        else
        {
            move_type = MOVE_TYPE.WAYPOINT;
        }

        switch (move_type)
        {
            case MOVE_TYPE.DIRECTIONAL:
                {
                    if (mov.GetWaypointCount() > 0)
                    {
                        mov.ClearWaypoints();
                    }

                    mov.SetDirection(vect);
                }
                break;

            case MOVE_TYPE.WAYPOINT:
                {
                    if (mov.GetWaypointCount() > 0)
                    {
                        // new Dir is (target pos - curr pos), then normalize
                        Vector3 targetPos = mov.GetWaypointFirstValue();
                        Vector3 currPos = gameObject.transform.position;

                        vect = targetPos - currPos;

                        if (vect != Vector3.zero)
                        {
                            vect.Normalize();
                            mov.SetDirection(vect);
                        }

                        // if player is near target pos, then take node off of waypoint
                        if (mov.GetPosDist(targetPos, currPos) < mov.RelativeSpeed())
                        {
                            gameObject.transform.position = targetPos;
                            mov.SetMoveIdle();
                            mov.RemoveFirstWaypoint();
                        }
                    }
                    else
                    {
                        mov.SetMoveIdle();
                    }
                }
                break;
            default:
                break;
        }
    }

    // Use this for initialization
    void Start()
    { }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            GridController grid = GameObject.Find("Grid").GetComponentInParent<GridController>();
            Vector3 mouse_pos = Input.mousePosition;

            if (grid.ScreenWithinGrid(mouse_pos))
            {
                //goal = grid.WorldToGrid(mouse_pos);
                transform.position = grid.ScreenToWorld(mouse_pos);
            }
        }
        else
        {
          MovePlayer();
        }
	}
}
