﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Visuals.Media.Imaging;

namespace Avalonia.Controls
{   
    public class RenderOptions
    {
        /// <summary>
        /// Defines the <see cref="BitmapScalingMode"/> property.
        /// </summary>
        public static readonly StyledProperty<BitmapScalingMode> BitmapScalingModeProperty =
            AvaloniaProperty.RegisterAttached<RenderOptions, Control, BitmapScalingMode>(
                "BitmapScalingMode",
                inherits: true);

        /// <summary>
        /// Gets the value of the BitmapScalingMode attached property for a control.
        /// </summary>
        /// <param name="element">The control.</param>
        /// <returns>The control's left coordinate.</returns>
        public static BitmapScalingMode GetBitmapScalingMode(AvaloniaObject element)
        {
            return element.GetValue(BitmapScalingModeProperty);
        }

        /// <summary>
        /// Sets the value of the BitmapScalingMode attached property for a control.
        /// </summary>
        /// <param name="element">The control.</param>
        /// <param name="value">The left value.</param>
        public static void SetBitmapScalingMode(AvaloniaObject element, double value)
        {
            element.SetValue(BitmapScalingModeProperty, value);
        }
    }
}
