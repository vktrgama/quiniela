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

loadStates = function () {
    $.ajax({
        url: _domainPath + "/wapi/GetStates",
        success: function (data) {
            var $states = $("#states");
            $states.empty();
            $.each(data.states, function () {
                $states.append($('<option></option>').attr("value", this).text(this));
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

            var $players = $("#players");
            $players.empty();
            var usersInvolved = 0;
            $.each(data.users, function () {
                if (this.State != "New") usersInvolved++;
                $players.append($('<option></option>').attr("value", this.Email).text(this.Name + '-' + this.Email + '-' + this.State + '-' + this.InviteCode));
            });

            $("#userCount").html('Total users:' + data.users.length + ' Involved:' + usersInvolved);
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

prepareScoreForm = function (userId, userState) {
    $('#back-to-bottom').stop(true).css({ "display": "block" }).animate({ "opacity": 1 }, 400);
    var user = (userId != "") ? userId : _userId
    var state = (userState != "") ? userState : _userState

    addMatchInputBoxed();
    populateUserScores(user);

    switch (state) {
        case "Active":
            $(".score input").attr('disabled', 'disabled');
            $("#collapseThree .score input").removeAttr("disabled")
            break;
        case "Playing":
            setTimeout(function () {
                populateResults();
            }, 1000);
        case "Submitted":
            $(".alert").hide();
            $(".btn").hide();
            $(".score input").attr('disabled', 'disabled');
            break;
        default:
            $(".alert").hide();
            $(".btn").hide();
            $(".score input").attr('disabled', 'disabled');

            break;
    }
}

addMatchInputBoxed = function () {
    // ("#newScores").find
    $(".home").each(function () {
        var matchId = (typeof $(this).closest('.fixture').attr('id') != "undefined") ? $(this).closest('.fixture').attr('id') : $(this).closest('.fixture').attr('data-id');
        var scoreId = matchId + "_" + $(this).find('.t-nTri').html() + '_home';
        $(this).prepend("<div class='score'><input type='text' onkeyup='maxLengthCheck(this);' onkeydown='maxLengthCheck(this);' maxlength='1' name='" + scoreId + "' /></div>");
    })
    $(".away").each(function () {
        var matchId = ($(this).closest('.fixture').attr('id')) ? $(this).closest('.fixture').attr('id') : $(this).closest('.fixture').attr('data-id');
        var scoreId = matchId + "_" + $(this).find('.t-nTri').html() + '_away';
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
    var btn = event.target;
    var btnCaption = $(btn).text();
    $(event.target).text($(btn).attr("data-title"));

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
                $(btn).text(btnCaption);
            } else {
                $(".alert-error").show();
                setTimeout(function () {
                    $(".alert-error").hide();
                }, 4000);
            }
        }
    });
}

populateUserScores = function (userId) {
    var user = (userId != "") ? userId : _userId;
    $.ajax({
        url: _domainPath + "/wapi/LoadUserScores",
        data: { userId: user },
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

populateResults = function () {
    $.ajax({
        url: _domainPath + "/wapi/LoadFinalResults",
        success: function (data) {
            if (data) {
                // populate scores
                var s = data.scores;
                $.each(s, function (i, score) {
                    $("input[name=" + score.name + "]").attr("title", score.value);
                    // Match exact scores
                    if ($("input[name=" + score.name + "]").val() == score.value) {
                        $("input[name=" + score.name + "]").css("background-color", "#06FA2D");
                    } else {
                        $("input[name=" + score.name + "]").css("background-color", "#B43659").css("color", "#fff");
                    }

                    // Determine winner
                    var inputs = $("div[id=" + score.name.split('_')[0] + "]").length > 0 ? $("div[id=" + score.name.split('_')[0] + "]").find("input") : $("div[data-id=" + score.name.split('_')[0] + "]").find("input")
                    if (inputs[0].title != "" && inputs[1].title != "" && inputs[0].value != "" && inputs[1].value != "") {
                        if (inputs[0].title - inputs[1].title == 0 && inputs[0].value - inputs[1].value == 0) {
                            // draw
                            $(inputs[0]).css("background-color", "#06FA2D").css("color", "#000");
                            $(inputs[1]).css("background-color", "#06FA2D").css("color", "#000");
                        }
                        if (inputs[0].title - inputs[1].title > 0 && inputs[0].value - inputs[1].value > 0) {
                            // home winner
                            $(inputs[0]).css("background-color", "#06FA2D").css("color", "#000");
                        } 
                        if (inputs[0].title - inputs[1].title < 0 && inputs[0].value - inputs[1].value < 0) {
                            // away winner
                            $(inputs[1]).css("background-color", "#06FA2D").css("color", "#000");
                        }
                    }
                });
            }
        }
    });
}

updateUser = function()
{
    if (confirm("update?")) {
        var userId = $("#players option:selected").val();
        var state = $("#states option:selected").val();
        if (userId != "" && state != "") {
            $.ajax({
                url: _domainPath + "/wapi/UpdateField",
                data: { id: userId, fieldName: "State", fieldValue: state },
                type: 'POST',
                success: function (data) {
                    loadParticipantsEdit();

                    $("#alertBox.alert-success").html("User updated");
                    $("#alertBox.alert-success").show();
                    setTimeout(function () {
                        $(".alert-success").hide();
                    }, 3000);
                }
            });
        }
    }
}

deleteUser = function () {
    var userId = $("#players option:selected").val();
    if (confirm(userId + " delete?")) {
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

    var currMsgName = "";
    var alignTo = "#popLeft";
    // Add a callback that is triggered for each chat message.
    messagesRef.limit(20).on('child_added', function (snapshot) {
        var message = snapshot.val();
        if (message) {
            $("#loadingMsg").hide();
            $("#chatSubtitle").show();
            var data = { name: message.name, msg: message.text, datetime: message.date }
            if (findSamePost(data) == 0) {
                if (currMsgName == "")
                    currMsgName = data.name;

                if (data.name != currMsgName) {
                    currMsgName = data.name;
                    alignTo = (alignTo == "#popLeft") ? "#popRight" : "#popLeft";
                }

                $(alignTo).tmpl(data).appendTo($('#myChat'));
                $('#myChat')[0].scrollTop = $('#myChat')[0].scrollHeight;
            }
        }
    });

    window.onresize = function () {
        if ($(window).height() > 600)
        {
            // var myh = $(window).height() - 500;
            // $("#myChat").height(myh);
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
        window.localStorage.setItem("qs", _userId + ',' + _userName + ',' + _userState);
    }
    if (keepCreddentials == false) {
        window.localStorage.removeItem("qs");
    }
}

resetSession = function () {
    var session = window.localStorage["qs"];
    if (session != null) {
        window.localStorage.removeItem("qs");
    }
}

readPersistedSession = function() {
    if (supports_html5_storage()) {
        var session = window.localStorage["qs"];
        if (session != null) {
            _userId = session.split(',')[0];
            _userName = session.split(',')[1];
            _userState = session.split(',')[2];
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
    var w = window.open(_domainPath + "/Home/Matches/dummy?userid=" + encodeURIComponent(email),
        "_blank", "location=0, status=0, width=1128, height=780, scrollbars=1");
}

calculatePoints = function () {
    var matchId = $("#matches option:selected").val();
    var thscore = $("#matchHome").val();
    var tascore = $("#matchAway").val();

    $.ajax({
        url: _domainPath + "/wapi/CalcPoints",
        type: 'POST',
        data: { matchId: matchId, th: thscore, ta: tascore },
        success: function (data) {
            if (data.err == 0) {
                $("#alertBox.alert-success").html("Puntos Calculados");
                $("#alertBox.alert-success").show();
                setTimeout(function () {
                    $(".alert-success").hide();
                    location.reload();
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

loadMatches = function (list) {
    var decodeList = $("<div/>").html(list).text();
    var data = eval(decodeList);
    if (data.length > 0) {
        var $matches = $("#matches");
        $matches.empty();
        $.each(data, function () {
            $matches.append($('<option></option>').attr("value", this.MatchId).text(this.MatchName));
        });
    }
}

initSession = function () {
    // user logged in
    if (_userId != "") {
        persistSession(true);
        $(".loginForm").hide();
        $("#logoutLink").show();
        $("#welcomeUser").html(_welcome + " " + _userName);
        $("#welcomeUser").show();
        $("#logoutLink a div").bind("click", function () {
            _userId = '';
            _userState = '';
            window.location.replace(_domainPath + "/");
            persistSession(false);
        });

        if (_userState == "Admin") {
            $(".sf-menu").find("li:contains('Admin')").find("a").show()
        }
    } else {
        $("#logoutLink").hide();
    }
}

SendReminderEmail = function()
{
    $.ajax({
        url: _domainPath + "/wapi/SendReminder",
        type: 'POST',
        success: function (data) {
            if (data.err == 0) {
                setTimeout(function () {
                    $(".alert-success").hide();
                    location.reload();
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