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

        function getSequence(FileName, SequenceName, window, slide, compositeRequirement, compositeCountRequirement) {
            return $http.get('api/rip/lrar/sequence/' + FileName + '/' + SequenceName + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + compositeCountRequirement)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); });

        }

        function getFile(FileName, window, slide, compositeRequirement, compositeCountRequirement) {
            return $http.get('api/rip/lrar/file/' + FileName + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + compositeCountRequirement)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); });
        }
    }
})();