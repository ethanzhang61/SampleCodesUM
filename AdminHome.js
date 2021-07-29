'use strict'

var SOMDetpBAdmin = angular.module('MSETSApp', ['MSETSAuth', 'MSETS-Menu', 'MSETS-Login', 'ui.bootstrap', 'ui.bootstrap.datetimepicker', 'ngRoute', 'checklist-model']);

SOMDetpBAdmin
    .config(function ($routeProvider) {
        $routeProvider
            .when("/PersonMgmt", {
                templateUrl: "Template/PersonMgmt.html",
                controller: "PersonMgmtCtrl"
            })
            .when("/DonationMgmt", {
                templateUrl: "Template/DonationMgmt.html",
                controller: "DonationMgmtCtrl"
            })
            .when("/EmailTemplate", {
                templateUrl: "Template/EmailTemplate.html",
                controller: "EmailTemplateCtrl"
            })
            .when("/ClinicScheduler", {
                templateUrl: "Template/ClinicScheduler.html",
                controller: "ClinicSchedulerCtrl"
            })
            .when("/MBM", {
                templateUrl: "Template/MBM.html",
                controller: "MBMCtrl"
            })
            .when("/LOAMgmt", {
                templateUrl: "Template/LOAMgmt.html",
                controller: "LOAMgmtCtrl"
            })
            .when("/RVUMgmt", {
                templateUrl: "Template/RVUMgmt.html",
                controller: "RVUMgmtCtrl"
            })
            .when("/paygmt", {
                templateUrl: "Template/PayMgmt.html",
                controller: "PayMgmtCtrl"
            })
            .when("/Reports", {
                templateUrl: "Template/Reports.html",
                controller: "ReportsCtrlCtrl"
            })
            .when("/SystemSetup", {
                templateUrl: "Template/SystemSetup.html",
                controller: "SystemSetupCtrl"
            })
            .otherwise("Template/PersonMgmt.html");
    })

   
    .controller("PersonMgmtCtrl", ['$scope', '$location', '$http', '$dialog', 'ui.config', '$timeout', '$route', '$window', function ($scope, $location, $http, $dialog, uiConfig, $timeout, $route, $window, $modalInstance) {
        //Initial variables, data containers and permission level.
		$scope.init = function () {

            $scope.showModal = false;
            $scope.reverseSort = false;
            $scope.PersonList = PersonList;
            $scope.searchFilter = { selectedCategory: 0, IsFormer: 0 };
            $scope.isDisabled = false;
            $scope.filterresult = [];
            $scope.personCategory = personCategory;

            $scope.permission = 0;
            for (var i = 0; i < loginPersonRole.length; i++)
            {
                if (loginPersonRole[i].PermisiosnCD == 2)
                {
                    $scope.permission = 3;
                    break;
                }
                else if (loginPersonRole[i].PermisiosnCD == 4)
                {
                    $scope.permission = 2;
                    break;
                }
                else if (loginPersonRole[i].PermisiosnCD == 3)
                {
                    $scope.permission = 1;
                    break;
                }
            }
            
            $scope.salutation = salutation;
            $scope.usstate = usstate;
            $scope.personAddressType = personAddressType;
            $scope.sexes = sexes;
            $scope.personCategory = personCategory;
            $scope.TitleList = TitleList;
            $scope.ClinicList = ClinicList;
            $scope.DegreeList = DegreeList;
            $scope.DesignationList = DesignationList;
            $scope.FullPersonList = FullPersonList;
            $scope.AssistantPersonList = AssistantPersonList;

            $scope.selectedPersonInfo = JSON.parse("{}");
            $scope.selectedpersonAddressInfo = [];
            $scope.selectedpersonCategoryInfo = [];
        };
        $scope.init();
		//Set up validation checker for each components
        function isNullString(val) {
            if (typeof val === "undefined" || !val) {
                return "";
            } else {
                return val;
            }
        };
        function isNullint(val) {
            if (typeof val === "undefined" || !val) {
                return 0;
            } else {
                return val;
            }
        };

        function isNullboolean(val) {
            if (typeof val === "undefined" || !val) {
                return false;
            } else {
                return val;
            }
        };
		//Clear person related containers for Reset
        $scope.ResetToAddNewPerson = function ()
        {
            $scope.selectedPersonInfo = JSON.parse("{}");
            $scope.selectedpersonAddressInfo = [];
            $scope.selectedpersonCategoryInfo = [];
        };

        $scope.close = function () {
            $("#PersonInfo").modal("hide");
        }
		//Search person related information by personId
        $scope.searchPerson = function (val) {
            $scope.showModal = !$scope.showModal; 
            var url = "Home.aspx/GetSelectedPersonInfo";
            var config = { headers: { "Content-Type": "application/json" } };
            var submitData = "{\"personId\" : \"" + isNullint(val)
                + "\" }";
            $http.post(url, submitData, { cache: false }).success(function (data, status, headers, config) {
                $scope.selectedPersonInfo = data.d != '' ? (JSON.parse(data.d))[0] : "";
                var url = "Home.aspx/GetSelectedPersonAddressInfo";
                var config = { headers: { "Content-Type": "application/json" } };
                $http.post(url, submitData, { cache: false }).success(function (data, status, headers, config) {
                    $scope.selectedpersonAddressInfo = data.d != '' ? JSON.parse(data.d) : [];
                    var url = "Home.aspx/GetSelectedPersonCategoryInfo";
                    var config = { headers: { "Content-Type": "application/json" } };
                    $http.post(url, submitData, { cache: false }).success(function (data, status, headers, config) {
                        $scope.selectedpersonCategoryInfo = data.d != '' ? JSON.parse(data.d) : [];
                        for (var i = 0; i < $scope.selectedpersonCategoryInfo.length;i++)
                        {
                            if ($scope.selectedpersonCategoryInfo[i].EffectiveDate != null)
                                $scope.selectedpersonCategoryInfo[i].EffectiveDate = $scope.selectedpersonCategoryInfo[i].EffectiveDate.split('T')[0];
                            if ($scope.selectedpersonCategoryInfo[i].TermDate != null)
                                $scope.selectedpersonCategoryInfo[i].TermDate = $scope.selectedpersonCategoryInfo[i].TermDate.split('T')[0];
                            if ($scope.selectedpersonCategoryInfo[i].IsFormer == 1)
                                $scope.selectedpersonCategoryInfo[i].IsFormer = true;
                            else
                                $scope.selectedpersonCategoryInfo[i].IsFormer = false;

                            $scope.PersonCategoryClinicInit($scope.selectedpersonCategoryInfo[i]);
                        }
                    }).error(function (data, status, headers, config) {
                        window.alert("Get selected PersonCategoryInfo failed");
                    });
                }).error(function (data, status, headers, config) {
                    window.alert("Get selected PersonAddressInfo failed");
                });
            }).error(function (data, status, headers, config) {
                window.alert("Get selected PersonInfo failed");
            });
            //$scope.isDisabled = false;
            $scope.activetab = 2;
        };
		//Search keyword within each record among all the retrieved dataset
        $scope.personfilter = personfilter;
        function personfilter(person) {
                if ($scope.searchFilter.selectedCategory == 0 && $scope.searchFilter.IsFormer == 0) {
                    return true;
                }
                else if ($scope.searchFilter.selectedCategory != 0 && $scope.searchFilter.IsFormer == 0) {
                    return person.statusId == $scope.searchFilter.selectedCategory && person.statusActive == 1;
                }
                else if ($scope.searchFilter.selectedCategory == 0 && $scope.searchFilter.IsFormer != 0) {
                    return false;
                }
                else {
                    return person.statusId == $scope.searchFilter.selectedCategory && person.statusActive == 0;
                }
        }
		//Create new obj after hitting "Add" button
        $scope.addNewAddress = function () {
            $scope.personAddressIdList = [];
            for (var i = 0; i < $scope.selectedpersonAddressInfo.length; i++) {
                $scope.personAddressIdList.push($scope.selectedpersonAddressInfo[i].AddressTypeId);
            }
            $scope.selectedpersonAddressInfo.push({});
        };

        $scope.addNewPersonCategory = function () {
            $scope.personCategoryInfoIdList = [];
            for (var i = 0; i < $scope.selectedpersonCategoryInfo.length; i++) {
                $scope.personCategoryInfoIdList.push($scope.selectedpersonCategoryInfo[i].PersonCategoryCD);
            }
            $scope.selectedpersonCategoryInfo.push({});
        };
		//Initial multi-select item in "Person Category" object.
        $scope.PersonCategoryClinicInit = function (personCategoryInfo) {
            personCategoryInfo.ClinicLists = new Object();
            //$scope.selectedPersonCategoryClinicList[index] = new Object();
            for (var i = 0; i < ClinicList.length; i++) {
                personCategoryInfo.ClinicLists[ClinicList[i].ClinicCD] = false;
            }
            var i = 0;
            if (personCategoryInfo.ClinicIds != null) {
                for (var i = 0; i < personCategoryInfo.ClinicIds.split(',').length; i++) {
                    personCategoryInfo.ClinicLists[personCategoryInfo.ClinicIds.split(',')[i]] = true;
                }
            }

        }
		//Save all the person information and send to back through web service
        $scope.SavePersonInfo = function () {
            var AddressPreferredChecker = 0;

            for (var i = 0; i < $scope.selectedpersonAddressInfo.length; i++) {
                $scope.selectedpersonAddressInfo[i].Street1 = isNullString($scope.selectedpersonAddressInfo[i].Street1);
                $scope.selectedpersonAddressInfo[i].Street2 = isNullString($scope.selectedpersonAddressInfo[i].Street2);
                $scope.selectedpersonAddressInfo[i].Street3 = isNullString($scope.selectedpersonAddressInfo[i].Street3);
                $scope.selectedpersonAddressInfo[i].City = isNullString($scope.selectedpersonAddressInfo[i].City);
                $scope.selectedpersonAddressInfo[i].StateId = isNullint($scope.selectedpersonAddressInfo[i].StateId);
                $scope.selectedpersonAddressInfo[i].PostalCode = isNullString($scope.selectedpersonAddressInfo[i].PostalCode);
                $scope.selectedpersonAddressInfo[i].country = isNullString($scope.selectedpersonAddressInfo[i].country);
                $scope.selectedpersonAddressInfo[i].IsPreferred = isNullboolean($scope.selectedpersonAddressInfo[i].IsPreferred);
                if ($scope.selectedpersonAddressInfo[i].IsPreferred == true) {
                    AddressPreferredChecker++;
                }
            }
            for (var i = 0; i < $scope.selectedpersonCategoryInfo.length; i++) {
                $scope.selectedpersonCategoryInfo[i].TitleId = isNullint($scope.selectedpersonCategoryInfo[i].TitleId);
                $scope.selectedpersonCategoryInfo[i].PartnerPersonId = isNullint($scope.selectedpersonCategoryInfo[i].PartnerPersonId);
                $scope.selectedpersonCategoryInfo[i].AssistantPersonId = isNullint($scope.selectedpersonCategoryInfo[i].AssistantPersonId);

                $scope.selectedpersonCategoryInfo[i].EffectiveDate = isNullString($scope.selectedpersonCategoryInfo[i].EffectiveDate);
                $scope.selectedpersonCategoryInfo[i].TermDate = isNullString($scope.selectedpersonCategoryInfo[i].TermDate);

                $scope.selectedpersonCategoryInfo[i].Position = isNullString($scope.selectedpersonCategoryInfo[i].Position);
                $scope.selectedpersonCategoryInfo[i].YearGraduation = isNullint($scope.selectedpersonCategoryInfo[i].YearGraduation);
                $scope.selectedpersonCategoryInfo[i].FellowSpecification = isNullString($scope.selectedpersonCategoryInfo[i].FellowSpecification);
                $scope.selectedpersonCategoryInfo[i].Note = isNullString($scope.selectedpersonCategoryInfo[i].Note);
                $scope.selectedpersonCategoryInfo[i].IsCompletedProgram = isNullboolean($scope.selectedpersonCategoryInfo[i].IsCompletedProgram);
                $scope.selectedpersonCategoryInfo[i].IsIntegratedResident = isNullboolean($scope.selectedpersonCategoryInfo[i].IsIntegratedResident);

                $scope.selectedpersonCategoryInfo[i].YearOfLastGift = isNullint($scope.selectedpersonCategoryInfo[i].YearOfLastGift);
                $scope.selectedpersonCategoryInfo[i].TotalDonation = isNullint($scope.selectedpersonCategoryInfo[i].TotalDonation);
                $scope.selectedpersonCategoryInfo[i].DesignationCD = isNullint($scope.selectedpersonCategoryInfo[i].DesignationCD);
                $scope.selectedpersonCategoryInfo[i].IsFormer = isNullboolean($scope.selectedpersonCategoryInfo[i].IsFormer);

                var newClinicIds = [];
                for (var j in $scope.selectedpersonCategoryInfo[i].ClinicLists) {
                    if ($scope.selectedpersonCategoryInfo[i].ClinicLists[j] == true) {
                        newClinicIds.push(j)
                    }
                }
                $scope.selectedpersonCategoryInfo[i].ClinicIds = isNullString(newClinicIds.join(","));
            }

            //check if multiple address has only and at least one preferred checked.
            if ($scope.selectedpersonAddressInfo.length > 1 && AddressPreferredChecker != 1) {
                window.alert("Preferred address needs to be exactly 1.");
            }
            else {
                var url = "Home.aspx/SavePersonInfo";
                var config = { headers: { "Content-Type": "application/json" } };
                var submitData = "{\"fname\" : \"" + isNullString($scope.selectedPersonInfo.FirstName)
                    + "\",\"mname\": \"" + isNullString($scope.selectedPersonInfo.MiddleName)
                    + "\",\"lname\": \"" + isNullString($scope.selectedPersonInfo.LastName)
                    + "\",\"preferredName\": \"" + isNullString($scope.selectedPersonInfo.preferredName)
                    + "\",\"personId\": \"" + isNullint($scope.selectedPersonInfo.personid)
                    //+ "\",\"IDXProviderNumber\": \"" + isNullint($scope.selectedPersonInfo.IDXProviderNumber)
                    + "\",\"departmentTitleId\": \"" + isNullint($scope.selectedPersonInfo.departmentTitleId)
                    //+ "\",\"DegreeId\": \"" + isNullInt($scope.selectedPersonInfo.Degree)
                    + "\",\"EducationInfo\": \"" + isNullString($scope.selectedPersonInfo.EducationInfo)
                    + "\",\"SalutationId\": \"" + isNullint($scope.selectedPersonInfo.SalutationId)
                    + "\",\"sexId\": \"" + isNullint($scope.selectedPersonInfo.SexId)
                    + "\",\"email\": \"" + isNullString($scope.selectedPersonInfo.EmailAddress)
                    + "\",\"alteremail\": \"" + isNullString($scope.selectedPersonInfo.AlterEmailAddress)
                    + "\",\"pawprint\": \"" + isNullString($scope.selectedPersonInfo.pawprint)
                    + "\",\"EmployeeID\": \"" + isNullString($scope.selectedPersonInfo.EmployeeID)
                    + "\",\"homephone\": \"" + isNullString($scope.selectedPersonInfo.homephone)
                    + "\",\"workphone\": \"" + isNullString($scope.selectedPersonInfo.workphone)
                    + "\",\"cellphone\": \"" + isNullString($scope.selectedPersonInfo.cellphone)
                    + "\",\"PartnerPersonId\": \"" + isNullint($scope.selectedPersonInfo.PartnerPersonId)
                    + "\",\"Note\": \"" + isNullString($scope.selectedPersonInfo.Note)
                    + "\",\"IsDeceased\": \"" + isNullboolean($scope.selectedPersonInfo.IsDeceased)
                    + "\",\"IsNonSolicit\": \"" + isNullboolean($scope.selectedPersonInfo.IsNonSolicit)
                    + "\",\"AddressInfo\": " + isNullString(JSON.stringify($scope.selectedpersonAddressInfo))
                    + ",\"CategoryInfo\": " + isNullString(JSON.stringify($scope.selectedpersonCategoryInfo))
                    + " }";
                $http.post(url, submitData, { cache: false }).success(function (data, status, headers, config) {
                    var s = String(data.d);
                    if (s.substring(0, 5) == "Error") {
                        window.alert(s);
                    }
                    else {
                        window.alert("Person Info has been saved successfullly");
                        $scope.PersonList = data.d != '' ? JSON.parse(data.d) : "";
                        $scope.close();
                    }
                }).error(function (data, status, headers, config) {
                    window.alert(data.Message);
                });
            }
        };
        //Delete person by setting IsActive to 0
		$scope.InactivePerson = function () {
            if (confirm('Are you sure you want to delete?')) {
                if ($scope.selectedPersonInfo.personid != null && $scope.selectedPersonInfo.personid != 0) {
                    var url = "Home.aspx/InactivePerson";
                    var config = { headers: { "Content-Type": "application/json" } };
                    var submitData = "{\"personId\" : \"" + isNullint($scope.selectedPersonInfo.personid)
                        + "\" }";
                    $http.post(url, submitData, { cache: false }).success(function (data, status, headers, config) {
                        var s = String(data.d);
                        if (s.substring(0, 5) == "Error") {
                            window.alert(s);
                        }
                        else {
                            window.alert("Delete this person Success");
                            $scope.PersonList = data.d != '' ? JSON.parse(data.d) : "";
                            $scope.Close();
                        }
                    }).error(function (data, status, headers, config) {
                        window.alert("Delete this person failed");
                    });
                }
                else {
                    window.alert("Please select a person first!");
                }
            }
            else {
                console.log('Thing was not saved to the database.');
            }


        };
        //Add restriction on "Person Category", so that user cannot choose same Person Category for same person, and set existing Person Category non-changable.
		$scope.personCategoryfilter = function (index) {
            return function (item) {
                if ($scope.selectedpersonCategoryInfo[index].PersonCategoryCD != null) // if this item is not new added
                {
                    return item.PersonCategoryCD == $scope.selectedpersonCategoryInfo[index].PersonCategoryCD;
                }
                else {
                    return $scope.personCategoryInfoIdList.indexOf(item.PersonCategoryCD) == -1;
                }
            }
        }
		//Add restriction on "Person Address", so that user cannot choose same Person Address for same person, and set existing Person Address non-changable.
        $scope.personAddressfilter = function (index) {
            return function (item) {
                if ($scope.selectedpersonAddressInfo[index].AddressTypeId != null) // if this item is not new added
                {
                    return item.AddressTypeId == $scope.selectedpersonAddressInfo[index].AddressTypeId;
                }
                else {
                    return $scope.personAddressIdList.indexOf(item.AddressTypeId) == -1;
                }
            }
        }

  

    }])
    .controller("DonationMgmtCtrl", ['$scope', '$location', '$http', '$dialog', 'ui.config', '$timeout', '$route', '$window', function ($scope, $location, $http, $dialog, uiConfig, $timeout, $route)
    {
  
    }])
    .controller("ClinicSchedulerCtrl", ['$scope', '$location', '$http', '$dialog', 'ui.config', '$timeout', '$route', '$window', function ($scope, $location, $http, $dialog, uiConfig, $timeout, $route, $window) 
    {
     
    }])

    .controller("MBMCtrl", ['$scope', 'MSETSGV', 'RequestSrv', 'local', '$http', 'dialog', 'ui.config', '$timeout', function ($scope, MSETSGV, RequestSrv, local, $http, dialog, uiConfig, $timeout)
    {
		
    }])

    .controller("LOAMgmtCtrl", ['$scope', 'MSETSGV', 'RequestSrv', 'local', '$http', 'dialog', 'ui.config', '$timeout', function ($scope, MSETSGV, RequestSrv, local, $http, dialog, uiConfig, $timeout)
    {
		
    }])

    .controller("RVUMgmtCtrl", ['$scope', 'MSETSGV', 'RequestSrv', 'local', '$http', 'dialog', 'ui.config', '$timeout', function ($scope, MSETSGV, RequestSrv, local, $http, dialog, uiConfig, $timeout)
    {

    }])

    .controller("ReportsCtrlCtrl", ['$scope', '$location', '$http', '$dialog', 'ui.config', '$timeout', '$route', '$window', function ($scope, $location, $http, $dialog, uiConfig, $timeout, $route)
    {
       
    }])

    .controller("SystemSetupCtrl", ['$scope', '$location', '$http', '$dialog', 'ui.config', '$timeout', '$route', '$window', function ($scope, $location, $http, $dialog, uiConfig, $timeout, $route) {
       
    }])
    .controller("PayMgmtCtrl", ['$scope', 'MSETSGV', 'RequestSrv', 'local', '$http', 'dialog', 'ui.config', '$timeout', function ($scope, MSETSGV, RequestSrv, local, $http, dialog, uiConfig, $timeout) {

	}])
 }])




   


