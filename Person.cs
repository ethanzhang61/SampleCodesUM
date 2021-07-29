﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using NLog;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Data.SqlClient;


namespace SOMDepartmentBusinessAdministrationBL
{
    public static class Person
    {
		/**
		* Returns personID generated by the database. 
		* <p>
		* This method will return immediately, if new person insertion success. 
		* When the insertion attempt fails, it would return 0.
		*
		* @param  EmployeeID  The person's campuswide employeeID issued by HR department
		* @param  GenderId 	By pre-Defined DB structure, GenderID is required, but it performs same functionality as sexID
		* @param  SexId  Same as GenderID
		* @param  IsNonSolicit	If the person has been tagged as "DO NOT Solicit"
		* @param  PartnerPersonId	Required for a 1:1 relationship called "Partner" in their business
		* @param  DegreeId	The person's highest degree
		* @param  IsDeceased	If the person is deceased or not
		* @param  IsActive		If the person is classified as deleted or not
		* @param  IsRetired		If the person is retired or not
		* @param  Note			Note for this person
		* @param  EducationInfo		Additional Education information for this person
		* @return      personId Generated by database
		*/
		public static int InsertPerson(string EmployeeID, int GenderId, int SexId, bool IsNonSolicit, int PartnerPersonId, int DegreeId, bool IsDeceased, bool IsActive, bool IsRetired, string Note, string EducationInfo)
        {
            int personId = 0;
            string sql2 = @"insert into [dbo].[Person] (EmployeeID,GenderId,SexId,IsNonSolicit,PartnerPersonId,DegreeId,EducationInfo,IsDeceased,IsActive,IsRetired,Note) 
                            OUTPUT Inserted.PersonId
                            values (@EmployeeID,@GenderId,@SexId,@IsNonSolicit,@PartnerPersonId,@DegreeId,@EducationInfo,@IsDeceased,@IsActive,@IsRetired,@Note)";
            SubSonic.QueryCommand qc2 = new SubSonic.QueryCommand(sql2, "SOMDBADBDataProvider");
            qc2.Parameters.Add("@EmployeeID", EmployeeID, DbType.String);
            qc2.Parameters.Add("@GenderId", GenderId, DbType.Int32);
            qc2.Parameters.Add("@SexId", SexId, DbType.Int32);
            qc2.Parameters.Add("@IsNonSolicit", (IsNonSolicit == true ? 1 : 0), DbType.Boolean);
            qc2.Parameters.Add("@PartnerPersonId", PartnerPersonId, DbType.Int32);
            qc2.Parameters.Add("@DegreeId", DegreeId, DbType.Int32);
            qc2.Parameters.Add("@EducationInfo", EducationInfo, DbType.String);
            qc2.Parameters.Add("@IsDeceased", (IsDeceased == true ? 1 : 0), DbType.Boolean);
            qc2.Parameters.Add("@IsActive", (IsActive == true ? 1 : 0), DbType.Boolean);
            qc2.Parameters.Add("@IsRetired", (IsRetired == true ? 1 : 0), DbType.Boolean);
            qc2.Parameters.Add("@Note", Note, DbType.String);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc2));
            if (DT.Rows.Count > 0)
                personId = Convert.ToInt32(DT.Rows[0]["PersonId"]);

            return personId;

        }
		
		public static int InsertPersonDepartmentPersonCategory(int PersonDepartmentId, int PersonCategoryCD, int TitleId, int PartnerPersonId, int AssistantPersonId, string EffectiveDate, string TermDate, string Position, string FellowSpecification, string Note, bool DoNotFinishProgram, bool IsIntegratedResident, int YearGraduation, int YearOfLastGift, int TotalDonation, int DesignationCD, bool IsActive)
        {
            string sql2;
            int PersonDepartmentPersonCategoryId = 0;
            if(EffectiveDate == "" && TermDate == "")
            {
                sql2 = @"insert into [dbo].[PersonDepartmentPersonCategory] (PersonDepartmentId,PersonCategoryCD,TitleId,PartnerPersonId,AssistantPersonId,Position,FellowSpecification,Note,DoNotFinishProgram,IsIntegratedResident,YearGraduation,YearOfLastGift,TotalDonation,DesignationCD,IsActive) 
                            OUTPUT Inserted.PersonDepartmentPersonCategoryId
                            values (@PersonDepartmentId,@PersonCategoryCD,@TitleId,@PartnerPersonId,@AssistantPersonId,@Position,@FellowSpecification,@Note,@DoNotFinishProgram,@IsIntegratedResident,@YearGraduation,@YearOfLastGift,@TotalDonation,@DesignationCD,@IsActive)";
            }
            else if (EffectiveDate != "" && TermDate == "")
            {
                sql2 = @"insert into [dbo].[PersonDepartmentPersonCategory] (PersonDepartmentId,PersonCategoryCD,TitleId,PartnerPersonId,AssistantPersonId,EffectiveDate,Position,FellowSpecification,Note,DoNotFinishProgram,IsIntegratedResident,YearGraduation,YearOfLastGift,TotalDonation,DesignationCD,IsActive) 
                            OUTPUT Inserted.PersonDepartmentPersonCategoryId
                            values (@PersonDepartmentId,@PersonCategoryCD,@TitleId,@PartnerPersonId,@AssistantPersonId,@EffectiveDate,@Position,@FellowSpecification,@Note,@DoNotFinishProgram,@IsIntegratedResident,@YearGraduation,@YearOfLastGift,@TotalDonation,@DesignationCD,@IsActive)";
            }
            else if (EffectiveDate == "" && TermDate != "")
            {
                sql2 = @"insert into [dbo].[PersonDepartmentPersonCategory] (PersonDepartmentId,PersonCategoryCD,TitleId,PartnerPersonId,AssistantPersonId,TermDate,Position,FellowSpecification,Note,DoNotFinishProgram,IsIntegratedResident,YearGraduation,YearOfLastGift,TotalDonation,DesignationCD,IsActive) 
                            OUTPUT Inserted.PersonDepartmentPersonCategoryId
                            values (@PersonDepartmentId,@PersonCategoryCD,@TitleId,@PartnerPersonId,@AssistantPersonId,@TermDate,@Position,@FellowSpecification,@Note,@DoNotFinishProgram,@IsIntegratedResident,@YearGraduation,@YearOfLastGift,@TotalDonation,@DesignationCD,@IsActive)";
            }
            else
            {
                sql2 = @"insert into [dbo].[PersonDepartmentPersonCategory] (PersonDepartmentId,PersonCategoryCD,TitleId,PartnerPersonId,AssistantPersonId,EffectiveDate,TermDate,Position,FellowSpecification,Note,DoNotFinishProgram,IsIntegratedResident,YearGraduation,YearOfLastGift,TotalDonation,DesignationCD,IsActive) 
                            OUTPUT Inserted.PersonDepartmentPersonCategoryId
                            values (@PersonDepartmentId,@PersonCategoryCD,@TitleId,@PartnerPersonId,@AssistantPersonId,@EffectiveDate,@TermDate,@Position,@FellowSpecification,@Note,@DoNotFinishProgram,@IsIntegratedResident,@YearGraduation,@YearOfLastGift,@TotalDonation,@DesignationCD,@IsActive)";
            }
            
            SubSonic.QueryCommand qc2 = new SubSonic.QueryCommand(sql2, "SOMDBADBDataProvider");
            qc2.Parameters.Add("@PersonDepartmentId", PersonDepartmentId, DbType.Int32);
            qc2.Parameters.Add("@PersonCategoryCD", PersonCategoryCD, DbType.Int32);
            qc2.Parameters.Add("@TitleId", TitleId, DbType.Int32);
            qc2.Parameters.Add("@PartnerPersonId", PartnerPersonId, DbType.Int32);
            qc2.Parameters.Add("@AssistantPersonId", AssistantPersonId, DbType.Int32);
            qc2.Parameters.Add("@EffectiveDate", EffectiveDate, DbType.String);
            qc2.Parameters.Add("@TermDate", TermDate, DbType.String);
            qc2.Parameters.Add("@Position", Position, DbType.String);
            qc2.Parameters.Add("@FellowSpecification", FellowSpecification, DbType.String);
            qc2.Parameters.Add("@Note", Note, DbType.String);
            qc2.Parameters.Add("@DoNotFinishProgram", (DoNotFinishProgram == true ? 1 : 0), DbType.Boolean);
            qc2.Parameters.Add("@IsIntegratedResident", (IsIntegratedResident == true ? 1 : 0), DbType.Boolean);
            qc2.Parameters.Add("@YearGraduation", YearGraduation, DbType.Int32);
            qc2.Parameters.Add("@YearOfLastGift", YearOfLastGift, DbType.Int32);
            qc2.Parameters.Add("@TotalDonation", TotalDonation, DbType.Int32);
            qc2.Parameters.Add("@DesignationCD", DesignationCD, DbType.Int32);
            qc2.Parameters.Add("@IsActive", (IsActive == true ? 1 : 0), DbType.Boolean);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc2));
            if (DT.Rows.Count > 0)
                PersonDepartmentPersonCategoryId = Convert.ToInt32(DT.Rows[0]["PersonDepartmentPersonCategoryId"]);

            return PersonDepartmentPersonCategoryId;
        }
		public static int InactivePerson(int PersonId, string loginUser)
        {
            string sql = @"
                              update [dbo].[Person]
                              set [IsActive] = 0, [ModifiedBy] = @loginUser, [ModifiedOn] = getdate()
                              where PersonId = @PersonId

                              update [dbo].[PersonDepartment]
                              set [IsDeleted] = 1, [ModifiedBy] = @loginUser, [ModifiedOn] = getdate()
                              where PersonId = @PersonId

                            update u
                            set u.[IsDeleted] = 1, [ModifiedBy] = @loginUser, [ModifiedOn] = getdate()
                            from [dbo].[PersonDepartmentPersonCategory] u
                                inner join [dbo].[PersonDepartment] s on
                                    u.PersonDepartmentId = s.PersonDepartmentId
                            where s.PersonId = @PersonId
                            select FCMIndividualId from [dbo].[PersonFCMIndividualMapping] where PersonId = @PersonId
                            ";

            SubSonic.QueryCommand qc = new SubSonic.QueryCommand(sql, "SOMDBADBDataProvider");
            qc.Parameters.Add("@PersonId", PersonId, DbType.Int32);
            qc.Parameters.Add("@loginUser", loginUser, DbType.String);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc));
            int FCMIndividualId = 0;
            if (DT.Rows.Count > 0)
            {
                FCMIndividualId = Convert.ToInt32(DT.Rows[0]["FCMIndividualId"]);
            }
            return FCMIndividualId;
        }
		public static void UpdatePersonTable(string EmployeeID, int PersonId, int GenderId, int SexId, int DegreeId, bool IsDeceased, bool IsNonSolicit, string Note, string LoggedInUser, string EducationInfo)
			{
				SubSonic.Query query = new SubSonic.Query(SOMDBADB.Person.Schema);
				query.QueryType = SubSonic.QueryType.Update;
				query.AddUpdateSetting(SOMDBADB.Person.Columns.EmployeeID, EmployeeID);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.GenderId, GenderId);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.SexId, SexId);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.IsNonSolicit, IsNonSolicit);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.DegreeId, DegreeId);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.EducationInfo, EducationInfo);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.IsDeceased, IsDeceased);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.Note, Note);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.ModifiedBy, LoggedInUser);
				query.AddUpdateSetting(SOMDBADB.Person.Columns.ModifiedOn, DateTime.Now);
				query.AddWhere(SOMDBADB.Person.Columns.PersonId, PersonId);
				query.Execute();

			}
		public static void UpdatePersonAddress(int PersonId, int addressType, string street1, string street2, string street3, string city, int stateId, string postalCode, string country, bool isPreferred, bool isDeleted, string LoggedInUser)
        {
            SubSonic.Query query = new SubSonic.Query(SOMDBADB.PersonAddress.Schema);
            query.QueryType = SubSonic.QueryType.Update;
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.Street1, street1);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.Street2, street2);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.Street3, street3);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.City, city);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.StateId, stateId);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.PostalCode, postalCode);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.CountryRegion, country);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.MailAddressSameAsPerm, isPreferred);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.ModifiedBy, LoggedInUser);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.ModifiedOn, DateTime.Now);
            query.AddUpdateSetting(SOMDBADB.PersonAddress.Columns.IsDeleted, isDeleted);
            query.AddWhere(SOMDBADB.PersonAddress.Columns.PersonId, PersonId);
            query.AddWhere(SOMDBADB.PersonAddress.Columns.AddressTypeId, addressType);
            query.Execute();

        }
		public static string GetPersonListByDepartmentCategory(int DepartmentId, int PersonCategoryId)
        {
            string sqlStr;
            if(PersonCategoryId != 0)
            {
                sqlStr = @"select PersonName = pn.LastName + ', ' + pn.FirstName, PersonId = pd.personid
                                from [dbo].[PersonDepartment] pd
                                left join [dbo].[PersonName] pn on pd.PersonId = pn.PersonId
								left join [dbo].[PersonDepartmentPersonCategory] pdpc on pdpc.PersonDepartmentId = pd.PersonDepartmentId and pdpc.IsActive = 1 and pdpc.IsDeleted = 0
								where pdpc.PersonCategoryCD = @PersonCategoryId and pd.DepartmentId = @DepartmentId and pd.IsDeleted = 0
                                order by pn.LastName";
            }
            else
            {
                sqlStr = @"select PersonName = pn.LastName + ', ' + pn.FirstName, PersonId = pd.personid
                                from [dbo].[PersonDepartment] pd
                                left join [dbo].[PersonName] pn on pd.PersonId = pn.PersonId
								left join [dbo].[PersonDepartmentPersonCategory] pdpc on pdpc.PersonDepartmentId = pd.PersonDepartmentId and pdpc.IsActive = 1 and pdpc.IsDeleted = 0
								where  pd.DepartmentId = @DepartmentId and pd.IsDeleted = 0
                                order by pn.LastName";
            }
            SubSonic.QueryCommand qc = new SubSonic.QueryCommand(sqlStr, "SOMDBADBDataProvider");
            qc.Parameters.Add("@DepartmentId", DepartmentId, DbType.Int32);
            qc.Parameters.Add("@PersonCategoryId", PersonCategoryId, DbType.Int32);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc));
            if (DT.Rows.Count > 0)
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(DT);
                return JSONString;
            }
            else
                return null;
        }
		public static string GetSelectedpersonInfo(int personId)
        {
            string sqlStr = @"select FirstName = pn.FirstName, MiddleName=pn.MiddleName,LastName = pn.LastName, preferredName = pn.PreferName, personid = p.personid, [EmployeeID], 
                                    SalutationId = pn.NameSalutationId, SexId = p.SexId,
                                    EmailAddress = pe.EmailAddress, AlterEmailAddress = pe2.EmailAddress, pawprint = pp.Pawprint, 
									homephone = pphone1.Number,workphone = pphone2.Number,cellphone = pphone3.Number, IsDeceased = p.IsDeceased, IsNonSolicit = p.IsNonSolicit, 
									DegreeId, EducationInfo, Note, p.PartnerPersonId

                                    from person p
                                    left join PersonName pn on p.PersonId = pn.PersonId
                                    left join PersonEmailAddress pe on p.PersonId = pe.PersonId and pe.IsPrimaryEmailAddress = 1
									left join PersonEmailAddress pe2 on p.PersonId = pe2.PersonId and pe2.IsPrimaryEmailAddress = 0
                                    left join PersonPawprint pp on p.PersonId = pp.PersonId 
									left join PersonPhone pphone1 on p.PersonId = pphone1.PersonId and pphone1.PhoneTypeId = 1
                                    left join PersonPhone pphone2 on p.PersonId = pphone2.PersonId and pphone2.PhoneTypeId = 2
									left join PersonPhone pphone3 on p.PersonId = pphone3.PersonId and pphone3.PhoneTypeId = 3
                                    where p.personId = @personId and p.IsActive = 1";
            SubSonic.QueryCommand qc = new SubSonic.QueryCommand(sqlStr, "SOMDBADBDataProvider");
            qc.Parameters.Add("@personId", personId, DbType.Int32);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc));
            if (DT.Rows.Count > 0)
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(DT);
                return JSONString;
            }
            else
                return null;
        }
        public static string GetSelectedpersonAddressInfo(int personId)
        {
            string sqlStr = @"SELECT [PersonAddressId]
                                  ,[PersonId]
                                  ,[AddressTypeId]
                                  ,[Street1]
                                  ,[Street2]
                                  ,[Street3]
                                  ,[City]
                                  ,[StateId]
                                  ,[PostalCode]
                                  ,country = CountryRegion
								  ,IsPreferred = ISNULL([MailAddressSameAsPerm],0)
                              FROM [dbo].[PersonAddress]
                                where [PersonId] = @personId and IsDeleted = 0";
            SubSonic.QueryCommand qc = new SubSonic.QueryCommand(sqlStr, "SOMDBADBDataProvider");
            qc.Parameters.Add("@personId", personId, DbType.Int32);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc));
            if (DT.Rows.Count > 0)
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(DT);
                return JSONString;
            }
            else
                return "";
        }
        public static string GetSelectedpersonCategoryInfo(int personId, int DepartmentId)
        {
            string sqlStr = @"SELECT [PersonDepartmentPersonCategoryId]
                                      ,pd.[PersonDepartmentId]
                                      ,[PersonCategoryCD]
                                      ,[TitleId]
                                      ,[PartnerPersonId]
                                      ,[AssistantPersonId]
                                      ,[EffectiveDate]
                                      ,[TermDate]
                                      ,[Position]
                                      ,[FellowSpecification]
                                      ,[Note]
                                      ,[DoNotFinishProgram]
                                      ,[IsIntegratedResident]
                                      ,[YearGraduation]
                                      ,[YearOfLastGift]
                                      ,[TotalDonation]
                                      ,[DesignationCD]
                                      ,[IsFormer] = case when [IsActive] = 1 then 0 else 1 end
                                      ,ClinicIds = STUFF(
                                                        (   SELECT ',' + CONVERT(NVARCHAR(20), a.ClinicCD) 
                                                            FROM [dbo].[PersonDepartmentPersonCategoryClinic] a
                                                            WHERE a.PersonDepartmentPersonCategoryId = pdpc.PersonDepartmentPersonCategoryId 
                                                            FOR xml path('')
                                                        )
                                                        , 1
                                                        , 1
                                                        , '')
                                  FROM [dbo].[PersonDepartmentPersonCategory] pdpc
                                  left join [dbo].[PersonDepartment] pd on pd.PersonDepartmentId=pdpc.PersonDepartmentId
                                  where pd.PersonId = @personId and pd.DepartmentId = @DepartmentId and pd.IsDeleted = 0 and pdpc.IsDeleted = 0";
            SubSonic.QueryCommand qc = new SubSonic.QueryCommand(sqlStr, "SOMDBADBDataProvider");
            qc.Parameters.Add("@personId", personId, DbType.Int32);
            qc.Parameters.Add("@DepartmentId", DepartmentId, DbType.Int32);
            DataTable DT = new DataTable();
            DT.Load(SubSonic.DataService.GetReader(qc));

            if (DT.Rows.Count > 0)
            {
                string JSONString = string.Empty;
                JSONString = JsonConvert.SerializeObject(DT);
                return JSONString;
            }
            else
                return "";
        }
	}


}