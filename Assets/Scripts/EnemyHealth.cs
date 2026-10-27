using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float vidaMaxima = 50f;
    private float vida;

    void Start()
    {
        vida = vidaMaxima;
    }

    public void RecibirDanio(float cantidad)
    {
        vida -= cantidad;
        Debug.Log(gameObject.name + " recibió daño. Vida: " + vida);

        if (vida <= 0)
        {
            Morir();
        }
    }

    void Morir()
    {
        Destroy(gameObject);
    }
}
