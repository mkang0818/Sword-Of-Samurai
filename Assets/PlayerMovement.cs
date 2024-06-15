using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    [HideInInspector] public PhotonView PV;
    Animator anim;
    Camera cam;
    CharacterController controller;
    CameraMovement camera;

    public Transform CameraTarget;

    public float speed = 1;
    public float runspeed = 1.2f;
    public float finalSpeed;
    public float smoothness = 10f;
    public bool run;

    public bool toggleCameraRotation;

    public float MaxHp;
    public float CurHp;


    private State state;
    enum State
    {
        Idle, Run, RightAt, LeftAt, RightDf, LeftDf, RightHit, LeftHit, Death
    }

    private Vector3 networkedPosition;
    private Quaternion networkedRotation;
    private float distance;
    private float angle;

    private Vector3 moveDirection;
    private Vector3 velocity;
    public float gravity = -9.81f;

    float blockT = 0.5f;
    void Start()
    {
        PV = this.GetComponent<PhotonView>();
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controller = this.GetComponent<CharacterController>();
        camera = GameObject.FindObjectOfType<CameraMovement>();

        if (PV.IsMine)
        {
            camera.Target = CameraTarget;
            camera.PV = PV;
        }
        else
        {
            networkedPosition = transform.position;
            networkedRotation = transform.rotation;
        }
    }

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
                speed = 1.2f;
            }
            else
            {
                run = false;
                speed = 1;
            }

            // 블록 구현
            if (Input.GetKey(KeyCode.Q))
            {
                state = State.LeftDf;
                print("LeftDf");
                anim.SetBool("Block", true);
                if (blockT <= 0.75f) blockT += Time.deltaTime;
                anim.SetFloat("Block X", blockT);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                state = State.RightDf;
                print("RightDf");
                anim.SetBool("Block", true);
                if (blockT >= 0.25f) blockT -= Time.deltaTime;
                anim.SetFloat("Block X", blockT);
            }
            else if (Input.GetKeyUp(KeyCode.Q) || Input.GetKeyUp(KeyCode.E))
            {
                state = State.Idle;
                blockT = 0.5f;
                anim.SetBool("Block", false);
            }

            // 기본 공격구현
            if (Input.GetMouseButtonDown(1))
            {
                state = State.RightAt;
                print("RightAt");
                anim.SetBool("RightAt", true);
                //PV.RPC("RightAtRPC", RpcTarget.AllBuffered);
            }
            /*else if (Input.GetMouseButtonDown(1))
            {
                state = State.RightAt;
                print("RightAt");
                //anim.SetBool("BoolAt", true);
                PV.RPC("AttackRPC", RpcTarget.AllBuffered);
            }*/

            // 죽는 애니메이션
            if (Input.GetKey(KeyCode.P))
            {
                //print("게임오버");
                //anim.SetTrigger("Death");
                //state = State.Death;

                state = State.LeftHit;
            }

            if (state == State.LeftHit)
            {
                PV.RPC("LeftHitRPC", RpcTarget.AllBuffered);
            }
            else if (state == State.RightHit)
            {
                PV.RPC("RightHitRPC", RpcTarget.AllBuffered);
            }

            InputMovement();
        }
        else
        {
            SmoothMove();
        }

        // 중력 적용
        if (!controller.isGrounded)
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else
        {
            velocity.y = 0;
        }

        controller.Move(velocity * Time.deltaTime);
    }


    [PunRPC]
    void RightAtRPC()
    {
        anim.SetBool("RightAt", true);
    }

    [PunRPC]
    void RightHitRPC()
    {
        anim.SetBool("RightHit", true);
    }
    [PunRPC]
    void LeftHitRPC()
    {
        anim.SetBool("LeftHit",true);
    }
    private void LateUpdate()
    {
        if (PV.IsMine && !toggleCameraRotation)
        {
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void InputMovement()
    {
        finalSpeed = (run) ? runspeed : speed;

        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * Input.GetAxisRaw("Vertical") + right * Input.GetAxisRaw("Horizontal")).normalized;

        controller.Move(moveDirection * finalSpeed * Time.deltaTime);

        // Set animator parameters
        float forwardAmount = Input.GetAxisRaw("Vertical");
        float strafeAmount = Input.GetAxisRaw("Horizontal");

        if (!run)
        {
            forwardAmount *= 0.5f;
            strafeAmount *= 0.5f;
        }

        anim.SetFloat("Walk Y", forwardAmount, 0.1f, Time.deltaTime);
        anim.SetFloat("Walk X", strafeAmount, 0.1f, Time.deltaTime);
    }

    void SmoothMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, networkedPosition, Time.deltaTime * distance / PhotonNetwork.SerializationRate);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, networkedRotation, Time.deltaTime * angle / PhotonNetwork.SerializationRate);
    }

    public void RightAtEnd()
    {
        print("공격끝");
        state = State.Idle;
        anim.SetBool("RightAt", false);
    }
    public void LeftHitEnd()
    {
        state = State.Idle;
        anim.SetBool("LeftHit", false);
    }
    public void RightHitEnd()
    {
        state = State.Idle;
        anim.SetBool("RightHit", false);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (anim != null)
            {
                // 데이터를 보냅니다
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(anim.GetBool("RightAttack"));
                stream.SendNext(anim.GetBool("Block"));
                stream.SendNext(anim.GetFloat("Block X"));
                stream.SendNext(anim.GetFloat("Walk X"));
                stream.SendNext(anim.GetFloat("Walk Y"));
            }
            else
            {
                print("error");
            }
        }
        else
        {
            if (anim != null)
            {
                // 데이터를 받습니다
                networkedPosition = (Vector3)stream.ReceiveNext();
                networkedRotation = (Quaternion)stream.ReceiveNext();
                anim.SetBool("RightAttack", (bool)stream.ReceiveNext());
                anim.SetBool("Block", (bool)stream.ReceiveNext());
                anim.SetFloat("Block X", (float)stream.ReceiveNext());
                anim.SetFloat("Walk X", (float)stream.ReceiveNext());
                anim.SetFloat("Walk Y", (float)stream.ReceiveNext());
            }
            else
            {
                print("error");
            }
            // 보간을 위한 거리와 각도 계산
            distance = Vector3.Distance(transform.position, networkedPosition);
            angle = Quaternion.Angle(transform.rotation, networkedRotation);
        }
    }
}
