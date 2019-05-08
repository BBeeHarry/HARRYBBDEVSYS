using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BBDEVSYS.Services.Shared
{
    public class ConstantVariableService
    {
        public const string ConnStringServer = "MIS_PAYMENTEntities";


        //Role and Menu
        public const string RootMenuCode = "ROOT";
        public const string RoleEveryone = "Role_Everyone";
        public const string RoleSuperAdmin = "Role_Super_Admin";

        //Form state
        public const string FormStateDisplay = "DISPLAY";
        public const string FormStateCreate = "CREATE";
        public const string FormStateEdit = "EDIT";
        public const string FormStateCopy = "COPY";
        public const string FormStateDelete = "DELETE";
        public const string FormStateInvalid = "INVALID";
        public const string FormStateEvaluate = "EVALUATE";
        public const string FormStateRequest = "REQUEST";
        public const string FormStateWorkflowActivity = "WFACT";
        public const string FormStateWorkflowApprover = "WFACTAPPROVER";
        public const string FormStateWorkflowApprover1 = "WFACTAPPROVER1";
        public const string FormStateWorkflowApprover2 = "WFACTAPPROVER2";
        public const string FormStateWorkflowApprover3 = "WFACTAPPROVER3";
        public const string FormStateWorkflowApprover4 = "WFACTAPPROVER4";
        public const string FormStateWorkflowApprover5 = "WFACTAPPROVER5";
        public const string FormStateManage = "MANAGE";
        public const string FormStateList = "LIST";
        public const string FormStateClosed = "CLOSED";
        public const string FormStateResendRequest = "WFACTRESENDREQUEST";
        public const string FormStateCanceled = "CANCELLED";
        public const string FormStateDeduct = "DEDUCT";
        public const string FormStateAcknowledge = "ACKNOWLEDGE";
        public const string FormStateDivisionApprover = "DIVISIONAPPROVED";
        public const string FormStateDivisionReviewer = "DIVISIONREVIEW";
        public const string FormStateFractionApprover = "FRACTIONAPPROVED";

        //Proposal Form State
        public const string FormStateEditApproved = "EDIT_APPROVED";
        public const string FormStateResendRequestApproved = "WFACTRESENDREQUEST_APPROVED";

        //Form State PRA Group
        public const string FormStateUndo = "UNDO";
        public const string FormStateReceive = "RECEIVE";

        //Admin Workflow State
        public const string FormStatusWorkflowAdmin = "WFACTADMIN";

        //Form action
        public const string FormActionDisplay = "DISPLAY";
        public const string FormActionCreate = "CREATE";
        public const string FormActionEdit = "EDIT";
        public const string FormActionCopy = "COPY";
        public const string FormActionDelete = "DELETE";
        public const string FormActionApprove = "APPROVE";
        public const string FormActionReject = "REJECT";
        public const string FormActionRework = "REWORK";
        public const string FormActionInvalid = "INVALID";
        public const string FormActionEvaluate = "EVALUATE";
        public const string FormActionFinishEva = "FINISHEVA";
        //public const string FormActionSendEva = "SENDEVA";
        public const string FormActionSendRequest = "SENDREQUEST";
        public const string FormActionResendRequest = "RESENDREQUEST";
        public const string FormActionCancelRequest = "CANCEL";
        public const string FormActionValidate = "VALIDATE";
        public const string FormActionUpload = "UPLOAD";
        public const string FormActionClosed = "CLOSED";
        public const string FormActionSave = "SAVE";
        public const string FormActionBack = "BACK";
        public const string FormActionDeduct = "DEDUCT";
        public const string FormActionAcknowledge = "ACKNOWLEDGE";

        public const string FormActionEditApproved = "EDIT_APPROVED";
        public const string FormActionAutoApprove = "AUTOAPPROVE";

        //Email
        public const string EmailReceiverTypeTo = "TO";
        public const string EmailReceiverTypeCc = "CC";
        public const string EmailReceiverTypeBcc = "BCC";

        //Budget plan
        public const string BudgetPlanTypeMain = "MAINPLAN";
        public const string BudgetPlanTypeSub = "SUBPLAN";
        public const string BudgetPlanStatusOpen = "OPEN";
        public const string BudgetPlanStatusClose = "CLOSE";

        //Html display
        public const string HtmlNewLine = "<br />";

        //Sign Operation
        public const string Plus = "+";
        public const string Minus = "-";

        //Configuration, record status
        public const string ConfigStatusActive = "Active";
        public const string ConfigStatusInactive = "INACTIVE";
        public const string ConfigStatusCancel = "Cancel";
        public const string ConfigStatusAll = "ALL";

        //Period type
        public const string PeriodWeek = "WEEK";
        public const string PeriodMonth = "MONTH";
        public const string PeriodQuarter = "QUARTER";
        public const string PeriodYear = "YEAR";


        //Status Name
        //Form state
        public const string STATUSTYPE = "STATUS_INVOICE";

        //Team Request
        //Form state
        public const string REQUESTTEAMTYPE = "TEAM REQUEST";

        public const string ISACTIONTYPE = "IS_ACTION";
        public const string DURATIONTYPE = "DURATIONS";
        public const string CHARGETYPE = "CHARGE_TYPE";

        //Approver

        public const string APPROVERID = "01013131";

        //User Authorize

        public const string AithorizeAdmin = "Administrator";


    }
}