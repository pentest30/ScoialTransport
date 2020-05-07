"use strict";
var connection = new signalR.HubConnectionBuilder().withUrl("/socialDataHub").build();
connection.on("ReceiveProgress", function(val) {
    console.log("progress received", val);
    changeCircle(val);
});
connection.start().then(function () {
    console.log("signalR started ...");
     
}).catch(function (err) {
    return console.error(err.toString());
});
function isGuid(stringToTest) {
    if (stringToTest != null && stringToTest[0] === "{") {
        stringToTest = stringToTest.substring(1, stringToTest.length - 1);
    }
    var regexGuid = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/gi;
    return regexGuid.test(stringToTest);
}
function changeCircle(val) {
    $(".progress-bar").css('width', val + '%').attr('aria-valuenow', val);   
}

function getCurrentWeek() {

    return getWeek(new Date());
}
function getLastMonth() {
    var today = getLastMonth();
    return { first: beginningOfMonth(today), last: endOfMonth(today) };
}
function getLastMonthDate() {
    var today = new Date();
    const month = today.getMonth();
    today.setMonth(today.getMonth() - 1);
    while (today.getMonth() === month) {
        today.setDate(today.getDate() - 1);
    }
    return today;
}
function getWeek(date) {
    var weekMap = [6, 0, 1, 2, 3, 4, 5];
    var now = new Date(date);
    now.setHours(0, 0, 0, 0);
    var monday = new Date(now);
    monday.setDate(monday.getDate() - weekMap[monday.getDay()]);
    var sunday = new Date(now);
    sunday.setDate(sunday.getDate() - weekMap[sunday.getDay()] + 6);
    sunday.setHours(23, 59, 59, 999);
    return { first: new Date(monday), last: new Date(sunday) };
}
function getLastWeek() {
    var d = new Date();
    // set to previous week
    d.setDate(d.getDate() - 7);
    return getWeek(d);
}

function beginningOfMonth(d) {
    var date = new Date(d);
    date.setDate(1);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);
    return (date);
}

// Returns a date set to the end of the month


function endOfMonth(d) {
    var date = new Date(d);
    date.setMonth(date.getMonth() + 1);
    date.setDate(0);
    date.setHours(23);
    date.setMinutes(59);
    date.setSeconds(59);
    return (date);
}
function clearDataTablesGrouping() {
    var oSettings = $('#socialData').dataTable().fnSettings();

    for (var f = 0; f < oSettings.aoDrawCallback.length; f++) {
        if (oSettings.aoDrawCallback[f].sName == 'fnRowGrouping') {
            oSettings.aoDrawCallback.splice(f, 1);
            break;
        }
    }

    oSettings.aaSortingFixed = null;
}
function hideFirstColumn(hide) {
    var table = $('#socialData').DataTable({
        initComplete: function (settings) {
            var api = new $.fn.dataTable.Api(settings);
            api.columns([0]).visible(hide);
        }
    });
}