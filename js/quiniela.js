loadModals = function (data) {
    $.get(_domainPath + '/Templates/_submitScores.tmpl.htm', function (templates) {
        $('body').append(templates);
        $('#submitScores').tmpl(data).appendTo('#modals');
    });
}

loadParticipants = function () {
    $.ajax({
        url: _domainPath + "/wapi/GetParticipants",
        success: function (data) {
            if (data.err != "") {
                $('body').append("<div style='display:none;' id='sqlError'>" + data.err + "</div>");
            }

            $.get(_domainPath + '/Templates/_userList.tmpl.htm', function (templates) {
                $('body').append(templates);
                $('#userListTemplate').tmpl(data).appendTo('#particpants');
            });
        },
        error: function (err) {
            console.log(err);
        }
    });
}

loadParticipantsEdit = function () {
    $.ajax({
        url: _domainPath + "/wapi/GetAllUsers",
        success: function (data) {
            if (data.err != "") {
                $('body').append("<div style='display:none;' id='sqlError'>" + data.err + "</div>");
            }

            $.get(_domainPath + '/Templates/_userListEdit.tmpl.htm', function (templates) {
                $('body').append(templates);
                $('#editUsers').html('');
                $('#userCount').html('(' + data.users.length + ' users)');
                $('#userEditListTemplate').tmpl(data).appendTo('#editUsers');
            });
        },
        error: function (err) {
            console.log(err);
        }
    });
}

setLang = function (lang) {
    $.ajax({
        url: _domainPath + "/wapi/SetLanguage",
        data: { lang: lang },
        success: function () {
            location.reload();
        },
        error: function (err) {
            console.log(err);
        }
    });
}

prepareScoreForm = function () {
    $('#back-to-bottom').stop(true).css({ "display": "block" }).animate({ "opacity": 1 }, 400);

    switch (_userState) {
        case "Active":
            addMatchInputBoxed();
            populateUserScores();
            break;
        case "Playing":
        case "Submitted":
            $(".alert").hide();
            $(".btn").hide();
            addMatchInputBoxed();
            populateUserScores();
            $(".score input").attr('disabled', 'disabled');
            break;

        default:
            $(".alert").hide();
            $(".btn").hide();
            break;
    }
}

addMatchInputBoxed = function () {
    $(".home").each(function () {
        var scoreId = $(this).closest('.fixture').attr('id') + "_" + $(this).find('.t-nTri').html() + '_home';
        $(this).prepend("<div class='score'><input type='text' onkeyup='maxLengthCheck(this);' onkeydown='maxLengthCheck(this);' maxlength='1' name='" + scoreId + "' /></div>");
    })
    $(".away").each(function () {
        var scoreId = $(this).closest('.fixture').attr('id') + "_" + $(this).find('.t-nTri').html() + '_away';
        $(this).prepend("<div class='score'><input type='text' onkeyup='maxLengthCheck(this);' onkeydown='maxLengthCheck(this);' maxlength='1' name='" + scoreId + "'/></div>");
    })
}

maxLengthCheck = function (object) {
    if (isNaN(object.value)) {
        object.value = 0;
    } else if (object.value.length > object.maxLength) {
        object.value = object.value.slice(0, object.maxLength);
    }
}

saveMatchScores = function (bSubmitted, form) {
    var inputs = JSON.stringify($(form).serializeArray());
    $.ajax({
        url: _domainPath + "/wapi/SubmitMatchScores",
        type: 'POST',
        data: { userId: _userId, submitted: bSubmitted, scores: inputs },
        success: function (data) {
            if (data.err == 0) {
                if (bSubmitted) {
                    _userState = 'Submitted';
                    $("#paypalForm").submit();
                } else {
                    $(".alert-success").show();
                    setTimeout(function () {
                        $(".alert-success").hide();
                    }, 3000);
                }
            } else {
                $(".alert-error").show();
                setTimeout(function () {
                    $(".alert-error").hide();
                }, 4000);
            }
        }
    });
}

populateUserScores = function () {
    $.ajax({
        url: _domainPath + "/wapi/LoadUserScores",
        data: { userId: _userId },
        success: function (data) {
            if (data) {
                // populate scores
                var s = data.scores;
                $.each(s, function (i, score) {
                    $("input[name=" + score.name + "]").val(score.value);
                });
            } else {
                if (data.msg != "") {
                    $(".alert-error").append(msg);
                    $(".alert-error").show();
                    setTimeout(function () {
                        $(".alert-error").hide();
                    }, 4000);
                }
            }
        }
    });
}

deleteUser = function (userId) {
    if (confirm("delete?")) {
        $.ajax({
            url: _domainPath + "/wapi/DeleteUser",
            data: { userId: userId },
            type: 'POST',
            success: function (data) {
                if (data.err == 0) {
                    loadParticipantsEdit();

                    $("#alertBox.alert-success").show();
                    setTimeout(function () {
                        $(".alert-success").hide();
                    }, 3000);
                } else {
                    $("#alertBox.alert-error").show();
                    setTimeout(function () {
                        $(".alert-error").hide();
                    }, 4000);
                }
            }
        });
    }
}

prepareChat = function()
{
    if (_userId == "") {
        // logout user
        $("#chatEntry").hide();
    }

    $("messageInput").show();

    // Get a reference to the root of the chat data.
    var messagesRef = new Firebase('https://quinielachat.firebaseIO.com/');

    // When the user presses enter on the message input, write the message to firebase.
    $('#messageInput').keypress(function (e) {
        if (e.keyCode == 13) {
            var currentdate = new Date();
            var datetime = " " + (currentdate.getMonth() + 1) + "/" + (currentdate.getDate())
                           + "/" + currentdate.getFullYear() + "-" + currentdate.getHours() + ":"
                           + currentdate.getMinutes() + ":" + currentdate.getSeconds();
            var name = _userName;
            var text = $('#messageInput').val();
            messagesRef.push({ name: name, text: text, date: datetime });
            $('#messageInput').val('');
        }
    });

    // Add a callback that is triggered for each chat message.
    messagesRef.limit(20).on('child_added', function (snapshot) {
        var message = snapshot.val();
        if (message) {
            $("#chatSubtitle").show();
            var data = { name: message.name, msg: message.text, datetime: message.date }
            if (findSamePost(data) == 0) {
                if (data.name != _userName) {
                    $('#popLeft').tmpl(data).appendTo($('#myChat'));
                } else {
                    $('#popRight').tmpl(data).appendTo($('#myChat'));
                }
                $('#myChat')[0].scrollTop = $('#myChat')[0].scrollHeight;
            }
        }
    });

    window.onresize = function () {
        if ($(window).height() > 600)
        {
            var myh = $(window).height() - 500;
            $("#myChat").height(myh);
        }
    };
}

findSamePost = function (data) {
    var found = $('#myChat').find("h3:contains('" + data.datetime + "')");
    if (found.length) {
        var found = $(found).find(":contains('" + data.name + "')");
    }
    return found.length;
}

persistSession = function(keepCreddentials) {
    if (supports_html5_storage() && _userId != '') {
        window.localStorage.setItem("qs", _userId + ',' + _userName);
    }
    if (keepCreddentials == false) {
        window.localStorage.removeItem("qs");
    }
}

readPersistedSession = function() {
    if (supports_html5_storage()) {
        var session = window.localStorage["qs"];
        if (session != null) {
            _userId = session.split(',')[0];
            _userName = session.split(',')[1];
            return true;
        }
    }
    return false;
}

supports_html5_storage = function() {
    try {
        return 'localStorage' in window && window['localStorage'] !== null;
    } catch (e) {
        return false;
    }
}

showUserScores = function (email) {
    $.ajax({
        url: _domainPath + "/Home/Matches",
        success: function (data, textStatus, xhr) {
            var dataPage = $(data),
                titlePage = dataPage.filter("title").text(),
                pageHead = dataPage.filter("link");
            getContent = dataPage.find(".dynamicContent").html();

            var w = window.open();
            $(w.document.head).append(pageHead);
            $(w.document.body).html(getContent);

        },
        error: function (err, xhr) {
        }
    });

}