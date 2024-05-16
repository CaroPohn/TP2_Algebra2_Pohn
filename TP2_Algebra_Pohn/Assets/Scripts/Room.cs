using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Wall> walls = new List<Wall>();
    private GameObject debug;

    private void Awake()
    {
        AddRoomWalls();
        debug = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    }

    private void AddRoomWalls()
    {
        foreach (Wall wall in transform.GetComponentsInChildren<Wall>())
        {
            walls.Add(wall);
            wall.owner = this;
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

    public Wall IsVectorIntersectingWall(Vector3 origin, Vector3 end)
    {
        foreach (Wall wall in walls)
        {
            if(PlaneRaycast(origin, (end - origin).normalized, wall.wallPlane, out Vector3 collisionPoint))
            {
                if(wall.hasDoor)
                {
                    if(wall.IsPointInsideDoor(collisionPoint))
                    {
                        return wall;
                    }
                } 
            }
        }
        return null;
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
        debug.transform.position = collisionPoint;

        return enter > 0f;
    }
}
