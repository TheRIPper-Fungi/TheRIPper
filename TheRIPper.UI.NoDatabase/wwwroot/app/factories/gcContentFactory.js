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

        function GCContentSingleSequenceTotal(FileName, SequenceName) {
            return $http.get('api/gccontent/sequence/' + FileName + '/' + SequenceName)
                .then(function (data) { return data; })
                .then(function (data) { return data.data; });
        }

        function GCContentFileTotal(FileName) {
            return $http.get('api/gccontent/file/' + FileName)
                .then(function (data) { return data; })
                .then(function (data) { return data.data; });
        }
    }
})();