using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BBDEVSYS.Models.Shared;

namespace BBDEVSYS.ViewModels.Shared
{
    public class ReturnMessageViewModel<T>
    {
        public static readonly int SUCCESS   = 1;
        public static readonly int WARNING   = 2;
        public static readonly int FAILURE   = 3;
        public static readonly int EXCEPTION = 4;

        private int status;
        private T returnObject;
        private string message;
        
        public bool IsSuccess
        {
            get
            {
                if (status == SUCCESS) return true; else return false;
            }
        }

        public bool IsWarning
        {
            get
            {
                if (status == WARNING) return true; else return false;
            }
        }

        public bool IsFailure
        {
            get
            {
                if (status == FAILURE) return true; else return false;
            }
        }

        public ValidationResult validationResult { get; set; }

        public ReturnMessageViewModel()
        {
            this.validationResult = new ValidationResult();
        }

        public ReturnMessageViewModel(int status)
        {
            this.status = status;
            this.validationResult = new ValidationResult();
        }

        public ReturnMessageViewModel(int status, T returnObject)
        {
            this.status = status;
            this.returnObject = returnObject;
            this.validationResult = new ValidationResult();
        }
        public ReturnMessageViewModel(int status, string message)
        {
            this.status = status;
            this.message = message;

            ValidationResult vr = new ValidationResult();
            vr.Message = message;
            if (status != ReturnMessageViewModel<object>.SUCCESS)
            {
                vr.ModelStateErrorList.Add(new ModelStateError("", message));
                vr.ErrorFlag = true;
            }

            this.validationResult = vr;
        }
        public ReturnMessageViewModel(int status, string message, T returnObject)
        {
            this.status       = status;
            this.message      = message;
            this.returnObject = returnObject;

            ValidationResult vr = new ValidationResult();
            vr.Message = message;
            if (status != ReturnMessageViewModel<object>.SUCCESS)
            {
                vr.ModelStateErrorList.Add(new ModelStateError("", message));
                vr.ErrorFlag = true;
            }

            this.validationResult = vr;
        }
        public ReturnMessageViewModel(int status, string message, ValidationResult validationResult)
        {
            this.status = status;
            this.message = message;
            this.validationResult = validationResult;
        }

        public T Result() {
            return returnObject;
        }

        public string GetMessage() {
            return message;
        }
        public string GetMessageHTML()
        {
            string text = System.Net.WebUtility.HtmlEncode(message);
            text = text.Replace("\r\n", "\r");
            text = text.Replace("\n", "\r");
            text = text.Replace("\r", "<br>\r\n");
            text = text.Replace("  ", " &nbsp;");
            text = text.Replace("  ", " &nbsp;");
            return text;
        }
    }
}