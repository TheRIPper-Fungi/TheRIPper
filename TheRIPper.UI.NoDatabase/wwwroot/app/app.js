var app = angular.module('app', [
    'ngRoute',
    'ngCookies',
    'home',
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

    $routeProvider.when('/contact', {
        templateUrl: 'App/Contact',
        controller: 'contactCtrl'
    });

    $routeProvider.when('/dashboard', {
        templateUrl: 'Dashboard/Index',
        controller: 'dashboardCtrl'
    });

    $routeProvider.when('/rip', {
        templateUrl: 'RIP/RIPSequenceView',
        controller: 'ripCtrl'
    });

    $routeProvider.when('/rip/lrar/sequence/:FileName/:SequenceName', {
        templateUrl: 'RIP/LRARView',
        controller: 'lrarCtrl'
    });

    $routeProvider.when('/rip/lrar/file/:FileName', {
        templateUrl: 'RIP/LRARView',
        controller: 'lrarCtrl'
    });

    $routeProvider.when('/rip/sequence/:FileName/:SequenceName', {
        templateUrl: 'RIP/RIPSequenceView',
        controller: 'ripCtrl'
    });

    $routeProvider.when('/rip/file/:FileName', {
        templateUrl: 'RIP/RIPSequenceView',
        controller: 'ripCtrl'
    });

    $routeProvider.when('/rip/profile/file/:FileName', {
        templateUrl: 'RIP/RIPProfileView',
        controller: 'ripProfileCtrl'
    });

    $routeProvider.when('/files', {
        templateUrl: 'Files/Files',
        controller: 'filesCtrl'
    });

    $routeProvider.when('/sequences/:FileName', {
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

    //$rootScope.logout = function () {

    //    $http.post('/api/account/Logout')
    //        .success(function (data, status, headers, config) {
    //            console.log(data, status, headers, config)
    //            $http.defaults.headers.common.Authorization = null;
    //            //$http.defaults.headers.common.RefreshToken = null;
    //            $cookieStore.remove('_Token');
    //            //$cookieStore.remove('_RefreshToken');
    //            $rootScope.username = '';
    //            $rootScope.loggedIn = false;
    //            window.location = '#/signin';
    //        })
    //        .error(function () {
    //            console.log("Error")
    //            //Ehhhh hopefully logged out
    //            $http.defaults.headers.common.Authorization = null;
    //            //$http.defaults.headers.common.RefreshToken = null;
    //            $cookieStore.remove('_Token');
    //            //$cookieStore.remove('_RefreshToken');
    //            $rootScope.username = '';
    //            $rootScope.loggedIn = false;
    //            window.location = '#/signin';
    //        });

    //}

    //$rootScope.$on('$locationChangeSuccess', function (event) {

    //    if ($http.defaults.headers.common.Authorization != null && $http.defaults.headers.common.Authorization !== 'Bearer undefined') {

    //        $rootScope.loggedIn = true;

    //    }
    //    else {
    //        $rootScope.loggedIn = false;
    //    }
    //});
}]);

