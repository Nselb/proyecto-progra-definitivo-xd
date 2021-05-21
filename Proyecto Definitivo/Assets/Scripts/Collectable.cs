using System.Collections;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public int vida;
    public float dureza;

    public void Die()
    {

        Destroy(this.gameObject);

    }
}