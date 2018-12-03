(function () {
    'use strict';

    angular
        .module('app')
        .controller('ripProfileCtrl', ripProfileCtrl);

    ripProfileCtrl.$inject = ['$scope', '$routeParams','ripProfileFactory'];

    function ripProfileCtrl($scope, $routeParams, ripProfileFactory) {
        $scope.title = 'ripProfileCtrl';
        $scope.ripProfileLoading = true;
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
                { field: 'SumAverage', width:150}
            ],
            onRegisterApi: function (gridApi) {
                //$scope.clientTargetsGridApi = gridApi;
            },
        };

        $scope.exportProfileAsCSV = function () {
            var data = $scope.profile;
            var CSVContent = "data:text/csv;charset=utf-8,";
            CSVContent = CSVContent + "Name" +","+ data.FileName + "\r\n";
            CSVContent = CSVContent + "Genome Size (bp)" + "," + data.FileBP + "\r\n";
            CSVContent = CSVContent + "Windows Investigated" + "," + data.WindowsInvestigated + "\r\n";
            CSVContent = CSVContent + "GC content of entire genome (%)" + "," + data.TotalGCContent + "\r\n";
            CSVContent = CSVContent + "RIP Positive Windows" + "," + data.RIPPositiveWindows + "\r\n";
            CSVContent = CSVContent + "Total estimated Genome-wide RIP (%)" + "," + data.EstimatedGenomeRIP + "\r\n";
            CSVContent = CSVContent + "Count Of LRAR" + "," + data.Count + "\r\n";
            CSVContent = CSVContent + "Average size of LRAR (bp)" + "," + data.SumAverage + "\r\n";
            CSVContent = CSVContent + "Average GC Content of LRAR (%)" + "," + data.LRARAverageGCContent + "\r\n";
            CSVContent = CSVContent + "Sum Of all LRAR (bp)" + "," + data.SumOfLRAR + "\r\n";
            CSVContent = CSVContent + "Product Average for LRAR" + "," + data.ProductAverage + "\r\n";
            CSVContent = CSVContent + "Substrate Average for LRAR" + "," + data.SubstrateAverage + "\r\n";
            CSVContent = CSVContent + "Composite Average for LRAR" + "," + data.CompositeAverage;

            var encodedUri = encodeURI(CSVContent);
            var link = document.createElement("a");
            link.setAttribute("href", encodedUri);
            link.setAttribute("download", data.FileName + "_profile.csv");
            link.innerHTML = "Click Here to download";
            document.body.appendChild(link); // Required for FF

            link.click(); // This will download the data file named "my_data.csv".
           
        }

        function activate() {
            $scope.ripProfileLoading = true;
            $scope.FileId = $routeParams.FileId;
            $scope.SequenceId = $routeParams.SequenceId;

            if ($scope.FileId !== undefined) {
                ripProfileFactory.getFileProfile($scope.FileId, 1000, 500, 0.01, 7)
                    .then(function (data) {
                        $scope.profile = data;

                        let data_array = [];
                        data_array.push(data);

                        $scope.ripProfileGridOptions.data = data_array;

                        $scope.ripProfileLoading = false;
                    })

            }
            else if ($scope.SequenceId !== undefined) {

            }
            else {

            }

        }
    }
})();
