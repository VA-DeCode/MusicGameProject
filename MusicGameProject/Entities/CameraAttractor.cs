using System.Collections.Generic;
using MusicGameProject.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MusicGameProject.Entities
{
    public class CameraAttractor : GameObject
    {
        public CameraAttractor(Vector2D<float> pos)
            : base(pos, new Vector2D<float>(40, 40), Layer.Background)
        {
            Color = new Vector4D<float>(1f, 0f, 0f, 0.5f);
        }

        public override void Update(double dt, EntityManager entityManager) { }
        public override void Render(GL gl, ShapeRenderer r, Camera c) { }
    }
}