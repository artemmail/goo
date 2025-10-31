using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace GooLib
{
    public static class Effects
    {
        public static void Skipes(bool filter, Bitmap dstBmp, Bitmap srcBmp, PointF pos, PointF shift, float radius, int count)
        {
            if (dstBmp is null)
            {
                throw new ArgumentNullException(nameof(dstBmp));
            }

            if (srcBmp is null)
            {
                throw new ArgumentNullException(nameof(srcBmp));
            }

            if (radius <= 0 || count <= 0)
            {
                CopyBitmap(dstBmp, srcBmp);
                return;
            }

            using var locker = new BitmapLocker(dstBmp, srcBmp);

            unsafe
            {
                ProcessSpikes(filter, locker.DestinationPixels, locker.SourcePixels, srcBmp.Width, srcBmp.Height, pos, shift, radius, count);
            }
        }

        public static void Smear(bool filter, Bitmap dstBmp, Bitmap srcBmp, PointF pos, PointF shift, float radius)
        {
            if (dstBmp is null)
            {
                throw new ArgumentNullException(nameof(dstBmp));
            }

            if (srcBmp is null)
            {
                throw new ArgumentNullException(nameof(srcBmp));
            }

            if (radius <= 0)
            {
                CopyBitmap(dstBmp, srcBmp);
                return;
            }

            using var locker = new BitmapLocker(dstBmp, srcBmp);

            unsafe
            {
                ProcessSmear(filter, locker.DestinationPixels, locker.SourcePixels, srcBmp.Width, srcBmp.Height, pos, shift, radius);
            }
        }

        private static void CopyBitmap(Bitmap dstBmp, Bitmap srcBmp)
        {
            using Graphics gr = Graphics.FromImage(dstBmp);
            gr.DrawImage(srcBmp, Point.Empty);
        }

        private static unsafe void ProcessSpikes(bool filter, uint* pDst, uint* pSrc, int width, int height, PointF pos, PointF shift, float radius, int k)
        {
            Vector2 shiftVector = new Vector2(shift.X, shift.Y);
            float shiftLength = shiftVector.Length();
            float amplitude = radius != 0 ? shiftLength / radius : 0f;

            for (int i = 0; i < height; i++)
            {
                uint* dstRow = pDst + (nint)(i * width);
                for (int j = 0; j < width; j++)
                {
                    float x = j;
                    float y = i;
                    float dx = x - pos.X;
                    float dy = y - pos.Y;

                    float angle = MathF.Atan2(dy, dx);
                    float distance = MathF.Sqrt(dx * dx + dy * dy);

                    if (distance < radius)
                    {
                        float s = 1.0001f - amplitude * MathF.Sin(k * angle);
                        float inner = s * s + 4f * distance / radius * (1f - s);
                        float denominator = 2f * (1f - s);
                        if (denominator != 0f)
                        {
                            distance = radius * (MathF.Sqrt(MathF.Max(inner, 0f)) - s) / denominator;
                        }
                    }

                    float sampleX = distance * MathF.Cos(angle) + pos.X;
                    float sampleY = distance * MathF.Sin(angle) + pos.Y;

                    dstRow[j] = filter
                        ? SampleLinear(pSrc, width, height, sampleX, sampleY)
                        : SampleNearest(pSrc, width, height, sampleX, sampleY);
                }
            }
        }

        private static unsafe void ProcessSmear(bool filter, uint* pDst, uint* pSrc, int width, int height, PointF pos, PointF shift, float radius)
        {
            Vector2 shiftVector = new Vector2(shift.X, shift.Y);
            float amplitude = shiftVector.Length();
            Vector2 dirY = amplitude > 0 ? Vector2.Normalize(shiftVector) : new Vector2(0f, 1f);
            Vector2 dirX = new Vector2(dirY.Y, -dirY.X);
            float maxAmp = radius * 0.5f;
            float scaleY = maxAmp > 0 && amplitude > maxAmp ? amplitude / maxAmp : 1f;

            for (int i = 0; i < height; i++)
            {
                uint* dstRow = pDst + (nint)(i * width);
                for (int j = 0; j < width; j++)
                {
                    float x = j;
                    float y = i;
                    float dx = x - pos.X;
                    float dy = y - pos.Y;

                    float xx = dx * dirX.X + dy * dirX.Y;
                    float yy = dx * dirY.X + dy * dirY.Y;

                    if (MathF.Abs(xx) < radius && MathF.Abs(yy) < scaleY * radius)
                    {
                        float ae = 3f * xx / radius;
                        ae = amplitude * MathF.Exp(-ae * ae);
                        float signY = yy >= ae ? 1f : -1f;
                        float denominator = 1f - signY * ae / (scaleY * radius);
                        if (MathF.Abs(denominator) > 1e-5f)
                        {
                            yy = (yy - ae) / denominator;
                        }
                        else
                        {
                            yy = 0f;
                        }

                        Vector2 transformed = dirX * xx + dirY * yy;
                        x = pos.X + transformed.X;
                        y = pos.Y + transformed.Y;
                    }

                    dstRow[j] = filter
                        ? SampleLinear(pSrc, width, height, x, y)
                        : SampleNearest(pSrc, width, height, x, y);
                }
            }
        }

        private static unsafe uint SampleNearest(uint* pSrc, int width, int height, float x, float y)
        {
            int si = Clamp((int)MathF.Floor(y + 0.5f), 0, height - 1);
            int sj = Clamp((int)MathF.Floor(x + 0.5f), 0, width - 1);
            return pSrc[si * width + sj];
        }

        private static unsafe uint SampleLinear(uint* pSrc, int width, int height, float x, float y)
        {
            float floorX = MathF.Floor(x);
            float floorY = MathF.Floor(y);
            float w10 = x - floorX;
            float w11 = w10;
            float w00 = 1f - w10;
            float w01 = w00;
            float remainder = y - floorY;
            w01 *= remainder;
            w11 *= remainder;
            remainder = 1f - remainder;
            w00 *= remainder;
            w10 *= remainder;

            int si = Clamp((int)floorY, 0, height - 1);
            int sj = Clamp((int)floorX, 0, width - 1);
            int si1 = Clamp(si + 1, 0, height - 1);
            int sj1 = Clamp(sj + 1, 0, width - 1);

            Vector4 v00 = UnpackColor(pSrc[si * width + sj]) * w00;
            Vector4 v10 = UnpackColor(pSrc[si * width + sj1]) * w10;
            Vector4 v01 = UnpackColor(pSrc[si1 * width + sj]) * w01;
            Vector4 v11 = UnpackColor(pSrc[si1 * width + sj1]) * w11;

            Vector4 color = v00 + v10 + v01 + v11;
            return PackColor(color);
        }

        private static Vector4 UnpackColor(uint color)
        {
            float b = (color & 0xFF) / 255f;
            float g = ((color >> 8) & 0xFF) / 255f;
            float r = ((color >> 16) & 0xFF) / 255f;
            float a = ((color >> 24) & 0xFF) / 255f;
            return new Vector4(b, g, r, a);
        }

        private static uint PackColor(Vector4 color)
        {
            color = Vector4.Clamp(color, Vector4.Zero, Vector4.One);
            uint b = (uint)MathF.Round(color.X * 255f);
            uint g = (uint)MathF.Round(color.Y * 255f);
            uint r = (uint)MathF.Round(color.Z * 255f);
            uint a = (uint)MathF.Round(color.W * 255f);
            return b | (g << 8) | (r << 16) | (a << 24);
        }

        private static int Clamp(int value, int min, int max) => value < min ? min : (value > max ? max : value);

        private sealed unsafe class BitmapLocker : IDisposable
        {
            private readonly Bitmap _dstBmp;
            private readonly Bitmap _srcBmp;
            private readonly BitmapData _dstData;
            private readonly BitmapData _srcData;

            public BitmapLocker(Bitmap dstBmp, Bitmap srcBmp)
            {
                if (dstBmp.Width != srcBmp.Width || dstBmp.Height != srcBmp.Height)
                {
                    throw new ArgumentException("Source and destination bitmaps must have the same dimensions.");
                }

                _dstBmp = dstBmp;
                _srcBmp = srcBmp;
                Rectangle rect = new Rectangle(0, 0, srcBmp.Width, srcBmp.Height);
                _srcData = srcBmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                _dstData = dstBmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            }

            public uint* DestinationPixels => (uint*)_dstData.Scan0.ToPointer();

            public uint* SourcePixels => (uint*)_srcData.Scan0.ToPointer();

            public void Dispose()
            {
                _dstBmp.UnlockBits(_dstData);
                _srcBmp.UnlockBits(_srcData);
            }
        }
    }
}

