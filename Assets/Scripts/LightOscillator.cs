using UnityEngine;

public class LightOscillator : MonoBehaviour
{
    [SerializeField] float dimmPeriod = 2f; // [s]
    [SerializeField] float timeDarkness = 1f; // [s]

    float startingIntensity;
    Light light;
    float startingTime;


    void Start()
    {
        light = GetComponent<Light>();
        startingIntensity = light.intensity;
        startingTime = Time.time;
    }


    void Update()
    {
        if (dimmPeriod < Mathf.Epsilon) return;

        float timeInWindow = (Time.time - startingTime) % (dimmPeriod + timeDarkness);
        if (timeInWindow < dimmPeriod)
        {
            float rate = Mathf.Sin(Mathf.PI * (timeInWindow / dimmPeriod)); // goes from -1 to 1
            light.intensity = startingIntensity * rate;
        }
        else
        {
            light.intensity = 0f;
        }

    }
}
