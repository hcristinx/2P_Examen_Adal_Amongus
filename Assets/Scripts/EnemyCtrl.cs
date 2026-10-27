using UnityEngine;
using UnityEngine.AI;

public class EnemyCtrl : MonoBehaviour
{
    public Transform player;
    private NavMeshAgent navMeshAgent;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

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
    }

    void Update()
    {
        if (player != null && navMeshAgent != null && navMeshAgent.isOnNavMesh)
        {
            navMeshAgent.SetDestination(player.position);
        }
    }
}
