using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(CircleCollider2D))]
public class Circle : MonoBehaviour
{
    public enum FACING
    {
        FRONT,
        BACK
    }
    [Range(1, 40)]
    public int vertaxCount = 40;

    [Range(0, 360)]
    public int degree = 360;

    [Range(0.0f, 0.1f)]
    public float lineWidth = 0.01f;

    [Range(0.0f, 20.0f)]
    public float idle_radius = 1.0f;

    [Range(0.0f, 20.0f)]
    public float walk_radius = 1.0f;
    [Range(0.0f, 20.0f)]
    public float run_radius = 5.0f;

    public float radius = 0.0f;

    private float radius_unit = 0.0f;

    //private bool lineToCenter;

    //public FACING face;


    private LineRenderer lineRenderer;
    private CircleCollider2D circleCollider2D;
    private GridController grid;
    private MovementController movementController;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        grid = GameObject.Find("Grid").GetComponent<GridController>();
        circleCollider2D = GetComponent<CircleCollider2D>();
        movementController = gameObject.GetComponentInParent<MovementController>();

        circleCollider2D.isTrigger = true;

        SetupCircle();

    }

    void Start()
    {
        lineRenderer.material = new Material(Shader.Find("Particles/Multiply"));
    }

    void Update()
    {
        if (movementController.MoveType == MovementController.MOVEMENT_TYPE.M_IDLE)
        {
            radius = idle_radius;
        }
        else if (movementController.MoveType == MovementController.MOVEMENT_TYPE.M_JOG)
        {
            radius = walk_radius;
        }
        else if (movementController.MoveType == MovementController.MOVEMENT_TYPE.M_RUN)
        {
            radius = run_radius;
        }


        SetupCircle();
        SetupCircleCollider();
    }

    private void SetupCircle()
    {

        

        radius_unit = radius / grid.NumberOfCells;

        lineRenderer.widthMultiplier = lineWidth;

        float deltaTheta = (2.0f * Mathf.PI) / vertaxCount;

        Vector3 v1;

        v1 = movementController.direction;

        float theta = Mathf.Atan2(v1.y, v1.x);
        theta -= ((float)degree / 2.0f) * Mathf.PI / 180.0f;

        int circle = degree * vertaxCount / 360 + 1;

        if (degree != 360)
        {
            lineRenderer.positionCount = circle + 2;
            lineRenderer.SetPosition(0, transform.position);

            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                Vector3 pos = new Vector3(radius_unit * Mathf.Cos(theta), radius_unit * Mathf.Sin(theta), 0.0f);
                lineRenderer.SetPosition(i, transform.position + pos);
                theta += deltaTheta;
            }

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);

        }
        else
        {
            lineRenderer.positionCount = circle;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 pos = new Vector3(radius_unit * Mathf.Cos(theta), radius_unit * Mathf.Sin(theta), 0.0f);
                lineRenderer.SetPosition(i, transform.position + pos);
                theta += deltaTheta;
            }
        }




    }

    private void SetupCircleCollider()
    {
        circleCollider2D.radius = radius * 1.25f;
    }


    private void OnDrawGizmos()
    {
        grid = GameObject.Find("Grid").GetComponent<GridController>();
        movementController = gameObject.GetComponentInParent<MovementController>();


        radius_unit = radius / grid.NumberOfCells;

        float deltatime = (2.0f * Mathf.PI) / vertaxCount;
        Vector3 v1;
        v1 = movementController.direction;
        float theta = Mathf.Atan2(v1.y, v1.x);
        theta -= ((float)degree / 2.0f) * Mathf.PI / 180.0f;

        Vector3 oldPos = transform.position;

        int circle = degree * vertaxCount / 360;
        for (int i = 0; i < circle + 1; i++)
        {
            Vector3 pos = new Vector3(radius_unit * Mathf.Cos(theta), radius_unit * Mathf.Sin(theta), 0.0f);
            Gizmos.DrawLine(oldPos, transform.position + pos);
            oldPos = transform.position + pos;

            theta += deltatime;
        }

        Gizmos.DrawLine(oldPos, transform.position);

    }

    

}
