using UnityEngine;

namespace _Scripts.Other
{
    [CreateAssetMenu(fileName = "New Level", menuName = "Level")]
    public class Level : ScriptableObject
    {
        public string levelName;
        public string sceneName;
        public int world;
        public int index;
        public int crystalCount;
        public int tabletCount;
        //music
    }
}
