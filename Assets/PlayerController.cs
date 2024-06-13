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
        // �⺻���� WASD �̵� �Է� �ޱ�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // �̵� ���� ����
        Vector3 moveDirection = new Vector3(horizontal, 0, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // ī�޶� �������� �̵� ������ ��ȯ
            moveDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
            moveDirection.y = 0;
            moveDirection.Normalize();

            // �÷��̾� �̵�
            Vector3 moveVelocity = moveDirection * moveSpeed;
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }
        else
        {
            // �÷��̾ �������� ���� �� XZ �ӵ��� 0���� ����
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }

    void TurnPlayer()
    {
        // ���콺 �Է� �޾Ƽ� ȸ��
        float mouseX = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;

        // �÷��̾� ȸ��
        Quaternion turnRotation = Quaternion.Euler(0, mouseX, 0);
        rb.MoveRotation(rb.rotation * turnRotation);
    }
}
