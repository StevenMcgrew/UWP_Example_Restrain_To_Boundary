using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace UWP_Example_RestrainToBoundary
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void myGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            MoveAndRestrain(myGrid, myBorder, gridTransform, e);
        }

        private void myBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double rightThick = myBorder.BorderThickness.Right;
            double bottomThick = myBorder.BorderThickness.Bottom;

            JailBoundaryChanged(myGrid, myBorder, gridTransform, bottomThick, rightThick);
        }

        private void MoveAndRestrain(FrameworkElement prisoner, FrameworkElement jail, CompositeTransform move, ManipulationDeltaRoutedEventArgs eventData)
        {
            // Get the top left point of the prisoner in relationship to the jail
            GeneralTransform gt = prisoner.TransformToVisual(jail);
            Point prisonerTopLeftPoint = gt.TransformPoint(new Point(0, 0));

            // Set these variables to represent the edges of the prisoner
            double left = prisonerTopLeftPoint.X;
            double top = prisonerTopLeftPoint.Y;
            double right = left + prisoner.ActualWidth;
            double bottom = top + prisoner.ActualHeight;

            // Combine those edges with the movement value (When these are used in the next step, it keeps the prisoner from getting stuck at the jail boundary)
            double leftAdjust = left + eventData.Delta.Translation.X;
            double topAdjust = top + eventData.Delta.Translation.Y;
            double rightAdjust = right + eventData.Delta.Translation.X;
            double bottomAdjust = bottom + eventData.Delta.Translation.Y;

            // Allow prisoner movement if within jail boundary (Use two separate "if" statements here, so the movement isn't sticky at the boundary)
            if ((leftAdjust >= 0) && (rightAdjust <= jail.ActualWidth))
            {
                move.TranslateX += eventData.Delta.Translation.X;
            }

            if ((topAdjust >= 0) && (bottomAdjust <= jail.ActualHeight))
            {
                move.TranslateY += eventData.Delta.Translation.Y;
            }
        }

        private void JailBoundaryChanged(FrameworkElement prisoner, FrameworkElement jail, CompositeTransform move, double borderThicknessRight, double borderThicknessBottom)
        {
            // Get the top left point of prisoner in relationship to jail
            GeneralTransform gt = prisoner.TransformToVisual(jail);
            Point prisonerTopLeftPoint = gt.TransformPoint(new Point(0, 0));

            // Set these variables to represent the edges of prisoner
            double left = prisonerTopLeftPoint.X;
            double top = prisonerTopLeftPoint.Y;
            double right = left + prisoner.ActualWidth;
            double bottom = top + prisoner.ActualHeight;

            // Reposition prisoner to keep in jail (jail's BorderThickness is subtracted for right and bottom because it affects postioning)
            if (left < 0)
            {
                move.TranslateX = 0;
            }
            else if ((right > jail.ActualWidth) && (left > 0))
            {
                double updatedLeft = jail.ActualWidth - prisoner.ActualWidth;
                move.TranslateX = updatedLeft - borderThicknessRight;
            }

            if (top < 0)
            {
                move.TranslateY = 0;
            }
            else if ((bottom > jail.ActualHeight) && (top > 0))
            {
                double updatedTop = jail.ActualHeight - prisoner.ActualHeight;
                move.TranslateY = updatedTop - borderThicknessBottom;
            }
        }
    }
}
