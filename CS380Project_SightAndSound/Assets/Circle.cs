using System;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
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

    [Range(0.01f, 1.0f)]
    public float lineWidth = 0.2f;

    [Range(0.1f, 100.0f)]
    public float radius = 1.0f;

    public bool lineToCenter;

    public FACING face;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        SetupCircle();

    }

    void Update()
    {
        SetupCircle();

    }

    private void SetupCircle()
    {
        lineRenderer.widthMultiplier = lineWidth;

        float deltaTheta = (2.0f * Mathf.PI) / vertaxCount;
        float theta = 0.0f;

        int circle = degree * vertaxCount / 360 + 1;

        if (lineToCenter)
        {
            lineRenderer.positionCount = circle + 2;
            lineRenderer.SetPosition(0, transform.position);

            for (int i = 1; i < lineRenderer.positionCount - 1; i++)
            {
                Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0.0f);
                lineRenderer.SetPosition(i, pos);
                theta += deltaTheta;
            }

            lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position);

        }
        else
        {
            lineRenderer.positionCount = circle;
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0.0f);
                lineRenderer.SetPosition(i, pos);
                theta += deltaTheta;
            }
        }
        



    }


    private void OnDrawGizmos()
    {
        float deltatime = (2.0f * Mathf.PI) / vertaxCount;
        float theta = 0.0f;

        Vector3 oldPos = transform.position;

        int circle = degree * vertaxCount / 360 ;
        for (int i = 0; i < circle + 1; i++)
        {
            Vector3 pos = new Vector3(radius * Mathf.Cos(theta), radius * Mathf.Sin(theta), 0.0f);
            Gizmos.DrawLine(oldPos, transform.position + pos);
            oldPos = transform.position + pos;

            theta += deltatime;
        }

        Gizmos.DrawLine(oldPos, transform.position);

    }


}
