declare @FiscalYear int
--set @FiscalYear = 2020

;with EE as
(
SELECT Name = i.individual_lastname+','+i.individual_firstname
	  ,FacultyId
	  ,[ActivityLevelId] = a.ActivityLevelId
	  ,ee.MetricId
	  ,fm.FCMMetric
	  ,fm.ContactUnits
	  ,fm.AssocUnits
      ,[Count]
	  ,TotalUnits = (fm.ContactUnits+fm.AssocUnits)*[Count]

  FROM [MBM].[dbo].[EducationEffort] ee
  inner join [fcmadmin].dbo.individual i on i.individual_id = ee.FacultyId
  inner join [MBM].[dbo].[FCMMetric] fm on fm.FCMMetricId = ee.MetricId
  inner join [MBM].[dbo].[ActivityLevel] a on a.ActivityLevelId = fm.ActivityLevelId
  where FiscalYear=@FiscalYear
),

summary as 
(
select
   Name,
   FacultyId,
Administration = sum(case [ActivityLevelId] when 1 then TotalUnits else 0 end),
Undergraduate = sum(case [ActivityLevelId] when 2 then TotalUnits else 0 end),
GraduateAndPostDoctoral = sum(case [ActivityLevelId] when 3 then TotalUnits else 0 end),
UndergradMedicalEducation12 = sum(case [ActivityLevelId] when 4 then TotalUnits else 0 end),
UndergradMedicalEducation3 = sum(case [ActivityLevelId] when 5 then TotalUnits else 0 end),
UndergradMedicalEducation4 = sum(case [ActivityLevelId] when 6 then TotalUnits else 0 end),
GraduateMedicalEducation = sum(case [ActivityLevelId] when 7 then TotalUnits else 0 end),
ResearchServiceScholarship = sum(case [ActivityLevelId] when 1001 then TotalUnits else 0 end),
[TotalUnits] = sum([TotalUnits])
from EE
group by Name,FacultyId
)

select Name
	  ,FacultyId
	  ,EducationWithM = CAST((Administration+Undergraduate+GraduateAndPostDoctoral+UndergradMedicalEducation12+UndergradMedicalEducation3+UndergradMedicalEducation4+GraduateMedicalEducation)/c.AnnualTargetFtes*100 as decimal(8,2))
	  ,EducationNoM = CAST (ResearchServiceScholarship/c.AnnualTargetFtes*100 as decimal(8,2))
from summary
inner join [clinic_faculty_activity_report].dbo.FacultyActivityConfig c on c.FiscalYear = @FiscalYear
order by Name