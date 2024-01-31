using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;

namespace YKWrandomizer.Level5.Image
{
    public class IMGCSwizzle
    {
        MasterSwizzle _zorderTrans;

        public int Width { get; }
        public int Height { get; }

        public IMGCSwizzle(int width, int height)
        {
            Width = (width + 0x7) & ~0x7;
            Height = (height + 0x7) & ~0x7;

            _zorderTrans = new MasterSwizzle(Width, new Point(0, 0), new[] { (0, 1), (1, 0), (0, 2), (2, 0), (0, 4), (4, 0) });
        }

        public Point Get(Point point)
        {
            return _zorderTrans.Get(point.Y * Width + point.X);
        }

        public IEnumerable<Point> GetPointSequence()
        {
            int strideWidth = Width;
            int strideHeight = Height;

            for (int i = 0; i < strideWidth * strideHeight; i++)
            {
                var point = new Point(i % strideWidth, i / strideWidth);
                if (_zorderTrans != null)
                    point = _zorderTrans.Get(point.Y * Width + point.X);

                yield return point;
            }
        }
    }

    public class MasterSwizzle
    {
        IEnumerable<(int, int)> _bitFieldCoords;
        IEnumerable<(int, int)> _initPointTransformOnY;

        public int MacroTileWidth { get; }
        public int MacroTileHeight { get; }

        int _widthInTiles;
        Point _init;

        /// <summary>
        /// Creates an instance of MasterSwizzle
        /// </summary>
        /// <param name="imageStride">Pixelcount of dimension in which should get aligned</param>
        /// <param name="init">the initial point, where the swizzle begins</param>
        /// <param name="bitFieldCoords">Array of coordinates, assigned to every bit in the macroTile</param>
        /// <param name="initPointTransformOnY">Defines a transformation array of the initial point with changing Y</param>
        public MasterSwizzle(int imageStride, Point init, IEnumerable<(int, int)> bitFieldCoords, IEnumerable<(int, int)> initPointTransformOnY = null)
        {
            _bitFieldCoords = bitFieldCoords;
            _initPointTransformOnY = initPointTransformOnY ?? Enumerable.Empty<(int, int)>();

            _init = init;

            MacroTileWidth = bitFieldCoords.Select(p => p.Item1).Aggregate((x, y) => x | y) + 1;
            MacroTileHeight = bitFieldCoords.Select(p => p.Item2).Aggregate((x, y) => x | y) + 1;
            _widthInTiles = (imageStride + MacroTileWidth - 1) / MacroTileWidth;
        }

        /// <summary>
        /// Transforms a given pointCount into a point
        /// </summary>
        /// <param name="pointCount">The overall pointCount to be transformed</param>
        /// <returns>The Point, which got calculated by given settings</returns>
        public Point Get(int pointCount)
        {
            var macroTileCount = pointCount / MacroTileWidth / MacroTileHeight;
            var (macroX, macroY) = (macroTileCount % _widthInTiles, macroTileCount / _widthInTiles);

            return new[] { (macroX * MacroTileWidth, macroY * MacroTileHeight) }
                .Concat(_bitFieldCoords.Where((v, j) => (pointCount >> j) % 2 == 1))
                .Concat(_initPointTransformOnY.Where((v, j) => (macroY >> j) % 2 == 1))
                .Aggregate(_init, (a, b) => new Point(a.X ^ b.Item1, a.Y ^ b.Item2));
        }
    }
}
