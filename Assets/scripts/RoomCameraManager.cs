using UnityEngine;

public class RoomCameraManager : MonoBehaviour
{
    [Header("Room Cameras")]
    public Camera[] roomCameras;
    private int currentRoomIndex = 0;

    public void ShowRoomCamera(int index)
    {
        currentRoomIndex = Mathf.Clamp(index, 0, roomCameras.Length - 1);
        for (int i = 0; i < roomCameras.Length; i++)
        {
            roomCameras[i].enabled = (i == currentRoomIndex);
        }
    }

    public int GetCurrentRoomIndex()
    {
        return currentRoomIndex;
    }
}
