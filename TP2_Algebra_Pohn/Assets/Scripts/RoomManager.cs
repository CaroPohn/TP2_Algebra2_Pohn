using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;
    private List<Room> rooms;

    private void Awake()
    {
        rooms = GetComponentsInChildren<Room>().ToList();
    }

    private void Update()
    {
        HideAllRooms();
        ShowPlayerRoom();
        ShowRoomsInPlayerSight();
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
        foreach(Room room in rooms)
        {
            if(IsPlayerInRoom(room))
            { 
                room.gameObject.SetActive(true);
                return;
            }
        }
    }

    private void ShowRoomsInPlayerSight()
    {

    }

    private bool IsPlayerInRoom(Room room)
    {
        return room.IsPointInRoom(playerCamera.position);
    }
}
