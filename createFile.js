$(document).ready(function () {
    //顯示選取的檔案名稱
    $("#AttachFiles").change(function () {
        var files = document.getElementById("AttachFiles").files;
        var inputVal = $("label.btn-item span");
        if (files.length == 1) { inputVal.text(files[0].name); }
        for (var f = 0; f < files.length; f++) {
            inputVal.append(", " + files[f].name);
        }
        inputVal.attr("title", inputVal.text());
        if (inputVal.text().length > 20) {
            inputVal.text(inputVal.text().substring(0, 20) + "...");
        }
    })

    //上傳
    $('#btnSubmit').click(function () {
        UploadFile();
        $(this).attr('disabled', 'disabled');
        $("#complete").hide();
    })
    //將資料用ajax傳遞
    function UploadFile() {
        //將要傳的資料存到formdata
        var data = new FormData();
        $.each($("#AttachFiles").get(0).files, function (i, file) {
            data.append('attachFiles', file);
        });
        data.append('attachmentUse', $("#AttachmentUse").val());
        data.append('connId', connId);

        //console.log("after click id : %o", connId);//connect id

        var ajaxRequest = $.ajax({
            type: "POST",
            url: $("#url").val(),
            contentType: false,
            processData: false,
            data: data,
            xhr: function () {  //監聽檔案存檔的進度(xhr level2)，並非資料庫存檔進度
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", function (evt) {
                    //$('#statustxt').html(Math.round(evt.loaded / evt.total * 100) + "%");
                }, false);
                return xhr;
            }
        });
        ajaxRequest.done(function (xhr, textStatus) {
            console.log(xhr);
            $("#AttachFiles").val('');
            $("#AttachmentUse").prop('selectedIndex', 0);
            $('#btnSubmit').removeAttr('disabled');
            $("#canvas-wrap").hide('slow', () => { $("#complete").slideDown('slow'); });

        });
        ajaxRequest.fail(function (xhr, textStatus) {
            console.log(xhr);
            $('#btnSubmit').removeAttr('disabled');
        })


    }

    //建立signalR連線，並取得connection Id，當有多個client才知道回傳哪個client
    var connId;
    var hub = $.connection.uploaderHub;
    $.connection.hub.start().done(function () {
        connId = $.connection.hub.id;

    });

    //server端傳回上傳進度時，更新狀態
    hub.client.updateProgress = function (name, percentage, progress, message) {
        //$('#statustxt').html(percentage + " % " + progress);
        $("#canvas-wrap").show('slow');
        speed = percentage;//環形進度條的輸入值
        progesstxt = progress;
        fileName = name;
        if (message) { alert(message); }
    }


    //畫環形進步條 - 取得canvas
    var canvas = document.getElementById('canvas'),
        context = canvas.getContext('2d'),
        centerX = canvas.width / 2,
        centerY = canvas.height / 2,
        rad = Math.PI * 2 / 100,
        speed = 0.1,
        progesstxt = "",
        fileName = "";

    //畫環形進步條 - 動態環形
    function frontCircle(n) {
        context.save();
        context.strokeStyle = "#9b4b73";
        context.lineWidth = 5;
        context.beginPath();
        context.arc(centerX, centerY, 100, -Math.PI / 2, -Math.PI / 2 + n * rad, false);
        context.stroke();
        context.closePath();
        context.restore();
    }
    //畫環形進步條 - 底圖環形
    function backCircle() {
        context.save();
        context.beginPath();
        context.lineWidth = 2;
        context.strokeStyle = "#8c8c8c";
        context.arc(centerX, centerY, 100, 0, Math.PI * 2, false);
        context.stroke();
        context.closePath();
        context.restore();
    }
    //畫環形進步條 - 顯示進度數字
    function text(n) {
        context.save();
        context.beginPath()
        context.fillStyle = '#8c8c8c';
        context.font = "40px Courier New";
        context.textBaseline = 'middle';
        context.textAlign = "center";
        context.fillText(n.toFixed(0) + "%", centerX, centerY);

        context.font = "18px Courier New";
        context.fillText(progesstxt, centerX, centerY + canvas.height * 0.14);
        context.fill();
        context.restore();
    }
    //畫環形進步條
    (function drawFrame() {
        window.requestAnimationFrame(drawFrame);//優化並合併動畫動作在一個渲染週期，html5 API
        context.clearRect(0, 0, canvas.width, canvas.height);//清除上一畫面
        backCircle();
        text(speed);
        frontCircle(speed);
        if (speed > 100) { speed = 0; }
        //speed += 0.1;

    }());

    
})