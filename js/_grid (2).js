
function Grid(controller, action) {
    var that = this;
    this.controller = controller;
    this.action = (action == null ? "UpdateField" : action);

    this.fieldsValidators = [{}];
}


Grid.prototype.bindFields = function () {
    var grid = this;

    $("td input").focus(function () {
        if ($(this).data("col") == undefined) return;

        $(this).addClass("gridFieldEdit");
    });


    $("td input").blur(function () {
        if ($(this).data("col") == undefined) return;

        $(this).removeClass("gridFieldEdit");
        //if field not modified: return
        if ($(this).val() == $(this).data("val")) return;

        //checkField
        if (!grid.checkField($(this).data("col").toLowerCase(), $(this).val())) {
            var msgError = grid.fieldsValidators.First("['fieldName']=='" + $(this).data("col").toLowerCase() + "'").msg;
            alert(msgError);
            $(this).val($(this).data("val"));
            $(this).focus();
            return;
        }

        var idValue = $("#" + $(this).data("row")).val();

        //set callback
        var that = this;
        var callback = function () {
            $(that).data("val", $(that).val());
        };

        //save
        grid.save(idValue, $(this).data("col"), $(this).val(), callback);
    });
}

Grid.prototype.checkField = function (fieldName, fieldValue) {
    var valid=true;
    $.each(this.fieldsValidators.Where("['fieldName']=='" + fieldName + "'"), function (index, item) {
        if (!item.checkValidateFunc(fieldValue)) {
            valid = false;
        }
    });

    return valid;
}

Grid.prototype.save = function (id, fieldName, fieldValue,callback) {
    var data = { id: id, fieldName: fieldName, fieldValue: fieldValue };
    //POST
    $.ajax({
        url: this.controller + "/" + this.action
        , type: "POST"
        , data: data
        , cache: false
        , success: function (node) {
            if (node.result == "success") {
                $("#alertSaved.alert-success").show();
                setTimeout(function () {
                    $(".alert-success").hide();
                }, 2000);
                if (callback != null) callback();
            }
        }
    });
}