using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GkProj3
{
    public unsafe class DirectBitmap
    {
        private readonly WriteableBitmap _bmp;
        public int Width { get; }
        public int Height { get; }
        private readonly int _stride;
        private readonly Byte[] _internalArray;
        private readonly int _pxC;

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            _bmp = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);
            _stride = _bmp.BackBufferStride;
            _internalArray = new byte[Height * _stride * 4];
            _pxC = _bmp.Format.BitsPerPixel / 8;
        }
        public DirectBitmap(BitmapImage img)
        {
            _bmp = new WriteableBitmap(img);
            Width = (int)img.Width;
            Height = (int)img.Height;
            _stride = _bmp.BackBufferStride;
            _internalArray = new byte[Height * _stride * 4];
            _pxC = _bmp.Format.BitsPerPixel / 8;
            _bmp.Lock();
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                {
                    Byte* px = (Byte*)_bmp.BackBuffer + _stride * y + _pxC * x;
                    _internalArray[y * _stride + _pxC * x + 0] = px[0];
                    _internalArray[y * _stride + _pxC * x + 1] = px[1];
                    _internalArray[y * _stride + _pxC * x + 2] = px[2];
                    _internalArray[y * _stride + _pxC * x + 3] = px[3];
                }
            _bmp.Unlock();
        }
        public ImageSource GetDrawable() => _bmp;
        public void PutPixel(int x, int y, Color color)
        {
            if (x >= Width || y >= Height)
                return;

            _internalArray[y * _stride + _pxC * x + 0] = color.B;
            _internalArray[y * _stride + _pxC * x + 1] = color.G;
            _internalArray[y * _stride + _pxC * x + 2] = color.R;
            _internalArray[y * _stride + _pxC * x + 3] = color.A;
        }
        public Color GetPixel(int x, int y)
        {
            int xp = x % Width;
            int yp = y % Height;

            return Color.FromArgb(_internalArray[yp * _stride + _pxC * xp + 3], _internalArray[yp * _stride + _pxC * xp + 2], _internalArray[yp * _stride + _pxC * xp + 1], _internalArray[yp * _stride + _pxC * xp]);
        }
        public void Lock()
        {
            _bmp.Lock();
        }
        public void Unlock()
        {
            _bmp.Unlock();
        }
        public void UpdateBitmap()
        {
            _bmp.WritePixels(new Int32Rect(0, 0, Width, Height), _internalArray, _stride, 0);
            _bmp.AddDirtyRect(new Int32Rect(0, 0, _bmp.PixelWidth, _bmp.PixelHeight));
        }
        public void Clear()
        {
            Array.Clear(_internalArray, 0, Height * _stride * 4);
        }
    }
}
