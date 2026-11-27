using UnityEngine;

// Blanco de la zona de practica: cuando le disparan cambia de color un momento
// y suena, para probar el disparo antes de ir con los enemigos.
public class Blanco : MonoBehaviour
{
    public Renderer diana;                 // La parte que cambia de color
    public Color colorGolpe = Color.green;
    public float tiempoColor = 1f;         // Segundos que dura el color de golpe
    public AudioSource sonido;

    private Color colorOriginal;
    private float volverEn = -1f;

    void Start()
    {
        if (diana == null)
        {
            diana = GetComponentInChildren<Renderer>();
        }

        colorOriginal = diana.material.color;
    }

    void Update()
    {
        // Regresar a su color despues del tiempo
        if (volverEn > 0f && Time.time >= volverEn)
        {
            diana.material.color = colorOriginal;
            volverEn = -1f;
        }
    }

    // La llama Weapon cuando el rayo le pega
    public void RecibirDisparo()
    {
        diana.material.color = colorGolpe;
        volverEn = Time.time + tiempoColor;

        if (sonido != null)
        {
            sonido.Play();
        }
    }
}
