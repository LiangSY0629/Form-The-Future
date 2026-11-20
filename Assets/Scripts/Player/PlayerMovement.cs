using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour,ISaveable
{
    Transform playerTransform;

    public string saveName = "move";

    public Animator anim;
    public CharacterController controller;
    public Transform mainCamera;
    public float fadeTime;

    [HideInInspector]public Vector3 targetDirection;   //记忆值，角色会永远朝向这个方向，
    [HideInInspector]public Vector3 inputDirection;     //当前值，不键入方向时为零；

    Vector2 moveInput;
    bool isRunning;
    bool isLockBody;

    public ArmStats currentArmStat;

    float currentTurnSpeed = 0;     //当前速度；

    float forwardSpeed = 0;
    float rightSpeed = 0;

    readonly float gravity = -9.8f;
    float verticalVelocity;

    private void Start()
    {
        SaveManager.Instance.saveables[saveName] = this;

        playerTransform = transform;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main.transform;

    }

    private void Update()
    {
        if (CharacterManager.Instance.currentCharacter.armStat != currentArmStat)
        {
            currentArmStat = CharacterManager.Instance.currentCharacter.armStat;
            SetArmStat();
        }

        PlayerDirection();

        //判断有没有锁定视角
        if (!isLockBody)
        {
            SwitchUnlockBodyStats();
            SetUnlockBodyAnimtor();
        }
        else
        {
            SwitchLockBodyStats();
            SetLockBodyAnimtor();
        }

        CaculateGravity();

    }

    public void GetMoveInput(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    public void GetRunInput(InputAction.CallbackContext ctx)
    {
        isRunning = ctx.ReadValueAsButton();
    }

    /// <summary>
    /// 当前身体朝向未锁定，根据移动速度切换移动动画；
    /// </summary>
    void SwitchUnlockBodyStats()
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
    /// 当前身体朝向被锁定，根据移动速度切换移动动画；
    /// </summary>
    void SwitchLockBodyStats()
    {
        float trunSpeed;

        if (moveInput.sqrMagnitude < 0.01f)
            trunSpeed = 0;
        else if (!isRunning)
            trunSpeed = 2;
        else
            trunSpeed = 5;

        forwardSpeed = Mathf.MoveTowards(forwardSpeed, moveInput.y, fadeTime * Time.deltaTime);
        rightSpeed = Mathf.MoveTowards(rightSpeed, moveInput.x, fadeTime * Time.deltaTime);
        currentTurnSpeed = Mathf.MoveTowards(currentTurnSpeed, trunSpeed, fadeTime * Time.deltaTime);

        anim.SetFloat("Forward", forwardSpeed * currentTurnSpeed);
        anim.SetFloat("Right", rightSpeed * currentTurnSpeed);

        //必须三个全部为零（即当前输入为零，移动倾向为零）再置为非奔跑状态；
        if (trunSpeed == 0 && rightSpeed == 0 && forwardSpeed == 0)
        {
            anim.SetBool("Running", false);
        }
        else
        {
            anim.SetBool("Running", isRunning);
        }

    }

    /// <summary>
    /// 更改输入向量值为本地化向量
    /// </summary>
    void PlayerDirection()
    {
        Vector3 cameraDir = new Vector3(mainCamera.forward.x, 0, mainCamera.forward.z).normalized;
        Vector3 cameraRight = new Vector3(mainCamera.right.x, 0, mainCamera.right.z).normalized;

        //保证角色沿摄像机方向前进；
        if (!isLockBody)
            inputDirection = (cameraDir * moveInput.y + cameraRight * moveInput.x).normalized;
        else
            inputDirection = cameraDir;

    }

    /// <summary>
    /// 更新到新的动作组；
    /// </summary>
    void SetArmStat()
    {
        if(currentArmStat == ArmStats.Normal)
        {
            anim.SetBool("Aim", false);
            isLockBody = false;
        }
        else if(currentArmStat == ArmStats.Aim)
        {
            //切换到对应动画组，并判断需不需要锁定身体；
            anim.SetBool("Aim", true);
            isLockBody = true;
        }
    }

    /// <summary>
    /// 当前身体朝向未锁定，更新当前动画，如果有转向更新转向动画；
    /// </summary>
    void SetUnlockBodyAnimtor()
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

        if (Mathf.Abs(rad) > 0.01f)
        {
            playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, finTarget, 240 * Time.deltaTime);
        }

    }

    /// <summary>
    /// 当前身体朝向锁定，更新当前动画，如果有转向更新转向动画；
    /// </summary>
    void SetLockBodyAnimtor()
    {
        if (inputDirection == Vector3.zero)
            return;

        Quaternion finTarget = Quaternion.LookRotation(inputDirection, Vector3.up);

        playerTransform.rotation = Quaternion.RotateTowards(playerTransform.rotation, finTarget, 240 * Time.deltaTime);
    }


    private void OnAnimatorMove()
    {
        Vector3 playerDeltaPosition = anim.deltaPosition;
        playerDeltaPosition.y = verticalVelocity * Time.deltaTime;

        if(currentTurnSpeed <= 0)
        {
            playerDeltaPosition.x = 0;
            playerDeltaPosition.z = 0;
        }

        controller.Move(playerDeltaPosition);
    }


    void CaculateGravity()
    {
        if (controller.isGrounded)
        {
            //verticalVelocity = gravity * Time.deltaTime;
            verticalVelocity = -2f;
            return;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

    }
        

    public Data GetSaveData()
    {
        Data data = new()
        {
            key = saveName,
            value = JsonUtility.ToJson(playerTransform.position),
        };

        return data;
    }

    public void SetSaveData(Data data)
    {
        if (data.key != saveName)
            return;

        controller.enabled = false;
        playerTransform.position = JsonUtility.FromJson<Vector3>(data.value);
        controller.enabled = true;
    }
}
