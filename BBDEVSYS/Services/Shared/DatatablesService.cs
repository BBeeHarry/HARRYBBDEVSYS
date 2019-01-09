using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using BBDEVSYS.ViewModels.Shared;
using System.Dynamic;
using System.Data;

namespace BBDEVSYS.Services.Shared
{
    public class DatatablesService
    {
     

        public static string ConvertObjectListToDatatables<T>(List<T> dataList)
        {
            JsonDatatablesViewModel result = new JsonDatatablesViewModel();
            result.data = new List<dynamic>();
            foreach (var data in dataList)
            {
                result.data.Add(ConvertObjectToDatatables<T>(data));
            }
            string resultStr = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            return resultStr;
        }

        public static dynamic ConvertObjectToDatatables<T>(T data)
        {

            List<string> resultList = new List<string>();
            var nodes = data.GetType().GetProperties();
            var result = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var node in nodes)
            {
                string objectData = GetObject(data, node.Name);
                //result[node.name] = objectData;
                result.Add(node.Name, objectData);
            }

            return result;
        }

        private static string GetObject<T>(T data, string name)
        {
            Type dataType = data.GetType();
            PropertyInfo dataInfo = dataType.GetProperty(name);
            Object dataObj = dataInfo.GetValue(data, null);
            string nameDate = name + "Date", nameDateTime = name + "DateTime";

            //Null Object
            if (dataObj == null)
                return "";

            //DataTime
            else if (dataInfo.PropertyType.Name == "DateTime" || dataInfo.PropertyType.FullName.IndexOf("DateTime") >= 0)
                return "<span class='" + nameDate + "' >" + ((DateTime)dataObj).ToString("dd MMM yyyy") + "</span><span class='" + nameDateTime + "' style='display:none;'>" + ((DateTime)dataObj).ToString("dd MMM yyyy HH:mm:ss") + "</span><input type='hidden'value='@" + ((DateTime)dataObj).ToString("dd/MM/yyyy/HH/mm/ss") + "@'/>";

            //List Object
            else if (IsGenericList(dataObj))
                return ""; //continue;

            //Default
            else
                return dataObj.ToString();
        }

        private static bool IsGenericList(Object o)
        {
            Type t = o.GetType();
            return (t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>)));
        }

        public static List<T> ToListof<T>( DataTable dt)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            var objectProperties = typeof(T).GetProperties(flags);
            var targetList = dt.AsEnumerable().Select(dataRow =>
            {
                var instanceOfT = Activator.CreateInstance<T>();

                foreach (var properties in objectProperties.Where(properties => columnNames.Contains(properties.Name) && dataRow[properties.Name] != DBNull.Value))
                {
                    properties.SetValue(instanceOfT, dataRow[properties.Name], null);
                }
                return instanceOfT;
            }).ToList();

            return targetList;
        }

    }
   



}