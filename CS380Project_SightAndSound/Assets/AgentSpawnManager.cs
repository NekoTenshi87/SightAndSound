using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class AgentSpawnManager : MonoBehaviour {

    [System.Serializable]
    private class AgentsWithMapData
    {
        public List<float> agentsPosX = new List<float>();
        public List<float> agentsPosY = new List<float>();
        public List<float> agentsPosZ = new List<float>();
        public List<AgentConfig> agents = new List<AgentConfig>();
    }

    public GameObject agentPrefab = new GameObject();

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
       
        if (Input.GetKeyDown(KeyCode.G))
        {
            Save();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateAgentsFromFile();
        }
    }

    void Save()
    {
        int currentMapIndex = MapController.CurrentMap;    
        GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");

        if (agents.Length != 0)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.dataPath + "/AgentsInMap" + currentMapIndex + ".agents", FileMode.OpenOrCreate);
           
            //Setup data    
            AgentsWithMapData data = new AgentsWithMapData();
            for (int count = 0; count < agents.Length; ++count)
            {
                AgentController acom = agents[count].GetComponent<AgentController>();
                if(acom != null)
                {
                    Vector3 pos = agents[count].transform.position;
                    data.agentsPosX.Add(pos.x);
                    data.agentsPosY.Add(pos.y);
                    data.agentsPosZ.Add(pos.z);
                    data.agents.Add(acom.agentData);
                }
            }

            bf.Serialize(file, data);
            file.Close();
        }
    }
    
    void CreateAgentsFromFile()
    {
        if(agentPrefab != null)
        { 
            int currentMapIndex = MapController.CurrentMap;
            if (File.Exists(Application.dataPath + "/AgentsInMap" + currentMapIndex + ".agents"))
            {
                BinaryFormatter bf = new BinaryFormatter();
                FileStream file = File.Open(Application.dataPath + "/AgentsInMap" + currentMapIndex + ".agents", FileMode.Open);

                AgentsWithMapData data = (AgentsWithMapData)bf.Deserialize(file);
                file.Close();
                for (int count = 0; count < data.agents.Count; ++count)
                {
                    Vector3 pos = new Vector3(data.agentsPosX[count], data.agentsPosY[count], data.agentsPosZ[count]);
                    GameObject instance = Instantiate(, typeof(GameObject)), pos, Quaternion.identity) as GameObject;
                    instance.GetComponent<AgentController>().agentData = data.agents[count];
                }
            }
        }
    }
}

