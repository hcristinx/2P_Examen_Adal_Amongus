using UnityEngine;

// Caja de municion: es un trigger que suma balas a la reserva del jugador
public class AmmoPickup : MonoBehaviour
{
    public int cantidad = 15;
    public AudioClip sonido;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        Weapon arma = FindFirstObjectByType<Weapon>();
        if (arma != null)
        {
            arma.AgregarMunicion(cantidad);
        }

        // PlayClipAtPoint sigue sonando aunque la caja se destruya
        if (sonido != null)
        {
            AudioSource.PlayClipAtPoint(sonido, transform.position);
        }

        Destroy(gameObject);
    }
}
