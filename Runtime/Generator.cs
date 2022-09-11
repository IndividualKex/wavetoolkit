using UnityEngine;

namespace WaveTools {
    public class Generator : MonoBehaviour {
        static Generator _instance;
        public static Generator instance {
            get {
                if(_instance == null) {
                    Generator[] instances = FindObjectsOfType<Generator>();
                    if(instances.Length == 0) {
                        Debug.LogWarning("Couldn't find a Generator instance");
                    } else {
                        if(instances.Length > 1)
                            Debug.LogWarning("Found multiple Generator instances");
                        _instance = instances[0];
                    }
                }
                return _instance;
            }
        }

        public static int delay { get; set; }

        public WaveGrid[] grids;
        public LoggingLevel loggingLevel;
        public bool drawGizmos = true;
        public float idelay = 0;

        void Update() {
            if(Input.GetKeyDown(KeyCode.B)) {
                for(int i = 0; i < grids.Length; i++)
                    grids[i].Generate();
            }
        }

        void OnDrawGizmos() {
            if(!drawGizmos) return;
            for(int i = 0; i < grids.Length; i++)
                grids[i].DrawGizmos();
        }

        void OnValidate() {
            if(grids == null)
                grids = new WaveGrid[0];
            for(int i = 0; i < grids.Length; i++)
                grids[i]?.Clear(true);
        }
    }
}