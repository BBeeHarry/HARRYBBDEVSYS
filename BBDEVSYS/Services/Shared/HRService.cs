using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using BBDEVSYS.Models.Shared;
using BBDEVSYS.Models.Entities;

namespace BBDEVSYS.Services.Shared
{
    public class HRService
    {
        /*
        public static List<Organization> GetOrgList(string[] orgLevel)
        {
            List<Organization> orgList = new List<Organization>();
            try
            {
                using (var context = new BBDEVSYSDB())
                {

                    for (int i = 0; i < orgLevel.Length; i++)
                    {
                        string getOrgLevel = orgLevel[i];
                        var getOrg = (from org in context.HROrgs
                                      where org.OrgLevel.Equals(getOrgLevel)
                                      select org).ToList();

                        foreach (var item in getOrg)
                        {
                            Organization orgModel = new Organization();
                            orgModel.OrgID = item.OrgID;
                            orgModel.OrgName = item.OrgName;
                            orgModel.OrgLevel = item.OrgLevel;
                            orgList.Add(orgModel);
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return orgList;
        }
        public static List<Organization> GetOrgText(string orgID)
        {
            List<Organization> orgValue = new List<Organization>();
            try
            {
                using (var context = new BBDEVSYSDB())
                {
                    var getOrg = context.HROrgs.Where(m => m.OrgID.Equals(orgID));
                    foreach (var item in getOrg)
                    {
                        Organization orgModel = new Organization();
                        orgModel.OrgID = item.OrgID;
                        orgModel.OrgName = item.OrgName;
                        orgModel.OrgLevel = item.OrgLevel;
                        orgValue.Add(orgModel);
                    }
                }

            }
            catch (Exception ex)
            {

            }

            return orgValue;
        }

        public static List<Organization> GetOrgReportLine(string orgID, bool withManager = true)
        {
            List<Organization> orgList = new List<Organization>();
            int maxLoop = 20;
            int countLoop = 0;

            try
            {
                using (var context = new BBDEVSYSDB())
                {
                    //Find report line from employee in org
                    List<Employee> managerList = new List<Employee>();
                    Employee selectedEmp = new Employee();

                    bool foundSelectedEmp = false;
                    string selectedOrgID = orgID;
                    countLoop = 0;
                    while (!foundSelectedEmp && countLoop < maxLoop)
                    {
                        var emp = (from m in context.HREmployees
                                   join n in context.HROrgs on m.OrgID equals n.OrgID
                                   join o in context.HRPositions on m.PositionID equals o.PosID
                                   where m.EmpStatus == ConstantVariableService.EmployeeStatusActive &&
                                   m.OrgID == selectedOrgID
                                   select new { Employee = m, Org = n, Position = o }).FirstOrDefault();
                        if (emp != null)
                        {
                            MVMMappingService.MoveData(emp.Employee, selectedEmp);
                            MVMMappingService.MoveData(emp.Org, selectedEmp);
                            MVMMappingService.MoveData(emp.Position, selectedEmp);

                            foundSelectedEmp = true;
                        }
                        else
                        {
                            var orgRelation = (from m in context.HROrgRelations where m.ParentOrgID == orgID select m).FirstOrDefault();
                            selectedOrgID = orgRelation.ChildOrgID;
                        }

                        countLoop++;
                    }

                    if (!string.IsNullOrEmpty(selectedEmp.EmpNo) && withManager)
                    {
                        managerList = HRService.GetEmpReportLine(selectedEmp.EmpNo);
                    }

                    //Check selected employee is manager of other org, then add to manager list
                    var checkManager = (from m in context.HREmployees where m.ManagerEmpNo == selectedEmp.EmpNo select m).FirstOrDefault();
                    if (checkManager != null)
                    {
                        managerList.Insert(0, selectedEmp);
                    }

                    Organization orgModel = new Organization();

                    var orgRelList = (from orgRel in context.HROrgRelations
                                      join orgChild in context.HROrgs on orgRel.ChildOrgID equals orgChild.OrgID
                                      join orgParent in context.HROrgs on orgRel.ParentOrgID equals orgParent.OrgID
                                      select new { OrgRel = orgRel, OrgChild = orgChild, OrgParent = orgParent }).ToList();

                    string nextOrgID = orgID;

                    countLoop = 0;
                    while (!string.IsNullOrEmpty(nextOrgID) && countLoop < maxLoop)
                    {
                        var manager = new Employee();
                        var org = orgRelList.Where(m => m.OrgRel.ChildOrgID == nextOrgID).FirstOrDefault();

                        if (org != null)
                        {
                            if (countLoop == 0) //First loop add child data
                            {
                                orgModel = new Organization();
                                orgModel.OrgID = org.OrgRel.ChildOrgID;
                                orgModel.OrgName = org.OrgRel.ChildOrgName;
                                orgModel.OrgLevel = org.OrgChild.OrgLevel;

                                //Org manager
                                manager = managerList.Where(m => m.OrgID == orgModel.OrgID).FirstOrDefault();
                                if (manager != null)
                                {
                                    orgModel.Manager = manager;
                                }
                                else //Get high manager or acting manager
                                {
                                    //If org is group or fraction level, find manager from BRB org
                                    if (string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelFraction, StringComparison.OrdinalIgnoreCase))
                                    {
                                        manager = managerList.Where(m => m.GradeCode == ConstantVariableService.HighEmpGrade && m.OrgLevel == ConstantVariableService.BRBOrgLevelFraction).FirstOrDefault();
                                    }
                                    else if (string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelGroup, StringComparison.OrdinalIgnoreCase))
                                    {
                                        manager = managerList.Where(m => m.GradeCode == ConstantVariableService.HighEmpGrade && m.OrgLevel == ConstantVariableService.BRBOrgLevelGroup).FirstOrDefault();
                                    }

                                    //Get acting manager from another Division or Part
                                    if (string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelDivision, StringComparison.OrdinalIgnoreCase) ||
                                        string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelPart, StringComparison.OrdinalIgnoreCase))
                                    {
                                        manager = managerList.Where(m => m.GradeCode != ConstantVariableService.HighEmpGrade && 
                                                                    (m.OrgLevel == ConstantVariableService.OrgLevelDivision || m.OrgLevel == ConstantVariableService.OrgLevelPart))
                                                                    .FirstOrDefault();
                                    }

                                    if (manager != null)
                                    {
                                        orgModel.Manager = manager;
                                    }
                                }

                                orgList.Add(orgModel);
                            }

                            orgModel = new Organization();
                            orgModel.OrgID = org.OrgRel.ParentOrgID;
                            orgModel.OrgName = org.OrgRel.ParentOrgName;
                            orgModel.OrgLevel = org.OrgParent.OrgLevel;

                            //Org manager
                            manager = managerList.Where(m => m.OrgID == orgModel.OrgID).FirstOrDefault();
                            if (manager != null)
                            {
                                orgModel.Manager = manager;
                            }
                            else //Get high manager or acting manager
                            {
                                //If org is group or fraction level, find manager from BRB org
                                if (string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelFraction, StringComparison.OrdinalIgnoreCase))
                                {
                                    manager = managerList.Where(m => m.GradeCode == ConstantVariableService.HighEmpGrade && m.OrgLevel == ConstantVariableService.BRBOrgLevelFraction).FirstOrDefault();
                                }
                                else if (string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelGroup, StringComparison.OrdinalIgnoreCase))
                                {
                                    manager = managerList.Where(m => m.GradeCode == ConstantVariableService.HighEmpGrade && m.OrgLevel == ConstantVariableService.BRBOrgLevelGroup).FirstOrDefault();
                                }

                                //Get acting manager from another Division or Part
                                if (string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelDivision, StringComparison.OrdinalIgnoreCase) ||
                                    string.Equals(orgModel.OrgLevel, ConstantVariableService.OrgLevelPart, StringComparison.OrdinalIgnoreCase))
                                {
                                    manager = managerList.Where(m => m.GradeCode != ConstantVariableService.HighEmpGrade &&
                                                                (m.OrgLevel == ConstantVariableService.OrgLevelDivision || m.OrgLevel == ConstantVariableService.OrgLevelPart))
                                                                .FirstOrDefault();
                                }

                                if (manager != null)
                                {
                                    orgModel.Manager = manager;
                                }
                            }

                            orgList.Add(orgModel);

                            nextOrgID = org.OrgRel.ParentOrgID;
                        }
                        else
                        {
                            nextOrgID = "";
                        }


                        countLoop++;
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return orgList;
        }

        public static List<Employee> GetEmpReportLine(string empNo)
        {
            List<Employee> reportLineList = new List<Employee>();
            Employee reportLine = new Employee();
            int maxLoop = 20;
            int countLoop = 0;

            try
            {
                using (var context = new BBDEVSYSDB())
                {
                    //var empList = (from m in context.HREmployees
                    //               join n in context.HROrgs on m.OrgID equals n.OrgID into ns
                    //               from n in ns.DefaultIfEmpty()
                    //               join o in context.HRPositions on m.PositionID equals o.PosID                                   
                    //               where m.EmpStatus == ConstantVariableService.EmployeeStatusActive
                    //               select new { Employee = m, Org = n, Position = o }).ToList();

                    var empList = (from m in context.HREmployees
                                   join n in context.HROrgs on m.OrgID equals n.OrgID
                                   join o in context.HRPositions on m.PositionID equals o.PosID into os
                                   from o in os.DefaultIfEmpty()
                                   where m.EmpStatus == ConstantVariableService.EmployeeStatusActive
                                   select new { Employee = m, Org = n, Position = o }).ToList();

                    string nextEmpNo = empNo;

                    while (!string.IsNullOrEmpty(nextEmpNo) && countLoop < maxLoop)
                    {
                        var emp = empList.Where(m => m.Employee.EmpNo == nextEmpNo).FirstOrDefault();

                        if (emp != null)
                        {
                            if (countLoop != 0)
                            {
                                reportLine = new Employee();
                                MVMMappingService.MoveData(emp.Employee, reportLine);
                                MVMMappingService.MoveData(emp.Org, reportLine);
                                if (emp.Position != null)
                                {
                                    MVMMappingService.MoveData(emp.Position, reportLine);
                                }

                                reportLineList.Add(reportLine);
                            }

                            nextEmpNo = emp.Employee.ManagerEmpNo;
                        }
                        else
                        {
                            nextEmpNo = "";
                        }

                        countLoop++;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return reportLineList;
        }

        public static List<Organization> GetOrgSaleGroup()
        {
            List<Organization> orgList = new List<Organization>();

            try
            {
                List<string> salesGroupList = new List<string>();
                var orgValueHelpList = ValueHelpService.GetValueHelp("ORGGROUPSALES");

                foreach (var org in orgValueHelpList)
                {
                    salesGroupList.Add(org.ValueKey);
                }

                using (var context = new BBDEVSYSDB())
                {
                    var hrOrgList = (from m in context.HROrgs
                                     where m.OrgLevel == ConstantVariableService.OrgLevelGroup &&
                                     salesGroupList.Any(n => n.Equals(m.OrgID))
                                     select m).OrderBy(m => m.OrgName).ToList();

                    foreach (var item in hrOrgList)
                    {
                        Organization org = new Organization();

                        MVMMappingService.MoveData(item, org);

                        orgList.Add(org);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return orgList;
        }

        public static List<Organization> GetBelowOrg(string orgID, int maxDeep = 10, bool getManager = false)
        {
            List<Organization> orgList = new List<Organization>();

            try
            {
                using (var context = new BBDEVSYSDB())
                {
                    var hrOrgList = (from m in context.HROrgs select m).ToList();
                    var orgRelationList = (from m in context.HROrgRelations select m).ToList();

                    int deep = 0;
                    var orgListTemp = HRService.GetBelowOrgFromList(orgRelationList, orgID, deep, maxDeep);

                    List<HREmployee> hrEmpList = new List<HREmployee>();
                    List<HRPosition> hrPosList = new List<HRPosition>();
                    if (getManager)
                    {
                        hrEmpList = (from m in context.HREmployees select m).ToList();
                        hrPosList = (from m in context.HRPositions select m).ToList();
                    }



                    foreach (var item in orgListTemp)
                    {
                        Organization org = new Organization();

                        org.OrgID = item.ChildOrgID;
                        org.OrgName = item.ChildOrgName;
                        org.parentOrgID = item.ParentOrgID;
                        org.parentOrgName = item.ParentOrgName;
                        org.OrgLevel = hrOrgList.Where(m => m.OrgID == org.OrgID).Select(m => m.OrgLevel).FirstOrDefault();

                        //Manager
                        if (getManager)
                        {
                            string managerOrgID = org.OrgID;
                            HREmployee manager = new HREmployee();
                            bool managerFound = false;

                            if (string.Equals(org.OrgLevel, ConstantVariableService.OrgLevelFraction, StringComparison.OrdinalIgnoreCase) ||
                               string.Equals(org.OrgLevel, ConstantVariableService.OrgLevelGroup, StringComparison.OrdinalIgnoreCase))
                            {
                                //Convert org from  org to BRB org for get manager
                                var brbOrg = HRService.ConvertOrgBRTBRB(org);
                                if (brbOrg != null)
                                {
                                    managerOrgID = brbOrg.OrgID;
                                }
                            }

                            var orgMemberList = hrEmpList.Where(m => m.OrgID == managerOrgID).ToList();
                            foreach (var emp in orgMemberList)
                            {
                                var checkManager = hrEmpList.Where(m => m.ManagerEmpNo == emp.EmpNo).FirstOrDefault();
                                if (checkManager != null)
                                {
                                    manager = emp;
                                    managerFound = true;
                                    break;
                                }
                            }

                            if (!managerFound)
                            {
                                manager = hrEmpList.Where(m => m.EmpNo.Equals(orgMemberList.Select(n => n.ManagerEmpNo).FirstOrDefault())).FirstOrDefault();
                            }

                            if (manager != null)
                            {
                                MVMMappingService.MoveData(manager, org.Manager);
                                org.Manager.PosName = hrPosList.Where(m => m.PosID == manager.PositionID).Select(m => m.PosName).FirstOrDefault();
                            }
                        }


                        orgList.Add(org);
                    }
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return orgList;
        }

        public static List<HROrgRelation> GetBelowOrgFromList(List<HROrgRelation> orgRelationList, string orgID, int deep, int maxDeep)
        {
            List<HROrgRelation> orgList = new List<HROrgRelation>();

            //int maxDeep = 10; 
            //Prevent infinity recursive
            if (deep > maxDeep)
            {
                return orgList;
            }

            try
            {
                var currentOrg = orgRelationList.Where(m => m.ChildOrgID == orgID).FirstOrDefault();
                if (currentOrg != null)
                {
                    orgList.Add(currentOrg);
                }

                var childOrgList = orgRelationList.Where(m => m.ParentOrgID == orgID).ToList();
                deep = deep + 1;
                foreach (var org in childOrgList)
                {
                    orgList.AddRange(GetBelowOrgFromList(orgRelationList, org.ChildOrgID, deep, maxDeep));
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return orgList;
        }

        public static Organization ConvertOrgBRTBRB(Organization org)
        {
            Organization brtOrg = new Organization();

            try
            {
                using (var context = new BBDEVSYSDB())
                {
                    var hrOrg = (from m in context.HROrgs where m.OrgName == org.OrgName && m.OrgID != org.OrgID select m).FirstOrDefault();
                    if (hrOrg != null)
                    {
                        brtOrg.OrgID = hrOrg.OrgID;
                        brtOrg.OrgName = hrOrg.OrgName;
                        brtOrg.OrgLevel = hrOrg.OrgLevel;
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return brtOrg;
        }

        public static Employee GetOrgManager(Organization org)
        {
            Employee manager = new Employee();

            try
            {
                using (var context = new BBDEVSYSDB())
                {
                    string managerOrgID = org.OrgID;
                    bool managerFound = false;

                    if (string.Equals(org.OrgLevel, ConstantVariableService.OrgLevelFraction, StringComparison.OrdinalIgnoreCase) ||
                       string.Equals(org.OrgLevel, ConstantVariableService.OrgLevelGroup, StringComparison.OrdinalIgnoreCase))
                    {
                        //Convert org from BRT org to BRB org for get manager
                        var brbOrg = HRService.ConvertOrgBRTBRB(org);
                        if (brbOrg != null)
                        {
                            managerOrgID = brbOrg.OrgID;
                        }
                    }

                    var hrEmpList = (from m in context.HREmployees select m).ToList();
                    var orgMemberList = hrEmpList.Where(m => m.OrgID == managerOrgID).ToList();
                    foreach (var emp in orgMemberList)
                    {
                        var checkManager = hrEmpList.Where(m => m.ManagerEmpNo == emp.EmpNo).FirstOrDefault();
                        if (checkManager != null)
                        {
                            MVMMappingService.MoveData(emp, manager);
                            managerFound = true;
                            break;
                        }
                    }

                    if (!managerFound)
                    {
                        var emp = hrEmpList.Where(m => m.EmpNo.Equals(orgMemberList.Select(n => n.ManagerEmpNo).FirstOrDefault())).FirstOrDefault();

                        MVMMappingService.MoveData(emp, manager);
                    }

                    if (manager != null)
                    {
                        manager.PosName = (from m in context.HRPositions where m.PosID == manager.PositionID select m.PosName).FirstOrDefault();
                    }
                }
                
            }
            catch (Exception ex)
            {

            }

            return manager;
        }
        */
    }
}