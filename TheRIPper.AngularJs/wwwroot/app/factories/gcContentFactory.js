(function () {
    'use strict';

    angular
        .module('app')
        .factory('gcContentFactory', gcContentFactory);

    gcContentFactory.$inject = ['$http'];

    function gcContentFactory($http) {
        var service = {
            GCContentSingleSequenceTotal: GCContentSingleSequenceTotal,
            GCContentFileTotal: GCContentFileTotal
        };

        return service;

        function GCContentSingleSequenceTotal(sequenceId) {
            return $http.get('api/gccontent/sequence/' + sequenceId)
                .then(function (data) { return data; })
                .then(function (data) { return data.data; })
        }

        function GCContentFileTotal(FileId) {
            return $http.get('api/gccontent/file/' + FileId)
                .then(function (data) { return data; })
                .then(function (data) { return data.data; })
        }
    }
})();