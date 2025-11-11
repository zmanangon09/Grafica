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
using NAudio.Wave.SampleProviders;
using TagLib;
using System.IO;

namespace ReproductorMusica
{
    public partial class ReproductorMusica : Form
    {
        private float hexagonPulseFactor = 1f;
        private float lastIntensity = 0f;
        private Image albumCover;
        private List<Particle> particles = new List<Particle>();
        private Random rand = new Random();
        private float[] visualizationBuffer = new float[1024]; // Buffer de audio
        private float intensity = 0f; // Intensidad de la música para animaciones
                                      // --- Temas dinámicos ---
        private Color[][] visualizationThemes = new Color[][]
        {
    new Color[] { Color.MediumPurple, Color.BlueViolet, Color.DarkOrchid },
    new Color[] { Color.Crimson, Color.OrangeRed, Color.Gold },
    new Color[] { Color.LightSeaGreen, Color.Turquoise, Color.CadetBlue }
        };
        private int currentThemeIndex = 0;
        private int themeFrameCounter = 0;

        private class Particle
        {
            public float X;
            public float Y;
            public float SpeedY;
            public Color Color;
        }
        private MeteringSampleProvider meter;
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;
        private bool volumenVisible = false;
        private bool isPaused = false;
        private bool isUserDragging = false;
        private CWaveLine waveLine;
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
            waveLine = new CWaveLine(pbVisualizer.Width, pbVisualizer.Height);

        }
        private void Meter_StreamVolume(object sender, StreamVolumeEventArgs e)
        {
            // Copiar y amplificar amplitud para visualización dinámica
            for (int i = 0; i < e.MaxSampleValues.Length && i < visualizationBuffer.Length; i++)
            {
                // Amplificar para que se vea mejor
                visualizationBuffer[i] = e.MaxSampleValues[i] * 5f;
                // Limitar a rango [-1,1] para evitar que se salga del PictureBox
                if (visualizationBuffer[i] > 1f) visualizationBuffer[i] = 1f;
                if (visualizationBuffer[i] < -1f) visualizationBuffer[i] = -1f;
            }
        }
        private void GenerateParticles()
        {
            int numNewParticles = (int)(intensity * 5);
            for (int i = 0; i < numNewParticles; i++)
            {
                particles.Add(new Particle()
                {
                    X = rand.Next((int)pbVisualizer.Width),
                    Y = pbVisualizer.Height,
                    SpeedY = 1f + (float)rand.NextDouble() * 3f,
                    Color = visualizationThemes[currentThemeIndex][rand.Next(visualizationThemes[currentThemeIndex].Length)]
                });
            }

            for (int i = particles.Count - 1; i >= 0; i--)
            {
                Particle p = particles[i];
                p.Y -= p.SpeedY;
                if (p.Y < 0) particles.RemoveAt(i);
                else
                    using (Pen pen = new Pen(p.Color, 2))
                        bufferGraphics.DrawEllipse(pen, p.X, p.Y, 3, 3);
            }
        }

        private void UpdateTheme()
        {
            themeFrameCounter++;
            if (themeFrameCounter > 300) // cambia cada 300 frames (~10s)
            {
                currentThemeIndex = (currentThemeIndex + 1) % visualizationThemes.Length;
                themeFrameCounter = 0;
            }
        }
        private void CheckBeat()
        {
            if (intensity > 0.6f && intensity > lastIntensity * 1.3f)
            {
                hexagonPulseFactor = 1.3f; // pulso temporal
            }
            lastIntensity = intensity;
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
        private void SetupAudio(string path)
        {
            // Liberar recursos anteriores si existen
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();

            // Cargar el archivo de audio
            audioFile = new AudioFileReader(path);
            var sampleProvider = audioFile.ToSampleProvider();

            // Crear el medidor para obtener la intensidad del sonido
            meter = new MeteringSampleProvider(sampleProvider);
            meter.StreamVolume += (s, e) =>
            {
                int len = Math.Min(e.MaxSampleValues.Length, visualizationBuffer.Length);
                for (int i = 0; i < len; i++)
                    visualizationBuffer[i] = e.MaxSampleValues[i];

                // Calcular energía promedio del sonido (intensidad)
                float sum = 0f;
                for (int i = 0; i < len; i++)
                    sum += Math.Abs(visualizationBuffer[i]);

                intensity = sum / len;
                intensity = Math.Min(intensity * 5f, 1f); // amplifica la respuesta
            };

            // Obtener carátula del archivo de música
            try
            {
                var tfile = TagLib.File.Create(path);

                if (tfile.Tag.Pictures != null && tfile.Tag.Pictures.Length > 0)
                {
                    var bin = (byte[])(tfile.Tag.Pictures[0].Data.Data);
                    using (var ms = new MemoryStream(bin))
                    {
                        albumCover = Image.FromStream(ms);
                        pictureBoxCover.Image = albumCover; // Muestra la carátula en tu PictureBox
                        pictureBoxCover.SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
                else
                {
                    // Si no tiene carátula, opcionalmente muestra una imagen por defecto
                   // pictureBoxCover.Image = Properties.Resources.defaultCover; // opcional
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("No se pudo obtener carátula: " + ex.Message);
            }

            // Inicializar la salida de audio
            outputDevice = new WaveOutEvent();
            outputDevice.Init(meter);
        }


        private void VisualizationTimer_Tick(object sender, EventArgs e)
        {
            // 1) Actualiza tema (cambia color cada X frames)
            UpdateTheme();

            // 2) Detecta beats (actualiza hexagonPulseFactor si hay pico)
            CheckBeat();

            // 3) Limpiar canvas
            bufferGraphics.Clear(Color.Black);

            float centerX = pbVisualizer.Width / 2f;
            float centerY = pbVisualizer.Height / 2f;

            // 4) Colores del tema actual (usar el tema global ya definido)
            Color[] themeColors = visualizationThemes[currentThemeIndex];

            // 5) Círculos pulsantes -> pasar themeColors al constructor
            pulsingCircles = new CPulsingCircles(bufferGraphics, pbVisualizer.Width, pbVisualizer.Height, frameCount, intensity, themeColors);
            pulsingCircles.Draw();

            // 6) Hexágono: velocidad + pulso
            float hexSpeed = -2.5f * intensity;
            hexagon.RotateAndDraw(hexSpeed * hexagonPulseFactor, bufferGraphics, centerX, centerY);
            // decaimiento suave del pulso
            hexagonPulseFactor = Math.Max(1f, hexagonPulseFactor - 0.02f);

            // 7) Triángulo: rotación según intensidad
            float triSpeed = 3f * intensity;
            triangle.Rotate(triSpeed, bufferGraphics, centerX, centerY);

            // 8) Partículas que suben desde abajo según intensidad
            GenerateParticles();

            // 9) Pintar buffer al PictureBox
            pbVisualizer.Image = (Bitmap)bufferBitmap.Clone();
            frameCount++;
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
            float localIntensity = 1.0f;
            if (audioFile != null && outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing)
            {
                localIntensity = intensity; // usamos la intensidad real del audio
            }
            else
            {
                localIntensity = 0.3f;
            }

            // --- Colores del tema actual ---
            Color[] themeColors = visualizationThemes[currentThemeIndex];

            // Dibujar círculos pulsantes (pasando los colores del tema)
            pulsingCircles = new CPulsingCircles(bufferGraphics, bufferBitmap.Width, bufferBitmap.Height, frameCount, localIntensity, themeColors);
            pulsingCircles.Draw();

            // Rotar y dibujar hexágono
            float hexSpeed = outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing ? -2.5f * localIntensity : -0.5f;
            hexagon.RotateAndDraw(hexSpeed, bufferGraphics, centerX, centerY);

            // Rotar y dibujar triángulo
            float triSpeed = outputDevice != null && outputDevice.PlaybackState == PlaybackState.Playing ? 3f * localIntensity : 0.5f;
            triangle.Rotate(triSpeed, bufferGraphics, centerX, centerY);

            // Actualizar contador de frames
            frameCount++;
            if (frameCount > 100) frameCount = 0;

            // Actualizar visualización en el PictureBox
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
                outputDevice?.Stop();
                outputDevice?.Dispose();
                audioFile?.Dispose();

                // Inicializamos audio y meter
                SetupAudio(ofd.FileName);

                audioFile.Volume = trackBarVolumen.Value / 100f;

                trackBarProgreso.Value = 0;
                trackBarProgreso.Maximum = (int)audioFile.TotalTime.TotalSeconds;
                btnPlayPause.Text = "▶️";

                if (waveLine == null)
                    waveLine = new CWaveLine(pbVisualizer.Width, pbVisualizer.Height);

                visualizationTimer.Start();
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

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}