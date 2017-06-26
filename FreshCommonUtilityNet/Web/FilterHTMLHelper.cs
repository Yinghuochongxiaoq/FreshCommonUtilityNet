using System.Net;
using System.Text.RegularExpressions;

// ReSharper disable once CheckNamespace
namespace FreshCommonUtility.Web
{
    /// <summary>
    /// Html标签过滤
    /// </summary>
    public class FilterHtmlHelper
    {
        /// <summary>
        /// 去除HTML标记
        /// </summary> 
        public static string NoHtml(string htmlString)
        {
            if (string.IsNullOrEmpty(htmlString)) return string.Empty;
            //删除脚本 
            htmlString = Regex.Replace(htmlString, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);

            htmlString = Regex.Replace(htmlString, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"-->", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"<!--.*", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&rarr;", "", RegexOptions.IgnoreCase);
            htmlString = Regex.Replace(htmlString, @"&mdash;", "", RegexOptions.IgnoreCase);
            htmlString = WebUtility.HtmlEncode(htmlString).Trim();

            return htmlString;
        }

        /// <summary>
        /// 去除脚本代码
        /// </summary>
        /// <param name="strHtml"></param>
        /// <returns></returns>
        public static string StripHtml(string strHtml)
        {
            if (string.IsNullOrEmpty(strHtml)) return string.Empty;
            string[] aryReg =
            {
              @"<script[^>]*?>.*?</script>",
              @"<(\/\s*)?!?((\w+:)?\w+)(\w+(\s*=?\s*(([""'])(\\[",
              @"'tbnr]|[^\7])*?\7|\w+)|.{0})|\s)*?(\/\s*)?>", @"([\r\n])[\s]+",
              @"&(quot|#34);", @"&(amp|#38);", @"&(lt|#60);", @"&(gt|#62);",
              @"&(nbsp|#160);", @"&(iexcl|#161);", @"&(cent|#162);", @"&(pound|#163);",
              @"&(copy|#169);", @"&#(\d+);", @"-->", @"<!--.*\n"
            };

            string[] aryRep =
            {
              "", "", "", "\"", "&", "<", ">", "   ", "\xa1",  //chr(161), 
              "\xa2",  //chr(162), 
              "\xa3",  //chr(163), 
              "\xa9",  //chr(169), 
              "", "\r\n", ""
            };

            string strOutput = strHtml;
            for (int i = 0; i < aryReg.Length; i++)
            {
                Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                strOutput = regex.Replace(strOutput, aryRep[i]);
            }
            strOutput = strOutput.Replace("<", "");
            strOutput = strOutput.Replace(">", "");
            strOutput = strOutput.Replace("\r\n", "");
            return strOutput;
        }

        /// <summary> 
        /// 取出文本中的图片地址 
        /// </summary> 
        /// <param   name="htmlStr">htmlStr</param> 
        public static string GetImgUrl(string htmlStr)
        {
            if (string.IsNullOrEmpty(htmlStr))
                return string.Empty;
            string str = string.Empty;
            Regex r = new Regex(@"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>", RegexOptions.Compiled);
            Match m = r.Match(htmlStr.ToLower());
            if (m.Success)
                str = m.Result("${url}");
            return str;
        }
    }
}
