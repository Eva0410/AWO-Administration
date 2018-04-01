function sendShareToServer(from) {
    var obj = {
        urlSlug: window.location.pathname,
        from: from
    };
    $.post('/SubmitShare', obj, function (data, status) {
        $(".shareCount").html(data);
    });
}

$(document).ready(function () {
    $(".facebookShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('0');
        window.open('http://www.facebook.com/sharer.php?u=' + fullUrl + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });

    $(".twitterShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var title = $(".pageShare").attr('data-title');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('1');
        window.open('https://twitter.com/intent/tweet?text=' + title + '&source=dotnetpattern&url=' + fullUrl + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });

    $(".linkedInShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var title = $(".pageShare").attr('data-title');
        var summary = $(".pageShare").attr('data-summary');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('2');
        window.open('https://www.linkedin.com/shareArticle?mini=true&url=' + fullUrl + '&title=' + title + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });
    $(".googlePlusShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('3');
        window.open('https://plus.google.com/share?url=' + fullUrl + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });

    $(".stumbleUponShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var title = $(".pageShare").attr('data-title');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('4');
        window.open('http://www.stumbleupon.com/submit?url=' + fullUrl + '&title=' + title + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });
    $(".redditShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var title = $(".pageShare").attr('data-title');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('5');
        window.open('https://www.reddit.com/submit?url=' + fullUrl + '&title=' + title + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });

    $(".deliciousShare").click(function () {
        var fullUrl = $(".pageShare").attr('data-fullurl');
        var title = $(".pageShare").attr('data-title');
        var sTop = window.screen.height / 2 - (218);
        var sLeft = window.screen.width / 2 - (313);
        sendShareToServer('6');
        window.open('http://delicious.com/post?url=' + fullUrl + '&title=' + title + '', 'sharer', 'toolbar=0,status=0,width=626,height=456,top=' + sTop + ',left=' + sLeft);
        return false;
    });
});

