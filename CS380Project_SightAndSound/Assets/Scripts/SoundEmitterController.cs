using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Analytics;

public class SoundEmitterController : MonoBehaviour
{
    public string lookingfor;

    private Circle circle;
    private AStarController Astar;

    private List<GameObject> Targets = new List<GameObject>();

    private void Awake()
    {
        circle = gameObject.GetComponent<Circle>();
        Astar = gameObject.GetComponent<AStarController>();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject target in Targets)
        {
            SoundListener lis = target.GetComponentInChildren<SoundListener>();

            if (lis)
            {
                Vector3 pos = target.transform.position;
                pos.z = 0.0f;

                Astar.ComputePath(pos, circle.radius, true);
                float dis = Astar.GetPathDistance();

                if (dis <= circle.radius)
                {
                    lis.UpdateWaypoints(Astar.GetInvertedWaypoints());
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == lookingfor)
        {
            if (!Targets.Contains(other.gameObject))
            {
                Targets.Add(other.gameObject);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (Targets.Contains(other.gameObject))
        {
            Targets.Remove(other.gameObject);
        }
    }
}
