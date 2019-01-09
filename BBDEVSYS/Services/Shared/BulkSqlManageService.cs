using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Shared
{
    public class BulkSqlManageService
    {

        public static void InsertToData(DataTable dt)
        {
            SqlTransaction transaction = null;
            try
            {
                using (var context =  new SqlConnection(@"Data Source=DESKTOP-S7KNP5L\SQLEXPRESS2008R2;persist security info=True;user id=sa;password=p@ssw0rd;initail catalog=0MIS_PAYMENT_FEE;MultipleActiveResultSets=True;App=EntityFramework"))
                {
                    
                    context.Open();

                    transaction = context.BeginTransaction();
                    using (var sqlBulkCopy = new SqlBulkCopy(context,SqlBulkCopyOptions.TableLock,transaction))
                    {
                        sqlBulkCopy.DestinationTableName = "";
                        sqlBulkCopy.ColumnMappings.Add("col_source","col_destination");
                        sqlBulkCopy.WriteToServer(dt);
                    }
                    transaction.Commit();
                    context.Close();

                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw ex;
            }

        }

    }
}