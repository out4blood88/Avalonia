using System;

namespace Avalonia.Input.Raw
{
    public class RawSizeEventArgs : EventArgs
    {
        internal RawSizeEventArgs(Size size)
        {
            Size = size;
        }

        internal RawSizeEventArgs(double width, double height)
        {
            Size = new Size(width, height);
        }

        public Size Size { get; private set; }
    }
}
