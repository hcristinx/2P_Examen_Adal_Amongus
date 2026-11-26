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

---

## 1. Optimización del nivel para VR (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> Implementa eso y haz que la optimizacion sea real. 

**Respuesta:** Encontré cinco problemas de rendimiento: el HUD de munición se reescribe en cada cuadro, cada enemigo recalcula su ruta en cada cuadro, ningún objeto está marcado como estático, las sombras son costosas y no hay XR instalado. Ahora corrijo el código.

**Decisión:** Bien implementado. Lo dejaremos así.

**Verificación:** Aún se traba, la UI no se ve, estoy algo pequeño.

**Lo que aprendí:** Las luces dinámicas consumen muchos recursos. Render Mode de Multi-pass a Single Pass Instanced ayuda a optimizar la escena, pero aún no es suficiente. La UI en world space no funciona del todo en VR, se necesitará hacer algo más.

---

## 2. Enemigos con nuestro modelo y otro color (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> El among us player es modelado por nosotros, los enemigos que son among us negros noi. Asi que haz que los enemigos sean tambien como el player pero de otro color por favor, que no sean rojos.

**Respuesta:** Encontró que el prefab del enemigo usa nuestro modelo, pero con el cuerpo rojo y dos piezas con un material negro del mapa descargado. Propuso materiales propios (azul para el base, morado y verde para las variantes) y crear las variantes Rápido y Resistente con distinta vida, velocidad y daño, con un Spawner que elige una al azar.

**Decisión:** Se aceptó.

**Verificación:** El script se aplicó sin errores: se crearon el enemigo base azul y las variantes Rápido (morado) y Resistente (verde), y el Spawner las tiene asignadas.

**Lo que aprendí:** Las variantes de un Prefab conservan las características del objeto original y permiten modificar únicamente ciertos atributos, como el color, la vida, la velocidad o el daño. Además, cualquier cambio realizado en el prefab principal se refleja automáticamente en sus variantes.


## 3. Optimización para VR y altura del jugador (Leslie)

**Fecha y herramienta:** 25/11/2026 · Claude (Claude Code, modelo Claude Opus 5.5)

**Prompt:**
> Claude, el nivel del proyecto de unity va muy lento, ayúdame a optimizarlo y encuentra lo que corra mal

**Respuesta:** Encontró cinco problemas que pesan en VR, donde todo se dibuja una vez por ojo: SSAO encendido en el renderer de PC (el que se usa con los lentes por Link), Depth Texture y Opaque Texture encendidos sin que nada los use, el mapa descargado sin marcar como estático, texturas de 2024×2024 para el visor, y el XR Origin con Tracking Origin Mode en Not Specified, que en Quest pone la cámara a una altura fija de 1.36 m en lugar de la estatura real. Apagó SSAO y las texturas extra, marcó el mapa como estático, bajó las texturas grandes a 1024 en Android y cambió el modo a Floor.
