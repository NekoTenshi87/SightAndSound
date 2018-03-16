using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float RunSpeed = 8.0f;
    public float JogSpeed = 4.0f;

    Vector3 prev_pos;

    float speed;

    // Use this for initialization
    void Start()
    {
        speed = RunSpeed;
    }

    [ExecuteInEditMode]
    public void ResizePlayer(float size)
    {
        Vector3 scale_vect = Vector3.one;
        scale_vect *= size * 0.8f;

        transform.localScale = scale_vect;
    }

    private void MovePlayer ()
    {
        prev_pos = transform.position;

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
        vect *= speed * Time.deltaTime * transform.localScale.x;

        transform.Translate(vect);

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

    // Update is called once per frame
    void Update ()
    {
        MovePlayer();
	}
}
