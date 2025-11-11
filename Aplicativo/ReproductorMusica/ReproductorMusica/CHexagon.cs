using System;
using System.Drawing;

namespace ReproductorMusica
{
    internal class CHexagon
    {
        private float hexSize;
        private float rotationAngle = 0f;

        public CHexagon(float size)
        {
            hexSize = size;
        }

        public void SetRotation(float angle)
        {
            rotationAngle = angle;
        }

        public void RotateAndDraw(float deltaAngle, Graphics g, float centerX, float centerY)
        {
            rotationAngle += deltaAngle;

            if (hexSize <= 0)
                return;

            // Crear array de 6 puntos para el hexágono
            PointF[] hexPoints = new PointF[6];
            double angleInRadians = rotationAngle * Math.PI / 180.0;

            // Calcular los 6 vértices del hexágono
            for (int i = 0; i < 6; i++)
            {
                // Ángulo de cada vértice (60 grados entre cada uno)
                double vertexAngle = angleInRadians + i * Math.PI / 3;

                float xPos = (float)(hexSize * Math.Cos(vertexAngle));
                float yPos = (float)(hexSize * Math.Sin(vertexAngle));

                hexPoints[i] = new PointF(centerX + xPos, centerY + yPos);
            }

            // Dibujar hexágono exterior con color morado
            using (Pen outerPen = new Pen(Color.FromArgb(180, 128, 0, 180), 3))
            {
                g.DrawPolygon(outerPen, hexPoints);
            }

            // Dibujar hexágono interior más pequeño
            PointF[] innerHexPoints = new PointF[6];
            float innerSize = hexSize * 0.6f;

            for (int i = 0; i < 6; i++)
            {
                double vertexAngle = angleInRadians + i * Math.PI / 3;
                float xPos = (float)(innerSize * Math.Cos(vertexAngle));
                float yPos = (float)(innerSize * Math.Sin(vertexAngle));
                innerHexPoints[i] = new PointF(centerX + xPos, centerY + yPos);
            }

            using (Pen innerPen = new Pen(Color.FromArgb(200, 186, 85, 211), 2))
            {
                g.DrawPolygon(innerPen, innerHexPoints);
            }

            // Dibujar líneas desde el centro hacia los vértices
            using (Pen linePen = new Pen(Color.FromArgb(150, 147, 112, 219), 1))
            {
                for (int i = 0; i < 6; i++)
                {
                    g.DrawLine(linePen, centerX, centerY, hexPoints[i].X, hexPoints[i].Y);
                }
            }
        }
    }
}
