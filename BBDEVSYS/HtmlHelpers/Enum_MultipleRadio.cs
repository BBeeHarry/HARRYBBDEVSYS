using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace System.Web.Helpers {
    public static class EnumMultipleRadioHtmlHelper {
        /// <summary>
        /// Multiple radios for Enum element
        /// </summary>
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="htmlHelper">The HTML helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="labels">Surround button + name with label</param>
        /// <returns></returns>

        public static MvcHtmlString RadioButtonsFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, bool labels = true) where TModel : class {
            return RadioButtonsFor<TModel, TProperty>(htmlHelper, expression, null, null, null, labels);
        }

        /// <summary>
        /// Multiple radios for Enum element
        /// </summary>
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="htmlHelper">The HTML helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="NameBefore">Extra string BEFPRE fieldname</param>
        /// <param name="NameAfter">Extra string AFTER fieldname</param>
        /// <param name="labels">Surround button + name with label</param>
        /// <returns></returns>

        public static MvcHtmlString RadioButtonsFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, string NameBefore, string NameAfter, bool labels = true) where TModel : class {
            return RadioButtonsFor<TModel, TProperty>(htmlHelper, expression, null, NameBefore, NameAfter, labels);
        }

        /// <summary>
        /// Multiple radios for Enum element
        /// </summary>
        /// <typeparam name="TModel">Type of the model</typeparam>
        /// <typeparam name="TProperty">Type of the property</typeparam>
        /// <param name="htmlHelper">The HTML helper</param>
        /// <param name="expression">Expression</param>
        /// <param name="seperator">Seperator between items</param>
        /// <param name="labels">Surround button + name with label</param>
        /// <param name="NameBefore">Extra string BEFPRE fieldname</param>
        /// <param name="NameAfter">Extra string AFTER fieldname</param>
        /// <returns></returns>

        public static MvcHtmlString RadioButtonsFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, string seperator, string NameBefore, string NameAfter, bool labels = true) where TModel : class {
            TProperty value = htmlHelper.ViewData.Model == null
                ? default(TProperty)
                : expression.Compile()(htmlHelper.ViewData.Model);
            string selected = value == null ? String.Empty : value.ToString();
            var name = expression.ReturnType.Name;
            var radios = new List<MvcHtmlString>();
            var lister = Enum.GetValues(expression.ReturnType).GetEnumerator();
            for (var i = 0; lister.MoveNext() == true; i++) {
                var item = lister.Current;
                var fi = expression.ReturnType.GetField(item.ToString());
                var attribute = fi.GetCustomAttributes(typeof(DescriptionAttribute), true).FirstOrDefault();
                var title = attribute == null ? item.ToString() : ((DescriptionAttribute)attribute).Description;
                if (i != 0) {
                    radios.Add(MvcHtmlString.Create(seperator));
                }
                if (labels) {
                    radios.Add(MvcHtmlString.Create("<label>"));
                }
                //radios.Add(htmlHelper.RadioButtonFor(expression, item.ToString()));
                radios.Add(htmlHelper.RadioButton((NameBefore + name + NameAfter), item.ToString(), (selected == item.ToString())));
                radios.Add(MvcHtmlString.Create(title));
                if (labels) {
                    radios.Add(MvcHtmlString.Create("</label>"));
                }
            }
            return MvcHtmlString.Create(string.Concat(radios));
        }
    }
}