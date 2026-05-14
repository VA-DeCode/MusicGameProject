using System.Collections.Generic;
using MusicGameProject.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MusicGameProject.Entities
{
    public class StaticPlatform : GameObject
    {
        public StaticPlatform(Vector2D<float> pos, Vector2D<float> size, Vector4D<float> color, Layer layer = Layer.Middleground)
            : base(pos, size, layer)
        {
            Color = color;
        }

        public override void Update(double deltaTime, EntityManager entityManager) { /* Стены не двигаются */ }

        public override void Render(GL gl, ShapeRenderer renderer, Camera camera) { /* Пока пустой */ }
    }
}