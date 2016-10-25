using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Xml;
using System.Configuration;
using AutoComplete.Comm;
using AutoComplete.ACChange;
using System.Data.Common;
using System.Threading;
using System.Text.RegularExpressions;

namespace AutoComplete
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void compareFileByKey()
        {
            Dictionary<string, Dictionary<string, string>> FileA = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> FileB = new Dictionary<string, Dictionary<string, string>>();

            string strAutoCSV_New = string.Format(@"D:\workfile\AutoComplete\QA\10-22\ACNew\AutocompleteUniverse\29_index.csv");
            string strAutoCSV_Old = string.Format(@"D:\workfile\AutoComplete\QA\10-22\ACOld\AutocompleteUniverse\29_index.csv");

            string[] keyArray = new string[] { "rtSymbol", "rtExchangeId", "rtSecurityType" };
            string[] ignoreArray = new string[] { "RecordId", "MarketCapital", "AverageVolume", "NetAssets" };
            Dictionary<string, Dictionary<string, string>> AutoComplete_New = readFileByKey(strAutoCSV_New, keyArray, ignoreArray);
            Dictionary<string, Dictionary<string, string>> AutoComplete_Old = readFileByKey(strAutoCSV_Old, keyArray, ignoreArray);

            compareDictionaryByKey(AutoComplete_New, AutoComplete_Old, @"D:\workfile\AutoComplete\QA\10-22\1111111.txt");
            int a = 1;
        }

        #region 根据Key比对File并输出差异(输入的Key包括："rtSymbol", "Symbol", "rtExchangeId", "ExchangeId", "rtSecurityType", "SecurityType", "CountryId", "ShareClassId" )
        public void compareFileByKey(string filepath1, string filepath2,string resultfile)
        {
            //string[] keyArray = new string[] { "rtSymbol", "rtExchangeId", "rtSecurityType" };
            string[] keyArray = new string[] { "rtSymbol", "Symbol", "rtExchangeId", "ExchangeId", "rtSecurityType", "SecurityType", "CountryId", "ShareClassId" };
            string[] ignoreArray = new string[] { "RecordId", "MarketCapital", "AverageVolume", "NetAssets" };
            Dictionary<string, Dictionary<string, string>> AutoComplete_New = readFileByKey(filepath1, keyArray, ignoreArray);
            Dictionary<string, Dictionary<string, string>> AutoComplete_Old = readFileByKey(filepath2, keyArray, ignoreArray);

            compareDictionaryByKey(AutoComplete_New, AutoComplete_Old, resultfile);

        }
        #endregion

        #region 根据Key比对Dictionary并输出差异
        public Dictionary<string, Dictionary<string, string>> compareDictionaryByKey(Dictionary<string, Dictionary<string, string>> FileA,
            Dictionary<string, Dictionary<string, string>> FileB,string filename) {
            
            Dictionary<string, Dictionary<string, string>> rtn = new Dictionary<string, Dictionary<string, string>>();
            foreach (var raw in FileA) {
                if (FileB.ContainsKey(raw.Key))
                {
                    if (FileA[raw.Key].Equals(FileB[raw.Key]))
                    {
                        //Tool.LogFile(filename, "Success:" + raw.Key);
                    }
                    else {
                        bool isSecondSame = true;
                        string diff = string.Empty;
                        foreach (var raw2 in FileA[raw.Key]) {
                            if (FileA[raw.Key][raw2.Key].Equals(FileB[raw.Key][raw2.Key]))
                            {
                                isSecondSame = isSecondSame & true;
                            }
                            else {
                                isSecondSame = isSecondSame & false;
                                diff = diff + raw2.Key + "{FileA:[" + FileA[raw.Key][raw2.Key] + "], FileB:[" + FileB[raw.Key][raw2.Key] + "]};";
                            }
                        }
                        if(isSecondSame){
                             //Tool.LogFile(filename, "Success:" + raw.Key);
                        }else{
                            Tool.LogFile(filename, "Fail:" + diff);
                        }
                    }
                }
                else {
                    Tool.LogFile(filename, "仅仅在A中存在：" + raw.Key);
                }
            }
            foreach (var raw in FileB)
            {
                if (!FileA.ContainsKey(raw.Key)) {
                    Tool.LogFile(filename, "仅仅在B中存在：" + raw.Key);
                }
            }
            return rtn;
        }
        #endregion

        #region 根据Key读取文件至Dictionary
        public Dictionary<string, Dictionary<string, string>> readFileByKey(string filePath, string[] keyArray,string[] ignoreArray)
        {
            Dictionary<string, Dictionary<string, string>> rtn = new Dictionary<string, Dictionary<string, string>>();
            if (keyArray.Count() == 0)
            {
                return rtn;
            }
            string errorkey = "";
            try
            {
                StreamReader sr = new StreamReader(filePath, Encoding.Default);
                string[] template = sr.ReadLine().Split(','); 
                string str = string.Empty;
                while ((str = sr.ReadLine()) != null)
                {
                    Dictionary<string, string> subrtn = new Dictionary<string, string>();
                    string key = string.Empty;
                    string[] Data = Regex.Split(str, "\",\"", RegexOptions.IgnoreCase);   //正则 以","分割
                    if (Data.Count() != template.Count())//若csv中间包含逗号
                    {   
                        MessageBox.Show("文件异常");
                    }else{
                        Data[0] = Data[0].Substring(1,Data[0].Length-1);
                        Data[Data.Count()-1] = Data[Data.Count()-1].Substring(0, Data[Data.Count()-1].Length - 1);
                    }
                    
                    for (int i = 0; i < template.Count(); i++)
                    {
                        if (keyArray.Contains(template[i].Replace("\"", "")))
                        {
                            key = key + Data[i] + "|";
                        }
                        if (!ignoreArray.Contains(template[i].Replace("\"", "")))
                        {
                            subrtn.Add(template[i].Replace("\"", ""), Data[i]);
                        }
                    }
                    errorkey = key;
                    if (key == "") {
                        int a = 1;
                    }
                    rtn.Add(key, subrtn);
                }
            }
            catch (Exception e)
            {
                errorkey = errorkey+"1";
                throw ;
            }
            return rtn;
        }
        //if (key == "|29|XI|")
        //{
        //    int a = 1;
        //}
        //errorkey = key;
        #endregion

        #region 校验SecType变化
        //如果存在预期文件，则多一个RT和预期比较的步骤

        #endregion 


        #region  按照文件夹多线程比对(屏蔽global_fund_list.csv)
        public void MultiThreadCompare(string filepath1, string filepath2)
        {
            string resultfile = @"D:\workfile\AutoComplete\QA\10-22\22222.txt";
            List<string> filelist1 = new List<string>();
            List<string> filelist2 = new List<string>();
            filelist1 = FindFileByType(filepath1, "*.csv");
            filelist2 = FindFileByType(filepath2, "*.csv");
            foreach (var file in filelist1) {
                string temp = file.Substring(file.IndexOf(filepath1) + filepath1.Length, file.Length - filepath1.Length);
                if (filelist2.Contains(filepath2 + temp) && temp != "global_fund_list.csv")
                {
                    compareFileByKey(filepath1 + temp, filepath2 + temp, resultfile);
                }
            }
        }
        #endregion 

        #region 读取指定文件夹下的所有指定类型的文件
        public List<string> FindFileByType(string dir,string fileType)                         
        {
            //在指定目录及子目录下查找文件,在listBox1中列出子目录及文件 
            DirectoryInfo Dir = new DirectoryInfo(dir);
            List<string> rtn = new List<string>();
            try
            {
                foreach (DirectoryInfo d in Dir.GetDirectories())     //查找子目录   
                {
                    rtn.AddRange(FindFileByType(Dir + d.ToString() + "\\ ", fileType));
                }
                foreach (FileInfo f in Dir.GetFiles(fileType))             //查找文件 
                {
                    rtn.Add(Dir + f.ToString());
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return rtn;
        } 
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            //FindFileByType(@"D:\workfile\AutoComplete\QA\10-22\ACOld\","*.csv");
            MultiThreadCompare(@"D:\workfile\AutoComplete\QA\10-22\ACNew\AutocompleteUniverse\", 
@"D:\workfile\AutoComplete\QA\10-22\ACOld\AutocompleteUniverse\");
            //compareFileByKey();
            //Verify1022();
           // string steRealTimeNewPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\RealTime\2016-06-10\MS-EUR-OTHER-SYMBOL-AC-LIST.txt");
           // string strRealTimeOldPath = string.Format(@"D:\workfile\AutoComplete\QA\6-4\MS-CANADA-SYMBOL-AC-LIST.txt");

           // string strAutoCompleteNewPath = string.Format(@"D:\workfile\AutoComplete\QA\6-4\log\global_index_list - Copy.csv");
           // string strAutoCompleteOldPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-04\AutocompleteUniverse\199.csv");

           // //string Title = "";
           // //string filePath1 = @"D:\workfile\Atlas\Alibaba_ETLTestFramwork_php\dumper.php";
           // //Title = filePath1.Substring(filePath1.LastIndexOf("\\")+1, filePath1.Length - filePath1.LastIndexOf("\\")-1);
           // //MessageBox.Show(Title);
           // //SecurityChangeTest();
           //// MessageBox.Show("Test Done\r\nPlease see log in D:\\workfile\\AutoComplete\\QA\\6-4\\log\\log.txt");
           // CompareDataTable(@"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-11-2\global_index_list.csv", @"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-04\AutocompleteUniverse-New\AutocompleteUniverse\global_index_list.csv", "AutoComplete", true);
           // //CompareDataTable(steRealTimeNewPath, strAutoCompleteOldPath, "AutoComplete", false);
        }

        public static void Verify1022() {
            string Success = string.Format(@"D:\workfile\AutoComplete\QA\10-22\Success.txt");
            string Failure = string.Format(@"D:\workfile\AutoComplete\QA\10-22\Failure.txt");

            //检查RT和AC取得的文件是否为最新，在AC处理完RT之后
            //223
            string strAutoCSV_New = string.Format(@"D:\workfile\AutoComplete\QA\10-22\ACNew\AutocompleteUniverse\223.csv");
            //223-index
            string strAutoCSV_Index_New = string.Format(@"D:\workfile\AutoComplete\QA\10-22\ACNew\AutocompleteUniverse\223_index.csv");
            //不用这个
            string strAutoCSV_Old = string.Format(@"D:\workfile\AutoComplete\QA\10-22\ACOld\AutocompleteUniverse\223_index.csv");
            string strRealTime_New = string.Format(@"D:\workfile\AutoComplete\QA\10-22\RT\2\MS-EUR-OTHER-SYMBOL-AC-LIST.txt");
            string strExpectedFile = string.Format(@"D:\workfile\AutoComplete\QA\10-22\Borsa Italiana Instrument Mapping.csv");

            Dictionary<string,Dictionary<string,string>> AutoComplete_New = new Dictionary<string,Dictionary<string,string>>();
            Dictionary<string, Dictionary<string, string>> AutoComplete_Index_New = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> AutoComplete_Old = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> RealTime_New = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> Expected = new Dictionary<string, Dictionary<string, string>>();
            
            AutoComplete_New = ReturnDicFromACCsv(strAutoCSV_New);
            AutoComplete_Index_New = ReturnDicFromACCsv(strAutoCSV_Index_New);
            AutoComplete_Old = ReturnDicFromACCsv(strAutoCSV_Old);
            RealTime_New = ReturnDicFromRTTxt(strRealTime_New);
            Expected = ReturnDicFromExpectedFile(strExpectedFile);


            #region 测试223_index
            foreach (var raw in AutoComplete_Index_New)
            {
                if (RealTime_New.ContainsKey(raw.Key))
                {
                    if (AutoComplete_Index_New[raw.Key]["SecurityType"].Equals(RealTime_New[raw.Key]["SecurityType"]))
                    {
                        Tool.LogFile(Success, raw.Key);
                    }
                    else
                    {
                        Tool.LogFile(Failure, raw.Key);
                    }
                }
                else
                {
                    Tool.LogFile(Failure, raw.Key);
                }

            }
            #endregion

            #region 测试223
            foreach (var raw in AutoComplete_New)
            {
                if (RealTime_New.ContainsKey(raw.Key))
                {
                    if (AutoComplete_New[raw.Key]["SecurityType"].Equals(RealTime_New[raw.Key]["SecurityType"]))
                    {
                        Tool.LogFile(Success, raw.Key);
                    }
                    else
                    {
                        Tool.LogFile(Failure, raw.Key);
                    }
                }
                else
                {
                    Tool.LogFile(Failure, raw.Key);
                }

            }
            #endregion

            #region 验证RT的变化符合他们的预期 
            foreach (var raw in Expected)
            {
                if (RealTime_New.ContainsKey(raw.Key))
                {
                    if (Expected[raw.Key]["NewsecType"].Equals(RealTime_New[raw.Key]["SecurityType"]))
                    {
                        Tool.LogFile(Success, raw.Key);
                    }
                    else
                    {
                        Tool.LogFile(Failure, raw.Key);
                    }
                }
                else
                {
                    Tool.LogFile(Failure, raw.Key);
                }

            }
            #endregion

            #region 测试700+变化的有多少在AC输出文件里面
            foreach (var raw in Expected)
            {
                if (AutoComplete_New.ContainsKey(raw.Key))
                {
                    if (Expected[raw.Key]["NewsecType"].Equals(AutoComplete_New[raw.Key]["SecurityType"]))
                    {
                        Tool.LogFile(Success, raw.Key);
                    }
                    else
                    {
                        Tool.LogFile(Failure, raw.Key);
                    }
                }
                else
                {
                    Tool.LogFile(Failure, raw.Key);
                }

            }
            #endregion

            //try
            //{
            //    foreach (var raw in AutoComplete_New)
            //    {
            //        if (Expected.ContainsKey(raw.Key))
            //        {
            //            if (AutoComplete_New[raw.Key]["SecurityType"].Equals(RealTime_New[raw.Key]["SecurityType"]) &&
            //                AutoComplete_New[raw.Key]["SecurityType"].Equals(Expected[raw.Key]["NewsecType"]))
            //            {
            //                Tool.LogFile(Success, raw.Key);
            //            }
            //            else
            //            {
            //                //此处有差异则Excel与RT最终给出的不一致，则以RT为准
            //                Tool.LogFile(Failure, raw.Key + "|AutoComplete_SecurityType：" + AutoComplete_New[raw.Key]["SecurityType"] +
            //                "|RealTime_New_SecurityType:" + RealTime_New[raw.Key]["SecurityType"] +
            //                "|Expected_SecurityType:" + Expected[raw.Key]["NewsecType"]);
            //            }
            //        }
            //        else
            //        {
            //            //不期望改动的，则预期与RT保持一致
            //            if(1==1){}
            //            if (AutoComplete_New[raw.Key]["SecurityType"].Equals(RealTime_New[raw.Key]["SecurityType"]))
            //            {
            //                Tool.LogFile(Success, raw.Key);
            //            }
            //            else
            //            {
            //                Tool.LogFile(Failure, raw.Key + "|AutoComplete_SecurityType：" + AutoComplete_New[raw.Key]["SecurityType"] +
            //                "|RealTime_New_SecurityType:" + RealTime_New[raw.Key]["SecurityType"]);
            //            }
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
                
            //    throw;
            //}

        }
        #region 读取RealTime文件至Dictionary
        public static Dictionary<string, Dictionary<string, string>> ReturnDicFromRTTxt(string strPath)
        {
            Dictionary<string, Dictionary<string, string>> rtn = new Dictionary<string, Dictionary<string, string>>();
            string error = "";
            try
            {
                StreamReader sr = new StreamReader(strPath, Encoding.Default);
                string[] template = sr.ReadLine().Split('|');

                string str = string.Empty;
                while ((str = sr.ReadLine()) != null)
                {
                    Dictionary<string, string> subrtn = new Dictionary<string, string>();
                    string Symbol = string.Empty;
                    string Exchange = string.Empty;
                    string SecurityType = string.Empty;
                    string Other = string.Empty;
                    string[] Data = str.Split('|');
                    Symbol = Data[0].Replace("\"","");
                    if (Symbol == "EXCHANGEINFO")
                    {
                        continue;
                    }
                    Exchange = Data[1];
                    if (Exchange!="223") {
                        continue;
                    }
                    SecurityType = Data[2];
                    for (int i = 0; i < Data.Count(); i++)
                    {
                        if (i != 0 && i != 1 && i != 2)
                        {
                            Other = Other + Data[i];
                        }
                    }
                    
                    try
                    {
                        subrtn.Add("Symbol", Symbol);
                        //if (Symbol == "CGB2/EU") {
                        //    int a = 1;
                        //}
                        subrtn.Add("Exchange", Exchange);
                        subrtn.Add("SecurityType", SecurityType);
                        subrtn.Add("Other", Other);
                        rtn.Add(Symbol, subrtn);
                        error = Symbol;
                    }
                    catch (Exception e)
                    {
                        
                        throw;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(error+"|"+e.ToString());
                
                throw;
            }
            return rtn;
        }
        #endregion

        #region 读取RealTime文件至DataTable
        public static DataTable ReturnDataTableFromRTTxt(string strPath)
        {
            int isIni = 0;
            string str = string.Empty;
            DataTable _dt = GeneratorRealTimeSimpaleTable();

            StreamReader sr = new StreamReader(strPath, Encoding.Default);
            while ((str = sr.ReadLine()) != null)
            {
                if (isIni == 0)
                {
                    isIni++;
                }
                else {
                    string[] s = str.Split('|');
                    DataRow dr = _dt.NewRow();
                    dr["Symbol"] = s[0];
                    dr["Exchange"] = s[1];
                    dr["SecurityType"] = s[2];
                    _dt.Rows.Add(dr);
                }
                
            }
            sr.Close();
            return _dt;
        }
        #endregion

        #region 读取AutoComplete文件至Dictionary
        public static Dictionary<string, Dictionary<string, string>> ReturnDicFromACCsv(string strPath)
        {
            Dictionary<string, Dictionary<string, string>> rtn = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                StreamReader sr = new StreamReader(strPath, Encoding.Default);
                string[] template = sr.ReadLine().Split(',');

                string str = string.Empty;
                while ((str = sr.ReadLine()) != null)
                {
                    Dictionary<string, string> subrtn = new Dictionary<string, string>();
                    string Symbol = string.Empty;
                    string Exchange = string.Empty;
                    string SecurityType = string.Empty;
                    string Other = string.Empty;
                    string[] Data = str.Split(',');
                    Symbol = Data[7].Replace("\"","");
                    Exchange = Data[6].Replace("\"","");
                    SecurityType = Data[9].Replace("\"", "");

                    for (int i = 0; i < Data.Count(); i++)
                    {
                        if (i != 6 && i != 7 && i != 9)
                        {
                            Other = Other + Data[i];
                        }
                    }
                    subrtn.Add("Symbol", Symbol);
                    subrtn.Add("Exchange", Exchange);
                    subrtn.Add("SecurityType", SecurityType);
                    subrtn.Add("Other", Other);
                    rtn.Add(Symbol, subrtn);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return rtn;
        }
        #endregion

        #region 读取AutoComplete文件至DataTable

        public static DataTable ReturnDataTableFromACCsv(string strPath)
        {
            int isIni = 0;
            string str = string.Empty;
            DataTable _dt = GeneratorRealTimeSimpaleTable();

            StreamReader sr = new StreamReader(strPath, Encoding.Default);
            while ((str = sr.ReadLine()) != null)
            {
                if (isIni == 0)
                {                    
                    isIni++;                
                }
                else {
                    string[] s = str.Split(',');
                    DataRow dr = _dt.NewRow();
                    dr["Symbol"] = s[7].Substring(1, s[7].Length - 2);
                    dr["Exchange"] = s[6].Substring(1, s[6].Length - 2);
                    dr["SecurityType"] = s[9].Substring(1, s[9].Length - 2);
                    _dt.Rows.Add(dr);
                }                
            }
            sr.Close();
            return _dt;
        }

        #endregion

        #region 读取ExpectedFile至Dictionary
        public static Dictionary<string, Dictionary<string, string>> ReturnDicFromExpectedFile(string strPath) {
            Dictionary<string, Dictionary<string, string>> rtn = new Dictionary<string, Dictionary<string, string>>();
            try
            {
                StreamReader sr = new StreamReader(strPath, Encoding.Default);
                string[] template = sr.ReadLine().Split(',');

                string str = string.Empty;
                while ((str = sr.ReadLine()) != null)
                {
                    Dictionary<string, string> subrtn = new Dictionary<string, string>();

                    string Symbol = string.Empty;
                    string NewsecType = string.Empty;
                    string OldsecType = string.Empty;
                    string Other = string.Empty;
                    string[] Data = str.Split(',');
                    Symbol = Data[3].Replace("\"","");;
                    NewsecType = Data[2];
                    OldsecType = Data[1];

                    subrtn.Add("Symbol", Symbol);
                    subrtn.Add("NewsecType", NewsecType);
                    subrtn.Add("OldsecType", OldsecType);
                    //subrtn.Add("Other", Other);
                    rtn.Add(Symbol, subrtn);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return rtn;
        }
        #endregion

        public static DataTable GeneratorRealTimeSimpaleTable()
        {
            DataTable _dt = new DataTable();
            _dt.Columns.Add("Symbol", typeof(string));
            _dt.Columns.Add("Exchange", typeof(string));
            _dt.Columns.Add("SecurityType", typeof(string));

            return _dt;
        }

        public static void SecurityChangeTest()
        {

            //先比对old和New 的 Rt给的文件的变化
            //1 除开SecurityType变化，其他的数据不应该发生变化
            //2对应的ExchangeID 的数据的SecurityTypeChange 符合预期，check RealTime的Data 
            //比对AutoComplete 和 RealTime的文件类容一致
            //1根据处理前获取RealTime本次所有的change，以此为源头A
            //2获取本次的change的配置，包括，哪个文件，哪个exchangeID 的 type发生变化
            //3根据2中的变化，验证auto产生的文件中的对应数据发生变化符合预期
            //4记录发生变化的源B
            //5比对A 和 B
            //6 若 A>B , realtime 是否有symbol的arrange发生了变化
            //7 若 B>A , 是否AutoComplete的问题

            string steRealTimeNewPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\RealTime\2016-06-11\MS-EUR-OTHER-SYMBOL-AC-LIST.txt");
            string strRealTimeOldPath = string.Format(@"D:\workfile\AutoComplete\QA\6-4\MS-CANADA-SYMBOL-AC-LIST.txt");

            string strAutoCompleteNewPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-11\199.csv");
            string strAutoCompleteOldPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-03\AutocompleteUniverse\127.csv");

            DataTable _dtNewRT = ReturnDataTableFromRTTxt(steRealTimeNewPath);
            DataTable _dtOldRT = ReturnDataTableFromRTTxt(strRealTimeOldPath);

            DataTable _dtNewAC = ReturnDataTableFromACCsv(strAutoCompleteNewPath);
            DataTable _dtOldAC = ReturnDataTableFromACCsv(strAutoCompleteOldPath);

            DataTable _dtDiffNew2Old = new DataTable();
            DataTable _dtDiffOld2New = new DataTable();
            DataTable _dtDiffRT2AC = new DataTable();
            DataTable _dtDiffAC2RT = new DataTable();

            //变化SecurityType的个数
            List<string> _detailChange = new List<string>();
            
            //#region  检查AC和RT的数据一致

            #region  检查AutoComplete文件与RealTime的差异 _dtNewAC - _dtNewRT
            IEnumerable<DataRow> queryAC2RT = _dtNewAC.AsEnumerable().Except(_dtNewRT.AsEnumerable(), DataRowComparer.Default);
            if (queryAC2RT.Count() > 0)
            {
                _dtDiffAC2RT = queryAC2RT.CopyToDataTable();
                Tool.LogFile("AC比RT的差异:个数[" + queryAC2RT.Count() + "]");
                _dtDiffAC2RT.AsEnumerable().ToList().ForEach(
                    x =>
                    {
                        Tool.LogFile("[RT2AC]" + x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
                    });
            }
            else
            {
                Tool.LogFile("[Release]AC比RT之间无文件差异");
            }
            #endregion

            #region  检查RealTime文件与AutoComplete的差异 _dtNewRT - _dtNewAC
            IEnumerable<DataRow> queryRT2AC = _dtNewRT.AsEnumerable().Except(_dtNewAC.AsEnumerable(), DataRowComparer.Default);
            if (queryRT2AC.Count() > 0)
            {
                _dtDiffRT2AC = queryRT2AC.CopyToDataTable();
                Tool.LogFile("RT比AC的差异:个数[" + queryRT2AC.Count() + "]");
                _dtDiffRT2AC.AsEnumerable().ToList().ForEach(
                    x =>
                    {
                        Tool.LogFile("[AC2RT]" + x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
                    });
            }
            else
            {
                Tool.LogFile("[Release]RT比AC之间无文件差异");
            }
            #endregion

            #region
            #endregion

            #region
            #endregion
            

            
            MessageBox.Show("Test1 完成");

            //#region  输出变化SecurityType的个数
            //List<SecurityTypeChange> securityTypeChange = new List<SecurityTypeChange>();
            //if (_dtDiffAC2RT.Rows.Count != 0)
            //{
            //    _dtDiffAC2RT.AsEnumerable().ToList().ForEach(
            //    x =>
            //    {
            //        DataRow[] matches = _dtDiffRT2AC.Select("Symbol = '" + x["Symbol"].ToString() + "' and Exchange='" + x["Exchange"].ToString() + "'");
            //        foreach (var datarow in matches)
            //        {
            //            //SecurityTypeChange stc = new SecurityTypeChange();
            //            //stc.Symbol = datarow["Symbol"].ToString();
            //            //stc.Exchange = datarow["Exchange"].ToString();
            //            //stc.OldSecurityType = ;
            //            //stc.NewSecurityType = ;
            //            _detailChange.Add(datarow["Symbol"].ToString());

            //        }
            //    });

            //    Tool.LogFile("[SecurityType Changed Number] SecurityType有变化的数据" + _detailChange.Count.ToString());
            //}
            //else
            //{
            //    Tool.LogFile("[Not SecurityType Changed]");
            //}
            //#endregion


            #region 检查RealTime的数据问题及前后差异

            //Data Numbercheck
            //if (_dtNewAC.Rows.Count != _dtOldAC.Rows.Count)
            //{
            //    Tool.LogFile("[RealTime DataIssue] New Data Number:[" + _dtNewRT.Rows.Count + "] Old Data Number:[" + _dtOldRT.Rows.Count + "]");
            //}

            //new - old
            //IEnumerable<DataRow> query1 = _dtNewAC.AsEnumerable().Except(_dtOldAC.AsEnumerable(), DataRowComparer.Default);
            //if (query1.Count() != 0)
            //{
            //    _dtDiffNew2Old = query1.CopyToDataTable();

            //    Tool.LogFile("[RealTime]中新文件与旧文件有差异的" + query1.Count());
            //    _dtDiffNew2Old.AsEnumerable().ToList().ForEach(
            //        x =>
            //        {
            //            Tool.LogFile(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
            //        });
            //}
            //else
            //{
            //    Tool.LogFile("[RealTime]中新文件数据在旧文件中存在且一致");
            //}

            //Old - New
            //IEnumerable<DataRow> query2 = _dtOldAC.AsEnumerable().Except(_dtNewAC.AsEnumerable(), DataRowComparer.Default);
            //if (query2.Count() != 0)
            //{
            //    _dtDiffOld2New = query2.CopyToDataTable();

            //    Tool.LogFile("[RealTime]中旧文件与新文件有差异的" + query2.Count());
            //    _dtDiffOld2New.AsEnumerable().ToList().ForEach(
            //        x =>
            //        {
            //            Tool.LogFile(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
            //        });
            //}
            //else
            //{
            //    Tool.LogFile("[RealTime]中旧文件数据在新文件中存在且一致");
            //}
            

            #endregion

            //#region  输出变化SecurityType的个数
            //if (_dtDiffNew2Old.Rows.Count != 0)
            //{
            //    _dtDiffNew2Old.AsEnumerable().ToList().ForEach(
            //    x =>
            //    {
            //        DataRow[] matches = _dtDiffOld2New.Select("Symbol = '" + x["Symbol"].ToString() + "' and Exchange='" + x["Exchange"].ToString() + "'");
            //        foreach (var datarow in matches)
            //        {
            //            _detailChange.Add(datarow["Symbol"].ToString());

            //        }
            //    });

            //    Tool.LogFile("[SecurityType Changed Number] SecurityType有变化的数据" + _detailChange.Count.ToString());
            //}
            //else {
            //    Tool.LogFile("[Not SecurityType Changed]");
            //}
            //#endregion

            #region  根据变化SecurityType的数据进行检查
            #endregion

            #region  检查AutoComplete文件的变化
            #endregion

            #region  检查AutoComplete的 SecurityType的数据变化个数是否和 RealTime变化个数是否一致
            #endregion

            #region  检查没有出现在AutoComplete的内容，是否是因为90天
            #endregion
            

            #region
            #endregion

            #region
            #endregion

            #region
            #endregion

            #region
            #endregion

            //Console.WriteLine("--------------------原来有重复数据的Table----------------------");
            //_dtNew.AsEnumerable().ToList().ForEach(
            //    x =>
            //    {
            //        Console.WriteLine(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
            //    });


            //Console.WriteLine();

            //Console.WriteLine("--------------------用Linq去重复后的Table----------------------");

            //var _comPresult = _dtNew.AsEnumerable().Distinct(new DataTableRowCompare());
            //DataTable _resultDt = _comPresult.CopyToDataTable();

            //_resultDt.AsEnumerable().ToList().ForEach(
            //   x =>
            //   {
            //       Console.WriteLine(x["id"].ToString() + "    " + x["name"].ToString() + "   " + x["address"].ToString());
            //   });

            //Console.WriteLine();

            //Console.WriteLine("--------------------用DefaultView去重复后的Table----------------------");
            //DataTable _dtDefalut = _dtNew.DefaultView.ToTable(true, "id", "name", "address");


            //_dtDefalut.AsEnumerable().ToList().ForEach(
            //  x =>
            //  {
            //      Console.WriteLine(x["id"].ToString() + "    " + x["name"].ToString() + "   " + x["address"].ToString());
            //  });

            Console.ReadLine();
        }

        public static void AutoCompleteFileCheck()
        {

            //先比对old和New 的 Rt给的文件的变化
            //1 除开SecurityType变化，其他的数据不应该发生变化
            //2对应的ExchangeID 的数据的SecurityTypeChange 符合预期，check RealTime的Data 
            //比对AutoComplete 和 RealTime的文件类容一致
            //1根据处理前获取RealTime本次所有的change，以此为源头A
            //2获取本次的change的配置，包括，哪个文件，哪个exchangeID 的 type发生变化
            //3根据2中的变化，验证auto产生的文件中的对应数据发生变化符合预期
            //4记录发生变化的源B
            //5比对A 和 B
            //6 若 A>B , realtime 是否有symbol的arrange发生了变化
            //7 若 B>A , 是否AutoComplete的问题

            string steRealTimeNewPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\RealTime\2016-06-11\MS-EUR-OTHER-SYMBOL-AC-LIST.txt");
            string strRealTimeOldPath = string.Format(@"D:\workfile\AutoComplete\QA\6-4\MS-CANADA-SYMBOL-AC-LIST.txt");

            string strAutoCompleteNewPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-11\199.csv");
            string strAutoCompleteOldPath = string.Format(@"D:\workfile\AutoComplete\HistoricalData\AutoComplete\2016-06-03\AutocompleteUniverse\127.csv");

            DataTable _dtNewRT = ReturnDataTableFromRTTxt(steRealTimeNewPath);
            DataTable _dtOldRT = ReturnDataTableFromRTTxt(strRealTimeOldPath);

            DataTable _dtNewAC = ReturnDataTableFromACCsv(strAutoCompleteNewPath);
            DataTable _dtOldAC = ReturnDataTableFromACCsv(strAutoCompleteOldPath);

            DataTable _dtDiffNew2Old = new DataTable();
            DataTable _dtDiffOld2New = new DataTable();
            DataTable _dtDiffRT2AC = new DataTable();
            DataTable _dtDiffAC2RT = new DataTable();

            //变化SecurityType的个数
            List<string> _detailChange = new List<string>();

            //#region  检查AC和RT的数据一致

            #region  检查AutoComplete文件与RealTime的差异 _dtNewAC - _dtNewRT
            IEnumerable<DataRow> queryAC2RT = _dtNewAC.AsEnumerable().Except(_dtNewRT.AsEnumerable(), DataRowComparer.Default);
            if (queryAC2RT.Count() > 0)
            {
                _dtDiffAC2RT = queryAC2RT.CopyToDataTable();
                Tool.LogFile("AC比RT的差异:个数[" + queryAC2RT.Count() + "]");
                _dtDiffAC2RT.AsEnumerable().ToList().ForEach(
                    x =>
                    {
                        Tool.LogFile("[RT2AC]" + x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
                    });
            }
            else
            {
                Tool.LogFile("[Release]AC比RT之间无文件差异");
            }
            #endregion

            #region  检查RealTime文件与AutoComplete的差异 _dtNewRT - _dtNewAC
            IEnumerable<DataRow> queryRT2AC = _dtNewRT.AsEnumerable().Except(_dtNewAC.AsEnumerable(), DataRowComparer.Default);
            if (queryRT2AC.Count() > 0)
            {
                _dtDiffRT2AC = queryRT2AC.CopyToDataTable();
                Tool.LogFile("RT比AC的差异:个数[" + queryRT2AC.Count() + "]");
                _dtDiffRT2AC.AsEnumerable().ToList().ForEach(
                    x =>
                    {
                        Tool.LogFile("[AC2RT]" + x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
                    });
            }
            else
            {
                Tool.LogFile("[Release]RT比AC之间无文件差异");
            }
            #endregion

            #region
            #endregion

            #region
            #endregion



            MessageBox.Show("Test1 完成");

            //#region  输出变化SecurityType的个数
            //List<SecurityTypeChange> securityTypeChange = new List<SecurityTypeChange>();
            //if (_dtDiffAC2RT.Rows.Count != 0)
            //{
            //    _dtDiffAC2RT.AsEnumerable().ToList().ForEach(
            //    x =>
            //    {
            //        DataRow[] matches = _dtDiffRT2AC.Select("Symbol = '" + x["Symbol"].ToString() + "' and Exchange='" + x["Exchange"].ToString() + "'");
            //        foreach (var datarow in matches)
            //        {
            //            //SecurityTypeChange stc = new SecurityTypeChange();
            //            //stc.Symbol = datarow["Symbol"].ToString();
            //            //stc.Exchange = datarow["Exchange"].ToString();
            //            //stc.OldSecurityType = ;
            //            //stc.NewSecurityType = ;
            //            _detailChange.Add(datarow["Symbol"].ToString());

            //        }
            //    });

            //    Tool.LogFile("[SecurityType Changed Number] SecurityType有变化的数据" + _detailChange.Count.ToString());
            //}
            //else
            //{
            //    Tool.LogFile("[Not SecurityType Changed]");
            //}
            //#endregion


            #region 检查RealTime的数据问题及前后差异

            //Data Numbercheck
            //if (_dtNewAC.Rows.Count != _dtOldAC.Rows.Count)
            //{
            //    Tool.LogFile("[RealTime DataIssue] New Data Number:[" + _dtNewRT.Rows.Count + "] Old Data Number:[" + _dtOldRT.Rows.Count + "]");
            //}

            //new - old
            //IEnumerable<DataRow> query1 = _dtNewAC.AsEnumerable().Except(_dtOldAC.AsEnumerable(), DataRowComparer.Default);
            //if (query1.Count() != 0)
            //{
            //    _dtDiffNew2Old = query1.CopyToDataTable();

            //    Tool.LogFile("[RealTime]中新文件与旧文件有差异的" + query1.Count());
            //    _dtDiffNew2Old.AsEnumerable().ToList().ForEach(
            //        x =>
            //        {
            //            Tool.LogFile(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
            //        });
            //}
            //else
            //{
            //    Tool.LogFile("[RealTime]中新文件数据在旧文件中存在且一致");
            //}

            //Old - New
            //IEnumerable<DataRow> query2 = _dtOldAC.AsEnumerable().Except(_dtNewAC.AsEnumerable(), DataRowComparer.Default);
            //if (query2.Count() != 0)
            //{
            //    _dtDiffOld2New = query2.CopyToDataTable();

            //    Tool.LogFile("[RealTime]中旧文件与新文件有差异的" + query2.Count());
            //    _dtDiffOld2New.AsEnumerable().ToList().ForEach(
            //        x =>
            //        {
            //            Tool.LogFile(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
            //        });
            //}
            //else
            //{
            //    Tool.LogFile("[RealTime]中旧文件数据在新文件中存在且一致");
            //}


            #endregion

            //#region  输出变化SecurityType的个数
            //if (_dtDiffNew2Old.Rows.Count != 0)
            //{
            //    _dtDiffNew2Old.AsEnumerable().ToList().ForEach(
            //    x =>
            //    {
            //        DataRow[] matches = _dtDiffOld2New.Select("Symbol = '" + x["Symbol"].ToString() + "' and Exchange='" + x["Exchange"].ToString() + "'");
            //        foreach (var datarow in matches)
            //        {
            //            _detailChange.Add(datarow["Symbol"].ToString());

            //        }
            //    });

            //    Tool.LogFile("[SecurityType Changed Number] SecurityType有变化的数据" + _detailChange.Count.ToString());
            //}
            //else {
            //    Tool.LogFile("[Not SecurityType Changed]");
            //}
            //#endregion

            #region  根据变化SecurityType的数据进行检查
            #endregion

            #region  检查AutoComplete文件的变化
            #endregion

            #region  检查AutoComplete的 SecurityType的数据变化个数是否和 RealTime变化个数是否一致
            #endregion

            #region  检查没有出现在AutoComplete的内容，是否是因为90天
            #endregion


            #region
            #endregion

            #region
            #endregion

            #region
            #endregion

            #region
            #endregion

            //Console.WriteLine("--------------------原来有重复数据的Table----------------------");
            //_dtNew.AsEnumerable().ToList().ForEach(
            //    x =>
            //    {
            //        Console.WriteLine(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
            //    });


            //Console.WriteLine();

            //Console.WriteLine("--------------------用Linq去重复后的Table----------------------");

            //var _comPresult = _dtNew.AsEnumerable().Distinct(new DataTableRowCompare());
            //DataTable _resultDt = _comPresult.CopyToDataTable();

            //_resultDt.AsEnumerable().ToList().ForEach(
            //   x =>
            //   {
            //       Console.WriteLine(x["id"].ToString() + "    " + x["name"].ToString() + "   " + x["address"].ToString());
            //   });

            //Console.WriteLine();

            //Console.WriteLine("--------------------用DefaultView去重复后的Table----------------------");
            //DataTable _dtDefalut = _dtNew.DefaultView.ToTable(true, "id", "name", "address");


            //_dtDefalut.AsEnumerable().ToList().ForEach(
            //  x =>
            //  {
            //      Console.WriteLine(x["id"].ToString() + "    " + x["name"].ToString() + "   " + x["address"].ToString());
            //  });

            Console.ReadLine();
        }
        
        
        public static void SymbolChangeTest() {
            string OldRealTimeFile = "";
            string NewRealTimeFile = "";
            string NewAutoComplete = "";
            string OldAutoComplete = "";

            //RealTime的变化只有Symbol变化
            //变化个数为N，检查AutoComplete变化格式M，预期N=M
            //新AutoComplete的和RealTime的一致
            //输出变化的AutoComplete的对应项
        }

        public static void CompareDataTable(string filePath1, string filePath2, string Title, bool isSameFile )
        {
            #region log initiate
            string fileName1 = filePath1.Substring(filePath1.LastIndexOf("\\")+1, filePath1.Length - filePath1.LastIndexOf("\\")-1);
            string fileName2 = filePath2.Substring(filePath2.LastIndexOf("\\") + 1, filePath2.Length - filePath2.LastIndexOf("\\")-1);
            string diffMessageOld2New = "";
            string diffMessageNew2Old = "";
            string isNoDifference = fileName1 + "文件数据在文件" + fileName2 + "中存在且一致";

            if (isSameFile)
            {
                diffMessageOld2New = "[" + Title + "]中旧文件与新文件有差异的:[";
                diffMessageNew2Old = "[" + Title + "]中新文件与旧文件有差异的:[";
            }
            else
            {
                diffMessageOld2New = fileName2 + "文件比" + fileName1 + "文件的差异：[";
                diffMessageNew2Old = fileName1 + "文件比" + fileName2 + "文件的差异：[";
            }
            #endregion


            #region  文件数量的检查
            DataTable _dtNew = new DataTable();
            DataTable _dtOld = new DataTable();
            if (filePath1.Substring(filePath1.LastIndexOf(".") + 1, filePath1.Length - filePath1.LastIndexOf(".") - 1).Equals("txt"))
            {
                _dtNew = ReturnDataTableFromRTTxt(filePath1);
            }else{
                _dtNew = ReturnDataTableFromACCsv(filePath1);
            }

            if (filePath2.Substring(filePath2.LastIndexOf(".") + 1, filePath2.Length - filePath2.LastIndexOf(".") - 1).Equals("txt"))
            {
                _dtOld = ReturnDataTableFromRTTxt(filePath2);
            }else{
                _dtOld = ReturnDataTableFromACCsv(filePath2);
            }

            Tool.LogFile("[文件个数]" + filePath1 + "[" + _dtNew.Rows.Count + "]");
            Tool.LogFile("[文件个数]" + filePath2 + "[" + _dtOld.Rows.Count + "]");
            DataTable _dtMergeO = GeneratorRealTimeSimpaleTable();

            //合并两个DataTable 并去重
            //拷贝DataTable的结构和数据
            DataTable _dtAll = _dtNew.Copy();
            //添加DataTable2的数据
            foreach (DataRow dr in _dtOld.Rows)
            {
                _dtAll.ImportRow(dr);
            }
            string[] filterString = { "Symbol", "Exchange", "SecurityType" };
            DataView _dv = new DataView(_dtAll);
            _dtMergeO = _dv.ToTable(true, filterString);
            MessageBox.Show(_dtMergeO.Rows.Count.ToString());
            Tool.LogFile("[合并去重后数据][" + _dtMergeO.Rows.Count.ToString() + "]");
            Tool.LogFile("[两个文件中相同数据条数][" + (_dtNew.Rows.Count + _dtOld.Rows.Count - _dtMergeO.Rows.Count) + "]");

            #endregion


            #region 差异检查

            DataTable _dtDiffNew2Old = new DataTable();
            DataTable _dtDiffOld2New = new DataTable();
            IEnumerable<DataRow> query1 = _dtOld.AsEnumerable().Except(_dtNew.AsEnumerable(), DataRowComparer.Default);
            if (query1.Count() != 0)
            {
                _dtDiffNew2Old = query1.CopyToDataTable();

                Tool.LogFile(diffMessageOld2New + query1.Count() + "]");
                //_dtDiffNew2Old.AsEnumerable().ToList().ForEach(
                //    x =>
                //    {
                //        Tool.LogFile(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
                //    });
            }
            else
            {
                Tool.LogFile(isNoDifference);
            }

            IEnumerable<DataRow> query2 = _dtNew.AsEnumerable().Except(_dtOld.AsEnumerable(), DataRowComparer.Default);
            if (query2.Count() != 0)
            {
                _dtDiffOld2New = query2.CopyToDataTable();

                Tool.LogFile(diffMessageNew2Old + query2.Count() + "]");
                //_dtDiffOld2New.AsEnumerable().ToList().ForEach(
                //    x =>
                //    {
                //        Tool.LogFile(x["Symbol"].ToString() + "    " + x["Exchange"].ToString() + "   " + x["SecurityType"].ToString());
                //    });
            }
            else
            {
                Tool.LogFile(isNoDifference);
            }

            if (query1.Count() != 0 && query2.Count() != 0)
            {
                DifferenceInfoComparer datacom = new DifferenceInfoComparer();
                List<DifferenceInfo> ResultList = new List<DifferenceInfo>();
                List<DifferenceInfo> leftResultList = new List<DifferenceInfo>();
                List<DifferenceInfo> rightResultList = new List<DifferenceInfo>();
                var leftOuterJoin =
                from newdiff in _dtDiffNew2Old.AsEnumerable()
                join olddiff in _dtDiffOld2New.AsEnumerable()
                    on newdiff.Field<string>("Symbol") equals
                       olddiff.Field<string>("Symbol") into Grp
                from grp in Grp.DefaultIfEmpty()
                select new DifferenceInfo()
                {
                    NewSymbol = newdiff.Field<string>("Symbol"),
                    NewExchange = newdiff.Field<string>("Exchange"),
                    NewSecurityType = newdiff.Field<string>("SecurityType"),
                    OldSymbol = (grp == null) ? "" : grp.Field<string>("Symbol"),
                    OldExchange = (grp == null) ? "" : grp.Field<string>("Exchange"),
                    OldSecurityType = (grp == null) ? "" : grp.Field<string>("SecurityType")
                };

                leftResultList = leftOuterJoin.ToList();

                var rightOuterJoin =
                from olddiff in _dtDiffOld2New.AsEnumerable()
                join newdiff in _dtDiffNew2Old.AsEnumerable()
                    on olddiff.Field<string>("Symbol") equals
                       newdiff.Field<string>("Symbol") into Grp
                from grp in Grp.DefaultIfEmpty()
                select new DifferenceInfo()
                {
                    NewSymbol = (grp == null) ? "" : grp.Field<string>("Symbol"),
                    NewExchange = (grp == null) ? "" : grp.Field<string>("Exchange"),
                    NewSecurityType = (grp == null) ? "" : grp.Field<string>("SecurityType"),
                    OldSymbol = olddiff.Field<string>("Symbol"),
                    OldExchange = olddiff.Field<string>("Exchange"),
                    OldSecurityType = olddiff.Field<string>("SecurityType")
                };

                ResultList = leftOuterJoin.Union(rightOuterJoin).Distinct().ToList();
                var ResultListDistinct = ResultList.Distinct(new DifferenceInfoComparer());
                //MessageBox.Show(aaa.Count().ToString());

                Tool.LogFile("NewSymbol\tNewExchange\tNewSecurityType\tOldSymbol\tOldExchange\tOldSecurityType");
                foreach (DifferenceInfo cominfo in ResultListDistinct)
                {
                    Tool.LogFile(cominfo.OldSymbol + "\t" + cominfo.OldExchange + "\t" + cominfo.OldSecurityType + "\t" + cominfo.NewSymbol + "\t" + cominfo.NewExchange + "\t" + cominfo.NewSecurityType);
                }
            }

            #endregion

            
        }
        #region  DifferenceInfo比对信息 及 实现IEqualityComparer接口
        public class DifferenceInfo 
        {
            public string NewSymbol { get; set; }
            public string NewExchange { get; set; }
            public string NewSecurityType { get; set; }
            public string OldSymbol { get; set; }
            public string OldExchange { get; set; }
            public string OldSecurityType { get; set; }

            

        }

        //实现IEqualityComparer接口
        public class DifferenceInfoComparer : IEqualityComparer<DifferenceInfo>
        {
            public bool Equals(DifferenceInfo newer, DifferenceInfo older)
            {
                if (newer == null && older == null)
                    return true;
                else if (newer == null | older == null)
                    return false;
                else if (newer.NewSymbol.Equals(older.NewSymbol) && newer.NewExchange.Equals(older.NewExchange) && newer.NewSecurityType.Equals(older.NewSecurityType)
                    && newer.OldSymbol.Equals(older.OldSymbol) && newer.OldExchange.Equals(older.OldExchange) && newer.OldSecurityType.Equals(older.OldSecurityType))
                    return true;
                else
                    return false;
            }

            //hashcode优先选取New，为空则取Old
            // If Equals() returns true for a pair of objects   
            // then GetHashCode() must return the same value for these objects.  
            public int GetHashCode(DifferenceInfo compare)
            {
                //Get hash code for the Id field.  
                int hashSymbol = compare.NewSymbol == null ? compare.OldSymbol.GetHashCode() : compare.NewSymbol.GetHashCode();

                //Get hash code for the Name field if it is not null.  
                int hashExchange = compare.NewExchange == null ? compare.OldExchange.GetHashCode() : compare.NewExchange.GetHashCode();

                //Get hash code for the Class field if it is not null.  
                int hashSecurityType = compare.NewSecurityType == null ? compare.OldSecurityType.GetHashCode() : compare.NewSecurityType.GetHashCode();


                //Calculate the hash code for the Student.  
                return (hashSymbol ^ hashExchange ^ hashSecurityType).GetHashCode();
            }
        }

        #endregion

        
    }
}
