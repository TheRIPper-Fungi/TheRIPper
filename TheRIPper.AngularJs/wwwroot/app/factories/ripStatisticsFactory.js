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

        function getFileProfile(FileId, window, slide, compositeRequirement, compositeCountRequirement) {
             return $http.get('api/rip/profile/file/' + FileId + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + compositeCountRequirement)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }
    }
})();