(function () {
    'use strict';

    angular
        .module('app')
        .controller('ripCtrl', ripCtrl);

    ripCtrl.$inject = ['$scope', 'ripFactory', 'gcContentFactory', '$routeParams', 'uiGridConstants', '$timeout'];

    function ripCtrl($scope, ripFactory, gcContentFactory, $routeParams, uiGridConstants, $timeout) {
        $scope.title = 'ripCtrl';
        $scope.ripIndex = {};
        $scope.startRange = 0;
        $scope.endRange = 100000;
        $scope.selectedSequence = "";
        $scope.loadingRIP = true;
        $scope.RIPPie = {};
        $scope.GCContentChart = {};
        $scope.window = 1000;
        $scope.slide = 500;

        $scope.ripGridOptions = {
            multiSelect: false,
            enableSorting: true,
            showGridFooter: true,
            showColumnFooter: true,
            enableFiltering: true,
            enableGridMenu: true,

            //export stuffs
            exporterCsvFilename: 'RIPAnalysis.csv',
            exporterExcelFilename: 'RIPAnalysis.xlsx',
            exporterExcelSheetName: 'RIPAnalysis',
            exporterPdfDefaultStyle: { fontSize: 8 },
            exporterPdfTableStyle: { margin: [10, 10, 10, 10] },
            exporterPdfTableHeaderStyle: { fontSize: 10, bold: true, italics: true, color: 'blue' },
            exporterPdfHeader: { text: "RIP Analysis Report", style: 'headerStyle' },
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
                { field: 'SequenceName', width: 270 },
                { field: 'Start', width: 80, type: 'number' },
                { field: 'End', width: 80, type: 'number' },
                { field: 'Product', type: 'number', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 120 },
                { field: 'Substrate', type: 'number', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 120 },
                { field: 'Composite', type: 'number', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 120 },
                { field: 'GCContent', type: 'number', footerCellFilter: 'number:2', displayName: "GC Content", aggregationType: uiGridConstants.aggregationTypes.avg, width: 120 }
            ],
            onRegisterApi: function (gridApi) {
                //$scope.clientTargetsGridApi = gridApi;

                //let GridMenuButton = document.getElementsByClassName('ui-grid-menu-button')[0];
                //GridMenuButton["data-toggle"] = "popover";
                //GridMenuButton["data-content"] = "asdasdasd";
                //data-toggle="popover" data-content="Upload nucleotide genome or scaffold assembly in FASTA format"


            }
        };



        activate();

        function activate() {
            $scope.loadingRIP = true;
            //if ($routeParams.SequenceId)
            $scope.IsSequence = $routeParams.SequenceName !== undefined;
            $scope.IsFile = $routeParams.FileName !== undefined;
            if ($scope.IsSequence) {
                $scope.SequenceName = $routeParams.SequenceName;
                //Logic
                ripFactory.RIPSequence($routeParams.FileName, $scope.SequenceName, $scope.window, $scope.slide).then(data => {
                    $scope.ripGridOptions.data = data;
                    $scope.ripData = data;
                    $scope.seqNames = data.map(m => { return m.SequenceName; });
                    $scope.seqNamesUnique = unique($scope.seqNames);

                    chartTest($scope.startRange, $scope.endRange);

                    $scope.selectedSequence = $scope.seqNamesUnique[0];
                    $scope.updateRange($scope.startRange, $scope.endRange);

                    calculateTotalRIP(data);
                    $scope.loadingRIP = false;

                });

                gcContentFactory.GCContentSingleSequenceTotal($routeParams.FileName, $scope.SequenceName)
                    .then(data => {
                        $scope.GCContent = Number.parseFloat(data).toFixed(2);
                        GCContentChart();
                    });
                //

            }
            else if ($scope.IsFile) {
                $scope.FileName = $routeParams.FileName;
                //Logic
                ripFactory.RIPFile($scope.FileName, $scope.window, $scope.slide)
                    .then(function (data) {
                        $scope.ripGridOptions.data = data;
                        $scope.ripData = data;
                        $scope.seqNames = data.map(m => { return m.SequenceName; });
                        $scope.seqNamesUnique = unique($scope.seqNames);

                        chartTest($scope.startRange, $scope.endRange);
                        calculateTotalRIP($scope.ripData);


                        gcContentFactory.GCContentFileTotal($scope.FileName)
                            .then(data => {
                                $scope.GCContent = Number.parseFloat(data).toFixed(2);
                                GCContentChart();
                                $scope.loadingRIP = false;
                            });                       
                    });
                //

            }
            else {
                //Something went wrong
            }

        }

        $scope.UpdateWindowAndSlide = function () {
            activate();
        }

        $scope.selectSequence = function (name) {
            $scope.selectedSequence = name;
            chartTest($scope.startRange, $scope.endRange);

        }

        function unique(arr) {
            var u = {}, a = [];
            for (var i = 0, l = arr.length; i < l; ++i) {
                if (!u.hasOwnProperty(arr[i])) {
                    a.push(arr[i]);
                    u[arr[i]] = 1;
                }
            }
            return a;
        }

        $scope.slideRange = function (direction) {
            if (direction === "forward") {
                let size = $scope.endRange - $scope.startRange;
                $scope.startRange = $scope.startRange + (size / 2);
                $scope.endRange = $scope.endRange + (size / 2);
                chartTest($scope.startRange, $scope.endRange);

                let link = document.getElementById('aDownloadLink');
                let canvas = document.getElementById('line')

                link.download = "RIPChart.png";
                link.href = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");


            }
            else if (direction === "backward") {
                let size = $scope.endRange - $scope.startRange;
                $scope.startRange = $scope.startRange - (size / 2);
                $scope.endRange = $scope.endRange - (size / 2);
                chartTest($scope.startRange, $scope.endRange);

                let link = document.getElementById('aDownloadLink');
                let canvas = document.getElementById('line')

                link.download = "RIPChart.png";
                link.href = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");


            }
        }

        $scope.updateRange = function (start, end) {
            chartTest(start, end);
        }


        function RIPPieChart() {
            $scope.RIPPie.data = [];
            $scope.RIPPie.data.push($scope.RIPPercentage);
            $scope.RIPPie.data.push(100 - $scope.RIPPercentage)
            $scope.RIPPie.labels = ['Total RIP %', 'Non Rip']
            $scope.RIPPie.colors = ['#46bf50', '#e0e0e0']
            $scope.RIPPie.series = ['Series']
        }

        function GCContentChart() {
            $scope.GCContentChart.data = [];
            $scope.GCContentChart.data.push($scope.GCContent);
            $scope.GCContentChart.data.push(100 - $scope.GCContent)
            $scope.GCContentChart.labels = ['Total GC Content %', 'Non GC']
            $scope.GCContentChart.colors = ['#ffc107', '#e0e0e0']
            $scope.GCContentChart.series = ['Series']
        }


        function chartTest(start, end) {

            let rip = $scope.ripData.filter(f => { return f.SequenceName === $scope.selectedSequence && f.Start >= start && f.End <= end });

            $scope.ripIndex.Start = rip.map(m => { return m.Start });
            $scope.ripIndex.Product = rip.map(m => { return m.Product });
            $scope.ripIndex.Substrate = rip.map(m => { return m.Substrate });
            $scope.ripIndex.Composite = rip.map(m => { return m.Composite });

            $scope.ripIndex.data = [];

            $scope.ripIndex.data.push($scope.ripIndex.Product);
            $scope.ripIndex.data.push($scope.ripIndex.Substrate);
            $scope.ripIndex.data.push($scope.ripIndex.Composite);

            $scope.ripIndex.series = ['Product', 'Substrate', 'Composite'];
            $scope.ripIndex.colors = ['#46bf50', '#fddf5c', '#e26253']


            $scope.ripIndex.options = {
                scales: {
                    yAxes: [
                        {
                            id: 'y-axis-1',
                            type: 'linear',
                            display: true,
                            position: 'left'
                        }
                    ]
                },
                legend: { display: true },
                title: {
                    display: true,
                    text: 'RIP Index'
                },
                tooltips: {
                    mode: 'index',
                    intersect: false,
                },
                hover: {
                    mode: 'nearest',
                    intersect: true
                },
            };


            $timeout(function () {
                let link = document.getElementById('aDownloadLink');
                var canvas = document.getElementById('line')

                link.download = "RIPChart.png";
                link.href = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");

                //Sets the ui-grid menu popover
                document.getElementsByClassName('ui-grid-menu-button')[0].setAttribute("title", "Download results, and hide columns");

            }, 3000)


        }

        $scope.saveLineChart = function () {
            var canvas = document.getElementById('line')
            document.location.href = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");

        }


        

        function calculateTotalRIP(data) {

            let totalRowCount = data.length;
            let RIPDataCount = data.filter(d => { return d.Product >= 1.1 && d.Substrate <= 0.9 }).length;

            let RIPPercentage = (RIPDataCount / totalRowCount) * 100;

            $scope.RIPPercentage = RIPPercentage.toFixed(2);
            RIPPieChart();

        }
    }
})();
