(function () {
    'use strict';

    angular
        .module('app')
        .factory('fileFactory', fileFactory);

    fileFactory.$inject = ['$http'];

    function fileFactory($http) {
        var service = {
            list:list
        };

        return service;

        //function save(FileName, Description, FilePath) {
        //    let file = {
        //        FileName: FileName,
        //        Description: Description,
        //        Location: FilePath
        //    };


        //    return $http({
        //        method: 'POST',
        //        url: 'api/files/save',
        //        data: file,
        //        headers: { 'Content-Type': 'application/json' }
        //    })
        //        .then(function (data) { return data; })
        //        .then(function (data) { return data; })

        //}

        function list() {
            return $http({
                method: 'GET',
                url: 'api/files/list',
                headers: { 'Content-Type': 'application/json' }
            })
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data).files; })
        }

        //function remove(FileId) {
        //    return $http.delete('api/files/remove/' + FileId)
        //        .then(function (data) { return data })
        //        .then(function (data) { return JSON.parse(data.data) })

        //}
    }
})();