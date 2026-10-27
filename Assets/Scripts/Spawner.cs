using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Enemigos")]
    public GameObject enemyPrefab;

    [Header("Puntos de aparición")]
    public Transform[] spawnPoints;

    [Header("Configuración")]
    public float tiempoEntreEnemigos = 0.8f;

    private int porGenerar = 0;
    private float nextSpawnTime;
    private GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();
    }

    // El GameManager le dice cuantos enemigos crear en la oleada
    public void GenerarOleada(int cantidad)
    {
        porGenerar = cantidad;
    }

    void Update()
    {
        // Crear los enemigos de uno en uno, con un tiempo entre cada uno
        if (porGenerar > 0 && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            porGenerar--;
            nextSpawnTime = Time.time + tiempoEntreEnemigos;
        }
    }

    void SpawnEnemy()
    {
        // Comprobar que exista el prefab y haya puntos disponibles
        if (enemyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogError("Spawner sin prefab o sin puntos de aparición.");
            return;
        }

        // Elegir un punto de aparición al azar
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform punto = spawnPoints[randomIndex];

        Instantiate(enemyPrefab, punto.position, punto.rotation);

        if (gameManager != null)
        {
            gameManager.EnemigoGenerado();
        }
    }

    public bool TerminoDeGenerar()
    {
        return porGenerar == 0;
    }
}
