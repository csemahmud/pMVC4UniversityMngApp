pMVC4UniversityMngApp.controller('AssignedCoursesController',
    function ($scope, AssignedCoursesService, SELECT_STR) {

        getAssignedCourses(SELECT_STR);
        function getAssignedCourses(SELECT_STR) {
            AssignedCoursesService.getAssignedCourses()
                .success(function (responce) {
                    $scope.assignedCourses = responce.assignedCourses;
                    console.log($scope.assignedCourses);

                    $scope.message = "Assigned Courses : ";
                    $scope.deptCode = SELECT_STR;
                    $scope.selectStr = SELECT_STR;

                    $scope.limitRange = [];
                    var maxPageSize = 10;
                    if ($scope.assignedCourses.length < 10) {
                        maxPageSize = $scope.assignedCourses.length;
                    }
                    for (var i = 1; i <= maxPageSize; i++) {
                        $scope.limitRange.push(i);
                    }

                    $scope.pageSize = 5;
                    if ($scope.assignedCourses.length < 5) {
                        $scope.pageSize = $scope.assignedCourses.length;
                    }

                    $scope.selectedPage = 1;
                    $scope.resetPage = function () {
                        $scope.selectedPage = 1;
                    };
                    $scope.resetSizeNPage = function () {
                        $scope.pageSize = 5;
                        $scope.selectedPage = 1;

                        $scope.limitRange = [];
                        maxPageSize = 10;

                        if ($scope.deptCode != $scope.selectStr) {
                            var count = 0;

                            for (var i = 0; i < $scope.assignedCourses.length; i++) {
                                if ($scope.assignedCourses[i].DeptCode == $scope.deptCode) {
                                    count++;
                                }
                            }

                            if (count < 10) {
                                maxPageSize = count;
                            }

                            for (var i = 1; i <= count; i++) {
                                $scope.limitRange.push(i);
                            }

                            if (count < 5) {
                                $scope.pageSize = count;
                            }
                        } else {
                            if ($scope.assignedCourses.length < 10) {
                                maxPageSize = $scope.assignedCourses.length;
                            }

                            for (var i = 1; i <= maxPageSize; i++) {
                                $scope.limitRange.push(i);
                            }

                            if ($scope.assignedCourses.length < 5) {
                                $scope.pageSize = $scope.assignedCourses.length;
                            }
                        }
                    };
                    $scope.IsCourseSelected = function (course) {
                        return $scope.deptCode == $scope.selectStr || $scope.deptCode == course.DeptCode;
                    }

                    $scope.selectPage = function (pageNo) {
                        $scope.selectedPage = pageNo;
                    }

                    $scope.getBtnClass = function (pageNo) {
                        return $scope.selectedPage == pageNo ? "active" : "";
                    }
                })
                .error(function (error) {
                    $scope.status = 'Unable to load Assigned Course data: ' + error.message;
                    console.log($scope.status);
                    alert($scope.status);
                });
        }

    });