using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Transform playerTransform;

    public Animator anim;
    public CharacterController controller;
    public Transform mainCamera;

    public float speed;
    public Vector3 direction;

    Vector2 moveInput;
    bool isRunning;
    bool isAiming;

    float gravity = -9.8f;
    float verticalVelocity;

    private void Start()
    {
        playerTransform = transform;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {

        SwitchPlayerStats();
        PlayerDirection();
        CaculateGravity();
        SetAnimtor();
    }

    public void GetMoveInput(InputAction.CallbackContext action)
    {
        moveInput = action.ReadValue<Vector2>();
    }

    public void GetRunInput(InputAction.CallbackContext action)
    {
        isRunning = action.ReadValueAsButton();
    }

    public void GetAimInput(InputAction.CallbackContext action)
    {
        isAiming = action.ReadValueAsButton();
    }

    void SwitchPlayerStats()
    {
        if(moveInput.magnitude == 0)
        {
            anim.SetFloat("TurnSpeed", 0, 0.2f, Time.deltaTime);
        }
        else if (!isRunning)
        {
            anim.SetFloat("TurnSpeed", 2, 0.2f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("TurnSpeed", 5, 0.2f, Time.deltaTime);
        }

    }


    /// <summary>
    /// 更改输入向量值为本地化向量
    /// </summary>
    void PlayerDirection()
    {
        Vector3 cameraDir = new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z).normalized;

        direction = (cameraDir * moveInput.y + mainCamera.right * moveInput.x).normalized;    //保证角色沿摄像机方向前进；
        direction = playerTransform.InverseTransformVector(direction);

    }


    void SetAnimtor()
    {

        float rad = Mathf.Atan2(direction.x, direction.z);

        anim.SetFloat("Turn", rad, 0.1f, Time.deltaTime);

        if(Mathf.Abs(rad) > 0.01f)
        {
            playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0);
        }

    }

    void CaculateGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = gravity * Time.deltaTime;
            return;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

    }

    private void OnAnimatorMove()
    {
        Vector3 playerDeltaPosition = anim.deltaPosition;
        playerDeltaPosition.y = verticalVelocity * Time.deltaTime;
        controller.Move(playerDeltaPosition);
    }


}
