# Simulador Balístico

Simulador de tiro parabólico en Unity. El jugador ajusta **ángulo, giro horizontal, fuerza y masa** del proyectil para derribar una estructura armada con Rigidbodies y Joints. Antes de disparar se ve una vista previa de la trayectoria, y cada intento genera un reporte de tiro con los datos físicos del impacto y un puntaje.

- **Versión de Unity:** 6000.0.x (Unity 6) — URP
- **Video:** ((https://youtu.be/yJzeUpAQN5M))

## Cómo jugar

| Control | Acción |
|---|---|
| Slider *Vertical* (Angle) | Inclinación del cañón, 0° = horizontal, 90° = apunta al cielo |
| Slider *Horizontal* (Yaw) | Gira el cañón a izquierda/derecha, como una torreta |
| Slider *Fuerza* | Magnitud del impulso aplicado al proyectil |
| Slider *Masa* | Masa del proyectil en kg |
| Línea amarilla | Vista previa de la trayectoria del próximo disparo, se recalcula sola al mover cualquier slider |
| **Espacio** o botón **DISPARAR** | Dispara el proyectil (cualquiera de los dos hace lo mismo) |
| Botón **Reset Settings** | Vuelve los 4 sliders a sus valores por defecto |
| Botón **Reiniciar objetivos** | Devuelve la estructura derribada a su posición original |



Solo se cuentan piezas que todavía no habían sido derribadas en intentos anteriores. Una pieza se considera derribada si se inclinó más de 40° o se desplazó más de 0,5 m respecto de su pose inicial.

## Reporte de tiro

Al final de cada intento se muestran: ángulo, fuerza y masa usados; tiempo de vuelo; punto de impacto; alcance horizontal; altura máxima; velocidad relativa en el choque; impulso de colisión; si el impacto fue directo sobre una pieza; piezas derribadas; puntaje del intento y puntaje total acumulado.

## Scripts

| Script | Responsabilidad |
|---|---|
| `Weapon.cs` | Lee la UI, orienta el cañón en dos ejes, dispara y resetea los controles |
| `Projectile.cs` | Mide vuelo e impacto, arma el `ShotData` en la primera colisión |
| `TargetPiece.cs` | Pose inicial de cada pieza de la estructura y detección de derribo |
| `ShotManager.cs` | Encuentra las piezas solo, evalúa el intento, calcula puntaje y muestra el reporte |
| `TrajectoryPreview.cs` | Dibuja la vista previa de la trayectoria con un Line Renderer |
| `ShotData.cs` | Estructura de datos de un intento (no es un componente, no va en ningún GameObject) |

Para el armado paso a paso de la escena (jerarquía completa, qué va en cada campo del Inspector, valores exactos), ver `GUIA-IMPLEMENTACION.md` en este mismo repositorio.

## Repositorio

- `.gitignore` de Unity (excluye `Library/`, `Temp/`, `Obj/`, `Build/`, `Builds/`, `Logs/`, `UserSettings/`).
- Se sube `Assets/`, `Packages/` y `ProjectSettings/`.


## Criterios de evaluación

1. El disparo se resuelve con el sistema de físicas (Rigidbody + AddForce), no con movimiento manual.
2. Ángulo, giro, fuerza y masa son configurables en runtime y afectan visiblemente la trayectoria.
3. La estructura de objetivos usa Joints y es estable antes del primer disparo.
4. Se registran y muestran tiempo de vuelo, punto de impacto, velocidad relativa, impulso de colisión y piezas derribadas.
5. Cada intento cierra con puntaje y reporte de tiro.
6. Repositorio ordenado, README completo y video de 1 a 3 minutos con la interfaz, al menos tres tiros distintos y el registro de resultados.
