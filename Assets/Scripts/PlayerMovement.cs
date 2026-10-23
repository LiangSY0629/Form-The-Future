using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody rb;
    public Animator anim;

    public float speed;
    public Vector3 direction;

    float rotation;
    float horizontal;
    float vertical;
    bool isTurn = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        PlayerInput();

        if(Mathf.Abs(horizontal) > 0 || Mathf.Abs(vertical) > 0)
        {
            PlayerMove();
        }
        else
        {
            rb.velocity = Vector3.zero;
            anim.SetBool("Walk", false);
        }

    }

    void PlayerInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
    }

    void PlayerMove()
    {
        direction = Vector3.forward * vertical + Vector3.right * horizontal;
        float newRotation;

        if (horizontal == 0)
        {
            if (vertical < 0)
            {
                newRotation = 180;
            }
            else newRotation = 0;
        }
        else newRotation = horizontal * 90;

        if(Mathf.Abs(rotation - newRotation) > 5 && !isTurn)
        {
            rotation = newRotation;
            StartCoroutine(SetRotation());
        }

        rb.velocity = direction * speed;

        anim.SetBool("Walk", true);

    }

    IEnumerator SetRotation()
    {
        isTurn = true;
        float newRotation;
        float oldRotation;

        oldRotation = transform.rotation.y;

        for(float t = 0; t < 0.1f; t += Time.deltaTime)
        {
            newRotation = Mathf.Lerp(oldRotation, rotation, t/0.1f);

            transform.rotation = Quaternion.Euler(0, newRotation, 0);

            yield return null;
        }
        isTurn = false;

    }

}
