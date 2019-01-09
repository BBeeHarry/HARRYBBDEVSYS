$(function () {
    //$.validator.methods.date = function (value, element) {
    //    if ($.browser.webkit) {
    //        var d = new Date(); console.log(value);
    //        return this.optional(element) || !/Invalid|NaN/.test(new Date(d.toLocaleDateString(value)));
    //    }
    //    else {
    //        return this.optional(element) || !/Invalid|NaN/.test(new Date(value));
    //    }
    //};
    //Fix date validation in chrome for date format (dd/MM/yyyy)
    $.validator.methods.date = function (value, element) {
        var dateParts = value.split('/');
        var dateStr = dateParts[2] + '-' + dateParts[1] + '-' + dateParts[0];
        return this.optional(element) || !/Invalid|NaN/.test(new Date(dateStr));
    };
});