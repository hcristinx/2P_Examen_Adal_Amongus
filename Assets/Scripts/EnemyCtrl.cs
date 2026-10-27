using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;
    private PlayerHealth playerHealth;

    [Header("Movimiento")]
    public float velocidad = 2f;   // Antes 3.5 (valor por defecto del NavMeshAgent)

    [Header("Ataque")]
    public float distanciaAtaque = 1.8f;
    public float danio = 10f;
    public float intervaloAtaque = 2f;   // Segundos entre cada ataque
    private float _nextAttackTime;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = velocidad;

        // Los enemigos que crea el Spawner no tienen al jugador asignado,
        // asi que lo buscan por su tag
        if (player == null)
        {
            GameObject jugador = GameObject.FindGameObjectWithTag("Player");

            if (jugador != null)
            {
                player = jugador.transform;
            }
            else
            {
                Debug.LogError("No se encontró al jugador. Revisa que tenga el tag Player.");
            }
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }

        // Que se detenga un poco antes de chocar con el jugador
        navMeshAgent.stoppingDistance = distanciaAtaque * 0.8f;
    }

    void Update()
    {
        if (player == null || navMeshAgent == null || !navMeshAgent.isOnNavMesh)
        {
            return;
        }

        navMeshAgent.SetDestination(player.position);

        // Atacar si esta cerca, respetando el tiempo entre ataques
        float distancia = Vector3.Distance(transform.position, player.position);

        if (distancia <= distanciaAtaque && Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + intervaloAtaque;

            if (playerHealth != null)
            {
                playerHealth.RecibirDanio(danio);
            }
        }
    }
}
