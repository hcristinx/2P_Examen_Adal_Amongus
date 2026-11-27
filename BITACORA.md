# Bitácora de desarrollo — Experiencia VR (Proyecto 3)

Equipo: Hilda Cristina Javier Montaña · Martha Alicia Saldívar Villanueva · Leslie Andrea Montemayor Zepeda · Ana Jeanette Zapata Blanco

Una entrada por sesión: qué se hizo, problema encontrado, cómo se resolvió, decisión tomada y pendientes.

---

## 25/11/2026 — Separar la entrada de la lógica del arma (Leslie)

**Qué se hizo**
- Se revisaron los scripts del FPS del segundo parcial para ver cuáles mezclan la lectura de controles con la lógica del juego. `Weapon`, `MovementCtrl`, `Jump` y `CamaraCRTL` leen el Input System directamente.
- `Weapon` ya no lee el mouse ni el teclado. Solo tiene la lógica: `Disparar()`, `Recargar()`, munición, raycast y sonidos.
- Se creó `EntradaArma`, un script de entrada que lee las acciones `Attack` y `Reload` y solo llama a `arma.Disparar()` y `arma.Recargar()`. Se agregó en la escena junto al arma.
- Se crearon `BITACORA.md` y `PROMPTS.md`.

**Problema encontrado**
- Al quitar el `InputActionAsset` de `Weapon`, la referencia que tenía en la escena se pierde, así que el arma dejaría de disparar si no se conecta el nuevo script.

**Cómo se resolvió**
- Se agregó `EntradaArma` al mismo objeto del arma con la referencia a `InputSystem_Actions` y al `Weapon`.

**Decisión tomada**
- `MovementCtrl`, `Jump` y `CamaraCRTL` se dejan como están: en VR los reemplaza la locomoción del XR Interaction Toolkit, así que solo se usarán para probar en el editor.
- `PlayerHealth.RecibirDanio()` y `EnemyHealth.RecibirDanio()` ya eran métodos públicos independientes de la entrada, no se cambiaron.
- El arma será la espada propia (`espada gomita.glb`) en lugar de la pistola descargada, y los enemigos usarán nuestro modelo de Among Us con otros colores.

**Pendientes**
- Prefabs y variantes de enemigo.
- La espada es `.glb` y Unity no la importa sin el paquete glTFast.

---

## 25/11/2026 — Optimización del nivel para VR (Leslie)

**Qué se hizo**
- Se buscó qué hacía lento el nivel, revisando scripts, escena y ajustes de render.
- `Weapon` reescribía el texto de munición del HUD en cada cuadro. Ahora solo lo actualiza cuando cambia la munición o el estado de recarga.
- Cada enemigo recalculaba su ruta con `SetDestination()` en cada cuadro. Ahora lo hace cada 0.25 s (`intervaloRuta`, configurable).
- Ningún objeto de la escena era estático. El suelo y los cubos del escenario se marcaron como estáticos para que Unity los agrupe al dibujarlos (static batching).
- La luz direccional tenía sombras suaves en tiempo real. Se cambiaron a sombras duras.
- Perfil de PC: sin sombras suaves, de 4 a 2 cascadas y distancia de sombras de 50 a 30 m.
- Perfil Mobile (el que usa Android, o sea el visor): HDR apagado y distancia de sombras de 50 a 20 m.
- Se instalaron XR Plug-in Management y OpenXR. Se activó OpenXR para Android y el Render Mode pasó de Multi-pass a Single Pass Instanced.

**Problema encontrado**
- La opción de Single Pass Instanced no existía en el proyecto porque no había ningún paquete de XR instalado.

**Cómo se resolvió**
- Se instalaron `com.unity.xr.management` y `com.unity.xr.openxr` con las versiones compatibles con Unity 6.5.

**Decisión tomada**
- OpenXR solo se activó para Android. En PC (Standalone) queda apagado por ahora, para que al dar Play en el editor sin visor no salgan errores.

**Pendientes**
- Configurar el perfil del visor (Meta Quest) en OpenXR e instalar XR Interaction Toolkit.
- Medir los FPS en el visor cuando ya esté el build.

---

## 25/11/2026 — Prefabs y variantes de enemigo (Leslie)

**Qué se hizo**
- El enemigo ahora usa nuestro modelo de Among Us (el mismo del jugador) en otro color. Antes tenía el cuerpo rojo como el jugador y dos piezas con un material negro que venía del mapa descargado (`QuadrosPreto`).
- Se crearon tres materiales propios: `Enemigo_Azul`, `Enemigo_Morado` y `Enemigo_Verde`. El visor se dejó igual.
- Se crearon dos Prefab Variants de `Enemigo`:
  - `Enemigo_Rapido` (morado): vida 25, velocidad 3.5, daño 5.
  - `Enemigo_Resistente` (verde): vida 100, velocidad 1.3, daño 20.
- `Spawner` tiene un arreglo `variantesEnemigo` y elige una al azar en cada aparición.
- El spawner y los obstáculos (`Cube (2)` a `Cube (6)`) se convirtieron en Prefabs (`Spawner.prefab`, `Obstaculo.prefab`).

**Problema encontrado**
- Los enemigos usaban un material del escenario descargado, que no es de nuestra autoría.

**Cómo se resolvió**
- Se reemplazó por materiales propios en el prefab base y en cada variante.

**Decisión tomada**
- Ningún enemigo es rojo, para que no se confundan con el color del jugador.

**Pendientes**
- Prefab del arma cuando se integre la espada.

---

## 25/11/2026 — Configuración XR (Leslie)

**Qué se hizo**
- Se instaló XR Interaction Toolkit 3.5.1 con sus Starter Assets y el XR Device Simulator.
- OpenXR para Android: se activaron Meta Quest Support y el perfil de controles Oculus Touch.
- OpenXR para PC: se activó para poder probar con los lentes conectados por Link desde el editor, también con el perfil Oculus Touch.
- Ajustes de Android para el visor: IL2CPP, ARM64 y API mínima 32.

**Problema encontrado**
- Sin un perfil de interacción, OpenXR no reconoce los botones de los controles.

**Cómo se resolvió**
- Se activó el perfil Oculus Touch Controller en Android y en PC.

**Decisión tomada**
- La plataforma activa sigue siendo PC mientras se desarrolla. Se cambiará a Android para el build.

**Pendientes**
- Build del `.apk` y prueba en el visor.

---

## 25/11/2026 — XR Origin y locomoción (Leslie)

**Qué se hizo**
- Se agregó el XR Origin (XR Rig) de los Starter Assets como `JugadorVR`, en el lugar del jugador FPS, y se guardó como Prefab (`JugadorVR.prefab`).
- Movimiento continuo con el joystick izquierdo a 2.5 m/s y giro por pasos de 45° con el derecho (Snap Turn).
- Viñeta de confort (Tunneling Vignette) debajo de la cámara, que se activa al moverse, girar y teletransportarse.
- `JugadorVR` tiene el tag `Player` y `PlayerHealth`, así que los enemigos lo persiguen y le hacen daño.
- El jugador FPS se desactivó, porque en VR lo reemplaza el XR Origin.
- Se agregó el XR Device Simulator a la escena, apagado, para probar sin lentes.

**Problema encontrado**
- Los enemigos buscan al jugador con `FindGameObjectWithTag("Player")`. Si el jugador FPS y el XR Origin estuvieran activos a la vez, podrían perseguir al equivocado.

**Cómo se resolvió**
- El jugador FPS se desactivó. `FindGameObjectWithTag` no encuentra objetos inactivos.

**Decisión tomada**
- El simulador se deja apagado: se enciende solo para probar sin lentes.

**Pendientes**
- El arma en VR (todavía está en el jugador FPS).
- El HUD está en Screen Space y en VR no se ve; se pasará a World Space.

---

## 25/11/2026 — Optimización para VR y altura del jugador (Leslie)

**Qué se hizo**
- Se probó con los lentes por Link: aún se trababa, la UI no se veía y la escala se sentía pequeña.
- Se apagó el SSAO (oclusión ambiental) del renderer de PC, que es un efecto de pantalla completa y en VR se calcula por cada ojo.
- Se apagaron Depth Texture y Opaque Texture del perfil de PC, porque ningún shader del juego los usa.
- Se marcaron como estáticos los objetos del mapa (antes solo el suelo y los cubos).
- Las texturas de 2024×2024 del mapa bajan a 1024 en Android.
- El `JugadorVR` pasó de Tracking Origin Mode "Not Specified" a "Floor".

**Problema encontrado**
- "Me siento pequeño": con "Not Specified", en Quest se usa el modo Device, que pone la cámara a una altura fija de 1.36 m en lugar de la estatura real de quien juega.

**Cómo se resolvió**
- Con el modo Floor, la altura de la cámara es la altura real del visor sobre el piso.

**Decisión tomada**
- La UI todavía no se pasa a World Space; queda para el avance del HUD.

**Pendientes**
- Volver a probar con los lentes y medir con el Profiler.
- HUD y pantalla final en World Space.

---

## 25/11/2026 — La espada propia como arma de VR (Hilda)

**Qué se hizo**
- La espada (`espada gomita.glb`, hecha en Nomad) se redujo con un script de Blender (`Fuentes/reducir_espada.py`): de 3.1 millones de triángulos a 3,065 triángulos y 4,571 vértices.
- Los colores por vértice se hornearon a una textura propia de 1024×1024 con UVs nuevas.
- Las piezas siguen separadas para poder animarlas: hoja, guarda, mango y pomo.
- El original de 85 MB se movió a `Fuentes/`, fuera de `Assets`.
- La espada se toma con el botón de agarre (XR Grab Interactable) desde un pedestal enfrente del jugador.
- El gatillo llama a `Weapon.Disparar()` y vibra el control si salió el disparo; A o X llaman a `Weapon.Recargar()` (`EntradaArmaVR`).
- El raycast y la bala salen de la punta de la espada (`origenRaycast` en `Weapon`).
- Se guardó como `Espada.prefab`.

**Problema encontrado**
- La primera reducción tenía 4,195 triángulos pero 9,700 vértices en Unity: con sombreado plano cada cara tiene sus propios vértices, y cada costura de UV los duplica.

**Cómo se resolvió**
- Sombreado suave, menos islas de UV y menos triángulos en la hoja.

**Decisión tomada**
- La espada dispara un rayo desde la punta, para conservar la mecánica de disparo y munición del FPS.

**Pendientes**
- Probar en los lentes.
- Mostrar la munición en la espada.
- Animaciones de disparo y recarga.

---

## 26/11/2026 — Munición en la espada y partículas de disparo (Martha)

**Qué se hizo**
- Se agregó un letrero de munición pegado a la espada (TextMeshPro en World Space), porque en VR no se ve el HUD de la pantalla. Muestra "cargador / reserva", "Recargando..." al recargar y se pone rojo con el cargador vacío.
- El letrero siempre mira hacia la cámara para que se pueda leer desde cualquier ángulo (`ContadorMunicionArma`).
- Se agregó una partícula de destello en la punta de la espada que se lanza en cada disparo.
- Se agregó una partícula de impacto (`ImpactoDisparo.prefab`) que aparece donde pega el rayo, orientada hacia afuera de la superficie, y se destruye sola al terminar.
- `Weapon` tiene dos campos nuevos (`particulaDisparo` y `particulaImpacto`) y los usa en `Disparar()`.

**Problema encontrado**
- Un texto en World Space solo se lee de frente: si la espada se gira, el letrero queda de lado o al revés.

**Cómo se resolvió**
- En `LateUpdate` el letrero se orienta hacia la cámara en cada cuadro.

**Decisión tomada**
- El letrero solo se actualiza cuando cambia la munición, igual que se hizo con el HUD para no reescribir el texto en cada cuadro.

**Pendientes**
- Probar en los lentes que el letrero se lea bien y ajustar su tamaño si hace falta.
- Zona de práctica con blancos.

---

## 27/11/2026 — Zona de práctica de tiro (Jeanette)

**Qué se hizo**
- Se creó el Prefab `Blanco`: un poste con una diana amarilla. Al recibir un disparo, la diana se pone verde un segundo y suena (`Blanco.cs`).
- `Weapon.Disparar()` ahora también avisa a los blancos: si el rayo pega en un objeto con `Blanco`, llama a `RecibirDisparo()`.
- Se puso una fila de 3 blancos (`ZonaPractica`) a 5 m enfrente del jugador, en un lugar libre del mapa, mirando hacia él.
- Cada blanco tiene un NavMeshObstacle con carving, para que los enemigos lo rodeen en lugar de atravesarlo.

**Problema encontrado**
- La diana es un cilindro aplanado, y el collider de cápsula que trae por defecto no tiene la forma de un disco: el rayo podía pasar por las orillas sin pegarle.

**Cómo se resolvió**
- Se cambió el collider de cápsula por un Mesh Collider con la forma exacta del disco.

**Decisión tomada**
- Los blancos no tienen vida ni desaparecen: sirven para practicar disparo y recarga cuantas veces se quiera antes del combate.

**Pendientes**
- Probar en los lentes la distancia y la altura de los blancos.

---

## 27/11/2026 — Controles, UI en VR y tiempo de reacción (Jeanette)

**Qué se hizo**
- Al probar con los lentes no se podía caminar, teletransportarse ni agarrar la espada, los enemigos atrapaban al jugador casi de inmediato y no se veía la UI (tampoco el panel de derrota).
- Se activaron en OpenXR los perfiles de controles Oculus Touch y Meta Quest Touch Plus, en PC y en Android.
- El Canvas del HUD (vida, munición, oleada, eliminados y panel final) pasó de Screen Space a World Space: flota a 1.5 m enfrente del jugador y sigue su cabeza con suavidad (`SeguirCabeza.cs`).
- El EventSystem usa XR UI Input Module y el HUD tiene Tracked Device Graphic Raycaster, para presionar el botón Reiniciar con el rayo del control.
- Se apagó un Canvas extra que solo tenía un panel vacío.
- `GameManager` espera 15 s (`tiempoPreparacion`) antes de la primera oleada, para tomar la espada y practicar.
- El enemigo Rápido bajó de 3.5 a 2.5 m/s.

**Problema encontrado**
- El reporte de OpenXR en la consola decía "Features requested to be enabled: (0)": no había ningún perfil de controles activo, así que Unity no recibía ningún botón ni joystick. Se habían activado antes, pero el cambio no se guardó en el archivo de ajustes.
- La UI en Screen Space no se dibuja en VR.

**Cómo se resolvió**
- Se activaron los perfiles y se guardó el archivo de ajustes de OpenXR, comprobando que quedaran con `m_enabled: 1`.
- La UI se pasó a World Space con un script que la mantiene enfrente de la cabeza. Usa `Time.unscaledDeltaTime` para seguir moviéndose aunque el juego esté en pausa en la pantalla final.

**Verificación**
- Se probó con los lentes: la UI ya se ve.

**Pendientes**
- La mira "+" del FPS sigue en el HUD; en VR se apunta con la espada.
- El botón Reiniciar todavía recarga la escena; el documento pide reiniciar sin cambiar de escena.
