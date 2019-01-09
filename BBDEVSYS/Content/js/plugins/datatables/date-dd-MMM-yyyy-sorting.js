/*
 * Adds a new sorting option to dataTables called `date-dd-mmm-yyyy`. Also
 * includes a type detection plug-in. Matches and sorts date strings in
 * the format: `dd/mmm/yyyy`. For example:
 * 
 * * 02-FEB-1978
 * * 17-MAY-2013
 * * 31-JAN-2014
 *
 * Please note that this plug-in is **deprecated*. The
 * [datetime](//datatables.net/blog/2014-12-18) plug-in provides enhanced
 * functionality and flexibility.
 *
 *  @name Date (dd-mmm-yyyy)
 *  @summary Sort dates in the format `dd-mmm-yyyy`
 *  @author [Jeromy French](http://www.appliedinter.net/jeromy_works/)
 *  @deprecated
 *
 *  @example
 *    $('#example').dataTable( {
 *       columnDefs: [
 *         { type: 'date-dd-mmm-yyyy', targets: 0 }
 *       ]
 *    } );
 */

(function () {

    var customDateDDMMMYYYYToOrd = function (date) {
        "use strict"; //let's avoid tom-foolery in this function
        // Convert to a number YYYYMMDD which we can use to order
        var colDataParts = date.split("@");
        if (typeof (colDataParts) == "undefined" || colDataParts == null)
            return 0;
        if (colDataParts.length != 3)
            return 0;

        var dateParts = colDataParts[1].split("/");
        if (typeof (dateParts) == "undefined" || dateParts == null)
            return 0;
        if (dateParts.length != 6)
            return 0;
        //var dateParts = date.split(/-/);
        return (dateParts[2] * 1000000000000) + ($.inArray(dateParts[1].toUpperCase(), ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"]) * 10000000000) + (dateParts[0] * 100000000) +
               (parseInt(dateParts[3]) * 10000) + (parseInt(dateParts[4]) * 100) + (parseInt(dateParts[5]) * 1);
        //return (dateParts[2] * 10000) + ($.inArray(dateParts[1].toUpperCase(), ["01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12"]) * 100) + (dateParts[0] * 1);
        //return (dateParts[2] * 10000) + ($.inArray(dateParts[1].toUpperCase(), ["JAN", "FEB", "MAR", "APR", "MAY", "JUN", "JUL", "AUG", "SEP", "OCT", "NOV", "DEC"]) * 100) + (dateParts[0] * 1);

    };

    // This will help DataTables magic detect the "dd-MMM-yyyy" format; Unshift
    // so that it's the first data type (so it takes priority over existing)
    jQuery.fn.dataTableExt.aTypes.unshift(
        function (sData) {
            "use strict"; //let's avoid tom-foolery in this function
            //if (/^([0-2]?\d|3[0-1])-(jan|feb|mar|apr|may|jun|jul|aug|sep|oct|nov|dec)-\d{4}/i.test(sData)) {
            if (/^([0-2]?\d|3[0-1])-(01|02|03|04|05|06|07|08|09|10|11|12)-\d{4}/i.test(sData)) {
                return 'date-dd-mmm-yyyy';
            }
            return null;
        }
    );

    // define the sorts
    jQuery.fn.dataTableExt.oSort['date-dd-mmm-yyyy-asc'] = function (a, b) {
        "use strict"; //let's avoid tom-foolery in this function
        var ordA = customDateDDMMMYYYYToOrd(a),
            ordB = customDateDDMMMYYYYToOrd(b);
        return (ordA < ordB) ? -1 : ((ordA > ordB) ? 1 : 0);
    };

    jQuery.fn.dataTableExt.oSort['date-dd-mmm-yyyy-desc'] = function (a, b) {
        "use strict"; //let's avoid tom-foolery in this function
        var ordA = customDateDDMMMYYYYToOrd(a),
            ordB = customDateDDMMMYYYYToOrd(b);
        return (ordA < ordB) ? 1 : ((ordA > ordB) ? -1 : 0);
    };

})();
