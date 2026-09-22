using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Vex.Debugging;

namespace Vex.Graphics
{
    public class Camera2D
    {
        private Vector2D<float> _position = new Vector2D<float>(0f, 0f);
        private float _rotation = 0.0f;
        private float _zoom = 1.0f;
        private float _aspectRatio = 16.0f / 9.0f;

        private bool _isDirty = true;
        private Matrix4x4 _viewMatrix = Matrix4x4.Identity;
        private Matrix4x4 _projection = Matrix4x4.Identity;
        private Matrix4x4 _viewProjection = Matrix4x4.Identity;

        public Camera2D(float aspectRatio = 16.0f / 9.0f)
        {
            _aspectRatio = aspectRatio;
            RecalculateMatrices();
        }

        public Matrix4x4 ViewMatrix
        {
            get
            {
                if (_isDirty) RecalculateMatrices();
                return _viewMatrix;
            }
        }

        public Matrix4x4 Projection
        {
            get
            {
                if (_isDirty) RecalculateMatrices();
                return _projection;
            }
        }

        public Matrix4x4 ViewProjection
        {
            get
            {
                if (_isDirty) RecalculateMatrices();
                return _viewProjection;
            }
        }

        public Vector2D<float> Position
        {
            get => _position;
            set
            {
                _position = value;
                _isDirty = true;
            }
        }
        public float Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _isDirty = true;
            }
        }
        public float Zoom
        {
            get => _zoom;
            set
            {
                _zoom = MathF.Max(0.001f, value);
                _isDirty = true;
            }
        }
        public float AspectRatio
        {
            get => _aspectRatio;
            set
            {
                _aspectRatio = value;
                _isDirty = true;
            }
        }
        public void SetViewportSize(int width, int height)
        {
            if (height == 0) throw new VexInvalidArgumentException("Height cannot be zero.");
            _aspectRatio = (float)width / height;
            _isDirty = true;
        }

        public void RecalculateMatrices()
        {
            float halfHeight = 1.0f / _zoom;
            float halfWidth = halfHeight * _aspectRatio;

            _projection = Matrix4x4.CreateOrthographicOffCenter(
                 -halfWidth, halfWidth,
                 -halfHeight, halfHeight,
                 -1.0f, 1.0f
             );

            Matrix4x4 transform = Matrix4x4.CreateRotationZ(Rotation) * Matrix4x4.CreateTranslation(new Vector3(Position.X, Position.Y, 0.0f));
            if(!Matrix4x4.Invert(transform, out _viewMatrix))
                _viewMatrix = Matrix4x4.Identity;

            _viewProjection = _viewMatrix * _projection;
            _isDirty = false;
        }

        //public Matrix4x4 GetViewProjectionMatrix()
        //{
        //    return GetViewMatrix() * GetProjectionMatrix();
        //}

        //public Matrix4x4 GetProjectionMatrix()
        //{
        //    return Matrix4x4.CreateOrthographicOffCenter(
        //        -HalfWidth, HalfWidth,
        //        -HalfHeight, HalfHeight,
        //        -1.0f, 1.0f // -1.0f and 1.0f for near and far planes
        //    );
        //}

        //public Matrix4x4 GetViewMatrix()
        //{
        //    Matrix4x4 transform = Matrix4x4.CreateRotationZ(Rotation) * Matrix4x4.CreateTranslation(new Vector3(Position.X, Position.Y, 0.0f));
        //    Matrix4x4.Invert(transform, out Matrix4x4 view);
        //    return view;
        //}
    }
}
