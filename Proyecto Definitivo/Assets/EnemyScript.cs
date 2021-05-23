using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public GameObject player;
    public float detectionDistance;
    private Animator animator;
    private bool nearPlayer;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("SearchForPlayer");
    }
    private void Update()
    {
        animator.SetBool("NearPlayer", nearPlayer);
        if (nearPlayer)
        {
            transform.LookAt(player.transform);
            StartCoroutine("FollowPlayer");
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
        Debug.Log("Following");
        yield return new WaitForSeconds(1f);
    }
}
