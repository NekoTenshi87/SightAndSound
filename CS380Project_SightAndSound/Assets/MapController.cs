using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using System.IO;

public class MapController : MonoBehaviour
{
    public enum TILE
    {
        TILE_WALL = -1,
        TILE_EMPTY,
        TILE_WALL_INVISIBLE
    }

    public int CurrentMap = 0;

    public TileBase WallTile;

    public TileBase InvisWallTile;

    int MaxNumberOfMaps = 0;

    int[] map_width;
    int[][][] map_data;

    // Use this for initialization
    void Start()
    {
        ReloadMaps();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            ++CurrentMap;

            if (!(CurrentMap < MaxNumberOfMaps))
            {
                CurrentMap = 0;
            }

            LoadMap(CurrentMap);
        }
    }

    [ExecuteInEditMode]
    private void OnValidate()
    {
        LoadMap(CurrentMap);
    }

    [ExecuteInEditMode]
    void ClearMap()
    {
        gameObject.GetComponent<Tilemap>().ClearAllTiles();
    }

    [ExecuteInEditMode]
    public void ReloadMaps()
    {
        LoadAllMapsFromFile();
        LoadMap(CurrentMap);
    }

    [ExecuteInEditMode]
    void LoadAllMapsFromFile()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Maps");

        FileInfo[] info = dir.GetFiles("Map*.txt");

        int num_maps = info.Length + 1;

        map_width = new int[num_maps];

        map_data = new int[num_maps][][];

        //Make Empty Map at index 0
        map_width[0] = 40;

        map_data[0] = new int[map_width[0]][];

        for (int i = 0; i < map_width[0]; ++i)
        {
            map_data[0][i] = new int[map_width[0]];

            for (int j = 0; j < map_width[0]; ++j)
            {
                map_data[0][i][j] = 0;
            }
        }

        MaxNumberOfMaps = 1;

        foreach (FileInfo f in info)
        {
            //Debug.Log(f.ToString());

            using (StreamReader reader = new StreamReader(f.ToString()))
            {
                string file;

                file = reader.ReadToEnd();

                if (file != null)
                {
                    string[] lines = file.Split('\n');

                    int width = int.Parse(lines[0]);

                    map_width[MaxNumberOfMaps] = width;

                    map_data[MaxNumberOfMaps] = new int[width][];

                    for (int line_num = 2; line_num < width + 2; ++line_num)
                    {
                        string[] values = lines[line_num].Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);

                        map_data[MaxNumberOfMaps][line_num - 2] = new int[width];

                        for (int value_num = 0; value_num < width; ++value_num)
                        {
                            map_data[MaxNumberOfMaps][line_num - 2][value_num] = int.Parse(values[value_num]);
                        }
                    }
                }
            }

            ++MaxNumberOfMaps;
        }
    }

    [ExecuteInEditMode]
    void LoadMap(int index)
    {
        ClearMap();

        int map_index = Math.Max(0, Math.Min(MaxNumberOfMaps, index));

        CurrentMap = map_index;

        int width = map_width[map_index];

        Tilemap map = gameObject.GetComponent<Tilemap>();

        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < width; ++j)
            {
                Vector3Int placement = Vector3Int.zero;
                placement.x = j;
                placement.y = i;

                switch (map_data[map_index][i][j])
                {
                    case (int)TILE.TILE_WALL:
                        map.SetTile(placement, WallTile);
                        break;
                    case (int)TILE.TILE_WALL_INVISIBLE:
                        map.SetTile(placement, InvisWallTile);
                        break;
                    default:
                        break;
                }

            }
        }

        GridController grid = gameObject.GetComponentInParent<GridController>();

        grid.Resize(width);
    }
}
