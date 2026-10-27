using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    public InputActionAsset inputActionAsset;
    private InputActionMap _inputActionMap;
    private InputAction _fire;
    private InputAction _reload;

    [Header("Disparo")]
    public float fireRate = 0.3f;      // Segundos entre cada disparo
    public float danio = 25f;          // Vida que le quita a un enemigo
    public float alcance = 100f;       // Distancia maxima del Raycast

    [Header("Bala visual")]
    public GameObject balaPrefab;      // Esfera que se ve salir de la pistola
    public Transform puntoDisparo;     // Boca de la pistola

    [Header("Municion")]
    public int capacidadCargador = 10;
    public int municionActual = 10;
    public int reservaMaxima = 40;
    public int municionReserva = 40;
    public float tiempoRecarga = 1.5f;

    [Header("Reserva automatica")]
    // Si el jugador se queda sin balas (cargador y reserva en 0),
    // despues de este tiempo la reserva se rellena sola
    public float tiempoParaRestaurar = 5f;

    [Header("Sonidos")]
    public AudioSource audioSource;
    public AudioClip sonidoDisparo;
    public AudioClip sonidoVacio;
    public AudioClip sonidoRecarga;

    private float _nextFireTime;
    private float _finRecarga;
    private bool _recargando = false;
    private float _tiempoSinBalas = 0f;
    private UIManager ui;

    void Start()
    {
        _inputActionMap = inputActionAsset.FindActionMap("Player");
        _fire = _inputActionMap.FindAction("Attack");
        _reload = _inputActionMap.FindAction("Reload");
        _inputActionMap.Enable();

        ui = FindFirstObjectByType<UIManager>();
    }

    void Update()
    {
        // Juego en pausa (pantalla final): no disparar
        if (Time.timeScale == 0f)
        {
            return;
        }

        // Terminar la recarga cuando ya paso el tiempo
        if (_recargando && Time.time >= _finRecarga)
        {
            TerminarRecarga();
        }

        if (_fire.triggered)
        {
            Disparar();
        }

        if (_reload.triggered)
        {
            Recargar();
        }

        RevisarReserva();

        if (ui != null)
        {
            ui.ActualizarMunicion(municionActual, municionReserva, _recargando);
        }
    }

    public void Disparar()
    {
        // Respetar la cadencia y no disparar mientras recarga
        if (Time.time < _nextFireTime || _recargando)
        {
            return;
        }

        _nextFireTime = Time.time + fireRate;

        // Cargador vacio: solo suena el clic
        if (municionActual <= 0)
        {
            audioSource.PlayOneShot(sonidoVacio);
            return;
        }

        municionActual--;
        audioSource.PlayOneShot(sonidoDisparo);

        // Rayo desde el centro de la camara hacia donde mira
        RaycastHit hit;
        // QueryTriggerInteraction.Ignore: el rayo atraviesa triggers como las cajas de municion
        bool golpeo = Physics.Raycast(transform.position, transform.forward, out hit, alcance,
            Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

        CrearBala(golpeo, hit);

        if (golpeo)
        {
            Debug.Log("El disparo golpeó: " + hit.collider.name);

            // Buscar la vida del enemigo en el objeto golpeado o en sus padres
            EnemyHealth enemigo = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemigo != null)
            {
                enemigo.RecibirDanio(danio);
            }
        }
    }

    void CrearBala(bool golpeo, RaycastHit hit)
    {
        if (balaPrefab == null || puntoDisparo == null)
        {
            return;
        }

        // La bala apunta hacia donde pego el rayo (o hacia el frente si no pego en nada)
        Vector3 destino = golpeo ? hit.point : transform.position + transform.forward * alcance;
        Quaternion direccion = Quaternion.LookRotation(destino - puntoDisparo.position);

        GameObject bala = Instantiate(balaPrefab, puntoDisparo.position, direccion);

        // Que la bala no choque con el jugador al salir
        Collider colBala = bala.GetComponent<Collider>();
        foreach (Collider col in GetComponentsInParent<Collider>())
        {
            Physics.IgnoreCollision(colBala, col);
        }
    }

    public void Recargar()
    {
        // No recargar si ya esta recargando, si el cargador esta lleno o si no hay reserva
        if (_recargando || municionActual >= capacidadCargador || municionReserva <= 0)
        {
            return;
        }

        _recargando = true;
        _finRecarga = Time.time + tiempoRecarga;
        audioSource.PlayOneShot(sonidoRecarga);
    }

    void TerminarRecarga()
    {
        _recargando = false;

        // Pasar balas de la reserva al cargador
        int diferencia = capacidadCargador - municionActual;
        int recarga = Mathf.Min(diferencia, municionReserva);

        municionActual += recarga;
        municionReserva -= recarga;
    }

    void RevisarReserva()
    {
        if (TieneBalas())
        {
            _tiempoSinBalas = 0f;
            return;
        }

        // Contar cuanto tiempo lleva el jugador sin ninguna bala
        _tiempoSinBalas += Time.deltaTime;

        if (_tiempoSinBalas >= tiempoParaRestaurar)
        {
            municionReserva = reservaMaxima;
            _tiempoSinBalas = 0f;
        }
    }

    // Cuenta el cargador y la reserva
    public bool TieneBalas()
    {
        return municionActual > 0 || municionReserva > 0;
    }

    // La usa la caja de municion
    public void AgregarMunicion(int cantidad)
    {
        municionReserva += cantidad;
    }

    public bool EstaRecargando()
    {
        return _recargando;
    }
}
