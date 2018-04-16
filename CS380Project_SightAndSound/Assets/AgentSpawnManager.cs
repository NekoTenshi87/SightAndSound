using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AgentSpawnManager : MonoBehaviour {

    public delegate void CreatedAgents();
    public static event CreatedAgents OnCreatedAgents;

    [System.Serializable]
    public class AgentSpawnData
    {
        public Vector3 position;
        public float scale;
        public StateType startState;
        public List<Vector3> patrolSpot = new List<Vector3>();
        public float sweepAngleDegree;
        public int sweepCount;
        public float timePerSweep;
        public float idleTime;
    }

    [System.Serializable]
    public class AgentSpawnDataList
    {
        public List<AgentSpawnData> AgentList = new List<AgentSpawnData>();
    }

    public List<AgentSpawnDataList> MapAgentData = new List<AgentSpawnDataList>();

    void OnMapChanged()
    {
        int currentMap = MapController.CurrentMap;
        if (currentMap < 0 || 
            MapAgentData == null || 
            MapAgentData[currentMap] == null || 
            MapAgentData[currentMap].AgentList.Count == 0)
        {
            return;
        }

        GameObject[] Agents = GameObject.FindGameObjectsWithTag("Agent");
        for (int i = 0; i < Agents.Length; ++i)
        {
            Object.DestroyObject(Agents[i]);
        }

        GameObject origial = GameObject.Find("OriginalAgent");
        if(origial != null)
        { 
            int many = MapAgentData[currentMap].AgentList.Count;
            for (int count = 0; count < many; ++count)
            {
                GameObject agent = Object.Instantiate(origial, MapAgentData[currentMap].AgentList[count].position, Quaternion.identity);
                AgentController agentController = agent.GetComponent<AgentController>();

                agentController.Active = true;
                agentController.patrolSpot = MapAgentData[currentMap].AgentList[count].patrolSpot;
                agentController.startState = MapAgentData[currentMap].AgentList[count].startState;
                agentController.sweepAngleDegree = MapAgentData[currentMap].AgentList[count].sweepAngleDegree;
                agentController.sweepCount = MapAgentData[currentMap].AgentList[count].sweepCount;
                agentController.timePerSweep = MapAgentData[currentMap].AgentList[count].timePerSweep;
                agentController.idleTime = MapAgentData[currentMap].AgentList[count].idleTime;
                agent.transform.position = MapAgentData[currentMap].AgentList[count].position;
                agent.tag = "Agent";
                agent.transform.localScale = new Vector3(MapAgentData[currentMap].AgentList[count].scale, MapAgentData[currentMap].AgentList[count].scale, MapAgentData[currentMap].AgentList[count].scale);
            }
        }

        //if (OnCreatedAgents != null)
        //{
        //    OnCreatedAgents();
        //}
    }

    void OnEnable()
    {
        MapController.OnMapChanged += OnMapChanged;
    }

    void OnDisable()
    {
        MapController.OnMapChanged -= OnMapChanged;
    }

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
       
    }
}

