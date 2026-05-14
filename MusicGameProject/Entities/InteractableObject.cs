using System;
using System.Collections.Generic;
using MusicGameProject.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MusicGameProject.Entities
{
    public class InteractableObject : GameObject
    {
        public Action OnInteract;

        public InteractableObject(Vector2D<float> pos)
            : base(pos, new Vector2D<float>(40, 60), Layer.Middleground)
        {
            Color = new Vector4D<float>(0.8f, 0.2f, 0.8f, 1f);
        }

        public override void Update(double dt, EntityManager entityManager) { }
        public override void Render(GL gl, ShapeRenderer renderer, Camera camera) { }

        public void Interact()
        {
            OnInteract?.Invoke();
        }
    }
}