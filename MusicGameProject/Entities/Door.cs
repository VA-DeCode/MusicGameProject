using System.Collections.Generic;
using MusicGameProject.Core;
using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MusicGameProject.Entities
{
    public class Door : GameObject
    {
        public string TargetLevelId;
        public char TargetDoorId;
        public char DoorId;

        public Vector2D<float> SpawnPosition; 

        public Door(Vector2D<float> pos, Vector2D<float> size, char doorId, string targetLevel, char targetDoorId, Vector2D<float> spawnPosition)
            : base(pos, size, Layer.Middleground)
        {
            DoorId = doorId;
            TargetLevelId = targetLevel;
            TargetDoorId = targetDoorId;
            SpawnPosition = spawnPosition;
            
            Color = new Vector4D<float>(0f, 1f, 0f, 0.0f); 
        }

        public override void Update(double deltaTime, EntityManager entityManager) { }
        public override void Render(GL gl, ShapeRenderer renderer, Camera camera) { }
    }
}