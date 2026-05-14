using MusicGameProject.Entities;
using Silk.NET.Maths;
using System.Collections.Generic;
using System;

namespace MusicGameProject.Core
{
    public static class LevelBuilder
    {
        public static float TileSize = Config.Level.TileSize;
        public static Door BuildFromData(LevelData data, EntityManager entityManager, char targetDoorToFind)
        {
            Door foundDoor = null;
            float tileSize = Config.Level.TileSize;
            int width = data.Map[0].Length;
            int height = data.Map.Length;

            // Создаем сетку под размеры уровня
            entityManager.InitGrid(width, height);

            for (int y = 0; y < height; y++)
            {
                string row = data.Map[y];
                for (int x = 0; x < width; x++)
                {
                    char tile = row[x];
                    var position = new Vector2D<float>(x * tileSize, y * tileSize);

                    if (tile == '#')
                    {
                        var platform = new StaticPlatform(position, new Vector2D<float>(tileSize, tileSize), new Vector4D<float>(0.3f, 0.3f, 0.3f, 1.0f));
                        entityManager.AddPlatformToGrid(x, y, platform);
                    }
                    else if (tile == 'T')
                    {
                        var terminalPos = new Vector2D<float>(x * TileSize, y * TileSize + TileSize - 60f);
                        var terminal = new InteractableObject(terminalPos);

                        terminal.OnInteract += () =>
                        {
                            Console.WriteLine("Взаимодействие с терминалом успешно!");
                        };

                        entityManager.AddObject(terminal);
                    }
                    else if (data.Doors != null && data.Doors.ContainsKey(tile))
                    {
                        var dData = data.Doors[tile];
                        var doorPos = new Vector2D<float>(x * TileSize, y * TileSize - TileSize);
                        var doorSize = new Vector2D<float>(TileSize, TileSize * 2);

                        float spawnY = (y * TileSize + TileSize) - Config.Player.Size.Y;
                        float spawnX = dData.Direction == "Right" ? doorPos.X - 50f : doorPos.X + doorSize.X + 10f;

                        var door = new Door(doorPos, doorSize, tile, dData.TargetLevel, dData.TargetDoor, new Vector2D<float>(spawnX, spawnY));
                        entityManager.AddObject(door);

                        if (tile == targetDoorToFind) foundDoor = door;
                    }
                }
            }
            return foundDoor;
        }
    }
}