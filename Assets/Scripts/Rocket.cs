using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 1000;
    [SerializeField] float rcsThrustRotation = 100;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody m_rigidbody;
    AudioSource audioSource;
    float audioVolumeDefault;
    bool collisionsDisabled = false;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Start is called before the first frame update
    void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioVolumeDefault = audioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }

        //if (Debug.isDebugBuild)
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
            return;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsDisabled) return; // ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                // do nothing here
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.Stop();
                audioSource.volume = audioVolumeDefault;
                audioSource.PlayOneShot(success);
                successParticles.Play();
                Invoke("LoadNextScene", levelLoadDelay);
                break;
            default:
                state = State.Dying;
                mainEngineParticles.Stop();
                audioSource.Stop();
                audioSource.volume = audioVolumeDefault * 0.5f;
                audioSource.PlayOneShot(death);
                deathParticles.Play();
                Destroy(GameObject.Find("/Rocket Ship/Parts"));
                Invoke("LoadFirstScene", levelLoadDelay);
                break;
        }

    }

    private void LoadFirstScene()
    {
        SceneManager.LoadScene(2);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1); // TODO: allow for more levels
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            float thrustMultiplier = 1f;
            float volumeMultiplier = 1f;

            // crazy mode if shift is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                thrustMultiplier = 3f;
                volumeMultiplier = 3f;
            }

            m_rigidbody.AddRelativeForce(Vector3.up * Time.deltaTime * rcsThrust * thrustMultiplier);

            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            if (!mainEngineParticles.isPlaying)
            {
                mainEngineParticles.Play();
            }
            audioSource.volume = audioVolumeDefault * volumeMultiplier;
        }
        else
        {
            mainEngineParticles.Stop();
            audioSource.Stop();
        }

    }

    private void Rotate()
    {
        m_rigidbody.freezeRotation = true; // own rotation handling

        float rotationThisFrame = rcsThrustRotation * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        m_rigidbody.freezeRotation = false; // resume rotation handling
    }
}
