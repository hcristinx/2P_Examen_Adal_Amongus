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
}
