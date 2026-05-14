using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System.Collections.Generic;

namespace MusicGameProject.Core
{
    public enum Layer { Background, Middleground, Foreground, UI }

    public abstract class GameObject
    {
        public Vector2D<float> Position;
        public Vector2D<float> Size;
        public Vector4D<float> Color = new(1, 1, 1, 1);
        public Layer ObjectLayer;

        public float Left => Position.X;
        public float Right => Position.X + Size.X;
        public float Top => Position.Y;
        public float Bottom => Position.Y + Size.Y;

        public GameObject(Vector2D<float> position, Vector2D<float> size, Layer layer = Layer.Middleground)
        {
            Position = position;
            Size = size;
            ObjectLayer = layer;
        }

        public abstract void Update(double deltaTime, EntityManager entityManager);
        public abstract void Render(GL gl, ShapeRenderer renderer, Camera camera);

        public bool Intersects(GameObject other) =>
            Left < other.Right && Right > other.Left && Top < other.Bottom && Bottom > other.Top;
    }
}