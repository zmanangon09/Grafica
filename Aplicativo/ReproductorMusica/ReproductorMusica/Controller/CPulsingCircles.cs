using System;
using System.Drawing;

namespace ReproductorMusica
{
    internal class CPulsingCircles
    {
        private Graphics graphics;
        private float canvasWidth;
        private float canvasHeight;
        private int currentFrame;
        private float intensity;
        private const int maxCircles = 8;
        private Color[] themeColors;
        public CPulsingCircles(Graphics g, float width, float height, int frame, float audioIntensity, Color[] colors)
        {
            graphics = g;
            canvasWidth = width;
            canvasHeight = height;
            currentFrame = frame;
            intensity = audioIntensity;
            themeColors = colors;
        }

        public void Draw()
        {
            float centerX = canvasWidth / 2f;
            float centerY = canvasHeight / 2f;
            float maxRadius = Math.Min(canvasWidth, canvasHeight) * 0.45f;

           
            Color[] purpleShades = {
                Color.FromArgb(100, 128, 0, 128),      // Purple
                Color.FromArgb(120, 138, 43, 226),     // Blue Violet
                Color.FromArgb(110, 147, 112, 219),    // Medium Purple
                Color.FromArgb(130, 153, 50, 204),     // Dark Orchid
                Color.FromArgb(100, 186, 85, 211),     // Medium Orchid
                Color.FromArgb(120, 148, 0, 211),      // Dark Violet
                Color.FromArgb(110, 138, 43, 226),     // Blue Violet
                Color.FromArgb(130, 199, 21, 133)      // Medium Violet Red
            };

            // Dibujar círculos concéntricos que pulsan
            for (int i = 0; i < maxCircles; i++)
            {
                // Calcular el radio con efecto de pulsación
                float phase = (currentFrame + i * 12) % 100;
                float pulseEffect = (float)Math.Sin(phase * Math.PI / 50) * 0.3f * intensity + 0.7f;

                float radius = (maxRadius / maxCircles) * (i + 1) * pulseEffect;

                // Calcular transparencia basada en el frame
                int alpha = (int)(100 + 50 * Math.Sin((currentFrame + i * 15) * Math.PI / 50) * intensity);

                Color circleColor = Color.FromArgb(
                    Math.Min(alpha, purpleShades[i].A),
                    purpleShades[i].R,
                    purpleShades[i].G,
                    purpleShades[i].B
                );

                using (Pen circlePen = new Pen(circleColor, 2f))
                {
                    // Dibujar círculo
                    graphics.DrawEllipse(
                        circlePen,
                        centerX - radius,
                        centerY - radius,
                        radius * 2,
                        radius * 2
                    );
                }
            }

            // Dibujar líneas decorativas que rotan
            DrawRotatingLines(centerX, centerY, maxRadius);
        }

        private void DrawRotatingLines(float centerX, float centerY, float radius)
        {
            int numLines = 6;
            double angleStep = 2 * Math.PI / numLines;
            double baseAngle = currentFrame * Math.PI / 180 * 3 * intensity;

            using (Pen linePen = new Pen(Color.FromArgb(80, 138, 43, 226), 1))
            {
                for (int i = 0; i < numLines; i++)
                {
                    double angle = baseAngle + i * angleStep;

                    float x1 = centerX + (float)(radius * 0.3 * Math.Cos(angle));
                    float y1 = centerY + (float)(radius * 0.3 * Math.Sin(angle));

                    float x2 = centerX + (float)(radius * 0.9 * Math.Cos(angle));
                    float y2 = centerY + (float)(radius * 0.9 * Math.Sin(angle));

                    graphics.DrawLine(linePen, x1, y1, x2, y2);
                }
            }
        }
    }
}
