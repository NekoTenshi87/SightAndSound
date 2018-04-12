using System;
using UnityEngine;

public class GridController : MonoBehaviour
{
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

    public Vector3 ClampWorldToGridSq(Vector3 trans)
    {


        return new Vector3();
    }

    public Vector3Int WorldToGrid(Vector3 trans)
    {



        return new Vector3Int(Mathf.FloorToInt(trans.x), Mathf.FloorToInt(trans.y), Mathf.FloorToInt(trans.z));
    }

    public Vector3 GetCoordinates(Vector2Int grid_pos)
    {
        Vector3 pos = new Vector3();

        float offset = 1.0f / NumberOfCells / 2.0f;

        pos.x = ((float)grid_pos.x / (float)NumberOfCells) + offset;
        pos.y = ((float)grid_pos.y / (float)NumberOfCells) + offset;
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
}
