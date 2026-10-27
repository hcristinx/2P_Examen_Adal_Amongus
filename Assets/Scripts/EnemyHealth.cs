using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float vidaMaxima = 50f;
    private float vida;
    private bool muerto = false;

    [Header("Munición soltada")]
    public GameObject cajaMunicion;
    [Range(0f, 1f)]
    public float probDrop = 0.3f;      // 0.3 = 30%

    void Start()
    {
        vida = vidaMaxima;
    }

    public void RecibirDanio(float cantidad)
    {
        if (muerto)
        {
            return;
        }

        vida -= cantidad;
        Debug.Log(gameObject.name + " recibió daño. Vida: " + vida);

        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        muerto = true;

        IntentarSoltarMunicion();

        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.EnemigoEliminado();
        }

        Destroy(gameObject);
    }

    void IntentarSoltarMunicion()
    {
        if (cajaMunicion == null)
        {
            return;
        }

        // Si el jugador no tiene balas (cargador y reserva en 0) la caja sale siempre
        Weapon arma = FindFirstObjectByType<Weapon>();
        float probabilidad = probDrop;

        if (arma != null && !arma.TieneBalas())
        {
            probabilidad = 1f;
        }

        if (Random.value < probabilidad)
        {
            Instantiate(cajaMunicion, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        }
    }
}
