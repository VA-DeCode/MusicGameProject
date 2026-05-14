using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using MusicGameProject.Core;
using System.Collections.Generic;
using System;

namespace MusicGameProject.Entities
{
    public class Player : GameObject
    {
        // --- СОБЫТИЯ И СОСТОЯНИЯ ---
        public Action<float, float> OnBonk;
        public Action<string, char> OnDoorEnter;

        public Vector2D<float> Velocity => _velocity;
        public bool IsFacingRight { get; private set; } = true;
        public bool IsRunning => _isRunning;
        public bool IsInputLocked = false;
        public bool CanInteract { get; private set; }

        private Vector2D<float> _velocity = Vector2D<float>.Zero;
        private bool _isRunning = false;
        private bool _isGrounded;
        private bool _wasJumpPressed;
        private float _coyoteTimeCounter;
        private float _jumpBufferCounter;

        private bool _isDashing = false;
        private bool _wasDashPressed;
        private float _dashTimeCounter;
        private float _dashCooldownCounter;

        private bool _isLedgeClimbing = false;
        private Vector2D<float> _ledgeClimbTarget;

        private InteractableObject _targetInteractable;
        private bool _wasActionPressed;

        public Player(Vector2D<float> position)
            : base(position, Config.Player.Size, Layer.Middleground)
        {
            Color = new Vector4D<float>(0f, 0.7f, 1f, 1f);
        }

        public void ResetPhysics()
        {
            _velocity = Vector2D<float>.Zero;
            _isGrounded = false;
            _isDashing = false;
            _dashTimeCounter = 0;
            _isLedgeClimbing = false;
            _coyoteTimeCounter = 0;
            _jumpBufferCounter = 0;
        }

        private float MoveTowards(float current, float target, float maxDelta)
        {
            if (Math.Abs(target - current) <= maxDelta) return target;
            return current + Math.Sign(target - current) * maxDelta;
        }

        public override void Update(double deltaTime, EntityManager entityManager)
        {
            float dt = (float)deltaTime;

            if (_dashCooldownCounter > 0) _dashCooldownCounter -= dt;

            // --- ВЗАИМОДЕЙСТВИЕ И ТРИГГЕРЫ ---
            CanInteract = false;
            _targetInteractable = null;
            Vector2D<float> playerCenter = Position + Size / 2f;

            foreach (var obj in entityManager.AllObjects)
            {
                // Двери
                if (obj is Door door && this.Intersects(door) && !IsInputLocked)
                {
                    OnDoorEnter?.Invoke(door.TargetLevelId, door.TargetDoorId);
                    return;
                }

                // Интерактивы
                if (obj is InteractableObject interactable)
                {
                    Vector2D<float> objCenter = obj.Position + obj.Size / 2f;
                    if (Vector2D.Distance(playerCenter, objCenter) < Config.Player.InteractRadius)
                    {
                        CanInteract = true;
                        _targetInteractable = interactable;
                    }
                }
            }

            // --- КАРАБКАНЬЕ ---
            if (_isLedgeClimbing)
            {
                Position.Y = MoveTowards(Position.Y, _ledgeClimbTarget.Y, Config.Player.LedgeClimbSpeed * dt);
                if (Math.Abs(Position.Y - _ledgeClimbTarget.Y) < Config.Player.LedgeTolerance)
                {
                    Position.Y = _ledgeClimbTarget.Y;
                    Position.X = MoveTowards(Position.X, _ledgeClimbTarget.X, Config.Player.LedgeClimbSpeed * dt);
                    if (Math.Abs(Position.X - _ledgeClimbTarget.X) < Config.Player.LedgeTolerance)
                    {
                        Position.X = _ledgeClimbTarget.X;
                        _isLedgeClimbing = false;
                        _isGrounded = true;
                    }
                }
                return;
            }

            // --- РЫВОК ---
            if (_isDashing)
            {
                _dashTimeCounter -= dt;
                if (_dashTimeCounter <= 0) _isDashing = false;
                else _velocity.Y = 0;
            }

            // --- ВЕРТИКАЛЬНАЯ ФИЗИКА ---
            if (!_isDashing)
            {
                if (_isGrounded) _coyoteTimeCounter = Config.Player.CoyoteTime;
                else _coyoteTimeCounter -= dt;
                _jumpBufferCounter -= dt;

                _velocity.Y += Config.Player.Gravity * dt;
                if (_velocity.Y > Config.Player.MaxFallSpeed) _velocity.Y = Config.Player.MaxFallSpeed;

                // ДВИГАЕМСЯ ПО Y
                Position.Y += _velocity.Y * dt;
                _isGrounded = false;

                // === НОВАЯ ПРОВЕРКА КОЛЛИЗИЙ ПО Y ===
                int minX = Math.Max(0, (int)(Left / Config.Level.TileSize));
                int maxX = Math.Min(entityManager.CollisionGrid.GetLength(0) - 1, (int)(Right / Config.Level.TileSize));
                int minY = Math.Max(0, (int)(Top / Config.Level.TileSize));
                int maxY = Math.Min(entityManager.CollisionGrid.GetLength(1) - 1, (int)(Bottom / Config.Level.TileSize));

                for (int x = minX; x <= maxX; x++)
                {
                    for (int y = minY; y <= Math.Min(maxY + 1, entityManager.CollisionGrid.GetLength(1) - 1); y++)
                    {
                        var obj = entityManager.CollisionGrid[x, y];
                        if (obj != null && this.Intersects(obj))
                        {
                            if (_velocity.Y > 0)
                            {
                                Position.Y = obj.Top - Size.Y;
                                if (_velocity.Y > Config.Player.HeavyFallThreshold)
                                    OnBonk?.Invoke(Config.Player.FallShakeDuration, Config.Player.FallShakeIntensity);
                                _velocity.Y = 0;
                                _isGrounded = true;
                            }
                            else if (_velocity.Y < 0)
                            {
                                Position.Y = obj.Bottom;
                                _velocity.Y = 0;
                                _coyoteTimeCounter = 0;
                            }
                        }
                    }
                }
                // ====================================

                if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
                {
                    _velocity.Y = Config.Player.MaxJumpImpulse;
                    _jumpBufferCounter = 0f;
                    _coyoteTimeCounter = 0f;
                }
            }

            // --- ГОРИЗОНТАЛЬНАЯ ФИЗИКА ---
            Position.X += _velocity.X * dt;

            // === НОВАЯ ПРОВЕРКА КОЛЛИЗИЙ ПО X ===
            int minX_X = Math.Max(0, (int)(Left / Config.Level.TileSize));
            int maxX_X = Math.Min(entityManager.CollisionGrid.GetLength(0) - 1, (int)(Right / Config.Level.TileSize));
            int minY_X = Math.Max(0, (int)(Top / Config.Level.TileSize));
            int maxY_X = Math.Min(entityManager.CollisionGrid.GetLength(1) - 1, (int)(Bottom / Config.Level.TileSize));

            for (int x = minX_X; x <= maxX_X; x++)
            {
                for (int y = minY_X; y <= Math.Min(maxY_X + 1, entityManager.CollisionGrid.GetLength(1) - 1); y++)
                {
                    var obj = entityManager.CollisionGrid[x, y];
                    if (obj != null && this.Intersects(obj))
                    {
                        // Проверка на зацеп за край (Ledge)
                        if (Bottom > obj.Top && (Bottom - obj.Top) <= (Size.Y * Config.Player.LedgeGrabRatio) && _velocity.Y >= 0)
                        {
                            _isLedgeClimbing = true;
                            float targetY = obj.Top - Size.Y;
                            float targetX = (_velocity.X > 0) ? obj.Left - Size.X + Config.Player.LedgePullXOffset : obj.Right - Config.Player.LedgePullXOffset;
                            _ledgeClimbTarget = new Vector2D<float>(targetX, targetY);
                            _velocity = Vector2D<float>.Zero;
                            _isRunning = false;
                            if (_isDashing) { _isDashing = false; _dashTimeCounter = 0; }
                            continue;
                        }

                        bool wasRunningFast = _isRunning || _isDashing || Math.Abs(_velocity.X) > Config.Player.BaseSpeed + Config.Player.StopVelocityThreshold;

                        if (_velocity.X > 0)
                        {
                            Position.X = obj.Left - Size.X;
                            if (wasRunningFast)
                            {
                                _velocity.X = -Config.Player.WallBonkForce;
                                _isRunning = false;
                                OnBonk?.Invoke(Config.Player.WallShakeDuration, Config.Player.WallShakeIntensity);
                            }
                            else _velocity.X = 0;
                        }
                        else if (_velocity.X < 0)
                        {
                            Position.X = obj.Right;
                            if (wasRunningFast)
                            {
                                _velocity.X = Config.Player.WallBonkForce;
                                _isRunning = false;
                                OnBonk?.Invoke(Config.Player.WallShakeDuration, Config.Player.WallShakeIntensity);
                            }
                            else _velocity.X = 0;
                        }
                        if (_isDashing) { _isDashing = false; _dashTimeCounter = 0; }
                    }
                }
            }
            // ====================================
        }

        public void HandleInput(IKeyboard keyboard, IMouse mouse, float dt)
        {
            if (IsInputLocked)
            {
                _velocity.X = MoveTowards(_velocity.X, 0, Config.Player.DecelRate * dt);
                return;
            }

            // Взаимодействие (E)
            bool isActionPressed = keyboard.IsKeyPressed(Key.E);
            if (isActionPressed && !_wasActionPressed && CanInteract && _targetInteractable != null)
                _targetInteractable.Interact();
            _wasActionPressed = isActionPressed;

            if (_isDashing || _isLedgeClimbing) return;

            bool moveLeft = keyboard.IsKeyPressed(Key.A);
            bool moveRight = keyboard.IsKeyPressed(Key.D);

            if (!moveLeft && !moveRight && Math.Abs(_velocity.X) < Config.Player.StopVelocityThreshold)
                _isRunning = false;

            float targetSpeed = _isRunning ? Config.Player.RunSpeed : Config.Player.BaseSpeed;
            float targetVelocityX = 0;

            if (moveLeft) { targetVelocityX = -targetSpeed; IsFacingRight = false; }
            if (moveRight) { targetVelocityX = targetSpeed; IsFacingRight = true; }

            if (targetVelocityX != 0)
            {
                float accel = (Math.Sign(targetVelocityX) != Math.Sign(_velocity.X) && _velocity.X != 0)
                    ? Config.Player.TurnFriction : Config.Player.AccelRate;
                _velocity.X = MoveTowards(_velocity.X, targetVelocityX, accel * dt);
            }
            else
                _velocity.X = MoveTowards(_velocity.X, 0, Config.Player.DecelRate * dt);

            // Рывок (ПКМ)
            bool isDashPressed = mouse.IsButtonPressed(MouseButton.Right);
            if (isDashPressed && !_wasDashPressed && _dashCooldownCounter <= 0)
            {
                _isDashing = true;
                _dashTimeCounter = Config.Player.DashDuration;
                _dashCooldownCounter = Config.Player.DashCooldown;
                _isRunning = true;
                _velocity.X = IsFacingRight ? Config.Player.DashSpeed : -Config.Player.DashSpeed;
                _velocity.Y = 0;
                _wasDashPressed = isDashPressed;
                return;
            }
            _wasDashPressed = isDashPressed;

            // Прыжок (Space)
            bool isJumpPressed = keyboard.IsKeyPressed(Key.Space);
            if (isJumpPressed && !_wasJumpPressed) _jumpBufferCounter = Config.Player.JumpBufferTime;
            if (!isJumpPressed && _wasJumpPressed && _velocity.Y < 0)
            {
                _velocity.Y *= Config.Player.JumpCutMultiplier;
                _coyoteTimeCounter = 0f;
            }
            _wasJumpPressed = isJumpPressed;
        }

        public override void Render(GL gl, ShapeRenderer renderer, Camera camera) { }
    }
}