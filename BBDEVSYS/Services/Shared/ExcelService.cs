using BBDEVSYS.Models.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

namespace BBDEVSYS.Services.Shared
{
    public class ExcelService
    {
        public static ValidationWithReturnResult<DataSet> ConvertExcelToDataSet1(string filePath, string fileName, string sheet)
        {
            ValidationWithReturnResult<DataSet> result = new ValidationWithReturnResult<DataSet>();

            DataSet ds = new DataSet();

            try
            {
                string fullPath = Path.Combine(filePath, fileName);

                string[] sSheetNameList;
                string sConnection;
                DataTable dtTablesList;
                OleDbCommand oleExcelCommand;
                OleDbConnection oleExcelConnection;

                sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPath + @";Extended Properties=""Excel 12.0;HDR=No;IMEX=1;""";
                /////
             
                    string fileExtension = System.IO.Path.GetExtension(fileName);

                    if (fileExtension == ".xls" || fileExtension == ".xlsx")
                    {
                        string fileLocation = fullPath;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }
                        string excelConnectionString = string.Empty;
                        excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        //connection String for xls file format.
                        if (fileExtension == ".xls")
                        {
                            excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                        }
                        //connection String for xlsx file format.
                        else if (fileExtension == ".xlsx")
                        {
                            excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                        }
                        //Create Connection to Excel work book and add oledb namespace
                        OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                        excelConnection.Open();
                        DataTable dt = new DataTable();

                        dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dt == null)
                        {
                            return null;
                        }

                        String[] excelSheets = new String[dt.Rows.Count];
                        int t = 0;
                        //excel data saves in temp file here.
                        foreach (DataRow row in dt.Rows)
                        {
                            excelSheets[t] = row["TABLE_NAME"].ToString();
                            t++;
                        }
                        OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);

                        string query = string.Format("SELECT * FROM [{0}]", excelSheets[0]);
                        ////////////////////////////////////////////TEST///////////////////////////////////////////////////////////////////////////////////
                        //string query = string.Format("SELECT * INTO [FSM].[temp_DFS_Akustik] FROM [{0}]", excelSheets[0]);
                        ////////////////////////////////////////////TEST///////////////////////////////////////////////////////////////////////////////////
                        using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                        {
                            dataAdapter.Fill(ds);
                            ////////////////////////////////////////////TEST///////////////////////////////////////////////////////////////////////////////////

                            ////////////////////////////////////////////TEST///////////////////////////////////////////////////////////////////////////////////
                        }
                    }
                    if (fileExtension.ToString().ToLower().Equals(".xml"))
                    {
                        string fileLocation = fullPath;
                        if (System.IO.File.Exists(fileLocation))
                        {
                            System.IO.File.Delete(fileLocation);
                        }

                        //Request.Files["FileUpload"].SaveAs(fileLocation);
                        XmlTextReader xmlreader = new XmlTextReader(fileLocation);
                        // DataSet ds = new DataSet();
                        ds.ReadXml(xmlreader);
                        xmlreader.Close();
                    }

                    //for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    //{
                    //    string conn = ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString;
                    //    SqlConnection con = new SqlConnection(conn);

                    //    string query = "INSERT INTO [fanselect_man].[FSM].[DFS_Akustik](MessID,KL_ID,MP_ID,LwLin50ss,LwLin63ss,LwLin80ss,LwLin100ss,LwLin125ss,LwLin160ss,LwLin200ss,LwLin250ss,LwLin315ss,LwLin400ss,LwLin500ss,LwLin630ss,LwLin800ss,LwLin1000ss,LwLin1250ss,LwLin1600ss,LwLin2000ss,LwLin2500ss,LwLin3150ss,LwLin4000ss,LwLin5000ss,LwLin6300ss,LwLin8000ss,LwLin10000ss,LwLin12500ss,LwLin16000ss,LwLin20000ss,LwLin50ds,LwLin63ds,LwLin80ds,LwLin100ds,LwLin125ds,LwLin160ds,LwLin200ds,LwLin250ds,LwLin315ds,LwLin400ds,LwLin500ds,LwLin630ds,LwLin800ds,LwLin1000ds,LwLin1250ds,LwLin1600ds,LwLin2000ds,LwLin2500ds,LwLin3150ds,LwLin4000ds,LwLin5000ds,LwLin6300ds,LwLin8000ds,LwLin10000ds,LwLin12500ds,LwLin16000ds,LwLin20000ds) VALUES ('" + ds.Tables[0].Rows[i][0].ToString() + "', '" + ds.Tables[0].Rows[i][1].ToString() + "', '" + ds.Tables[0].Rows[i][2].ToString() + "', '" + ds.Tables[0].Rows[i][3].ToString() + "', '" + ds.Tables[0].Rows[i][4].ToString() + "', '" + ds.Tables[0].Rows[i][5].ToString() + "', '" + ds.Tables[0].Rows[i][6].ToString() + "', '" + ds.Tables[0].Rows[i][7].ToString() + "', '" + ds.Tables[0].Rows[i][8].ToString() + "', '" + ds.Tables[0].Rows[i][9].ToString() + "', '" + ds.Tables[0].Rows[i][10].ToString() + "', '" + ds.Tables[0].Rows[i][11].ToString() + "', '" + ds.Tables[0].Rows[i][12].ToString() + "', '" + ds.Tables[0].Rows[i][13].ToString() + "', '" + ds.Tables[0].Rows[i][14].ToString() + "', '" + ds.Tables[0].Rows[i][15].ToString() + "', '" + ds.Tables[0].Rows[i][16].ToString() + "', '" + ds.Tables[0].Rows[i][17].ToString() + "', '" + ds.Tables[0].Rows[i][18].ToString() + "', '" + ds.Tables[0].Rows[i][19].ToString() + "', '" + ds.Tables[0].Rows[i][20].ToString() + "', '" + ds.Tables[0].Rows[i][21].ToString() + "', '" + ds.Tables[0].Rows[i][22].ToString() + "', '" + ds.Tables[0].Rows[i][23].ToString() + "', '" + ds.Tables[0].Rows[i][24].ToString() + "', '" + ds.Tables[0].Rows[i][25].ToString() + "', '" + ds.Tables[0].Rows[i][26].ToString() + "', '" + ds.Tables[0].Rows[i][27].ToString() + "', '" + ds.Tables[0].Rows[i][28].ToString() + "', '" + ds.Tables[0].Rows[i][29].ToString() + "', '" + ds.Tables[0].Rows[i][30].ToString() + "', '" + ds.Tables[0].Rows[i][31].ToString() + "', '" + ds.Tables[0].Rows[i][32].ToString() + "', '" + ds.Tables[0].Rows[i][33].ToString() + "', '" + ds.Tables[0].Rows[i][34].ToString() + "', '" + ds.Tables[0].Rows[i][35].ToString() + "', '" + ds.Tables[0].Rows[i][36].ToString() + "', '" + ds.Tables[0].Rows[i][37].ToString() + "', '" + ds.Tables[0].Rows[i][38].ToString() + "', '" + ds.Tables[0].Rows[i][39].ToString() + "', '" + ds.Tables[0].Rows[i][40].ToString() + "', '" + ds.Tables[0].Rows[i][41].ToString() + "', '" + ds.Tables[0].Rows[i][42].ToString() + "', '" + ds.Tables[0].Rows[i][43].ToString() + "', '" + ds.Tables[0].Rows[i][44].ToString() + "', '" + ds.Tables[0].Rows[i][45].ToString() + "', '" + ds.Tables[0].Rows[i][46].ToString() + "', '" + ds.Tables[0].Rows[i][47].ToString() + "', '" + ds.Tables[0].Rows[i][48].ToString() + "', '" + ds.Tables[0].Rows[i][49].ToString() + "', '" + ds.Tables[0].Rows[i][50].ToString() + "', '" + ds.Tables[0].Rows[i][51].ToString() + "', '" + ds.Tables[0].Rows[i][52].ToString() + "', '" + ds.Tables[0].Rows[i][53].ToString() + "', '" + ds.Tables[0].Rows[i][54].ToString() + "', '" + ds.Tables[0].Rows[i][55].ToString() + "', '" + ds.Tables[0].Rows[i][56].ToString() + "')";
                    //    con.Open();
                    //    SqlCommand cmd = new SqlCommand(query, con);
                    //    cmd.ExecuteNonQuery();
                    //    con.Close();
                    //}
                
                
                /////


                oleExcelConnection = new OleDbConnection(sConnection);
                oleExcelConnection.Open();

                dtTablesList = oleExcelConnection.GetSchema("Tables");
                List<string> listSheet = new List<string>();
                foreach (DataRow drSheet in dtTablesList.Rows)
                {
                    if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                    {
                        listSheet.Add(drSheet["TABLE_NAME"].ToString());
                    }
                }
                if (listSheet.Any())
                {
                    sheet = "[" + listSheet[0] + "]";
                }
                else
                {
                    sheet = "[" + sheet + "$]";
                }
                if (dtTablesList.Rows.Count > 0)
                {
                    sSheetNameList = new string[dtTablesList.Rows.Count];
                    oleExcelCommand = new OleDbCommand(("Select * From " + sheet), oleExcelConnection);

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    ds.Tables.Add(dt);
                }

                oleExcelConnection.Close();
                dtTablesList.Clear();
                dtTablesList.Dispose();

                result.ReturnResult = ds;
            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");
            }

            return result;
        }
        public static ValidationWithReturnResult<DataSet> ConvertExcelToDataSet(string filePath, string fileName, string sheet)
        {
            ValidationWithReturnResult<DataSet> result = new ValidationWithReturnResult<DataSet>();

            DataSet ds = new DataSet();

            try
            {
                string fullPath = Path.Combine(filePath, fileName);

                string[] sSheetNameList;
                string sConnection;
                DataTable dtTablesList;
                OleDbCommand oleExcelCommand;                
                OleDbConnection oleExcelConnection;

                sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPath + @";Extended Properties=""Excel 12.0;HDR=No;IMEX=1;""";

                

                oleExcelConnection = new OleDbConnection(sConnection);
                oleExcelConnection.Open();

                dtTablesList = oleExcelConnection.GetSchema("Tables");
                List<string> listSheet = new List<string>();
                foreach (DataRow drSheet in dtTablesList.Rows)
                {
                    if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                    {
                        listSheet.Add(drSheet["TABLE_NAME"].ToString());
                    }
                }
                if (listSheet.Any())
                {
                    sheet = "[" + listSheet[0] + "]";
                }
                else
                {
                    sheet = "[" + sheet + "$]";
                }
                if (dtTablesList.Rows.Count > 0)
                {
                    sSheetNameList = new string[dtTablesList.Rows.Count];
                    oleExcelCommand = new OleDbCommand(("Select * From " + sheet), oleExcelConnection);

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);
                    DataTable dt = new DataTable();
                    dataAdapter.Fill(dt);

                    ds.Tables.Add(dt);
                }

                oleExcelConnection.Close();
                dtTablesList.Clear();
                dtTablesList.Dispose();

                result.ReturnResult = ds;
            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");
            }

            return result;
        }
    }
}