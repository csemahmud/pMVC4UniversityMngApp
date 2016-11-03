pMVC4UniversityMngApp.controller('AllocatedRoomsController',
    function ($scope, AllocatedRoomsService, SELECT_STR) {

        getAllocatedRooms(SELECT_STR);
        function getAllocatedRooms(SELECT_STR) {
            AllocatedRoomsService.getAllocatedRooms()
                .success(function (responce) {
                    $scope.courses = responce.courses;
                    console.log($scope.courses);

                    $scope.message = "Courses with Allocated Rooms : ";
                    $scope.deptCode = SELECT_STR;
                    $scope.selectStr = SELECT_STR;

                    $scope.limitRange = [];
                    var maxPageSize = 10;
                    if ($scope.courses.length < 10) {
                        maxPageSize = $scope.courses.length;
                    }
                    for (var i = 1; i <= maxPageSize; i++) {
                        $scope.limitRange.push(i);
                    }

                    $scope.pageSize = 5;
                    if ($scope.courses.length < 5) {
                        $scope.pageSize = $scope.courses.length;
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

                            for (var i = 0; i < $scope.courses.length; i++) {
                                if ($scope.courses[i].DeptCode == $scope.deptCode) {
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
                            if ($scope.courses.length < 10) {
                                maxPageSize = $scope.courses.length;
                            }

                            for (var i = 1; i <= maxPageSize; i++) {
                                $scope.limitRange.push(i);
                            }

                            if ($scope.courses.length < 5) {
                                $scope.pageSize = $scope.courses.length;
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
                    $scope.status = 'Unable to load Allocated Rooms data: ' + error.message;
                    console.log($scope.status);
                    alert($scope.status);
                });
        }

    });