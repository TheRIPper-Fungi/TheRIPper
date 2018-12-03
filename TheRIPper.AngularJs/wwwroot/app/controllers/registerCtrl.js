angular.module('register', [])
    .controller('registerCtrl',['$scope','$http', function ($scope, $http) {
        $scope.register = function()
        {
            var params = {
                Email: $scope.username,
                Password: $scope.password1,
                ConfirmPassword: $scope.password2
            };
            $http.post('/api/Account/Register', params)
            .success(function (data, status, headers, config) {
                $scope.successMessage = "Registration Complete.  Please Sign In.";
                $scope.showErrorMessage = false;
                $scope.showSuccessMessage = true;
            })
                .error(function (data, status, headers, config) {

                    $scope.errorMessages = [];
                    if (angular.isArray(data))
                        $scope.errorMessages = data;
                    else {
                        if (data.Email !== undefined) {
                            $scope.errorMessages.push("Email : " + data.Email);
                        }
                        if (data.Password !== undefined) {
                            $scope.errorMessages.push("Password : " + data.Password);
                        }
                        if (typeof (data[""] === "array")) {
                            data[""].forEach(f => {
                                $scope.errorMessages.push("Password : " + f);
                            })
                        }
                    }
                    //$scope.errorMessages = new Array(data.replace(/["']{1}/gi, ""));

                $scope.showSuccessMessage = false;
                $scope.showErrorMessage = true;
            });
        }

        $scope.showAlert = false;
        $scope.showSuccess = false;
    }]);