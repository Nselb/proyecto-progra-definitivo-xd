using System.Collections;
using UnityEngine;

public enum ResourceType
{
    Rock, Wood, Leather, Metal,
    All,
    Plant
}
public class Collectable : MonoBehaviour
{
    public int vida;
    public float dureza;
    public ResourceType type;

    public void Die()
    {

        Destroy(this.gameObject);

    }
}