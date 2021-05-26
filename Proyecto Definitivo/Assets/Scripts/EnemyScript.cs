using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float speed;
    public float detectionDistance;
    public float attackRange;
    private Animator animator;
    private bool nearPlayer;
    private bool attacking;
    private Rigidbody rb;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine("SearchForPlayer");
    }
    private void Update()
    {
        animator.SetBool("NearPlayer", nearPlayer);
        if (nearPlayer)
        {
            Vector3 lookAt = player.transform.position;
            lookAt.y = transform.position.y;
            transform.LookAt(lookAt);
            StartCoroutine("FollowPlayer");
        }
        else
        {
            StopCoroutine("FollowPlayer");
        }
    }

    IEnumerator SearchForPlayer()
    {
        for (; ; )
        {
            nearPlayer = Vector3.Distance(transform.position, player.transform.position) < detectionDistance;
            yield return new WaitForSeconds(.2f);
        }
    }
    IEnumerator FollowPlayer()
    {
        while (Vector3.Distance(transform.position, player.transform.position) > attackRange)
        {
            attacking = false;
            animator.SetBool("Attack", attacking);
            StopCoroutine("AttackPlayer");
            rb.MovePosition(Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime));
            yield return null;
        }
        attacking = true;
        StartCoroutine("AttackPlayer");
    }
    IEnumerator AttackPlayer()
    {
        StopCoroutine("FollowPlayer");
        animator.SetBool("Attack", attacking);
        yield return null;
    }
}
