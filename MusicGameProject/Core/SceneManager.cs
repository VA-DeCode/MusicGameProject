using System;
using System.IO;
using System.Text.Json;
using MusicGameProject.Entities;
using Silk.NET.Maths;

namespace MusicGameProject.Core
{
    public class SceneManager
    {
        private EntityManager _entityManager;
        private Camera _camera;
        private Player _player;

        public float FadeAlpha { get; private set; } = 0f;

        private bool _isFadingOut = false;
        private bool _isFadingIn = false;
        private string _pendingLevelId;
        private char _pendingDoorId;

        public SceneManager(EntityManager em, Camera cam, Player p)
        {
            _entityManager = em;
            _camera = cam;
            _player = p;
        }

        public void StartTransition(string targetLevel, char targetDoor)
        {
            if (_isFadingOut || _isFadingIn) return;
            _pendingLevelId = targetLevel;
            _pendingDoorId = targetDoor;
            _isFadingOut = true;
            _player.IsInputLocked = true;
        }

        public void Update(double dt)
        {
            float deltaTime = (float)dt;
            if (_isFadingOut)
            {
                FadeAlpha += Config.UI.SceneFadeSpeed * deltaTime;
                if (FadeAlpha >= 1f)
                {
                    FadeAlpha = 1f;
                    _isFadingOut = false;
                    LoadLevelFromFile(_pendingLevelId, _pendingDoorId);
                    _isFadingIn = true;
                }
            }
            else if (_isFadingIn)
            {
                FadeAlpha -= Config.UI.SceneFadeSpeed * deltaTime;
                if (FadeAlpha <= 0f) { FadeAlpha = 0f; _isFadingIn = false; _player.IsInputLocked = false; }
            }
        }

        private void LoadLevelFromFile(string levelId, char targetDoorId)
        {
            string path = $"Assets/Levels/{levelId}.json";
            if (!File.Exists(path)) throw new FileNotFoundException($"Уровень {path} не найден!");

            string jsonString = File.ReadAllText(path);
            var levelData = JsonSerializer.Deserialize<LevelData>(jsonString);

            _entityManager.Clear();
            _entityManager.AddObject(_player);
            _player.ResetPhysics();

            float levelWidth = levelData.Map[0].Length * LevelBuilder.TileSize;
            float levelHeight = levelData.Map.Length * LevelBuilder.TileSize;
            _camera.LevelMaxBounds = new Vector2D<float>(levelWidth, levelHeight);

            Door spawnDoor = LevelBuilder.BuildFromData(levelData, _entityManager, targetDoorId);

            var promptUI = new InteractionPromptUI(_player);
            _entityManager.AddObject(promptUI);

            if (spawnDoor != null)
            {
                _player.Position = spawnDoor.SpawnPosition;
                _camera.Position = _player.Position - _camera.CurrentViewportSize / 2f;
            }
        }
    }
}