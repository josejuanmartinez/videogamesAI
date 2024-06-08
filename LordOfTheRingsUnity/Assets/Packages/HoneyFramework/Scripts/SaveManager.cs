using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace HoneyFramework
{
    [System.Serializable]
    public class TextureData
    {
        public int width;
        public int height;
        public byte[] imageData;
        public FilterMode filterMode;
        public string name;
        public TextureWrapMode wrapMode;
        public TextureWrapMode wrapModeU;
        public TextureWrapMode wrapModeV;
        public TextureWrapMode wrapModeW;
        public TextureFormat format;
        public bool mipChain;
    }

    [Serializable()]
    public class SaveStructure
    {
        public Dictionary<Vector3i, List<Vector2i>> hexesChunks;
        public int worldHexRadius;
        public List<Hex> hexes;
        public List<RiverData> riverData;
        public List<SaveChunkStructure> saveChunkStructures;
    }

    [Serializable()]
    public class SaveChunkStructure
    {
        public Chunk chunk;
        public RiverData riverData;
        public TextureData d;
        public TextureData h;
        public TextureData shadows;
        public List<ForegroundData> foregroundData;
    }

    /*
     * Class used to save / load map data from/to drive
     */
    public class SaveManager
    {
        public static string fileName = "map";
        public static string saveName = Path.Combine(Application.streamingAssetsPath, fileName);
        public static string saveNameb64 = Path.Combine(Application.streamingAssetsPath, fileName + ".txt");


        /// <summary>
        /// Saves current state of the world
        /// </summary>
        /// <param name="w"></param>
        /// <param name="withTextures"></param>
        /// <returns></returns>
        static public void Save(World w)
        {
            SaveStructure save = new();
            save.worldHexRadius = w.GetHexRadius();
            save.riverData = w.riversStart;
            save.hexesChunks = new();

            save.hexes = new List<Hex>();
            foreach(KeyValuePair<Vector3i, Hex> pair in w.hexes)
            {
                save.hexes.Add(pair.Value);
            }

            save.saveChunkStructures = new();
            foreach (Chunk chunk in w.chunks.Values)
            {
                SaveChunkStructure saveChunkStructure = new SaveChunkStructure();

                saveChunkStructure.chunk = chunk;
                saveChunkStructure.d = SerializeTexture(chunk.diffuse);
                saveChunkStructure.h = SerializeTexture(chunk.height);
                saveChunkStructure.foregroundData = chunk.foregroundData;
                saveChunkStructure.shadows = SerializeTexture(chunk.shadows);

                foreach (Vector3i v3Hex in chunk.hexesCovered.Keys)
                {
                    Vector3i v3iz0 = new Vector3i(v3Hex.x, v3Hex.y, 0);
                    if (!save.hexesChunks.ContainsKey(v3Hex))
                        save.hexesChunks[v3iz0] = new();
                    save.hexesChunks[v3iz0].Add(chunk.position);
                }

                SaveChunkSeparately(saveChunkStructure);
            }

            Stream stream = File.Open(saveName, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, save);
            stream.Close();

            Debug.Log("File saved successfully to " +  saveName);
        }

        static public void SaveChunkSeparately(SaveChunkStructure saveChunkStructure)
        {
            string chunkSaveName = Path.Combine(Application.streamingAssetsPath, "Chunks", saveChunkStructure.chunk.position.ToString());
            Stream stream = File.Open(chunkSaveName, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, saveChunkStructure);
            stream.Close();
        }

        /// <summary>
        /// Load world from save file
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        static public Dictionary<Vector3i, List<Vector2i>> LoadTerrain(World w)
        {
            try
            {
                Stream stream = File.Open(saveName, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                SaveStructure save = (SaveStructure)bFormatter.Deserialize(stream);
                stream.Close();
                w.SetHexRadius(save.worldHexRadius);
                w.riversStart = save.riverData;

                foreach (Hex hex in save.hexes)
                {
                    w.hexes[hex.position] = hex;
                }

                w.chunks = new();
                List<SaveChunkStructure> chunks = save.saveChunkStructures;

                for (int i = 0; i < chunks.Count; i++)
                    LoadChunk(w, chunks[i]);

                return save.hexesChunks;
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
                return null;
            }
        }

        public static void LoadChunkFromChunkPosition(World w, Vector2i chunkPosition)
        {
            try
            {
                string chunkSaveName = Path.Combine(Application.streamingAssetsPath, "chunks", chunkPosition.ToString());
                Stream stream = File.Open(chunkSaveName, FileMode.Open);
                BinaryFormatter bFormatter = new BinaryFormatter();
                SaveChunkStructure chunk = (SaveChunkStructure)bFormatter.Deserialize(stream);
                stream.Close();

                LoadChunk(w, chunk);
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError(e.Message);
            }
        }
        private static void LoadChunk (World w, SaveChunkStructure chunk)
        {
            Texture2D d = new (
                        chunk.d.width,
                        chunk.d.height,
                        chunk.d.format,
                        chunk.d.mipChain
                    );
            d.name = chunk.d.name;
            d.wrapMode = chunk.d.wrapMode;
            d.wrapModeU = chunk.d.wrapModeU;
            d.wrapModeV = chunk.d.wrapModeV;
            d.wrapModeW = chunk.d.wrapModeW;
            d.LoadImage(chunk.d.imageData);


            Texture2D h = new Texture2D(
                chunk.h.width,
                chunk.h.height,
                chunk.h.format,
                chunk.h.mipChain
            );
            h.name = chunk.h.name;
            h.wrapMode = chunk.h.wrapMode;
            h.wrapModeU = chunk.h.wrapModeU;
            h.wrapModeV = chunk.h.wrapModeV;
            h.wrapModeW = chunk.h.wrapModeW;
            h.LoadImage(chunk.h.imageData);

            Texture2D shadows = new Texture2D(
                chunk.shadows.width,
                chunk.shadows.height,
                chunk.shadows.format,
                chunk.shadows.mipChain
            );
            shadows.name = chunk.shadows.name;
            shadows.wrapMode = chunk.shadows.wrapMode;
            shadows.wrapModeU = chunk.shadows.wrapModeU;
            shadows.wrapModeV = chunk.shadows.wrapModeV;
            shadows.wrapModeW = chunk.shadows.wrapModeW;
            shadows.LoadImage(chunk.shadows.imageData);

            Chunk worldChunk = chunk.chunk;
            worldChunk.diffuse = d;
            worldChunk.height = h;
            worldChunk.shadows = shadows;
            worldChunk.foregroundData = chunk.foregroundData;
            CompressDiffuse(worldChunk);
            w.chunks.Add(worldChunk.position, worldChunk);
        }

        public static TextureData SerializeTexture(Texture2D texture)
        {
            byte[] imageData;
            if (IsTextureCompressed(texture))
                imageData = ConvertCompressedTextureToJPG(texture);
            else
                imageData = texture.EncodeToPNG();


            return new TextureData()
            {
                width = texture.width,
                height = texture.height,
                filterMode = texture.filterMode,
                name = texture.name,
                wrapMode = texture.wrapMode,
                wrapModeU = texture.wrapModeU,
                wrapModeV = texture.wrapModeV,
                wrapModeW = texture.wrapModeW,
                format = texture.format,
                mipChain = texture.mipmapCount > 1,
                imageData = imageData
            };
        }

        public static byte[] ConvertCompressedTextureToJPG(Texture2D compressedTexture)
        {
            // Create a new Texture2D with the same dimensions but in an uncompressed format
            Texture2D uncompressedTexture = new Texture2D(compressedTexture.width, compressedTexture.height, TextureFormat.RGBA32, false);

            // Copy the pixel data from the compressed texture to the new texture
            uncompressedTexture.SetPixels(compressedTexture.GetPixels());

            // Apply the changes to the new texture
            uncompressedTexture.Apply();

            // Encode the new texture to PNG
            byte[] pngData = uncompressedTexture.EncodeToJPG();

            return pngData;
        }

        public static void CompressDiffuse(Chunk chunk)
        {
            if (!chunk.diffuseCompressed)
            {
                chunk.diffuseCompressed = true;
                chunk.diffuse.Compress(false);
                chunk.diffuse.Apply();
            }
        }

        public static bool IsTextureCompressed(Texture2D texture)
        {
            TextureFormat format = texture.format;
            return IsCompressedFormat(format);
        }

        public static bool IsCompressedFormat(TextureFormat format)
        {
            switch (format)
            {
                case TextureFormat.DXT1:
                case TextureFormat.DXT5:
                case TextureFormat.ETC_RGB4:
                case TextureFormat.ETC2_RGB:
                case TextureFormat.ETC2_RGBA1:
                case TextureFormat.ETC2_RGBA8:
                case TextureFormat.ASTC_4x4:
                case TextureFormat.ASTC_5x5:
                case TextureFormat.ASTC_6x6:
                case TextureFormat.ASTC_8x8:
                case TextureFormat.ASTC_10x10:
                case TextureFormat.ASTC_12x12:
                case TextureFormat.PVRTC_RGB2:
                case TextureFormat.PVRTC_RGBA2:
                case TextureFormat.PVRTC_RGB4:
                case TextureFormat.PVRTC_RGBA4:
                case TextureFormat.BC4:
                case TextureFormat.BC5:
                case TextureFormat.BC6H:
                case TextureFormat.BC7:
                    return true;
                default:
                    return false;
            }
        }
    }
}