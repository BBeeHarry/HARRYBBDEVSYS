using BBDEVSYS.Content.text;
using BBDEVSYS.Models.Entities;
using BBDEVSYS.Models.Shared;
using BBDEVSYS.Services.Invoice;
using BBDEVSYS.Services.Shared;
using BBDEVSYS.ViewModels.Invoice;
using BBDEVSYS.ViewModels.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace BBDEVSYS.Services.Abstract
{
    public abstract class AbstractControllerService<T> where T : IViewModel
    {

        public abstract string GetList();

        public abstract T GetDetail(int id);

        public abstract T NewFormData();

        public abstract ValidationResult ValidateFormData(T formData, ModelStateDictionary modelState);

        public abstract ValidationResult SaveCreate(T formData, ModelStateDictionary modelState);

        public virtual T InitialEdit(int id)
        {
            return GetDetail(id);
        }

        public abstract ValidationResult SaveEdit(T formData, ModelStateDictionary modelState);

        public virtual T InitialCopy(int fromID)
        {
            T viewModel = GetDetail(fromID);

            //Clear ID
            viewModel.ID = 0;

            return viewModel;
        }

        public virtual ValidationResult SaveCopy(T formData, ModelStateDictionary modelState)
        {
            return SaveCreate(formData, modelState);
        }

        public virtual T InitialDelete(int id)
        {
            return GetDetail(id);
        }

        public abstract ValidationResult SaveDelete(T formData, ModelStateDictionary modelState);

        public virtual T InitialCreate()
        {
            return NewFormData();
        }

        public virtual T InitialDisplay(int id)
        {
            return GetDetail(id);
        }

        public virtual T InitialDetailView(int recordKey, string formState)
        {
            T viewModel = default(T);

            if (string.Equals(formState, ConstantVariableService.FormStateDisplay, StringComparison.OrdinalIgnoreCase))
            {
                viewModel = InitialDisplay(recordKey);
                viewModel.FormState = formState;
                viewModel.FormAction = formState;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateEdit, StringComparison.OrdinalIgnoreCase))
            {
                viewModel = InitialEdit(recordKey);
                viewModel.FormState = formState;
                viewModel.FormAction = formState;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateCopy, StringComparison.OrdinalIgnoreCase))
            {
                viewModel = InitialCopy(recordKey);
                viewModel.FormState = formState;
                viewModel.FormAction = formState;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateDelete, StringComparison.OrdinalIgnoreCase))
            {
                viewModel = InitialDelete(recordKey);
                viewModel.FormState = formState;
                viewModel.FormAction = formState;
            }
            else if (string.Equals(formState, ConstantVariableService.FormStateCreate, StringComparison.OrdinalIgnoreCase))
            {
                viewModel = InitialCreate();
                viewModel.FormState = formState;
                viewModel.FormAction = formState;
            }

            return viewModel;
        }

        public virtual ValidationResult SubmitForm(T formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            if (string.Equals(formData.FormAction, ConstantVariableService.FormActionEdit, StringComparison.OrdinalIgnoreCase))
            {
                result = SaveEdit(formData, modelState);
            }
            else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionCopy, StringComparison.OrdinalIgnoreCase))
            {
                result = SaveCopy(formData, modelState);
            }
            else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionDelete, StringComparison.OrdinalIgnoreCase))
            {
                result = SaveDelete(formData, modelState);
            }
            else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionCreate, StringComparison.OrdinalIgnoreCase))
            {
                result = SaveCreate(formData, modelState);
            }
            else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionValidate, StringComparison.OrdinalIgnoreCase))
            {
                result = ValidateFormData(formData, modelState);
            }
            //else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionClosed, StringComparison.OrdinalIgnoreCase))
            //{
            //    result = SaveClose(formData, modelState);
            //}
            //else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionSendRequest, StringComparison.OrdinalIgnoreCase))
            //{
            //    result = SendRequest(formData, modelState);
            //}
            //else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionApprove, StringComparison.OrdinalIgnoreCase))

            //    result = ApproveRequest(formData, modelState);
            //}
            //else if (string.Equals(formData.FormAction, ConstantVariableService.FormActionReject, StringComparison.OrdinalIgnoreCase))
            //{
            //    result = RejectRequest(formData, modelState);
            //}
            else
            {
                result.ErrorFlag = true;
                result.MessageType = "E";
                result.Message = "Invalid action";
                result.ModelStateErrorList.Add(new ModelStateError("", "Invalid action"));
            }

            return result;
        }

        //*****************************************************************//
        //
        // Workflow method
        //
        //*****************************************************************//
        public virtual ValidationResult SaveClose(T formData, ModelStateDictionary modelState)
        {
            throw new NotImplementedException();
        }
        public virtual ValidationResult SendRequest(T formData, ModelStateDictionary modelState)
        {
            throw new NotImplementedException();
        }

        //public virtual T OpenTask(string sn, string imUser = null, bool isIM = false)
        //{
        //    T viewModel = default(T);

        //    WorkflowActivityViewModel workflowActivity = WorkflowServices.GetWorkflowActivityFromWorkItem(sn);

        //    int dataID = workflowActivity.DataID;
        //    viewModel = GetDetail(dataID);

        //    viewModel.WorkflowActivityAction = workflowActivity;

        //    int currentStep = workflowActivity.CurrentStep;

        //    viewModel.FormState = workflowActivity.FormState;
        //    viewModel.FormAction = "";

        //    //Approver detail
        //    User userInfo = UserService.GetSessionUserInfo();
        //    viewModel.WorkflowActivityAction.Name = userInfo.DisplayNameTH;

        //    return viewModel;
        //}

        //public virtual ValidationResult ApproveRequest(T formData, ModelStateDictionary modelState)
        //{
        //    ValidationResult result = new ValidationResult();
        //    WorkflowActivityStep workflowActivityStep = new WorkflowActivityStep();

        //    var dataFields = new Dictionary<string, object>();

        //    try
        //    {
        //        //Workflow step list
        //        int nextStep = formData.WorkflowActivityAction.CurrentStep + 1;
        //        using (var context = new BBDEVSYSDB())
        //        {
        //            workflowActivityStep = (from m in context.WorkflowActivitySteps
        //                                    where m.ProcessInstanceID == formData.WorkflowActivityAction.ProcessInstanceID &&
        //                                    m.Step == nextStep
        //                                    select m).FirstOrDefault();
        //        }

        //        if (workflowActivityStep != null) //Have next action user
        //        {
        //            //loop to same activity for next action user
        //            dataFields.Add("GoNextActivity", false);
        //            dataFields.Add("ActionUser", workflowActivityStep.ActionUser);
        //            dataFields.Add("CurrentStep", workflowActivityStep.Step);
        //            dataFields.Add("FormState", workflowActivityStep.FormState);

        //            formData.WorkflowActivityAction.Action = ConstantVariableService.FormActionApprove;
        //            formData.WorkflowActivityAction.NextStep = workflowActivityStep.Step; //Go text step
        //            formData.WorkflowActivityAction.ProcessStatus = ConstantVariableService.TransStatusInWorkflowProcess;
        //        }
        //        else
        //        {
        //            //No more action user then finish workflow
        //            dataFields.Add("GoNextActivity", true);

        //            formData.WorkflowActivityAction.Action = ConstantVariableService.FormActionApprove;
        //            formData.WorkflowActivityAction.NextStep = 0; //Finish workflow, no next step
        //            formData.WorkflowActivityAction.ProcessStatus = ConstantVariableService.TransStatusCompleted;
        //        }

        //        result = ActionWorkListItem(formData, modelState, dataFields);
        //    }
        //    catch (Exception ex)
        //    {

        //    }

        //    return result;
        //}

        public virtual ValidationResult RejectRequest(T formData, ModelStateDictionary modelState)
        {
            ValidationResult result = new ValidationResult();

            var dataFields = new Dictionary<string, object>();
            dataFields.Add("GoNextActivity", true);

            //formData.WorkflowActivityAction.Action = ConstantVariableService.FormActionReject;
            //formData.WorkflowActivityAction.NextStep = 0; //Finish workflow, no next step
            //formData.WorkflowActivityAction.ProcessStatus = ConstantVariableService.TransStatusCompleted;

            //result = ActionWorkListItem(formData, modelState, dataFields);

            return result;
        }


        public virtual ValidationResult UpdateDataFromAction(T formData, ModelStateDictionary modelState)
        {
            return new ValidationResult();
        }
    }
}