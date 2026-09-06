using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(2f, 1f, -10f);
    public float minX = 0f; // Bloque la caméra pour ne pas reculer hors de la map

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = new Vector3(Mathf.Max(target.position.x + offset.x, minX), transform.position.y, offset.z);
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }
}