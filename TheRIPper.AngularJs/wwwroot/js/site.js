var app = angular.module('app', [
    'ngRoute',
    'ngCookies',
    'home',
    'signIn',
    'register',
    'ui.grid',
    'ui.grid.grouping',
    'ui.grid.selection',
    'ui.grid.resizeColumns',
    'ui.grid.expandable',
    'ui.grid.cellNav',
    'ui.grid.edit',
    'ui.grid.infiniteScroll',
    'ui.grid.exporter',
    'ngFileUpload',
    'chart.js'
]);




app.config(['$provide', '$routeProvider', '$httpProvider', '$logProvider', function ($provide, $routeProvider, $httpProvider, $logProvider) {

    $logProvider.debugEnabled(true);

    //================================================
    // Ignore Template Request errors if a page that was requested was not found or unauthorized.  The GET operation could still show up in the browser debugger, but it shouldn't show a $compile:tpload error.
    //================================================
    $provide.decorator('$templateRequest', ['$delegate', function ($delegate) {
        var mySilentProvider = function (tpl, ignoreRequestError) {
            return $delegate(tpl, true);
        }
        return mySilentProvider;
    }]);

    //================================================
    // Add an interceptor for AJAX errors
    //================================================
    $httpProvider.interceptors.push(['$q', '$location', function ($q, $location) {
        return {
            'responseError': function (response) {
                if (response.status === 401)
                    $location.url('/signin');
                return $q.reject(response);
            }
        };
    }]);


    //================================================
    // Routes
    //================================================
    $routeProvider.when('/home', {
        templateUrl: 'App/Home',
        controller: 'homeCtrl'
    });
    $routeProvider.when('/background', {
        templateUrl: 'App/Background',
        controller: 'backgroundCtrl'
    });

    $routeProvider.when('/dashboard', {
        templateUrl: 'Dashboard/Index',
        controller: 'dashboardCtrl'
    });


    $routeProvider.when('/register', {
        templateUrl: 'api/account/Register',
        controller: 'registerCtrl'
    });

    $routeProvider.when('/signin/:message?', {
        templateUrl: 'api/account/SignIn',
        controller: 'signInCtrl'
    });

    $routeProvider.when('/forgotpassword', {
        templateUrl: 'api/account/Forgot',
        controller: 'forgotCtrl'
    });

    $routeProvider.when('/reset/:code', {
        templateUrl: 'api/account/Reset',
        controller: 'resetCtrl'
    });

    $routeProvider.when('/rip', {
        templateUrl: 'RIP/RIPSequenceView',
        controller: 'ripCtrl'
    });

    $routeProvider.when('/rip/lrar/sequence/:SequenceId', {
        templateUrl: 'RIP/LRARView',
        controller: 'lrarCtrl'
    });

    $routeProvider.when('/rip/lrar/file/:FileId', {
        templateUrl: 'RIP/LRARView',
        controller: 'lrarCtrl'
    });

    $routeProvider.when('/rip/sequence/:SequenceId', {
        templateUrl: 'RIP/RIPSequenceView',
        controller: 'ripCtrl'
    });

    $routeProvider.when('/rip/file/:FileId', {
        templateUrl: 'RIP/RIPSequenceView',
        controller: 'ripCtrl'
    });

    $routeProvider.when('/rip/profile/file/:FileId', {
        templateUrl: 'RIP/RIPProfileView',
        controller: 'ripProfileCtrl'
    });

    $routeProvider.when('/files', {
        templateUrl: 'Files/Files',
        controller: 'filesCtrl'
    });

    $routeProvider.when('/sequences/:FileName/:FileId', {
        templateUrl: 'Sequence/Sequences',
        controller: 'sequenceCtrl'
    });

    

    
    

    $routeProvider.otherwise({
        redirectTo: '/home'
    });
}]);

app.run(['$http', '$cookies', '$cookieStore', function ($http, $cookies, $cookieStore) {
    //If a token exists in the cookie, load it after the app is loaded, so that the application can maintain the authenticated state.
    $http.defaults.headers.common.Authorization = 'Bearer ' + $cookieStore.get('_Token');
    //$http.defaults.headers.common.RefreshToken = $cookieStore.get('_RefreshToken');
}]);


//GLOBAL FUNCTIONS - pretty much a root/global controller.
//Get username on each page
//Get updated token on page change.
//Logout available on each page.
app.run(['$rootScope', '$http', '$cookies', '$cookieStore', function ($rootScope, $http, $cookies, $cookieStore) {

    $rootScope.logout = function () {

        $http.post('/api/account/Logout')
            .success(function (data, status, headers, config) {
                console.log(data,status,headers,config)
                $http.defaults.headers.common.Authorization = null;
                //$http.defaults.headers.common.RefreshToken = null;
                $cookieStore.remove('_Token');
                //$cookieStore.remove('_RefreshToken');
                $rootScope.username = '';
                $rootScope.loggedIn = false;
                window.location = '#/signin';
            })
            .error(function () {
                console.log("Error")
                //Ehhhh hopefully logged out
                $http.defaults.headers.common.Authorization = null;
                //$http.defaults.headers.common.RefreshToken = null;
                $cookieStore.remove('_Token');
                //$cookieStore.remove('_RefreshToken');
                $rootScope.username = '';
                $rootScope.loggedIn = false;
                window.location = '#/signin';
            });

    }

    $rootScope.$on('$locationChangeSuccess', function (event) {

        if ($http.defaults.headers.common.Authorization != null && $http.defaults.headers.common.Authorization !== 'Bearer undefined') {

            $rootScope.loggedIn = true;
            
        }
        else {
            $rootScope.loggedIn = false;
        }
    });
}]);


(function () {
    'use strict';

    angular
        .module('app')
        .controller('backgroundCtrl', backgroundCtrl);

    backgroundCtrl.$inject = ['$scope'];

    function backgroundCtrl($scope) {
        $scope.title = 'backgroundCtrl';

        activate();

        function activate() { }
    }
})();

(function () {
    'use strict';

    angular
        .module('app')
        .controller('filesCtrl', filesCtrl);

    filesCtrl.$inject = ['$scope', '$http','fileFactory'];

    function filesCtrl($scope, $http, fileFactory) {
        $scope.title = 'filesCtrl';
        $scope.CurrentFileName = null;
        $scope.file = {};
        $scope.loadingFiles = true;
        activate();

        function activate() {
            fileFactory.list().then((data) => { 
                $scope.files = data;
                $scope.loadingFiles = false;
            });
        }

        $scope.submit = function () {
            if ($scope.form.file.$valid && $scope.file) {
                $scope.upload($scope.file);
            }
        };

        // upload on file select or drop
        //https://stackoverflow.com/questions/38144194/iformfile-is-always-empty-in-asp-net-core-webapi
        $scope.upload = function () {
            $scope.uploadingFile = true;
            var fileUpload = $("#fileup").get(0);
            var files = fileUpload.files;
            var data = new FormData();
            for (var i = 0; i < files.length; i++) {
                data.append(files[i].name, files[i]);
            }


            $http.post("/api/files/upload", data, {
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            }).success(function (data, status, headers, config) {
                    $scope.CurrentFileName = JSON.parse(data).FileName;
                $scope.file.FilePath = JSON.parse(data).FileName;
                $scope.uploadingFile = false;
                }).error(function (data, status, headers, config) {
                    alert("Something went wrong!")
            });


        };

        $scope.save = () => {
            $scope.savingFile = true;
            fileFactory.save($scope.file.FileName, $scope.file.FileDescription, $scope.file.FilePath)
                .then(data => {
                    activate();
                    $scope.savingFile = false;
                })

        };

        $scope.removeFile = function(FileId){
            fileFactory.remove(FileId)
                .then(function (data) {
                    if (data.IsRemoved === true) {
                        alert("The file has been removed");
                    }
                    activate();
                })
        }

    }
})();

(function () {
    'use strict';

    angular
        .module('app')
        .controller('forgotCtrl', forgotCtrl);

    forgotCtrl.$inject = ['$scope','$http'];

    function forgotCtrl($scope,$http) {
        $scope.title = 'forgotCtrl';

        activate();

        function activate() { }


        $scope.submitForgotPassword = function () {
            $http({
                url: '/api/account/forgot',
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                data: {
                    Email:$scope.email
                }
            })
        }
    }
})();

angular.module('home', [])
    .controller('homeCtrl', ['$scope', '$http', function ($scope, $http) {
        

    }]);
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
                { field: 'Start', width: 150, type: 'number' },
                { field: 'End', width: 150, type: 'number' },
                { field: 'Size', aggregationType: uiGridConstants.aggregationTypes.sum, width: 150, type: 'number' },
                { field: 'Count', displayName: "Count Of Windows", aggregationType: uiGridConstants.aggregationTypes.sum, width: 75, type: 'number' },
                { field: 'Product', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number' },
                { field: 'Substrate', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number' },
                { field: 'Composite', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number' },
                { field: 'GCContent', footerCellFilter: 'number:2', displayName: "GC Content", aggregationType: uiGridConstants.aggregationTypes.avg, width: 75, type: 'number' },
            ],
            onRegisterApi: function (gridApi) {
                //$scope.clientTargetsGridApi = gridApi;
            },
        };

        $scope.update = function () { activate(); }

        function activate() {
            $scope.lrarLoading = true;

            $scope.SequenceId = $routeParams.SequenceId;
            $scope.FileId = $routeParams.FileId;

            if ($scope.SequenceId !== undefined) {
                $scope.type = 's'; //S for Sequence

                lrarFactory.getSequence($scope.SequenceId, $scope.window, $scope.slide, $scope.compositeRequirement, $scope.compositeCountRequirement)
                    .then(function (data) {
                        $scope.lrarGridOptions.data = data;

                        ChartSize();

                        $scope.lrarLoading = false;
                    })
            }
            else if ($scope.FileId !== undefined) {
                $scope.type = 'f';

                lrarFactory.getFile($scope.FileId, $scope.window, $scope.slide, $scope.compositeRequirement, $scope.compositeCountRequirement)
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
angular.module('register', [])
    .controller('registerCtrl',['$scope','$http', function ($scope, $http) {
        $scope.register = function()
        {
            var params = {
                Email: $scope.username,
                Password: $scope.password1,
                ConfirmPassword: $scope.password2
            };
            $http.post('/api/Account/Register', params)
            .success(function (data, status, headers, config) {
                $scope.successMessage = "Registration Complete.  Please Sign In.";
                $scope.showErrorMessage = false;
                $scope.showSuccessMessage = true;
            })
                .error(function (data, status, headers, config) {

                    $scope.errorMessages = [];
                    if (angular.isArray(data))
                        $scope.errorMessages = data;
                    else {
                        if (data.Email !== undefined) {
                            $scope.errorMessages.push("Email : " + data.Email);
                        }
                        if (data.Password !== undefined) {
                            $scope.errorMessages.push("Password : " + data.Password);
                        }
                        if (typeof (data[""] === "array")) {
                            data[""].forEach(f => {
                                $scope.errorMessages.push("Password : " + f);
                            })
                        }
                    }
                    //$scope.errorMessages = new Array(data.replace(/["']{1}/gi, ""));

                $scope.showSuccessMessage = false;
                $scope.showErrorMessage = true;
            });
        }

        $scope.showAlert = false;
        $scope.showSuccess = false;
    }]);
(function () {
    'use strict';

    angular
        .module('app')
        .controller('resetCtrl', resetCtrl);

    resetCtrl.$inject = ['$scope','$routeParams','$http'];

    function resetCtrl($scope,$routeParams,$http) {
        $scope.title = 'resetCtrl';

        activate();

        function activate() {
            $scope.code = $routeParams.code;
        }

        $scope.resetPassword = function () {
            $http({
                url: '/api/account/reset',
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                data: {
                    Email: $scope.email,
                    Password: $scope.password,
                    ConfirmPassword: $scope.passwordconfirm,
                    Code: $scope.code
                }
            })
        }
    }
})();

(function () {
    'use strict';

    angular
        .module('app')
        .controller('ripCtrl', ripCtrl);

    ripCtrl.$inject = ['$scope', 'ripFactory', 'gcContentFactory', '$routeParams','uiGridConstants','$timeout'];

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
                { field: 'SequenceName', width: 270},
                { field: 'Start', width: 80, type: 'number'},
                { field: 'End', width: 80, type: 'number' },
                { field: 'Product', type: 'number', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 120 },
                { field: 'Substrate', type: 'number', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg,width: 120 },
                { field: 'Composite', type: 'number', footerCellFilter: 'number:2', aggregationType: uiGridConstants.aggregationTypes.avg, width: 120 },
                { field: 'GCContent', type: 'number', footerCellFilter: 'number:2', displayName:"GC Content", aggregationType: uiGridConstants.aggregationTypes.avg,width:120}
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
            $scope.IsSequence = $routeParams.SequenceId !== undefined;
            $scope.IsFile = $routeParams.FileId !== undefined;
            if ($scope.IsSequence) {
                $scope.SequenceId = $routeParams.SequenceId;
                //Logic
                ripFactory.RIPSequence($scope.SequenceId, $scope.window, $scope.slide).then(data => {
                    $scope.ripGridOptions.data = data;
                    $scope.ripData = data;
                    $scope.seqNames = data.map(m => { return m.SequenceName });
                    $scope.seqNamesUnique = unique($scope.seqNames);

                    chartTest($scope.startRange, $scope.endRange);

                    $scope.selectedSequence = $scope.seqNamesUnique[0];
                    $scope.updateRange($scope.startRange, $scope.endRange);

                    calculateTotalRIP(data);
                    $scope.loadingRIP = false;
                    
                })

                gcContentFactory.GCContentSingleSequenceTotal($scope.SequenceId)
                    .then(data => {
                        $scope.GCContent = Number.parseFloat(data).toFixed(2);
                        GCContentChart();
                    })
                //
                
            }
            else if ($scope.IsFile) {
                $scope.FileId = $routeParams.FileId;
                //Logic
                ripFactory.RIPFile($scope.FileId, $scope.window, $scope.slide)
                    .then(function (data) {
                        $scope.ripGridOptions.data = data;
                        $scope.ripData = data;
                        $scope.seqNames = data.map(m => { return m.SequenceName });
                        $scope.seqNamesUnique = unique($scope.seqNames);

                        chartTest($scope.startRange, $scope.endRange);

                        ripFactory.RIPIndexes($scope.FileId).then(data => {
                            let filtered = data.filter(f => { return f.RIPIndex < 80 });

                            let clabels = filtered.map(d => { return d.SequenceName.slice(0, 20) });
                            let cdata = filtered.map(d => { return d.RIPIndex });
                            RadarChart(clabels, cdata);

                            calculateTotalRIP($scope.ripData);


                            gcContentFactory.GCContentFileTotal($scope.FileId)
                                .then(data => {
                                    $scope.GCContent = Number.parseFloat(data).toFixed(2);
                                    GCContentChart();
                                    $scope.loadingRIP = false;
                                })
                            
                        })

                        
                    })
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
                var canvas = document.getElementById('line')

                link.download = "RIPChart.png";
                link.href = canvas.toDataURL("image/png").replace("image/png", "image/octet-stream");


            }
            else if (direction === "backward") {
                let size = $scope.endRange - $scope.startRange;
                $scope.startRange = $scope.startRange - (size / 2);
                $scope.endRange = $scope.endRange - (size / 2);
                chartTest($scope.startRange, $scope.endRange);

                let link = document.getElementById('aDownloadLink');
                var canvas = document.getElementById('line')

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
            $scope.RIPPie.data.push(100-$scope.RIPPercentage)
            $scope.RIPPie.labels = ['Total RIP %','Non Rip']
            $scope.RIPPie.colors = ['#46bf50','#e0e0e0']
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
            $scope.ripIndex.colors = ['#46bf50', '#fddf5c','#e26253']


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


        function RadarChart(chartLabels, chartData) {
            var color = Chart.helpers.color;

            var radarconfig = {
                type: 'radar',
                data: {
                    labels: chartLabels,
                    datasets: [{
                        label: 'RIP Per Sequence',
                        backgroundColor: color("#ff0000").alpha(0.2).rgbString(),
                        borderColor: "#FF0000",
                        pointBackgroundColor: "#FF0000",
                        data: chartData
                    }]
                },
                options: {
                    legend: {
                        position: 'top',
                    },
                    title: {
                        display: true,
                        text: '% RIP Affected'
                    },
                    scale: {
                        ticks: {
                            beginAtZero: true
                        }
                    }
                }
            };

            window.myRadar = new Chart(document.getElementById('canvasradar'), radarconfig);

        }

        function calculateTotalRIP(data) {

            let totalRowCount = data.length;
            let RIPDataCount = data.filter(d => { return d.Product >= 1.1 && d.Substrate <= 0.9}).length;

            let RIPPercentage = (RIPDataCount / totalRowCount) * 100;

            $scope.RIPPercentage = RIPPercentage.toFixed(2);
            RIPPieChart();

        }
    }
})();

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

(function () {
    'use strict';

    angular
        .module('app')
        .controller('sequenceCtrl', sequenceCtrl);

    sequenceCtrl.$inject = ['$scope', '$routeParams','sequenceFactory'];

    function sequenceCtrl($scope, $routeParams, sequenceFactory) {
        $scope.title = 'sequenceCtrl';
        $scope.FileId = $routeParams.FileId;
        $scope.FileName = $routeParams.FileName
        $scope.loadingSequences = true;
        activate();

        function activate() {
            sequenceFactory.list($scope.FileId).then(data => {
                $scope.sequences = data;
                $scope.loadingSequences = false;
            })
        }
    }
})();

angular.module('signIn', ['ngCookies'])
    .controller('signInCtrl', ['$scope' ,'$rootScope', '$http', '$cookies', '$cookieStore', '$location', '$routeParams', function ($scope, $rootScope, $http, $cookies, $cookieStore, $location, $routeParams) {
        $scope.message = $routeParams.message;
        $scope.signIn = function () {
            $scope.showMessage = false;
            //var params = "grant_type=password&username=" + $scope.username + "&password=" + $scope.password;
            var params = {
                Email: $scope.username,
                Password: $scope.password
            };
            $http({
                url: '/api/account/login',
                method: "POST",
                headers: { 'Content-Type': 'application/json' },
                data: params
            })
                .success(function (data, status, headers, config) {
                    $http.defaults.headers.common.Authorization = "Bearer " + data;//.access_token;
                //$http.defaults.headers.common.RefreshToken = data.refresh_token;
                
                    $cookieStore.put('_Token', data);//.access_token);
                    window.location = '#/home';
            })
            .error(function (data, status, headers, config) {
                $scope.message = data.error_description.replace(/["']{1}/gi, "");
                $scope.showMessage = true;
            });
        }
    }]);
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
(function () {
    'use strict';

    angular
        .module('app')
        .factory('fileFactory', fileFactory);

    fileFactory.$inject = ['$http'];

    function fileFactory($http) {
        var service = {
            save: save,
            list: list,
            remove: remove
        };

        return service;

        function save(FileName, Description, FilePath) {
            let file = {
                FileName: FileName,
                Description: Description,
                Location: FilePath
            };


            return $http({
                method: 'POST',
                url: 'api/files/save',
                data: file,
                headers: { 'Content-Type': 'application/json' }
            })
                .then(function (data) { return data; })
                .then(function (data) { return data; })

        }

        function list() {
            return $http({
                method: 'GET',
                url: 'api/files/list',
                headers: { 'Content-Type': 'application/json' }
            })
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data).files; })
        }

        function remove(FileId) {
            return $http.delete('api/files/remove/' + FileId)
                .then(function (data) { return data })
                .then(function (data) { return JSON.parse(data.data) })

        }
    }
})();
(function () {
    'use strict';

    angular
        .module('app')
        .factory('gcContentFactory', gcContentFactory);

    gcContentFactory.$inject = ['$http'];

    function gcContentFactory($http) {
        var service = {
            GCContentSingleSequenceTotal: GCContentSingleSequenceTotal,
            GCContentFileTotal: GCContentFileTotal
        };

        return service;

        function GCContentSingleSequenceTotal(sequenceId) {
            return $http.get('api/gccontent/sequence/' + sequenceId)
                .then(function (data) { return data; })
                .then(function (data) { return data.data; })
        }

        function GCContentFileTotal(FileId) {
            return $http.get('api/gccontent/file/' + FileId)
                .then(function (data) { return data; })
                .then(function (data) { return data.data; })
        }
    }
})();
(function () {
    'use strict';

    angular
        .module('app')
        .factory('ripFactory', ripFactory);

    ripFactory.$inject = ['$http'];

    function ripFactory($http) {
        var service = {
            test: test,
            RIPIndexes: RIPIndexes,
            RIPSequence: RIPSequence,
            RIPFile: RIPFile
        };

        return service;

        function test() {
            return $http.get('/api/rip')
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }

        function RIPIndexes(FileId) {
            return $http.get('api/rip/indexes/'+FileId)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }

        function RIPSequence(SequenceId, WindowSize, SlidingSize) {

            if (WindowSize !== undefined && SlidingSize !== undefined) {
                return $http.get('/api/rip/sequence/' + SequenceId + "/" + WindowSize + "/" + SlidingSize)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            else {
                return $http.get('/api/rip/sequence/' + SequenceId)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            
        }

        function RIPFile(FileId, WindowSize, SlidingSize) {
            if (WindowSize !== undefined && SlidingSize !== undefined) {
                return $http.get('/api/rip/file/' + FileId + "/" + WindowSize + "/" + SlidingSize)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            else {
                return $http.get('api/rip/file/' + FileId)
                    .then(function (data) { return data; })
                    .then(function (data) { return JSON.parse(data.data); })
            }
            
        }
    }
})();
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
(function () {
    'use strict';

    angular
        .module('app')
        .factory('sequenceFactory', sequenceFactory);

    sequenceFactory.$inject = ['$http'];

    function sequenceFactory($http) {
        var service = {
            list: list
        };

        return service;

        function list(FileId) {
            return $http.get('/api/sequence/' + FileId)
                .then(function (data) { return data; })
                .then(function (data) { return JSON.parse(data.data); })
        }
    }
})();