using UnityEngine;

public class ForceCameraBehindPlayer : MonoBehaviour
{
    public Transform player;  // Drag Player here
    public float smoothSpeed = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        // Match camera rotation with player's facing direction
        Quaternion targetRotation = Quaternion.Euler(0, player.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
    }
}
