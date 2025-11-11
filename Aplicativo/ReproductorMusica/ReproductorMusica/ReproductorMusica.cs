using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using NAudio.Wave;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReproductorMusica
{
    public partial class ReproductorMusica : Form
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private bool volumenVisible = false;
        private bool isPaused = false;
        private bool isUserDragging = false;

        // Componentes de visualización
        private Bitmap bufferBitmap;
        private Graphics bufferGraphics;
        private CHexagon hexagon;
        private CPulsingCircles pulsingCircles;
        private CTriangle triangle;
        private int frameCount = 0;
        private Timer visualizationTimer;

        public ReproductorMusica()
        {
            InitializeComponent();
            InitializeVisualization();
        }

        private void InitializeVisualization()
        {
            // Inicializar buffer de visualización
            bufferBitmap = new Bitmap(pbVisualizer.Width, pbVisualizer.Height);
            bufferGraphics = Graphics.FromImage(bufferBitmap);
            bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            bufferGraphics.Clear(Color.Black);

            // Inicializar figuras
            hexagon = new CHexagon(120f);
            triangle = new CTriangle(90f);

            // Timer para animación
            visualizationTimer = new Timer();
            visualizationTimer.Interval = 33; // ~30 FPS
            visualizationTimer.Tick += VisualizationTimer_Tick;
            visualizationTimer.Start(); // Iniciar la visualización aunque no haya música
        }

        private void VisualizationTimer_Tick(object sender, EventArgs e)
        {
            RenderVisualization();
        }

        private void RenderVisualization()
        {
            // Efecto de desvanecimiento
            using (SolidBrush fadeBrush = new SolidBrush(Color.FromArgb(40, 0, 0, 0)))
            {
                bufferGraphics.FillRectangle(fadeBrush, 0, 0, bufferBitmap.Width, bufferBitmap.Height);
            }

            float centerX = bufferBitmap.Width / 2f;
            float centerY = bufferBitmap.Height / 2f;

            // Calcular intensidad basada en el volumen y estado de reproducción
            float intensity = 1.0f;
            if (audioFile != null && outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                // Usar el volumen como factor de intensidad (más intensidad cuando hay música)
                intensity = audioFile.Volume * 2.0f;
            }
            else
            {
                // Animación suave cuando no hay música
                intensity = 0.3f;
            }

            // Dibujar círculos pulsantes con intensidad
            pulsingCircles = new CPulsingCircles(bufferGraphics, bufferBitmap.Width, bufferBitmap.Height, frameCount, intensity);
            pulsingCircles.Draw();

            // Rotar y dibujar hexágono (más rápido con música)
            float hexSpeed = outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing ? -2.5f * intensity : -0.5f;
            hexagon.RotateAndDraw(hexSpeed, bufferGraphics, centerX, centerY);

            // Rotar y dibujar triángulo (más rápido con música)
            float triSpeed = outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing ? 3f * intensity : 0.5f;
            triangle.Rotate(triSpeed, bufferGraphics, centerX, centerY);

            frameCount++;
            if (frameCount > 100) frameCount = 0;

            // Actualizar canvas
            pbVisualizer.Image = (Bitmap)bufferBitmap.Clone();
        }

        private void btnVolumen_Click(object sender, EventArgs e)
        {
            // Mostrar u ocultar el control de volumen
            volumenVisible = !volumenVisible;
            trackBarVolumen.Visible = volumenVisible;
        }

        private void trackBarVolumen_Scroll(object sender, EventArgs e)
        {
            if (audioFile != null)
                audioFile.Volume = trackBarVolumen.Value / 100f;
        }

        private void btnCargar_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Archivos de audio|*.mp3;*.wav";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Si ya hay una canción cargada, la detenemos
                    if (outputDevice != null)
                    {
                        outputDevice.Stop();
                        outputDevice.Dispose();
                        audioFile.Dispose();
                    }

                    audioFile = new AudioFileReader(ofd.FileName);
                    outputDevice = new WaveOutEvent();
                    outputDevice.Init(audioFile);

                    // Configurar volumen inicial
                    audioFile.Volume = trackBarVolumen.Value / 100f;

                    // Configurar barra de progreso
                    trackBarProgreso.Value = 0;
                    trackBarProgreso.Maximum = (int)audioFile.TotalTime.TotalSeconds;
                    btnPlayPause.Text = "▶️";

                    timer1.Start();

                    MessageBox.Show("Archivo cargado correctamente. Presiona Play para reproducir.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar el archivo: " + ex.Message);
                }
            }
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            if (outputDevice == null || audioFile == null)
            {
                MessageBox.Show("Primero carga un archivo de audio.");
                return;
            }

            if (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                outputDevice.Pause();
                isPaused = true;
                btnPlayPause.Text = "▶️";
            }
            else
            {
                if (isPaused)
                {
                    outputDevice.Play();
                    isPaused = false;
                    btnPlayPause.Text = "⏸️";
                }
                else
                {
                    // Reinicia desde el inicio si estaba detenido
                    audioFile.Position = 0;
                    outputDevice.Play();
                    btnPlayPause.Text = "⏸️";
                }
            }
        }

        private void trackBarProgreso_Scroll(object sender, EventArgs e)
        {
            // Solo actualizamos si el usuario está soltando (opcional, para compatibilidad)
            if (!isUserDragging && audioFile != null)
            {
                audioFile.CurrentTime = TimeSpan.FromSeconds(trackBarProgreso.Value);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (audioFile != null && outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                int segundos = (int)audioFile.CurrentTime.TotalSeconds;

                // Evita error si el valor supera el máximo
                if (segundos >= 0 && segundos <= trackBarProgreso.Maximum)
                {
                    // Solo actualiza si el usuario no está moviendo manualmente
                    if (!isUserDragging)
                        trackBarProgreso.Value = segundos;
                }
                // Actualizar label con tiempo transcurrido y total
                lblTiempo.Text = $"{audioFile.CurrentTime.Minutes:D2}:{audioFile.CurrentTime.Seconds:D2} / " +
                                 $"{audioFile.TotalTime.Minutes:D2}:{audioFile.TotalTime.Seconds:D2}";
            }
        }

        private void trackBarProgreso_MouseDown(object sender, MouseEventArgs e)
        {
            isUserDragging = true;
        }

        private void trackBarProgreso_MouseUp(object sender, MouseEventArgs e)
        {
            if (audioFile != null)
            {
                // Cambia el tiempo de reproducción según la posición del trackbar
                audioFile.CurrentTime = TimeSpan.FromSeconds(trackBarProgreso.Value);
            }
            isUserDragging = false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Limpiar recursos
            visualizationTimer?.Stop();
            visualizationTimer?.Dispose();
            timer1?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();
            bufferGraphics?.Dispose();
            bufferBitmap?.Dispose();
            base.OnFormClosing(e);
        }
    }
}