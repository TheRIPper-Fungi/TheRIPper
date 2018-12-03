(function () {
    'use strict';

    angular
        .module('app')
        .controller('sequenceCtrl', sequenceCtrl);

    sequenceCtrl.$inject = ['$scope', '$routeParams','sequenceFactory'];

    function sequenceCtrl($scope, $routeParams, sequenceFactory) {
        $scope.title = 'sequenceCtrl';
        $scope.FileId = $routeParams.FileId;
        $scope.FileName = $routeParams.FileName
        $scope.loadingSequences = true;
        activate();

        function activate() {
            sequenceFactory.list($scope.FileId).then(data => {
                $scope.sequences = data;
                $scope.loadingSequences = false;
            })
        }
    }
})();
