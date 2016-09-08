var app = angular.module("WebFileSystem", []);

app.controller("HomeController", ["$scope", "$http", function ($scope, $http) {
    $scope.currentPath = "";
    $scope.previousPath = "";

    $scope.getBack = function () {
        $scope.previousPath = $scope.currentPath.split("\\");

        if ($scope.previousPath.length == 2 && !$scope.previousPath[1]) {
            $scope.previousPath.pop();
        }

        $scope.previousPath.pop();
        var drive = $scope.previousPath.shift();

        if (!drive)
            drive = "";
        else
            drive += "\\";

        $scope.previousPath = $scope.previousPath.join("\\");
        $scope.getPath(drive + $scope.previousPath);
    };

    $scope.getPath = function (path) {
        $scope.currentPath = path;

        $http.get("/api/values?path=" + encodeURIComponent(path))
            .success(function (data) {
                $scope.result = data;
                console.log(data);
            }).error(function () {
                alert('oops something went wrong');
            });
    };



    $scope.getPath($scope.currentPath);
}]);