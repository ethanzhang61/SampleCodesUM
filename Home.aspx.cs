


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Web.Services;
using System.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net;
using System.Collections;
using System.Configuration;
using System.Web.Script.Serialization;
using LumenWorks.Framework.IO.Csv;
using System.Text.RegularExpressions;
using System.IO;
using SOMDeptBASystemUtility;

namespace SOMDeptAdministration
{
    public class PersonCategoryInfo
    {
        public int PersonDepartmentPersonCategoryId { get; set; }
        public int PersonDepartmentId { get; set; }
        public int PersonId { get; set; }
        public int PersonCategoryCD { get; set; }
        public int TitleId { get; set; }
        public int PartnerPersonId { get; set; }
        public int AssistantPersonId { get; set; }
        public string EffectiveDate { get; set; }
        public string TermDate { get; set; }
        public string Position { get; set; }
        public string FellowSpecification { get; set; }
        public string Note { get; set; }
        public Boolean DoNotFinishProgram { get; set; }
        public Boolean IsIntegratedResident { get; set; }
        public string ClinicIds { get; set; }
        public int YearGraduation { get; set; }
        public int YearOfLastGift { get; set; }
        public int TotalDonation { get; set; }
        public int DesignationCD { get; set; }
        public Boolean IsFormer { get; set; }
        public PersonCategoryInfo()
        {

        }
    }
    public class PersonAddressInfo
    {
        public int PersonAddressId { get; set; }
        public int PersonId { get; set; }
        public int AddressTypeId { get; set; }
        public string Street1 { get; set; }
        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public string PostalCode { get; set; }
        public string country { get; set; }
        public Boolean IsPreferred { get; set; }
        public Boolean IsDeleted { get; set; }
        public PersonAddressInfo()
        {

        }
    }

    public partial class Home : SOMDeptBASystemUtility.Web.BasePage //System.Web.UI.Page
    {
        public static string salutation = "{}";
        public static string usstate = "{}";
        public static string personAddressType = "{}";
        public static string sexes = "{}";
        public static string RoleList = "{}";
        public static string ClinicList = "{}";
        public static string TitleList = "{}";
        public static string DesignationList = "{}";
        public static string DegreeList = "{}";
        public static string personCategory = "{}";
        public static string FullPersonList = "{}";
        public static string FacultyList = "{}";
        public static string AssistantPersonList = "{}";

        public static string PersonList = "{}";
        public static string RolePersonList = "{}";
        public static string DonationList = "{}";
        public static string DonationCateogryList = "{}";

        public static string selectedPersonInfo = "{}";
        public static string selectedpersonCategoryInfo = "{}";
        public static string selectedDonationInfo = "{}";
        public static string loginUser;
        public static string loginPersonRole = "{}";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //personMgmt Page --"38" is the DepartmentID for the current only client department, it is extendable if more departments are using our app in the future.
                //--Display
                PersonList = SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartment(38);
                //--PersonInfo
                salutation = GetSalutation();
                sexes = GetSexes();
                FullPersonList = SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartmentCategory(38, 0);
                DegreeList = SOMDepartmentBusinessAdministrationBL.Person.GetDegreeList();
                //--PersonAddressInfo
                usstate = GetUSstate();
                personAddressType = SOMDepartmentBusinessAdministrationBL.Person.GetPersonAddressType();
                //--PersonCategoryInfo
                personCategory = GetpersonCategory();
                AssistantPersonList = SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartmentCategory(38, 4);
                TitleList = SOMDepartmentBusinessAdministrationBL.Person.GetTitleListByDepartment(38);
                ClinicList = SOMDepartmentBusinessAdministrationBL.Person.GetClinicListByDepartment(38);
                DesignationList = SOMDepartmentBusinessAdministrationBL.Person.GetDesignation();

                //PersonRoleMgmt Page
                RoleList = SOMDepartmentBusinessAdministrationBL.Person.GetRoleList();
                RolePersonList = SOMDepartmentBusinessAdministrationBL.Person.GetRolePersonListByDepartmentId(38);

                //DonationMgmt Page
                DonationList = SOMDepartmentBusinessAdministrationBL.Person.GetDonationList(1);
                DonationCateogryList = SOMDepartmentBusinessAdministrationBL.Person.GetDonationCategoryList();
                
                //Report Page
                FacultyList = SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartmentCategory(38, 1);


                loginUser = SOMDeptBASystemUtility.Utility.UserPawprint;
                loginPersonRole = SOMDepartmentBusinessAdministrationBL.Person.GetPersonPermissionByPersonId(SOMDepartmentBusinessAdministrationBL.Person.GetPersonIdByPawprint(SOMDeptBASystemUtility.Utility.UserPawprint));
                //PersonList = SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByUserDepartment(SOMDeptBASystemUtility.Utility.ActualUserPawprint);
            }
        }

        public static string GetSalutation()
        {
            return JsonConvert.SerializeObject(SOMDepartmentBusinessAdministrationBL.Person.GetPersonSalution());
        }
        public static string GetUSstate()
        {
            return JsonConvert.SerializeObject(SOMDepartmentBusinessAdministrationBL.Person.GetUsStateInfo());
        }
        public static string GetSexes()
        {
            return SOMDepartmentBusinessAdministrationBL.Person.GetSex();
        }
        public static string GetpersonCategory()
        {
            return SOMDepartmentBusinessAdministrationBL.Person.GetPersonCategory();
        }
        public static string SavePersonCategoryClinic(int PDPCId, string ClinicCDs)
        {
            string[] ClinicCDarray = ClinicCDs.Split(',');
            if (ClinicCDs != "")
            {
                for (int i = 0; i < ClinicCDarray.Length; i++)
                {
                    if (!SOMDepartmentBusinessAdministrationBL.Person.hasPersonDepartmentPersonCategoryClinic(PDPCId, Convert.ToInt32(ClinicCDarray[i])))
                    {
                        try
                        {
                            SOMDepartmentBusinessAdministrationBL.Person.InsertPersonDepartmentPersonCategoryClinic(PDPCId, Convert.ToInt32(ClinicCDarray[i]));
                        }
                        catch
                        {
                            return "Error: Fail to insert Clinics to this Person Category.";
                        }
                    }
                }
                try
                {
                    SOMDepartmentBusinessAdministrationBL.Person.DeletePersonDepartmentPersonCategoryClinic(PDPCId, ClinicCDs);
                }
                catch
                {
                    return "Error: Fail to delete Clinics for this Person Category.";
                }
                return "";
            }
            else
            {
                try
                {
                    SOMDepartmentBusinessAdministrationBL.Person.DeletePersonDepartmentPersonCategoryClinic(PDPCId, ClinicCDs);
                }
                catch
                {
                    return "Error: Fail to delete Clinics for this Person Category.";
                }
                return "";
            }


        }

        [WebMethod]
        public static string GetSelectedPersonInfo(int personId)
        {
            return SOMDepartmentBusinessAdministrationBL.Person.GetSelectedpersonInfo(personId);
        }
        [WebMethod]
        public static string GetSelectedPersonCategoryInfo(int personId)
        {
            return SOMDepartmentBusinessAdministrationBL.Person.GetSelectedpersonCategoryInfo(personId, 38);
        }
        [WebMethod]
        public static string GetSelectedPersonAddressInfo(int personId)
        {
            return SOMDepartmentBusinessAdministrationBL.Person.GetSelectedpersonAddressInfo(personId);
        }
        [WebMethod]
        public static string SearchIDXProvNumByName(string lname, string fname)
        {
            return SOMDepartmentBusinessAdministrationBL.Person.SearchIDXProvNumByName(lname, fname);
        }
        
		/**
		* Returns person List string or Error Msg string to show if the function has proceeded successfully. 
		* <p>
		* This method will return immediately, if Add/Edit person successfully or it stops at anywhere that has an error with msg. 
		* It received data from http post.
		*
		* @param  fname  
		* @param  mname 	
		* @param  lname  
		* @param  preferredName	
		* @param  personId 	 0 if it is new person
		* @param  departmentTitleId	
		* @param  SalutationId	
		* @param  EducationInfo		
		* @param  sexId		
		* @param  email			
		* @param  alteremail
		* @param  pawprint	
		* @param  EmployeeID	
		* @param  homephone	
		* @param  workphone	
		* @param  cellphone	
		* @param  PartnerPersonId	
		* @param  Note	
		* @param  IsDeceased	
		* @param  IsNonSolicit			
		* @param  AddressInfo	received from JS obj
		* @param  CategoryInfo	
		* @return  Error msg or personList string
		*/
        [WebMethod]
        public static string SavePersonInfo(string fname, string mname, string lname, string preferredName, int personId, int departmentTitleId, int SalutationId,
            string EducationInfo, int sexId, string email, string alteremail, string pawprint, string EmployeeID, string homephone, string workphone, string cellphone, 
            int PartnerPersonId, string Note, bool IsDeceased, bool IsNonSolicit, PersonAddressInfo[] AddressInfo, PersonCategoryInfo[] CategoryInfo)
        {
			//Some database tables are pre-defined, here is to adjust the Ids and datatypes received from javascript.
            int genderId = (sexId == 1 ? 2 : (sexId == 3 ? 3 : 2));
            int personDepartmentId = 0;
            pawprint = pawprint.ToLower();
                    
            if (personId == 0) // add new person
            {
                //--------------FCM DATABASE Insert person-----------
                if (SOMDepartmentBusinessAdministrationBL.Person.IsPawprintExist(pawprint) || SOMDepartmentBusinessAdministrationBL.Person.IsEmailExist(email))
                {
                    return "Error: Add new person failed, Pawprint exist or email exists."; 
                }
                int FCMindividualId = 0;
                try
                {
                    FCMindividualId = SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonToIndividual(fname, mname, lname, email, pawprint);
                    for (int i = 0; i < AddressInfo.Length; i++)
                    {
                        if (AddressInfo[i].AddressTypeId == 2)
                        {
                            SOMDepartmentBusinessAdministrationBL.PersonFCM.UpdatePersonInfo(FCMindividualId, fname, mname, lname, email, pawprint);
                        }
                    }
                }
                catch
                {
                    SOMDepartmentBusinessAdministrationBL.PersonFCM.DeleteFromIndividual(FCMindividualId);
                    return "Error: FCM DATABASE insert person and address failed.";
                }
                //----------------SOM DB Insert person------------------
                try
                {
                    personId = SOMDepartmentBusinessAdministrationBL.Person.InsertPerson(EmployeeID, genderId, sexId, IsNonSolicit, PartnerPersonId, 0, IsDeceased, true, false, Note, EducationInfo);

                    try
                    {
                        personDepartmentId = SOMDepartmentBusinessAdministrationBL.Person.InsertPersonDepartment(personId, 38);
                    }
                    catch
                    {
                        SOMDepartmentBusinessAdministrationBL.PersonFCM.DeleteFromIndividual(SOMDepartmentBusinessAdministrationBL.Person.DeletePerson(personId));
                        return "Error: SOM Person Department insert failed.";
                    }
                    try
                    {
                        SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonFCMIndividualToMapping(personId, FCMindividualId);
                    }
                    catch
                    {
                        SOMDepartmentBusinessAdministrationBL.PersonFCM.DeleteFromIndividual(SOMDepartmentBusinessAdministrationBL.Person.DeletePerson(personId));
                        return "Error: FCM individual ID mapping to SOMDBA failed.";
                    }
                    SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Name(personId, lname, fname, mname, preferredName, SalutationId, 0);
                    if (email != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.InsertPersonEmailAddress(personId, true, email.Split('@')[0], email.Split('@')[1]);
                    }
                    if (alteremail != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.InsertPersonEmailAddress(personId, false, alteremail.Split('@')[0], alteremail.Split('@')[1]);
                    }
                    if (homephone != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Phone(personId, homephone, true, 1, loginUser);
                    }
                    if (workphone != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Phone(personId, workphone, true, 2, loginUser);
                    }
                    if (cellphone != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Phone(personId, cellphone, true, 3, loginUser);
                    }
                    if (departmentTitleId != 0) // if departmentTitle empty, skip adding it
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.InsertDepartmentPersonTitle(departmentTitleId, SOMDepartmentBusinessAdministrationBL.Person.GetPersonDepartmentId(personId, 38));
                    }
                    if (pawprint != "") // if pawprint empty, skip adding pawprint
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Pawprint(personId, pawprint, "");
                    }
                }
                catch
                {
                    SOMDepartmentBusinessAdministrationBL.PersonFCM.DeleteFromIndividual(SOMDepartmentBusinessAdministrationBL.Person.DeletePerson(personId));
                    return "Error: SOM DATABASE insert person failed.";
                }
                for (int i = 0; i < AddressInfo.Length; i++)
                {
                    if (AddressInfo[i].StateId != 0)
                    {
                        try
                        {
                            SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Address(AddressInfo[i].PersonId, AddressInfo[i].AddressTypeId, AddressInfo[i].Street1, AddressInfo[i].Street2, AddressInfo[i].Street3, AddressInfo[i].City, AddressInfo[i].PostalCode, AddressInfo[i].StateId, AddressInfo[i].country, AddressInfo[i].IsPreferred, AddressInfo[i].IsDeleted, loginUser);
                        }
                        catch
                        {
                            return "Error: New person address Insert to SOMDBA failed. No." + (i + 1) + ".";
                        }
                    }
                }
                for (int i = 0; i < CategoryInfo.Length; i++) // insert person Category
                {
                    //--------------FCM DATABASE Insert-----------
                    if (CategoryInfo[i].IsFormer == false)
                    {
                        if (!SOMDepartmentBusinessAdministrationBL.PersonFCM.CheckExistCurrentTable(FCMindividualId, CategoryInfo[i].PersonCategoryCD))
                        {
                            try
                            {
                                SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonCategoryInfoToCurrentTable(FCMindividualId, CategoryInfo[i].PersonCategoryCD, CategoryInfo[i].EffectiveDate);
                            }
                            catch
                            {
                                return "Error: New person category Insert to FCM DB failed. No." + (i + 1) + ".";
                            }
                        }
                    }
                    else
                    {
                        if (!SOMDepartmentBusinessAdministrationBL.PersonFCM.CheckExistFormerTable(FCMindividualId, CategoryInfo[i].PersonCategoryCD))
                        {
                            try
                            {
                                SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonCategoryInfoToFormerTable(FCMindividualId, CategoryInfo[i].PersonCategoryCD);
                            }
                            catch
                            {
                                return "Error: Former person category Insert to FCM DB failed. No." + (i + 1) + ".";
                            }
                        }
                    }
                    try
                    {
                        int PDPCId = SOMDepartmentBusinessAdministrationBL.Person.InsertPersonDepartmentPersonCategory(personDepartmentId, CategoryInfo[i].PersonCategoryCD, CategoryInfo[i].TitleId, CategoryInfo[i].PartnerPersonId, CategoryInfo[i].AssistantPersonId, CategoryInfo[i].EffectiveDate,
                            CategoryInfo[i].TermDate, CategoryInfo[i].Position, CategoryInfo[i].FellowSpecification, CategoryInfo[i].Note, CategoryInfo[i].DoNotFinishProgram, CategoryInfo[i].IsIntegratedResident, CategoryInfo[i].YearGraduation, CategoryInfo[i].YearOfLastGift, CategoryInfo[i].TotalDonation, CategoryInfo[i].DesignationCD, (CategoryInfo[i].IsFormer == true) ? false : true);
                        string error = SavePersonCategoryClinic(PDPCId, CategoryInfo[i].ClinicIds);
                        if (error != "")
                        {
                            return error;
                        }
                    }
                    catch
                    {
                        return "Error: New person category Insert to SOMDBA failed. No." + (i + 1) + ".";
                    }

                }
                return SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartment(38); // add new person successfully        
                
            }
            else // edit person
            {
                //--------------FCM DATABASE Update person-----------
                int FCMIndividualId = 0;
                try
                {
                    FCMIndividualId = SOMDepartmentBusinessAdministrationBL.PersonFCM.GetIndividualIdByPersonId(personId);
                }
                catch
                {
                    return "Error: Update person, get FCM Individual ID failed.";
                }
                if (FCMIndividualId == 0)
                {
                    try
                    {
                        FCMIndividualId = SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonToIndividual(fname, mname, lname, email, pawprint);  
                    }
                    catch
                    {
                        return "Error: Update person, insert FCM Individual ID failed.";
                    }
                    try
                    {
                        SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonFCMIndividualToMapping(personId, FCMIndividualId);
                    }
                    catch
                    {
                        return "Error: Update person, insert FCM Individual ID, then mapping failed.";
                    }
                }
                try
                {
                    SOMDepartmentBusinessAdministrationBL.PersonFCM.UpdatePersonInfo(FCMIndividualId, fname, mname, lname, email, pawprint);  
                }
                catch
                {
                    return "Error: FCM DATABASE update person info failed.";
                }
                //--------------SOM DATABASE Update person-----------
                try
                {
                    SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonTable(EmployeeID, personId, genderId, sexId, 0, IsDeceased, IsNonSolicit, Note, loginUser, EducationInfo);
                    if (SOMDepartmentBusinessAdministrationBL.Person.GetPersonDepartmentId(personId, 38) == 0 && departmentTitleId != 0)
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.InsertDepartmentPersonTitle(departmentTitleId, SOMDepartmentBusinessAdministrationBL.Person.GetPersonDepartmentId(personId, 38));
                    }
                    else if (departmentTitleId != 0)
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.UpdateDepartmentPersonTitle(departmentTitleId, SOMDepartmentBusinessAdministrationBL.Person.GetPersonDepartmentId(personId, 38));
                    }
                    if (SOMDepartmentBusinessAdministrationBL.Person.HasPersonName(personId))
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonName(personId, lname, fname, mname, preferredName, " ", SalutationId, 0, loginUser);
                    }
                    else
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Name(personId, lname, fname, mname, preferredName, SalutationId, 0);
                    }
                    //phone check
                    if (SOMDepartmentBusinessAdministrationBL.Person.hasHomePhone(personId))
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonPhone(personId, homephone, true, 1, loginUser);
                    }
                    else if (homephone != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Phone(personId, homephone, true, 1, loginUser);
                    }
                    if (SOMDepartmentBusinessAdministrationBL.Person.hasCellPhone(personId))
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonPhone(personId, cellphone, true, 3, loginUser);
                    }
                    else if (cellphone != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Phone(personId, cellphone, true, 3, loginUser);
                    }
                    if (SOMDepartmentBusinessAdministrationBL.Person.hasWorkPhone(personId))
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonPhone(personId, workphone, true, 2, loginUser);
                    }
                    else if (workphone != "")
                    {
                        SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Phone(personId, workphone, true, 2, loginUser);
                    }
                    for (int i = 0; i < AddressInfo.Length; i++) // edit address
                    {
                        if (AddressInfo[i].StateId != 0)
                        {
                            try // Update address
                            {
                                if (!SOMDepartmentBusinessAdministrationBL.Person.HasPersonAddress(personId, AddressInfo[i].AddressTypeId))
                                {
                                    SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Address(personId, AddressInfo[i].AddressTypeId, AddressInfo[i].Street1, AddressInfo[i].Street2, AddressInfo[i].Street3, AddressInfo[i].City, AddressInfo[i].PostalCode, AddressInfo[i].StateId, AddressInfo[i].country, AddressInfo[i].IsPreferred, AddressInfo[i].IsDeleted, loginUser);
                                }
                                else
                                {
                                    SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonAddress(personId, AddressInfo[i].AddressTypeId, AddressInfo[i].Street1, AddressInfo[i].Street2, AddressInfo[i].Street3, AddressInfo[i].City, AddressInfo[i].StateId, AddressInfo[i].PostalCode, AddressInfo[i].country, AddressInfo[i].IsPreferred, AddressInfo[i].IsDeleted, loginUser);
                                }
                            }
                            catch
                            {
                                return "Error: Update person address "+ i +" failed.";
                            }
                            
                        }
                    }
                    
                    try // Update pawprint
                    {
                        if (pawprint != "")
                        {
                            if (!SOMDepartmentBusinessAdministrationBL.Person.IsPawprintExist(pawprint))
                            {
                                if (!SOMDepartmentBusinessAdministrationBL.Person.HasPersonPawprint(personId))
                                {
                                    SOMDepartmentBusinessAdministrationBL.Person.SavePersonInfo_Pawprint(personId, pawprint, "");
                                }
                                else
                                {
                                    SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonPawprint(personId, pawprint, loginUser);
                                }
                            }
                        }
                    }
                    catch
                    {
                        return "Error: Update person pawprint failed.";
                    }
                    try // Update email
                    {
                        if (email != "")
                        {
                            if (SOMDepartmentBusinessAdministrationBL.Person.hasEmail(personId, true))
                            {
                                SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonEmail(personId, email, true, loginUser);
                            }
                            else if (!SOMDepartmentBusinessAdministrationBL.Person.hasEmail(personId, true) && !SOMDepartmentBusinessAdministrationBL.Person.IsEmailExist(email))
                            {
                                SOMDepartmentBusinessAdministrationBL.Person.InsertPersonEmailAddress(personId, true, email.Split('@')[0], email.Split('@')[1]);
                            }
                            else
                            {
                                return "Error: Email Exists.";
                            }
                        }
                        if (alteremail != "")
                        {
                            if (SOMDepartmentBusinessAdministrationBL.Person.hasEmail(personId, false))
                            {
                                SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonEmail(personId, alteremail, false, loginUser);
                            }
                            else if (!SOMDepartmentBusinessAdministrationBL.Person.hasEmail(personId, false) && !SOMDepartmentBusinessAdministrationBL.Person.IsEmailExist(alteremail))
                            {
                                SOMDepartmentBusinessAdministrationBL.Person.InsertPersonEmailAddress(personId, false, alteremail.Split('@')[0], alteremail.Split('@')[1]);
                            }
                            else
                            {
                                return "Error: Email Exists.";
                            }
                        }
                    }
                    catch
                    {
                        return "Error: Update person email failed.";
                    }
                }
                catch
                {
                    return "Error: Edit person Info failed."; // failed at update editting infos
                }
                if (CategoryInfo != null)
                {
                    for (int i = 0; i < CategoryInfo.Length; i++)
                    {
                        //--------------FCM DATABASE Insert/Update person Category-----------
                        try
                        {
                            SOMDepartmentBusinessAdministrationBL.PersonFCM.UpdatePersonCategoryInfo(FCMIndividualId, CategoryInfo[i].PersonCategoryCD, CategoryInfo[i].EffectiveDate);
                            if (CategoryInfo[i].IsFormer == false && !SOMDepartmentBusinessAdministrationBL.PersonFCM.CheckExistCurrentTable(FCMIndividualId, CategoryInfo[i].PersonCategoryCD))
                            {
                                SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonCategoryInfoToCurrentTable(FCMIndividualId, CategoryInfo[i].PersonCategoryCD, CategoryInfo[i].EffectiveDate);
                            }
                            else if (CategoryInfo[i].IsFormer == true && !SOMDepartmentBusinessAdministrationBL.PersonFCM.CheckExistFormerTable(FCMIndividualId, CategoryInfo[i].PersonCategoryCD))
                            {
                                SOMDepartmentBusinessAdministrationBL.PersonFCM.InsertPersonCategoryInfoToFormerTable(FCMIndividualId, CategoryInfo[i].PersonCategoryCD);
                            }
                        }
                        catch
                        {
                            return "Error: New person category Insert/Update to FCM DB failed. No." + (i + 1) + ".";
                        }
                        //--------------SOM DATABASE Insert person Category-----------
                        if (CategoryInfo[i].PersonDepartmentPersonCategoryId == 0) // new Category Insert
                        {
                            try
                            {
                                personDepartmentId = SOMDepartmentBusinessAdministrationBL.Person.GetPersonDepartmentId(personId, 38);
                                int PDPCId = SOMDepartmentBusinessAdministrationBL.Person.InsertPersonDepartmentPersonCategory(personDepartmentId, CategoryInfo[i].PersonCategoryCD, CategoryInfo[i].TitleId, CategoryInfo[i].PartnerPersonId, CategoryInfo[i].AssistantPersonId, CategoryInfo[i].EffectiveDate,
                            CategoryInfo[i].TermDate, CategoryInfo[i].Position, CategoryInfo[i].FellowSpecification, CategoryInfo[i].Note, CategoryInfo[i].DoNotFinishProgram, CategoryInfo[i].IsIntegratedResident, CategoryInfo[i].YearGraduation, CategoryInfo[i].YearOfLastGift, CategoryInfo[i].TotalDonation, CategoryInfo[i].DesignationCD, (CategoryInfo[i].IsFormer == true) ? false : true);
                                string error = SavePersonCategoryClinic(PDPCId, CategoryInfo[i].ClinicIds);
                                if (error != "")
                                {
                                    return error;
                                }
                            }
                            catch
                            {
                                return "Error: New person category Update to SOMDBA failed. No." + (i + 1) + ".";
                            }
                        }
                        else
                        {
                            try
                            {
                                SOMDepartmentBusinessAdministrationBL.Person.UpdatePersonDepartmentPersonCategory(CategoryInfo[i].PersonDepartmentPersonCategoryId, CategoryInfo[i].TitleId, CategoryInfo[i].PartnerPersonId, CategoryInfo[i].AssistantPersonId, CategoryInfo[i].EffectiveDate,
                            CategoryInfo[i].TermDate, CategoryInfo[i].Position, CategoryInfo[i].FellowSpecification, CategoryInfo[i].Note, CategoryInfo[i].DoNotFinishProgram, CategoryInfo[i].IsIntegratedResident, CategoryInfo[i].YearGraduation, CategoryInfo[i].YearOfLastGift, CategoryInfo[i].TotalDonation, CategoryInfo[i].DesignationCD, (CategoryInfo[i].IsFormer == true) ? false : true, loginUser);
                                string error = SavePersonCategoryClinic(CategoryInfo[i].PersonDepartmentPersonCategoryId, CategoryInfo[i].ClinicIds);
                                if (error != "")
                                {
                                    return error;
                                }
                            }
                            catch
                            {
                                return "Error: Person category Update to SOMDBA failed. No." + (i + 1) + ".";
                            }
                            
                        }
                    }
                }
                return SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartment(38);
            }

        }

        [WebMethod]
        public static string InactivePerson(int personId)
        {
            try
            {
                SOMDepartmentBusinessAdministrationBL.Person.InactivePerson(personId, loginUser);
            }
            catch
            {
                return "Error: Fail to delete Person.";
            }
            return SOMDepartmentBusinessAdministrationBL.Person.GetPersonListByDepartment(38);
        }
          
    }

}