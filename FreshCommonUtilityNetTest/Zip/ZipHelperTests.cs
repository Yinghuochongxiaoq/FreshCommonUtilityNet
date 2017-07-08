using FreshCommonUtility.Zip;

namespace FreshCommonUtilityNetTest.Zip
{
    public class ZipHelperTests
    {
        public void GZipCompressStringTest()
        {
            var str = "1";
            var enCode = ZipHelper.GZipCompressString(str);
            var deCode = ZipHelper.GZipDecompressString(enCode);
            str.IsEqualTo(deCode);
        }

        public void GZipDecompressStringTest()
        {
            var str = "1djifaniJHUNj&94850#$@^pdnfmcf478451578`~)(（）+- _)(%@! {}[]【】";
            var enCode = ZipHelper.GZipCompressString(str);
            var deCode = ZipHelper.GZipDecompressString(enCode);
            str.IsEqualTo(deCode);
        }
    }
}