var app = angular.module("WebFileSystem", []);

app.controller("HomeController", ["$scope", "$http", function ($scope, $http) {

    $http.get('/api/values').success(function (data) {
        console.log(data);
        $scope.result = data;
    }).error(function () {
        alert('oops something went wrong');
    });

    $scope.currentPath = "/";

    $scope.backToNode = function () {
        
    }


    $scope.goToNode = function (path) {
        $scope.currentPath = path;
        $http.get("/api/values?path=" + encodeURIComponent(path))
            .success(function (data) {
                $scope.result = data;
                console.log(data);
            }).error(function () {
                alert('oops something went wrong');
            });
    }
}]);


