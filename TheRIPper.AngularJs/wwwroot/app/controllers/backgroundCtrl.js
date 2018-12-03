(function () {
    'use strict';

    angular
        .module('app')
        .controller('backgroundCtrl', backgroundCtrl);

    backgroundCtrl.$inject = ['$scope'];

    function backgroundCtrl($scope) {
        $scope.title = 'backgroundCtrl';

        activate();

        function activate() { }
    }
})();
