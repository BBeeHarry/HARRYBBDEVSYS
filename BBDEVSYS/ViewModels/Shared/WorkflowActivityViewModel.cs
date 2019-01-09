using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;
using BBDEVSYS.Content.text;

namespace BBDEVSYS.ViewModels.Shared
{
    public class WorkflowActivityViewModel
    {
        public int ProcessInstanceID { get; set; }

        public string SerialNo { get; set; }

        public int CurrentStep { get; set; }

        public int NextStep { get; set; }

        [Display(Name = "EmpNo", ResourceType = typeof(ResourceText))]
        public string EmpNo { get; set; }

        [Display(Name = "ApproverName", ResourceType = typeof(ResourceText))]
        public string Name { get; set; }

        public string PositionID { get; set; }

        [Display(Name = "PositionName", ResourceType = typeof(ResourceText))]
        public string PositionName { get; set; }

        public string OrgID { get; set; }

        [Display(Name = "OrgName", ResourceType = typeof(ResourceText))]
        public string OrgName { get; set; }

        [Display(Name = "ApproverName", ResourceType = typeof(ResourceText))]
        public string ActionUser { get; set; }

        [Display(Name = "ApproverName", ResourceType = typeof(ResourceText))]
        public string ActionUserFullName { get; set; }

        [Display(Name = "ApproverAction", ResourceType = typeof(ResourceText))]
        public string Action { get; set; }

        [Display(Name = "ApproverAction", ResourceType = typeof(ResourceText))]
        public string ActionText { get; set; }

        [Display(Name = "ActionDate", ResourceType = typeof(ResourceText))]
        public DateTime ActionDate { get; set; }

        [Display(Name = "Comment", ResourceType = typeof(ResourceText))]
        public string Comment { get; set; }

        public int Sequence { get; set; }

        public int DataID { get; set; }

        public string ProcessStatus { get; set; }

        public string FormState { get; set; }
    }
}