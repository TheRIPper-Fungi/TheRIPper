(function () {
    'use strict';

    angular
        .module('app')
        .factory('accountFactory', accountFactory);

    accountFactory.$inject = ['$http'];

    function accountFactory($http) {
        var service = {
            getData: getData
        };

        return service;

        function getData() { }
    }
})();