using System.Collections;
using UnityEngine;

public enum ResourceType
{
    Rock, Wood, Leather, Metal, All, Plant
}
public class Collectable : MonoBehaviour
{
    public GameObject destroyedPrefab;
    public int vida;
    public float dureza;
    public int dropQuantity;
    public ResourceType type;
    public void Die()
    {
        Instantiate(destroyedPrefab, this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}