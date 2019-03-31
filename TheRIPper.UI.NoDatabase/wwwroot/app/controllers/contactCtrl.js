(function () {
    'use strict';

    angular
        .module('app')
        .controller('contactCtrl', contactCtrl);

    contactCtrl.$inject = ['$scope'];

    function contactCtrl($scope) {
        $scope.title = 'contactCtrl';

        activate();

        function activate() { }
    }
})();
