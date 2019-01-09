$(document).ready(function () {
    //Combobox
    $("select").not(".true-multipleselect").select2();
    //Select2 for Modal
    $.fn.modal.Constructor.prototype.enforceFocus = function () { };

    $('.datepicker-th').datepicker(
        $.extend({
            changeMonth: true,
            changeYear: true,
            dateFormat: 'dd/mm/yy',
            //isBuddhist: true,
        },
        $.datepicker.regional['en-GB']
    ));

    $('.mobile').inputmask({
        'mask': "[999-9999999]"
    });

    $('.integer').inputmask({
        'mask': '9{0,10}',
        'rightAlign': true,

    });
    $('.integeronly').inputmask({
        'alias': 'integer',
        'greedy': false,
        'rightAlign': true
        //'mask': '9{0,200}',
        //'rightAlign': true,
    });

    //$('.decimal2digit').inputmask({        
    //    'alias': 'decimal',
    //    'placeholder': '0',
    //    'mask': '9{0,14}[.]9{0,2}',
    //    'greedy': false,
    //    'rightAlign': true,
    //    //'groupSeparator': '.',
    //    //'autoGroup': true
    //});

    $('.decimal2digit').inputmask("decimal", {
        placeholder: "0",
        digits: 2,
        greedy: false,
        digitsOptional: false,
        radixPoint: ".",
        groupSeparator: ",",
        autoGroup: true,
        allowPlus: false,
        allowMinus: false,
        clearMaskOnLostFocus: false,
        removeMaskOnSubmit: true,
        autoUnmask: true,
        //onUnMask: function (maskedValue, unmaskedValue) {
        //    var x = unmaskedValue.split(',');
          
        //    if (x.length != 2)
        //        return "0.00";
        //    return x[0].replace(/\./g, '') + '.' + x[1];
        //}
    });

    $('.decimal2digitwithminus').inputmask("decimal", {
        radixPoint: ".",
        digits: 2,
        allowMinus: true,
        rightAlign: true
    });

    $('.nodecimal').inputmask({
        'alias': 'integer',
        'greedy': false,
        'rightAlign': true
    });

    $('.decimal4digit').inputmask({
        'alias': 'decimal',
        'mask': '9{0,12}[.]9{0,4}',
        'greedy': false,
        'rightAlign': true
    });

    $('.idcard').inputmask({
        'mask': '9{13,13}'
    });

    $('.postcode').inputmask({
        'mask': '9{5,5}'
    });

    $('.numberonly').inputmask({
        'mask': '9{0,200}'
    });

    $('input[type="checkbox"], input[type="radio"]').not(".true-radio-exception").iCheck({
        checkboxClass: "icheckbox_minimal-red",
        radioClass: "iradio_minimal-red"
    });


});