using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 5f; // 기본 속도
    [HideInInspector]
    public Vector2 input;
    Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 입력 받기
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 애니메이션 설정
        anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y)));
    }

    void FixedUpdate()
    {
        // 입력이 있으면 정규화하여 일정한 속도로 이동
        if (input.magnitude > 0.01f)
        {
            Vector2 movement = input.normalized * speed;
            rb.linearVelocity = movement;
        }
        else
        {
            // 입력이 없으면 즉시 정지 (미끄러짐 방지)
            rb.linearVelocity = Vector2.zero;
        }
    }
}
