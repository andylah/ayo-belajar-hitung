// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("sQOAo7GMh4irB8kHdoyAgICEgYK8k2OUcFzrRe7hT/zWGCvvZg/qhQOAjoGxA4CLgwOAgIEV1KOifa9wm5mccufxnl995SaFBoqT+3SjcQMzn74iekdPAwO279DOlE1hBmvFETX/IzerEgTMiCIJ+W3hvmAnQRrnD5HAJSM/bia1DRevNrwngB6xwOKPACnQOXt1jeTUN8gpG2iFYar5Nvp6RpGQPevJjDiGSyAKuc9LTWWk6Y9XaO29dyT0T+h5dWCUcRwIghu6i9OOhQ1GUACB9W6l7V1FcjNAcar5t9KVr/anA+oPXDwh16i14z5w1DA7IPk7wsxJmpmRcS3HrNoMx9OkftButMsOhmTAuJX93lLRvDgT6qtDxdrA4ro60IOCgIGA");
        private static int[] order = new int[] { 0,11,11,7,8,7,11,13,12,11,12,13,12,13,14 };
        private static int key = 129;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
