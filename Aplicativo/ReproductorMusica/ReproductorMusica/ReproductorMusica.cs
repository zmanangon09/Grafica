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
        public ReproductorMusica()
        {
            InitializeComponent();
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
    }
}