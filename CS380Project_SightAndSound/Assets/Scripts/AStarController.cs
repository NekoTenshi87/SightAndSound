using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarController : MonoBehaviour
{
    public float HeuristicWeight = 1.01f;

    int map_size = 40;

    int search_space_size = 40;

    Vector2Int start, goal;

    int OpenListSize = 0;

    float distance = 0.0f;

    LinkedList<Vector3> waypoints = new LinkedList<Vector3>();

    GridController grid;
    //MovementController move;
    MapController map;

  enum DIRECTION
    {
        D_NONE = 0,
        D_N = 1,
        D_E = 2,
        D_S = 4,
        D_W = 8,
        D_NE = 16,
        D_SE = 32,
        D_SW = 64,
        D_NW = 128
    };

    DIRECTION BlockedDirs = DIRECTION.D_NONE;

    public class NodeData
    {
        public int parent_x = -1;
        public int parent_y = -1;
        public float given = 0;
        public float cost = float.MaxValue;
        public bool open = false;
        public bool closed = false;

        public void PutOnOpen()
        {
            open = true;
            closed = false;
        }

        public void PutOnClosed()
        {
            open = false;
            closed = true;
        }

        public void Update(NodeData n)
        {
            parent_x = n.parent_x;
            parent_y = n.parent_y;
            given = n.given;
            cost = n.cost;
            open = n.open;
            closed = n.closed;
        }

        public void Clear()
        {
            parent_x = -1;
            parent_y = -1;
            given = 0;
            cost = float.MaxValue;
            open = false;
            closed = false;
        }
    }

    NodeData[][] searchSpace = null;
  
    void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<GridController>();
        //move = gameObject.GetComponent<MovementController>();
        map = grid.GetComponentInChildren<MapController>();
    }
	  // Use this for initialization
	  void Start ()
    {
        InitSearchSpace();
    }

    // Update is called once per frame
    void Update()
    { }

    public void ValidateSearchSpaceSize()
    {
        if (search_space_size < grid.NumberOfCells)
        {
            ResizeSearchSpace(grid.NumberOfCells);
        }

        map_size = grid.NumberOfCells;
    }

    void ResizeSearchSpace(int size)
    {
        search_space_size = size;

        InitSearchSpace();
    }

    void InitSearchSpace()
    {
        searchSpace = new NodeData[search_space_size][];

        for (int i = 0; i < search_space_size; ++i)
        {
            searchSpace[i] = new NodeData[search_space_size];

            for (int j = 0; j < search_space_size; ++j)
            {
                searchSpace[i][j] = new NodeData();
            }
        }
    }

    public void SetStart(Vector2Int pos)
    {
        start = pos;
    }

    public void SetGoal(Vector2Int pos)
    {
        goal = pos;
    }

    public Vector2Int GetStart()
    {
        return start;
    }

    public Vector2Int GetGoal()
    {
        return goal;
    }

    public void Clear()
    {
        OpenListSize = 0;

        for (int i = 0; i < map_size; ++i)
        {
            for (int j = 0; j < map_size; ++j)
            {
                searchSpace[i][j].Clear();
            }
        }
    }

    public bool Empty()
    {
        return OpenListSize == 0;
    }

    public void Push(Vector2Int pos, NodeData n)
    {
        n.PutOnOpen();

        if (!searchSpace[pos.y][pos.x].open && n.open)
        {
            ++OpenListSize;
        }

        searchSpace[pos.y][pos.x].Update(n);
    }

    public void Pop(Vector2Int pos)
    {
        if (searchSpace[pos.y][pos.x].open)
        {
            --OpenListSize;
        }

        searchSpace[pos.y][pos.x].PutOnClosed();
    }

    public bool Update(float max_dist)
    {
        Vector2Int index = findOpenListMinIndex();
        
        if (index.x < 0 || index.y < 0)
        {
            return false;
        }

        Pop(index);

        // If node is goal, complete path, then return false to leave update
        if (index == goal)
        {
            Vector2Int curr_pos = goal;

            distance = searchSpace[curr_pos.y][curr_pos.x].given;

            while (curr_pos != start)
            {
                if (curr_pos.x < 0 || curr_pos.y < 0)
                {
                    break;
                }

                if (searchSpace[curr_pos.y][curr_pos.x].cost == float.MaxValue)
                {
                    break;
                }

                PushWaypointFirst(grid.GetCoordinates(curr_pos.y, curr_pos.x));

                int p_x = searchSpace[curr_pos.y][curr_pos.x].parent_x;
                int p_y = searchSpace[curr_pos.y][curr_pos.x].parent_y;

                curr_pos.x = p_x;
                curr_pos.y = p_y;
            }

            PushWaypointFirst(grid.GetCoordinates(start.y, start.x));

            return false;
        }

        pushNeighborNodes(index, max_dist);

        return true;
    }

    public void SetPathDistance(float dist)
    {
        distance = dist;
    }

    public float GetPathDistance()
    {
        return distance;
    }

    public float CalcHeuristic(Vector2Int pos, Vector2Int goal)
    {
        return Octile(pos.y, pos.x, goal.y, goal.x) * HeuristicWeight;
    }

    Vector2Int findOpenListMinIndex()
    {
        Vector2Int minIndex = new Vector2Int(-1, -1);

        if (Empty())
        {
            return minIndex;
        }

        float cheapest_cost = float.MaxValue;
        
        for (int i = 0; i < map_size; ++i)
        {
            for (int j = 0; j < map_size; ++j)
            {
                if (searchSpace[i][j].open)
                {
                    if (searchSpace[i][j].cost < cheapest_cost)
                    {
                        cheapest_cost = searchSpace[i][j].cost;
                        minIndex.x = j;
                        minIndex.y = i;
                    }
                }
            }
        }

        return minIndex;
    }

    void pushNeighborNodes(Vector2Int parent, float max_dist)
    {
        BlockedDirs = DIRECTION.D_NONE;

        setBlockedDir(parent.y, parent.x, searchSpace[parent.y][parent.x].parent_y, searchSpace[parent.y][parent.x].parent_x);

        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                // Not need to check parent node
                if (i == 0 && j == 0)
                {
                    continue;
                }

                Vector2Int pos = new Vector2Int();
                pos.y = parent.y + i;
                pos.x = parent.x + j;

                // Check if outside of bounds
                if (pos.y < 0)
                {
                    if (pos.x == parent.x)
                    {
                        setBlockedDir(parent.y, parent.x, pos.y, pos.x);
                    }
                    continue;
                }
                else if (pos.y > map_size - 1)
                {
                    if (pos.x == parent.x)
                    {
                        setBlockedDir(parent.y, parent.x, pos.y, pos.x);
                    }
                    continue;
                }

                if (pos.x < 0)
                {
                    if (pos.y == parent.y)
                    {
                        setBlockedDir(parent.y, parent.x, pos.y, pos.x);
                    }
                    continue;
                }
                else if (pos.x > map_size - 1)
                {
                    if (pos.y == parent.y)
                    {
                        setBlockedDir(parent.y, parent.x, pos.y, pos.x);
                    }
                    continue;
                }

                // Check for wall

                if (map.IsWall(pos.y, pos.x))
                {
                    setBlockedDir(parent.y, parent.x, pos.y, pos.x);
                }
            }
        }

        for (int i = -1; i < 2; ++i)
        {
            for (int j = -1; j < 2; ++j)
            {
                // Don't push parent node
                if (i == 0 && j == 0)
                {
                    continue;
                }

                if (i < 0)
                {
                    if (j < 0)
                    {
                        if ((BlockedDirs & DIRECTION.D_SW) == DIRECTION.D_SW)
                        {
                            continue;
                        }
                    }
                    else if (j > 0)
                    {
                        if ((BlockedDirs & DIRECTION.D_SE) == DIRECTION.D_SE)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if ((BlockedDirs & DIRECTION.D_S) == DIRECTION.D_S)
                        {
                            continue;
                        }
                    }
                }
                else if (i > 0)
                {
                    if (j < 0)
                    {
                        if ((BlockedDirs & DIRECTION.D_NW) == DIRECTION.D_NW)
                        {
                            continue;
                        }
                    }
                    else if (j > 0)
                    {
                        if ((BlockedDirs & DIRECTION.D_NE) == DIRECTION.D_NE)
                        {
                            continue;
                        }
                    }
                    else
                    {
                        if ((BlockedDirs & DIRECTION.D_N) == DIRECTION.D_N)
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    if (j < 0)
                    {
                        if ((BlockedDirs & DIRECTION.D_W) == DIRECTION.D_W)
                        {
                            continue;
                        }
                    }
                    else if (j > 0)
                    {
                        if ((BlockedDirs & DIRECTION.D_E) == DIRECTION.D_E)
                        {
                            continue;
                        }
                    }
                }

                Vector2Int pos = new Vector2Int();
                pos.y = parent.y + i;
                pos.x = parent.x + j;

                NodeData child = new NodeData();

                child.parent_x = parent.x;
                child.parent_y = parent.y;

                // Compute cost

                // If diagonal
                if (i * j > 0)
                {
                    child.given = searchSpace[parent.y][parent.x].given + 1.41f;
                }
                else
                {
                    child.given = searchSpace[parent.y][parent.x].given + 1.0f;
                }

                if (child.given > max_dist)
                {
                    continue;
                }

                child.cost = child.given + CalcHeuristic(pos, goal);

                bool onOL = searchSpace[pos.y][pos.x].open;
                bool onCL = searchSpace[pos.y][pos.x].closed;

                // If child node isn't on OpenList or ClosedList, put on OpenList
                if (!onOL && !onCL)
                {
                    Push(pos, child);
                }

                else if (onOL && !onCL)
                {
                    if (child.cost < searchSpace[pos.y][pos.x].cost)
                    {
                        Push(pos, child);
                    }
                }
                else if (!onOL && onCL)
                {
                    if (child.cost < searchSpace[pos.y][pos.x].cost)
                    {
                        Push(pos, child);
                    }
                }
            }
        }
    }

    void setBlockedDir(int node_row, int node_col, int wall_row, int wall_col)
    {
        Vector2Int diff = new Vector2Int(wall_col - node_col, wall_row - node_row);

        if (diff.x < 0)
        {
            if (diff.y < 0)
            {
                BlockedDirs |= DIRECTION.D_SW;
            }
            else if (diff.y > 0)
            {
                BlockedDirs |= DIRECTION.D_NW;
            }
            else
            {
                BlockedDirs |= DIRECTION.D_NW | DIRECTION.D_W | DIRECTION.D_SW;
            }
        }
        else if (diff.x > 0)
        {
            if (diff.y < 0)
            {
                BlockedDirs |= DIRECTION.D_SE;
            }
            else if (diff.y > 0)
            {
                BlockedDirs |= DIRECTION.D_NE;
            }
            else
            {
                BlockedDirs |= DIRECTION.D_NE | DIRECTION.D_E | DIRECTION.D_SE;
            }
        }
        else
        {
            if (diff.y < 0)
            {
                BlockedDirs |= DIRECTION.D_SW | DIRECTION.D_S | DIRECTION.D_SE;
            }
            else if (diff.y > 0)
            {
                BlockedDirs |= DIRECTION.D_NW | DIRECTION.D_N | DIRECTION.D_NE;
            }
        }
    }

    float Octile(int a_row, int a_col, int b_row, int b_col)
    {
        float row = Mathf.Abs((float)(b_row - a_row));
        float col = Mathf.Abs((float)(b_col - b_col));

        return Mathf.Min(row, col) * 1.41f + Mathf.Max(row, col) - Mathf.Min(row, col);
    }


  public bool ComputePath(Vector3 goal_pos, float max_dist, bool newRequest)
  {
    if (newRequest)
    {
      ValidateSearchSpaceSize();

      Vector2Int goal_grid = grid.GetRowColumn(goal_pos);

        if (map.IsWall(goal_grid.y, goal_grid.x))
        {
            Vector2Int p = grid.GetRowColumn(gameObject.transform.position);
            waypoints.AddFirst(grid.GetCoordinates(p.y, p.x));
            SetPathDistance(float.MaxValue);

            return true;
        }

      Vector2Int start_grid = grid.GetRowColumn(gameObject.transform.position);

        if (map.IsWall(start_grid.y, start_grid.x))
        {
            Vector2Int p = grid.GetRowColumn(gameObject.transform.position);
            waypoints.AddFirst(grid.GetCoordinates(p.y, p.x));
            SetPathDistance(float.MaxValue);

            return true;
        }

      waypoints.Clear();

      if (goal_grid.x == start_grid.x && goal_grid.y == start_grid.y)
      {
        waypoints.AddLast(grid.GetCoordinates(goal_grid.y, goal_grid.x));
        SetPathDistance(0.0f);
        return true;
      }

      if (grid.IsClearPath(start_grid, goal_grid, gameObject.transform.lossyScale.x / 2.0f))
      {
        Vector2Int diff = goal_grid - start_grid;

        float dist = Mathf.Sqrt((float)(diff.x * diff.x + diff.y * diff.y));

        if (dist > max_dist)
        {
          waypoints.AddFirst(gameObject.transform.position);
          SetPathDistance(float.MaxValue);
        }
        else
        {
          waypoints.AddFirst(goal_pos);
          waypoints.AddFirst(gameObject.transform.position);
          SetPathDistance(dist);
        }

        return true;
      }


      // Clear List
      Clear();

      //Set Variables
      SetStart(start_grid);
      SetGoal(goal_grid);

      NodeData node = new NodeData();

      node.cost = CalcHeuristic(start_grid, goal_grid);

      Push(start_grid, node);
    }

    while (!Empty())
    {
      if (!Update(max_dist))
      {
        return true;
      }
    }

    Vector2Int pos = grid.GetRowColumn(gameObject.transform.position);
    waypoints.AddFirst(grid.GetCoordinates(pos.y, pos.x));
    SetPathDistance(float.MaxValue);

    return true;
  }

  public int GetWaypointCount()
  {
    return waypoints.Count;
  }

  public Vector3 GetWaypointFirstValue()
  {
    return waypoints.First.Value;
  }

  public void PushWaypointLast(Vector3 pos)
  {
    waypoints.AddLast(pos);
  }

  public void PushWaypointFirst(Vector3 pos)
  {
    waypoints.AddFirst(pos);
  }

  public void RemoveFirstWaypoint()
  {
    if (waypoints.Count > 0)
    {
      waypoints.RemoveFirst();
    }
  }

  public void ClearWaypoints()
  {
    waypoints.Clear();
  }

    public LinkedList<Vector3> GetWaypoints()
    {
        return waypoints;
    }

  public LinkedList<Vector3> GetInvertedWaypoints()
  {
    LinkedList<Vector3> wp = new LinkedList<Vector3>();

    foreach (Vector3 vect in waypoints)
    {
      wp.AddFirst(vect);
    }

    return wp;
  }

  public void SetWaypoints(LinkedList<Vector3> wp)
  {
        ClearWaypoints();

        foreach (Vector3 vect in wp)
        {
            waypoints.AddLast(vect);
        }
  }

    public void DrawPath(LineRenderer rend)
    {
        int count = waypoints.Count;

        if (count > 0)
        {
            rend.positionCount = count + 1;

            int index = 0;

            rend.SetPosition(index++, gameObject.transform.position);

            foreach (Vector3 pos in waypoints)
            {
                rend.SetPosition(index++, pos);
            }
        }
    }
}
