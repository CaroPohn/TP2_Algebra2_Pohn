using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<Wall> walls = new List<Wall>();

    private void Awake()
    {
        AddRoomWalls();
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
        if(point.y > walls[0].transform.position.y + 1.5f || point.y < walls[0].transform.position.y - 1.5f)
            return false;

        foreach(Wall wall in walls)
        {
            if(!wall.wallPlane.GetSide(point))
            {
                return false;
            }
        }

        return true;
    }

    public bool IsVectorIntersectingWall(Vector3 origin, Vector3 end)
    {
        foreach (Wall wall in walls)
        {
            Vector3 direction = (end - origin).normalized;

            if(wall.wallPlane.Raycast(new Ray(origin, direction), out float enter))/*PlaneRaycast(origin, (end - origin).normalized, wall.wallPlane, out Vector3 collisionPoint)*/
            {
                Vector3 collisionPoint = origin + direction * enter;

                Debug.DrawLine(origin, collisionPoint, Color.yellow);
                if(wall.hasDoor)
                {
                    if(wall.IsPointInsideDoor(collisionPoint))
                    {
                        return false;
                    }
                } 
                return true;
            }
        }
        return false;
    }

    public Wall GetIntersectingWall(Vector3 pointOne, Vector3 pointTwo)
    {
        foreach (Wall wall in walls)
        {
            if(!wall.wallPlane.SameSide(pointOne, pointTwo))
            {
                return wall;
            }
        }

        return null;
    }

    private bool PlaneRaycast(Vector3 origin, Vector3 direction, Plane plane, out Vector3 collisionPoint) //https://www.cs.princeton.edu/courses/archive/fall00/cs426/lectures/raycast/sld017.htm
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
