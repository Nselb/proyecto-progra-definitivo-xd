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
            transform.position = hit.point;
        }
    }
    IEnumerator CheckForRespawn()
    {
        while (destructionDate + timeToRespawnInDays > dia.day)
        {
            yield return new WaitForSeconds(dia.dayLenght);
        }
        var obj = Instantiate(resource, this.transform.position, Quaternion.identity);
        obj.transform.localScale = new Vector3(Random.Range(0.5f,1.8f),Random.Range(0.5f,1.8f),Random.Range(0.5f,1.8f));
        if (Physics.Raycast(obj.transform.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayer))
        {
            obj.transform.position = hit.point + Vector3.up*0.5f;
        }
        Destroy(this.gameObject);
    }
}
