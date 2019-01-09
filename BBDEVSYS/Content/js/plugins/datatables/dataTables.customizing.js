//$.fn.TableSortingAndFilter = function (columnTypes, columnDefs, options) {
$.fn.TableSortingAndFilter = function (columnTypes, options) {
    var o = $(this[0]) // It's your element
    var tableId = "#" + o.attr("id");

    //Hide Page Number and Search Box
    if (options.tools == "none") {
        $($(".dataTables_wrapper .row")[0]).css("display", "none");
    }

    //create element link
    var headerColList = $(tableId + " thead th");
    for (var i = 0; i < headerColList.length; i++) {

        //Copy Sorting Column
        var headerCol = $(headerColList[i]);
        headerCol.addClass("columnStyle");

        //Fix column width - Chairith
        //var events = headerCol.data("events");
        //var headerColClone = headerCol.clone(true, true);
        //headerColClone.removeClass("sorting");
        //headerColClone.removeClass("sorting_asc");
        //headerColClone.removeClass("sorting_desc");
        ////headerColClone.addClass("limit_col");                                                 
        ////headerColClone.css("display", "inline");

        
        //if (columnTypes[i].headerWidth) {
        //    /*
        //    var className  = "columnStyle" + i;
        //    var childIndex = i + 1;
        //    var styleHeader = "<style type='text/css'> ";
        //    styleHeader    +=   "." + className + " thead tr .columnStyle:nth-child(" + childIndex + ") th ," ;
        //    if (columnTypes[i].headerWidth)
        //        styleHeader += "max-width:" + columnTypes[i].headerWidth + "; ";
        //    styleHeader    +=  "}";
        //    styleHeader    += "</style>";

        //    $(styleHeader).appendTo("head");
        //    $(tableId).addClass(className);
        //    */
        //    headerColClone.css("max-width", columnTypes[i].headerWidth);
        //}
        //else 
        //headerColClone.css("max-width", "100px"); 
        ////headerCol.css("max-width", "100px");

        ////Modify section
        ////console.log(headerCol);//modify
        ////headerColClone.addClass("disable-sorting");//modify
        ////jQuery('<span/>', {
        ////    id: 'foo',
        ////    class: 'limit_col',
        ////    text: headerCol.html()
        ////}).appendTo(headerCol);

        var spantext = jQuery('<span/>', {
            id: 'dtHeaderText',
            class: 'true-datatable-span-header',
            text: headerCol.html()
        });

        headerCol.html(spantext);
        headerCol.addClass("true-datatable-header");


        //Add new Sorting Colur
        //headerColClone.html("<p class='limit_col'><a href='#' style='text-decoration: none;'>" + headerCol.html() + "</a></p>");
        headerCol.addClass("disable-sorting");
        //headerCol.html(headerColClone); 
        //End fix column width - Chairith

        //Check Column is render by content
        if (columnTypes[i].hasOwnProperty("defaultContent")) {
            continue;
        }
        //var columnDef = columnDefs.filter(function (obj) {
        //    return obj.targets == i;
        //});

        //if (columnDef.length > 0) {
        //    if (columnDef[0].defaultContent != window.undefined)
        //        continue;
        //    if (columnDef[0].hidden) {

        //    }
        //}

        //Display Time
        if (columnTypes[i].DisplayTime) {
            var nameDate =  "." + columnTypes[i].data + "Date";
            var nameDateTime = "." + columnTypes[i].data + "DateTime";
            $(nameDate).css("display", "none");
            $(nameDateTime).css("display", "");
            $(document.body).append("<style>" + nameDate + "{display:none !important;} " + nameDateTime + "{display:inline !important;}</style>")
        }

        //Display Option
        if (columnTypes[i].hidden) {
            var colNum = i + 1;
            $(tableId).addClass("hide_col_" + colNum);
        }

        //Add filter
        var additionalClass = "", additionalId = "inputFilter" + i; additionalClearId = "clearFilter" + i, operatorUI = "";
        if (columnTypes[i].type == "date-dd-mmm-yyyy") {
            additionalClass = "datePick datepicker-th";
        }

        if (columnTypes[i].type == "num"||
            columnTypes[i].type == "currency") {
            additionalClass = "numberInput";
        }

        if (additionalClass != "") {
            operatorUI = '<li style="padding-top: 8px;padding-left: 8px;padding-right: 8px;">' +
                                    '<select id="operaterSelect' + i + '" class="form-control sign-operator" data-id="' + additionalId + '">' +
                                        '<option class="ard-eq" value="="></option>' +
                                        '<option class="ard-gt" value=">"></option>' +
                                        '<option class="ard-lt" value="<"></option>' +
                                        '<option class="ard-ge" value=">="></option>' +
                                        '<option class="ard-le" value="<="></option>' +
                                    '</select>' +
                                 '</li>';

            operatorUI = '<li style="padding-top: 8px;padding-left: 8px;padding-right: 8px;">' +
                            '<div class="dropdown-operation input-group">' +
                                '<input type="TextBox" id="operaterSelect' + i + '" Class="form-control operaterSelect" value="=" data-id="' + additionalId + '" readonly></input>' +
                                '<div class="input-group-btn">' +
                                    '<button type="button" class="btn dropdown-toggle" data-toggle="dropdown">' +
                                        '<span class="caret"></span>' +
                                    '</button>' +
                                    '<ul class="dropdown-menu">' +
                                        '<li><option data-id="operaterSelect' + i + '" class="ard-op ard-eq" data-id-search="' + additionalId + '" value="="></option></li>' +
                                        '<li><option data-id="operaterSelect' + i + '" class="ard-op ard-gt" data-id-search="' + additionalId + '" value=">"></option></li>' +
                                        '<li><option data-id="operaterSelect' + i + '" class="ard-op ard-lt" data-id-search="' + additionalId + '" value="<"></option></li>' +
                                        '<li><option data-id="operaterSelect' + i + '" class="ard-op ard-ge" data-id-search="' + additionalId + '" value=">="></option></li>' +
                                        '<li><option data-id="operaterSelect' + i + '" class="ard-op ard-le" data-id-search="' + additionalId + '" value="<="></option></li>';

            if (columnTypes[i].type == "date-dd-mmm-yyyy") {
                operatorUI = operatorUI + '<li><option data-id="operaterSelect' + i + '" class="ard-op ard-et" data-id-search="' + additionalId + '" value="[ ]"></option></li>';
            }

            operatorUI = operatorUI + '</ul>' +
                                '</div>' +
                            '</div>' +
                        '</li>';
        }

        var menuRight = "dropdown-menu-right";
        if (i == 0) menuRight = "";
        //headerCol.append("<div class='btn-group dropdown-group' style='display:inline;'>" +
        headerCol.append("<span id='dropdown-group" + i + "' class='btn-group dropdown-group' style='display:block;padding-right:8px'>" +
                             "<span id='headerFilter" + i + "' class='fa fa-filter dropdown-toggle true-datatable-filter' aria-haspopup='true' aria-expanded='false'></span>" +
                             "<ul class='dropdown-menu " + menuRight + "'>" +
                                 '<li style="padding:15px;">' +
                                    '<span id="closeFilter' + i + '" class="fa fa-times icon-success opacityDown pull-right" style="color:#d9534f;"></span>' +
                                 '</li>' +
                                 operatorUI +
                                 '<li style="padding:8px;">' +
                                    '<input type="text" id="' + additionalId + '" class="input-filter-column form-control ' + additionalClass + '" placeholder="Search ' + headerCol.text() + '" data-col="' + i + '" data-operation="operaterSelect' + i + '"/>' +
                                 '</li>' +
                                 '<li style="padding-bottom: 8px;padding-left: 8px;padding-right: 8px;">' +
                                    '<button type="button" class="btn btn-primary fa fa-trash-o" id="' + additionalClearId + '" data-input-id="operaterSelect' + i + '"></button></li>' +
                             "</ul>" +
                         "</span>");
        /*
        headerCol.addClass("btn-group dropdown-group");
        headerCol.append(    "<span id='headerFilter" + i + "' class='fa fa-filter dropdown-toggle' aria-haspopup='true' aria-expanded='false'></span>" +
                             "<ul class='dropdown-menu " + menuRight + "'>" +
                                 '<li style="padding:15px;">' +
                                    '<span id="closeFilter' + i + '" class="fa fa-times icon-success opacityDown pull-right" style="color:#d9534f;"></span>' +
                                 '</li>' +
                                 operatorUI +
                                 '<li style="padding:8px;">' +
                                    '<input type="text" id="' + additionalId + '" class="input-filter-column form-control ' + additionalClass + '" placeholder="Search ' + headerCol.text() + '" data-col="' + i + '" />' +
                                 '</li>' +
                                 '<li style="padding-bottom: 8px;padding-left: 8px;padding-right: 8px;">' +
                                    '<button type="button" class="btn btn-primary fa fa-trash-o" id="' + additionalClearId + '"></button></li>' +
                             "</ul>" 
                            );
        */

        //Set Clear Filter Button
        $("#" + additionalClearId).data("additionalId", "#" + additionalId);
        $("#" + additionalClearId).click(function () {
            $('#' + $(this).data("input-id")).val("=");
            $($(this).data("additionalId")).val("");
            $($(this).data("additionalId")).trigger("keyup", []);
        });

        //if (additionalClass == "datePick") {
        //    //$("#"+additionalId).datepicker();
        //    $("#" + additionalId).datepicker({
        //        autoclose: true,
        //        language: 'th-th',
        //        format: 'dd/mm/yyyy'
        //    });
        //}

        //Set Button Style
        var headerFilter = $("#headerFilter" + i);
        headerFilter.css("float", "right");
        headerFilter.css("margin-left", "5px");
        headerFilter.css("margin-top", "2px");
        headerFilter.click(function () {
            $('.dropdown-group').removeClass("open");
            $($(this).parent()).addClass("open");
            $(document).data("filterDropdown", "#" + $($(this).parent()).attr("id"));
        });

        //Close Dropdown
        $('#closeFilter' + i).click(function () {
            $('.dropdown-group').removeClass("open");
        });
    }
    // Apply the search
    o.DataTable().columns().every(function () {
        var that = this;

        //Add Event Keyup Change
        $('.input-filter-column', this.header()).on('keyup change', function () {

            that.draw();
            /*
            if ($(this).hasClass("datePick") || $(this).hasClass("numberInput")) {
                that.draw();
            }
            else{
                if (that.search() !== this.value) {
                    that.search(this.value).draw();
                }
            }
            */

            //Change Color Filter
            if (this.value) {
                $("#headerFilter" + $(this).data("col")).css("color", "#FF530D");
            } else {
                if ($('#' + $(this).data("operation")).val() == "[ ]") {
                    $("#headerFilter" + $(this).data("col")).css("color", "#FF530D");
                } else {
                    $("#headerFilter" + $(this).data("col")).css("color", "");
                }
            }

        });

        $('.sign-operator').on('keyup change', function () {
            $("#" + $(this).data("id")).trigger("change");
        });

    });//End Apply the search

    //unbind header sorting
    $('.disable-sorting').unbind('click');

    //Fix column width - Chairith
    var oTable = $(tableId).dataTable();
    for (var i = 0; i < oTable.fnSettings().aoColumns.length; i++) {
        //console.log($('span.true-column-header-sorting span:eq(20)'));

        //if ($('span.true-datatable-span-header:eq(' + i + ')').length) {
        //    console.log("found");
        //} else {
        //    console.log("notfound");
        //}

        oTable.fnSortListener($('span.true-datatable-span-header:eq(' + i + ')'), i);
    }
    //End fix column width - Chairith

    //Add Event Close Dropdown
    $(document).data("filterDropdown", "");
    $(document).mouseup(function (e) {
        if ($(document).data("filterDropdown") != "") {
            var container = $($(document).data("filterDropdown"));

            if (!container.is(e.target) // if the target of the click isn't the container...
                && container.has(e.target).length === 0) // ... nor a descendant of the container
            {
                $('.dropdown-group').removeClass("open");
                $(document).data("filterDropdown", "");
            }
        }
    });

    //disable close dropdown
    //$('.dropdown-menu').click(function(event){
    // event.stopPropagation();
    //});​

    //Set Select Value Dropdown Operation
    $(".dropdown-operation option").click(function () {
        $("#" + $(this).data("id")).val($(this).val());
        $("#" + $(this).data("id-search")).trigger("change");
    });

    return this; // This is needed so others can keep chaining off of this
};

$.fn.dataTable.ext.search.push(
    function (settings, data, dataIndex) {
        var result = true;

        for (var idxData = 0; idxData < data.length; idxData++) {
            var inputId = "#inputFilter" + idxData;
            var operatorSelect = "#operaterSelect" + idxData;
            var value = data[idxData];//data[$(".open .datePick").data("col")];

            if ($(inputId).length == 0) {
                continue;
            }

            if ($(inputId).hasClass("datePick")) {
                /*
                    Date Search
                */
                var operation = $(operatorSelect).val();//$(".open select").val();
                var valFil = $(inputId).val();//$(".open .datePick").val();

                if (operation == "[ ]") {
                    if (value == "") {
                        result = true;
                    } else {
                        result = false;
                    }
                } else {

                    if (valFil == "") {
                        result = true;
                    } else {

                        var dateInputIsOk = true;
                        var colArr = value.split("@");
                        if (typeof (colArr) == "undefined" || colArr == null)
                            dateInputIsOk = false;
                        if (colArr.length != 3)
                            dateInputIsOk = false;

                        if (dateInputIsOk == true) {
                            var dateArr = colArr[1].split("/");
                            if (typeof (dateArr) == "undefined" || dateArr == null)
                                dateInputIsOk = false;
                            if (dateArr.length != 6)
                                dateInputIsOk = false;
                        }

                        var dateFilArr = valFil.split("/");
                        if (typeof (dateFilArr) == "undefined" || dateFilArr == null)
                            dateInputIsOk = false;
                        if (dateFilArr.length != 3)
                            dateInputIsOk = false;

                        if (dateInputIsOk == false) {
                            result = false;
                        } else {

                            var date = "", dateFil = "";

                            if (dateArr.length != 6 || dateFilArr.length != 3) {
                                result = true
                            } else {
                                for (var i = 3 - 1; i >= 0; i--) {
                                    date = date + dateArr[i];
                                }

                                for (var i = dateFilArr.length - 1; i >= 0; i--) {
                                    dateFil = dateFil + dateFilArr[i];
                                }

                                if (operation == ">") {
                                    if (date > dateFil)
                                        result = true;
                                    else
                                        result = false;
                                } else if (operation == "<") {
                                    if (date < dateFil)
                                        result = true;
                                    else
                                        result = false;
                                } else if (operation == ">=") {
                                    if (date >= dateFil)
                                        result = true;
                                    else
                                        result = false;
                                } else if (operation == "<=") {
                                    if (date <= dateFil)
                                        result = true;
                                    else
                                        result = false;
                                } else {
                                    if (date == dateFil)
                                        result = true;
                                    else
                                        result = false;
                                }

                            }//if (dateArr.length != 3 || dateFilArr.length != 3)
                        }//if (dateInputIsOk == false) {
                    }//if (valFil == "") {
                }//if (operation == "[]") {
            } else if ($(inputId).hasClass("numberInput")) {
                /*
                    Number Search
                */
                var operation = $(operatorSelect).val(); //$(".open select").val();
                var valFil = $(inputId).val();//$(".open .numberInput").val();

                if (valFil == "") {
                    result = true;
                } else {

                    var comma = ',';
                    var regEx = new RegExp(comma, 'g');
                    value = value.replace(regEx, "");
                    valFil = valFil.replace(regEx, "");
                    var num = Number(value);
                    var numFil = Number(valFil);

                    if (operation == ">") {
                        if (num > numFil)
                            result = true;
                        else
                            result = false;
                    } else if (operation == "<") {
                        if (num < numFil)
                            result = true;
                        else
                            result = false;
                    } else if (operation == ">=") {
                        if (num >= numFil)
                            result = true;
                        else
                            result = false;
                    } else if (operation == "<=") {
                        if (num <= numFil)
                            result = true;
                        else
                            result = false;
                    } else {
                        if (num == numFil)
                            result = true;
                        else
                            result = false;
                    }
                }
            } else {
                /*
                    String Search
                */

                var valFil = $(inputId).val();//$(".open .numberInput").val();
                if (valFil == "") {
                    result = true;
                } else {
                    var valFilUpperCase = valFil.toUpperCase();
                    var valueUpperCase = value.toUpperCase();
                    if (valueUpperCase.indexOf(valFilUpperCase) >= 0) {
                        result = true;
                    } else {
                        result = false;
                    }
                }
            }

            if (!result) {
                return result;
            }

        }

        return result;
    }
);

$.fn.GetDatatableData = function (attr) {
    var tableElement = $(this).parents('table');
    var trElement = $(this).parents('tr');
    var table = tableElement.DataTable();


    //Get Data
    if ($(trElement).attr("class").indexOf("child") < 0)
        var data = table.row(trElement).data();
    else
        var data = table.row($(trElement).prev()).data();

    if (attr)
        return data[attr.index];
    else
        return data;
}

$.fn.TableNoSortingAndFilter = function (columnTypes, options) {
    var o = $(this[0]) // It's your element
    var tableId = "#" + o.attr("id");

    //Hide Page Number and Search Box
    $($(tableId + "_wrapper .row")[0]).css("display", "none");

    //create element link
    var headerColList = $(tableId + " thead th");
    for (var i = 0; i < headerColList.length; i++) {

        //Copy Sorting Column
        var headerCol = $(headerColList[i]);
        headerCol.addClass("columnStyle");

        //Remove Sorting 
        headerCol.addClass("true-table-column-no-sorting");

        var spantext = jQuery('<span/>', {
            id: 'dtHeaderText',
            class: 'true-datatable-span-header',
            text: headerCol.html()
        });

        headerCol.html(spantext);
        headerCol.addClass("true-datatable-header");


        //Add new Sorting Colur
        //headerColClone.html("<p class='limit_col'><a href='#' style='text-decoration: none;'>" + headerCol.html() + "</a></p>");
        headerCol.addClass("disable-sorting");
        //headerCol.html(headerColClone); 
        //End fix column width - Chairith

        //Check Column is render by content
        if (columnTypes[i].hasOwnProperty("defaultContent")) {
            continue;
        }

        //Display Time
        if (columnTypes[i].DisplayTime) {
            var nameDate = "." + columnTypes[i].data + "Date";
            var nameDateTime = "." + columnTypes[i].data + "DateTime";
            $(nameDate).css("display", "none");
            $(nameDateTime).css("display", "");
            $(document.body).append("<style>" + nameDate + "{display:none !important;} " + nameDateTime + "{display:inline !important;}</style>")
        }

        //Display Option
        if (columnTypes[i].hidden) {
            var colNum = i + 1;
            $(tableId).addClass("hide_col_" + colNum);
        }

        //Add filter
        var additionalClass = "", additionalId = "inputFilter" + i; additionalClearId = "clearFilter" + i, operatorUI = "";
        if (columnTypes[i].type == "date-dd-mmm-yyyy") {
            additionalClass = "datePick datepicker-th";
        }

        if (columnTypes[i].type == "num" ||
            columnTypes[i].type == "currency") {
            additionalClass = "numberInput";
        }

        //Set Clear Filter Button
        $("#" + additionalClearId).data("additionalId", "#" + additionalId);
        $("#" + additionalClearId).click(function () {
            $('#' + $(this).data("input-id")).val("=");
            $($(this).data("additionalId")).val("");
            $($(this).data("additionalId")).trigger("keyup", []);
        });
        
    }

    //Fix column width - Chairith
    var oTable = $(tableId).dataTable();
    for (var i = 0; i < oTable.fnSettings().aoColumns.length; i++) {
        
        oTable.fnSortListener($('span.true-datatable-span-header:eq(' + i + ')'), i);
    }
    //End fix column width - Chairith

    return this; // This is needed so others can keep chaining off of this
};