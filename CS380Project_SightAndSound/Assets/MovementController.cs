using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public float RunSpeed = 8.0f;
    public float JogSpeed = 4.0f;

    float speed;
    Vector3 goal;

    Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);

    LinkedList<Vector3> waypoints = new LinkedList<Vector3>();

    float distance_sq = 0.0f;

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

    public float GetPathDistance()
    {
        return Mathf.Sqrt(distance_sq);
    }

    public float GetPathDistanceSq()
    {
        return distance_sq;
    }

    public float GetPosDist(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
    }

    public bool ComputePath(Vector2Int grid_pos, bool newRequest)
    {
        AStarController A_Star = gameObject.GetComponent<AStarController>();
        GridController grid = GameObject.Find("Grid").GetComponent<GridController>();

        if (newRequest)
        {
            A_Star.ValidateSearchSpaceSize();

            goal = grid.GetCoordinates(grid_pos);

            Vector3 pos = gameObject.transform.position;

            Vector2Int start = grid.GetRowColumn(pos);

            waypoints.Clear();

            if (grid_pos.x == start.x && grid_pos.y == start.y)
            {
                distance_sq = 0.0f;
                return true;
            }


        }










        return true;
    }

}
