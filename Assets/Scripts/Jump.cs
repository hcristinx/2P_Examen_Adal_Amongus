using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    InputAction jumpAction;

    public float alturaSalto = 1.5f;
    // Distancia hacia abajo para revisar si hay piso debajo del jugador
    public float distanciaSuelo = 1.1f;

    //Variable para audio source
    public AudioSource jumpSound;

    private Rigidbody rb;

    private void OnEnable()
    {
        inputActionAsset.FindActionMap("Player").Enable();
    }

    private void OnDisable()
    {
        inputActionAsset.FindActionMap("Player").Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jumpAction = inputActionAsset.FindActionMap("Player").FindAction("Jump");
    }

    void Update()
    {
        // Rayo hacia abajo: si toca algo, el jugador esta en el suelo
        bool estaEnSuelo = Physics.Raycast(transform.position, Vector3.down, distanciaSuelo);

        if (jumpAction.WasPressedThisFrame() && estaEnSuelo)
        {
            float fuerzaSalto = Mathf.Sqrt(2f * alturaSalto * Mathf.Abs(Physics.gravity.y));
            rb.AddForce(Vector3.up * fuerzaSalto, ForceMode.VelocityChange);

            if (jumpSound != null)
            {
                jumpSound.Play();
            }
        }
    }
}
