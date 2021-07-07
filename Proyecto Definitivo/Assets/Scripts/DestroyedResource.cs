using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedResource : MonoBehaviour
{
    private int destructionDate;
    private DayNightCycle dia;
    public int timeToRespawnInDays;
    public GameObject resource;
    public LayerMask floorLayer;
    private void Start()
    {
        dia = FindObjectOfType<DayNightCycle>();
        destructionDate = dia.day;
        StartCoroutine("CheckForRespawn");
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
        {
            Debug.Log(hit.collider.gameObject.name);
            transform.position = hit.point;
        }
    }
    IEnumerator CheckForRespawn()
    {
        while (destructionDate + timeToRespawnInDays > dia.day)
        {
            yield return new WaitForSeconds(dia.dayLenght);
        }
        Instantiate(resource, this.transform.position, Quaternion.identity);
    }
}
