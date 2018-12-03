(function () {
    'use strict';

    angular
        .module('app')
        .factory('lrarFactory', lrarFactory);

    lrarFactory.$inject = ['$http'];

    function lrarFactory($http) {
        var service = {
            getSequence: getSequence,
            getFile: getFile
        };

        return service;

        function getSequence(SequenceId, window, slide, compositeRequirement, compositeCountRequirement) {
            return $http.get('api/rip/lrar/sequence/' + SequenceId + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + compositeCountRequirement)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })

        }

        function getFile(FileId, window, slide, compositeRequirement, compositeCountRequirement) {
            return $http.get('api/rip/lrar/file/' + FileId + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + compositeCountRequirement)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }
    }
})();