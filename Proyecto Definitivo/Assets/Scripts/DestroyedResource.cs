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
    }
    IEnumerator CheckForRespawn()
    {
        while (destructionDate + timeToRespawnInDays > dia.day)
        {
            yield return new WaitForSeconds(dia.dayLenght);
        }
        var obj = Instantiate(resource, this.transform.position, Quaternion.identity);
        if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
        {
            obj.transform.position = hit.point;
        }
    }
}
