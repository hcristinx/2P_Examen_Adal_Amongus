# Historial de prompts — Experiencia VR (Proyecto 3)

Registro de todos los prompts usados con herramientas de IA, incluidos los que no funcionaron.

| Campo | Qué se escribe |
|---|---|
| Fecha y herramienta | Cuándo y con qué IA se trabajó |
| Prompt | El texto exacto, copiado, no resumido |
| Respuesta | Resumen breve de lo que propuso la IA |
| Decisión | Qué se aceptó, modificó o rechazó, y por qué |
| Verificación | Cómo se comprobó |
| Lo que aprendí | Explicación con palabras propias |

---

## 1. Separar la entrada de la lógica del arma (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> Si, empieza con el avance 1 por favor

(El avance 1 del plan que propuso la IA era: "Separar la entrada de la lógica: un script de entrada que solo llama a `Disparar()`, `Recargar()`, etc. Se crean `BITACORA.md` y `PROMPTS.md`".)

**Respuesta:** Revisó los scripts y encontró que `Weapon` leía el mouse/teclado dentro de su `Update()`. Propuso quitar la lectura de controles de `Weapon` y crear `EntradaArma`, que lee `Attack` y `Reload` y llama a `arma.Disparar()` y `arma.Recargar()`. Usó un script temporal de editor para agregar `EntradaArma` en la escena y se borró solo.

**Decisión:** Se aceptó. Así el gatillo del control VR podrá llamar al mismo `Disparar()` sin reescribir la lógica. No se cambiaron `MovementCtrl`, `Jump` ni `CamaraCRTL` porque en VR se reemplazan por la locomoción del XR Interaction Toolkit.

**Verificación:** Se probó en Play antes de pasar a VR: el arma sigue disparando y recargando y no hay errores en consola.

**Lo que aprendí:** Al tener los controles del mouse directamente dentro del código del arma, sería necesario modificarlo para adaptarlo a VR. Separar ambas partes permite que el arma mantenga únicamente sus funciones de disparo y recarga, mientras que cualquier dispositivo de entrada puede ejecutarlas sin alterar su funcionamiento interno.

---

## 2. Optimización del nivel para VR (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> Implementa eso y haz que la optimizacion sea real. Y en la bitácora ponlo como te dije.

**Respuesta:** Encontré cinco problemas de rendimiento: el HUD de munición se reescribe en cada cuadro, cada enemigo recalcula su ruta en cada cuadro, ningún objeto está marcado como estático, las sombras son costosas y no hay XR instalado. Ahora corrijo el código.

**Decisión:** Bien implementado. Lo dejaremos así.

**Verificación:** Aún se traba, la UI no se ve, estoy algo pequeño.

**Lo que aprendí:** Las luces dinámicas consumen muchos recursos. Render Mode de Multi-pass a Single Pass Instanced ayuda a optimizar la escena, pero aún no es suficiente. La UI en world space no funciona del todo en VR, se necesitará hacer algo más.

---

## 3. Enemigos con nuestro modelo y otro color (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> El among us player es modelado por nosotros, los enemigos que son among us negros noi. Asi que haz que los enemigos sean tambien como el player pero de otro color por favor, que no sean rojos.

**Respuesta:** Encontró que el prefab del enemigo usa nuestro modelo, pero con el cuerpo rojo y dos piezas con un material negro del mapa descargado. Propuso materiales propios (azul para el base, morado y verde para las variantes) y crear las variantes Rápido y Resistente con distinta vida, velocidad y daño, con un Spawner que elige una al azar.

**Decisión:** Se aceptó.

**Verificación:** El script se aplicó sin errores: se crearon el enemigo base azul y las variantes Rápido (morado) y Resistente (verde), y el Spawner las tiene asignadas.

**Lo que aprendí:** Las variantes de un Prefab conservan las características del objeto original y permiten modificar únicamente ciertos atributos, como el color, la vida, la velocidad o el daño. Además, cualquier cambio realizado en el prefab principal se refleja automáticamente en sus variantes.

---

## 4. Configuración XR y XR Origin (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> Prefiero mejor que en este commit de leslie hagas hasta el avance 4 y ya mañana vemos que otros commits haran cada persona del equipo

**Respuesta:** Instaló XR Interaction Toolkit 3.5.1 con Starter Assets y XR Device Simulator, activó en OpenXR el soporte de Meta Quest y el perfil Oculus Touch, y configuró Android (IL2CPP, ARM64, API 32). Agregó el XR Origin de los Starter Assets como JugadorVR con movimiento continuo, giro por pasos y viñeta de confort, le puso el tag Player y PlayerHealth, y desactivó el jugador FPS.

**Decisión:** Se aceptó.

**Verificación:** Se probó en Play con los lentes conectados: la escena carga en VR. Aún se traba, la UI no se ve y la escala se siente pequeña.

**Lo que aprendí:** El XR Origin funciona como la estructura principal del jugador en realidad virtual, sustituyendo el sistema tradicional de cámara del FPS. Este permite que la cámara responda a los movimientos de la cabeza y que los controles detecten las manos. También es necesario asignarle el tag Player para que los enemigos puedan identificarlo y seguirlo correctamente.

---

## 5. Optimización para VR y altura del jugador (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> Claude, el nivel del proyecto de unity va muy lento, ayúdame a optimizarlo y encuentra lo que corra mal

**Respuesta:** Encontró cinco problemas que pesan en VR, donde todo se dibuja una vez por ojo: SSAO encendido en el renderer de PC (el que se usa con los lentes por Link), Depth Texture y Opaque Texture encendidos sin que nada los use, el mapa descargado sin marcar como estático, texturas de 2024×2024 para el visor, y el XR Origin con Tracking Origin Mode en Not Specified, que en Quest pone la cámara a una altura fija de 1.36 m en lugar de la estatura real. Apagó SSAO y las texturas extra, marcó el mapa como estático, bajó las texturas grandes a 1024 en Android y cambió el modo a Floor.

**Decisión:** Se aceptó.

**Verificación:** Pendiente de probar con los lentes.

**Lo que aprendí:** Debido a que la realidad virtual renderiza una imagen independiente para cada ojo, algunos efectos gráficos requieren más recursos de procesamiento. Por ejemplo, el SSAO puede afectar considerablemente el rendimiento. Pero configurar el escenario como estático ayuda a optimizar su representación y utilizar el modo Floor permite ajustar la cámara de acuerdo con la altura real del usuario.
