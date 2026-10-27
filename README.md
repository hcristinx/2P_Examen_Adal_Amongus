# Arena de Oleadas — Among Us

Shooter en primera persona hecho en **Unity 6 (6000.5.8f1)** para LAD537 Motores Gráficos Orientados a Videojuegos (Proyecto 2).
El jugador defiende la sala de Among Us contra oleadas de enemigos que lo persiguen con NavMesh. Al morir, los enemigos pueden soltar cajas de munición.

## Integrantes

- Leslie Andrea Montemayor Zepeda
- Hilda Cristina Javier Montaña
- Martha Alicia Saldivar Villanueva
- Ana Jeanette Zapata Blanco

## Controles

| Acción | Tecla |
|---|---|
| Moverse | W A S D |
| Mirar | Mouse |
| Disparar | Clic izquierdo |
| Recargar | R |
| Saltar | Espacio |
| Liberar el cursor | Esc (clic izquierdo para volver a capturarlo) |

Todas las entradas usan el **New Input System** (`Assets/InputSystem_Actions.inputactions`, mapa `Player`).

## Clases

| Clase | Responsabilidad |
|---|---|
| `MovementCtrl` | Movimiento del jugador con WASD usando su Rigidbody (choca con las paredes). |
| `CamaraCRTL` | Cámara en primera persona: el cuerpo gira en horizontal y la cámara en vertical, con límite de inclinación. Bloquea el cursor. |
| `Jump` | Salto con Espacio, solo cuando el jugador está en el suelo. |
| `PlayerHealth` | Vida del jugador y recepción de daño. Al llegar a 0 avisa la derrota. |
| `Weapon` | Disparo con `Physics.Raycast` desde el centro de la cámara, cadencia, cargador y reserva, recarga y sonidos. |
| `BalaCtrl` | Bala visual que sale de la pistola. El daño lo hace el Raycast. |
| `EnemyCtrl` | Persecución del jugador con `NavMeshAgent` y ataque con intervalo de tiempo. |
| `EnemyHealth` | Vida del enemigo, daño, eliminación con `Destroy()` y probabilidad de soltar munición. |
| `AmmoPickup` | Caja de munición (trigger): suma balas a la reserva, suena y se destruye. |
| `Spawner` | Crea los enemigos de cada oleada desde los puntos de aparición. |
| `GameManager` | Estado de la partida (oleada, enemigos vivos y eliminados), victoria, derrota y reinicio. |
| `UIManager` | HUD (vida, munición, oleada, eliminados, mira) y pantalla final. |

## Parámetros configurables (Inspector)

**GameManager**
- `numeroOleadas` = 4
- `enemigosIniciales` = 3
- `incrementoPorOleada` = 2 (oleadas de 3, 5, 7 y 9 enemigos)
- `tiempoEntreOleadas` = 3 s

**Spawner**
- `spawnPoints`: 4 puntos en los bordes de la sala
- `tiempoEntreEnemigos` = 0.8 s

**Weapon**
- `fireRate` = 0.3 s entre disparos
- `danio` = 25
- `alcance` = 100
- `capacidadCargador` = 10, `reservaMaxima` = 40
- `tiempoRecarga` = 1.5 s
- `tiempoParaRestaurar` = 5 s (si el jugador se queda sin ninguna bala, la reserva se rellena después de este tiempo)

**EnemyHealth**
- `vidaMaxima` = 50 (2 disparos)
- `probDrop` = 0.3 (30 %; 100 % si el jugador tiene 0 balas en cargador y reserva)

**EnemyCtrl**
- `velocidad` = 2
- `distanciaAtaque` = 1.8, `danio` = 10, `intervaloAtaque` = 2 s

**PlayerHealth**
- `vidaMaxima` = 100

**AmmoPickup**
- `cantidad` = 15 balas

**MovementCtrl / CamaraCRTL**
- `velocidad` = 5, `sensitivity` = 0.3, límite vertical de ±80°

## Cómo abrir el proyecto

1. Clonar con Git LFS (los modelos y texturas están en LFS):
   ```
   git lfs install
   git clone https://github.com/hcristinx/2P_Examen_Adal_Amongus.git
   ```
2. Guardarlo en una ruta corta (por ejemplo `C:\Unity\`). Con rutas muy largas Unity no puede importar algunos paquetes.
3. Abrirlo con Unity 6000.5.8f1 y abrir la escena `Assets/Scenes/SampleScene`.

## Créditos de assets

- Escenario (`Assets/Esenario/among-us-cenario-3d-mapa`): [Among Us Cenário 3D Mapa](https://sketchfab.com/3d-models/among-us-cenario-3d-mapa-feb46db9aba5400e9325a0cc5a6e848c) en Sketchfab
- Modelo del jugador (`Assets/Prefab/AmongUS.fbx`): hecho por Leslie Andrea Montemayor Zepeda
- Modelo del enemigo: [Among Us 3D Crewmates Black](https://sketchfab.com/3d-models/among-us-3d-crewmates-black-f54c7a737fa047d18ac8f9c4ad65103d) en Sketchfab
- Pistola (`Assets/Model/pistol_gun.fbx`): modelo FPS de Adalberto Lopez Torrez
- Sonidos (Pixabay, licencia de contenido de Pixabay):
  - Disparo (`shotgun_adal.mp3`): [Laser Gun Sound](https://pixabay.com/es/sound-effects/pel%C3%ADculas-y-efectos-especiales-laser-gun-sound-40813/)
  - Clic vacío (`empty_gun_adal.mp3`): [Empty Gun Shot](https://pixabay.com/es/sound-effects/pel%C3%ADculas-y-efectos-especiales-empty-gun-shot-6209/)
  - Recarga (`gun_reload_adal.mp3`): [Gun Reload 2](https://pixabay.com/es/sound-effects/pel%C3%ADculas-y-efectos-especiales-gun-reload-2-511308/)
  - Recoger munición (`pick_up_Adal.mp3`): [Item Equip](https://pixabay.com/es/sound-effects/pel%C3%ADculas-y-efectos-especiales-item-equip-6904/)

## Uso de inteligencia artificial

Usamos un asistente de IA (Claude) como apoyo para completar algunas funcionalidades
(arma con Raycast, oleadas, munición soltada y HUD), diagnosticar errores de la consola
y comprender mejor la lógica del juego. Revisamos y probamos el código en Unity, y las
decisiones de diseño están documentadas en BITACORA.md.
