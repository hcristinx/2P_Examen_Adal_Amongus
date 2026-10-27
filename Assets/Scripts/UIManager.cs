using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public TextMeshProUGUI textoVida;
    public TextMeshProUGUI textoMunicion;
    public TextMeshProUGUI textoOleada;
    public TextMeshProUGUI textoEliminados;

    [Header("Pantalla final")]
    public GameObject panelFinal;
    public TextMeshProUGUI textoResultado;
    public TextMeshProUGUI textoEstadisticas;

    void Start()
    {
        panelFinal.SetActive(false);
    }

    public void ActualizarVida(float vida, float vidaMaxima)
    {
        textoVida.text = "Vida: " + Mathf.CeilToInt(vida) + " / " + vidaMaxima;
    }

    public void ActualizarMunicion(int cargador, int reserva, bool recargando)
    {
        if (recargando)
        {
            textoMunicion.text = "Recargando...";
        }
        else
        {
            textoMunicion.text = "Balas: " + cargador + " / " + reserva;
        }

        // Rojo cuando el cargador esta vacio, como en su ContadorMunicion
        textoMunicion.color = cargador == 0 ? Color.red : Color.white;
    }

    public void ActualizarOleada(int oleada, int total)
    {
        textoOleada.text = "Oleada: " + oleada + " / " + total;
    }

    public void ActualizarEliminados(int eliminados)
    {
        textoEliminados.text = "Eliminados: " + eliminados;
    }

    public void MostrarFinal(string resultado, int eliminados, int oleada)
    {
        panelFinal.SetActive(true);
        textoResultado.text = resultado;
        textoEstadisticas.text = "Enemigos eliminados: " + eliminados + "\nOleada alcanzada: " + oleada;
    }
}
