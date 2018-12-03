(function () {
    'use strict';

    angular
        .module('app')
        .controller('forgotCtrl', forgotCtrl);

    forgotCtrl.$inject = ['$scope','$http'];

    function forgotCtrl($scope,$http) {
        $scope.title = 'forgotCtrl';

        activate();

        function activate() { }


        $scope.submitForgotPassword = function () {
            $http({
                url: '/api/account/forgot',
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                data: {
                    Email:$scope.email
                }
            })
        }
    }
})();
