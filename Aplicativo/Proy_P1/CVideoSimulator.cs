using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Proy_P1
{
    internal class CVideoSimulator
    {
        public PictureBox canvas;
        public Timer animationTimer;
        public CHexagon hexagon;
        public CPulsingCircles pulsingCircles;
        public CTriangle triangle;

        public Bitmap bufferBitmap;
        public Graphics bufferGraphics;

        public const float hexagonSize = 120f;

        private int frameCount = 0;

        public CVideoSimulator(PictureBox pictureBox)
        {
            canvas = pictureBox;

            // Inicializar hexágono en lugar de estrella
            hexagon = new CHexagon(hexagonSize);

            // Inicializar triángulo
            triangle = new CTriangle(90f);

            // Crear buffer
            bufferBitmap = new Bitmap(canvas.Width, canvas.Height);
            bufferGraphics = Graphics.FromImage(bufferBitmap);
            bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            bufferGraphics.Clear(Color.Black);

            animationTimer = new Timer();
            animationTimer.Interval = 200;
            animationTimer.Tick += OnAnimationTick;
        }

        public void Start()
        {
            animationTimer.Start();
        }

        public void Stop()
        {
            animationTimer.Stop();
        }

        public void OnAnimationTick(object sender, EventArgs e)
        {
            // Efecto de desvanecimiento
            SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));
            bufferGraphics.FillRectangle(fadeBrush, 0, 0, bufferBitmap.Width, bufferBitmap.Height);

            float centerX = bufferBitmap.Width / 2f;
            float centerY = bufferBitmap.Height / 2f;

            // Dibujar círculos pulsantes
            pulsingCircles = new CPulsingCircles(bufferGraphics, bufferBitmap.Width, bufferBitmap.Height, frameCount);
            pulsingCircles.Draw();

            // Rotar y dibujar hexágono
            hexagon.RotateAndDraw(-1.5f, bufferGraphics, centerX, centerY);

            // Rotar y dibujar triángulo
            triangle.Rotate(2f, bufferGraphics, centerX, centerY);

            frameCount++;
            if (frameCount > 100) frameCount = 0;

            // Actualizar canvas
            canvas.Image = bufferBitmap;
        }

        public void RenderFrame(int frameNumber)
        {
            SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0));
            bufferGraphics.FillRectangle(fadeBrush, 0, 0, bufferBitmap.Width, bufferBitmap.Height);

            float centerX = bufferBitmap.Width / 2f;
            float centerY = bufferBitmap.Height / 2f;

            // Círculos pulsantes
            pulsingCircles = new CPulsingCircles(bufferGraphics, bufferBitmap.Width, bufferBitmap.Height, frameNumber);
            pulsingCircles.Draw();

            // Hexágono con ángulo específico
            hexagon.SetRotation(frameNumber * -1.5f);
            hexagon.RotateAndDraw(0f, bufferGraphics, centerX, centerY);

            // Triángulo con ángulo específico
            triangle.SetAngle(frameNumber * 2f);
            triangle.Rotate(0f, bufferGraphics, centerX, centerY);

            canvas.Image = bufferBitmap;
        }
    }
}