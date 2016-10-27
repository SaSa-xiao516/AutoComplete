using AutoComplete.Comm;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoComplete
{
    class AutoCompleteTest : AutoCompleteTestBase
    {
        #region 校验SecType变化
        //以RT的SecType为准进行验证，再验证RT的变化是否合理

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
                    Symbol = Data[7].Replace("\"", "");
                    Exchange = Data[6].Replace("\"", "");
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


        public static void Verify1022()
        {
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

            Dictionary<string, Dictionary<string, string>> AutoComplete_New = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> AutoComplete_Index_New = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> AutoComplete_Old = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> RealTime_New = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, Dictionary<string, string>> Expected = new Dictionary<string, Dictionary<string, string>>();

            AutoComplete_New = ReturnDicFromACCsv(strAutoCSV_New);
            AutoComplete_Index_New = ReturnDicFromACCsv(strAutoCSV_Index_New);
            AutoComplete_Old = ReturnDicFromACCsv(strAutoCSV_Old);
            RealTime_New = ReturnDicFromRTTxt(strRealTime_New,"223");
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

            DataTable _dtNewRT = new DataTable();//ReturnDataTableFromRTTxt(steRealTimeNewPath);
            DataTable _dtOldRT = new DataTable();//ReturnDataTableFromRTTxt(strRealTimeOldPath);

            DataTable _dtNewAC = new DataTable();//ReturnDataTableFromACCsv(strAutoCompleteNewPath);
            DataTable _dtOldAC = new DataTable();//ReturnDataTableFromACCsv(strAutoCompleteOldPath);

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

            DataTable _dtNewRT = new DataTable(); //ReturnDataTableFromRTTxt(steRealTimeNewPath);
            DataTable _dtOldRT = new DataTable(); //ReturnDataTableFromRTTxt(strRealTimeOldPath);

            DataTable _dtNewAC = new DataTable(); //ReturnDataTableFromACCsv(strAutoCompleteNewPath);
            DataTable _dtOldAC = new DataTable(); //ReturnDataTableFromACCsv(strAutoCompleteOldPath);

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

        #region 读取ExpectedFile至Dictionary
        public static Dictionary<string, Dictionary<string, string>> ReturnDicFromExpectedFile(string strPath)
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
                    string NewsecType = string.Empty;
                    string OldsecType = string.Empty;
                    string Other = string.Empty;
                    string[] Data = str.Split(',');
                    Symbol = Data[3].Replace("\"", ""); ;
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

        #region 验证Share超过90天无交易 提取SP提高批量验证效率  单个采用SP
        public static Dictionary<string, int> VerifyExceed90Day(List<String> ShareClassIdList)
        {
            Dictionary<string, int> toCheckDic = new Dictionary<string, int>();
            try
            {
                string ShareClassIdString = "";
                string sql = "select ShareClassId,ISNULL(LastTradeDate,TradingDate) LastTradeDate from ( select  row_number() over(partition by ShareClassId order by TradingDate desc) as rownum,ShareClassId ,LastTradeDate,TradingDate from CentralRawData..HistoricalRawPrice where ShareClassId in ("+ ShareClassIdString+")) AA where AA.rownum=1 ";

                AccessOutputDB accessOutputDB = new AccessOutputDB();  //LastTradingDate
                DataSet ds = accessOutputDB.getEquityXOICurrentShareClassInfo("0P000002RH");
                
                foreach (var ShareClassId in ShareClassIdList) {
                    toCheckDic.Add(ShareClassId, 0);
                }


                foreach (DataRow raw in ds.Tables[0].Rows) {
                    toCheckDic[raw["ShareClassId"].ToString()] = Int32.Parse(raw["LastTradingDate"].ToString());
                }

                foreach (KeyValuePair<string, int> pair in toCheckDic) {
                    if (pair.Value>=90) {
                        Tool.LogFile("Info:[" + pair.Key + "]超过90天");
                    }else{
                        Tool.LogFile("Info:["+pair.Key+"]没有超过90天");
                    }
                }
                return toCheckDic;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:" + ex.ToString());
                return toCheckDic;
            }
            //WebServiceCheck.LoginGID();
            //WebServiceCheck.test();
        }

        public static bool VerifyExceed90Day(string ShareClassIdList)
        {
            bool isExceed = false;
            try
            {
                AccessOutputDB accessOutputDB = new AccessOutputDB();  
                DataSet ds = accessOutputDB.getEquityXOICurrentShareClassInfo(ShareClassIdList);
                DateTime nowDate = DateTime.Now;
                DateTime lastTradingDate = DateTime.Parse(ds.Tables[0].Rows[0]["LastTradingDate"].ToString());
                return Int32.Parse((nowDate - lastTradingDate).TotalDays.ToString())>90?true:false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error:[" + ShareClassIdList + "查询验证是否超过90天]" + ex.ToString());
                Tool.LogFile("Error:[" + ShareClassIdList + "查询验证是否超过90天]");
                return isExceed;
            }
        }
        #endregion
    }
}
