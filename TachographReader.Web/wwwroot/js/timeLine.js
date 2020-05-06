function initData(activityPeriodsSlot, slot, height) {
    if (activityPeriodsSlot.length == 0) return;
    var container1 = document.getElementById(slot);
    var data = [];
    var endUtc = activityPeriodsSlot[activityPeriodsSlot.length - 1].endUtc;
    var startUtc = activityPeriodsSlot[0].startUtc;
    $.each(activityPeriodsSlot,
        function (i, v) {
            var activity = "";
            var style = "";
            switch (v.activityType) {
                case 1:
                    {
                        activity = localization.Availability;
                        style =
                            "background-color:green;height:25px; border-color:transparent;border-radius:0;margin-top: 20px;";

                    }
                    break;
                case 2:
                    {
                        activity = localization.Work;
                        style =
                            "background-color:#dab30a;height:32px;; border-color:transparent;border-radius:0;margin-top: 20px;border-width:0";

                    }
                    break;
                case 3:
                    {
                        activity = localization.Drive;
                        style = "background-color:#048b9a;height:38px; border-color:transparent;border-radius:0;margin-top: 20px;border-width:0";

                    }
                    break;
                case 0:
                    {
                        activity = localization.BreakRest;
                        style = "background-color:#DC143C;height:11px; border-color:transparent;border-radius:0;margin-top: 20px;border-width:0";

                    }
                    break;
                default:
                    {
                        activity = "Repos";
                        style = "background-color:#DC143C;height:11px; border-color:transparent;border-radius:0;margin-top: 20px;border-width:0";

                    }
                    break;
            }
            if (v.endUtc == null && activityPeriodsSlot.length > i + 1)
                v.endUtc = activityPeriodsSlot[i + 1].startUtc;
            var startUtcTime = moment(v.startUtc).format("hh:mm");
            var endUtcTime;
            var nextDuration = 0;
            endUtcTime = moment(v.endUtc).format("hh:mm"); 
            var duration = "";
            if (v.duration !== "") {
                // console.log(nextDuration);
                duration = secondsToHms(parseInt(v.duration) + parseInt(nextDuration));
            }
            var template = '' + activity + ' de ' + startUtcTime + ' à ' + endUtcTime;
            if (duration !== "") {
                template = template + ' ('+localization.Duration +' ' + duration + ')';
            }
            //    console.log(v.duration);
            template = template + '\n';
            if (v.driverName !== null)
                template = template + localization["Driver"]+' : ' + v.driverName + '\n';


            data.push({ id: i, group: null, content: v.activityType, style: style, start: v.startUtc, end: v.endUtc, title: template });



        });
    var result = new vis.DataSet(data);
    //  console.log(endUtc);
    loadTimelineChart(container1, result, startUtc, endUtc, height, activityPeriodsSlot);
}

function secondsToHms(seconds) {
    var d = Number(seconds);
    var h = Math.floor(d / 3600);
    var m = Math.floor(d % 3600 / 60);
    var s = Math.floor(d % 3600 % 60);

    var hDisplay = h > 0 ? h + (h == 1 ? " heure, " : " heures, ") : "";
    var mDisplay = m > 0 ? m + (m == 1 ? " minute " : " minutes ") : "";
    //  var sDisplay = s > 0 ? s + (s == 1 ? " second" : " seconds") : "";
    if (hDisplay !== "" || mDisplay !== "")
        return hDisplay + mDisplay;
    return "moins d'1 min";

}

function loadTimelineChart(container1, data, startUtc, endUtc, height, activityPeriodsSlot) {

    // var currentBar= $(container1).attr('id');
    var today = new Date();
    today.setHours(24, 0, 0, 0);
    var max;
    // console.log((endUtc));
    if (sameDay(new Date(endUtc), new Date())) {
        // ...
        max = today;
        endUtc = today;
    } else max = endUtc;
    var options = {
        width: '100%',
        locale: 'fr',
        height: height,
        editable: false,
        margin: {
            item: 10
        },
        zoomable: true,
        //  maxZoom:20,
        end: endUtc,
        start: startUtc,
        min: startUtc,
        max: max,
        showCurrentTime: false,
        template: function (item) {
            var template = item.content;
            return "";
        },
        stack: false,
        format: {
            minorLabels: {
                minute: 'HH:mm',
                hour: 'HH'

            }
        }

    }

    // Create a Timeline
    var timeline = new vis.Timeline(container1, null, options);
    timeline.setItems(data);


}

function sameDay(d1, d2) {
    return d1.getUTCFullYear() === d2.getUTCFullYear() &&
        d1.getUTCMonth() === d2.getUTCMonth() &&
        d1.getUTCDate() === d2.getUTCDate();
}