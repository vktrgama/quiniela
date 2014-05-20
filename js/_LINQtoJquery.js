//LinQ to javascript extensions methods for class Array
//Need JQuery


// *** Samples of use ****

    var myArray = [
                     { id: 1, name: "luis", checked: false }
                    , { id: 2, name: "juan", checked: true }
                    , { id: 3, name: "ana", checked: false }
    ];


    //console.log("Count sample: " + myArray.Count("item['checked']==true || item['id']==1"));
    ////word item it's not necessary
    //console.log("Count sample: " + myArray.Count("['checked']==true || ['id']==1"));
    //console.log("Count sample: " + myArray.Count("['name']=='luis'"));
    ////If not params, return array length
    //console.log("Count sample: " + myArray.Count());

    //console.log("First sample: " + myArray.First("['id']==1").id);
    //console.log("First sample: " + myArray.First().id);

    //console.log("Exists and First sample: " + (myArray.Exists("['name']=='anaXX'")
    //                                            ? myArray.First("['name']=='anaXX'").id
    //                                            : "not exits"));

    //var vectResult = myArray.Where("item['id']==2");

//*** Samples of use ****



function itemParser(str) {
    var result;

    result = str.replace(/item/gi, "");
    result = result.replace(/\[/g, "item[");

    return result;
}


//Extend Array with prototype

Array.prototype.Count = function (expr) {
    if (expr == null || expr == "") return this.length;

    return ($.grep(this, function (item, index) {
        return (eval(itemParser(expr)));
    }).length);
};

Array.prototype.First = function (expr) {
    if (expr == null || expr == "") expr = "true";

    var result = ($.grep(this, function (item, index) {
        return (eval(itemParser(expr)));
    }));

    return (result.length > 0 ? result[0] : null);
};

Array.prototype.Last = function (expr) {
    if (expr == null || expr == "") expr = "true";

    var result = ($.grep(this, function (item, index) {
        return (eval(itemParser(expr)));
    }));

    return (result.length > 0 ? result[result.length-1] : null);
};


Array.prototype.Exists = function (expr) {
    if (expr == null || expr == "") expr = true;

    var result = ($.grep(this, function (item, index) {
        return (eval(itemParser(expr)));
    }));

    return (result.length > 0);
};


Array.prototype.Where = function (expr) {
    return ($.grep(this, function (item, index) {
        return (eval(itemParser(expr)));
    }));
};


Array.prototype.Remove = function (expr) {
    var array = this;
    var idx = -1;

    $.each(this, function (index, item) {
        if (eval(itemParser(expr))) { idx = index; }
    });

    if(idx>-1) return array.splice(idx, 1);

    return array;
}

