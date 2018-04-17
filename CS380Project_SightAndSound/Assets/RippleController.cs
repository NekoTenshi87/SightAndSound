using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RippleController : MonoBehaviour
{
    private Circle m_circle;
    private ParticleSystem particle;

    private MovementController movementController;

    private void Awake()
    {
        m_circle = gameObject.GetComponentInParent<Circle>();
        particle = gameObject.GetComponent<ParticleSystem>();

        movementController = gameObject.GetComponentInParent<MovementController>();
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {

        particle.enableEmission = true;


        if (movementController.MoveType == MovementController.MOVEMENT_TYPE.M_IDLE)
        {
            particle.enableEmission = false;


        }
        else
        {
            particle.enableEmission = true;


        }

        gameObject.transform.localScale = new Vector3(m_circle.radius / 11.0f, 0.0f, m_circle.radius / 11.0f);
    }
}
