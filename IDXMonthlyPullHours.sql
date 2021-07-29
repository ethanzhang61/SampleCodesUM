with original_hours
                                as
                                ( SELECT  [Month] = datepart(m, [Schedule_Date]),
	                                       [Year] = datepart(yyyy, [Schedule_Date]),
	                                       Provider302Id = hr.Prov_Id_302, 
	                                       ProviderNum = p.PROVIDER_NUM, 
	                                       Provider = p.PROVIDER_NAME, 
	                                       ProviderCategory = c.PROVIDER_CATEGORY_NAME, 
	                                       [Hours] = Sum(cast(([Duration]/60.0) as decimal(8,2))),
	                                       DepartmentId = d.Id_301, 
	                                       DeparmentNum = d.DEPARTMENT_NUM, 
	                                       Department = d.DEPARTMENT_NAME, 
	                                       LocationId = hr.Loc_Id_331, 
	                                       LocationNum = l.LOCATION_NUM, 
	                                       Location = l.LOCATION_NAME
                                    FROM dbo.UMC_Sched_Provider_Wkhr hr
                                    INNER JOIN dbo.SCHEDULING_PROVIDERS  p
                                        ON hr.Prov_Id_302 = p.ID_302 
                                    INNER JOIN dbo.SCHEDULING_LOCATIONS l
                                        ON hr.Loc_Id_331 = l.ID_331 
                                    INNER JOIN dbo.SCHEDULING_PROVIDER_CATEG c
                                        ON p.PROV_CAT_ID_324 = c.ID_324 
                                    INNER JOIN dbo.SCHEDULING_DEPARTMENTS  d
                                        ON hr.Dept_Id_301 = d.ID_301
                                    WHERE (datepart(m,hr.Schedule_Date) = @Month and datepart(yyyy,hr.Schedule_Date) = @Year)
                                    GROUP BY hr.Prov_Id_302, 
	                                       p.PROVIDER_NUM, 
	                                       p.PROVIDER_NAME, 
	                                       c.PROVIDER_CATEGORY_NAME, 
	                                       --0, 
	                                       d.Id_301, 
	                                       d.DEPARTMENT_NUM, 
	                                       d.DEPARTMENT_NAME, 
	                                       hr.Loc_Id_331, 
	                                       l.LOCATION_NUM, 
	                                       l.LOCATION_NAME, 
	                                       datepart(m, [Schedule_Date] ),
	                                       datepart(yyyy, [Schedule_Date])
                                    HAVING (((hr.Prov_Id_302)<>1) AND ((d.Id_301) In (639,383,368,26,381,542,334,744,680,682,129,645,926,927,192,823,744,949,938,946,934)))
                                    )
                                    select [Month], [Year], Provider302Id, ProviderNum, [Provider], ProviderCategory, [Hours] = sum([Hours]), DepartmentId, DeparmentNum, [Department]
									from original_hours
									group by [Month], [Year], Provider302Id, ProviderNum, [Provider], ProviderCategory, DepartmentId, DeparmentNum, [Department]
									order by Provider, Department