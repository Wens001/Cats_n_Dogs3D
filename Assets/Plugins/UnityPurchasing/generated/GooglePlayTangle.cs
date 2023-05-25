#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rGNg8qHijLOjk2eTs7oZk+5QPGOfq67mcF4Cl6gxl3FCKG+p/4D/dNRyPllRBCdPkIGjv5WP36JnwKp1C7k6GQs2PTIRvXO9zDY6Ojo+Ozgyak4EJhznqX62RATszmBTI+RQ9vsqNVRWxzMJT5ZD4tGOlEslSwOpZ6szElnDnHgaNi5bwdLrr8huJ5y5OjQ7C7k6MTm5Ojo74CvG8yDtOKPnsDuvvg2ENpZ6NXD01cf4ODQWcq4OONc1GpQN8I+JWMaXVUEQ1gJq6nZwBeW3fgJdyXpCv0BvGSKKZlSJM7dztidnKZDXyFjVLPtjCwi1zCoKA+O3wcS3VvNFejK2ryrAzEiEWoN3oOocDiZBbcnDd3scFEwZ0wtQXRYdc32HWjk4Ojs6");
        private static int[] order = new int[] { 4,2,11,4,9,5,6,11,13,10,11,12,12,13,14 };
        private static int key = 59;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
