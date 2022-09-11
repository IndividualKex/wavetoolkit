using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace WaveTools {
    [System.Serializable]
    public class WaveGrid {
        static readonly int MAX_ITERATIONS = 10000;
        static readonly int MAX_ATTEMPTS = 10;
        static readonly int[] OPPOSITE = { 2, 3, 0, 1, 5, 4 };

        public string name = "WaveGrid";
        public List<Module> modules;
        public int width = 4;
        public int length = 4;
        public int height = 1;
        public float moduleSize = 4f;
        public Vector3 offset;

        Vector3 moduleOffset;
        GameObject[] items;
        Transform root;
        Task task;
        bool[,,,] wave;
        bool[,] possibilities;
        int[] entropy;
        int ticks;
        bool failed;

        void Initialize() {
            root = new GameObject(name).transform;
            root.SetParent(Generator.instance.transform);
            root.localPosition = offset;
            moduleOffset = new Vector3(width * moduleSize / 2f, height * moduleSize / 2f, length * moduleSize / 2f);
            wave = new bool[width * length * height, 6, modules.Count, modules.Count];
            possibilities = new bool[width * length * height, modules.Count];
            entropy = new int[width * length * height];
            items = new GameObject[width * length * height];
            for(int index = 0; index < wave.GetLength(0); index++) {
                for(int j = 0; j < modules.Count; j++) {
                    bool possible = true;
                    for(int i = 0; i < 6; i++) {
                        if(!GetAdjacent(index, i, out _)) continue;
                        bool connected = false;
                        for(int k = 0; k < modules.Count; k++) {
                            if(modules[j].ConnectsTo(modules[k], i)) {
                                wave[index, i, j, k] = true;
                                connected = true;
                            }
                        }
                        if(!connected)
                            possible = false;
                    }
                    possibilities[index, j] = possible;
                    if(possible)
                        entropy[index]++;
                }
            }
        }

        public void Clear(bool safe = false) {
            if(root != null) {
#if UNITY_EDITOR
                if(safe)
                    EditorApplication.delayCall += () => GameObject.DestroyImmediate(root.gameObject);
                else
                    GameObject.DestroyImmediate(root.gameObject);
#else
                GameObject.DestroyImmediate(root.gameObject);
#endif
            }
            failed = false;
            wave = null;
            ticks = 0;
        }

        public void Generate() {
            if(task != null && !task.IsCompleted) return;
            task = Run();
        }

        async Task Run() {
            for(int i = 0; i < MAX_ATTEMPTS; i++) {
                Clear();
                Initialize();
                for(int j = 0; j < MAX_ITERATIONS; j++) {
                    int index = NextSlot();
                    if(index >= 0)
                        await Observe(index);
                    else
                        break;
                }
                if(failed)
                    Logging.Log($"Failed attempt {i + 1}", LoggingLevel.Warning);
                else
                    break;
            }
        }

        int NextSlot() {
            if(failed) return -1;
            int argmin = -1;
            float min = float.MaxValue;
            for(int i = 0; i < wave.GetLength(0); i++) {
                int entropy = this.entropy[i];
                if(entropy > 1 && entropy <= min) {
                    float noise = Random.Range(0f, 1f);
                    if(entropy + noise < min) {
                        min = entropy + noise;
                        argmin = i;
                    }
                }
            }
            return argmin;
        }

        async Task Observe(int index) {
            List<int> candidates = new List<int>();
            float min = float.MaxValue;
            float choice = -1;
            for(int i = 0; i < modules.Count; i++) {
                if(possibilities[index, i]) {
                    float noise = Random.Range(0f, 1f);
                    if(noise < min) {
                        min = noise;
                        choice = i;
                    }
                    candidates.Add(i);
                }
            }
            foreach(int candidate in candidates) {
                if(candidate != choice) {
                    await Propagate(index, candidate);
                }
            }
        }

        async Task Propagate(int index, int module) {
            if(Generator.delay > 0 && ++ticks >= Generator.delay) {
                ticks = 0;
                await Task.Delay(10);
            }
            if(entropy[index] <= 1) {
                failed = true;
                return;
            }
            possibilities[index, module] = false;
            entropy[index]--;
            for(int i = 0; i < 6; i++) {
                for(int j = 0; j < modules.Count; j++) {
                    wave[index, i, module, j] = false;

                    if(GetAdjacent(index, i, out int adjacent)) {
                        int opposite = OPPOSITE[i];
                        if(possibilities[adjacent, j]) {
                            wave[adjacent, opposite, j, module] = false;
                            bool possible = false;
                            for(int k = 0; k < modules.Count; k++) {
                                if(wave[adjacent, opposite, j, k]) {
                                    possible = true;
                                    break;
                                }
                            }
                            if(!possible)
                                await Propagate(adjacent, j);
                        }
                    }
                }
            }

            if(entropy[index] == 1)
                Collapse(index);
        }

        void Collapse(int index) {
            int module = -1;
            for(int i = 0; i < modules.Count; i++) {
                if(possibilities[index, i]) {
                    module = i;
                    break;
                }
            }
            if(module == -1) {
                Debug.LogError("Failed to collapse");
                return;
            }
            if(modules[module].prefab != null) {
                GameObject item = GameObject.Instantiate(modules[module].prefab, root);
                (int x, int y, int z) = Reshape(index);
                item.transform.localPosition = GetPosition(x, y, z);
                items[index] = item;
            }
        }

        public void DrawGizmos() {
            if(wave == null) return;
            Gizmos.color = Color.red;
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    for(int z = 0; z < length; z++) {
                        int index = Flatten(x, y, z);
                        int entropy = this.entropy[index];
                        if(entropy == 1) continue;
                        Gizmos.DrawCube(GetPosition(x, y, z), Vector3.one * 2f * entropy / modules.Count);
                    }
                }
            }
        }

        #region HELPERS
        bool GetAdjacent(int index, int direction, out int adjacent) {
            adjacent = -1;
            (int x, int y, int z) = Reshape(index);
            switch(direction) {
                // Down
                case 0:
                    if(z > 0) {
                        adjacent = index - width;
                        return true;
                    }
                    return false;
                // Left
                case 1:
                    if(x > 0) {
                        adjacent = index - 1;
                        return true;
                    }
                    return false;
                // Up
                case 2:
                    if(z < length - 1) {
                        adjacent = index + width;
                        return true;
                    }
                    return false;
                // Right
                case 3:
                    if(x < width - 1) {
                        adjacent = index + 1;
                        return true;
                    }
                    return false;
                // Above
                case 4:
                    if(y < height - 1) {
                        adjacent = index + length * width;
                        return true;
                    }
                    return false;
                // Below
                case 5:
                    if(y > 0) {
                        adjacent = index - length * width;
                        return true;
                    }
                    return false;
                default:
                    throw new System.ArgumentException("Invalid direction");
            }
        }

        public int Flatten(int x, int y, int z) {
            return length * width * y + width * z + x;
        }

        public (int, int, int) Reshape(int index) {
            int y = index / (length * width);
            index -= y * length * width;
            int z = index / width;
            int x = index % width;
            return (x, y, z);
        }

        Vector3 GetPosition(int x, int y, int z) {
            Vector3 position = new Vector3(
                x * moduleSize - moduleOffset.x + moduleSize / 2f,
                y * moduleSize - moduleOffset.y + moduleSize / 2f,
                z * moduleSize - moduleOffset.z + moduleSize / 2f
            );
            return position;
        }
        #endregion
    }
}