using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Oleadas")]
    public int numeroOleadas = 4;
    public int enemigosIniciales = 3;
    public int incrementoPorOleada = 2;
    public float tiempoEntreOleadas = 3f;

    [Header("Estado (solo lectura)")]
    public int oleadaActual = 0;
    public int enemigosVivos = 0;
    public int enemigosEliminados = 0;
    public bool juegoTerminado = false;

    private Spawner spawner;
    private UIManager ui;

    void Start()
    {
        Time.timeScale = 1f;
        spawner = FindFirstObjectByType<Spawner>();
        ui = FindFirstObjectByType<UIManager>();

        IniciarOleada();
    }

    public void IniciarOleada()
    {
        if (juegoTerminado)
        {
            return;
        }

        oleadaActual++;

        // Cada oleada trae mas enemigos que la anterior
        int cantidad = enemigosIniciales + incrementoPorOleada * (oleadaActual - 1);
        spawner.GenerarOleada(cantidad);

        Debug.Log("Oleada " + oleadaActual + ": " + cantidad + " enemigos");

        if (ui != null)
        {
            ui.ActualizarOleada(oleadaActual, numeroOleadas);
            ui.ActualizarEliminados(enemigosEliminados);
        }
    }

    // Lo llama el Spawner cada vez que crea un enemigo
    public void EnemigoGenerado()
    {
        enemigosVivos++;
    }

    // Lo llama EnemyHealth cuando un enemigo muere
    public void EnemigoEliminado()
    {
        enemigosVivos--;
        enemigosEliminados++;

        if (ui != null)
        {
            ui.ActualizarEliminados(enemigosEliminados);
        }

        // La oleada termina cuando no quedan vivos y el Spawner ya genero a todos
        if (enemigosVivos <= 0 && spawner.TerminoDeGenerar())
        {
            if (oleadaActual >= numeroOleadas)
            {
                Victoria();
            }
            else
            {
                Invoke(nameof(IniciarOleada), tiempoEntreOleadas);
            }
        }
    }

    public void Victoria()
    {
        TerminarJuego("¡VICTORIA!");
    }

    public void Derrota()
    {
        TerminarJuego("DERROTA");
    }

    void TerminarJuego(string resultado)
    {
        if (juegoTerminado)
        {
            return;
        }

        juegoTerminado = true;

        // Pausar el juego y liberar el cursor para poder dar clic al boton
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (ui != null)
        {
            ui.MostrarFinal(resultado, enemigosEliminados, oleadaActual);
        }
    }

    // Lo llama el boton Reiniciar
    public void Reiniciar()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
