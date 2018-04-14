using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float RunSpeed = 8.0f;
    public float JogSpeed = 4.0f;

    float speed;

    Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);

    LinkedList<Vector3> waypoints = new LinkedList<Vector3>();

    // Use this for initialization
    void Start()
    {
        SetMoveIdle();
        //waypoints.AddLast(new Vector3(0.9f, 0.9f, 0.0f));
        //waypoints.AddLast(new Vector3(0.5f, 0.5f, 0.0f));
        //waypoints.AddLast(new Vector3(0.9f, 0.1f, 0.0f));
        //waypoints.AddLast(new Vector3(0.5f, 0.5f, 0.0f));
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (speed > 0.0f)
        {
            Vector3 vect = direction * RelativeSpeed();

            transform.Translate(vect);
        }

        float radius = transform.lossyScale.x / 2.0f;

        // Clamp Player to map
        if (transform.position.x < radius)
        {
            transform.Translate(new Vector3(radius - transform.position.x, 0.0f, 0.0f));
        }
        else if (transform.position.x > 1.0f - radius)
        {
            transform.Translate(new Vector3(1.0f - radius - transform.position.x, 0.0f, 0.0f));
        }

        if (transform.position.y < radius)
        {
            transform.Translate(new Vector3(0.0f, radius - transform.position.y, 0.0f));
        }
        else if (transform.position.y > 1.0f - radius)
        {
            transform.Translate(new Vector3(0.0f, 1.0f - radius - transform.position.y, 0.0f));
        }
    }

    public float RelativeSpeed()
    {
        return speed * Time.deltaTime * transform.localScale.x;
    }

    public int GetWaypointCount()
    {
        return waypoints.Count;
    }

    public Vector3 GetWaypointFirstValue()
    {
        return waypoints.First.Value;
    }

    public void PushWaypointLast(Vector3 pos)
    {
        waypoints.AddLast(pos);
    }

    public void PushWaypointFirst(Vector3 pos)
    {
        waypoints.AddFirst(pos);
    }

    public void RemoveFirstWaypoint()
    {
        if (waypoints.Count > 0)
        {
            waypoints.RemoveFirst();
        }
    }

    public void ClearWaypoints()
    {
        waypoints.Clear();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void SetMoveRun()
    {
        speed = RunSpeed;
    }

    public void SetMoveJog()
    {
        speed = JogSpeed;
    }

    public void SetMoveIdle()
    {
        if (speed != 0.0f)
        {
            speed = 0.0f;
        }
    }

    public float GetPosDist(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
    }

    public bool ComputePath(Vector3 goal_pos, float max_dist, bool newRequest)
    {
        AStarController A_Star = gameObject.GetComponent<AStarController>();
        GridController grid = GameObject.Find("Grid").GetComponent<GridController>();

        if (newRequest)
        {
            A_Star.ValidateSearchSpaceSize();

            Vector2Int goal_grid = grid.GetRowColumn(goal_pos);

            Vector2Int start_grid = grid.GetRowColumn(gameObject.transform.position);

            waypoints.Clear();

            if (goal_grid.x == start_grid.x && goal_grid.y == start_grid.y)
            {
                waypoints.AddLast(grid.GetCoordinates(goal_grid.y, goal_grid.x));
                A_Star.SetPathDistance(0.0f);
                return true;
            }

            if (grid.IsClearPath(start_grid, goal_grid, gameObject.transform.lossyScale.x / 2.0f))
            {
                Vector2Int diff = goal_grid - start_grid;

                float dist = Mathf.Sqrt((float)(diff.x * diff.x + diff.y * diff.y));

                if (dist > max_dist)
                {
                    waypoints.AddLast(gameObject.transform.position);
                    A_Star.SetPathDistance(0.0f);
                }
                else
                {
                    waypoints.AddLast(goal_pos);
                    A_Star.SetPathDistance(dist);
                }

                return true;
            }


            // Clear List
            A_Star.Clear();

            //Set Variables
            A_Star.SetStart(start_grid);
            A_Star.SetGoal(goal_grid);

            AStarController.NodeData node = new AStarController.NodeData();

            node.cost = A_Star.CalcHeuristic(start_grid, goal_grid);

            A_Star.Push(start_grid, node);
        }

        while (!A_Star.Empty())
        {
            if (!A_Star.Update(max_dist))
            {
                return true;
            }
        }

        Vector2Int pos = grid.GetRowColumn(gameObject.transform.position);
        waypoints.AddLast(grid.GetCoordinates(pos.y, pos.x));
        A_Star.SetPathDistance(0.0f);

        return true;
    }

}
