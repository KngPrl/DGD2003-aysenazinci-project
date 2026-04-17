using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characternew : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private CharacterController characterController;
    [SerializeField] private float speed = 25f;

    [SerializeField] private Animator anim;
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        Vector3 move = transform.forward * vertical + transform.right * horizontal;
        characterController.Move(move * speed * Time.deltaTime);
        if (anim != null)
        {
            anim.SetFloat("Speed", move.magnitude);
        }
    }
}
