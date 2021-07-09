using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float vida;
    public float speed;
    public float detectionDistance;
    public float attackRange;
    public float damage;
    public GameObject player;
    private Animator animator;
    private bool nearPlayer;
    private bool following;
    private bool attacking;
    private Rigidbody rb;
    private float distance;
    private float attackTime;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine("SearchForPlayer");
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name.CompareTo("attack") == 0)
            {
                attackTime = clip.length;
            }
        }
    }
    private void Update()
    {
        if (player.GetComponent<PlayerController>() != null)
        {
            animator.SetBool("NearPlayer", nearPlayer);
            if (nearPlayer)
            {
                Vector3 lookAt = player.transform.position;
                lookAt.y = transform.position.y;
                transform.LookAt(lookAt);
                if (!following)
                {
                    StartCoroutine("FollowPlayer");
                    following = true;
                }
                if (distance <= attackRange)
                {
                    if (!attacking)
                    {
                        attacking = true;
                        StartCoroutine("AttackPlayer");
                    }
                }
                else
                {
                    animator.SetBool("Attack", false);
                    StopCoroutine("AttackPlayer");
                    attacking = false;
                    following = false;
                }
            }
            else
            {
                StopCoroutine("FollowPlayer");
                following = false;
            }
        }
        else
        {
            StopAllCoroutines();
        }
    }

    public void GetDamage(float damage)
    {
        vida -= damage;
        if (vida <= 0)
        {
            player.GetComponent<PlayerController>().AddXP(200);
            Destroy(this.gameObject);
        }
    }

    IEnumerator SearchForPlayer()
    {
        for (; ; )
        {
            distance = Vector3.Distance(transform.position, player.transform.position);
            nearPlayer = distance <= detectionDistance;
            yield return new WaitForSeconds(.2f);
        }
    }
    IEnumerator FollowPlayer()
    {
        while (distance > attackRange)
        {
            rb.MovePosition(Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime));
            yield return null;
        }
    }
    IEnumerator AttackPlayer()
    {
        StopCoroutine("FollowPlayer");
        following = false;
        animator.SetBool("Attack", true);
        yield return new WaitForSeconds(attackTime / 1.8f);
        player.GetComponent<PlayerController>().GetDamage(damage);
        attacking = false;
    }
}
