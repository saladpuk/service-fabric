var app = angular.module('MessageApp', ['ui.bootstrap']);
app.run(function () { });

app.controller('MessageAppController', ['$rootScope', '$scope', '$http', '$timeout', function ($rootScope, $scope, $http, $timeout) {

    $scope.autoRefreshState = 'Auto Refresh (On)';
    $scope.autoRefreshStateEnable = true;

    $scope.refresh = function () {
        $http.get('api/Message?c=' + new Date().getTime())
            .then(function (response) {
                console.log(response);
                $scope.serverStatus = response.data;
            }, function (data, status) {
                $scope.serverStatus = undefined;
            });
    };

    $scope.start = function () {
        $http.post('api/Message').then(function (status) {
        });
    };

    $scope.toggleAutoRefresh = function () {
        $scope.autoRefreshStateEnable = !$scope.autoRefreshStateEnable;
        if ($scope.autoRefreshStateEnable) {
            $scope.autoRefreshState = 'Auto Refresh (On)';
        }
        else {
            $scope.autoRefreshState = 'Auto Refresh (Off)';
        }
    };

    setInterval(() => {
        if ($scope.autoRefreshStateEnable) {
            $scope.refresh();
        }
    }, 700);

}]);