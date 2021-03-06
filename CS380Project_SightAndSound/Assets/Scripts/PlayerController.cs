﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    MovementController move;
    AStarController A_Star;
    GridController grid;

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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetAxis("Vertical") > 0.0f)
        {
            vect.y += 1.0f;
        }

        float check = Input.GetAxis("Horizontal");

        if (Input.GetAxis("Horizontal") > 0.0f)
        {
            vect.x += 1.0f;
        }

        if (Input.GetAxis("Vertical") < 0.0f)
        {
            vect.y -= 1.0f;
        }

        if (Input.GetAxis("Horizontal") < 0.0f)
        {
            vect.x -= 1.0f;
        }

        vect.Normalize();

        if (Input.GetAxis("Slowdown") == 0.0f)
        {
            move.SetMoveRun();
            //move.SetMoveJog();
        }
        else
        {
            move.SetMoveJog();
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
                    if (A_Star.GetWaypointCount() > 0)
                    {
                        A_Star.ClearWaypoints();
                    }

                    move.SetDirection(vect);
                }
                break;

            case MOVE_TYPE.WAYPOINT:
                {
                    if (A_Star.GetWaypointCount() > 0)
                    {
                        // new Dir is (target pos - curr pos), then normalize
                        Vector3 targetPos = A_Star.GetWaypointFirstValue();
                        Vector3 currPos = gameObject.transform.position;

                        vect = targetPos - currPos;

                        if (vect != Vector3.zero)
                        {
                            vect.Normalize();
                            move.SetDirection(vect);
                        }

                        // if player is near target pos, then take node off of waypoint
                        if (move.GetPosDist(targetPos, currPos) < move.RelativeSpeed())
                        {
                            gameObject.transform.position = targetPos;
                            //move.SetMoveIdle();
                            A_Star.RemoveFirstWaypoint();
                        }
                    }
                    else
                    {
                        move.SetMoveIdle();
                    }
                }
                break;
            default:
                break;
        }
    }

    void Awake()
    {
        move = gameObject.GetComponent<MovementController>();
        A_Star = gameObject.GetComponent<AStarController>();
        grid = GameObject.Find("Grid").GetComponentInParent<GridController>();
    }

    // Use this for initialization
    void Start()
    { }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Vector3 mouse_pos = Input.mousePosition;
            mouse_pos.z = gameObject.transform.localPosition.z;

            if (grid.ScreenWithinGrid(mouse_pos))
            {
                //goal = grid.WorldToGrid(mouse_pos);
                //transform.position = grid.ScreenToWorld(mouse_pos);
                //move.PushWaypointLast(grid.ScreenToWorld(mouse_pos));

                // Call Compute Path
                A_Star.ComputePath(grid.ScreenToWorld(mouse_pos), float.MaxValue, true);

                if (A_Star.GetWaypointCount() > 0)
                {
                    A_Star.RemoveFirstWaypoint();
                }
            }
        }
        else
        {
            if (Input.GetAxis("Pause") == 0.0f)
            {
                if (move.isPaused())
                {
                    move.Resume();
                }

                MovePlayer();
            }
            else
            {
                if (!move.isPaused())
                {
                    move.Pause();
                }
            }
        }
	}
}
