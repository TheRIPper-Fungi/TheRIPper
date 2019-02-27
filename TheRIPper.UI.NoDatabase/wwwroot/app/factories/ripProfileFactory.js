(function () {
    'use strict';

    angular
        .module('app')
        .factory('ripProfileFactory', ripProfileFactory);

    ripProfileFactory.$inject = ['$http'];

    function ripProfileFactory($http) {
        var service = {
            getFileProfile: getFileProfile
        };

        return service;

        function getFileProfile(FileName, window, slide, compositeRequirement, compositeCountRequirement, checkGcContent) {
            return $http.get('api/rip/profile/file/' + FileName + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + compositeCountRequirement + '/' + checkGcContent)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); });
        }
    }
})();