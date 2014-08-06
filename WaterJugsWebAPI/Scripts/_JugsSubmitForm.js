
$(document).ready(function () {
    // Loading animation
    $(document).ajaxStart(function () { $('body').addClass("loading"); });
    $(document).ajaxStop(function () { $('body').removeClass("loading"); });

    // Forms validation
    $("#formJugs").validate({
        rules: {
            txtJugA: {
                required: true,
                number: true
            },
            txtJugB: {
                required: true,
                number: true
            },
            txtTarget: {
                required: true,
                number: true
            }
        }
    });

    // Submit click event
    $("#btnSubmit").click(function () {
        if ($('#formJugs').valid()) {
            submitJugs();
        }
    });
});

function formatItem(item) {
    return "[ " + item.JugA + " | " + item.JugB + " ]";
}

function submitJugs() {
    var data = {
        JugA_Max: $("#txtJugA").val(),
        JugB_Max: $("#txtJugB").val(),
        TargetGallons: $("#txtTarget").val()
    };

    $.ajax({
        url: '/WaterJugsService/Submit',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify(data),
        success: function (response) {
            var stepCount = -1;
            // remove old data
            $('#steps li').remove();
            $('#stepCount p').remove();
            $.each(response, function (key, item) {
                // add new data
                $('<li>', { text: "Step " + key + ": " + formatItem(item) }).appendTo($('#steps'));
                stepCount++;
            });

            $('<p>', { text: 'Found target in ' + stepCount + " steps!" }).appendTo($('#stepCount'));
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('ERROR');
            console.log(errorThrown);
            console.log(jqXHR);
            alert(textStatus + ": " + errorThrown);
        }
    });
}