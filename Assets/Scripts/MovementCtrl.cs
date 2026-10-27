using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovementCtrl : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    private InputActionMap _inputActionMap;
    private InputAction _move;

    public float velocidad = 5f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        _inputActionMap = inputActionAsset.FindActionMap("Player");
        _move = _inputActionMap.FindAction("Move");
        _inputActionMap.Enable();
    }

    // FixedUpdate se usa para las fisicas del Rigidbody
    void FixedUpdate()
    {
        // Leer WASD (x = izquierda/derecha, y = adelante/atras)
        Vector2 movimiento = _move.ReadValue<Vector2>();

        // Convertir a una direccion segun hacia donde mira el jugador
        Vector3 direccion = transform.right * movimiento.x + transform.forward * movimiento.y;

        // Mover con la velocidad del Rigidbody para que choque con las paredes.
        // Se conserva la velocidad en Y para no romper la gravedad ni el salto.
        rb.linearVelocity = new Vector3(
            direccion.x * velocidad,
            rb.linearVelocity.y,
            direccion.z * velocidad
        );
    }
}
