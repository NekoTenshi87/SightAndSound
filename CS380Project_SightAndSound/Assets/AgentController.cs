﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StateType { ePatrol = 0, eSweep = 1, eIdle = 2, eCount = 3, eMove = 4 };

// it is going to work as a state machine
public class AgentController : MonoBehaviour
{
    public bool Active;
    // variables for translating states
    public StateType startState;
    private StateType currentState;

    private MovementController move;
    private AStarController Astar;
    private SoundListener sound;

    private GridController grid;
    private AgentSpawnManager spawn;

    //variables for patrol state
    public List<Vector3> patrolSpot = new List<Vector3>();
    private int currentSpot = 0;

    //variables for idle/sweeping state
    public float sweepAngleDegree;  // the degree amount of sweeping left and right from view direction  
    public int sweepCount;   // number of sweeping per idel
    public float timePerSweep; // time took by one sweeping
    private int NbSweeped;
    private float viewAngle;

    public float idleTime;

    private float timer;
    
    void ValidateInputs()
    {
        // validate the spots in the valid region
        GridController grid = GameObject.Find("Grid").GetComponentInParent<GridController>();
        MapController map = GameObject.Find("Tilemap").GetComponentInParent<MapController>();
        int[] invalidSpots = new int[patrolSpot.Count];
        int invalidCount = 0;
        for (int current = 0; current < patrolSpot.Count; ++current)
        {
            Vector2Int coord = grid.GetRowColumn(patrolSpot[current]);
            if (map.IsWall(coord.y, coord.x))
            {
                invalidSpots[invalidCount] = current;
                ++invalidCount;
            }
        }
        for (int count = 0; count < invalidCount; ++count)
        {
            patrolSpot.RemoveAt(invalidSpots[count]);
        }

        // clamp the invalid degree range
        if (sweepAngleDegree < 0)
        {
            sweepAngleDegree = 0;
            Debug.Log("AgentController Warning: invaild sweep degree [0, 360]");
        }
        else if (sweepAngleDegree > 360)
        {
            sweepAngleDegree = 360;
            Debug.Log("AgentController Warning: invaild sweep degree [0, 360]");
        }
        if (timePerSweep <= 0)
        {
            timePerSweep = 1;
            Debug.Log("AgentController Warning: invaild time range time should > 0");
        }

        if (idleTime <= 0)
        {
            idleTime = 0.2f;
            Debug.Log("AgentController Warning: invaild time range time should > 0");
        }
    }

    void Awake()
    {
        Astar = gameObject.GetComponent<AStarController>();
        move = gameObject.GetComponent<MovementController>();
        sound = gameObject.GetComponent<SoundListener>();
        grid = GameObject.Find("Grid").GetComponent<GridController>();
        spawn = GameObject.Find("AgentSpawnHelper").GetComponent<AgentSpawnManager>();
    }

    public void OnMapChanged()
    {
        ValidateInputs();
        SetNextState(currentState);
    }

    [ExecuteInEditMode]
    public void Resize(float size)
    {
        Vector3 scale_vect = Vector3.one;
        scale_vect *= size * 0.8f;

        gameObject.transform.localScale = scale_vect;
    }

    void OnEnable()
    {
        //spawn.OnCreatedAgents += OnMapChanged;
        grid.OnResized += Resize;
    }

    void OnDisable()
    {
        //spawn.OnCreatedAgents -= OnMapChanged;
        grid.OnResized -= Resize;
    }

    // Use this for initialization
    void Start()
    {
        if(patrolSpot.Count <= 0)
        {
            patrolSpot.Add( new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), gameObject.transform.localPosition.z));
            patrolSpot.Add( new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), gameObject.transform.localPosition.z));
            patrolSpot.Add( new Vector3(Random.Range(0.1f, 0.9f), Random.Range(0.1f, 0.9f), gameObject.transform.localPosition.z));
        }
 
        ValidateInputs();
        SetNextState(startState);
    }

    // Update is called once per frame
    void Update()
    {
        if(Active)
        { 
            switch (currentState)
            {
                case StateType.ePatrol:
                    UpdatePatrol();
                    break;
                case StateType.eSweep:
                    UpdateSweep();
                    break;
                case StateType.eIdle:
                    UpdateIdle();
                    break;
                case StateType.eMove:
                    UpdateMove();
                    break;
            }
            Debug.DrawLine(gameObject.transform.position, gameObject.transform.position + move.direction * 0.1f, Color.red);
        }
    }

    public void SetNextState(StateType nextState)
    {
        currentState = nextState;

        switch (currentState)
        {
            case StateType.ePatrol:
                InitPatrol();
                break;
            case StateType.eSweep:
                InitSweep();
                break;
            case StateType.eIdle:
                InitIdle();
                break;
            case StateType.eMove:
                InitMove();
                break;
        }
    }

    // states implementation
    void InitPatrol()
    {
        if (patrolSpot.Count != 0)
        {
            if (patrolSpot.Count <= currentSpot)
            {
                currentSpot = currentSpot % patrolSpot.Count;
            }

            Astar.ComputePath(patrolSpot[currentSpot], float.MaxValue, true);

            if (Astar.GetWaypointCount() > 0)
            {
                Astar.RemoveFirstWaypoint();
            }

            ++currentSpot;
        }

        move.SetMoveJog();
    }

    void UpdatePatrol()
    {
        MoveAgent();
    }

    void InitSweep()
    {
        timer = 0;
        NbSweeped = 0;
        viewAngle = Vector3.Angle(move.direction, new Vector3(1, 0, 0));
        if (move.direction.y < 0)
            viewAngle = Mathf.PI * 2 - viewAngle;
        move.SetMoveIdle();
    }

    void UpdateSweep()
    {
        if (timer < timePerSweep)
        {
            timer += Time.deltaTime;

            float degree = Mathf.Sin(Mathf.PI * 2 / timePerSweep * timer) * sweepAngleDegree;
            Quaternion rotate = Quaternion.AngleAxis(degree + viewAngle, new Vector3(0, 0, 1));
            move.SetDirection(rotate * new Vector3(1, 0, 0));

            if (timer >= timePerSweep)
            {
                ++NbSweeped;
                if (NbSweeped < sweepCount)
                {
                    timer = 0;
                }
                else
                {
                    SetNextState(StateType.ePatrol);
                }
            }
        }
    }

    void InitIdle()
    {
        timer = 0;
        move.SetMoveJog();
    }

    void UpdateIdle()
    {
        if (timer < timePerSweep)
        {
            timer += Time.deltaTime;
            
            if (timer >= timePerSweep)
            {
                SetNextState(StateType.ePatrol);
            }
        }
    }

    void InitMove()
    {
        move.SetMoveRun();
    }
    
    void UpdateMove()
    {
        MoveAgent();
    }

    void MoveAgent()
    {
        // walking to the current target spot
        if (Astar.GetWaypointCount() > 0)
        {
            Vector3 nextPos = Astar.GetWaypointFirstValue();
            Vector3 currentPos = gameObject.transform.position;

            Vector3 movement = nextPos - currentPos;

            //bool isZero = movement == Vector3.zero;

            if (movement != Vector3.zero)
            {
                movement.Normalize();
                move.SetDirection(movement);
            }

            // if owner is near target pos, then take node off of waypoint
            if (move.GetPosDist(nextPos, currentPos) < move.RelativeSpeed())
            {
                gameObject.transform.position = nextPos;
                Astar.RemoveFirstWaypoint();
            }
        }
        else
        {
            SetNextState(StateType.eSweep);
        }
    }

    public StateType GetState()
    {
        return currentState;
    }
}
