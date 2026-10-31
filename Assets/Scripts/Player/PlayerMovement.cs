using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Transform playerTransform;

    public Animator anim;
    public CharacterController controller;
    public Transform mainCamera;

    public Vector3 targetDirection;   //记忆值，角色会永远朝向这个方向，
    public Vector3 inputDirection;     //当前值，不键入方向时为零；

    Vector2 moveInput;
    bool isRunning;
    bool isAiming;

    float currentTurnSpeed = 0;     //当前速度；
    readonly float gravity = -9.8f;
    float verticalVelocity;

    private void Start()
    {
        playerTransform = transform;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;

        //Cursor.lockState = CursorLockMode.Locked;
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
        float turnSpeed;


        if (moveInput.sqrMagnitude < 0.01f)
            turnSpeed = 0;
        else if (!isRunning)
            turnSpeed = 2;
        else
            turnSpeed = 5;

        currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, turnSpeed, 8 * Time.deltaTime);

        anim.SetFloat("TurnSpeed", currentTurnSpeed);
    }


    /// <summary>
    /// 更改输入向量值为本地化向量
    /// </summary>
    void PlayerDirection()
    {
        Vector3 cameraDir = new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(mainCamera.right.x, 0, mainCamera.right.z).normalized;

        inputDirection = (cameraDir * moveInput.y + cameraRight * moveInput.x).normalized;    //保证角色沿摄像机方向前进；

    }


    void SetAnimtor()
    {
        if (inputDirection != Vector3.zero)
            targetDirection = inputDirection;        //input direction不为零时更新，确保不会被归零；
        else return;

        if (targetDirection == Vector3.zero)         //转向是零是返回；
            return;

        Quaternion finTarget = Quaternion.LookRotation(targetDirection, Vector3.up);

        Vector3 playerDirection = playerTransform.InverseTransformVector(targetDirection);

        float rad = Mathf.Atan2(playerDirection.x, playerDirection.z);

        anim.SetFloat("Turn", rad, 0.1f, Time.deltaTime);

        if (Mathf.Abs(rad) > 0.1f)
        {
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, finTarget, 240 * Time.deltaTime);

        }


    }



    private void OnAnimatorMove()
    {
        Vector3 playerDeltaPosition = anim.deltaPosition;
        playerDeltaPosition.y = verticalVelocity * Time.deltaTime;

        if(currentTurnSpeed > 0)
            controller.Move(playerDeltaPosition);
    }

    void CaculateGravity()
    {
        if (controller.isGrounded)
        {
            verticalVelocity = 0;
            return;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

    }

}
