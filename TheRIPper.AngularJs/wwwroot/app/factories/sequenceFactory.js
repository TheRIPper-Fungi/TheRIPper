(function () {
    'use strict';

    angular
        .module('app')
        .factory('sequenceFactory', sequenceFactory);

    sequenceFactory.$inject = ['$http'];

    function sequenceFactory($http) {
        var service = {
            list: list
        };

        return service;

        function list(FileId) {
            return $http.get('/api/sequence/' + FileId)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }
    }
})();