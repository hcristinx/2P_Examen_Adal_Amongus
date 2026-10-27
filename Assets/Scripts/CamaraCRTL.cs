using UnityEngine; 
using UnityEngine.InputSystem; 


public class CamaraCRTL : MonoBehaviour 
{
    public InputActionAsset inputActionAsset; // Referencia al recurso que contiene los mapas de acciones de entrada
    private InputActionMap _inputActionMap; // Referencia interna al mapa de acciones específico


    [Header("Referencias")] // Encabezado visual en el Inspector de Unity
    [Tooltip("Cuerpo que rota en horizontal (yaw). Si lo dejas vacío, rota este mismo objeto.")] // Ayuda visual al pasar el ratón en el Inspec
    [SerializeField] private Transform yawTarget; // Referencia al objeto que rotará lateralmente (ej. el jugador)
    [Header("Ajustes")] // Encabezado visual para los ajustes de la cámara
    [SerializeField] private float sensitivity = 0.3f; // Sensibilidad del movimiento del ratón
    [SerializeField] private float minPitch = -80f; // Límite inferior para la rotación vertical
    [SerializeField] private float maxPitch = 80f; // Límite superior para la rotación vertical
    private InputAction _look; // Acción de entrada para detectar el movimiento de mirada
    private float pitch; // Variable para almacenar la rotación vertical acumulada
    
    // Start se llama una vez antes de la primera ejecución de Update
    void Start()
    {
        _inputActionMap = inputActionAsset.FindActionMap("Player"); // Busca el mapa de acciones llamado "Player"
        _look = _inputActionMap.FindAction("Look"); // Busca la acción "Look" dentro del mapa
        
        if (yawTarget == null) yawTarget = transform; // Si no se asignó un objetivo, usa este mismo objeto para el yaw
        SetCursorLocked(true); // Bloquea y oculta el cursor al iniciar
    }


    // Update se llama en cada frame
    void Update()
    {
        // Juego en pausa (pantalla final): dejar el cursor libre para el boton
        if (Time.timeScale == 0f) return;

        // Si se presiona la tecla Escape, libera el cursor
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            SetCursorLocked(false);


        // Si el cursor no está bloqueado y se hace clic izquierdo, vuelve a capturarlo
        else if (Cursor.lockState != CursorLockMode.Locked &&
                 Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            SetCursorLocked(true);
    }
    
    // LateUpdate se llama después de todos los Updates (ideal para cámaras)
    private void LateUpdate()
    {
        if (Time.timeScale == 0f) return; // Juego en pausa: no mover la camara
        if (Cursor.lockState != CursorLockMode.Locked) return; // Si el cursor está libre, no procesa la rotación


        Vector2 delta = _look.ReadValue<Vector2>() * sensitivity; // Lee el movimiento del ratón y aplica la sensibilidad


        // Yaw: rota el cuerpo asignado sobre el eje Y global
        yawTarget.Rotate(Vector3.up, delta.x, Space.World);


        // Pitch: calcula la rotación vertical en X con límites (clamping)
        pitch = Mathf.Clamp(pitch - delta.y, minPitch, maxPitch);
        // Aplica la rotación local en el eje X para mirar arriba/abajo
        transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }


    // Método auxiliar para controlar el estado del cursor
    private void SetCursorLocked(bool locked)
    {
        // Si locked es true, bloquea el cursor al centro; si no, lo libera
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        // Oculta el cursor si está bloqueado
        Cursor.visible = !locked;
    }
}

