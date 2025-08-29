using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private float followSpeed = 5.0f;
    private Vector3 offset;
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Using a late update to update the camera position(as a follow to the player) 
    void LateUpdate()
    {
        Vector3 finalPosition = player.transform.position + offset;
        transform.position = Vector3.Slerp
        (transform.position, finalPosition, followSpeed * Time.deltaTime);
    }
}
