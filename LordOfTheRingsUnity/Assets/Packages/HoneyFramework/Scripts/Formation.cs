using UnityEngine;
using System.Collections.Generic;

namespace HoneyFramework
{
    /*
     *  Formation class to manage group of the character actors within single hex and their movement
     */
    public class Formation : MonoBehaviour
    {
        static public List<Formation> formations = new List<Formation>();

        public Vector3i position;
        public float formationSpeed = 0.5f;
        List<CharacterActor> characters = new List<CharacterActor>();
        public Vector3 direction = Vector3.back;
        public float formationLazines = 1.0f;

        #region FormationManagement
        /// <summary>
        /// Creates formation at specified position (hex space coordinate)
        /// </summary>
        /// <param name="name"> name for formation </param>
        /// <param name="position"> hex position of the formation </param>
        /// <returns> returns instance of the formation </returns>
        static public Formation CreateFormation(string name, Vector3i position)
        {
            GameObject go = new GameObject(name);
            go.transform.parent = World.instance.transform;
            go.transform.localPosition = Vector3.zero;
            Formation f = go.AddComponent<Formation>();
            formations.Add(f);
            f.position = position;

            return f;
        }


        /// <summary>
        /// Destroy single or more formations where you may specify either of name and position (or both), each formation passing requirements would be destroyed.
        /// If no requirements specified ALL formations would be cleared
        /// </summary>
        /// <param name="name">option to provide name of the formations considered for destruction</param>
        /// <param name="position">option to provide position of formations considered for destruction</param>
        /// <returns></returns>
        static public void DestroyFormations(string name, Vector3i? position)
        {

            List<Formation> formationGroup = formations;

            if (position != null)
            {
                formationGroup = formationGroup.FindAll(o => o.position == position);
            }

            if (name.Length > 0)
            {
                string n = name.ToString();
                formationGroup = formationGroup.FindAll(o => o.gameObject.name == n);
            }

            foreach (Formation f in formationGroup)
            {
                formations.Remove(f);
                Destroy(f);
            }
        }
        #endregion

        /// <summary>
        /// adds single character to the formation
        /// </summary>
        /// <param name="a">character instance to join formation</param>
        /// <returns></returns>
        public void AddCharacter(CharacterActor a)
        {
            Vector3 offset = GetFormationOffset(characters.Count, direction);
            a.transform.parent = transform;
            
            a.SetWorldPosition(HexCoordinates.HexToWorld3D(position) + offset);

            characters.Add(a);
            a.parent = this;
        }
       
        /// <summary>
        /// Finds position within formation for the character (an offset from hex center)
        /// </summary>
        /// <param name="index"> index within formation </param>
        /// <param name="dir"> direction at which formation looks at </param>
        /// <returns></returns>
        public Vector3 GetFormationOffset(int index, Vector3 dir)
        {
            Vector3 right = Vector3.right * 0.3f;
            Vector3 left = new Vector3(-right.x, 0, right.z);
            Vector3 back = Vector3.back * 0.3f;
            int perRowCount = 5;

            int row = (int)(index / perRowCount);
            index -= row * perRowCount;

            int k = index % 2;
            int p = (int)(index / 2);

            p += k;
            Vector3 offset = p * (k > 0 ? right : left);
            offset += row * back;

            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, dir);
            return rot * offset;
        }
    }
}