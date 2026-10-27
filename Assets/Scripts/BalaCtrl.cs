using UnityEngine;

// Bala visual: el daño lo hace el Raycast del Weapon.
// Esta esfera solo se ve volar y desaparece al chocar.
[RequireComponent(typeof(Rigidbody))]
public class BalaCtrl : MonoBehaviour
{
    public float velocidad = 40f;
    public float tiempoVida = 2f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        rb.linearVelocity = transform.forward * velocidad;

        // Si no choca con nada, se destruye sola
        Destroy(gameObject, tiempoVida);
    }

    void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
