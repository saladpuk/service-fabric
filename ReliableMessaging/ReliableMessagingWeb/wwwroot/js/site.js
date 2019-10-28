var app = angular.module('MessageApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('MessageAppController', ['$rootScope', '$scope', '$http', '$timeout', function ($rootScope, $scope, $http, $timeout) {

    $scope.refresh = function () {
        $http.get('api/Message?c=' + new Date().getTime())
            .then(function (data, status) {
                $scope.serverStatus = data;
            }, function (data, status) {
                $scope.serverStatus = undefined;
            });
    };

    $scope.start = function () {
        $http.post('api/Message').then(function (status) {
            console.log(status);
        });
    };

}]);