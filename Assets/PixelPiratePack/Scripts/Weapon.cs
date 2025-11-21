using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float angleOffset;
    public Transform player;
    public float damage;
    public Animator anim;
    public bool canAttack = true;
    public Collider2D weaponCollider;
    public bool facingRight;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        if (angle > 93 || angle < -93)
        {
            player.localRotation = Quaternion.Euler(player.rotation.x, 180, player.localScale.z);
            angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            facingRight = false;
        }
        else
        {
            player.localRotation = Quaternion.Euler(player.rotation.x, 0, player.localScale.z);
            facingRight = true;

        }

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            anim.SetTrigger("Attack");
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            weaponCollider.enabled = true;
        }
        else
        {
            weaponCollider.enabled = false;
        }
    }
}
