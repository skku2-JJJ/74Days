using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{

    Rigidbody2D rb;
    public float speed;
    float diagonalSpeed;
    [HideInInspector]
    public Vector2 input;
    Animator anim;
    Animator weaponAnim;
    // Use this for initialization
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        weaponAnim = GameObject.FindGameObjectWithTag("HandWithWeapon").GetComponent<Animator>();
        diagonalSpeed = speed * 0.78f;
    }

    // Update is called once per frame
    void Update()
    {
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        anim.SetFloat("Speed", Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y)));
        //weaponAnim.SetFloat("Speed", Mathf.Max(Mathf.Abs(input.x), Mathf.Abs(input.y)));
    }

    void FixedUpdate()
    {
        if (input.y == 0 || input.x == 0)
            rb.linearVelocity = new Vector2(input.x * Time.fixedDeltaTime, input.y * Time.fixedDeltaTime) * speed;
        else
            rb.linearVelocity = new Vector2(input.x * Time.fixedDeltaTime, input.y * Time.fixedDeltaTime) * diagonalSpeed;
    }

}
