using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace MiniPaint
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        bool isDrawing;
        Point startingPoint;
        SolidColorBrush brush;
        Line previousLine;
        Stack<UIElement> stackDoUndo;


        public MainPage()
        {
            this.InitializeComponent();
            isDrawing = false;
            brush = new SolidColorBrush(Windows.UI.Colors.Black);
            stackDoUndo = new Stack<UIElement>();
        }

        private void drawingBox_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (isDrawing == true)
            {
                Point pktAkt = e.GetCurrentPoint(drawingBox).Position;
                var line = new Line
                {
                    X1 = pktAkt.X,
                    Y1 = pktAkt.Y,
                    X2 = startingPoint.X,
                    Y2 = startingPoint.Y,
                    StrokeThickness = thickLine.Value,
                    Stroke = brush,
                    StrokeStartLineCap = PenLineCap.Round,
                    StrokeEndLineCap = PenLineCap.Round,
                };
                drawingBox.Children.Add(line);
                if (rbAny.IsChecked == true)
                {
                    startingPoint = pktAkt;
                    stackDoUndo.Push(line);
                }
                if (rbStraight.IsChecked == true)
                {
                    if (line != null)
                    {
                        drawingBox.Children.Remove(previousLine);
                        previousLine = line;
                    }
                }


            }
        }

        private void drawingBox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            isDrawing = true;
            startingPoint = e.GetCurrentPoint(drawingBox).Position;
        }

        private void drawingBox_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            isDrawing = false;
            if (rbStraight.IsChecked == true && previousLine != null)
            {
                stackDoUndo.Push(previousLine);
                previousLine = null;
            }
        }

        private void brushColor(object sender, PointerRoutedEventArgs e)
        {
            var rectangle = e.OriginalSource as Rectangle;
            brush = rectangle.Fill as SolidColorBrush;
        }

        private void drawingBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            drawingBox.Clip = new RectangleGeometry()
            {
                Rect = new Rect(0, 0, drawingBox.ActualWidth, drawingBox.ActualHeight)
            };
        }

        private void btUndo_Click(object sender, RoutedEventArgs e)
        {
            if (stackDoUndo.Count > 0)
            {
                var undo = stackDoUndo.Pop();
                drawingBox.Children.Remove(undo);
            }
        }

        private void prColor_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            brush = new SolidColorBrush(prColor.Color);
            btColor.Flyout.Hide();
        }
    }
}
