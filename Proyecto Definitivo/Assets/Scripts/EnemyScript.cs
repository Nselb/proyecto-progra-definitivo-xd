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
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("SearchForPlayer");
        speed /= 1000f;
    }
    private void Update()
    {
        animator.SetBool("NearPlayer", nearPlayer);
        if (nearPlayer)
        {
            transform.LookAt(player.transform);
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
            transform.position += transform.forward * speed;
            yield return new WaitForSeconds(.5f);
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
