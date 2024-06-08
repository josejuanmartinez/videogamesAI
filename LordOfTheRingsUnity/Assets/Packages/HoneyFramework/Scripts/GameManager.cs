using System.Collections.Generic;
using UnityEngine;

namespace HoneyFramework
{
    /*
     * Example game manager class which controls world seed and pathfinder resources.
     */
    public class GameManager : MonoBehaviour
    {
        static public GameManager instance;
        public int currentSeed = 0;
        bool initializedSeed = false;

        public List<Formation> formations = new List<Formation>();

        /// <summary>
        /// Prepares pathfinder component 
        /// </summary>
        /// <returns></returns>
        void Awake()
        {
            instance = this;
        }

        /// <summary>
        /// Prepares some basic gameplay resources (used mostly for demo/debug) 
        /// and sets seed from game settings or if they are 0 there then from scene
        /// if its 0 there a well it will leave seed randomized
        /// </summary>
        /// <returns></returns>
        void Start()
        {
            if (!initializedSeed)
            {
                Random.State originalState = Random.state;
                DataManager.GetInstance();
                int settingsSeed = MHGameSettings.GetWorldSeed();
                if (settingsSeed != 0)
                {
                    UnityEngine.Random.InitState(settingsSeed);
                }
                else if (World.GetInstance().seed != 0)
                {
                    UnityEngine.Random.InitState(World.GetInstance().seed);
                }
                currentSeed = settingsSeed != 0 ? settingsSeed : World.GetInstance().seed;
                initializedSeed = true;
            }            
        }
    }
}

