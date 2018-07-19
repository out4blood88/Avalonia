﻿// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Rendering.SceneGraph
{
    using Avalonia.Visuals.Media.Imaging;

    /// <summary>
    /// A node in the scene graph which represents an image draw.
    /// </summary>
    internal class ImageNode : DrawOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImageNode"/> class.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <param name="source">The image to draw.</param>
        /// <param name="opacity">The draw opacity.</param>
        /// <param name="sourceRect">The source rect.</param>
        /// <param name="destRect">The destination rect.</param>
        /// <param name="bitmapScalingMode">The bitmap scaling mode.</param>
        public ImageNode(Matrix transform, IRef<IBitmapImpl> source, double opacity, Rect sourceRect, Rect destRect, BitmapScalingMode bitmapScalingMode)
            : base(destRect, transform, null)
        {
            Transform = transform;
            Source = source.Clone();
            Opacity = opacity;
            SourceRect = sourceRect;
            DestRect = destRect;
            BitmapScalingMode = bitmapScalingMode;
        }        

        /// <summary>
        /// Gets the transform with which the node will be drawn.
        /// </summary>
        public Matrix Transform { get; }

        /// <summary>
        /// Gets the image to draw.
        /// </summary>
        public IRef<IBitmapImpl> Source { get; }

        /// <summary>
        /// Gets the draw opacity.
        /// </summary>
        public double Opacity { get; }

        /// <summary>
        /// Gets the source rect.
        /// </summary>
        public Rect SourceRect { get; }

        /// <summary>
        /// Gets the destination rect.
        /// </summary>
        public Rect DestRect { get; }

        /// <summary>
        /// Gets the bitmap scaling mode.
        /// </summary>
        /// <value>
        /// The scaling mode.
        /// </value>
        public BitmapScalingMode BitmapScalingMode { get; }

        /// <summary>
        /// Determines if this draw operation equals another.
        /// </summary>
        /// <param name="transform">The transform of the other draw operation.</param>
        /// <param name="source">The image of the other draw operation.</param>
        /// <param name="opacity">The opacity of the other draw operation.</param>
        /// <param name="sourceRect">The source rect of the other draw operation.</param>
        /// <param name="destRect">The dest rect of the other draw operation.</param>
        /// <param name="bitmapScalingMode"></param>
        /// <returns>True if the draw operations are the same, otherwise false.</returns>
        /// <remarks>
        /// The properties of the other draw operation are passed in as arguments to prevent
        /// allocation of a not-yet-constructed draw operation object.
        /// </remarks>
        public bool Equals(Matrix transform, IRef<IBitmapImpl> source, double opacity, Rect sourceRect, Rect destRect, BitmapScalingMode bitmapScalingMode)
        {
            return transform == Transform &&
                Equals(source.Item, Source.Item) &&
                opacity == Opacity &&
                sourceRect == SourceRect &&
                destRect == DestRect &&
                bitmapScalingMode == BitmapScalingMode;
        }

        /// <inheritdoc/>
        public override void Render(IDrawingContextImpl context)
        {
            // TODO: Probably need to introduce some kind of locking mechanism in the case of
            // WriteableBitmap.
            context.Transform = Transform;
            context.DrawImage(Source, Opacity, SourceRect, DestRect, BitmapScalingMode);
        }

        /// <inheritdoc/>
        public override bool HitTest(Point p) => Bounds.Contains(p);

        public override void Dispose()
        {
            Source?.Dispose();
        }
    }
}
