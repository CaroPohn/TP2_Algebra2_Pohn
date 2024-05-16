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

    Vector3[] nearPlanePoints = new Vector3[1];
    Vector3[] farPlanePoints = new Vector3[1];

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

    private void ShowRoomsInPlayerSight(Room room = null)
    {
        Room roomToCheck = room == null ? rooms[playerRoomIndex] : room;

        for (int i = 0; i < nearPlanePoints.Length; i++)
        {
            Wall wall = roomToCheck.IsVectorIntersectingWall(nearPlanePoints[i], farPlanePoints[i]);

            if (wall != null)
            {
                bool isEnabled = wall.owner.gameObject.activeSelf;
                wall.owner.gameObject.SetActive(true);

                if(!isEnabled || roomToCheck == rooms[playerRoomIndex])
                {
                    if(wall.conection != null)
                        ShowRoomsInPlayerSight(wall.conection.owner);
                }
            }
        }
    }

    private void SetFrustumPoints()
    {
        nearPlanePoints = GetFrustumPlanePoints(cam.nearClipPlane);
        farPlanePoints = GetFrustumPlanePoints(cam.farClipPlane);
    }

    private Vector3[] GetFrustumPlanePoints(float planeDistanceFromCamera)
    {
        Vector3[] planePoints = new Vector3[segmentsAmount];

        float planeWidth = 2 * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad) * planeDistanceFromCamera;
        float stepSize = planeWidth / (segmentsAmount - 1);

        for (int i = 0; i < segmentsAmount; i++)
        {
            float x = -planeWidth * 0.5f + i * stepSize; //Resto el nearplanewidth dividido 2 para desplazarme desde el medio del nearplane hasta el inicio y luego le sumo el
                                                             //stepsize multiplicado por el numero de segmento para que se vaya desplazando simetricamente a lo largo del eje x.

            Vector3 offset = cam.transform.position + cam.transform.right * x; //El x que setee antes tengo que usarlo basandome en el transform de mi camara y ahi tomando su vector
                                                                               //right para que se posicionen en las coordenadas locales de la camara y no siguiendo el vector right 
                                                                               //del mundo.

            planePoints[i] = planeDistanceFromCamera * cam.transform.forward + offset;
        }

        return planePoints;
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
    }

    //private Vector3 PointInSegmentMiddle()

    //private bool IsSegmentCollitioningPlayerWall()


}
