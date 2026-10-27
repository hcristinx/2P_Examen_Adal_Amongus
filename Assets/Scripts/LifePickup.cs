using UnityEngine;

// Cubito de vida (Life+): es un trigger que le suma vida al jugador sin pasar de la vida maxima
public class LifePickup : MonoBehaviour
{
    public float cantidad = 25f;
    public AudioClip sonido;

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        PlayerHealth vidaJugador = FindFirstObjectByType<PlayerHealth>();
        if (vidaJugador == null)
        {
            return;
        }

        // Si la vida ya esta llena el cubito se queda en el mapa para despues
        if (!vidaJugador.Curar(cantidad))
        {
            return;
        }

        // PlayClipAtPoint sigue sonando aunque el cubito se destruya
        if (sonido != null)
        {
            AudioSource.PlayClipAtPoint(sonido, transform.position);
        }

        // Avisarle al LifeSpawner que ya hay un cubito menos en el mapa
        LifeSpawner spawner = FindFirstObjectByType<LifeSpawner>();
        if (spawner != null)
        {
            spawner.CuboRecogido();
        }

        Destroy(gameObject);
    }
}
