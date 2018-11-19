"use strict";

function createAlert(style, innerContent) {
    return "<div class=\"alert " + style + " alert-dismissible fade in\" role=\"alert\"> " +
        "<button type=\"button\" class=\"close\" data-dismiss=\"alert\" aria-label=\"Close\">" +
        "<span aria-hidden=\"true\">×</span>" +
        "</button>" + innerContent + "</div>";
}

function onCollapse(element, jobId) {
    var targetElementId = "#" + jobId + "-body";
    $(targetElementId).toggleClass('hide');

    $(element).find('.glyphicon').toggleClass('glyphicon-chevron-down glyphicon-chevron-up');
}

function onEnqueue(element, jobId) {
    var formElementId = "#" + jobId;
    var alertsElementId = formElementId + "-alerts";
    var data = $(formElementId).serializeArray();
    var launch = !requireConfirmation || confirm(messageLaunch.confirm);
    if (!launch) return;

    element.disabled = true;
    $.ajax({
        async: true,
        cache: false,
        timeout: 10000,
        url: launchUrl + "?" + idFieldName + "=" + jobId,
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        data: data,
        type: "post",
        success: function (r) {
            var jobLink = jobLinkBaseUrl + r;
            var alert = createAlert("alert-success", messageLaunch.success + "<a href=\"" + jobLink + "\"><strong>" + r + "</strong></a>");
            $(alertsElementId).append(alert);
            element.disabled = false;
        },
        error: function (r) {
            var alert = createAlert("alert-danger", messageLaunch.failure + "<br /><strong>" + r.responseText + "</strong>");
            $(alertsElementId).append(alert);
            element.disabled = false;
        }
    });
};

function onRecurring(element, jobId) {
    var formElementId = "#" + jobId;
    var alertsElementId = formElementId + "-alerts";
    var data = $(formElementId).serializeArray();
    var launch = !requireConfirmation || confirm(messageLaunch.confirmRecurring);
    if (!launch) return;

    element.disabled = true;
    $.ajax({
        async: true,
        cache: false,
        timeout: 10000,
        url: recurringUrl + "?" + idFieldName + "=" + jobId,
        contentType: "application/x-www-form-urlencoded; charset=UTF-8",
        data: data,
        type: "post",
        success: function (r) {
            var alert = createAlert("alert-success", messageLaunch.successRecurring);
            $(alertsElementId).append(alert);
            element.disabled = false;
        },
        error: function (r) {
            var alert = createAlert("alert-danger", messageLaunch.failure + "<br /><strong>" + r.responseText + "</strong>");
            $(alertsElementId).append(alert);
            element.disabled = false;
        }
    });
};
