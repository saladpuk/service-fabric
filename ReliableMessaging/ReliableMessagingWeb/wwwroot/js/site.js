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

    setInterval(() => {
        $scope.refresh();
    }, 700);

    $scope.ServerKey = 1;
    $scope.update = function () {
        console.log('update');
        $http.put('api/Message/' + $scope.ServerKey).then(function (response) {
            if (response.status === 200) {
                $scope.ServerKey++;
                if ($scope.ServerKey > $scope.serverStatus.data.length) {
                    $scope.ServerKey = 1;
                }
            }
        });
    };

}]);