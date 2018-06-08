function tableColumnSort(orderAscend, orderer) {

    //處理排序的link和箭頭符號
    var OrderAscend = orderer;
    var Orderer = orderer;
    console.log("OrderAscend:", OrderAscend);
    console.log("Orderer:", Orderer);

    //每一個table的標題欄，都加上連結
    $("table").find("tr").first().find("th").each(function () {
        var _href = $(this).find("a").attr("href");

        //OrderAscend != true && Orderer == 欄位 箭頭改成向上，表示由此欄位逆向排序
        //其餘情況加上orderDescend=true ，表示可逆向排序的欄位
        if (_href != undefined && Orderer != "") {
            if (_href.includes(Orderer) && OrderAscend != "True") {
                $(this).find("a").find("label").wrap("<span class='dropup'></span>")
            }
            else {
                $(this).find("a").attr("href", _href + "&orderDescend=true");
            }

            //被選到的欄位轉成紅色
            if (_href.includes(Orderer)) {
                if ($(this).find("a").find(".dropup").length != 0) {
                    $(this).find("a").find("label").css({ 'border-bottom': ' 4px solid red' });
                } else {
                    $(this).find("a").find("label").css({ 'border-top': ' 4px solid red' });
                }
            }
        }
        if (OrderAscend == "True") {
            $(this).find("a").attr("href", _href + "&orderDescend=true");
        }

    })

}