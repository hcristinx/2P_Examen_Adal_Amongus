using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float vidaMaxima = 100f;
    private float vida;

    private GameManager gameManager;
    private UIManager ui;

    void Start()
    {
        vida = vidaMaxima;
        gameManager = FindFirstObjectByType<GameManager>();
        ui = FindFirstObjectByType<UIManager>();

        if (ui != null)
        {
            ui.ActualizarVida(vida, vidaMaxima);
        }
    }

    public void RecibirDanio(float cantidad)
    {
        if (vida <= 0)
        {
            return;
        }

        vida -= cantidad;
        if (vida < 0) vida = 0;

        if (ui != null)
        {
            ui.ActualizarVida(vida, vidaMaxima);
        }

        if (vida <= 0)
        {
            gameManager.Derrota();
        }
    }

    // La usa el cubito de vida. Regresa false si no se pudo curar (vida llena o muerto)
    public bool Curar(float cantidad)
    {
        if (vida <= 0 || vida >= vidaMaxima)
        {
            return false;
        }

        vida += cantidad;
        if (vida > vidaMaxima) vida = vidaMaxima;

        if (ui != null)
        {
            ui.ActualizarVida(vida, vidaMaxima);
        }

        return true;
    }
}
