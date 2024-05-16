using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
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

    public bool IsVectorIntersectingWall(Vector3 vector)
    {
        return true;
    }

    public bool PlaneRaycast(Vector3 origin, Vector3 direction, Plane plane, out Vector3 collisionPoint) //https://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld017.htm
    {
        float product1 = Vector3.Dot(direction, plane.normal);
        float product2 = 0f - Vector3.Dot(origin, plane.normal) - plane.distance;

        if (Mathf.Approximately(product1, 0f))
        {
            collisionPoint = Vector3.zero;
            return false;
        }

        float enter = product2 / product1;

        collisionPoint = origin + (direction * enter);

        return enter > 0f;
    }
}
