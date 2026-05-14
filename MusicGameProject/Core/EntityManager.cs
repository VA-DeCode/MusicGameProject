using MusicGameProject.Entities;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;

namespace MusicGameProject.Core
{
    public class EntityManager
    {
        private readonly List<GameObject>[] _layerLists;
        private readonly List<GameObject> _updateList = new();

        public StaticPlatform[,] CollisionGrid { get; private set; }
        private float _tileSize;

        public void InitGrid(int widthInTiles, int heightInTiles)
        {
            CollisionGrid = new StaticPlatform[widthInTiles, heightInTiles];
            _tileSize = Config.Level.TileSize;
        }

        public void AddPlatformToGrid(int tileX, int tileY, StaticPlatform platform)
        {
            CollisionGrid[tileX, tileY] = platform;
            _layerLists[(int)Layer.Middleground].Add(platform);
        }

        public EntityManager()
        {
            int layerCount = Enum.GetNames(typeof(Layer)).Length;
            _layerLists = new List<GameObject>[layerCount];
            for (int i = 0; i < layerCount; i++)
                _layerLists[i] = new List<GameObject>();
        }

        public void AddObject(GameObject obj)
        {
            _layerLists[(int)obj.ObjectLayer].Add(obj);
            _updateList.Add(obj);
        }

        // TODO: Публичный доступ к списку (только для чтения, чтобы никто случайно не удалил объекты)
        public IReadOnlyList<GameObject> AllObjects => _updateList;

        public void UpdateAll(double deltaTime)
        {
            for (int i = 0; i < _updateList.Count; i++)
                _updateList[i].Update(deltaTime, this);
        }

        public void RenderAll(GL gl, ShapeRenderer renderer, Camera camera)
        {
            // Рисуем слои по порядку индексов (0: Background, 1: Middleground, 2: Foreground, 3: UI)
            for (int l = 0; l < _layerLists.Length; l++)
            {
                var currentLayer = _layerLists[l];
                if (currentLayer.Count == 0) continue;

                renderer.BeginLayer(camera, (Layer)l);

                for (int i = 0; i < currentLayer.Count; i++)
                {
                    var obj = currentLayer[i];
                    if (obj.Color.W <= 0.001f) continue;

                    renderer.Submit(obj.Position, obj.Size, obj.Color);
                }

                renderer.EndLayer();
            }
        }

        public void Clear()
        {
            for (int i = 0; i < _layerLists.Length; i++)
                _layerLists[i].Clear();
            _updateList.Clear();
        }
    }
}