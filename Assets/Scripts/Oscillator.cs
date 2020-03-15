using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 0f, 0f);
    [SerializeField] Vector3 rotationVector;
    [SerializeField] float period = 2f; // [s]

    Vector3 startingPosition;
    Quaternion startingRotation;

    void Start()
    {
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }


    void Update()
    {
        if (period < Mathf.Epsilon) return;
        const float tau = Mathf.PI * 2;
        float cycles = Time.time / period;
        float rawSine = Mathf.Sin(tau * cycles); // goes from -1 to 1
        float rate = rawSine / 2f + 0.5f;

        transform.position = startingPosition + movementVector * rate;
        transform.rotation = startingRotation * Quaternion.Euler(rotationVector * rate);
    }
}
