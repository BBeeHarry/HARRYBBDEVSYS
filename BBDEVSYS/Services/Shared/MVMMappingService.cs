using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using System.Reflection;


namespace BBDEVSYS.Services.Shared
{
    public class MVMMappingService
    {
        public static void MoveData<TSource, TTarget>(TSource source, TTarget target)
        {
            var sourceNodes = source.GetType().GetProperties();
            var targetNodes = target.GetType().GetProperties();
            foreach (var targetNode in targetNodes)
            {
                var foundNode = sourceNodes.SingleOrDefault(item => item.Name == targetNode.Name);
                if (foundNode != null)
                {
                    Object sourceObj = source.GetType().GetProperty(foundNode.Name).GetValue(source, null);
                    target.GetType().GetProperty(targetNode.Name).SetValue(target, sourceObj);
                }
            }
        }

    }

}