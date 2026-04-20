using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 720f;

    void Update()
    {
        // 1. Get Input
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // 2. Calculate Direction relative to where the PLAYER is facing
        // transform.forward is the direction the character's face is pointing
        // transform.right is the direction of the character's right shoulder
        Vector3 movementDirection = (transform.forward * verticalInput) + (transform.right * horizontalInput);

        // 3. Normalize to prevent 'diagonal speed boost'
        if (movementDirection.magnitude > 1)
        {
            movementDirection.Normalize();
        }

        // 4. Move the character in World Space based on that Local direction
        transform.Translate(movementDirection * speed * Time.deltaTime, Space.World);

        // 5. Rotation Logic (Only rotate the body if we are moving)
        // Note: In First Person, your MouseLook script usually handles Y-rotation, 
        // but this ensures the body stays aligned if you use keys to turn.
        if (movementDirection != Vector3.zero && horizontalInput != 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }
    }
}