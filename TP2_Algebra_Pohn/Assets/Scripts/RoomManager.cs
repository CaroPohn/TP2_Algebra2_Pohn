using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Camera cam;
    private List<Room> rooms;
    [SerializeField] private int segmentsAmount = 1;
    private int playerRoomIndex = 0;

    [SerializeField] Vector3[] nearPlanePoints = new Vector3[1];
    [SerializeField] Vector3[] farPlanePoints = new Vector3[1];

    List<Vector3> BSPPoints = new List<Vector3>();

    private void Awake()
    {
        rooms = GetComponentsInChildren<Room>().ToList();
    }

    private void Update()
    {
        HideAllRooms();
        ShowPlayerRoom();
        ShowRoomsInPlayerSight();
        SetFrustumPoints();    
    }

    private void HideAllRooms()
    {
        foreach (Room room in rooms)
        {
            room.gameObject.SetActive(false);
        }
    }

    private void ShowPlayerRoom()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (IsPlayerInRoom(rooms[i]))
            {
                rooms[i].gameObject.SetActive(true);
                playerRoomIndex = i;
                return;
            }
        }
    }

    private bool IsPlayerInRoom(Room room)
    {
        return room.IsPointInRoom(playerCamera.position);
    }

    private void ShowRoomsInPlayerSight()
    {
        BSPPoints.Clear();

        List<Room> roomsToDraw = new List<Room>();
        roomsToDraw.Add(rooms[playerRoomIndex]);

        for (int i = 0; i < nearPlanePoints.Length; i++)
        {
            RecursiveBSP(nearPlanePoints[i], farPlanePoints[i], roomsToDraw);
        }

        foreach (Room room in roomsToDraw)
        {
            room.gameObject.SetActive(true);
        }
    }

    private void RecursiveBSP(Vector3 initPoint, Vector3 endPoint, List<Room> roomsToDraw)
    {
        Vector3 middlePoint = Vector3.Lerp(initPoint, endPoint, 0.5f);

        if (Vector3.Distance(middlePoint, endPoint) < 0.25f)
            return;

        BSPPoints.Add(middlePoint);

        bool middlePointInNewRoom = false;

        foreach (Room room in rooms)
        {
            if (!roomsToDraw.Contains(room) && room.IsPointInRoom(middlePoint))
            {
                //Wall intersectedWall = room.GetIntersectingWall(initPoint, middlePoint);

                //if (intersectedWall != null && intersectedWall.IsPointInsideDoor(middlePoint))
                //{
                //}

                middlePointInNewRoom = true;
                roomsToDraw.Add(room);
                
                //if (!room.IsVectorIntersectingWall(initPoint, middlePoint))
                //{
                //}
            }
        }

        if(middlePointInNewRoom)
        {
            RecursiveBSP(initPoint, middlePoint, roomsToDraw);
            RecursiveBSP(middlePoint, endPoint, roomsToDraw);
        }
        else
        {
            RecursiveBSP(middlePoint, endPoint, roomsToDraw);                
        }
    }

    private void SetFrustumPoints()
    {
        nearPlanePoints = GetFrustumPlanePoints(cam.nearClipPlane);
        farPlanePoints = GetFrustumPlanePoints(cam.farClipPlane);
    }

    private Vector3[] GetFrustumPlanePoints(float planeDistanceFromCamera)
    {
        List<Vector3> planePoints = new List<Vector3>(segmentsAmount * segmentsAmount);

        float planeSize = 2 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * planeDistanceFromCamera;
        float stepSize = planeSize / (segmentsAmount - 1);

        for (int i = 0; i < segmentsAmount; i++)
        {
            for (int j = 0; j < segmentsAmount; j++)
            {
                float x = -planeSize * 0.5f + i * stepSize; //Resto el nearplanewidth dividido 2 para desplazarme desde el medio del nearplane hasta el inicio y luego le sumo el
                                                            //stepsize multiplicado por el numero de segmento para que se vaya desplazando simetricamente a lo largo del eje x.

                float y = -planeSize * 0.5f + j * stepSize;

                Vector3 offset = cam.transform.position + cam.transform.right * x + cam.transform.up * y; //El x que setee antes tengo que usarlo basandome en el transform de mi camara y ahi tomando su vector
                                                                                                          //right para que se posicionen en las coordenadas locales de la camara y no siguiendo el vector right 
                                                                                                          //del mundo.

                planePoints.Add(planeDistanceFromCamera * cam.transform.forward + offset);
            }
        }

        return planePoints.ToArray();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < nearPlanePoints.Length; i++)
        {
            Gizmos.DrawSphere(nearPlanePoints[i], .05f);
        }

        for (int i = 0; i < farPlanePoints.Length; i++)
        {
            Gizmos.DrawSphere(farPlanePoints[i], .05f);
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < nearPlanePoints.Length; i++)
        {
            Gizmos.DrawLine(nearPlanePoints[i], farPlanePoints[i]);
        }

        Gizmos.color = Color.red;
        foreach (Vector3 point in BSPPoints) 
        {
            Gizmos.DrawSphere(point, .3f);
        }
    }

    //private Vector3 PointInSegmentMiddle()

    //private bool IsSegmentCollitioningPlayerWall()
}
