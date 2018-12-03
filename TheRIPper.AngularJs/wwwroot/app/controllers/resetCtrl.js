(function () {
    'use strict';

    angular
        .module('app')
        .controller('resetCtrl', resetCtrl);

    resetCtrl.$inject = ['$scope','$routeParams','$http'];

    function resetCtrl($scope,$routeParams,$http) {
        $scope.title = 'resetCtrl';

        activate();

        function activate() {
            $scope.code = $routeParams.code;
        }

        $scope.resetPassword = function () {
            $http({
                url: '/api/account/reset',
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                data: {
                    Email: $scope.email,
                    Password: $scope.password,
                    ConfirmPassword: $scope.passwordconfirm,
                    Code: $scope.code
                }
            })
        }
    }
})();
