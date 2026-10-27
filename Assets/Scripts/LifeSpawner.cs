using UnityEngine;
using UnityEngine.AI;

// Pone cubitos de vida (Life+) en lugares al azar del mapa
public class LifeSpawner : MonoBehaviour
{
    [Header("Cubito de vida")]
    public GameObject lifePrefab;

    [Header("Zona del mapa (de aquí salen los puntos al azar)")]
    public float minX = -20f;
    public float maxX = 10f;
    public float minZ = -10f;
    public float maxZ = 10f;

    [Header("Configuración")]
    public int maximoEnMapa = 3;
    public float tiempoEntreCubos = 10f;

    private int cubosEnMapa = 0;
    private float nextSpawnTime;

    void Update()
    {
        // Crear un cubito cada cierto tiempo, sin pasar del maximo
        if (cubosEnMapa < maximoEnMapa && Time.time >= nextSpawnTime)
        {
            CrearCubo();
        }
    }

    void CrearCubo()
    {
        // Elegir un punto al azar dentro de la zona
        Vector3 punto = new Vector3(Random.Range(minX, maxX), 1f, Random.Range(minZ, maxZ));

        // Buscar el piso (NavMesh) mas cercano a ese punto.
        // Si el punto cayo fuera del mapa no encuentra piso y se intenta otra vez en el siguiente frame
        NavMeshHit hit;
        if (NavMesh.SamplePosition(punto, out hit, 3f, NavMesh.AllAreas))
        {
            Instantiate(lifePrefab, hit.position + Vector3.up * 0.5f, Quaternion.identity);
            cubosEnMapa++;
            nextSpawnTime = Time.time + tiempoEntreCubos;
        }
    }

    // Lo llama LifePickup cuando el jugador recoge un cubito
    public void CuboRecogido()
    {
        cubosEnMapa--;
    }
}
