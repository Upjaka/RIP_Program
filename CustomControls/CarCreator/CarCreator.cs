using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.IO;

namespace AvaloniaApplication2.CustomControls
{
    public class CarCreator
    {
        public static Bitmap CreateImage(bool isFixed, bool isLoaded)
        {
            // Размер изображения
            int width = 12;
            int height = 24;

            // Создание RenderTargetBitmap
            var renderTarget = new RenderTargetBitmap(new PixelSize(width, height), new Vector(96, 96));

            // Создание контекста для рисования
            using (var context = renderTarget.CreateDrawingContext())
            {
                Color rectangleColor = Colors.Red;
                if (isFixed)
                {
                    rectangleColor = (isLoaded) ? Colors.Blue : Colors.Black;
                }
                // Рисование прямоугольника с переданным цветом
                var brush = Brushes.Transparent;
                var pen = new Pen(new SolidColorBrush(rectangleColor), 1.5f);
                var rectangleGeometry = new RectangleGeometry(new Rect(0, 0, width, height));

                var triangleGeometry = new PathGeometry();
                var pathFigure = new PathFigure { StartPoint = new Point(2, 24) };

                var lineSegmets = new PathSegments
                {
                    new LineSegment { Point = new Point(2, 18) },
                    new LineSegment { Point = new Point(6, 18) },
                    new LineSegment { Point = new Point(2, 24) }
                };

                pathFigure.Segments = lineSegmets;
                triangleGeometry.Figures = new PathFigures { pathFigure };

                context.DrawGeometry(brush, pen, triangleGeometry);

                triangleGeometry = new PathGeometry();
                pathFigure = new PathFigure { StartPoint = new Point(10, 24) };

                lineSegmets = new PathSegments
                {
                    new LineSegment { Point = new Point(10, 18) },
                    new LineSegment { Point = new Point(6, 18) },
                    new LineSegment { Point = new Point(10, 24) }
                };

                pathFigure.Segments = lineSegmets;
                triangleGeometry.Figures = new PathFigures { pathFigure };

                context.DrawGeometry(brush, pen, triangleGeometry);

                triangleGeometry = new PathGeometry();
                pathFigure = new PathFigure { StartPoint = new Point(6, 18) };

                lineSegmets = new PathSegments
                {
                    new LineSegment { Point = new Point(0, 12) },
                    new LineSegment { Point = new Point(6, 6) },
                    new LineSegment { Point = new Point(12, 12) },
                    new LineSegment { Point = new Point(6, 18) }
                };

                pathFigure.Segments = lineSegmets;
                triangleGeometry.Figures = new PathFigures { pathFigure };

                context.DrawGeometry(brush, pen, triangleGeometry);

                /**
                var startPoint = new Point(3, 4);
                var endPoint = new Point(0, 7);
                context.DrawLine(pen, startPoint, endPoint);
                endPoint = new Point(6, 7);
                context.DrawLine(pen, startPoint, endPoint);
                startPoint = new Point(0, 12);
                context.DrawLine(pen, startPoint, endPoint);
                startPoint = new Point(0, 6);
                endPoint = new Point(6, 12);
                context.DrawLine(pen, startPoint, endPoint);
                */
            }

            return renderTarget;
        }


    }
}
