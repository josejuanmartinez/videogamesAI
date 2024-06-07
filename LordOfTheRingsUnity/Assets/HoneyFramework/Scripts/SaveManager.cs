using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace HoneyFramework
{
    [Serializable()]
    public class SaveStructure
    {
        public int worldHexRadius;
        public List<Chunk> chunks;
        public List<Hex> hexes;
        public List<RiverData> riverData;
        public List<Texture2D> d;
        public List<Texture2D> h;
        public List<Texture2D> shadows;
        public List<List<ForegroundData>> foregroundData;
    }

    /*
     * Class used to save / load map data from/to drive
     */
    public class SaveManager
    {
        static string saveName = Application.persistentDataPath + "LOTR";

        /// <summary>
        /// Saves current state of the world
        /// </summary>
        /// <param name="w"></param>
        /// <param name="withTextures"></param>
        /// <returns></returns>
        static public void Save(World w, bool withTextures)
        {
            SaveStructure save = new SaveStructure();
            save.worldHexRadius = w.hexRadius;
            save.riverData = w.riversStart;

            save.hexes = new List<Hex>();
            foreach(KeyValuePair<Vector3i, Hex> pair in w.hexes)
            {
                save.hexes.Add(pair.Value);
            }

            if (withTextures)
            {
                save.chunks = new();
                save.d = new();
                save.h = new();
                save.foregroundData = new();
                save.shadows = new();
                foreach (Chunk chunk in w.chunks.Values)
                {
                    save.chunks.Add(chunk);
                    save.d.Add(chunk.diffuse);
                    save.h.Add(chunk.height);
                    save.foregroundData.Add(chunk.foregroundData);
                    save.shadows.Add(chunk.shadows);
                }
                save.chunks = w.chunks.Values.ToList();
            }

            Stream stream = File.Open(saveName, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, save);
            stream.Close();            
        }

        /// <summary>
        /// Load world from save file
        /// </summary>
        /// <param name="w"></param>
        /// <returns></returns>
        static public bool Load(World w)
        {
            SaveStructure save;
			try
			{
	            Stream stream = File.Open(saveName, FileMode.Open);
	            BinaryFormatter bFormatter = new BinaryFormatter();
	            save = (SaveStructure)bFormatter.Deserialize(stream);
	            stream.Close();

	            if (save == null) return false;

	            w.hexRadius = save.worldHexRadius;
	            w.riversStart = save.riverData;

	            foreach (Hex hex in save.hexes)
	            {
	                w.hexes[hex.position] = hex;
	            }

	            if (save.chunks != null)
	            {
                    w.chunks = new();
                    
                    List<Chunk> chunks = save.chunks;
                    List<Texture2D> d = save.d;
                    List<Texture2D> h = save.h;
                    List<Texture2D> shadows = save.shadows;
                    List<List<ForegroundData>> foregroundData = save.foregroundData;

                    for(int i=0; i<chunks.Count; i++)
                    {
                        Chunk chunk = chunks[i];
                        chunk.diffuse = d[i];
                        chunk.height = h[i];
                        chunk.shadows= shadows[i];
                        chunk.foregroundData= foregroundData[i];
                        w.chunks.Add(chunk.position, chunk);
                    }
	            }

	            return true;
			}
			catch 
			{
				return false;
			}
        }


        /*static private Savemanager instance;

        static public Savemanager GetInstance()
        {
            if (instance == null)
            {
                instance = new Savemanager();
            }

            return instance;
        }*/


    }
}