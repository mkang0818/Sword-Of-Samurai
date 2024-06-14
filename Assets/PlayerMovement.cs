using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPun
{
    PhotonView PV;
    Animator anim;
    Camera cam;
    CharacterController controller;
    CameraMovement camera;

    public Transform CameraTarget;

    public float speed = 3F;
    public float runspeed = 8f;
    public float finalSpeed;
    public float smoothness = 10f;
    public bool run;

    public bool toggleCameraRotation;

    private State state;
    enum State 
    {
        Idle, Run, Attack
    }

    // Start is called before the first frame update
    void Start()
    {
        PV = this.GetComponent<PhotonView>();
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        camera = GameObject.FindFirstObjectByType<CameraMovement>();
        if (PV.IsMine)
        {
            camera.Target = CameraTarget;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                toggleCameraRotation = true;
            }
            else
            {
                toggleCameraRotation = false;
            }

            if (Input.GetKey(KeyCode.LeftShift))
            {
                run = true;
                speed = 5;
            }
            else
            {
                run = false;

                speed = 3;
            }

            if (Input.GetMouseButtonDown(0))
            {
                anim.SetBool("BoolAt", true);
            }
            else
            {
                anim.SetBool("BoolAt", false);
            }

            InputMovement();
        }
        
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

        // Calculate percent based on movement direction
        float forwardAmount = Input.GetAxisRaw("Vertical");
        float strafeAmount = Input.GetAxisRaw("Horizontal");

        // Calculate percent for Walk Y (forward/backward)
        float walkYPercent = (forwardAmount > 0) ? forwardAmount : -forwardAmount;
        float walkXPercent = (strafeAmount > 0) ? strafeAmount : -strafeAmount;

        // Adjust percent based on run state
        float movePercent = ((run) ? 1 : 0.5f) * Mathf.Max(walkYPercent, walkXPercent);

        // Set animator parameters
        if (!run)
        {
            forwardAmount *= 0.5f;
            strafeAmount *= 0.5f;
        }

        anim.SetFloat("Walk Y", forwardAmount, 0.1f, Time.deltaTime);

        anim.SetFloat("Walk X", strafeAmount, 0.1f, Time.deltaTime);
    }

}
