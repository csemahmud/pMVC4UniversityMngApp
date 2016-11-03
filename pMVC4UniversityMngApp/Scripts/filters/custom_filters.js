pMVC4UniversityMngApp.filter("unique", function (SELECT_STR) {
    return function (data, property) {
        if (angular.isArray(data)) {
            var results = [];
            var keys = {};
            results.push(SELECT_STR);
            for (var i = 0; i < data.length; i++) {
                var val = data[i][property];
                if (angular.isUndefined(keys[val])) {
                    keys[val] = true;
                    results.push(val);
                }
            }
            return results;
        }
        return data;
    };
});

pMVC4UniversityMngApp.filter("range", function ($filter) {
    return function (data, pageNo, pageSize) {
        if (angular.isArray(data) && angular.isNumber(pageNo) && angular.isNumber(pageSize)) {
            var startIndex = (pageNo - 1) * pageSize;
            if (data.length < startIndex) {
                return [];
            }
            return $filter("limitTo")(data.splice(startIndex), pageSize);
        }
        return data;
    };
});

pMVC4UniversityMngApp.filter("pageCount", function () {
    return function (data, size) {
        if (angular.isArray(data)) {
            var results = [];
            for (var i = 0; i < Math.ceil(data.length/size); i++) {
                results.push(i);
            }
            return results;
        }
        return data;
    };
});