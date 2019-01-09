using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Script.Serialization;
using BBDEVSYS.Models.Shared;
using System.Diagnostics;
using System.ComponentModel;
using BBDEVSYS.Models.Entities;
using System.Transactions;
using System.Xml.Serialization;
using System.Linq.Expressions;

namespace BBDEVSYS.Services.Shared
{
    public class AppLogService
    {
        public static void Log(string message, params object[] logData)
        {
            try
            {
                // Get date
                Nullable<System.DateTime> LogDate = DateTime.Now;

                // Get User
                User user = UserService.GetSessionUserInfo();
                string userName = user.ADUser;

                // Get calling method name
                StackTrace stackTrace = new StackTrace();
                var callerMethod = stackTrace.GetFrame(1).GetMethod();
                var callerClass = callerMethod.DeclaringType;

                string method = callerClass.Name + "." + callerMethod.Name;

                // Dump data obj to string
                string data1 = string.Empty;
                int dataCount = 1;
                foreach (var item in logData)
                {
                    if (dataCount == 1)
                    {
                        data1 += ToXML(item) + Environment.NewLine;
                    }
                    else
                    {
                        data1 += Environment.NewLine + ToXML(item) + Environment.NewLine;
                    }

                    dataCount++;
                }

                //Save Log to database
                using (TransactionScope scope = new TransactionScope())
                {
                    //var appLog = new AppLog();
                    //appLog.LogDate = LogDate;
                    //appLog.LogBy = userName;
                    //appLog.Method = method;
                    //appLog.Message = message;
                    //appLog.Data1 = data1;

                    //using (var context = new BBDEVSYSDB())
                    //{
                    //    context.AppLogs.Add(appLog);
                    //    context.SaveChanges();
                    //}

                    //scope.Complete();
                }
            }
            catch (Exception ex)
            {
                AppLogService.Log(ex.ToString());
            }

        }

        public static string ToXML(object obj)
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(obj.GetType());
            serializer.Serialize(stringwriter, obj);
            return stringwriter.ToString();
        }
    }
}