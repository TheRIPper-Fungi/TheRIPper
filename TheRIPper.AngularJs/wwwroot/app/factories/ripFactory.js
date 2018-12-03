(function () {
    'use strict';

    angular
        .module('app')
        .factory('ripFactory', ripFactory);

    ripFactory.$inject = ['$http'];

    function ripFactory($http) {
        var service = {
            test: test,
            RIPIndexes: RIPIndexes,
            RIPSequence: RIPSequence,
            RIPFile: RIPFile
        };

        return service;

        function test() {
            return $http.get('/api/rip')
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }

        function RIPIndexes(FileId) {
            return $http.get('api/rip/indexes/'+FileId)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }

        function RIPSequence(SequenceId, WindowSize, SlidingSize) {

            if (WindowSize !== undefined && SlidingSize !== undefined) {
                return $http.get('/api/rip/sequence/' + SequenceId + "/" + WindowSize + "/" + SlidingSize)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            else {
                return $http.get('/api/rip/sequence/' + SequenceId)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            
        }

        function RIPFile(FileId, WindowSize, SlidingSize) {
            if (WindowSize !== undefined && SlidingSize !== undefined) {
                return $http.get('/api/rip/file/' + FileId + "/" + WindowSize + "/" + SlidingSize)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            else {
                return $http.get('api/rip/file/' + FileId)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            
        }
    }
})();