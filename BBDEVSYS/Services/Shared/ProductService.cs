using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.Models.Shared;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.ViewModels.Shared;
using System.Reflection;
using BBDEVSYS.Content.text;

namespace BBDEVSYS.Services.Shared
{
    public class ProductService
    {

        public static List<Product> GetProductValueHelp(string matType = "", string matCode = "")
        {
            List<Product> productList = new List<Product>();
            try
            {
                //using (var context = new BBDEVSYSDB())
                //{
                //    var valueProducts = (from m in context.InvoiceUploadGens
                //                         select m).OrderBy(m => m.InvoiceUploadCode);
                //    if (!string.IsNullOrEmpty(matType))
                //    {
                //        valueProducts = (from m in context.InvoiceUploadGens
                //                         where m.InvoiceUploadType == ConstantVariableService.ProductInvoiceUploadType
                //                         select m).OrderBy(m => m.InvoiceUploadCode);
                //    }

                //    foreach (var value in valueProducts)
                //    {
                //        Product productModel = new Product();
                //        MVMMappingService.MoveData(value, productModel);
                //        productModel.ProductNameWithCode = string.Concat(value.InvoiceUploadCode, " ", value.DescriptionTH);
                //        productList.Add(productModel);
                //    }
                //}
            }
            catch (Exception ex)
            {

            }

            return productList;
        }
        public static Product GetProductValueHelpText(string matCode)
        {
            Product productList = new Product();

            try
            {
                //using (var context = new BBDEVSYSDB())
                //{
                //    var getProduct = ProductService.GetProductValueHelp(ConstantVariableService.ProductInvoiceUploadType).Single(m => m.InvoiceUploadCode == matCode);
                //    //context.InvoiceUploadGens.Where(m => m.InvoiceUploadCode.Contains(matCode)).FirstOrDefault();
                //    if (getProduct != null)
                //    {
                //        MVMMappingService.MoveData(getProduct, productList);
                //    }
                //}
            }
            catch (Exception ex)
            {

            }

            return productList;
        }

     

    }
}