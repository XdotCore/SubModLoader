using SubmachineModLib;
using SubmachineModLib.Models;
using System;
using System.Linq;
using static SubmachineModLib.Models.GameMakerRoom;

namespace SubModLoader.GameData.Extensions {
    /// <summary>
    /// Helpful extension funcs
    /// </summary>
    public static class GameMakerRoomExt {
        /// <summary>
        /// Adds a new layer to the room
        /// </summary>
        /// <typeparam name="T">The type of layer data either <see cref="Layer.LayerInstancesData"/>, <see cref="Layer.LayerTilesData"/>, <see cref="Layer.LayerBackgroundData"/>, <see cref="Layer.LayerAssetsData"/>, or <see cref="Layer.LayerEffectData"/></typeparam>
        /// <param name="room">The room to add the layer to</param>
        /// <param name="gameData">The game data</param>
        /// <param name="name">The name of the new layer</param>
        /// <returns>The new layer</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>Only for Gamemaker Studio 2 and above</remarks>
        public static Layer AddLayer<T>(this GameMakerRoom room, GameMakerData gameData, string name) where T : Layer.LayerData, new() {
            if (!gameData.IsGameMaker2())
                throw new InvalidOperationException("Layers do not exist before GMS2.");

            uint layerId = gameData.Rooms.SelectMany(r => r.Layers).Select(l => l.LayerId).Max() + 1;

            int layerDepth = Math.Min(room.Layers.Select(l => l.LayerDepth).Max() + 1, int.MaxValue);

            Type layerDataType = typeof(T);
            LayerType layerType;
            if (layerDataType == typeof(Layer.LayerInstancesData))
                layerType = LayerType.Instances;
            else if (layerDataType == typeof(Layer.LayerTilesData))
                layerType = LayerType.Tiles;
            else if (layerDataType == typeof(Layer.LayerBackgroundData))
                layerType = LayerType.Background;
            else if (layerDataType == typeof(Layer.LayerAssetsData))
                layerType = LayerType.Assets;
            else if (layerDataType == typeof(Layer.LayerEffectData))
                layerType = LayerType.Effect;
            else
                throw new InvalidOperationException($"Type {layerDataType} is not a recognized layer type.");

            Layer layer = new() {
                LayerName = gameData.Strings.MakeString(name),
                LayerId = layerId,
                LayerType = layerType,
                LayerDepth = layerDepth,
                Data = new T(),
            };
            room.Layers.Add(layer);
            layer.ParentRoom = room;

            if (layer.LayerType == LayerType.Assets) {
                layer.AssetsData.LegacyTiles = new GameMakerPointerList<Tile>();
                layer.AssetsData.Sprites = new GameMakerPointerList<SpriteInstance>();
                layer.AssetsData.Sequences = new GameMakerPointerList<SequenceInstance>();

                if (!gameData.IsVersionAtLeast(2, 3, 2))
                    layer.AssetsData.NineSlices = new GameMakerPointerList<SpriteInstance>();
            } else if (layer.LayerType == LayerType.Tiles)
                layer.TilesData.TileData ??= Array.Empty<uint[]>();

            return layer;
        }
    }
}
