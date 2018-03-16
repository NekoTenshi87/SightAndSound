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
}
