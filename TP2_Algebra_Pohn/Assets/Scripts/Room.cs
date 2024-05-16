using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int roomIndex;
    private List<Wall> walls = new List<Wall>();

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

    public bool IsPointInRoom(Vector3 point)
    {
        foreach(Wall wall in walls)
        {
            if(!wall.wallPlane.GetSide(point))
            {
                return false;
            }
        }

        return true;
    }
}
