using UnityEngine;
using UnityEngine.InputSystem;

// Script de entrada: solo lee el mouse/teclado y le pide al arma que dispare o recargue.
// Toda la logica (municion, raycast, sonidos) vive en Weapon.
public class EntradaArma : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    public Weapon arma;

    private InputActionMap _inputActionMap;
    private InputAction _fire;
    private InputAction _reload;

    void Start()
    {
        _inputActionMap = inputActionAsset.FindActionMap("Player");
        _fire = _inputActionMap.FindAction("Attack");
        _reload = _inputActionMap.FindAction("Reload");
        _inputActionMap.Enable();

        // Si no se asigno en el Inspector, usar el arma de este mismo objeto
        if (arma == null)
        {
            arma = GetComponent<Weapon>();
        }
    }

    void Update()
    {
        // Juego en pausa (pantalla final): no disparar
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (_fire.triggered)
        {
            arma.Disparar();
        }

        if (_reload.triggered)
        {
            arma.Recargar();
        }
    }
}
