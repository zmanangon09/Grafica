using System;
using System.Drawing;

public class CWaveLine
{
    private int width;
    private int height;
    private float[] samples; // Aquí guardaremos las muestras
    private int maxSamples;

    public CWaveLine(int width, int height, int maxSamples = 1024)
    {
        this.width = width;
        this.height = height;
        this.maxSamples = maxSamples;
        samples = new float[maxSamples];
    }

    // Actualiza las muestras para dibujar
    public void UpdateSamples(float[] inputSamples)
    {
        int len = Math.Min(inputSamples.Length, maxSamples);
        Array.Copy(inputSamples, samples, len);
    }

    // Dibuja la línea en el Graphics
    public void Draw(Graphics g)
    {
        if (samples == null || samples.Length == 0)
            return;

        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        Pen pen = new Pen(Color.Lime, 2f);

        int midY = height / 2;
        float xStep = (float)width / (float)samples.Length;

        for (int i = 0; i < samples.Length - 1; i++)
        {
            float x1 = i * xStep;
            float y1 = midY - samples[i] * (midY); // Mapear [-1,1] a píxeles
            float x2 = (i + 1) * xStep;
            float y2 = midY - samples[i + 1] * (midY);

            g.DrawLine(pen, x1, y1, x2, y2);
        }

        pen.Dispose();
    }
}
