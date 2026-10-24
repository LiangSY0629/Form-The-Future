using Cinemachine;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Animator anim;
    public CinemachineFreeLook mainCamera;

    public float speed;
    public float turnSpeed;
    public Vector3 direction;

    float horizontal;
    float vertical;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        mainCamera = FindObjectOfType<CinemachineFreeLook>();
    }

    private void Update()
    {
        PlayerInput();    //检测玩家输入；

        anim.SetFloat("Forward", vertical);
        anim.SetFloat("Right", horizontal);
        Quaternion findir = Quaternion.Euler(0, mainCamera.m_XAxis.Value, 0);

        direction = findir * Vector3.right * horizontal + findir * Vector3.forward * vertical;
        direction.Normalize();

    }

    private void FixedUpdate()
    {

        if (horizontal == 0 && vertical == 0)
        {
            rb.velocity = Vector3.zero;
        }
        else
        {
            rb.velocity = direction * speed;

            Vector3 finDir = new Vector3(transform.rotation.x, mainCamera.m_XAxis.Value, transform.rotation.z);   //行走时触发转身，平常移动不触发；

            if (Mathf.Abs(transform.rotation.y - mainCamera.m_XAxis.Value) >= 1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(finDir), turnSpeed);
            }
        }

    }


    /// <summary>
    /// 检测玩家输入；
    /// </summary>
    void PlayerInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }


}
