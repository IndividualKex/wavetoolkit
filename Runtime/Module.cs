using UnityEngine;

namespace WaveTools {
    [CreateAssetMenu(menuName = "Wave Toolkit/Module")]
    public class Module : ScriptableObject {
        public GameObject prefab;
        public Connection down;
        public Connection left;
        public Connection up;
        public Connection right;
        public Connection above;
        public Connection below;

        public bool ConnectsTo(Module other, int direction) {
            switch(direction) {
                case 0:
                    return down.ConnectsTo(other.up);
                case 1:
                    return left.ConnectsTo(other.right);
                case 2:
                    return up.ConnectsTo(other.down);
                case 3:
                    return right.ConnectsTo(other.left);
                case 4:
                    return above.ConnectsTo(other.below);
                case 5:
                    return below.ConnectsTo(other.above);
                default:
                    throw new System.ArgumentException("Invalid direction");
            }
        }
    }
}
