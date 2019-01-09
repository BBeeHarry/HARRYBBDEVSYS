using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace BBDEVSYS.Services.Shared
{
  
    public class SettingService
    {
        //Constant Value
        private static readonly int DEFAULT_STRING_LENGHT = 10;
        public static void SetRuleForStringLength<T, TT>(AbstractValidator<T> validator)
        {
            foreach (var propertyInfoModel in typeof(TT).GetProperties().Where(m => m.PropertyType.FullName == "System.String"))
            {
                foreach (var propertyInfoViewModel in typeof(T).GetProperties().Where(p => p.Name == propertyInfoModel.Name))
                {
                    var parameterViewModel = Expression.Parameter(typeof(T), "p");
                    var propertyViewModel = Expression.Property(parameterViewModel, propertyInfoViewModel);
                    var conversionViewModel = Expression.Convert(propertyViewModel, typeof(String));
                    var lambdaViewModel = Expression.Lambda<Func<T, String>>(conversionViewModel, parameterViewModel);

                    var length = GetColumnLength<TT>(propertyInfoModel.Name);
                    var objectViewModel = Activator.CreateInstance(typeof(T));
                    validator.RuleFor(lambdaViewModel).Length(0, length ?? DEFAULT_STRING_LENGHT).WithLocalizedMessage(typeof(ValidatorMessage), "string_maxlength");
                }
            }
        }

        public static int? GetColumnLength<T>(String columnName)
        {
            int? result = null;
            //using (var context = new MIS_PAYMENTEntities())
            //{
            //    var entType = typeof(T);
            //    var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            //    var items = objectContext.MetadataWorkspace.GetItems(DataSpace.CSpace);

            //    if (items == null)
            //        return null;

            //    var q = items
            //        .Where(m => m.BuiltInTypeKind == BuiltInTypeKind.EntityType)
            //        .SelectMany(meta => ((EntityType)meta).Properties
            //        .Where(p => p.Name == columnName && p.TypeUsage.EdmType.Name == "String"));

            //    var queryResult = q.Where(p =>
            //    {
            //        var match = p.DeclaringType.Name == entType.Name;
            //        if (!match)
            //            match = entType.Name == p.DeclaringType.Name;

            //        return match;

            //    })
            //        .Select(sel => sel.TypeUsage.Facets["MaxLength"].Value)
            //        .ToList();

            //    if (queryResult.Any())
            //    {
            //        String checkMaxText = queryResult.First().ToString();
            //        if (checkMaxText.ToUpper().IndexOf("MAX") >= 0)
            //        {
            //            result = 1000;
            //        }
            //        else
            //        {
            //            result = Convert.ToInt32(queryResult.First());
            //        }
            //    }

            //    return result;
            //}
            return result;
        }

    }
}