using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

// Script de entrada para VR: el gatillo y el boton del control solo llaman a los
// mismos Disparar() y Recargar() de Weapon que usaba el mouse en el FPS.
[RequireComponent(typeof(XRGrabInteractable))]
public class EntradaArmaVR : MonoBehaviour
{
    public Weapon arma;

    [Header("Vibracion al disparar")]
    public float intensidadVibracion = 0.6f;
    public float duracionVibracion = 0.1f;

    private XRGrabInteractable agarrable;
    private InputAction _reload;

    void Awake()
    {
        agarrable = GetComponent<XRGrabInteractable>();

        if (arma == null)
        {
            arma = GetComponent<Weapon>();
        }

        // Recargar con A o X (boton principal de cualquier control). R para probar en el editor
        _reload = new InputAction("Recargar", InputActionType.Button);
        _reload.AddBinding("<XRController>{RightHand}/primaryButton");
        _reload.AddBinding("<XRController>{LeftHand}/primaryButton");
        _reload.AddBinding("<Keyboard>/r");
    }

    void OnEnable()
    {
        // Evento Activated: se lanza al presionar el gatillo mientras se tiene la espada
        agarrable.activated.AddListener(AlPresionarGatillo);
        _reload.Enable();
    }

    void OnDisable()
    {
        agarrable.activated.RemoveListener(AlPresionarGatillo);
        _reload.Disable();
    }

    void Update()
    {
        // Solo recargar si alguien tiene la espada en la mano
        if (agarrable.isSelected && _reload.WasPressedThisFrame())
        {
            arma.Recargar();
        }
    }

    void AlPresionarGatillo(ActivateEventArgs args)
    {
        int antes = arma.municionActual;
        arma.Disparar();

        // Vibrar el control solo si de verdad salio un disparo
        if (arma.municionActual < antes && args.interactorObject is XRBaseInputInteractor control)
        {
            control.SendHapticImpulse(intensidadVibracion, duracionVibracion);
        }
    }
}
