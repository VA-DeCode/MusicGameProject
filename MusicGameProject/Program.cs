using MusicGameProject.Core;
using MusicGameProject.Entities;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace MusicGameProject
{
    class Program
    {
        private static IWindow window;
        private static GL Gl;

        private static EntityManager entityManager;
        private static ShapeRenderer renderer;
        private static Camera camera;
        private static Player player;
        private static SceneManager sceneManager;

        private static IInputContext input;
        private static IKeyboard keyboard;
        private static IMouse mouse;

        static void Main(string[] args)
        {
            var options = WindowOptions.Default;

            options.Size = Config.Window.DefaultSize;
            options.WindowState = WindowState.Fullscreen;
            options.Title = Config.Window.Title;
            window = Window.Create(options);

            window.Load += OnLoad;
            window.Update += OnUpdate;
            window.Render += OnRender;
            window.Closing += OnClose;

            window.Run();
        }

        private static void OnLoad()
        {
            Gl = GL.GetApi(window);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            input = window.CreateInput();
            keyboard = input.Keyboards[0];
            mouse = input.Mice[0];

            keyboard.KeyDown += (kb, key, arg3) => { if (key == Key.Escape) window.Close(); };

            entityManager = new EntityManager();
            renderer = new ShapeRenderer(Gl);

            camera = new Camera(new Vector2D<float>(0, 0), new Vector2D<float>(4000, 2000));

            player = new Player(new Vector2D<float>(200, 200));
            player.OnBonk += camera.Shake;

            sceneManager = new SceneManager(entityManager, camera, player);
            player.OnDoorEnter += sceneManager.StartTransition;

            sceneManager.StartTransition("Level_1", '\0');
        }

        private static double _physicsAccumulator = 0.0;
        private const double FixedDt = 1.0 / 60.0;

        private static void OnUpdate(double deltaTime)
        {
            if (deltaTime > 0.2) deltaTime = 0.2;

            _physicsAccumulator += deltaTime;

            while (_physicsAccumulator >= FixedDt)
            {
                player.HandleInput(keyboard, mouse, (float)FixedDt);
                sceneManager.Update(FixedDt);
                entityManager.UpdateAll(FixedDt);

                _physicsAccumulator -= FixedDt;
            }

            camera.Update(deltaTime, player.Position, player.Velocity);
        }

        private static void OnRender(double deltaTime)
        {
            Gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            entityManager.RenderAll(Gl, renderer, camera);

            // 2. --- ОТРИСОВКА ЗАТЕМНЕНИЯ ПОВЕРХ ЭКРАНА ---
            if (sceneManager.FadeAlpha > 0f)
            {
                renderer.BeginLayer(camera, Layer.UI);

                renderer.Submit(
                    Vector2D<float>.Zero,
                    new Vector2D<float>(Config.Window.DefaultSize.X, Config.Window.DefaultSize.Y),
                    new Vector4D<float>(0f, 0f, 0f, sceneManager.FadeAlpha)
                );

                renderer.EndLayer();
            }
        }

        private static void OnClose()
        {
            input.Dispose();
            renderer.Dispose();
        }
    }
}