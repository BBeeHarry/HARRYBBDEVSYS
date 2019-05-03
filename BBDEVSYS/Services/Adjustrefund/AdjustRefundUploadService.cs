using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Adjustrefund;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Services.Adjustrefund
{
    public class AdjustRefundUploadService
    {
        public static DataSet dataSet = new DataSet();
        public static DataTable dataTable = new DataTable();

        public static int indexData = 0;

        //'=== > excel connection var
        public static OleDbConnection cn = new OleDbConnection();
        public static OleDbDataAdapter da = new OleDbDataAdapter();
        //public static DataTable dt = new DataTable();

        public static SqlConnection sqlConn = new SqlConnection();
        public static SqlDataAdapter sqlDa = new SqlDataAdapter();
        public static SqlBulkCopy bulkCopy;
        // SqlBulkCopy bulkCopy = new SqlBulkCopy();
        public ValidationWithReturnResult<List<AdjustrefundUploadViewModel>> ImportExcel(string fileName, string sheetName)
        {

            ValidationWithReturnResult<List<AdjustrefundUploadViewModel>> result = new ValidationWithReturnResult<List<AdjustrefundUploadViewModel>>();
            result.ReturnResult = new List<AdjustrefundUploadViewModel>();

            List<AdjustrefundUploadViewModelItems> uploadItemList = new List<AdjustrefundUploadViewModelItems>();
            ValidationWithReturnResult<DataSet> resultDataSet = new ValidationWithReturnResult<DataSet>();

            try
            {
                //Check required field


                //if (string.IsNullOrEmpty(matType))
                //{
                //    result.ErrorFlag = true;
                //    //result.Message = ValidatorMessage.notempty_error.Replace("{PropertyName}", ResourceText.InvoiceCategory);
                //    result.ModelStateErrorList.Add(new ModelStateError("", result.Message));

                //}
                //var keyDateVal = DateTime.ParseExact(keyDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"));

                if (result.ErrorFlag)
                {
                    return result;
                }


                string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);
                //DataSet excelDS = ExcelService.ConvertExcelToDataSet(tempFilePath, fileName, 1);
                resultDataSet = ConvertExcelToDataSet(tempFilePath, fileName, sheetName);

                DataSet excelDS = new DataSet();

                if (resultDataSet.ErrorFlag)
                {
                    result.ErrorFlag = true;
                    result.Message = resultDataSet.Message;

                    return result;
                }
                else
                {
                    excelDS = resultDataSet.ReturnResult;
                }
                dataSet = excelDS;

                //Convert dataset to data view
                int rowIndex = 1; //Start data at row 1 (begin row is 0)

                //foreach (DataTable table in excelDS.Tables)
                //{
                //    while (rowIndex < table.Rows.Count)
                //    {
                //        DataRow row = table.Rows[rowIndex];
                //        AdjustrefundUploadViewModel uploadItem = new AdjustrefundUploadViewModel();

                //        //uploadItem.INV_MONTH = row[++col].ToString();





                //        uploadItemList.Add(uploadItem);
                //        rowIndex++;
                //        t++;
                //    }
                //    rowIndex = 1;
                //}

                //result =  ValidateUploadItemFormat(uploadItemList);

                var adjList = new List<AdjustrefundUploadViewModel>();
                var adjItemList = new List<AdjustrefundUploadViewModelItems>();
                var adjmodel = new AdjustrefundUploadViewModel();


                foreach (DataTable item in excelDS.Tables)
                {
                    for (int i = 1; i < item.Rows.Count; i++)
                    {
                        AdjustrefundUploadViewModelItems model = new AdjustrefundUploadViewModelItems();
                        DataRow row = item.Rows[i];
                        int col = 0;
                        //for (int col = 0; col < item.Columns.Count; col++)
                        //{
                        //    AdjustrefundUploadViewModelItems model = new AdjustrefundUploadViewModelItems();
                        //    MVMMappingService.MoveData(dr[col], model);
                        //    adjItemList.Add(model);
                        //}
                        model.SR_STATUS = "";// row[col].ToString();
                        //DateTime.ParseExact(keyDate, "dd/MM/yyyy", CultureInfo.CreateSpecificCulture("en-US"))
                        model.SR_OPEN_DATE = DateTime.Parse(row[col].ToString(), CultureInfo.CreateSpecificCulture("en-US"));
                        model.SR_NO = row[++col].ToString();
                        model.CATEGORY = row[++col].ToString();
                        model.SUB_CATEGORY = row[++col].ToString();
                        model.ISSUE = row[++col].ToString();
                        model.BAN_1 = row[++col].ToString();
                        model.PRIM_RESOURCE = row[++col].ToString();
                        model.SR_DETAILS = row[++col].ToString();
                        model.SR_DIVISION = row[++col].ToString();
                        model.SR_OWNER = row[++col].ToString();
                        model.BAN_1 = row[++col].ToString();
                        model.BAN_2 = row[++col].ToString();
                        model.AR_BALANCE_1 = Convert.ToDecimal(row[++col].ToString());
                        model.BAN_1 = row[++col].ToString();
                        model.CUSTOMER_NAME_1 = row[++col].ToString();
                        model.ACCOUNT_TYPE_1 = row[++col].ToString();
                        model.COMP_CODE_1 = row[++col].ToString();
                        model.AR_BALANCE_1 = Convert.ToDecimal(row[++col].ToString());
                        model.BEN_STATUS_1 = row[++col].ToString();
                        model.IDENT_1 = row[++col].ToString();
                        model.CONV_IND_1 = row[++col].ToString();
                        model.CONV_CODE_1 = row[++col].ToString();
                        model.T_FORM_ACCOUNT_BC_ID = "";// row[++col].ToString();
                        model.BAN_2 = row[++col].ToString();
                        model.CUSTOMER_NAME_2 = row[++col].ToString();
                        model.ACCOUNT_TYPE_2 = row[++col].ToString();
                        model.COMP_CODE_2 = row[++col].ToString();
                        model.AR_BALANCE_2 = Convert.ToDecimal(row[++col].ToString());
                        model.BEN_STATUS_2 = row[++col].ToString();
                        model.IDENT_2 = row[++col].ToString();
                        model.CONV_IND_2 = row[++col].ToString();
                        model.CONV_CODE_2 = row[++col].ToString();
                        model.T_TO_ACCOUNT_BC_ID = "";// row[++col].ToString();
                        model.RECEIPT_NO = row[++col].ToString();
                        model.PAY_AMOUNT = Convert.ToDecimal(row[++col].ToString());
                        model.DEPOSIT_DATE = DateTime.Parse(row[++col].ToString(), CultureInfo.CreateSpecificCulture("en-US"));
                        model.SOURCE_ID = row[++col].ToString();
                        model.DOC_BILL_TYPE = row[++col].ToString();
                        model.FILE_NAME = fileName;

                        adjItemList.Add(model);
                    }
                }
                adjmodel.adjList.AddRange(adjItemList);
                adjList.Add(adjmodel);
                result.ReturnResult = adjList;

            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");

                return result;
            }

            return result;
        }

        public byte[] SubmitFormFileContent(AdjustrefundUploadViewModel formData)
        {
            byte[] filecontent = null;
            try
            {
                DataSet getDataset = new DataSet();

                getDataset = InitialDataFormViewModel(formData);

                //if (_data.Any())
                //{
                //DataTable _dt = new DataTable();
                //_dt = ReportService.ToDataTable(_data);
                //dataMerge.TableName = "";
                //ds = new DataSet();
                //ds.Tables.Add(dataMerge);
                //}


                if (getDataset.Tables.Count > 0)
                {

                    filecontent = ExcelExportHelper.ExportExcelMutiSheetData(getDataset, "", false);//, columns);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return filecontent;
        }

        public static DataSet InitialDataFormViewModel(AdjustrefundUploadViewModel formData)
        {
            DataSet getDataset = new DataSet();
            try
            {
                #region reading excel multi files
                DataSet ds = new DataSet();

                ValidationWithReturnResult<DataTable> resultDataTable = new ValidationWithReturnResult<DataTable>();


                int i = 1;
                foreach (var item in formData.AttachmentList)
                {
                    //string fileName = "";
                    string sheetName = "SheetFile" + i.ToString();
                    //string fileNameIndex = "";
                    var sheetModel = item.SheetNameExcel;//formData.GetType().GetProperty(sheetName).GetValue(formData);


                    string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                    tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);

                    string fileUniqueKey = AttachmentService.GetFileUniqueKey();
                    string savedFileName = item.SavedFileName;// fileUniqueKey + "_" + item.FileName;


                    DataTable dtFile = new DataTable();
                    resultDataTable = ConvertExcelToDataTable(tempFilePath, savedFileName, Convert.ToString(sheetModel));

                    if (!resultDataTable.ErrorFlag)
                    {
                        dtFile = resultDataTable.ReturnResult.Copy();

                        dtFile.Columns.Add("FileName", typeof(string));
                        dtFile.Columns["FileName"].SetOrdinal(0);
                        foreach (DataRow row in dtFile.Rows)
                        {
                            row[0] = item.FileName;
                        }

                    }
                    ds.Tables.Add(dtFile);
                    ds.Tables[(i - 1)].TableName = item.FileName;
                    i++;


                }
                #endregion


                if (ds.Tables.Count > 0)
                {

                    //List<AdjustrefundUploadViewModel> _data = MergeFormData();
                    DataTable dataMerge = MergeAllData(ds).AsEnumerable().CopyToDataTable();
                    dataMerge.TableName = "SQL_Results";
                    getDataset.Tables.Add(dataMerge);


                    DataTable dtParam = dataMerge.AsEnumerable().CopyToDataTable(); dtParam.TableName = "DataParamMearge";
                    DataTable dataResultVerified3 = GenerateFormatAllData(dtParam).AsEnumerable().CopyToDataTable();
                    dataResultVerified3.TableName = "Verify#3";
                    getDataset.Tables.Add(dataResultVerified3);

                    if (dataResultVerified3.AsEnumerable().Any())
                    {
                        DataTable sheetdataResultCheckLastadj = dataResultVerified3.AsEnumerable().CopyToDataTable();
                        #region sheet distinct
                        DataTable dataResultCheckLastadj = new DataTable();
                        dataResultCheckLastadj = sheetdataResultCheckLastadj.DefaultView.ToTable(true, "BAN_INCORRECT").AsEnumerable().Where(m => m["BAN_INCORRECT"] != DBNull.Value).CopyToDataTable();
                        dataResultCheckLastadj.TableName = "BAN Last Adjust";
                        getDataset.Tables.Add(dataResultCheckLastadj);
                        #endregion

                        DataTable sheetdataResultSendtoVerify = dataResultVerified3.AsEnumerable().CopyToDataTable();
                        DataTable dataResultSendtoVerify = new DataTable();
                        dataResultSendtoVerify = (from m in sheetdataResultSendtoVerify.AsEnumerable()
                                                  where m.Field<string>("Result_PRS_Before_Batch") == "Send To Verify"//Fund Transfer //Batch Refund
                                                  select m).CopyToDataTable();
                        dataResultSendtoVerify.TableName = "Send To Verify";
                        getDataset.Tables.Add(dataResultSendtoVerify);

                        DataTable sheetdataResultBatchFundTransfer = dataResultVerified3.AsEnumerable().CopyToDataTable();
                        DataTable dataResultBatchFundTransfer = new DataTable();
                        dataResultBatchFundTransfer = (from m in sheetdataResultBatchFundTransfer.AsEnumerable()
                                                       where m.Field<string>("Result_PRS_Before_Batch") == "Batch Fund Transfer"//Fund Transfer //Batch Refund
                                                       select m).CopyToDataTable();
                        dataResultBatchFundTransfer.TableName = "Batch Fund Transfer";
                        getDataset.Tables.Add(dataResultBatchFundTransfer);

                        DataTable sheetdatadataResultBatch_Refund = dataResultVerified3.AsEnumerable().CopyToDataTable();
                        DataTable dataResultBatch_Refund = new DataTable();
                        dataResultBatch_Refund = (from m in sheetdatadataResultBatch_Refund.AsEnumerable()
                                                  where m.Field<string>("Result_PRS_Before_Batch") == "Batch Refund"//Fund Transfer //Batch Refund
                                                  select m).CopyToDataTable();
                        dataResultBatch_Refund.TableName = "Batch Refund";
                        getDataset.Tables.Add(dataResultBatch_Refund);



                    }
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return getDataset;
        }
        public ValidationWithReturnResult<DataTable> InitialDataFormUploadViewModel(AdjustrefundUploadViewModel formData)
        {
            DataSet getDataset = new DataSet();
            ValidationWithReturnResult<DataTable> resultDataTable = new ValidationWithReturnResult<DataTable>();
            try
            {
                #region reading excel multi files
                DataSet ds = new DataSet();
                int i = 1;
                if (formData.AttachmentList.Any())
                {
                    var item = formData.AttachmentList.FirstOrDefault();

                    string sheetName = string.IsNullOrEmpty(item.SheetNameExcel) ? "Sheet" + i.ToString() : item.SheetNameExcel;

                    string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                    tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);

                    string savedFileName = item.SavedFileName;


                    DataTable dtFile = new DataTable();


                    resultDataTable = ConvertExcelToDataTable(tempFilePath, savedFileName, sheetName);

                    if (!resultDataTable.ErrorFlag)
                    {
                        DataTable data = new DataTable();

                        data = resultDataTable.ReturnResult.AsEnumerable().CopyToDataTable();
                        dtFile = data;// GenerateFormatFieldUploadtoMIS(data);

                        resultDataTable.ReturnResult.Clear();
                        resultDataTable.ReturnResult = dtFile.AsEnumerable().CopyToDataTable();
                    }
                    getDataset.Tables.Add(dtFile);
                }
                #endregion

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return resultDataTable;
        }
        private static DataTable GenerateFormatFieldUploadtoMIS(DataTable data)
        {


            DataTable dataUpload = new DataTable();
            DataTable dataMapping = new DataTable();
            try
            {
                if (data.Rows.Count > 0)
                {
                    dataUpload = data.AsEnumerable().CopyToDataTable();

                    var existscol = dataUpload.Columns.Contains("RF_ID");

                    if (!dataUpload.Columns.Contains("RF_ID")) dataUpload.Columns.Add("RF_ID");
                    if (!dataUpload.Columns.Contains("DOC_NO")) dataUpload.Columns.Add("DOC_NO");
                    if (!dataUpload.Columns.Contains("REF_NO")) dataUpload.Columns.Add("REF_NO");
                    if (!dataUpload.Columns.Contains("REASON_ID")) dataUpload.Columns.Add("REASON_ID");
                    if (!dataUpload.Columns.Contains("REASON_REFUND")) dataUpload.Columns.Add("REASON_REFUND");
                    if (!dataUpload.Columns.Contains("ADDRESS")) dataUpload.Columns.Add("ADDRESS");
                    if (!dataUpload.Columns.Contains("EXPLANATION")) dataUpload.Columns.Add("EXPLANATION");
                    if (!dataUpload.Columns.Contains("REQEST_BY")) dataUpload.Columns.Add("REQEST_BY");
                    if (!dataUpload.Columns.Contains("COMPLETE_BY")) dataUpload.Columns.Add("COMPLETE_BY");
                    if (!dataUpload.Columns.Contains("COMPLETE_DATE")) dataUpload.Columns.Add("COMPLETE_DATE", typeof(DateTime));
                    if (!dataUpload.Columns.Contains("DUE_DATE")) dataUpload.Columns.Add("DUE_DATE", typeof(DateTime));
                    if (!dataUpload.Columns.Contains("DOC_STATUS")) dataUpload.Columns.Add("DOC_STATUS");
                    if (!dataUpload.Columns.Contains("DESIGNATION_CODE_FROM")) dataUpload.Columns.Add("DESIGNATION_CODE_FROM");
                    if (!dataUpload.Columns.Contains("REF1_FROM")) dataUpload.Columns.Add("REF1_FROM");
                    if (!dataUpload.Columns.Contains("REF2_FROM")) dataUpload.Columns.Add("REF2_FROM");
                    if (!dataUpload.Columns.Contains("DESIGNATION_CODE_TO")) dataUpload.Columns.Add("DESIGNATION_CODE_TO");
                    if (!dataUpload.Columns.Contains("REF1_TO")) dataUpload.Columns.Add("REF1_TO");
                    if (!dataUpload.Columns.Contains("REF2_TO")) dataUpload.Columns.Add("REF2_TO");


                    DataTable dtCol = new DataTable();
                    foreach (DataColumn item in dataUpload.Columns)
                    {
                        dtCol.Columns.Add(item.ColumnName);
                        dtCol.Columns[item.ColumnName].DataType = System.Type.GetType(item.DataType.FullName);
                    }
                    var query = (from order in dataUpload.AsEnumerable()
                                 select new
                                 {
                                     RF_ID = dataUpload.Columns.Contains("RF_ID") ? null : order["RF_ID"],
                                     REQUEST_NO = dataUpload.Columns.Contains("SR_NO") ? null : order["SR_NO"],
                                     DOC_NO = dataUpload.Columns.Contains("DOC_NO") ? null : order["DOC_NO"],
                                     REF_NO = dataUpload.Columns.Contains("REF_NO") ? null : order["REF_NO"],
                                     PAID_FROM = dataUpload.Columns.Contains("COMP_CODE_1") ? null : order["COMP_CODE_1"],
                                     ACCOUNT_FROM = dataUpload.Columns.Contains("BAN_12") ? null : order["BAN_12"],
                                     CUSTOMER_NAME = dataUpload.Columns.Contains("CUSTOMER_NAME_1") ? null : order["CUSTOMER_NAME_1"],
                                     PAID_TO = dataUpload.Columns.Contains("COMP_CODE_2") ? null : order["COMP_CODE_2"],
                                     ACCOUNT_TO = dataUpload.Columns.Contains("BAN_21") ? null : order["BAN_21"],
                                     CUSTOMER_NAME_TO = dataUpload.Columns.Contains("CUSTOMER_NAME_2") ? null : order["CUSTOMER_NAME_2"],
                                     DEPOSIT_DATE = dataUpload.Columns.Contains("DEPOSITE_DATE") ? (DateTime?)null : order["DEPOSITE_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("DEPOSITE_DATE"),
                                     SOURCE_ID = dataUpload.Columns.Contains("SOURCE_ID") ? null : order["SOURCE_ID"],
                                     AMOUNT = dataUpload.Columns.Contains("PAY_AMOUNT") ? 0 : order["PAY_AMOUNT"] == System.DBNull.Value ? 0 : order.Field<double>("PAY_AMOUNT"),
                                     RECEIPT_NO = dataUpload.Columns.Contains("RECEIPT_NO") ? null : order["RECEIPT_NO"],
                                     REASON_ID = dataUpload.Columns.Contains("REASON_ID") ? null : order["REASON_ID"],
                                     REASON_REFUND = dataUpload.Columns.Contains("REASON_REFUND") ? null : order["REASON_REFUND"],
                                     ADDRESS = dataUpload.Columns.Contains("ADDRESS") ? null : order["ADDRESS"],
                                     EXPLANATION = dataUpload.Columns.Contains("EXPLANATION") ? null : order["EXPLANATION"],
                                     REQEST_BY = dataUpload.Columns.Contains("REQEST_BY") ? null : order["REQEST_BY"],
                                     CREATE_DATE = dataUpload.Columns.Contains("SR_OPEN_DATE") ? null : order["SR_OPEN_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("SR_OPEN_DATE"),
                                     COMPLETE_BY = dataUpload.Columns.Contains("COMPLETE_BY") ? null : order["COMPLETE_BY"],
                                     COMPLETE_DATE = dataUpload.Columns.Contains("COMPLETE_DATE") ? null : order["COMPLETE_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("COMPLETE_DATE"),
                                     DUE_DATE = dataUpload.Columns.Contains("DUE_DATE") ? null : order["DUE_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("DUE_DATE"),
                                     DOC_STATUS = dataUpload.Columns.Contains("DOC_STATUS") ? null : order["DOC_STATUS"],
                                     ACCOUNT_TYPE_FROM = dataUpload.Columns.Contains("ACCOUNT_TYPE_1") ? null : order["ACCOUNT_TYPE_1"],
                                     AR_BALANCE_FROM = dataUpload.Columns.Contains("AR_BALANCE_11") ? 0 : order["AR_BALANCE_11"] == System.DBNull.Value ? 0 : order.Field<double>("AR_BALANCE_11"),
                                     BEN_ACCOUNT_STATUS_FROM = dataUpload.Columns.Contains("BEN_STATUS_1") ? null : order["BEN_STATUS_1"],
                                     IDENTIFICATION_FROM = dataUpload.Columns.Contains("IDENT_1") ? null : order["IDENT_1"],
                                     CONVERGENCE_INDICATER_FROM = dataUpload.Columns.Contains("CONV_IND_1") ? null : order["CONV_IND_1"],
                                     CONVERGENCE_CODE_FROM = dataUpload.Columns.Contains("CONV_CODE_1") ? null : order["CONV_CODE_1"],
                                     ACCOUNT_BC_ID_FROM = dataUpload.Columns.Contains("T_FORM_ACCOUNT_BC_ID") ? null : order["T_FORM_ACCOUNT_BC_ID"],
                                     DESIGNATION_CODE_FROM = dataUpload.Columns.Contains("DESIGNATION_CODE_FROM") ? null : order["DESIGNATION_CODE_FROM"],
                                     REF1_FROM = dataUpload.Columns.Contains("REF1_FROM") ? null : order["REF1_FROM"],
                                     REF2_FROM = dataUpload.Columns.Contains("REF2_FROM") ? null : order["REF2_FROM"],
                                     ACCOUNT_TYPE_TO = dataUpload.Columns.Contains("ACCOUNT_TYPE_2") ? null : order["ACCOUNT_TYPE_2"],
                                     AR_BALANCE_TO = dataUpload.Columns.Contains("AR_BALANCE_2") ? 0 : order["AR_BALANCE_2"] == System.DBNull.Value ? 0 : order.Field<double>("AR_BALANCE_2"),
                                     BEN_ACCOUNT_STATUS_TO = dataUpload.Columns.Contains("BEN_STATUS_2") ? null : order["BEN_STATUS_2"],
                                     IDENTIFICATION_TO = dataUpload.Columns.Contains("IDENT_2") ? null : order["IDENT_2"],
                                     CONVERGENCE_INDICATER_TO = dataUpload.Columns.Contains("CONV_IND_2") ? null : order["CONV_IND_2"],
                                     CONVERGENCE_CODE_TO = dataUpload.Columns.Contains("CONV_CODE_2") ? null : order["CONV_CODE_2"],
                                     ACCOUNT_BC_ID_TO = dataUpload.Columns.Contains("T_TO_ACCOUNT_BC_ID") ? null : order["T_TO_ACCOUNT_BC_ID"],
                                     DESIGNATION_CODE_TO = dataUpload.Columns.Contains("DESIGNATION_CODE_TO") ? null : order["DESIGNATION_CODE_TO"],
                                     REF1_TO = dataUpload.Columns.Contains("REF2_TO") ? null : order["REF1_TO"],
                                     REF2_TO = dataUpload.Columns.Contains("REF2_TO") ? null : order["REF2_TO"],
                                     CATEGORY = dataUpload.Columns.Contains("CATEGORY") ? null : order["CATEGORY"],
                                     SUB_CATEGORY = dataUpload.Columns.Contains("SUB_CATEGORY") ? null : order["SUB_CATEGORY"],
                                     ISSUE = dataUpload.Columns.Contains("ISSUE") ? null : order["ISSUE"],
                                     BAN = dataUpload.Columns.Contains("BAN") ? null : order["BAN"],
                                     PRIM_RESOURCE = dataUpload.Columns.Contains("PRIM_RESOURCE") ? null : order["PRIM_RESOURCE"],
                                     DETAIL = dataUpload.Columns.Contains("SR_DETAILS") ? null : order["SR_DETAILS"],
                                     SR_DIVISION = dataUpload.Columns.Contains("SR_DIVISION") ? null : order["SR_DIVISION"],
                                     SR_OWNER = dataUpload.Columns.Contains("SR_OWNER") ? null : order["SR_OWNER"],

                                 }).ToList();


                    dataMapping = ReportService.ConvertListToDatatable(query);

                    #region set index column
                    // mapping compare column name like mis SR_DETAILS	Detail

                    //string[] oldColumnName = new string[] { "SR_NO", "COMP_CODE_1", "BAN_12", "CUSTOMER_NAME_1", "COMP_CODE_2", "BAN_21", "CUSTOMER_NAME_2", "DEPOSITE_DATE", "PAY_AMOUNT", "SR_OPEN_DATE", "ACCOUNT_TYPE_1", "AR_BALANCE_11", "BEN_STATUS_1", "IDENT_1", "CONV_IND_1", "CONV_CODE_1", "T_FORM_ACCOUNT_BC_ID", "ACCOUNT_TYPE_2", "AR_BALANCE_2", "BEN_STATUS_2", "IDENT_2", "CONV_IND_2", "CONV_CODE_2", "T_TO_ACCOUNT_BC_ID", "SR_DETAILS" };
                    //string[] newColumnName = new string[] { "REQUEST_NO", "PAID_FROM", "ACCOUNT_FROM", "CUSTOMER_NAME", "PAID_TO", "ACCOUNT_TO", "CUSTOMER_NAME_TO", "DEPOSIT_DATE", "AMOUNT", "CREATE_DATE", "ACCOUNT_TYPE_FROM", "AR_BALANCE_FROM", "BEN_ACCOUNT_STATUS_FROM", "IDENTIFICATION_FROM", "CONVERGENCE_INDICATER_FROM", "CONVERGENCE_CODE_FROM", "ACCOUNT_BC_ID_FROM", "ACCOUNT_TYPE_TO", "AR_BALANCE_TO", "BEN_ACCOUNT_STATUS_TO", "IDENTIFICATION_TO", "CONVERGENCE_INDICATER_TO", "CONVERGENCE_CODE_TO", "ACCOUNT_BC_ID_TO" , "DETAIL" };


                    //foreach (DataColumn item in dataUpload.Columns)
                    //{

                    //    var existvalue = oldColumnName.ToList().Where(m => m == item.ColumnName).FirstOrDefault();
                    //    if (existvalue != null)
                    //    {
                    //        int keyIndex = Array.IndexOf(oldColumnName.ToArray(), item.ColumnName);
                    //        if (keyIndex > -1)
                    //        {
                    //            item.ColumnName = newColumnName[keyIndex];

                    //        }
                    //    }
                    //}


                    //dataUpload.Columns["RF_ID"].SetOrdinal(0);
                    //dataUpload.Columns["REQUEST_NO"].SetOrdinal(1);
                    //dataUpload.Columns["DOC_NO"].SetOrdinal(2);
                    //dataUpload.Columns["REF_NO"].SetOrdinal(3);
                    //dataUpload.Columns["PAID_FROM"].SetOrdinal(4);
                    //dataUpload.Columns["ACCOUNT_FROM"].SetOrdinal(5);
                    //dataUpload.Columns["CUSTOMER_NAME"].SetOrdinal(6);
                    //dataUpload.Columns["PAID_TO"].SetOrdinal(7);
                    //dataUpload.Columns["ACCOUNT_TO"].SetOrdinal(8);
                    //dataUpload.Columns["CUSTOMER_NAME_TO"].SetOrdinal(9);
                    //dataUpload.Columns["DEPOSIT_DATE"].SetOrdinal(10);
                    //dataUpload.Columns["SOURCE_ID"].SetOrdinal(11);
                    //dataUpload.Columns["AMOUNT"].SetOrdinal(12);
                    //dataUpload.Columns["RECEIPT_NO"].SetOrdinal(13);
                    //dataUpload.Columns["REASON_ID"].SetOrdinal(14);
                    //dataUpload.Columns["REASON_REFUND"].SetOrdinal(15);
                    //dataUpload.Columns["ADDRESS"].SetOrdinal(16);
                    //dataUpload.Columns["EXPLANATION"].SetOrdinal(17);
                    //dataUpload.Columns["REQEST_BY"].SetOrdinal(18);
                    //dataUpload.Columns["CREATE_DATE"].SetOrdinal(19);
                    //dataUpload.Columns["COMPLETE_BY"].SetOrdinal(20);
                    //dataUpload.Columns["COMPLETE_DATE"].SetOrdinal(21);
                    //dataUpload.Columns["DUE_DATE"].SetOrdinal(22);
                    //dataUpload.Columns["DOC_STATUS"].SetOrdinal(23);
                    //dataUpload.Columns["ACCOUNT_TYPE_FROM"].SetOrdinal(24);
                    //dataUpload.Columns["AR_BALANCE_FROM"].SetOrdinal(25);
                    //dataUpload.Columns["BEN_ACCOUNT_STATUS_FROM"].SetOrdinal(26);
                    //dataUpload.Columns["IDENTIFICATION_FROM"].SetOrdinal(27);
                    //dataUpload.Columns["CONVERGENCE_INDICATER_FROM"].SetOrdinal(28);
                    //dataUpload.Columns["CONVERGENCE_CODE_FROM"].SetOrdinal(29);
                    //dataUpload.Columns["ACCOUNT_BC_ID_FROM"].SetOrdinal(30);
                    //dataUpload.Columns["DESIGNATION_CODE_FROM"].SetOrdinal(31);
                    //dataUpload.Columns["REF1_FROM"].SetOrdinal(32);
                    //dataUpload.Columns["REF2_FROM"].SetOrdinal(33);
                    //dataUpload.Columns["ACCOUNT_TYPE_TO"].SetOrdinal(34);
                    //dataUpload.Columns["AR_BALANCE_TO"].SetOrdinal(35);
                    //dataUpload.Columns["BEN_ACCOUNT_STATUS_TO"].SetOrdinal(36);
                    //dataUpload.Columns["IDENTIFICATION_TO"].SetOrdinal(37);
                    //dataUpload.Columns["CONVERGENCE_INDICATER_TO"].SetOrdinal(38);
                    //dataUpload.Columns["CONVERGENCE_CODE_TO"].SetOrdinal(39);
                    //dataUpload.Columns["ACCOUNT_BC_ID_TO"].SetOrdinal(40);
                    //dataUpload.Columns["DESIGNATION_CODE_TO"].SetOrdinal(41);
                    //dataUpload.Columns["REF1_TO"].SetOrdinal(42);
                    //dataUpload.Columns["REF2_TO"].SetOrdinal(43);
                    //dataUpload.Columns["CATEGORY"].SetOrdinal(44);
                    //dataUpload.Columns["SUB_CATEGORY"].SetOrdinal(45);
                    //dataUpload.Columns["ISSUE"].SetOrdinal(46);
                    //dataUpload.Columns["BAN"].SetOrdinal(47);
                    //dataUpload.Columns["PRIM_RESOURCE"].SetOrdinal(48);
                    //dataUpload.Columns["DETAIL"].SetOrdinal(49);
                    //dataUpload.Columns["SR_DIVISION"].SetOrdinal(50);
                    //dataUpload.Columns["SR_OWNER"].SetOrdinal(51);


                    #endregion






                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dataMapping;
        }

        private static DataTable GenerateFormatFieldData(DataTable data)
        {


            DataTable dataUpload = new DataTable();
            DataTable dataMapping = new DataTable();
            try
            {
                if (data.Rows.Count > 0)
                {
                    dataUpload = data.AsEnumerable().CopyToDataTable();

                    dataUpload.Columns.Add("RF_ID");
                    dataUpload.Columns.Add("DOC_NO");
                    dataUpload.Columns.Add("REF_NO");
                    dataUpload.Columns.Add("REASON_ID");
                    dataUpload.Columns.Add("REASON_REFUND");
                    dataUpload.Columns.Add("ADDRESS");
                    dataUpload.Columns.Add("EXPLANATION");
                    dataUpload.Columns.Add("REQEST_BY");
                    dataUpload.Columns.Add("COMPLETE_BY");
                    dataUpload.Columns.Add("COMPLETE_DATE", typeof(DateTime));
                    dataUpload.Columns.Add("DUE_DATE", typeof(DateTime));
                    dataUpload.Columns.Add("DOC_STATUS");
                    dataUpload.Columns.Add("DESIGNATION_CODE_FROM");
                    dataUpload.Columns.Add("REF1_FROM");
                    dataUpload.Columns.Add("REF2_FROM");
                    dataUpload.Columns.Add("DESIGNATION_CODE_TO");
                    dataUpload.Columns.Add("REF1_TO");
                    dataUpload.Columns.Add("REF2_TO");


                    //DataTable dtCol = new DataTable();
                    //foreach (DataColumn item in dataUpload.Columns)
                    //{
                    //    dtCol.Columns.Add(item.ColumnName);
                    //    dtCol.Columns[item.ColumnName].DataType = System.Type.GetType(item.DataType.FullName);
                    //}
                    var query = (from order in dataUpload.AsEnumerable()
                                 select new
                                 {
                                     RF_ID = order["RF_ID"],
                                     REQUEST_NO = order["SR_NO"],
                                     DOC_NO = order["DOC_NO"],
                                     REF_NO = order["REF_NO"],
                                     PAID_FROM = order["COMP_CODE_1"],
                                     ACCOUNT_FROM = order["BAN_12"],
                                     CUSTOMER_NAME = order["CUSTOMER_NAME_1"],
                                     PAID_TO = order["COMP_CODE_2"],
                                     ACCOUNT_TO = order["BAN_21"],
                                     CUSTOMER_NAME_TO = order["CUSTOMER_NAME_2"],
                                     //DEPOSIT_DATE = order["DEPOSITE_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("DEPOSITE_DATE"),
                                     DEPOSIT_DATE = order["DEPOSITE_DATE"] == System.DBNull.Value ? (DateTime?)null : order["DEPOSITE_DATE"],
                                     SOURCE_ID = order["SOURCE_ID"],
                                     AMOUNT = order["PAY_AMOUNT"] == System.DBNull.Value ? 0 : order.Field<double>("PAY_AMOUNT"),
                                     RECEIPT_NO = order["RECEIPT_NO"],
                                     REASON_ID = order["REASON_ID"],
                                     REASON_REFUND = order["REASON_REFUND"],
                                     ADDRESS = order["ADDRESS"],
                                     EXPLANATION = order["EXPLANATION"],
                                     REQEST_BY = order["REQEST_BY"],
                                     //CREATE_DATE = order["SR_OPEN_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("SR_OPEN_DATE"),
                                     CREATE_DATE = order["SR_OPEN_DATE"] == System.DBNull.Value ? (DateTime?)null : order["SR_OPEN_DATE"],
                                     COMPLETE_BY = order["COMPLETE_BY"],
                                     //COMPLETE_DATE = order["COMPLETE_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("COMPLETE_DATE"),
                                     //DUE_DATE = order["DUE_DATE"] == System.DBNull.Value ? (DateTime?)null : order.Field<DateTime>("DUE_DATE"),
                                     COMPLETE_DATE = order["COMPLETE_DATE"] == System.DBNull.Value ? (DateTime?)null : order["COMPLETE_DATE"],
                                     DUE_DATE = order["DUE_DATE"] == System.DBNull.Value ? (DateTime?)null : order["DUE_DATE"],
                                     DOC_STATUS = order["DOC_STATUS"],
                                     ACCOUNT_TYPE_FROM = order["ACCOUNT_TYPE_1"],
                                     AR_BALANCE_FROM = order["AR_BALANCE_11"] == System.DBNull.Value ? 0 : order.Field<double>("AR_BALANCE_11"),
                                     BEN_ACCOUNT_STATUS_FROM = order["BEN_STATUS_1"],
                                     IDENTIFICATION_FROM = order["IDENT_1"],
                                     CONVERGENCE_INDICATER_FROM = order["CONV_IND_1"],
                                     CONVERGENCE_CODE_FROM = order["CONV_CODE_1"],
                                     ACCOUNT_BC_ID_FROM = order["T_FORM_ACCOUNT_BC_ID"],
                                     DESIGNATION_CODE_FROM = order["DESIGNATION_CODE_FROM"],
                                     REF1_FROM = order["REF1_FROM"],
                                     REF2_FROM = order["REF2_FROM"],
                                     ACCOUNT_TYPE_TO = order["ACCOUNT_TYPE_2"],
                                     AR_BALANCE_TO = order["AR_BALANCE_2"] == System.DBNull.Value ? 0 : order.Field<double>("AR_BALANCE_2"),
                                     BEN_ACCOUNT_STATUS_TO = order["BEN_STATUS_2"],
                                     IDENTIFICATION_TO = order["IDENT_2"],
                                     CONVERGENCE_INDICATER_TO = order["CONV_IND_2"],
                                     CONVERGENCE_CODE_TO = order["CONV_CODE_2"],
                                     ACCOUNT_BC_ID_TO = order["T_TO_ACCOUNT_BC_ID"],
                                     DESIGNATION_CODE_TO = order["DESIGNATION_CODE_TO"],
                                     REF1_TO = order["REF1_TO"],
                                     REF2_TO = order["REF2_TO"],
                                     CATEGORY = order["CATEGORY"],
                                     SUB_CATEGORY = order["SUB_CATEGORY"],
                                     ISSUE = order["ISSUE"],
                                     BAN = order["BAN"],
                                     PRIM_RESOURCE = order["PRIM_RESOURCE"],
                                     DETAIL = order["SR_DETAILS"],
                                     SR_DIVISION = order["SR_DIVISION"],
                                     SR_OWNER = order["SR_OWNER"],

                                 }).ToList();


                    dataMapping = ReportService.ConvertListToDatatable(query);



                    #region set index column
                    // mapping compare column name like mis SR_DETAILS	Detail

                    //string[] oldColumnName = new string[] { "SR_NO", "COMP_CODE_1", "BAN_12", "CUSTOMER_NAME_1", "COMP_CODE_2", "BAN_21", "CUSTOMER_NAME_2", "DEPOSITE_DATE", "PAY_AMOUNT", "SR_OPEN_DATE", "ACCOUNT_TYPE_1", "AR_BALANCE_11", "BEN_STATUS_1", "IDENT_1", "CONV_IND_1", "CONV_CODE_1", "T_FORM_ACCOUNT_BC_ID", "ACCOUNT_TYPE_2", "AR_BALANCE_2", "BEN_STATUS_2", "IDENT_2", "CONV_IND_2", "CONV_CODE_2", "T_TO_ACCOUNT_BC_ID", "SR_DETAILS" };
                    //string[] newColumnName = new string[] { "REQUEST_NO", "PAID_FROM", "ACCOUNT_FROM", "CUSTOMER_NAME", "PAID_TO", "ACCOUNT_TO", "CUSTOMER_NAME_TO", "DEPOSIT_DATE", "AMOUNT", "CREATE_DATE", "ACCOUNT_TYPE_FROM", "AR_BALANCE_FROM", "BEN_ACCOUNT_STATUS_FROM", "IDENTIFICATION_FROM", "CONVERGENCE_INDICATER_FROM", "CONVERGENCE_CODE_FROM", "ACCOUNT_BC_ID_FROM", "ACCOUNT_TYPE_TO", "AR_BALANCE_TO", "BEN_ACCOUNT_STATUS_TO", "IDENTIFICATION_TO", "CONVERGENCE_INDICATER_TO", "CONVERGENCE_CODE_TO", "ACCOUNT_BC_ID_TO" , "DETAIL" };


                    //foreach (DataColumn item in dataUpload.Columns)
                    //{

                    //    var existvalue = oldColumnName.ToList().Where(m => m == item.ColumnName).FirstOrDefault();
                    //    if (existvalue != null)
                    //    {
                    //        int keyIndex = Array.IndexOf(oldColumnName.ToArray(), item.ColumnName);
                    //        if (keyIndex > -1)
                    //        {
                    //            item.ColumnName = newColumnName[keyIndex];

                    //        }
                    //    }
                    //}


                    //dataUpload.Columns["RF_ID"].SetOrdinal(0);
                    //dataUpload.Columns["REQUEST_NO"].SetOrdinal(1);
                    //dataUpload.Columns["DOC_NO"].SetOrdinal(2);
                    //dataUpload.Columns["REF_NO"].SetOrdinal(3);
                    //dataUpload.Columns["PAID_FROM"].SetOrdinal(4);
                    //dataUpload.Columns["ACCOUNT_FROM"].SetOrdinal(5);
                    //dataUpload.Columns["CUSTOMER_NAME"].SetOrdinal(6);
                    //dataUpload.Columns["PAID_TO"].SetOrdinal(7);
                    //dataUpload.Columns["ACCOUNT_TO"].SetOrdinal(8);
                    //dataUpload.Columns["CUSTOMER_NAME_TO"].SetOrdinal(9);
                    //dataUpload.Columns["DEPOSIT_DATE"].SetOrdinal(10);
                    //dataUpload.Columns["SOURCE_ID"].SetOrdinal(11);
                    //dataUpload.Columns["AMOUNT"].SetOrdinal(12);
                    //dataUpload.Columns["RECEIPT_NO"].SetOrdinal(13);
                    //dataUpload.Columns["REASON_ID"].SetOrdinal(14);
                    //dataUpload.Columns["REASON_REFUND"].SetOrdinal(15);
                    //dataUpload.Columns["ADDRESS"].SetOrdinal(16);
                    //dataUpload.Columns["EXPLANATION"].SetOrdinal(17);
                    //dataUpload.Columns["REQEST_BY"].SetOrdinal(18);
                    //dataUpload.Columns["CREATE_DATE"].SetOrdinal(19);
                    //dataUpload.Columns["COMPLETE_BY"].SetOrdinal(20);
                    //dataUpload.Columns["COMPLETE_DATE"].SetOrdinal(21);
                    //dataUpload.Columns["DUE_DATE"].SetOrdinal(22);
                    //dataUpload.Columns["DOC_STATUS"].SetOrdinal(23);
                    //dataUpload.Columns["ACCOUNT_TYPE_FROM"].SetOrdinal(24);
                    //dataUpload.Columns["AR_BALANCE_FROM"].SetOrdinal(25);
                    //dataUpload.Columns["BEN_ACCOUNT_STATUS_FROM"].SetOrdinal(26);
                    //dataUpload.Columns["IDENTIFICATION_FROM"].SetOrdinal(27);
                    //dataUpload.Columns["CONVERGENCE_INDICATER_FROM"].SetOrdinal(28);
                    //dataUpload.Columns["CONVERGENCE_CODE_FROM"].SetOrdinal(29);
                    //dataUpload.Columns["ACCOUNT_BC_ID_FROM"].SetOrdinal(30);
                    //dataUpload.Columns["DESIGNATION_CODE_FROM"].SetOrdinal(31);
                    //dataUpload.Columns["REF1_FROM"].SetOrdinal(32);
                    //dataUpload.Columns["REF2_FROM"].SetOrdinal(33);
                    //dataUpload.Columns["ACCOUNT_TYPE_TO"].SetOrdinal(34);
                    //dataUpload.Columns["AR_BALANCE_TO"].SetOrdinal(35);
                    //dataUpload.Columns["BEN_ACCOUNT_STATUS_TO"].SetOrdinal(36);
                    //dataUpload.Columns["IDENTIFICATION_TO"].SetOrdinal(37);
                    //dataUpload.Columns["CONVERGENCE_INDICATER_TO"].SetOrdinal(38);
                    //dataUpload.Columns["CONVERGENCE_CODE_TO"].SetOrdinal(39);
                    //dataUpload.Columns["ACCOUNT_BC_ID_TO"].SetOrdinal(40);
                    //dataUpload.Columns["DESIGNATION_CODE_TO"].SetOrdinal(41);
                    //dataUpload.Columns["REF1_TO"].SetOrdinal(42);
                    //dataUpload.Columns["REF2_TO"].SetOrdinal(43);
                    //dataUpload.Columns["CATEGORY"].SetOrdinal(44);
                    //dataUpload.Columns["SUB_CATEGORY"].SetOrdinal(45);
                    //dataUpload.Columns["ISSUE"].SetOrdinal(46);
                    //dataUpload.Columns["BAN"].SetOrdinal(47);
                    //dataUpload.Columns["PRIM_RESOURCE"].SetOrdinal(48);
                    //dataUpload.Columns["DETAIL"].SetOrdinal(49);
                    //dataUpload.Columns["SR_DIVISION"].SetOrdinal(50);
                    //dataUpload.Columns["SR_OWNER"].SetOrdinal(51);


                    #endregion






                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dataMapping;
        }

        public static DataSet InitialDataFormMappingViewModel(AdjustrefundUploadViewModel formData)
        {
            DataSet getDataset = new DataSet();
            try
            {
                #region reading excel multi files
                DataSet ds = new DataSet();

                ValidationWithReturnResult<DataTable> resultDataTable = new ValidationWithReturnResult<DataTable>();


                int i = 1;
                string sheetTemp = string.Empty;// Create Temporarily  testing
                bool checkfileExist = false;
                if (formData.AttachmentList.Count ==1)
                {
                    checkfileExist = true;
                }
                foreach (var item in formData.AttachmentList)
                {

                    //string fileName = "";
                    // string sheetName = "SheetFile" + i.ToString();
                    //string fileNameIndex = "";
                    var sheetModel = item.SheetNameExcel;//formData.GetType().GetProperty(sheetName).GetValue(formData);


                    string tempFilePath = ConfigurationManager.AppSettings["TempFilePath"];
                    tempFilePath = System.Web.HttpContext.Current.Server.MapPath(tempFilePath);

                    string fileUniqueKey = AttachmentService.GetFileUniqueKey();
                    string savedFileName = item.SavedFileName;// fileUniqueKey + "_" + item.FileName;


                    //DataTable dtFile = new DataTable();
                    //resultDataTable = ConvertExcelToDataTable(tempFilePath, savedFileName, Convert.ToString(sheetModel));

                    #region reading excel
                    string fullPath = Path.Combine(tempFilePath, savedFileName);

                    string[] sSheetNameList;
                    string sConnection;
                    DataTable dtTablesList;
                    OleDbCommand oleExcelCommand;
                    OleDbConnection oleExcelConnection;


                    sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPath + @";Extended Properties=""Excel 12.0;HDR=Yes;IMEX=1;""";

                    oleExcelConnection = new OleDbConnection(sConnection);
                    if (oleExcelConnection.State == ConnectionState.Open)
                    {
                        oleExcelConnection.Close();
                    }
                    oleExcelConnection.Open();

                    //dtTablesList = oleExcelConnection.GetSchema("Tables");
                    //// Get the data table containg the schema guid.
                    dtTablesList = oleExcelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                    List<string> listSheet = new List<string>();
                    if (string.IsNullOrEmpty(sheetModel))
                    {
                        foreach (DataRow drSheet in dtTablesList.Rows)
                        {
                            if (drSheet["TABLE_NAME"].ToString().Contains("$"))//checks whether row contains '_xlnm#_FilterDatabase' or sheet name(i.e. sheet name always ends with $ sign)
                            {
                                listSheet.Add(drSheet["TABLE_NAME"].ToString());
                            }
                        }

                        if (dtTablesList.Rows.Count > 0)
                        {
                            if (listSheet.Count <= 2)
                            {
                                string sheet = listSheet[0];
                                DataTable dtFile = new DataTable();
                                sSheetNameList = new string[dtTablesList.Rows.Count];
                                oleExcelCommand = new OleDbCommand(("Select * From " + "[" + sheet + "]"), oleExcelConnection);

                                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);

                                dataAdapter.Fill(dtFile);
                                string tbName = sheet.Replace("'", "").Replace("$", "");
                                if (!ds.Tables.Contains(tbName.Trim()))
                                {
                                    dtFile.TableName = tbName.Trim();
                                }

                      //          List<DataTable> tables =ds.Tables.Cast<DataTable>().Where(t => t.Columns.Cast<DataColumn>()
                      //.All(c => columnNames.Contains(c.ColumnName)) ).Distinct().ToList();
                                ds.Tables.Add(dtFile);
                            }
                            else
                            {
                                foreach (var sheet in listSheet)
                                {
                                    DataTable dtFile = new DataTable();
                                    sSheetNameList = new string[dtTablesList.Rows.Count];
                                    oleExcelCommand = new OleDbCommand(("Select * From " + "[" + sheet + "]"), oleExcelConnection);

                                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);

                                    dataAdapter.Fill(dtFile);
                                    string tbName = sheet.Replace("'", "").Replace("$", "");
                                    
                                    if (!ds.Tables.Contains(tbName.Trim()))
                                    {
                                        dtFile.TableName = tbName.Trim();
                                    }
                                    
                                    ds.Tables.Add(dtFile);
                                }
                            }
                        }
                    }
                    else
                    {
                        string sheet = sheetModel;
                        DataTable dtFile = new DataTable();
                        sSheetNameList = new string[dtTablesList.Rows.Count];
                        oleExcelCommand = new OleDbCommand(("Select * From " + "[" + sheet + "]"), oleExcelConnection);

                        OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);

                        dataAdapter.Fill(dtFile);
                        string tbName = sheet.Replace("'", "").Replace("$", "");
                        if (!ds.Tables.Contains(tbName.Trim()))
                        {
                            dtFile.TableName = tbName.Trim();
                        }
                        ds.Tables.Add(dtFile);
                    }

                    oleExcelConnection.Close();
                    dtTablesList.Clear();
                    dtTablesList.Dispose();
                    #endregion

                }
                #endregion

                string[] sheetList = new string[] { "SQL_Results", "Verify#3", "BAN Last Adjust", "Send To Verify", "Batch Fund Transfer", "Batch Refund" };

                if (ds.Tables.Count > 0)
                {
                    string tbNameMap = string.Empty;
                    if (ds.Tables[0].TableName == "SQL_Results")
                    {
                        tbNameMap = ds.Tables[ds.Tables.Count - 1].TableName;
                    }
                    else
                    {
                        tbNameMap = ds.Tables[0].TableName;
                    }
                    var sheetName = new List<string>();
                    for (int t = 0; t < ds.Tables.Count; t++)
                    {
                        sheetName.Add(ds.Tables[t].TableName);
                    }
                    foreach (DataTable item in ds.Tables)
                    {
                        DataTable dataMerge = new DataTable();


                        if (item.TableName == "Verify#3")
                        {
                            var sh = sheetName.Where(n => sheetList.ToList().All(u => n != u)).FirstOrDefault();
                            string sheetBan = sh != null ? sh.ToString() : "Sheet1";
                            if (checkfileExist)
                            {
                                dataMerge = item.AsEnumerable().CopyToDataTable();
                            }
                            else
                            {
                                dataMerge = GenerateFormatAllMappingData(item, ds.Tables[sheetBan]).AsEnumerable().CopyToDataTable();
                            }
                            dataMerge.TableName = item.TableName;
                            getDataset.Tables.Add(dataMerge);
                        }
                        else
                        {
                            if (sheetList.ToList().Any(m => m == item.TableName))
                            {
                                if (item.TableName == "Batch Refund")
                                {
                                    dataMerge = GenerateFormatFieldData(item.AsEnumerable().CopyToDataTable());
                                    dataMerge.TableName = item.TableName;
                                    getDataset.Tables.Add(dataMerge);
                                }
                                else
                                {
                                    dataMerge = item.AsEnumerable().CopyToDataTable();
                                    dataMerge.TableName = item.TableName;
                                    getDataset.Tables.Add(dataMerge);
                                }


                            }
                        }
                    }

                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return getDataset;
        }

        private static DataTable GenerateFormatAllMappingData(DataTable dataVerify, DataTable dataLastAdjust)
        {
            try
            {
                var adjdata = (from adj in dataLastAdjust.AsEnumerable()
                               join data in dataVerify.AsEnumerable()
                               on Convert.ToDouble(adj["ACCOUNT_ID"] == DBNull.Value ? 0 : adj["ACCOUNT_ID"]) equals Convert.ToDouble(data["BAN_INCORRECT"] == DBNull.Value ? 0 : data["BAN_INCORRECT"])
                               //where dataLastAdjust.AsEnumerable().Any(a => data.Field<string>("BAN_INCORRECT") ==Convert.ToString( a.Field<double> ("ACCOUNT_ID")) )
                               select adj).CopyToDataTable();

                for (int i = 0; i < dataVerify.Rows.Count; i++)
                {

                    foreach (DataRow dr in dataVerify.Rows)
                    {
                        string ban_incorrect = dr["BAN_INCORRECT"] == DBNull.Value ? "0" : dr["BAN_INCORRECT"].ToString();
                        double ban = Convert.ToDouble(ban_incorrect);
                        var existsadj = (from m in adjdata.AsEnumerable()
                                         where Convert.ToDouble(m["ACCOUNT_ID"] == DBNull.Value ? 0 : m["ACCOUNT_ID"]) == Convert.ToDouble(ban_incorrect)
                                         select m).ToList();
                        if (existsadj.Any())
                        {
                            var adjList = existsadj.AsEnumerable().CopyToDataTable();
                            foreach (DataRow dradj in adjList.Rows)
                            {

                                dr["Last_Reason_Adjust"] = dradj[5].ToString();
                                dr["Last_Adjust_Date"] = dradj[2];
                                dr["Last_Adjust_Amount"] = Convert.ToDecimal(dradj[7]) + Convert.ToDecimal(dradj[8]);
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return dataVerify;
        }

        public byte[] SubmitFormFileMappingContent(AdjustrefundUploadViewModel formData)
        {
            byte[] filecontent = null;
            try
            {
                DataSet getDataset = new DataSet();

                getDataset = InitialDataFormMappingViewModel(formData);

                //if (_data.Any())
                //{
                //DataTable _dt = new DataTable();
                //_dt = ReportService.ToDataTable(_data);
                //dataMerge.TableName = "";
                //ds = new DataSet();
                //ds.Tables.Add(dataMerge);
                //}


                if (getDataset.Tables.Count > 0)
                {

                    filecontent = ExcelExportHelper.ExportExcelMutiSheetData(getDataset, "", false);//, columns);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return filecontent;
        }


        private static DataTable GenerateFormatAllData(DataTable dataMerge)
        {
            DataTable adjModelList = new DataTable();
            try
            {
                DataTable dt = new DataTable();

                DataSet ds = new DataSet();
                dt = dataMerge;
                #region initail Column Name
                dt.Columns.Add("SR_STATUS");

                dt.Columns.Add("T_FORM_ACCOUNT_BC_ID", typeof(double));
                dt.Columns.Add("T_TO_ACCOUNT_BC_ID", typeof(double));
                dt.Columns.Add("PYM_Designate_Code");
                dt.Columns.Add("Last_Reason_Adjust");
                dt.Columns.Add("Last_Adjust_Date");//, typeof(DateTime));
                dt.Columns.Add("Last_Adjust_Amount", typeof(double));
                dt.Columns.Add("W_off_Status");
                dt.Columns.Add("45");
                dt.Columns.Add("46");
                dt.Columns.Add("47");
                dt.Columns.Add("48");
                dt.Columns.Add("49");
                dt.Columns.Add("50");
                dt.Columns.Add("Account_Type");
                dt.Columns.Add("Identification_From_BAN");
                dt.Columns.Add("AR_Balance");
                dt.Columns.Add("Ben_Account_Status");
                dt.Columns.Add("Comapany_All");
                dt.Columns.Add("PYM_Designation_Code");
                dt.Columns.Add("Map");
                dt.Columns.Add("Result");
                dt.Columns.Add("Sum_Amount_Receipt", typeof(double));
                dt.Columns.Add("Result_AR_Balance");
                dt.Columns.Add("Company_From_BAN");
                dt.Columns.Add("Result_Map");
                dt.Columns.Add("Result_PRS_Before_Batch");
                dt.Columns.Add("Result_PRS_After_Batch");

                #endregion

                dt.Columns["SR_STATUS"].SetOrdinal(0);
                dt.Columns["T_FORM_ACCOUNT_BC_ID"].SetOrdinal(23);
                dt.Columns["T_TO_ACCOUNT_BC_ID"].SetOrdinal(33);
                dt.Columns["FileName"].SetOrdinal((dt.Columns.Count - 1));
                for (int i = 0; i < dt.Rows.Count; i++)
                {

                    var colName = (from m in dt.Columns.Cast<DataColumn>() select m.ColumnName).ToList();
                    foreach (DataRow dr in dt.Rows)
                    {

                        string existCol = colName.Where(c => c == "ACCOUNT_TYPE_1").FirstOrDefault();
                        dr["Account_Type"] = (!string.IsNullOrEmpty(existCol) ? dr["ACCOUNT_TYPE_1"].ToString() : "") == "I" ? "Yes" : "No";

                        existCol = string.Empty;
                        existCol = colName.Where(c => c == "IDENT_1").FirstOrDefault();
                        string existCol2 = string.Empty;
                        existCol2 = colName.Where(c => c == "IDENT_2").FirstOrDefault();
                        dr["Identification_From_BAN"] = (!string.IsNullOrEmpty(existCol) ? dr["IDENT_1"].ToString() : "") == (!string.IsNullOrEmpty(existCol2) ? dr["IDENT_2"].ToString() : "") ? "Yes" : "No";

                        #region AR_Balance
                        string arexistCol = string.Empty;
                        arexistCol = colName.Where(c => c == "AR_BALANCE_1").FirstOrDefault();
                        var ar_balance = !string.IsNullOrEmpty(arexistCol) ? (dr["AR_BALANCE_1"] != System.DBNull.Value ? dr["AR_BALANCE_1"] : 0) : 0;
                        //dr["AR_BALANCE_1"] = dr["AR_BALANCE_1"] != System.DBNull.Value ? dr["AR_BALANCE_1"] : 0;
                        dr["AR_Balance"] = Convert.ToDecimal(ar_balance) > -50 ? "No" : "Yes";
                        #endregion

                        #region Ben_Account_Status
                        existCol = string.Empty;
                        existCol = colName.Where(c => c == "BEN_STATUS_2").FirstOrDefault();
                        existCol2 = string.Empty;
                        existCol2 = colName.Where(c => c == "DOC_BILL_TYPE").FirstOrDefault();
                        string chk_ben = ((!string.IsNullOrEmpty(existCol) ? dr["BEN_STATUS_2"].ToString() : "") + (!string.IsNullOrEmpty(existCol2) ? dr["DOC_BILL_TYPE"].ToString() : ""));
                        dr["Ben_Account_Status"] = chk_ben == "CRG" || chk_ben == "cFR" || chk_ben == "tRG" || chk_ben == "tFR" ? "No" : "Yes";
                        #endregion

                        dr["Comapany_All"] = "Yes";
                        //dr["PYM_Designation_Code"] = "";
                        dr["Map"] = string.Concat(dr["Account_Type"].ToString(), "|", dr["Identification_From_BAN"].ToString(), "|", dr["AR_Balance"].ToString(), "|", dr["Ben_Account_Status"].ToString(), "|", dr["Comapany_All"].ToString());
                        dr["Result"] = dr["Map"].ToString() == "Yes|Yes|Yes|Yes|Yes" ? "OK" : "Fail";

                        #region Sum_Amount_Receipt
                        //BAN_1_1
                        existCol = string.Empty;
                        existCol = colName.Where(c => c == "BAN").FirstOrDefault();
                        existCol2 = string.Empty;
                        existCol2 = colName.Where(c => c == "PAY_AMOUNT").FirstOrDefault();
                        dr["Sum_Amount_Receipt"] = 0;
                        if (!string.IsNullOrEmpty(existCol) && !string.IsNullOrEmpty(existCol2))
                        {
                            if (dr["BAN"] != System.DBNull.Value)
                            {
                                double ban = Convert.ToDouble(dr["BAN"]);

                                DataTable data = new DataTable();
                                data = dt.AsEnumerable().CopyToDataTable();
                                var dtGroup = (from c in data.AsEnumerable()
                                               where Convert.ToDouble(c["BAN"] == DBNull.Value ? 0 : c["BAN"]) == ban
                                               select c).CopyToDataTable();

                                //var sum = dtGroup.AsEnumerable().Sum(s => (s.Field<double>("PAY_AMOUNT") != null ? Convert.ToDecimal(s.Field<double>("PAY_AMOUNT")) : 0));
                                var sum = dtGroup.AsEnumerable().Sum(s => Convert.ToDouble(s["PAY_AMOUNT"] == DBNull.Value ? 0 : s["PAY_AMOUNT"]));

                                if (dtGroup.Rows.Count > 0)
                                {
                                    dr["Sum_Amount_Receipt"] = sum;//results.Sum(x => x.PAY_AMOUNT);
                                }
                            }
                        }

                        #endregion

                        #region Result_AR_Balance

                        dr["Result_AR_Balance"] = (Convert.ToDecimal(ar_balance) + Convert.ToDecimal(dr["Sum_Amount_Receipt"])) == 0 ? "OK" : "Fail";

                        #endregion
                        existCol = string.Empty;
                        existCol = colName.Where(c => c == "COMP_CODE_1").FirstOrDefault();
                        existCol2 = string.Empty;
                        existCol2 = colName.Where(c => c == "COMP_CODE_2").FirstOrDefault();
                        dr["Company_From_BAN"] = (!string.IsNullOrEmpty(existCol) ? dr["COMP_CODE_1"].ToString() : "") == (!string.IsNullOrEmpty(existCol2) ? dr["COMP_CODE_2"].ToString() : "") ? "Fund Transfer" : "Refund";

                        dr["Result_Map"] = string.Concat(dr["Result"].ToString(), "|", dr["Result_AR_Balance"].ToString(), "|", dr["Company_From_BAN"].ToString());
                        #region Result_PRS_Before_Batch
                        if (dr["Result_Map"].ToString() == "OK|OK|Refund")
                        {
                            dr["Result_PRS_Before_Batch"] = "Batch Refund";
                        }
                        else if (dr["Result_Map"].ToString() == "OK|OK|Fund Transfer")
                        {
                            dr["Result_PRS_Before_Batch"] = "Batch Fund Transfer";
                        }
                        else
                        {
                            dr["Result_PRS_Before_Batch"] = "Send To Verify";
                        }
                        #endregion
                        //dr["Result_PRS_After_Batch"] = "";


                    }
                }

                adjModelList = dt;
                //dgvVerify.AllowUserToAddRows = false;
                //dgvVerify.BorderStyle = BorderStyle.None;
                //dgvVerify.DataSource = dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return adjModelList;
        }

        private static DataTable MergeAllData(DataSet ds)
        {
            DataTable adjModelList = new DataTable();
            try
            {
                DataTable dt = new DataTable();

                #region initail Column Replace Name Matching other table

                //dgvVerify.DataSource = null;
                for (int t = 0; t < ds.Tables.Count; t++)
                {
                    //int seq = 0;
                    //int cseq = 0;
                    if (ds.Tables[t].Rows.Count > 0)
                    {
                        var columnsGroup = ds.Tables[t].Columns.Cast<DataColumn>().GroupBy(c => c.ColumnName).ToList();
                        foreach (DataColumn col in ds.Tables[t].Columns)
                        {
                            if (col.ColumnName == "BAN_1")
                            {
                                col.ColumnName = "BAN";
                            }
                            if (col.ColumnName == "BAN_11")
                            {
                                col.ColumnName = "BAN_INCORRECT";
                            }
                        }
                    }

                    #region mark 

                    //for (int i = 0; i < ds.Tables[t].Columns.Count; i++)
                    //{
                    //    if (i > 0)
                    //    {
                    //        var columns = ds.Tables[t].Columns.Cast<DataColumn>()
                    //           .Where(x => x.ColumnName == ds.Tables[t].Rows[0][i].ToString()).ToList();
                    //        if (columns.Any())
                    //        {
                    //            seq++;
                    //            if (seq == 1)
                    //            {
                    //                ds.Tables[t].Columns[i].ColumnName = "BAN_INCORRECT";
                    //            }
                    //            else
                    //            {
                    //                cseq++;
                    //                var dupcolumns = ds.Tables[t].Columns.Cast<DataColumn>()
                    //          .Where(x => x.ColumnName == ds.Tables[t].Rows[0][i].ToString() + "_" + cseq.ToString()).ToList();
                    //                if (dupcolumns.Any())
                    //                {
                    //                    ds.Tables[t].Rows[0][i].ToString().Replace(ds.Tables[t].Rows[0][i].ToString() + "_" + cseq.ToString(), ds.Tables[t].Rows[0][i].ToString() + "_" + (++cseq).ToString());
                    //                    ds.Tables[t].Columns[i].ColumnName = ds.Tables[t].Rows[0][i].ToString();
                    //                }
                    //                else
                    //                {
                    //                    ds.Tables[t].Columns[i].ColumnName = ds.Tables[t].Rows[0][i].ToString() + "_" + cseq.ToString();

                    //                    cseq = 0;
                    //                }



                    //            }

                    //        }
                    //        else
                    //        {
                    //            ds.Tables[t].Columns[i].ColumnName = ds.Tables[t].Rows[0][i].ToString();
                    //        }
                    //    } }
                    #endregion
                    //dtOtpNot6.Columns[i].ColumnName = columns == null ? dtOtpNot6.Rows[0][i].ToString() : "BAN_INCORRECT";
                    /*
DataColumn colTimeSpan = new DataColumn("TimeSpanCol");
colTimeSpan.DataType = System.Type.GetType("System.TimeSpan");
myTable.Columns.Add(colTimeSpan);*/

                    //ds.Tables[t].Rows[0].Delete();
                    //ds.Tables[t].AcceptChanges();
                }
                #endregion
                int colMaxLenght = 0;
                for (int i = 1; i < ds.Tables.Count; i++)
                {
                    if (ds.Tables[colMaxLenght].Columns.Count <= ds.Tables[i].Columns.Count)
                    {
                        colMaxLenght = i;
                    }
                }

                foreach (DataColumn col in ds.Tables[colMaxLenght].Columns)
                {
                    string typeData = col.DataType.FullName;
                    col.DataType = System.Type.GetType(typeData);
                    Type dataT = col.DataType;
                    dt.Columns.Add(col.Caption);
                    if (dataT.FullName == "System.DateTime")
                    {
                        dt.Columns[col.Caption].DataType = System.Type.GetType("System.String");
                    }
                    else
                    {
                        dt.Columns[col.Caption].DataType = System.Type.GetType(typeData);
                    }
                }
                for (int i = 0; i < ds.Tables.Count; i++)
                {

                    foreach (DataRow dr in ds.Tables[i].Rows)
                    {
                        DataRow datadr = dt.NewRow();
                        foreach (DataColumn item in dt.Columns)
                        {
                            //if (i == 5 && item.Caption == "BAN_1")
                            //{
                            //    datadr[item.Caption] = dr["BAN"];
                            //}
                            //else
                            //{
                            var columns = ds.Tables[i].Columns.Cast<DataColumn>()
                           .Where(x => x.ColumnName == item.Caption).FirstOrDefault();
                            if (columns != null)
                            {
                                datadr[item.Caption] = dr[item.Caption].GetType().FullName == "System.DateTime" ? ((DateTime)(dr[item.Caption])).Date.ToShortDateString() : dr[item.Caption];
                            }
                            //}
                        }
                        dt.Rows.Add(datadr);
                    }
                }

                adjModelList = dt;

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return adjModelList;
        }



        public AdjustrefundUploadViewModel SubmitMergeFormData(AdjustrefundUploadViewModel formData)
        {
            AdjustrefundUploadViewModel adjfndModel = new AdjustrefundUploadViewModel();
            try
            {
                MVMMappingService.MoveData(formData, adjfndModel);
                if (formData.NameFormView == "AdjustrefundUploadDetail")
                {
                    adjfndModel.adjDataTable = InitialDataFormViewModel(formData).Tables["Verify#3"];
                }
                else if (formData.NameFormView == "AdjustrefundCheckAdjDetail")
                {
                    adjfndModel.adjDataTable = InitialDataFormMappingViewModel(formData).Tables["Verify#3"];
                }
                else
                {
                    adjfndModel.adjDataTable = InitialDataFormUploadViewModel(formData).ReturnResult;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return adjfndModel;
        }

        public ValidationResult SubmitForm(AdjustrefundUploadViewModel formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();
            try
            {
                ValidationWithReturnResult<DataTable> resultDataTable = new ValidationWithReturnResult<DataTable>();
                resultDataTable = InitialDataFormUploadViewModel(formData);
                if (!resultDataTable.ErrorFlag)
                {
                    formData.adjDataTable = resultDataTable.ReturnResult;

                    //'=== > server's detail
                    string server_machine = "DESKTOP-S7KNP5L\\SQLEXPRESS2008R2";//"RM-T15-SOMCHAI";
                    string server_user = "sa";
                    string server_password = "p@ssw0rd";// "True2017";
                    string server_dbname = "MIS_PAYMENT_ADJUST";// "MIS_PAYMENT";
                    string server_table = "TEMP_REFUND";

                    //'=== > sql server variables
                    sqlConn = new SqlConnection();
                    sqlDa = new SqlDataAdapter();

                    string strConn = "Data Source='" + server_machine + "';Initial Catalog= '" + server_dbname + "' ;User ID= '" + server_user + "'; Password= '" + server_password + "'";

                    //using (sqlConn = new SqlConnection())
                    //{
                    //    if (sqlConn.State == ConnectionState.Open)
                    //    {
                    //        sqlConn.Close();

                    //        //MessageBox.Show("database is connected")
                    //    }
                    //    sqlConn.ConnectionString = strConn;
                    //    sqlConn.Open();
                    //}
                    ////'=== > connect sql server


                    ////'=== > load data table to sql server
                    //bulkCopy = new SqlBulkCopy(sqlConn);
                    //bulkCopy.DestinationTableName = server_table;
                    //bulkCopy.WriteToServer(dt);

                    SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                    builder.DataSource = server_machine;   // update me
                    builder.UserID = server_user;              // update me
                    builder.Password = server_password;      // update me
                    builder.InitialCatalog = server_dbname;


                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        string strtemp =
                            "IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' " +
                 "AND TABLE_NAME = " + "'" + server_table + "'" + ")) " +
"BEGIN " +
  "DROP TABLE " + server_table +
" END " +

"ELSE  BEGIN " +

                            "CREATE TABLE [dbo].[TEMP_REFUND](" +
    "	[RF_ID] [nvarchar](40)  NULL," +
    "	[REQUEST_NO] [nvarchar](50) NULL," +
    "	[DOC_NO] [nvarchar](50) NULL," +
    "	[REF_NO] [nvarchar](50) NULL," +
    "	[PAID_FROM] [nvarchar](5) NULL," +
    "	[ACCOUNT_FROM] [nvarchar](30) NULL," +
    "	[CUSTOMER_NAME] [nvarchar](50) NULL," +
    "	[PAID_TO] [nvarchar](5) NULL," +
    "	[ACCOUNT_TO] [nvarchar](30) NULL," +
    "	[CUSTOMER_NAME_TO] [nvarchar](50) NULL," +
    "	[DEPOSIT_DATE] [datetime] NULL," +
    "	[SOURCE_ID] [nvarchar](15) NULL," +
    "	[AMOUNT] [decimal](18, 2) NULL," +
    "	[RECEIPT_NO] [nvarchar](50) NULL," +
    "	[REASON_ID] [nvarchar](50) NULL," +
    "	[REASON_REFUND] [nvarchar](50) NULL," +
    "	[ADDRESS] [nvarchar](200) NULL," +
    "	[EXPLANATION] [nvarchar](300) NULL," +
    "	[REQEST_BY] [nvarchar](15) NULL," +
    "	[CREATE_DATE] [datetime] NULL," +
    "	[COMPLETE_BY] [nvarchar](50) NULL," +
    "	[COMPLETE_DATE] [datetime] NULL," +
    "	[DUE_DATE] [datetime] NULL," +
    "	[DOC_STATUS] [nvarchar](5) NULL," +
    "	[ACCOUNT_TYPE_FROM] [char](5) NULL," +
    "	[AR_BALANCE_FROM] [decimal](18, 2) NULL," +
    "	[BEN_ACCOUNT_STATUS_FROM] [char](3) NULL," +
    "	[IDENTIFICATION_FROM] [nvarchar](50) NULL," +
    "	[CONVERGENCE_INDICATER_FROM] [nvarchar](50) NULL," +
    "	[CONVERGENCE_CODE_FROM] [nvarchar](50) NULL," +
    "	[ACCOUNT_BC_ID_FROM] [nvarchar](50) NULL," +
    "	[DESIGNATION_CODE_FROM] [nvarchar](50) NULL," +
    "	[REF1_FROM] [nvarchar](50) NULL," +
    "	[REF2_FROM] [nvarchar](50) NULL," +
    "	[ACCOUNT_TYPE_TO] [char](5) NULL," +
    "	[AR_BALANCE_TO] [decimal](18, 2) NULL," +
    "	[BEN_ACCOUNT_STATUS_TO] [char](3) NULL," +
    "	[IDENTIFICATION_TO] [nvarchar](50) NULL," +
    "	[CONVERGENCE_INDICATER_TO] [nvarchar](50) NULL," +
    "	[CONVERGENCE_CODE_TO] [nvarchar](50) NULL," +
    "	[ACCOUNT_BC_ID_TO] [nvarchar](50) NULL," +
    "	[DESIGNATION_CODE_TO] [nvarchar](50) NULL," +
    "	[REF1_TO] [nvarchar](50) NULL," +
    "	[REF2_TO] [nvarchar](50) NULL," +
    "	[CATEGORY] [nvarchar](50) NULL," +
    "	[SUB_CATEGORY] [nvarchar](50) NULL," +
    "	[ISSUE] [nvarchar](300) NULL," +
    "	[BAN] [nvarchar](50) NULL," +
    "	[PRIM_RESOURCE] [nvarchar](50) NULL," +
    "	[DETAIL] [nvarchar](max) NULL," +
    "	[SR_DIVISION] [nvarchar](300) NULL," +
    "	[SR_OWNER] [nvarchar](300) NULL" +
    ") ON [PRIMARY]" +
    " END";
                        connection.Open();
                        using (SqlCommand sqlComm = new SqlCommand(strtemp, connection))
                        {
                            sqlComm.ExecuteNonQuery();
                        }
                        connection.Close();
                        try
                        {

                            // make sure to enable triggers
                            // more on triggers in next post
                            bulkCopy =
                                new SqlBulkCopy
                                (
                                connection,
                                SqlBulkCopyOptions.TableLock |
                                SqlBulkCopyOptions.FireTriggers |
                                SqlBulkCopyOptions.UseInternalTransaction,
                                null
                                );

                            // set the destination table name
                            bulkCopy.DestinationTableName = server_table;
                            connection.Open();

                            // write the data in the "dataTable"
                            DataTable dt = new DataTable();

                            dt = formData.adjDataTable;

                            foreach (DataRow row in dt.Rows)
                            {
                                if (row[1] == System.DBNull.Value)
                                {
                                    row.Delete();

                                }
                                else
                                {
                                    if (row[18] != "REQEST_BY")
                                    {
                                        row[18] = formData.UserRequest;
                                    }
                                }
                            }
                            dt.AcceptChanges();

                            if (dt.Rows.Count > 0)
                            {
                                bulkCopy.WriteToServer(dt);
                            }
                            else
                            {
                                //dgvShowPreview.DataSource = null;
                                //btnUpload.Enabled = false;
                                //txtPathFile.Text = "";
                                //return;

                                string dropSql = "DROP TABLE " + server_dbname + ".[dbo].TEMP_REFUND";
                                //Drop table Temp
                                SqlCommand oCmd = new SqlCommand(dropSql, connection);
                                oCmd.ExecuteNonQuery();

                                result.Message = "";
                                result.ModelStateErrorList.Add(new ModelStateError("", ""));
                                result.ErrorFlag = true;
                            }
                            connection.Close();


                        }
                        catch (Exception ex)
                        {

                            //MessageBox.Show("Please close the file being uploaded. (ปิดไฟล์ที่กำลังอัพโหลด.)");
                            //dgvShowPreview.DataSource = null;
                            //btnUpload.Enabled = false;
                            //txtPathFile.Text = "";
                            string dropSql = "DROP TABLE " + server_dbname + ".[dbo].TEMP_REFUND";
                            //Drop table Temp
                            SqlCommand oCmd = new SqlCommand(dropSql, connection);
                            oCmd.ExecuteNonQuery();

                            result.Message = ex.ToString();
                            result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                            result.ErrorFlag = true;



                        }
                    }

                    builder = new SqlConnectionStringBuilder();
                    builder.DataSource = server_machine;   // update me
                    builder.UserID = server_user;              // update me
                    builder.Password = server_password;      // update me
                    builder.InitialCatalog = server_dbname;// "MIS_PAYMENT";


                    using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                    {
                        string sql = //"insert into Main (Firt Name, Last Name) values(textbox2.Text,textbox3.Text)";
                        "IF NOT EXISTS ( SELECT * FROM   [" + server_dbname + "].[dbo].[REFUND_PAYMENT_REQUSITION] " +
                        "WHERE [REQUEST_NO] IN (SELECT DISTINCT([REQUEST_NO]) FROM   [" + server_dbname + "].[dbo].[TEMP_REFUND]) ) BEGIN " +
                        "INSERT INTO  [" + server_dbname + "].[dbo].[REFUND_PAYMENT_REQUSITION] (" +
    "            [RF_ID]" +
    "      ,[REQUEST_NO]" +
    "      ,[DOC_NO]" +
    "      ,[REF_NO]" +
    "      ,[PAID_FROM]" +
    "      ,[ACCOUNT_FROM]" +
    "      ,[CUSTOMER_NAME]" +
    "      ,[PAID_TO]" +
    "      ,[ACCOUNT_TO]" +
    "      ,[CUSTOMER_NAME_TO]" +
    "      ,[DEPOSIT_DATE]" +
    "      ,[SOURCE_ID]" +
    "      ,[AMOUNT]" +
    "      ,[RECEIPT_NO]" +
    "      ,[REASON_ID]" +
    "      ,[REASON_REFUND]" +
    "      ,[ADDRESS]" +
    "      ,[EXPLANATION]" +
    "      ,[REQEST_BY]" +
    "      ,[CREATE_DATE]" +
    "      ,[COMPLETE_BY]" +
    "      ,[COMPLETE_DATE]" +
    "      ,[DUE_DATE]" +
    "      ,[DOC_STATUS]" +
    "      ,[ACCOUNT_TYPE_FROM]" +
    "      ,[AR_BALANCE_FROM]" +
    "      ,[BEN_ACCOUNT_STATUS_FROM]" +
    "      ,[IDENTIFICATION_FROM]" +
    "      ,[CONVERGENCE_INDICATER_FROM]" +
    "      ,[CONVERGENCE_CODE_FROM]" +
    "      ,[ACCOUNT_BC_ID_FROM]" +
    "      ,[DESIGNATION_CODE_FROM]" +
    "      ,[REF1_FROM]" +
    "      ,[REF2_FROM]" +
    "      ,[ACCOUNT_TYPE_TO]" +
    "      ,[AR_BALANCE_TO]" +
    "      ,[BEN_ACCOUNT_STATUS_TO]" +
    "      ,[IDENTIFICATION_TO]" +
    "      ,[CONVERGENCE_INDICATER_TO]" +
    "      ,[CONVERGENCE_CODE_TO]" +
    "      ,[ACCOUNT_BC_ID_TO]" +
    "      ,[DESIGNATION_CODE_TO]" +
    "      ,[REF1_TO]" +
    "      ,[REF2_TO]" +
    "      ,[CATEGORY]" +
    "      ,[SUB_CATEGORY]" +
    "      ,[ISSUE]" +
    "      ,[BAN]" +
    "      ,[PRIM_RESOURCE]" +
    "      ,[DETAIL]" +
    "      ,[SR_DIVISION]" +
    "      ,[SR_OWNER] )" +
    "      SELECT  " +
    "      (" +
    "        CONVERT(NVARCHAR(4),YEAR(CONVERT(DATETIME, [CREATE_DATE],120))  )" +
    "         + RIGHT([" + server_dbname + "].dbo.fn_all_replace_string (CONVERT(NVARCHAR(2),MONTH(CONVERT(DATETIME, [CREATE_DATE],120))),'0','R','R',3),2) " +
    "         + RIGHT([" + server_dbname + "].dbo.fn_all_replace_string (CONVERT(NVARCHAR(2),DAY(CONVERT(DATETIME, [CREATE_DATE],120))),'0','R','R',3),2) " +
    "         + CONVERT(NVARCHAR(2),DATEPART(MINUTe,GETDATE()))+ CONVERT(NVARCHAR(2),DATEPART(second,CONVERT(DATETIME, [CREATE_DATE],120))) " +
    "         + 'C' +  CONVERT(NVARCHAR(10), ROW_NUMBER() OVER(PARTITION BY CONVERT(DATETIME, [CREATE_DATE],120) ORDER BY [REQUEST_NO] ASC) )" +
    "         + 'ID' + [REQUEST_NO]) RF_ID " +
    "      ,[REQUEST_NO]" +
    "      ,[DOC_NO]" +
    "      ,[REF_NO]" +
    "      ,[PAID_FROM]" +
    "      ,[ACCOUNT_FROM]" +
    "      ,[CUSTOMER_NAME]" +
    "      ,[PAID_TO]" +
    "      ,[ACCOUNT_TO]" +
    "      ,[CUSTOMER_NAME_TO]" +
    "      ,CONVERT(DATETIME, [DEPOSIT_DATE],120)" +
    "      ,[SOURCE_ID]" +
    "      ,CONVERT(DECIMAL(18,2), [AMOUNT])" +
    "      ,[RECEIPT_NO]" +
    "      ,'RS09'" +
    "      ,(CASE WHEN PAID_FROM =PAID_TO THEN 'CRF-CO'" +
    "		ELSE " +
    "		CASE" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS') and PAID_TO in ('OR') THEN 'CRF-OR'" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS')and PAID_TO in ('OR')THEN	'CRF-OR'" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS')and PAID_TO in ('RF')THEN 'CRF-RF'" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS')and PAID_TO in ('RM') THEN 'CRF-RM'" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS')and PAID_TO in ('TC')THEN 	'CRF-TC'" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS')and PAID_TO in ('TD') THEN	'CRF-TD'" +
    "		WHEN PAID_FROM in('RM','RF','TI','VX','VC','TD','TS')	and PAID_TO in ('TI') THEN	'CRF-TI'" +
    "		WHEN PAID_FROM in('RM','RF','TI','TD','TS')	and PAID_TO in ('VC') THEN	'CRF-OT'" +
    "		WHEN PAID_FROM in('RM','RF','TI','TD','TS')	and PAID_TO in ('VX') THEN	'CRF-OT'" +
    "		WHEN PAID_FROM in('RM','RF','TI','TD','TS')	and PAID_TO in ('TS') THEN	'CRF-OT'" +
    "		WHEN PAID_FROM in('VX','VC','TD','TS') and PAID_TO in ('VC') THEN	'CRF-CA'" +
    "		WHEN PAID_FROM in('VX','VC','TD','TS') and PAID_TO in ('VX') THEN	'CRF-CA' " +
    "		END" +
    "		END )[REASON_REFUND]" +
    "      ,[ADDRESS]" +
    "      ,('Refer '+[REQUEST_NO]+" +
    "	  ' Refund from '+[PAID_FROM]+' Acc.ID '+[ACCOUNT_FROM]+' to '+[PAID_TO]+" +
    "	  ' Acc.ID '+ [ACCOUNT_TO]+' amount '+ CONVERT(NVARCHAR,[AMOUNT]) +' Baht SLA 7 days')[EXPLANATION]" +
    "      ,[REQEST_BY]" +
    "      ,CONVERT(DATETIME, [CREATE_DATE],120)" +
    "      ,[COMPLETE_BY]" +
    "      ,CONVERT(DATETIME, [COMPLETE_DATE],120)" +
    "      ,CONVERT(DATETIME, [DUE_DATE],120)" +
    "      ,'PR' [DOC_STATUS]" +
    "      ,CONVERT(CHAR,[ACCOUNT_TYPE_FROM])" +
    "      ,CONVERT(DECIMAL(18,2), [AR_BALANCE_FROM])" +
    "      ,[BEN_ACCOUNT_STATUS_FROM]" +
    "      ,[IDENTIFICATION_FROM]" +
    "      ,[CONVERGENCE_INDICATER_FROM]" +
    "      ,[CONVERGENCE_CODE_FROM]" +
    "      ,[ACCOUNT_BC_ID_FROM]" +
    "      ,[DESIGNATION_CODE_FROM]" +
    "      ,[REF1_FROM]" +
    "      ,[REF2_FROM]" +
    "      ,CONVERT(CHAR,[ACCOUNT_TYPE_TO])" +
    "      ,CONVERT(DECIMAL(18,2), [AR_BALANCE_TO])" +
    "      ,[BEN_ACCOUNT_STATUS_TO]" +
    "      ,[IDENTIFICATION_TO]" +
    "      ,[CONVERGENCE_INDICATER_TO]" +
    "      ,[CONVERGENCE_CODE_TO]" +
    "      ,[ACCOUNT_BC_ID_TO]" +
    "      ,[DESIGNATION_CODE_TO]" +
    "      ,[REF1_TO]" +
    "      ,[REF2_TO]" +
    "       ,[CATEGORY]" +
    "      ,[SUB_CATEGORY]" +
    "      ,[ISSUE]" +
    "      ,[BAN]" +
    "      ,[PRIM_RESOURCE]" +
    "      ,[DETAIL]" +
    "      ,[SR_DIVISION]" +
    "      ,[SR_OWNER]" +
    "  FROM [" + server_dbname + "].[dbo].TEMP_REFUND END";

                        try
                        {
                            connection.Open();
                            SqlCommand oCmd = new SqlCommand(sql, connection);
                            sqlDa.InsertCommand = oCmd;//new SqlCommand(sql, connection);
                            sqlDa.InsertCommand.ExecuteNonQuery();
                            //MessageBox.Show("Row inserted !! ");

                            string dropSql = "DROP TABLE [" + server_dbname + "].[dbo].TEMP_REFUND";
                            //Drop table Temp
                            oCmd = new SqlCommand(dropSql, connection);
                            oCmd.ExecuteNonQuery();


                        }
                        catch (Exception ex)
                        {
                            string dropSql = "DROP TABLE [" + server_dbname + "].[dbo].TEMP_REFUND";
                            //Drop table Temp
                            SqlCommand oCmd = new SqlCommand(dropSql, connection);
                            oCmd.ExecuteNonQuery();

                            result.Message = ex.ToString();
                            result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                            result.ErrorFlag = true;


                        }
                    }
                    result.Message = ResourceText.SuccessfulUpload;
                    result.MessageType = "S";
                    //MessageBox.Show("Upload completed.", "Completed", MessageBoxButtons.OK, MessageBoxIcon.None);
                    //'resetAll()
                    sqlConn.Close();
                }
                else
                {
                    result.Message = resultDataTable.Message.ToString();
                    result.ModelStateErrorList.Add(new ModelStateError("", resultDataTable.Message.ToString()));
                    result.ErrorFlag = true;
                }

            }
            catch (Exception ex)
            {

                //string dropSql = "DROP TABLE ["+ server_dbname + "].[dbo].TEMP_REFUND";
                ////Drop table Temp
                //SqlCommand oCmd = new SqlCommand(dropSql, connection);
                //oCmd.ExecuteNonQuery();

                result.Message = ex.ToString();
                result.ModelStateErrorList.Add(new ModelStateError("", ex.ToString()));
                result.ErrorFlag = true;
            }
            return result;
        }



        public static ValidationWithReturnResult<DataTable> ConvertExcelToDataTable(string filePath, string fileName, string sheet)
        {
            ValidationWithReturnResult<DataTable> result = new ValidationWithReturnResult<DataTable>();

            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            try
            {
                string fullPath = Path.Combine(filePath, fileName);

                string[] sSheetNameList;
                string sConnection;
                DataTable dtTablesList;
                OleDbCommand oleExcelCommand;
                OleDbConnection oleExcelConnection;


                sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPath + @";Extended Properties=""Excel 12.0;HDR=Yes;IMEX=1;""";

                oleExcelConnection = new OleDbConnection(sConnection);
                if (oleExcelConnection.State == ConnectionState.Open)
                {
                    oleExcelConnection.Close();
                }
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
                if (string.IsNullOrEmpty(sheet))
                {
                    if (listSheet.Any())
                    {
                        sheet = "[" + listSheet[0] + "]";
                    }
                    else
                    {
                        sheet = "[" + "Sheet1" + "$]";
                    }
                }
                else
                {
                    var shname = listSheet.Where(n => n.Contains(sheet)).FirstOrDefault();
                    if (shname != null)
                    {
                        sheet = "[" + shname + "]";
                    }
                    else
                    {
                        sheet = "['" + sheet.Trim() + "$']";
                    }

                }
                //[' Batch Refund$']
                //[' Batch Refund'$]
                if (dtTablesList.Rows.Count > 0)
                {
                    sSheetNameList = new string[dtTablesList.Rows.Count];
                    oleExcelCommand = new OleDbCommand(("Select * From " + sheet), oleExcelConnection);

                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);

                    dataAdapter.Fill(dt);


                }

                oleExcelConnection.Close();
                dtTablesList.Clear();
                dtTablesList.Dispose();

                result.ReturnResult = dt;//ds;
            }
            catch (Exception ex)
            {
                result.ErrorFlag = true;
                result.Message = ex.Message;

                AppLogService.Log(ex.ToString(), "");
            }

            return result;
        }


        public ValidationWithReturnResult<DataSet> ConvertExcelToDataSet(string filePath, string fileName, string sheet)
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

                //sConnection = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fullPath + ";Extended Properties=Excel 12.0;Persist Security Info=False";

                //sConnection = @"Provider = Microsoft.Jet.OLEDB.4.0; Data Source = " + fullPath + "; Extended Properties =\"Excel 8.0;HDR=Yes;IMEX=1\"";
                //sConnection = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                // case".xls": //Excel 97-03.
                //    conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                //break;
                //case".xlsx": //Excel 07 and above.
                //    conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                //break;
                /*
                 * // Connect EXCEL sheet with OLEDB using connection string
        // if the File extension is .XLS using below connection string
        //In following sample 'szFilePath' is the variable for filePath
        szConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;
                       "Data Source='" + szFilePath + 
                       "';Extended Properties=\"Excel 8.0;HDR=YES;\"";

        // if the File extension is .XLSX using below connection string
        szConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;
                      "Data Source='" + szFilePath + 
                      "';Extended Properties=\"Excel 12.0;HDR=YES;\"";*/
                oleExcelConnection = new OleDbConnection(sConnection);

                if (oleExcelConnection.State == ConnectionState.Open)
                {
                    oleExcelConnection.Close();
                }

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
                if (string.IsNullOrEmpty(sheet))
                {
                    if (listSheet.Any())
                    {
                        sheet = "[" + listSheet[0] + "]";
                    }
                    else
                    {
                        sheet = "[" + "Sheet1" + "$]";
                    }
                }
                else
                {
                    sheet = "[" + sheet + "$]";

                }
                if (dtTablesList.Rows.Count > 0)
                {
                    sSheetNameList = new string[dtTablesList.Rows.Count];
                    oleExcelCommand = new OleDbCommand(("Select * From " + sheet), oleExcelConnection);
                    DataTable dt = new DataTable();
                    OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleExcelCommand);

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