(function () {
    'use strict';

    angular
        .module('app')
        .controller('filesCtrl', filesCtrl);

    filesCtrl.$inject = ['$scope', '$http','fileFactory'];

    function filesCtrl($scope, $http, fileFactory) {
        $scope.title = 'filesCtrl';
        $scope.loadingFiles = true;

        activate();

        function activate() {
            fileFactory.list().then((data) => {
                $scope.files = data;
                $scope.loadingFiles = false;
            });
        }

        $scope.submit = function () {
            if ($scope.form.file.$valid && $scope.file) {
                $scope.upload($scope.file);
            }
        };

        // upload on file select or drop
        //https://stackoverflow.com/questions/38144194/iformfile-is-always-empty-in-asp-net-core-webapi
        $scope.upload = function () {
            $scope.uploadingFile = true;
            var fileUpload = $("#fileup").get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }


            $http.post("/api/files/upload", data, {
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).success(function (data, status, headers, config) {
                    activate();
                    $scope.uploadingFile = false;
                }).error(function (data, status, headers, config) {
                    $scope.uploadingFile = false;
                    alert("Something went wrong!")
            });


        };

    }
})();
