using Silk.NET.Maths;
using System;

namespace MusicGameProject.Core
{
    public class Camera
    {
        public Vector2D<float> Position;
        public Vector2D<float> CurrentViewportSize;

        public Vector2D<float> LevelMinBounds;
        public Vector2D<float> LevelMaxBounds;

        private Vector2D<float> _shakeOffset;
        private float _shakeTimer;
        private float _shakeIntensity;
        private Random _rnd = new Random();

        public Camera(Vector2D<float> levelMin, Vector2D<float> levelMax)
        {
            LevelMinBounds = levelMin;
            LevelMaxBounds = levelMax;

            CurrentViewportSize = Config.Camera.BaseViewport;
        }

        public void Shake(float duration, float intensity)
        {
            _shakeTimer = duration;
            _shakeIntensity = intensity;
        }

        public void Update(double deltaTime, Vector2D<float> playerPos, Vector2D<float> playerVelocity, Vector2D<float>? attractor = null)
        {
            float dt = (float)deltaTime;

            if (_shakeTimer > 0)
            {
                _shakeTimer -= dt;
                _shakeOffset = new Vector2D<float>((float)(_rnd.NextDouble() * 2 - 1) * _shakeIntensity, (float)(_rnd.NextDouble() * 2 - 1) * _shakeIntensity);
            }
            else { _shakeOffset = Vector2D<float>.Zero; }

            float speedFactor = playerVelocity.Length / 1000f;
            Vector2D<float> targetViewport = Config.Camera.BaseViewport * (1f + speedFactor * Config.Camera.MaxZoomFactor);
            CurrentViewportSize.X = MathHelper.Lerp(CurrentViewportSize.X, targetViewport.X, dt * Config.Camera.ZoomSpeed);
            CurrentViewportSize.Y = MathHelper.Lerp(CurrentViewportSize.Y, targetViewport.Y, dt * Config.Camera.ZoomSpeed);

            // Логика Аттрактора
            Vector2D<float> focusPoint = playerPos;
            if (attractor.HasValue)
            {
                float marginX = (CurrentViewportSize.X / 2f) * Config.Camera.AttractorMarginRatio;
                float marginY = (CurrentViewportSize.Y / 2f) * Config.Camera.AttractorMarginRatio;

                bool inBoundsX = Math.Abs(playerPos.X - attractor.Value.X) < marginX;
                bool inBoundsY = Math.Abs(playerPos.Y - attractor.Value.Y) < marginY;

                if (inBoundsX && inBoundsY)
                {
                    focusPoint = attractor.Value;
                }
                else
                {
                    float targetX = Math.Clamp(attractor.Value.X, playerPos.X - marginX, playerPos.X + marginX);
                    float targetY = Math.Clamp(attractor.Value.Y, playerPos.Y - marginY, playerPos.Y + marginY);
                    focusPoint = new Vector2D<float>(targetX, targetY);
                }
            }

            Vector2D<float> targetPos = focusPoint - CurrentViewportSize / 2f;

            // Плавное следование
            Position.X = MathHelper.Lerp(Position.X, targetPos.X, dt * Config.Camera.FollowSpeed);
            Position.Y = MathHelper.Lerp(Position.Y, targetPos.Y, dt * (Config.Camera.FollowSpeed * Config.Camera.VerticalDamping));

            // Ограничение границами уровня
            float effectiveMinX = LevelMinBounds.X + Config.Camera.BorderOffset;
            float effectiveMaxX = LevelMaxBounds.X - Config.Camera.BorderOffset - CurrentViewportSize.X;

            float effectiveMinY = LevelMinBounds.Y + Config.Camera.BorderOffset;
            float effectiveMaxY = LevelMaxBounds.Y - Config.Camera.BorderOffset - CurrentViewportSize.Y;

            // Логика для оси X
            if (effectiveMaxX < effectiveMinX)
                Position.X = LevelMinBounds.X + (LevelMaxBounds.X - LevelMinBounds.X - CurrentViewportSize.X) / 2f;
            else
                Position.X = Math.Clamp(Position.X, effectiveMinX, effectiveMaxX);

            // Логика для оси Y
            if (effectiveMaxY < effectiveMinY)
                Position.Y = LevelMinBounds.Y + (LevelMaxBounds.Y - LevelMinBounds.Y - CurrentViewportSize.Y) / 2f;
            else
                Position.Y = Math.Clamp(Position.Y, effectiveMinY, effectiveMaxY);
        }

        public Matrix4X4<float> GetProjectionMatrix(float parallaxFactor = 1.0f)
        {
            float x = (Position.X * parallaxFactor) + _shakeOffset.X;
            float y = (Position.Y * parallaxFactor) + _shakeOffset.Y;
            return Matrix4X4.CreateOrthographicOffCenter(x, x + CurrentViewportSize.X, y + CurrentViewportSize.Y, y, -1f, 1f);
        }

        // Специальная матрица для интерфейса (Screen Space)
        public Matrix4X4<float> GetUIProjectionMatrix() =>
            Matrix4X4.CreateOrthographicOffCenter(0, Config.Camera.BaseViewport.X, Config.Camera.BaseViewport.Y, 0, -1f, 1f);
    }
}