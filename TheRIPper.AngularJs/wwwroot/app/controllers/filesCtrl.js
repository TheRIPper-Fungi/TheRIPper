(function () {
    'use strict';

    angular
        .module('app')
        .controller('filesCtrl', filesCtrl);

    filesCtrl.$inject = ['$scope', '$http','fileFactory'];

    function filesCtrl($scope, $http, fileFactory) {
        $scope.title = 'filesCtrl';
        $scope.CurrentFileName = null;
        $scope.file = {};
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
                    $scope.CurrentFileName = JSON.parse(data).FileName;
                $scope.file.FilePath = JSON.parse(data).FileName;
                $scope.uploadingFile = false;
                }).error(function (data, status, headers, config) {
                    alert("Something went wrong!")
            });


        };

        $scope.save = () => {
            $scope.savingFile = true;
            fileFactory.save($scope.file.FileName, $scope.file.FileDescription, $scope.file.FilePath)
                .then(data => {
                    activate();
                    $scope.savingFile = false;
                })

        };

        $scope.removeFile = function(FileId){
            fileFactory.remove(FileId)
                .then(function (data) {
                    if (data.IsRemoved === true) {
                        alert("The file has been removed");
                    }
                    activate();
                })
        }

    }
})();
