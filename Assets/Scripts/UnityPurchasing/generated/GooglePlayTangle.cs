// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("FZaYl6cVlp2VFZaWlwPCtbRruWYj6TUhvQQS2p40H+9796h2MVcM8SWJqDRsUVkVFaD5xtiCW3cQfdMHsmjGeKLdGJBy1q6D68hEx6ouBfysncWYkxtQRhaX43iz+0tTZCVWZ6qFdYJmSv1T+PdZ6sAOPflwGfyTjY+KZPHniElr8zCTEJyF7WK1ZxWZFj/GL21jm/LCId4/DX6Td7zvIOxsUIeGK/3fmi6QXTYcr9ldW3OyGYfWMzUpeDCjGwG5IKoxlgin1vSnFZa1p5qRnr0R3xFgmpaWlpKXlLzvocSDueCxFfwZSio3wb6j9ShmwiYtNu8t1NpfjI+HZzvRuswa0cX/mUF++6thMuJZ/m9jdoJnCh6UDb1V08zW9KwsxpWUlpeW");
        private static int[] order = new int[] { 1,3,8,9,4,11,7,13,12,12,12,11,12,13,14 };
        private static int key = 151;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
