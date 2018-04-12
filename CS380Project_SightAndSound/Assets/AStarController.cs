using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarController : MonoBehaviour
{
    int map_size = 40;

    class NodeData
    {
        int parent_x = -1;
        int parent_y = -1;
        float given = 0;
        float cost = float.MaxValue;
    }

    NodeData[][] searchSpace = null;

	// Use this for initialization
	void Start ()
    {
        InitSearchSpace();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ValidateSearchSpaceSize()
    {
        GridController grid = GameObject.Find("Grid").GetComponent<GridController>();

        if (map_size < grid.NumberOfCells)
        {
            ResizeSearchSpace(grid.NumberOfCells);
        }
    }

    void ResizeSearchSpace(int size)
    {
        map_size = size;

        InitSearchSpace();
    }

    void InitSearchSpace()
    {
        searchSpace = new NodeData[map_size][];

        for (int i = 0; i < map_size; ++i)
        {
            searchSpace[i] = new NodeData[map_size];
        }
    }
}
