using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ConstraintThingyGUI
{
    enum AreaType { Rectangle, Circle }

    class DragableAreaTypeListItem : DragableConstraintListItem
    {
        [Bindable(true)]
        public AreaType AreaType { get; set; }

        public const String AreaTypeDataFormat = "AreaType";

        protected override string DataFormat
        {
            get { return AreaTypeDataFormat; }
        }

        protected override object Data
        {
            get { return AreaType; }
        }
    }

    abstract class DragableConstraintListItem : ListBoxItem
    {
        public DragableConstraintListItem()
        {
            PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
            PreviewMouseMove += OnPreviewMouseMove;
        }

        private Point _startLocation;

        protected abstract String DataFormat { get; }

        protected abstract object Data { get; }

        private void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = _startLocation - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DataObject dragData = new DataObject(DataFormat, Data);
                DragDrop.DoDragDrop(this, dragData, DragDropEffects.Move);
            }
        }

        private void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startLocation = e.GetPosition(null);
        }
    }
}