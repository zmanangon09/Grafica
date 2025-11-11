# Reproductor de Música con Visualización Animada

## ?? Descripción
Este proyecto es un reproductor de música moderno que combina reproducción de audio con visualizaciones animadas sincronizadas. Las figuras geométricas (hexágonos, triángulos y círculos pulsantes) se mueven y responden dinámicamente a la música que se está reproduciendo.

## ? Características Principales

### ?? Funcionalidades de Audio
- ? Carga de archivos MP3 y WAV
- ? Controles de reproducción (Play/Pause)
- ? Barra de progreso interactiva
- ? Control de volumen deslizable
- ? Visualización del tiempo transcurrido y total

### ?? Visualizaciones Animadas
- ? **Hexágono rotatorio** con colores morados y efectos visuales
- ? **Triángulo rotatorio** con animación suave
- ? **Círculos pulsantes** concéntricos con diferentes tonalidades
- ? **Líneas decorativas rotatorias** que añaden profundidad
- ? **Sincronización con la música** - Las animaciones se aceleran e intensifican al son de la música

### ?? Características Técnicas
- Framework: **.NET Framework 4.7.2**
- Lenguaje: **C# 7.3**
- Librería de Audio: **NAudio 2.2.1**
- Renderizado: **GDI+ con double buffering**
- FPS: **~30 frames por segundo**

## ?? Cómo Usar

1. **Ejecuta la aplicación**
2. **Clic en "Cargar"** para seleccionar un archivo de audio (MP3 o WAV)
3. **Presiona Play (??)** para reproducir la música
4. **Disfruta de las visualizaciones** que se sincronizan con tu música
5. **Ajusta el volumen** haciendo clic en el botón ??
6. **Navega por la canción** usando la barra de progreso

## ?? Estructura del Proyecto

```
ReproductorMusica/
??? Program.cs                    # Punto de entrada de la aplicación
??? ReproductorMusica.cs         # Lógica principal del reproductor
??? ReproductorMusica.Designer.cs # Diseño del formulario
??? CHexagon.cs                  # Clase para hexágono animado
??? CTriangle.cs                 # Clase para triángulo animado
??? CPulsingCircles.cs           # Clase para círculos pulsantes
??? Properties/                   # Configuraciones del proyecto
```

## ?? Integración Realizada

Este proyecto unifica dos aplicaciones anteriores:
1. **ReproductorMusica** - Reproductor de audio con NAudio
2. **Proy_P1** - Sistema de visualización de figuras geométricas

### Cambios Implementados:
- ? Integración de todas las clases de figuras en ReproductorMusica
- ? Sincronización de animaciones con el estado de reproducción
- ? Sistema de intensidad basado en el volumen del audio
- ? Limpieza de archivos duplicados y código innecesario
- ? Optimización de recursos con `using` statements
- ? Gestión adecuada de memoria con `Dispose()`

## ?? Sistema de Animación

Las animaciones funcionan en dos modos:

### Modo Pasivo (Sin música)
- Las figuras rotan lentamente
- Intensidad baja (0.3x)
- Colores suaves y movimiento fluido

### Modo Activo (Con música)
- Las figuras rotan más rápido
- Intensidad basada en el volumen (hasta 2.0x)
- Movimientos más pronunciados y vibrantes
- Los círculos pulsan con más amplitud

## ?? Paleta de Colores

Las visualizaciones utilizan una paleta de colores morados:
- Purple (#800080)
- Blue Violet (#8A2BE2)
- Medium Purple (#9370DB)
- Dark Orchid (#9932CC)
- Medium Orchid (#BA55D3)
- Dark Violet (#9400D3)
- Medium Violet Red (#C71585)

## ?? Requisitos del Sistema

- Windows 7 o superior
- .NET Framework 4.7.2 o superior
- Tarjeta gráfica compatible con GDI+
- Archivos de audio en formato MP3 o WAV

## ?? Solución de Problemas

**Problema**: No se carga el archivo de audio
- **Solución**: Verifica que el archivo sea MP3 o WAV y no esté corrupto

**Problema**: Las animaciones se ven lentas
- **Solución**: Cierra otras aplicaciones que consuman recursos gráficos

**Problema**: No se escucha el audio
- **Solución**: Verifica que el volumen del sistema esté activado y ajusta el volumen en la aplicación

## ????? Desarrollador

Proyecto desarrollado para la materia de Gráficas por Computadora - 6to Semestre

## ?? Notas de Versión

**v1.0** - Versión Unificada (2025)
- Integración completa de ReproductorMusica y Proy_P1
- Sistema de visualización sincronizado con audio
- Controles de reproducción optimizados
- Gestión mejorada de recursos

---

**¡Disfruta de tu música con visualizaciones increíbles! ???**
