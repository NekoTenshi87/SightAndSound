using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightListener : MonoBehaviour
{
    private AStarController Astar;
    private AgentController agent;
    private LineRenderer rend;

    bool drawLine = false;

    // Use this for initialization
    void Awake ()
    {
        Astar = gameObject.GetComponentInParent<AStarController>();
        agent = gameObject.GetComponentInParent<AgentController>();
        rend = gameObject.GetComponent<LineRenderer>();
    }

    void Start()
    {
        rend.material = new Material(Shader.Find("Particles/Multiply"));
    }

    // Update is called once per frame
    void Update ()
    {
        if (agent.GetState() == StateType.eMove)
        {
            if (drawLine)
            {
                if (Astar.GetWaypointCount() > 0)
                {
                    rend.enabled = true;
                    Astar.DrawPath(rend);
                }
                else
                {
                    rend.enabled = false;
                }
            }
        }
        else
        {
            rend.enabled = false;
            drawLine = false;
        }
	}

    public void UpdateWaypoints(LinkedList<Vector3> wp)
    {
        Astar.SetWaypoints(wp);

        if (Astar.GetWaypointCount() > 0)
        {
            Astar.RemoveFirstWaypoint();
        }

        drawLine = true;

        agent.SetNextState(StateType.eMove);
    }
}
