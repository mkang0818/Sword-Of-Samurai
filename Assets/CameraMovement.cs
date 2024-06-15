using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraMovement : MonoBehaviour
{
    [HideInInspector] public PhotonView PV;
    public Transform Target;
    public float followSpeed = 10f;
    public float sensitivity = 100f;
    public float clampAngle = 70f;

    private float rotX;
    private float rotY;

    public Transform realCamera;
    public Vector3 offset; // 카메라와 플레이어 사이의 오프셋
    public float smoothness = 0.1f;

    void Start()
    {
        if (Target != null)
        {
            Vector3 angles = transform.eulerAngles;
            rotX = angles.x;
            rotY = angles.y;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (Target != null && PV.IsMine)
        {
            rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
            rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

            rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);

            // 카메라의 회전만 처리
            Quaternion rotation = Quaternion.Euler(rotX, rotY, 0);
            transform.rotation = rotation;
        }
    }

    private void LateUpdate()
    {
        if (Target != null && PV.IsMine)
        {
            // 목표 위치 계산 (Y축 회전만 적용)
            Vector3 desiredPosition = Target.position + Quaternion.Euler(0, rotY, 0) * offset;

            // 목표 위치로 부드럽게 이동
            transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * followSpeed);

            // 카메라가 항상 플레이어를 바라보도록 설정
            realCamera.LookAt(Target.position + Vector3.up * offset.y);
        }
    }
}
