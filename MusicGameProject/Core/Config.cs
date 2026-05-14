using Silk.NET.Maths;

namespace MusicGameProject.Core
{
    public static class Config
    {
        public static class Window
        {
            // Разрешение окна по умолчанию при запуске игры.
            public static readonly Vector2D<int> DefaultSize = new(1920, 1080);

            // Название окна приложения, которое отображается в системе.
            public const string Title = "Coda: Celestial Harp";
        }

        public static class Player
        {
            // --- Визуал и Коллизии ---

            // Физические размеры хитбокса игрока (Ширина x Высота).
            public static readonly Vector2D<float> Size = new(40f, 60f);

            // --- Базовое движение ---

            // Стандартная скорость перемещения (при обычной ходьбе).
            public const float BaseSpeed = 150f;

            // Максимальная скорость перемещения (например, при беге или после рывка).
            public const float RunSpeed = 250f;

            // Ускорение: как быстро игрок набирает скорость с нуля. Чем больше, тем резче старт.
            public const float AccelRate = 1000f;

            // Торможение: как быстро игрок останавливается, когда отпущены кнопки движения.
            public const float DecelRate = 2500f;

            // Трение при развороте: как быстро гасится инерция, если игрок бежит в одну сторону и резко нажимает в другую.
            public const float TurnFriction = 4000f;

            // Порог скорости, ниже которого физика принудительно обнуляет скорость игрока, чтобы избежать микро-скольжения.
            public const float StopVelocityThreshold = 10f;

            // --- Физика прыжка ---

            // Сила гравитации, тянущая игрока вниз каждый кадр.
            public const float Gravity = 1800f;

            // Максимальная скорость падения (терминальная скорость), чтобы игрок не пролетал сквозь текстуры насквозь.
            public const float MaxFallSpeed = 800f;

            // Начальный импульс прыжка. Значение отрицательное, так как в 2D ось Y направлена вниз.
            public const float MaxJumpImpulse = -750f;

            // Срез прыжка: если игрок отпускает "Пробел" раньше времени, скорость прыжка умножается на это число (позволяет делать короткие прыжки).
            public const float JumpCutMultiplier = 0.43f;

            // Буфер прыжка: время (в секундах), в течение которого игра "запоминает" нажатие прыжка перед приземлением, чтобы прыгнуть сразу после касания земли.
            public const float JumpBufferTime = 0.15f;

            // Время койота: окно (в секундах), позволяющее игроку совершить прыжок уже после того, как он соскользнул с края платформы (прощает ошибки тайминга).
            public const float CoyoteTime = 0.15f;

            // --- Рывок (Dash) ---

            // Скорость перемещения во время рывка.
            public const float DashSpeed = 500f;

            // Длительность выполнения рывка (в секундах).
            public const float DashDuration = 0.1f;

            // Перезарядка: сколько времени (в секундах) нужно подождать перед следующим рывком.
            public const float DashCooldown = 0.5f;

            // --- Карабканье (Ledge Climb) ---

            // Скорость анимации подтягивания на уступ.
            public const float LedgeClimbSpeed = 500f;

            // Процент роста игрока: какая часть тела должна быть ниже края уступа, чтобы сработал триггер захвата.
            public const float LedgeGrabRatio = 0.6f;

            // Горизонтальный отступ от края платформы при подтягивании, чтобы хитбокс не застрял в стене.
            public const float LedgePullXOffset = 4f;

            // Погрешность (в пикселях) для завершения интерполяции подтягивания и возврата управления игроку.
            public const float LedgeTolerance = 0.5f;

            // --- Взаимодействие и эффекты ---

            // Радиус вокруг центра игрока, в котором он может взаимодействовать с терминалами и кнопками (по кнопке 'E').
            public const float InteractRadius = 80f;

            // Сила отскока по оси X, когда игрок на большой скорости (например, в рывке) врезается в стену.
            public const float WallBonkForce = 350f;

            // Порог скорости падения (чуть меньше MaxFallSpeed), при достижении которого приземление считается тяжелым и вызывает эффекты.
            public const float HeavyFallThreshold = 799f;

            // Длительность тряски камеры после долгого (тяжелого) падения.
            public const float FallShakeDuration = 0.15f;

            // Амплитуда (интенсивность) тряски камеры при падении.
            public const float FallShakeIntensity = 1f;

            // Длительность тряски камеры при жестком ударе головой или телом о стену.
            public const float WallShakeDuration = 0.2f;

            // Амплитуда тряски камеры при ударе о стену.
            public const float WallShakeIntensity = 1f;
        }

        public static class Camera
        {
            // Базовое разрешение видимой области игровой камеры (без учета динамического зума).
            public static readonly Vector2D<float> BaseViewport = new(1280f, 720f);

            // Скорость плавного следования (Lerp) камеры за игроком. Чем выше, тем жестче привязана камера.
            public const float FollowSpeed = 4f;

            // Множитель замедления камеры по оси Y (сохраняет стабильность экрана при частых прыжках).
            public const float VerticalDamping = 0.5f;

            // Скорость плавного отдаления/приближения камеры (динамический зум).
            public const float ZoomSpeed = 2f;

            // Максимальный процент (15%), на который отдаляется камера при беге на высоких скоростях.
            public const float MaxZoomFactor = 0.02f;

            // Размер "свободной зоны" камеры (в % от размера экрана), внутри которой камера реагирует на объекты притяжения (Attractors).
            public const float AttractorMarginRatio = 0.5f;

            // Отступ от границ уровня, предотвращающий показ пустоты за пределами карты (камера упирается в этот лимит).
            public const float BorderOffset = 40f;
        }

        public static class UI
        {
            // Скорость исчезновения и появления UI элементов (например, подсказки взаимодействия 'E').
            public const float PromptFadeSpeed = 5f;

            // Размер UI элемента (подсказки взаимодействия).
            public static readonly Vector2D<float> PromptSize = new(40f, 40f);

            // Фиксированная позиция UI подсказки на экране пользователя (Screen Space).
            public static readonly Vector2D<float> PromptPos = new(780f, 50f);

            // Скорость черного затемнения и осветления экрана (Fade) при смене комнат/уровней.
            public const float SceneFadeSpeed = 2.5f;
        }

        public static class Level
        {
            // Универсальный размер одного квадратного блока (тайла) геометрии уровня в пикселях.
            public const float TileSize = 40f;
        }

        public static class Graphics
        {
            // Множитель скорости движения фоновых объектов. 0.5 означает, что фон движется в два раза медленнее камеры (создает глубину).
            public const float BackgroundParallax = 0.5f;

            // Множитель движения основного слоя с платформами и игроком (всегда движется синхронно с камерой = 1.0).
            public const float DefaultParallax = 1.0f;
        }

        public static class Paths
        {
            // Локальный путь к файлу вершинного шейдера в папке проекта.
            public const string VertexShader = "Assets/Shaders/shader.vert";

            // Локальный путь к файлу фрагментного шейдера в папке проекта.
            public const string FragmentShader = "Assets/Shaders/shader.frag";
        }
    }
}