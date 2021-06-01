using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedResource : MonoBehaviour
{
    private int destructionDate;
    private DayNightCycle dia;
    public int timeToRespawnInDays;
    public GameObject resource;

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
        Instantiate(resource, this.transform.position, Quaternion.identity);
    }
}
