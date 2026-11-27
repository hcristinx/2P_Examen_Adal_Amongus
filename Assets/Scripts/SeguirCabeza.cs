using UnityEngine;

// Pone este Canvas (World Space) enfrente de la cabeza del jugador y lo va siguiendo
// con suavidad, para que en VR se vean el HUD y la pantalla final.
public class SeguirCabeza : MonoBehaviour
{
    public float distancia = 1.5f;     // Metros enfrente de la cabeza
    public float altura = -0.1f;       // Un poco abajo de los ojos
    public float suavidad = 3f;        // Que tan rapido alcanza a la cabeza

    private Transform cabeza;

    void Start()
    {
        if (Camera.main != null)
        {
            cabeza = Camera.main.transform;
            // Empezar ya en su lugar
            transform.position = PuntoDeseado();
            transform.rotation = RotacionDeseada();
        }
    }

    void LateUpdate()
    {
        if (cabeza == null)
        {
            return;
        }

        // unscaledDeltaTime: tambien se mueve cuando el juego esta en pausa (pantalla final)
        float t = suavidad * Time.unscaledDeltaTime;
        transform.position = Vector3.Lerp(transform.position, PuntoDeseado(), t);
        transform.rotation = Quaternion.Slerp(transform.rotation, RotacionDeseada(), t);
    }

    Vector3 PuntoDeseado()
    {
        // Solo la direccion horizontal de la mirada, para que no se ladee al ver arriba o abajo
        Vector3 frente = cabeza.forward;
        frente.y = 0f;
        frente.Normalize();
        return cabeza.position + frente * distancia + Vector3.up * altura;
    }

    Quaternion RotacionDeseada()
    {
        Vector3 haciaFuera = transform.position - cabeza.position;
        haciaFuera.y = 0f;
        return Quaternion.LookRotation(haciaFuera);
    }
}
