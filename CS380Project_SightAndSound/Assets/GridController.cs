using System;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public delegate void Resized(float size);
    public event Resized OnResized;

    public int NumberOfCells = 40;

    // Use this for initialization
    void Start ()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        Resize(NumberOfCells);
    }

    [ExecuteInEditMode]
    public void Resize(int size)
    {
        Transform trans = gameObject.GetComponent<Transform>();

        NumberOfCells = Math.Max(1, size);

        Vector3 scale = Vector3.one;

        scale.x = 1.0f / NumberOfCells;
        scale.y = 1.0f / NumberOfCells;

        trans.localScale = scale;

        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();

        player.ResizePlayer(scale.x);

        if (OnResized!=null)
        {
            OnResized(scale.x);
        }
    }

    public bool ScreenWithinGrid(Vector3 trans)
    {
        if (trans.y < 0.0f || trans.y > Screen.height)
        {
            return false;
        }

        float offset = (Screen.width - Screen.height) / 2.0f;

        if (trans.x < offset || trans.x > Screen.width - offset)
        {
            return false;
        }

        return true;
    }

    public Vector3 ScreenToWorld(Vector3 trans)
    {
        trans.x -= (Screen.width - Screen.height) / 2.0f;
        trans.x /= Screen.height;
        trans.y /= Screen.height;

        return trans;
    }

    /*
    public Vector3 ClampWorldToGridSq(Vector3 trans)
    {


        return new Vector3();
    }

    public Vector3Int WorldToGrid(Vector3 trans)
    {



        return new Vector3Int(Mathf.FloorToInt(trans.x), Mathf.FloorToInt(trans.y), Mathf.FloorToInt(trans.z));
    }
    */

    public Vector3 GetCoordinates(int row, int col)
    {
        Vector3 pos = new Vector3();

        float offset = 1.0f / (NumberOfCells * 2.0f);

        pos.x = ((float)col / (float)NumberOfCells) + offset;
        pos.y = ((float)row / (float)NumberOfCells) + offset;
        pos.z = 0.0f;

        return pos;
    }

    public Vector2Int GetRowColumn(Vector3 pos)
    {
        Vector2Int result = new Vector2Int();

        if (pos.x >= 0.0f & pos.x <= 1.0f && pos.y >= 0.0f && pos.y <= 1.0f)
        {
            result.x = (int)(pos.x * NumberOfCells);
            result.y = (int)(pos.y * NumberOfCells);
        }
        else
        {
            result.x = -1;
            result.y = -1;
        }

        return result;
    }

    public bool IsClearPath(Vector2Int gp0, Vector2Int gp1, float epsilon)
    {
        MapController map = gameObject.GetComponentInChildren<MapController>();

        if (gp0 == gp1)
        {
            return !map.IsWall(gp0.y, gp0.x);
        }

        Vector3 p0 = GetCoordinates(gp0.y, gp0.x);
        Vector3 p1 = GetCoordinates(gp1.y, gp1.x);

        // Using rotation by 90
        float norm_x = p0.y - p1.y;
        float norm_y = p1.x - p0.x;

        float mag = Mathf.Sqrt(norm_x * norm_x + norm_y * norm_y);

        norm_x /= mag;
        norm_y /= mag;

        float norm_d = p0.x * norm_x + p0.y * norm_y;

        Vector2Int min = new Vector2Int(Math.Min(gp0.x, gp1.x), Math.Min(gp0.y, gp1.y));
        Vector2Int max = new Vector2Int(Math.Max(gp0.x, gp1.x) + 1, Math.Max(gp0.y, gp1.y) + 1);

        float offset = 1.0f / (NumberOfCells * 2.0f);
        float offset_sqrt = 1.0f / (NumberOfCells * 1.4142136f);  // SQRT OF 2

        int i, j;

        for (i = min.y; i < max.y; ++i)
        {
            for (j = min.x; j < max.x; ++j)
            {
                if (map.IsWall(i, j))
                {
                    Vector3 wall_p = GetCoordinates(i, j);

                    float dist = norm_x * wall_p.x + norm_y * wall_p.y - norm_d;

                    // Early out test
                    // If ABS(dist) of wall to line is 1/(width*2) or less
                    if (Math.Abs(dist) <= offset + epsilon)
                    {
                        return false;
                    }

                    // If ABS(dist) > 1 / (width * sqrt(2)) + EPS then wall is
                    // too far away from the line to matter.
                    if (offset_sqrt + epsilon < Math.Abs(dist))
                    {
                        continue;
                    }

                    // Offsets to get the 4 corners
                    int x_off, y_off;

                    char side = (char)0;        //0000
                    char side_index = (char)1;  //0001

                    // Loops 4 times to check all 4 corners
                    for (x_off = -1; x_off < 2; x_off += 2)
                    {
                        for (y_off = -1; y_off < 2; y_off += 2)
                        {
                            float point_x = wall_p.x + x_off * offset;
                            float point_y = wall_p.y + y_off * offset;

                            dist = norm_x * point_x + norm_y * point_y - norm_d;

                            // If dist <= puffed line of sight
                            // Corner is on line
                            if (Math.Abs(dist) <= epsilon)
                            {
                                return false;
                            }

                            if (0.0f < dist)
                            {
                                side |= side_index;
                            }

                            side_index = (char)(side_index << 1);
                        }
                    }

                    // Make sure all corners landed on the same side.
                    // If not all neg (0000) and if not all pos (1111),
                    // then the sight line segment went through the wall.
                    if (side != 0 && side != 15)
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

}
