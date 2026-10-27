using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.InputSystem;

// Herramienta de editor: conecta el arma, la pistola, la bala y la vida del enemigo.
// Corre sola al compilar si la camara todavia no tiene Weapon,
// o desde el menu Herramientas > Configurar arma y enemigo.
[InitializeOnLoad]
public class ConfigurarEscena
{
    const string rutaBala = "Assets/Prefab/Bala.prefab";

    static ConfigurarEscena()
    {
        EditorApplication.delayCall += RevisarAlAbrir;
    }

    static void RevisarAlAbrir()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (EditorSceneManager.GetActiveScene().name != "SampleScene") return;

        Camera camara = Camera.main;
        if (camara != null && camara.GetComponent<Weapon>() == null)
        {
            Configurar();
        }
    }

    [MenuItem("Herramientas/Configurar arma y enemigo")]
    static void Configurar()
    {
        InputActionAsset acciones = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        AudioClip disparo = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/shotgun_adal.mp3");
        AudioClip vacio = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/empty_gun_adal.mp3");
        AudioClip recarga = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/gun_reload_adal.mp3");
        GameObject modeloPistola = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Model/pistol_gun.fbx");

        // Jugador: el objeto que tiene MovementCtrl lleva el tag Player
        MovementCtrl jugador = Object.FindFirstObjectByType<MovementCtrl>();
        if (jugador != null)
        {
            Undo.RecordObject(jugador.gameObject, "Tag Player");
            jugador.gameObject.tag = "Player";
        }
        else
        {
            Debug.LogError("No se encontró al jugador (MovementCtrl).");
        }

        Camera camara = Camera.main;
        if (camara == null)
        {
            Debug.LogError("No se encontró la Main Camera.");
            return;
        }
        GameObject cam = camara.gameObject;

        // Sensibilidad del mouse
        CamaraCRTL control = cam.GetComponent<CamaraCRTL>();
        if (control != null)
        {
            SerializedObject so = new SerializedObject(control);
            so.FindProperty("sensitivity").floatValue = 0.3f;
            so.ApplyModifiedProperties();
        }

        // Pistola en la mano: hija de la camara, abajo a la derecha
        Transform pistola = cam.transform.Find("Pistola");
        if (pistola == null && modeloPistola != null)
        {
            GameObject nueva = (GameObject)PrefabUtility.InstantiatePrefab(modeloPistola);
            Undo.RegisterCreatedObjectUndo(nueva, "Pistola");
            nueva.name = "Pistola";
            nueva.transform.SetParent(cam.transform, false);
            nueva.transform.localPosition = new Vector3(0.25f, -0.2f, 0.5f);
            nueva.transform.localRotation = Quaternion.identity;
            EscalarA(nueva, 0.3f);

            // Que la pistola no estorbe al Raycast ni a la bala
            foreach (Collider c in nueva.GetComponentsInChildren<Collider>())
            {
                Object.DestroyImmediate(c);
            }
            pistola = nueva.transform;
        }

        // Boca de la pistola: de aqui sale la bala
        Transform punto = cam.transform.Find("PuntoDisparo");
        if (punto == null)
        {
            GameObject p = new GameObject("PuntoDisparo");
            Undo.RegisterCreatedObjectUndo(p, "PuntoDisparo");
            p.transform.SetParent(cam.transform, false);
            p.transform.localPosition = new Vector3(0.25f, -0.15f, 0.8f);
            punto = p.transform;
        }

        // Arma en la camara
        AudioSource audio = cam.GetComponent<AudioSource>();
        if (audio == null) audio = Undo.AddComponent<AudioSource>(cam);
        audio.playOnAwake = false;

        Weapon arma = cam.GetComponent<Weapon>();
        if (arma == null) arma = Undo.AddComponent<Weapon>(cam);
        arma.inputActionAsset = acciones;
        arma.audioSource = audio;
        arma.sonidoDisparo = disparo;
        arma.sonidoVacio = vacio;
        arma.sonidoRecarga = recarga;
        arma.balaPrefab = CrearBala();
        arma.puntoDisparo = punto;
        EditorUtility.SetDirty(arma);

        // Vida en cada enemigo de la escena
        foreach (EnemyCtrl enemigo in Object.FindObjectsByType<EnemyCtrl>(FindObjectsSortMode.None))
        {
            if (enemigo.GetComponent<EnemyHealth>() == null)
            {
                Undo.AddComponent<EnemyHealth>(enemigo.gameObject);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Escena configurada: pistola, Weapon con bala, sensibilidad, tag Player y EnemyHealth.");
    }

    // Prefab de la bala: esfera pequena amarilla con Rigidbody y BalaCtrl
    static GameObject CrearBala()
    {
        GameObject existente = AssetDatabase.LoadAssetAtPath<GameObject>(rutaBala);
        if (existente != null) return existente;

        GameObject esfera = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        esfera.name = "Bala";
        esfera.transform.localScale = Vector3.one * 0.08f;
        esfera.AddComponent<Rigidbody>().useGravity = false;
        esfera.AddComponent<BalaCtrl>();

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader != null)
        {
            Material mat = new Material(shader);
            mat.SetColor("_BaseColor", Color.yellow);
            AssetDatabase.CreateAsset(mat, "Assets/Materiales/Bala.mat");
            esfera.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(esfera, rutaBala);
        Object.DestroyImmediate(esfera);
        return prefab;
    }

    // Ajusta la escala para que el objeto mida "largo" metros en su lado mas grande
    static void EscalarA(GameObject obj, float largo)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return;

        Bounds b = renderers[0].bounds;
        foreach (Renderer r in renderers) b.Encapsulate(r.bounds);

        float mayor = Mathf.Max(b.size.x, b.size.y, b.size.z);
        if (mayor > 0f)
        {
            obj.transform.localScale *= largo / mayor;
        }
    }
}
