namespace RayTracing.Textures
{
    using System;
    using System.IO;
    using System.Numerics;
    using StbImageSharp;

    public record ImageTexture() : Texture
    {
        public const ColorComponents BytesPerPixel = ColorComponents.RedGreenBlue;

        public ImageTexture(string filePath)
            : this()
        {
            using FileStream stream = File.OpenRead(filePath);
            this.Data = ImageResult.FromStream(stream, ImageTexture.BytesPerPixel);
        }

        private ImageResult? Data { get; init; }

        public override Vector3 Value(float u, float v, Vector3 point)
        {
            if (this.Data?.Data is null)
            {
                return new Vector3(0, 1, 1);
            }

            u = Math.Clamp(u, 0, 1);
            v = 1 - Math.Clamp(v, 0, 1);

            int i = (int)(u * this.Data.Width);
            int j = (int)(v * this.Data.Height);

            if (i >= this.Data.Width)
            {
                i = this.Data.Width - 1;
            }

            if (j >= this.Data.Height)
            {
                j = this.Data.Height - 1;
            }

            const float colorScale = 1 / 255f;
            int bytesPerScanline = (int)ImageTexture.BytesPerPixel * this.Data.Width;
            int index = (j * bytesPerScanline) + (i * (int)ImageTexture.BytesPerPixel);
            byte[] data = this.Data.Data;
            return new Vector3(data[index], data[index + 1], data[index + 2]) * colorScale;
        }
    }
}
