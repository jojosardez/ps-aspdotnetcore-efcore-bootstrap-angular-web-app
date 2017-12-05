// app-trips.js
(function () {

    "use strict";

    // create module
    angular.module("app-trips", ["simpleControls", "ngRoute"])
        .config(function ($routeProvider, $locationProvider) {
            $locationProvider.hashPrefix('');

            $routeProvider.when("/",
                {
                    controller: "tripsController",
                    controllerAs: "vm",
                    templateUrl: "/views/tripsView.html"
                });

            $routeProvider.when("/editor/:tripName",
                {
                    controller: "tripEditorController",
                    controllerAs: "vm",
                    templateUrl: "/views/tripEditorView.html"
                });

            $routeProvider.otherwise({ redirectTo: "/" });
        });

})();