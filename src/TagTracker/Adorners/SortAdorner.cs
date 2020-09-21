using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

namespace TagTracker.Adorners
{
    public class SortAdorner : Adorner
    {
        private readonly FrameworkElement _adornedElement;
        private readonly ListSortDirection _direction;
        private readonly ImageSource _sortGlyph;

        public SortAdorner(
            FrameworkElement adornedElement,
            ListSortDirection direction,
            ImageSource sortGlyph)
            : base(adornedElement)
        {
            _adornedElement = adornedElement;
            _direction = direction;
            _sortGlyph = sortGlyph;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (_sortGlyph != null)
            {
                double x = _adornedElement.ActualWidth - 13;
                double y = (_adornedElement.ActualHeight / 2) - 5;
                Rect rect = new Rect(x, y, 10, 10);
                drawingContext.DrawImage(_sortGlyph, rect);
            }
            else
            {
                drawingContext.DrawGeometry(
                    Brushes.LightGray,
                    new Pen(Brushes.Gray, 1.0),
                    GetDefaultGlyph());
            }
        }

        private Geometry GetDefaultGlyph()
        {
            double x1 = _adornedElement.ActualWidth - 13;
            double x2 = x1 + 10;
            double x3 = x1 + 5;
            double y1 = (_adornedElement.ActualHeight / 2) - 3;
            double y2 = y1 + 5;

            if (_direction == ListSortDirection.Ascending)
            {
                double tmp = y1;
                y1 = y2;
                y2 = tmp;
            }

            PathSegmentCollection pathSegmentCollection = new PathSegmentCollection
            {
                new LineSegment(new Point(x2, y1), true),
                new LineSegment(new Point(x3, y2), true)
            };

            PathFigure pathFigure = new PathFigure(
                new Point(x1, y1),
                pathSegmentCollection,
                true);

            PathFigureCollection pathFigureCollection = new PathFigureCollection { pathFigure };

            PathGeometry pathGeometry = new PathGeometry(pathFigureCollection);
            return pathGeometry;
        }
    }
}
