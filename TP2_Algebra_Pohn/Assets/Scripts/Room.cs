using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomIndex;
    List<Wall> walls;

    private void Awake()
    {
        AddRoomWalls();
    }

    private void AddRoomWalls()
    {
        foreach (Wall wall in transform.GetComponentsInChildren<Wall>())
        {
            walls.Add(wall);
        }
    }

}
