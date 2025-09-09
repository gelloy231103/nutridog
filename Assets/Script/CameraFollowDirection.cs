using UnityEngine;

public class CameraFollowDirection : MonoBehaviour
{
    public Transform player; // Drag your Player here
    public float smoothSpeed = 10f;

    void LateUpdate()
    {
        if (player == null) return;

        // Follow player position
        transform.position = player.position;

        // Match player's forward direction
        Quaternion targetRotation = Quaternion.Euler(0, player.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }
}
