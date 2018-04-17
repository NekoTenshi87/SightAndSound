using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public enum MOVEMENT_TYPE
    {
        M_IDLE,
        M_JOG,
        M_RUN
    };

    public float RunSpeed = 8.0f;
    public float JogSpeed = 4.0f;

    float speed;

    public Vector3 direction = new Vector3(0.0f, 1.0f, 0.0f);

    public MOVEMENT_TYPE MoveType = MOVEMENT_TYPE.M_IDLE;

    // Use this for initialization
    void Start()
    {
        SetMoveIdle();
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
  
    public void SetDirection(Vector3 dir)
    {
        direction = dir;
    }

    public void SetMoveRun()
    {
        MoveType = MOVEMENT_TYPE.M_RUN;
        speed = RunSpeed;
    }

    public void SetMoveJog()
    {
        MoveType = MOVEMENT_TYPE.M_JOG;
        speed = JogSpeed;
    }

    public void SetMoveIdle()
    {
        MoveType = MOVEMENT_TYPE.M_IDLE;
        if (speed != 0.0f)
        {
            speed = 0.0f;
        }
    }

    public float GetPosDist(Vector3 a, Vector3 b)
    {
        return Mathf.Sqrt((b.x - a.x) * (b.x - a.x) + (b.y - a.y) * (b.y - a.y));
    }
}
