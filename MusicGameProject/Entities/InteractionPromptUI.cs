using System.Collections.Generic;
using MusicGameProject.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MusicGameProject.Entities
{
    public class InteractionPromptUI : GameObject
    {
        private Player _player;

        public InteractionPromptUI(Player player)
            : base(Config.UI.PromptPos, Config.UI.PromptSize, Layer.UI)
        {
            _player = player;
            Color = new Vector4D<float>(1f, 1f, 0f, 0f);
        }

        public override void Update(double dt, EntityManager entityManager)
        {
            float deltaTime = (float)dt;
            float targetAlpha = _player.CanInteract ? 1f : 0f;

            Color.W = MathHelper.Lerp(Color.W, targetAlpha, deltaTime * Config.UI.PromptFadeSpeed);

            if (Color.W < 0.01f) Color.W = 0f;
        }

        public override void Render(GL gl, ShapeRenderer r, Camera c) { }
    }
}