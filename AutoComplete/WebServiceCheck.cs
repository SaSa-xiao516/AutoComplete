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
        private string _hostName = "";
        private string _userName = "";
        private string _password = "";


        public WebServiceCheck(string GIDOrEXOI) {
            if (GIDOrEXOI.Trim().ToUpper().Equals("GID"))
            {
                _hostName = @"";
                _userName = "GlobalEquityData@morningstar.com";
                _password = "GXy1q88E";
            }
            else {
                _hostName = @"";
                _userName = "GlobalEquityData@morningstar.com";
                _password = "GXy1q88E";
            }
        }
        String lastestPrice = "";
        //data members
        private static string _detailcookieHeader = null;
        private DateTime _lastCookieTime;
        private DateTime _lastDetailCookieTime;
        private int _cookieExpirationTime = 30 * 2 * 2;
        private int _timeoutInterval = 60000 * 60 * 2;//1 min

        //private string _login = "GlobalEquityData@morningstar.com";
        //private string _password = "GXy1q88E";

        private string _loginGID = @"";
        private string _autoCompleteOutput = @"";

        private string _loginEXOIUrl = @"http://equitydata.morningstar.com/login/login.aspx?username=GlobalEquityData@morningstar.com&password=GXy1q88E";
        private string _check90 = @"http://equitydata.xoi.morningstar.com/DataOutput.aspx?";

        //private string _loginDll = @"http://funddata.xoi.morningstar.com/XOISuite/login.aspx?token=CGC}I6hm5[8OE%BT";
        //private string _accessDll = @"http://funddata.xoi.morningstar.com/XOIAccess/login.aspx?";
        //private string _loginDllBeta = @"http://xoibeta.morningstar.com/XOISuite/login.aspx?token=CGC}I6hm5[8OE%BT";
        //private string _accessDllBeta = @"http://xoibeta.morningstar.com/XOIAccess/login.aspx?";


        private string url;
        private bool _bUserAuthentication = true;

        #region 批量校验AutoComplete的Output {json-无需登录} 
        #endregion

        #region 返回GID的结果{Xml-需登录}
        #endregion

        #region 检查ShareClassOperation的LastTradingDate和现在是否超过90天-确认workday还是calendar {-需登录}
        #endregion

        public void GetURL(string package, string Content, string ID, string IdType)
        {

            StringBuilder URL = new StringBuilder(_check90);
            URL.Append("Package=EquityData");
            URL.Append("&Content=ShareClassInfo");
            //URL.Append(Content);
            URL.Append("&ID=");
            URL.Append(ID);
            URL.Append("&IdType=EquityShareClassId");
            //URL.Append(IdType
            url = URL.ToString();
        }

        public void BatchCheck(){
            HttpWebRequest webRequest;

            webRequest = (HttpWebRequest)WebRequest.Create(_autoCompleteOutput);
            webRequest.CookieContainer = new CookieContainer();
            webRequest.AllowAutoRedirect = false;

            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            string ch = webResponse.Headers.Get("Set-Cookie");

            if (ch != null)
            {
                _detailcookieHeader = webResponse.Headers.Get("Set-Cookie");
            }
        }

        public bool isOver90(string link)
        {
            bool rtn = false;
            GZipInputStream gStream = null;
            Stream stream = null;
            XmlDocument outdoc = new XmlDocument();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(link);
            try
            {
                // using basic authentication				 
                //Authenticate();
                webRequest = (HttpWebRequest)WebRequest.Create(link);
                webRequest.Headers.Set("Accept-Language", "zh-cn\r\n");
                webRequest.Headers.Set("Accept-Encoding", "gzip,deflate\r\n");
                webRequest.Timeout = _timeoutInterval;
                webRequest.KeepAlive = true;
                webRequest.CookieContainer = new CookieContainer();
                webRequest.CookieContainer.SetCookies(webRequest.RequestUri, _detailcookieHeader);

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();


                if (webResponse.StatusCode.Equals(HttpStatusCode.OK))
                {
                    string ch = webResponse.Headers.Get("Set-Cookie");
                    if (ch != null)
                    {
                        _detailcookieHeader = webResponse.Headers.Get("Set-Cookie");
                        _lastDetailCookieTime = DateTime.Now;
                    }

                    if (webResponse.ContentType.Contains("gzip") || webResponse.ContentEncoding.Equals("gzip"))
                    {
                        gStream = new GZipInputStream(webResponse.GetResponseStream());
                        outdoc.Load(gStream);
                    }
                    else
                    {
                        stream = webResponse.GetResponseStream();
                        outdoc.Load(stream);

                    }
                }
                else
                {
                    outdoc = null;
                    //Log.ErrorLog(DateTime.Today.ToString("yyyy-MM-dd"), "XOI Error: " + link + webResponse.StatusCode, DateTime.Now);
                }
                
                webResponse.Close();
                webRequest = null;
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
                //Log.ErrorLog(DateTime.Today.ToString("yyyy-MM-dd"), "XOI Error: " + link + httpEx.Message, DateTime.Now);
            }
            
            return rtn;
        }
    }
}
