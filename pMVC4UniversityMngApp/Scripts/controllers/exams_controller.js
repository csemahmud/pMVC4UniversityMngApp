pMVC4UniversityMngApp.controller('ExamsController',
    function ($scope, ExamsService, SELECT_STR) {

        getExams(SELECT_STR);
        function getExams(SELECT_STR) {
            ExamsService.getExams()
                .success(function (responce) {
                    $scope.students = responce.students;
                    $scope.student = null;

                    console.log($scope.students);

                    $scope.message = "Enrolled Courses : ";
                    $scope.regNo = SELECT_STR;
                    $scope.selectStr = SELECT_STR;

                    $scope.pageSize = 1;
                    $scope.limitRange = [];
                    $scope.limitRange.push(1);

                    $scope.selectedPage = 1;
                    $scope.resetPage = function () {
                        $scope.selectedPage = 1;
                    };
                    $scope.changePage = function () {
                        $scope.student = null;
                        for (var i = 0; i < $scope.students.length; i++) {
                            if ($scope.students[i].RegNo == $scope.regNo) {
                                $scope.student = $scope.students[i];
                                break;
                            }
                        }

                        $scope.selectedPage = 1;
                        $scope.limitRange = [];
                        $scope.limitRange.push(1);

                        if ($scope.student != null)
                        {
                            var maxPageSize = 10;
                            if ($scope.student.Exams.length < 10) {
                                maxPageSize = $scope.student.Exams.length;
                            }
                            for (var i = 2; i <= maxPageSize; i++) {
                                $scope.limitRange.push(i);
                            }

                            $scope.pageSize = 5;
                            if (student.Exams.length < 5) {
                                $scope.pageSize = $scope.exams.length;
                            }
                        }
                        else {
                            $scope.pageSize = 1;
                        }
                    };

                    $scope.enrollemntDate = function (dateStr) {
                        return new Date(dateStr);
                    }

                    $scope.selectPage = function (pageNo) {
                        $scope.selectedPage = pageNo;
                    }

                    $scope.getBtnClass = function (pageNo) {
                        return $scope.selectedPage == pageNo ? "active" : "";
                    }
                })
                .error(function (error) {
                    $scope.status = 'Unable to Exam data: ' + error.message;
                    console.log($scope.status);
                    alert($scope.status);
                });
        }

    });