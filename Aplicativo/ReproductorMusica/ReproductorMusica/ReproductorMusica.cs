using AxWMPLib;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TagLib;

namespace ReproductorMusica
{
    public partial class ReproductorMusica : Form
    {
        private float hexagonPulseFactor = 1f;
        private float lastIntensity = 0f;
       // private Image albumCover;
        private List<Particle> particles = new List<Particle>();
        private Random rand = new Random();
        private float[] visualizationBuffer = new float[1024]; // Buffer de audio
        private float intensity = 0f; // Intensidad de la música para animaciones

        private Color[][] visualizationThemes = new Color[][]
        {
          new Color[] { Color.MediumPurple, Color.BlueViolet, Color.DarkOrchid },
            new Color[] { Color.Crimson, Color.OrangeRed, Color.Gold },
            new Color[] { Color.LightSeaGreen, Color.Turquoise, Color.CadetBlue }
        };
        private int currentThemeIndex = 0;

        private int themeFrameCounter = 0;
        // Carpeta donde están tus canciones
        private string carpetaMusica = @"D:\Music\";

        // Lista que se llenará automáticamente con todos los MP3 de la carpeta
        private List<string> canciones;

        private int indiceActual = 0; // Para saber qué canción se está reproduciendo

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
            canciones = Directory.GetFiles(carpetaMusica, "*.mp3").ToList();

            if (canciones.Count > 0)
            {
                // Preparar la primera canción
                SetupAudio(canciones[indiceActual]);
            }
        }
        private void Meter_StreamVolume(object sender, StreamVolumeEventArgs e)
        {
            for (int i = 0; i < e.MaxSampleValues.Length && i < visualizationBuffer.Length; i++)
            {
                // Amplificar para que se vea mejor
                visualizationBuffer[i] = e.MaxSampleValues[i] * 5f;
                // Limitar a rango [-1,1] para evitar que se salga del PictureBox
                if (visualizationBuffer[i] > 1f) visualizationBuffer[i] = 1f;
                if (visualizationBuffer[i] < -1f) visualizationBuffer[i] = -1f;
            }
        }
        private void btnSiguiente_Click(object sender, EventArgs e)
        {
            if (canciones.Count == 0) return;

            indiceActual++;
            if (indiceActual >= canciones.Count)
                indiceActual = 0; // Vuelve al inicio si llega al final

            // Configura el audio de la nueva canción
            SetupAudio(canciones[indiceActual]);
            audioFile.Volume = trackBarVolumen.Value / 100f;
            outputDevice.Play();
        }
        private void btnAnterior_Click(object sender, EventArgs e)
        {

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
            if (themeFrameCounter > 300)
            {
                currentThemeIndex = (currentThemeIndex + 1) % visualizationThemes.Length;
                themeFrameCounter = 0;
            }
        }
        private void CheckBeat()
        {
            if (intensity > 0.6f && intensity > lastIntensity * 1.3f)
            {
                hexagonPulseFactor = 1.3f; 
            }
            lastIntensity = intensity;
        }

        private void InitializeVisualization()
        {
            bufferBitmap = new Bitmap(pbVisualizer.Width, pbVisualizer.Height);
            bufferGraphics = Graphics.FromImage(bufferBitmap);
            bufferGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            bufferGraphics.Clear(Color.Black);

            hexagon = new CHexagon(120f);
            triangle = new CTriangle(90f);

            visualizationTimer = new Timer();
            visualizationTimer.Interval = 33; 
            visualizationTimer.Tick += VisualizationTimer_Tick;
            visualizationTimer.Start(); 
        }
        private void SetupAudio(string path)
        {
            outputDevice?.Stop();
            outputDevice?.Dispose();
            audioFile?.Dispose();
            lblCancion.Text = Path.GetFileName(canciones[indiceActual]);

            audioFile = new AudioFileReader(path);
            var sampleProvider = audioFile.ToSampleProvider();

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
           // try
           // {
               // var tfile = TagLib.File.Create(path);

               // if (tfile.Tag.Pictures != null && tfile.Tag.Pictures.Length > 0)
               // {
                    //var bin = (byte[])(tfile.Tag.Pictures[0].Data.Data);
                    //using (var ms = new MemoryStream(bin))
                    //{
                       // albumCover = Image.FromStream(ms);
                      //  pictureBoxCover.Image = albumCover; // Muestra la carátula en tu PictureBox
                    //    pictureBoxCover.SizeMode = PictureBoxSizeMode.StretchImage;
                  //  }
                //}
               // else
               // {
                    // Si no tiene carátula, opcionalmente muestra una imagen por defecto
                   // pictureBoxCover.Image = Properties.Resources.defaultCover; // opcional
             //   }
           // }
           // catch (Exception ex)
           // {
           //     Console.WriteLine("No se pudo obtener carátula: " + ex.Message);
           // }

            // Inicializar la salida de audio
            outputDevice = new WaveOutEvent();
            outputDevice.Init(meter);
        }


        private void VisualizationTimer_Tick(object sender, EventArgs e)
        {
            UpdateTheme();

            CheckBeat();
            ActualizarTiempo();
            // 3) Limpiar canvas
            bufferGraphics.Clear(Color.Black);

            float centerX = pbVisualizer.Width / 2f;
            float centerY = pbVisualizer.Height / 2f;

            Color[] themeColors = visualizationThemes[currentThemeIndex];

            pulsingCircles = new CPulsingCircles(bufferGraphics, pbVisualizer.Width, pbVisualizer.Height, frameCount, intensity, themeColors);
            pulsingCircles.Draw();

            float hexSpeed = -2.5f * intensity;
            hexagon.RotateAndDraw(hexSpeed * hexagonPulseFactor, bufferGraphics, centerX, centerY);
            hexagonPulseFactor = Math.Max(1f, hexagonPulseFactor - 0.02f);

            float triSpeed = 3f * intensity;
            triangle.Rotate(triSpeed, bufferGraphics, centerX, centerY);

            GenerateParticles();

            pbVisualizer.Image = (Bitmap)bufferBitmap.Clone();
            frameCount++;
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
                // Pausar música y animación
                outputDevice.Pause();
                isPaused = true;
                visualizationTimer.Stop(); // Detiene las burbujas
                btnPlayPause.Text = "▶️";
            }
            else
            {
                if (isPaused)
                {
                    // Reanudar música y animación
                    outputDevice.Play();
                    isPaused = false;
                    visualizationTimer.Start(); // Reanuda las burbujas
                    btnPlayPause.Text = "⏸️";
                }
                else
                {
                    // Si estaba detenido, reinicia desde el inicio
                    audioFile.Position = 0;
                    outputDevice.Play();
                    isPaused = false;
                    visualizationTimer.Start(); // Inicia burbujas desde cero
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

        private void ReproductorMusica_Load(object sender, EventArgs e)
        {

        }
        private void ActualizarTiempo()
        {
            if (audioFile != null)
            {
                TimeSpan tiempoActual = audioFile.CurrentTime;
                TimeSpan duracionTotal = audioFile.TotalTime;

                lblTiempo.Text = $"{tiempoActual.Minutes:D2}:{tiempoActual.Seconds:D2} / " +
                                 $"{duracionTotal.Minutes:D2}:{duracionTotal.Seconds:D2}";
            }
            else
            {
                lblTiempo.Text = "00:00 / 00:00";
            }
        }
    }
}