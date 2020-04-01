(function () {
    'use strict';

    angular
        .module('app')
        .factory('gff3Factory', gff3Factory);

    gff3Factory.$inject = ['$http'];

    function gff3Factory($http) {
        var service = {
            GFF3File: GFF3File,
        };

        return service;
        //api/gff3/file/{FileName}/{window}/{slide}/{compositeRequirement}/{productRequirement}/{substrateRequirement}/{compositeCountRequirement}/{checkGcContent}
        function GFF3File() {
            return $http.get('/api/gff3/file/' + FileName + '/' + window + '/' + slide + '/' + compositeRequirement + '/' + productRequirement + '/' + substrateRequirement + '/' + compositeCountRequirement + '/' + checkGcContent)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }

       
    }
})();