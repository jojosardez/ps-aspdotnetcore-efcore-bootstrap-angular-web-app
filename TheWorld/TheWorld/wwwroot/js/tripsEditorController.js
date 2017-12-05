// tripEditorController.js
(function() {
    "use strict";

    angular.module("app-trips")
        .controller("tripEditorController", tripEditorController);

    function tripEditorController($routeParams, $http) {
        var vm = this;
        vm.tripName = $routeParams.tripName;
        vm.stops = [];
        vm.errorMessage = "";
        vm.isBusy = true;
        vm.newStop = {};

        $http.get("/api/trips/" + vm.tripName + "/stops")
            .then(function(response) {
                    // success
                    angular.copy(response.data, vm.stops);
                    _showMap(vm.stops);
                },
                function(err) {
                    // failure
                    vm.errorMessage = "Failed to load stops";
                })
            .finally(function() {
                vm.isBusy = false;
            });
    }

    function _showMap(stops) {
        if (stops && stops.length > 0) {
            var mapStops = _.map(stops, function(item) {
                return {
                    lat: item.latitude,
                    long: item.longitude,
                    info: item.name
                }
            });

            // show map
            travelMap.createMap({
                stops: stops,
                selector: "#map",
                currentStop: 1,
                initialZoom: 3
            });
        }
    }
})();