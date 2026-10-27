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

        if (Object.FindFirstObjectByType<GameManager>() == null)
        {
            ArmarJuego();
        }

        GameObject caja = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefab/CajaMunicion.prefab");
        if (caja != null && caja.GetComponent<Rigidbody>() == null)
        {
            CajaConGravedad();
        }
    }

    // Caja mas pequena que cae al suelo: un collider solido para chocar con el piso
    // y un trigger un poco mas grande para que el jugador la recoja
    [MenuItem("Herramientas/Caja de munición pequeña con gravedad")]
    static void CajaConGravedad()
    {
        const string ruta = "Assets/Prefab/CajaMunicion.prefab";
        GameObject contenido = PrefabUtility.LoadPrefabContents(ruta);

        contenido.transform.localScale = Vector3.one * 0.2f;

        // Capa "Ignore Raycast" para que no bloquee los disparos
        contenido.layer = 2;

        BoxCollider solido = contenido.GetComponent<BoxCollider>();
        solido.isTrigger = false;

        BoxCollider[] colliders = contenido.GetComponents<BoxCollider>();
        if (colliders.Length < 2)
        {
            BoxCollider zona = contenido.AddComponent<BoxCollider>();
            zona.isTrigger = true;
            zona.size = Vector3.one * 3f;   // 3 veces la caja: facil de recoger
        }

        Rigidbody rb = contenido.GetComponent<Rigidbody>();
        if (rb == null) rb = contenido.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.mass = 0.5f;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        PrefabUtility.SaveAsPrefabAsset(contenido, ruta);
        PrefabUtility.UnloadPrefabContents(contenido);
        Debug.Log("Caja de munición: más pequeña y con gravedad.");
    }

    [MenuItem("Herramientas/Armar juego (pistola, enemigos, oleadas y HUD)")]
    static void ArmarJuego()
    {
        MovementCtrl jugador = Object.FindFirstObjectByType<MovementCtrl>();
        Camera camara = Camera.main;
        if (jugador == null || camara == null)
        {
            Debug.LogError("Falta el jugador (MovementCtrl) o la Main Camera.");
            return;
        }

        // 1. Pistola y boca de disparo pasan de la camara al cuerpo,
        //    para que se muevan junto con la mano del modelo
        foreach (string nombre in new string[] { "Pistola", "PuntoDisparo" })
        {
            Transform t = camara.transform.Find(nombre);
            if (t != null)
            {
                Undo.SetTransformParent(t, jugador.transform, nombre);
            }
        }

        // 2. Vida del jugador
        if (jugador.GetComponent<PlayerHealth>() == null)
        {
            Undo.AddComponent<PlayerHealth>(jugador.gameObject);
        }

        // 3. Caja de municion y prefab del enemigo
        GameObject caja = CrearCajaMunicion();
        GameObject enemigoPrefab = CrearPrefabEnemigo(caja);

        // 4. GameManager con el Spawner y los puntos de aparicion
        GameObject gm = new GameObject("GameManager");
        Undo.RegisterCreatedObjectUndo(gm, "GameManager");
        GameManager gameManager = gm.AddComponent<GameManager>();
        Spawner spawner = gm.AddComponent<Spawner>();
        spawner.enemyPrefab = enemigoPrefab;
        spawner.spawnPoints = CrearPuntosAparicion(jugador.transform.position);

        // 5. HUD y pantalla final
        CrearHUD(gameManager);

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorSceneManager.SaveOpenScenes();
        Debug.Log("Juego armado: pistola en el cuerpo, PlayerHealth, prefab Enemigo, caja de munición, GameManager, Spawner y HUD.");
    }

    static GameObject CrearCajaMunicion()
    {
        const string ruta = "Assets/Prefab/CajaMunicion.prefab";
        GameObject existente = AssetDatabase.LoadAssetAtPath<GameObject>(ruta);
        if (existente != null) return existente;

        GameObject cubo = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cubo.name = "CajaMunicion";
        cubo.transform.localScale = Vector3.one * 0.4f;
        cubo.GetComponent<BoxCollider>().isTrigger = true;

        AmmoPickup pickup = cubo.AddComponent<AmmoPickup>();
        pickup.sonido = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/pick_up_Adal.mp3");

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader != null)
        {
            Material mat = new Material(shader);
            mat.SetColor("_BaseColor", new Color(0.2f, 0.8f, 0.2f));
            AssetDatabase.CreateAsset(mat, "Assets/Materiales/CajaMunicion.mat");
            cubo.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(cubo, ruta);
        Object.DestroyImmediate(cubo);
        return prefab;
    }

    // Convierte al enemigo de la escena en prefab y lo quita de la escena:
    // a partir de ahora los enemigos solo los crea el Spawner
    static GameObject CrearPrefabEnemigo(GameObject caja)
    {
        const string ruta = "Assets/Prefab/Enemigo.prefab";
        GameObject existente = AssetDatabase.LoadAssetAtPath<GameObject>(ruta);
        if (existente != null) return existente;

        EnemyCtrl enemigo = Object.FindFirstObjectByType<EnemyCtrl>();
        if (enemigo == null)
        {
            Debug.LogError("No hay enemigo en la escena para crear el prefab.");
            return null;
        }

        enemigo.player = null;   // El prefab busca al jugador por su tag

        EnemyHealth vida = enemigo.GetComponent<EnemyHealth>();
        if (vida == null) vida = enemigo.gameObject.AddComponent<EnemyHealth>();
        vida.cajaMunicion = caja;

        GameObject raiz = PrefabUtility.GetOutermostPrefabInstanceRoot(enemigo.gameObject);
        if (raiz == null) raiz = enemigo.gameObject;

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(raiz, ruta);
        Undo.DestroyObjectImmediate(raiz);
        return prefab;
    }

    // 4 puntos en las esquinas del NavMesh, lejos del jugador
    static Transform[] CrearPuntosAparicion(Vector3 posJugador)
    {
        UnityEngine.AI.NavMeshTriangulation nav = UnityEngine.AI.NavMesh.CalculateTriangulation();
        Vector3[] direcciones = {
            new Vector3(1, 0, 1), new Vector3(-1, 0, 1),
            new Vector3(1, 0, -1), new Vector3(-1, 0, -1)
        };

        GameObject padre = new GameObject("PuntosAparicion");
        Undo.RegisterCreatedObjectUndo(padre, "PuntosAparicion");
        Transform[] puntos = new Transform[direcciones.Length];

        for (int i = 0; i < direcciones.Length; i++)
        {
            // El vertice del NavMesh que mas se aleja del jugador en esa diagonal
            Vector3 mejor = posJugador + direcciones[i] * 10f;
            float mejorValor = float.MinValue;
            foreach (Vector3 v in nav.vertices)
            {
                if (Mathf.Abs(v.y - posJugador.y) > 2f) continue;   // Solo el piso donde esta el jugador
                float valor = Vector3.Dot(v - posJugador, direcciones[i]);
                if (valor > mejorValor)
                {
                    mejorValor = valor;
                    mejor = v;
                }
            }

            GameObject p = new GameObject("Spawn " + (i + 1));
            p.transform.SetParent(padre.transform, false);
            p.transform.position = mejor;
            puntos[i] = p.transform;
        }
        return puntos;
    }

    static void CrearHUD(GameManager gameManager)
    {
        GameObject canvasGO = new GameObject("HUD", typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(canvasGO, "HUD");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        UnityEngine.UI.CanvasScaler escalador = canvasGO.AddComponent<UnityEngine.UI.CanvasScaler>();
        escalador.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
        escalador.referenceResolution = new Vector2(1920, 1080);
        canvasGO.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        UIManager ui = canvasGO.AddComponent<UIManager>();
        RectTransform raiz = canvasGO.GetComponent<RectTransform>();

        // Textos del HUD
        ui.textoVida = CrearTexto(raiz, "Vida", new Vector2(0, 1), new Vector2(40, -40), 40, TMPro.TextAlignmentOptions.TopLeft);
        ui.textoOleada = CrearTexto(raiz, "Oleada", new Vector2(0, 1), new Vector2(40, -100), 40, TMPro.TextAlignmentOptions.TopLeft);
        ui.textoEliminados = CrearTexto(raiz, "Eliminados", new Vector2(0, 1), new Vector2(40, -160), 40, TMPro.TextAlignmentOptions.TopLeft);
        ui.textoMunicion = CrearTexto(raiz, "Municion", new Vector2(1, 0), new Vector2(-40, 40), 48, TMPro.TextAlignmentOptions.BottomRight);

        // Mira al centro
        TMPro.TextMeshProUGUI mira = CrearTexto(raiz, "Mira", new Vector2(0.5f, 0.5f), Vector2.zero, 50, TMPro.TextAlignmentOptions.Center);
        mira.text = "+";

        // Pantalla final: fondo oscuro, resultado, estadisticas y boton
        GameObject panel = new GameObject("PanelFinal", typeof(RectTransform));
        panel.transform.SetParent(raiz, false);
        RectTransform rtPanel = panel.GetComponent<RectTransform>();
        rtPanel.anchorMin = Vector2.zero;
        rtPanel.anchorMax = Vector2.one;
        rtPanel.offsetMin = Vector2.zero;
        rtPanel.offsetMax = Vector2.zero;
        panel.AddComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.75f);

        ui.panelFinal = panel;
        ui.textoResultado = CrearTexto(rtPanel, "Resultado", new Vector2(0.5f, 0.5f), new Vector2(0, 180), 100, TMPro.TextAlignmentOptions.Center);
        ui.textoEstadisticas = CrearTexto(rtPanel, "Estadisticas", new Vector2(0.5f, 0.5f), new Vector2(0, 20), 44, TMPro.TextAlignmentOptions.Center);

        GameObject boton = new GameObject("BotonReiniciar", typeof(RectTransform));
        boton.transform.SetParent(rtPanel, false);
        RectTransform rtBoton = boton.GetComponent<RectTransform>();
        rtBoton.anchoredPosition = new Vector2(0, -160);
        rtBoton.sizeDelta = new Vector2(360, 100);
        boton.AddComponent<UnityEngine.UI.Image>().color = new Color(0.85f, 0.15f, 0.2f);
        UnityEngine.UI.Button btn = boton.AddComponent<UnityEngine.UI.Button>();
        UnityEditor.Events.UnityEventTools.AddPersistentListener(btn.onClick, gameManager.Reiniciar);

        TMPro.TextMeshProUGUI textoBoton = CrearTexto(rtBoton, "Texto", new Vector2(0.5f, 0.5f), Vector2.zero, 44, TMPro.TextAlignmentOptions.Center);
        textoBoton.text = "Reiniciar";
    }

    static TMPro.TextMeshProUGUI CrearTexto(RectTransform padre, string nombre, Vector2 ancla, Vector2 posicion, float tamano, TMPro.TextAlignmentOptions alineacion)
    {
        GameObject go = new GameObject(nombre, typeof(RectTransform));
        go.transform.SetParent(padre, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = ancla;
        rt.anchorMax = ancla;
        rt.pivot = ancla;
        rt.anchoredPosition = posicion;
        rt.sizeDelta = new Vector2(700, 120);

        TMPro.TextMeshProUGUI texto = go.AddComponent<TMPro.TextMeshProUGUI>();
        texto.fontSize = tamano;
        texto.alignment = alineacion;
        texto.color = Color.white;
        texto.text = nombre;
        return texto;
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
