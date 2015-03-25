/// <reference path="types/angularjs/angular.d.ts"/>
var TodoApp;
(function (TodoApp) {
    var app = angular.module("todoApp", []);
    // Typescript translates enums to objects you can access via value or string
    var Columns;
    (function (Columns) {
        Columns[Columns["Id"] = 0] = "Id";
        Columns[Columns["Completed"] = 1] = "Completed";
        Columns[Columns["Description"] = 2] = "Description";
        Columns[Columns["DueDate"] = 3] = "DueDate";
    })(Columns || (Columns = {}));
    // to make this available to other modules I'd export it, but because I
    // am just registering with Angular I keep it local to the module
    var TodoService = (function () {
        function TodoService($q, $http) {
            this.$q = $q;
            this.$http = $http;
        }
        // promises are a way to handle asynchronous operations
        TodoService.prototype.getItems = function (filter) {
            var defer = this.$q.defer();
            this.$http.get("/api/todo", {
                params: filter
            }).then(function (result) {
                var list = result.data;
                defer.resolve(list);
            }, function (err) {
                defer.reject(err);
            });
            return defer.promise;
        };
        TodoService.prototype.updateItem = function (item) {
            var defer = this.$q.defer();
            this.$http.post("/api/todo", JSON.stringify(item)).then(function (result) {
                defer.resolve(result);
            }, function (err) {
                defer.reject(err);
            });
            return defer.promise;
        };
        TodoService.prototype.deleteItem = function (item) {
            var defer = this.$q.defer();
            this.$http.delete("/api/todo/" + item.Id).then(function () { return defer.resolve(); }, function (err) { return defer.reject(err); });
            return defer.promise;
        };
        // this let's Angular know what dependencies to inject even after your
        // code is minimized
        TodoService.$inject = ["$q", "$http"];
        return TodoService;
    })();
    // this registers the service with dependency injection
    app.service("todoSvc", TodoService);
    // controllers provide the "glue" for data binding. Any public properties
    // can be accessed via declarative markup
    var TodoController = (function () {
        function TodoController(todoSvc) {
            this.todoSvc = todoSvc;
            this.loading = false;
            this.items = [];
            this.filter = {
                filterText: "",
                columnName: Columns[3 /* DueDate */],
                sortAscending: false
            };
            this.refresh();
        }
        TodoController.prototype.refresh = function () {
            var _this = this;
            this.loading = true;
            this.todoSvc.getItems(this.filter).then(function (list) {
                _this.items = list;
                _this.loading = false;
            }, function (err) {
                alert(err);
                _this.loading = false;
            });
        };
        Object.defineProperty(TodoController.prototype, "filterText", {
            get: function () {
                return this.filter.filterText;
            },
            set: function (value) {
                this.filter.filterText = value;
                this.refresh();
            },
            enumerable: true,
            configurable: true
        });
        TodoController.prototype.overdue = function (todoItem) {
            var now = new Date(), due = new Date(todoItem.DueDate);
            return now > due;
        };
        TodoController.prototype.update = function (todoItem) {
            var _this = this;
            this.loading = true;
            this.todoSvc.updateItem(todoItem).then(function () {
                _this.loading = false;
            }, function (err) {
                alert(err);
                _this.loading = false;
            });
        };
        TodoController.prototype.changeColumn = function (columnName) {
            if (this.filter.columnName === columnName) {
                this.filter.sortAscending = !this.filter.sortAscending;
            }
            else {
                this.filter.columnName = columnName;
            }
            this.refresh();
        };
        TodoController.prototype.noCompleted = function () {
            var hasDone = false;
            angular.forEach(this.items, function (item) {
                if (item.IsDone) {
                    hasDone = true;
                }
            });
            return !hasDone;
        };
        TodoController.prototype.removeCompleted = function () {
            var _this = this;
            if (!confirm("Are you sure?")) {
                return;
            }
            var count = 0, checkCount = function () {
                count -= 1;
                if (count == 0) {
                    _this.refresh();
                }
            };
            this.loading = true;
            angular.forEach(this.items, function (item) {
                if (item.IsDone) {
                    count += 1;
                    _this.todoSvc.deleteItem(item).then(checkCount, checkCount);
                }
            });
        };
        TodoController.$inject = ["todoSvc"];
        return TodoController;
    })();
    app.controller("todoCtrl", TodoController);
    // directives provide reusable components. In this case the HTML for a header
    // and the logic to change the sort when the column header is clicked is all
    // encapsulated in a reusable component
    function columnToggle() {
        var directive = {
            restrict: "E",
            scope: {
                sortAscending: "=",
                changeColumn: "&",
                currentColumn: "=",
                columnDescription: "@",
                columnName: "@"
            },
            link: function (scope) {
                scope.toggle = function () {
                    scope.changeColumn({ colName: scope.columnName });
                };
            },
            template: '<span ng-click="toggle()">{{columnDescription || columnName}}<span ng-if="currentColumn===columnName">' + '<span ng-if="sortAscending"> ^</span>' + '<span ng-if="!sortAscending"> v</span>' + '</span></span>'
        };
        return directive;
    }
    app.directive("columnToggle", columnToggle);
})(TodoApp || (TodoApp = {}));
//# sourceMappingURL=app.js.map