using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time;
    public int day;
    public float dayLenght;
    public float startTime = 0.2f;
    private float timeRate;
    public Vector3 noon;

    [Header("Sun Settings")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon Settings")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensity;
    public AnimationCurve reflectionsIntensity;

    private void Start()
    {
        timeRate = 1.0f / dayLenght;
        time = startTime;
    }

    private void Update()
    {
        time += timeRate * Time.deltaTime;
        if (time >= 1.0f)
        {
            day++;
            time = 0.0f;
        }
        sun.transform.eulerAngles = (time - 0.25f) * noon * 4f;
        moon.transform.eulerAngles = (time - 0.75f) * noon * 4f;
        sun.intensity = sunIntensity.Evaluate(time);
        moon.intensity = moonIntensity.Evaluate(time);
        if (sun.intensity == 0 && sun.gameObject.activeInHierarchy) sun.gameObject.SetActive(false);
        else if (sun.intensity > 0 && !sun.gameObject.activeInHierarchy) sun.gameObject.SetActive(true);
        if (moon.intensity == 0 && moon.gameObject.activeInHierarchy) moon.gameObject.SetActive(false);
        else if (moon.intensity > 0 && !moon.gameObject.activeInHierarchy) moon.gameObject.SetActive(true);

        RenderSettings.ambientIntensity = lightingIntensity.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionsIntensity.Evaluate(time);
    }
}
