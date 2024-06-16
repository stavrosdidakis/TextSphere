using UnityEngine;

public class Rotation3Axis : MonoBehaviour
{
    public float minRotationSpeed = -10f;
    public float maxRotationSpeed = 10f;

    private float rotationSpeedX;
    private float rotationSpeedY;
    private float rotationSpeedZ;

    void Start()
    {
        // Generate random rotation speeds for each axis within the specified range.
        rotationSpeedX = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationSpeedY = Random.Range(minRotationSpeed, maxRotationSpeed);
        rotationSpeedZ = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void Update()
    {
        // Rotate the object around the X, Y, and Z axes based on the random rotation speeds.
        // Time.deltaTime is used to make the rotation frame rate independent.
        transform.Rotate(Vector3.right * rotationSpeedX * Time.deltaTime);
        transform.Rotate(Vector3.up * rotationSpeedY * Time.deltaTime);
        transform.Rotate(Vector3.forward * rotationSpeedZ * Time.deltaTime);
    }
}
