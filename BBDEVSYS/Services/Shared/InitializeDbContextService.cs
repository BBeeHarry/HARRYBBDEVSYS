using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Posting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Shared
{
    
    public partial class InitializeDbContextService //: DbContext
    {
        //public InitializeDbContextService()
        //    : base("name= " + ConstantVariableService.ConnStringServer)
        //{
        //}

        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    throw new UnintentionalCodeFirstException();
        //}


        #region Data CRUD
        public SqlConnection con;
        public void connection()
        {
            string constr = ConfigurationManager.ConnectionStrings[ConstantVariableService.ConnStringServer].ToString();
            con = new SqlConnection(constr);

        }
        //To Add Employee details 
        //public bool AddEmployee(T obj)
        //{

        //    connection();
        //    SqlCommand com = new SqlCommand("AddNewEmpDetails", con);
        //    com.CommandType = CommandType.StoredProcedure;
        //    com.Parameters.AddWithValue("@Name", obj.Name);
        //    com.Parameters.AddWithValue("@City", obj.City);
        //    com.Parameters.AddWithValue("@Address", obj.Address);

        //    con.Open();


        //   // string result = com.ExecuteScalar().ToString();

        //    //return result;


        //    int i = com.ExecuteNonQuery();
        //    con.Close();
        //    if (i >= 1)
        //    {

        //        return true;

        //    }
        //    else
        //    {

        //        return false;
        //    }


        //}
        //To view employee details with generic list  
        //public List<T> GetAllEmployees()
        //{
        //    connection();
        //    List<T> EmpList = new List<T>();
        //    SqlCommand com = new SqlCommand("GetEmployees", con);
        //    com.CommandType = CommandType.StoredProcedure;
        //    SqlDataAdapter da = new SqlDataAdapter(com);
        //    DataTable dt = new DataTable();
        //    con.Open();
        //    da.Fill(dt);
        //    con.Close();

        //    //Bind EmpModel generic list using LINQ  
        //    EmpList = (from DataRow dr in dt.Rows

        //               select new EmpModel()
        //               {
        //                   Empid = Convert.ToInt32(dr["Id"]),
        //                   Name = Convert.ToString(dr["Name"]),
        //                   City = Convert.ToString(dr["City"]),
        //                   Address = Convert.ToString(dr["Address"])
        //               }).ToList();


        //    return EmpList;


        //}
        ////To Update Employee details 
        //public bool UpdateEmployee(EmpModel obj)
        //{

        //    connection();
        //    SqlCommand com = new SqlCommand("UpdateEmpDetails", con);

        //    com.CommandType = CommandType.StoredProcedure;
        //    com.Parameters.AddWithValue("@EmpId", obj.Empid);
        //    com.Parameters.AddWithValue("@Name", obj.Name);
        //    com.Parameters.AddWithValue("@City", obj.City);
        //    com.Parameters.AddWithValue("@Address", obj.Address);
        //    con.Open();
        //    int i = com.ExecuteNonQuery();
        //    con.Close();
        //    if (i >= 1)
        //    {

        //        return true;

        //    }
        //    else
        //    {

        //        return false;
        //    }


        //}
        ////To delete Employee details 
        //public bool DeleteEmployee(int Id)
        //{

        //    connection();
        //    SqlCommand com = new SqlCommand("DeleteEmpById", con);

        //    com.CommandType = CommandType.StoredProcedure;
        //    com.Parameters.AddWithValue("@EmpId", Id);

        //    con.Open();
        //    int i = com.ExecuteNonQuery();
        //    con.Close();
        //    if (i >= 1)
        //    {

        //        return true;

        //    }
        //    else
        //    {

        //        return false;
        //    }


        //}
        #endregion


        public  void ExecuteQuery(string queryString, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(
                       connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public  DataSet SelectRows(string connectionString, string queryString)
        {
            using (SqlConnection connection =
                new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    queryString, connection);
                DataSet dataset = new DataSet();
                adapter.Fill(dataset);
                return dataset;
            }
        }

        public  void ConnectDB()
        {
            //1. Get the connection string
            string constr = "Data Source = HCW_T15-APATSAR; Initial Catalog = MIS_PAYMENT; user id = sa; password = True2017; MultipleActiveResultSets = true";
                //ConfigurationManager.ConnectionStrings[ConstantVariableService.ConnStringServer].ToString();

            DataTable dt = new DataTable(); //used to store the store procedure result.
            //List<MyEmployee> list = new List<MyEmployee>();  //used to store the model.

            using (SqlConnection con = new SqlConnection(constr))
            {
                using (SqlCommand cmd = new SqlCommand("sp_mis_payment_posting_timeliness", con))
                {
                    con.Open();

                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter ada = new SqlDataAdapter();
                    ada.SelectCommand = cmd;
                    ada.Fill(dt);

                    //--Convert the query result to MVC model.

                    //list = dt.AsEnumerable()
                    //    .Select(c => new MyEmployee
                    //    {
                    //        EmployeeID = c.Field<int>("EmployeeID"),
                    //        LastName = c.Field<string>("LastName"),
                    //        FirstName = c.Field<string>("FirstName")
                    //    }).ToList();
                }
            }
        }
    }

    public partial class DbConnectEntities<T> where T : DbContext
    {

        public virtual ObjectResult<PaymentPostingViewModel> GetStoredbyModel(DateTime paramDate)
        {
            var paramLst = new ObjectParameter("--fieldParam--", typeof(DateTime));

            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<PaymentPostingViewModel>("Stored", paramLst);
        }

        public List<T> Get(string procedureName, params object[] parameters)
        {
            var returnId =
            ((IObjectContextAdapter)this).ObjectContext.ExecuteStoreQuery<T>(procedureName, parameters).ToList();
            return returnId;
        }

       

    }

}