(function () {
    'use strict';

    angular
        .module('app')
        .controller('lrarCtrl', lrarCtrl);

    lrarCtrl.$inject = ['$scope', '$routeParams', 'lrarFactory', 'uiGridConstants'];

    function lrarCtrl($scope, $routeParams, lrarFactory, uiGridConstants) {
        $scope.title = 'lrarCtrl';
        $scope.lrarLoading = true;
        $scope.chartSize = {};
        $scope.sequenceSize = {};

        $scope.window = 1000;
        $scope.slide = 500;
        $scope.compositeRequirement = 0.01;
        $scope.productRequirement = 1.1;
        $scope.substrateRequirement = 0.75;
        $scope.compositeCountRequirement = 7;

        activate();

        $scope.lrarGridOptions = {
            multiSelect: false,
            showGridFooter: true,
            showColumnFooter: true,
            enableSorting: true,
            enableFiltering: true,
            enableGridMenu: true,
            //export options
            exporterCsvFilename: 'LRAR analysis.csv',
            exporterExcelFilename: 'LRAR analysis.xlsx',
            exporterExcelSheetName: 'LRAR analysis',
            exporterPdfDefaultStyle: { fontSize: 8 },
            exporterPdfTableStyle: { margin: [10, 10, 10, 10] },
            exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'blue' },
            exporterPdfHeader: { text: "LRAR analysis", style: 'headerStyle' },
            exporterPdfFooter: function (currentPage, pageCount) {
                return { text: currentPage.toString() + ' of ' + pageCount.toString(), style: 'footerStyle' };
            },
            exporterPdfCustomFormatter: function (docDefinition) {
                docDefinition.styles.headerStyle = { fontSize: 22, bold: true };
                docDefinition.styles.footerStyle = { fontSize: 10, bold: true };
                return docDefinition;
            },
            exporterPdfOrientation: 'landscape',
            exporterPdfPageSize: 'LETTER',
            exporterPdfMaxGridWidth: 600,
            exporterCsvLinkElement: angular.element(document.querySelectorAll(".custom-csv-link-location")),
            //end export

            columnDefs: [
                { field: 'Name', width: 150 },
                {
                    field: 'Start', width: 150, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] },
                {
                    field: 'End', width: 150, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] },
                {
                    field: 'Size', aggregationType: uiGridConstants.aggregationTypes.sum, width: 150, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] },
                {
                    field: 'Count', displayName: "Count Of Windows", aggregationType: uiGridConstants.aggregationTypes.sum, width: 75, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ]  },
                {
                    field: 'Product', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] },
                {
                    field: 'Substrate', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] },
                {
                    field: 'Composite', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] },
                {
                    field: 'GCContent', footerCellFilter: 'number:2', displayName: "GC Content", aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number', filters: [
                        {
                            condition: uiGridConstants.filter.GREATER_THAN,
                            placeholder: '>'
                        },
                        {
                            condition: uiGridConstants.filter.LESS_THAN,
                            placeholder: '<'
                        }
                    ] }
            ],
            onRegisterApi: function (gridApi) {
                //$scope.clientTargetsGridApi = gridApi;
            }
        };

        $scope.update = function () { activate(); };

        function activate() {
            $scope.lrarLoading = true;

            $scope.SequenceName = $routeParams.SequenceName;
            $scope.FileName = $routeParams.FileName;

            if ($scope.SequenceName !== undefined) {
                $scope.type = 's'; //S for Sequence

                lrarFactory.getSequence($scope.FileName, $scope.SequenceName, $scope.window, $scope.slide, $scope.compositeRequirement, $scope.productRequirement, $scope.substrateRequirement, $scope.compositeCountRequirement, $scope.checkGcContent)
                    .then(function (data) {
                        $scope.lrarGridOptions.data = data;

                        ChartSize();

                        $scope.lrarLoading = false;
                    });
            }
            else if ($scope.FileName !== undefined) {
                $scope.type = 'f';

                lrarFactory.getFile($scope.FileName, $scope.window, $scope.slide, $scope.compositeRequirement, $scope.productRequirement, $scope.substrateRequirement, $scope.compositeCountRequirement, $scope.checkGcContent)
                    .then(function (data) {
                        $scope.lrarGridOptions.data = data;

                        ChartSize();

                        $scope.lrarLoading = false;
                    })
            }
        }

        function SequenceLRARGroups() {
            let data = $scope.lrarGridOptions.data;
            let names = data.map(d => { return d.Name });
            let unique_name = [...new Set(names)];

            let bar_data = [];

            unique_name.forEach(n => {
                let filtered_data = data.filter(d => {
                    return d.Name === n
                });
                let filtered_size = filtered_data.map(d => { return d.Size });

                let total_size = filtered_size.reduce(function (total, num) {
                    return total + num;
                }, 0)

                bar_data.push({ Name: n, Size: total_size });
            })
            $scope.sequenceSize.data = bar_data.map(d => { return d.Size });
            $scope.sequenceSize.labels = bar_data.map(d => { return d.Name.slice(0, 12) });
        }

        function ChartSize() {
            SequenceLRARGroups();

            $scope.chartSize.labels = $scope.lrarGridOptions.data.map(function (d) {
                return d.Start + "-" + d.End
            });
            $scope.chartSize.data = $scope.lrarGridOptions.data.map(function (d) {
                return d.Size;
            })
        }
    }
})();