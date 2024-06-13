using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator anim;
    Camera cam;
    CharacterController controller;

    public float speed = 5f;
    public float runspeed = 8f;
    public float finalSpeed;
    public float smoothness = 10f;
    public bool run;

    public bool toggleCameraRotation;

    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true;
        }else
        {
            toggleCameraRotation = false;
        }

        if(Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
            
            InputMovement();
    }
    private void LateUpdate()
    {
        if(toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }
    void InputMovement()
    {
        finalSpeed = (run) ? runspeed : speed;

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal");

        controller.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);

        float percent = ((run) ? 1 : 0.5f) * moveDirection.magnitude;
        anim.SetFloat("Blend", percent, 0.1f, Time.deltaTime);
    }
}
