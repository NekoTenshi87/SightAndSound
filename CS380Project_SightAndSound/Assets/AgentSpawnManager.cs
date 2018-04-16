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
        if (Agents.Length > MapAgentData[currentMap].AgentList.Count)
        {
            int removeFrom = MapAgentData[currentMap].AgentList.Count;
            for (int i = removeFrom; i < Agents.Length; ++i)
            {
                Object.Destroy(Agents[i]);
            }
        }
        else if (Agents.Length < MapAgentData[currentMap].AgentList.Count)
        {
            GameObject original;
            if (Agents.Length != 0)
            {
                original = Agents[0];
            }
            else
            {
                original = Resources.Load<GameObject>("/Prefab/Enemy");
            }
            if (original == null)
            {
                Debug.LogWarning("AgentSpawnManager Fail to instantiate agent!");
            }

            int many = MapAgentData[currentMap].AgentList.Count - Agents.Length;
            for (int count = 0; count < many; ++count)
            {
                GameObject agent = Object.Instantiate(original, MapAgentData[currentMap].AgentList[count].position, Quaternion.identity);
                AgentController agentController = agent.GetComponent<AgentController>();
                agentController.patrolSpot = MapAgentData[currentMap].AgentList[count].patrolSpot;
                agentController.startState = MapAgentData[currentMap].AgentList[count].startState;
                agentController.sweepAngleDegree = MapAgentData[currentMap].AgentList[count].sweepAngleDegree;
                agentController.sweepCount = MapAgentData[currentMap].AgentList[count].sweepCount;
                agentController.timePerSweep = MapAgentData[currentMap].AgentList[count].timePerSweep;
                agentController.idleTime = MapAgentData[currentMap].AgentList[count].idleTime;
                Agents[count].transform.position = MapAgentData[currentMap].AgentList[count].position;
                Agents[count].transform.localScale = new Vector3(MapAgentData[currentMap].AgentList[count].scale, MapAgentData[currentMap].AgentList[count].scale, MapAgentData[currentMap].AgentList[count].scale);
            }
        }

        for (int count = 0; count < Agents.Length; ++count)
        {
            AgentController agentController = Agents[count].GetComponent<AgentController>();
            agentController.patrolSpot = MapAgentData[currentMap].AgentList[count].patrolSpot;
            agentController.startState = MapAgentData[currentMap].AgentList[count].startState;
            agentController.sweepAngleDegree = MapAgentData[currentMap].AgentList[count].sweepAngleDegree;
            agentController.sweepCount = MapAgentData[currentMap].AgentList[count].sweepCount;
            agentController.timePerSweep = MapAgentData[currentMap].AgentList[count].timePerSweep;
            agentController.idleTime = MapAgentData[currentMap].AgentList[count].idleTime;
            Agents[count].transform.position = MapAgentData[currentMap].AgentList[count].position;
            Agents[count].transform.localScale = new Vector3(MapAgentData[currentMap].AgentList[count].scale, MapAgentData[currentMap].AgentList[count].scale, MapAgentData[currentMap].AgentList[count].scale);
        }

        if (OnCreatedAgents != null)
        {
            OnCreatedAgents();
        }
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

