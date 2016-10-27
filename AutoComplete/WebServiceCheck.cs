using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Xml;
using ICSharpCode.SharpZipLib.GZip;
using System.IO;
namespace AutoComplete
{
    class WebServiceCheck
    {

        private string URL;
        private string _hostName = "";
        private string _userName = "";
        private string _password = "";

        public WebServiceCheck() { 

        }


        public WebServiceCheck(string GIDOrEXOI) {
            if (GIDOrEXOI.Trim().ToUpper().Equals("GID"))
            {
                _hostName = @"http://globalid.morningstar.com/GIDDataIO/reg/login.aspx?";
                _userName = "GlobalEquityData@morningstar.com";
                _password = "GXy1q88E";
            }
            else {
                _hostName = @"";
                _userName = "GlobalEquityData@morningstar.com";
                _password = "GXy1q88E";
            }
        }

        private static string _detailcookieHeader = null;
        private DateTime _lastCookieTime;
        private DateTime _lastDetailCookieTime;
        private int _cookieExpirationTime = 30 * 2 * 2;
        private static int _timeoutInterval = 60000 * 60 * 2;//1 min

        private string _loginEXOIUrl = @"http://equitydata.morningstar.com/login/login.aspx?username=GlobalEquityData@morningstar.com&password=GXy1q88E";
        private static string _check90 = @"http://globalid.morningstar.com/GIDDataIO/feed/asmx/Axis.asmx/GetYAxisDataTable?outputDPs=-2,-3,-5,1,91,84,70,120,100,101,103,50,53,85,250,40,52,51&inputQueryString=D91=DNAI&IdType=InvestmentProductId&Content=Operation";
        private static string _autoCompleteOutput = @"";
        private bool _bUserAuthentication = true;

        #region 批量校验AutoComplete的Output {json-无需登录} 
        #endregion

        #region 返回GID的结果{Xml-需登录}

        #endregion

        #region 检查ShareClassOperation的LastTradingDate和现在是否超过90天-确认workday还是calendar {-需登录}
        #endregion


        #region 获取Cookie
        public static void LoginGID() {
            HttpWebRequest webRequest = null;
            webRequest = (HttpWebRequest)WebRequest.Create(@"http://globalid.morningstar.com/GIDDataIO/reg/login.aspx?username=GlobalEquityData@morningstar.com&password=GXy1q88E");
            webRequest.CookieContainer = new CookieContainer();
            webRequest.AllowAutoRedirect = false;

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            _detailcookieHeader = webResponse.Headers.Get("Set-Cookie");
            //webResponse.Close();
        }
        #endregion

        public static void test() {
            isOver90(_check90);
        }

        public void GetURL(string package, string Content, string ID, string IdType)
        {
        }

        public void BatchCheck(){
            HttpWebRequest webRequest;

            webRequest = (HttpWebRequest)WebRequest.Create(_autoCompleteOutput);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.AllowAutoRedirect = false;

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            string ch = webResponse.Headers.Get("Set-Cookie");

            if (webResponse.StatusCode.Equals(HttpStatusCode.OK))
            {
                _detailcookieHeader = webResponse.Headers.Get("Set-Cookie");
            }
        }

        public static bool isOver90(string URL)
        {
            HttpWebRequest webRequest = null;
            HttpWebResponse webResponse = null;
            bool rtn = false;
            XmlDocument outdoc = null;
            try
            {
                webRequest = (HttpWebRequest) WebRequest.Create(URL);
                webRequest.Headers.Set("Accept-Language", "zh-cn\r\n");
                webRequest.Headers.Set("Accept-Encoding", "gzip,deflate\r\n");
                webRequest.Timeout = _timeoutInterval;
                webRequest.KeepAlive = true;
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.SetCookies(webRequest.RequestUri, _detailcookieHeader);

                webResponse = (HttpWebResponse) webRequest.GetResponse();
                if (webResponse.StatusCode.Equals(HttpStatusCode.OK)) {
                    outdoc = new XmlDocument();
                    if (webResponse.ContentEncoding.Equals("gzip"))
                    {
                        var gStream = new GZipInputStream(webResponse.GetResponseStream());
                        outdoc.Load(gStream);
                    }
                    else
                    {
                        var stream = webResponse.GetResponseStream();
                        StringBuilder sb = new StringBuilder();
                        using (StreamReader sr = new StreamReader(stream,Encoding.UTF8))
                        {
                            sb.Append(sr.ReadToEnd());
                        }
                        var bc = sb.ToString();
                        outdoc.Load(stream);
                        var b = "";
                    }
                }
                
            }
            catch (WebException httpEx)
            {
                //var xoiErrorCode = httpEx.Response.Headers.Get("X-XOI-ErrorCode");
                //Debug.WriteLine("No Data:" + link);
                //if (!string.IsNullOrEmpty(xoiErrorCode))
                //    return new XoiDocument(null, link) { StatusCode = Convert.ToInt32(xoiErrorCode) };
                //return new XoiDocument(null, link) { StatusCode = (int)httpEx.Status };
            }
            catch (Exception httpEx2)
            {
                //tryTime--;
                //if (tryTime <= 0)
                //    return new XoiDocument(null, link) { StatusCode = httpEx2.HResult };
                //return GetDetailXml(link, tryTime);
                //outdoc = null;
                //Tool.LogLog(DateTime.Today.ToString("yyyy-MM-dd"), "XOI Error: " + link + httpEx.Message, DateTime.Now);
            }
            
            return rtn;
        }
    }
}
