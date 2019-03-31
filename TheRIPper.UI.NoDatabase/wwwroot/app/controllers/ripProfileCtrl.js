(function () {
    'use strict';

    angular
        .module('app')
        .controller('ripProfileCtrl', ripProfileCtrl);

    ripProfileCtrl.$inject = ['$scope', '$routeParams', 'ripProfileFactory'];

    function ripProfileCtrl($scope, $routeParams, ripProfileFactory) {
        $scope.title = 'ripProfileCtrl';
        $scope.ripProfileLoading = true;

        $scope.window = 1000;
        $scope.slide = 500;
        $scope.compositeRequirement = 0.01;
        $scope.productRequirement = 1.1;
        $scope.substrateRequirement = 0.9;
        $scope.compositeCountRequirement = 7;

        activate();


        $scope.ripProfileGridOptions = {
            multiSelect: false,
            enableSorting: true,
            enableFiltering: true,
            enableGridMenu: true,
            //export options
            exporterCsvFilename: 'RIPProfile.csv',
            exporterExcelFilename: 'RIPProfile.xlsx',
            exporterExcelSheetName: 'RIPProfile',
            exporterPdfDefaultStyle: { fontSize: 8 },
            exporterPdfTableStyle: { margin: [10, 10, 10, 10] },
            exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'blue' },
            exporterPdfHeader: { text: "RIP Profile Report", style: 'headerStyle' },
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
                { field: 'Count', width: 150 },
                { field: 'FileBP', width: 150 },
                { field: 'ProductAverage', width: 150 },
                { field: 'SubstrateAverage', width: 150 },
                { field: 'CompositeAverage', width: 150 },
                { field: 'SumOfLRAR', width: 150 },
                { field: 'EstimatedGenomeRIP', width: 150 },
                { field: 'SumAverage', width: 150 }
            ],
            onRegisterApi: function (gridApi) {
                //$scope.clientTargetsGridApi = gridApi;
            },
        };

        $scope.exportProfileAsCSV = function () {
            var data = $scope.profile;
            var CSVContent = "data:text/csv;charset=utf-8,";
            CSVContent = CSVContent + "Name" + "," + data.FileName + "\r\n";
            CSVContent = CSVContent + "Genome Size (bp)" + "," + data.FileBP + "\r\n";
            CSVContent = CSVContent + "Count of Genomic Windows Investigated" + "," + data.WindowsInvestigated + "\r\n";
            CSVContent = CSVContent + "GC Content of Genome Assembly (%)" + "," + data.TotalGCContent + "\r\n";
            CSVContent = CSVContent + "Number of RIP Affected Windows" + "," + data.RIPPositiveWindows + "\r\n";
            CSVContent = CSVContent + "RIP Affected Genomic Proportion (%)" + "," + data.EstimatedGenomeRIP + "\r\n";
            CSVContent = CSVContent + "Count of LRARs" + "," + data.Count + "\r\n";
            CSVContent = CSVContent + "Average Size of LRARs (bp)" + "," + data.SumAverage + "\r\n";
            CSVContent = CSVContent + "Average GC Content of LRARs (%)" + "," + data.LRARAverageGCContent + "\r\n";
            CSVContent = CSVContent + "Genomic Proportion of LRARs (bp)" + "," + data.SumOfLRAR + "\r\n";
            CSVContent = CSVContent + "Product Value for LRARs" + "," + data.ProductAverage + "\r\n";
            CSVContent = CSVContent + "Substrate Value for LRARs" + "," + data.SubstrateAverage + "\r\n";
            CSVContent = CSVContent + "Composite Value for LRARs" + "," + data.CompositeAverage;

            var encodedUri = encodeURI(CSVContent);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);
            link.setAttribute("download", data.FileName + "_profile.csv");
            link.innerHTML = "Click Here to download";
            document.body.appendChild(link); // Required for FF

            link.click(); // This will download the data file named "my_data.csv".

        }

        $scope.update = function () {
            activate();
        };

        function activate() {
            $scope.ripProfileLoading = true;
            $scope.FileName = $routeParams.FileName;
            $scope.SequenceId = $routeParams.SequenceId;

            if ($scope.FileName !== undefined) {
                ripProfileFactory.getFileProfile($scope.FileName, $scope.window, $scope.slide, $scope.compositeRequirement, $scope.productRequirement, $scope.substrateRequirement, $scope.compositeCountRequirement, $scope.checkGcContent)
                    .then(function (data) {
                        $scope.profile = data;

                        let data_array = [];
                        data_array.push(data);

                        $scope.ripProfileGridOptions.data = data_array;

                        $scope.ripProfileLoading = false;
                    })

            }
            //else if ($scope.SequenceId !== undefined) {

            //}
            //else {

            //}

        }
    }
})();
