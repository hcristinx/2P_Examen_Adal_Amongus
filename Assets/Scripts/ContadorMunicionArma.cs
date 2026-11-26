using UnityEngine;
using TMPro;

// Muestra la municion en un letrero pegado a la espada (World Space),
// porque en VR no se ve el HUD de la pantalla.
public class ContadorMunicionArma : MonoBehaviour
{
    public Weapon arma;
    public TextMeshPro texto;

    // Ultimos valores mostrados, para no reescribir el texto en cada cuadro
    private int _ultimoCargador = -1;
    private int _ultimaReserva = -1;
    private bool _ultimoRecargando = false;

    void Update()
    {
        if (arma == null || texto == null)
        {
            return;
        }

        bool recargando = arma.EstaRecargando();

        if (arma.municionActual != _ultimoCargador || arma.municionReserva != _ultimaReserva
            || recargando != _ultimoRecargando)
        {
            if (recargando)
            {
                texto.text = "Recargando...";
            }
            else
            {
                texto.text = arma.municionActual + " / " + arma.municionReserva;
            }

            // Rojo cuando el cargador esta vacio
            texto.color = arma.municionActual > 0 ? Color.white : Color.red;

            _ultimoCargador = arma.municionActual;
            _ultimaReserva = arma.municionReserva;
            _ultimoRecargando = recargando;
        }
    }

    void LateUpdate()
    {
        // Que el letrero siempre mire hacia la camara para que se pueda leer
        if (Camera.main != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
        }
    }
}
