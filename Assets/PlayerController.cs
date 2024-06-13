using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float turnSpeed = 720.0f; // degrees per second
    public Transform cameraTransform;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        MovePlayer();
        TurnPlayer();
    }

    void MovePlayer()
    {
        // 기본적인 WASD 이동 입력 받기
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // 이동 방향 설정
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // 카메라 기준으로 이동 방향을 변환
            moveDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            moveDirection.y = 0;
            moveDirection.Normalize();

            // 플레이어 이동
            Vector3 moveVelocity = moveDirection * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }
        else
        {
            // 플레이어가 움직이지 않을 때 XZ 속도를 0으로 설정
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void TurnPlayer()
    {
        // 마우스 입력 받아서 회전
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;

        // 플레이어 회전
        Quaternion turnRotation = Quaternion.Euler(0, mouseX, 0);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
