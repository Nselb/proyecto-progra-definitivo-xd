using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public Transform Arenaout;
    // Start is called before the first frame update
    void Start()
    {      
    }
    // Update is called once per frame
    void Update()
    {      
    }
    public void OnTriggerEnter(Collider other)
    {
        other.transform.position = Arenaout.transform.position;
    }
}
