function validateEmail(email) {
    var reg = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/
    if (reg.test(email)) {
        return true;
    }
    else {
        return false;
    }
}

$(document).ready(function () {
    $("#searchButton").click(function () {
        location.href = "http://dotnetpattern.com/search/" + $("#searchText").val();
    })

    $("#subscribeButton").click(function () {
        var emailTxt = $("#emailText").val();
        $("#subscribeError").hide();
        $("#subscribeSuccess").hide();
        $("#subscribeAlreadyRegistered").hide();
        var reg = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/
        if (reg.test(emailTxt)) {
            var obj = {
                email: emailTxt
            };
            $.post('/SubscribeSubmit', obj, function (data, status) {
                if(data == "0")
                {
                    $("#subscribeError").show();
                }
                else if( data == "2")
                {
                    $("#subscribeAlreadyRegistered").show();
                }
                else
                {
                    $("#subscribeSuccess").show();
                }
            });
        }
        else {
            $("#subscribeError").show();
        }
    })
    //Share Sidebar Left Right

    //$("#shareSidebarContent button").css({ 'display': 'none' });
    //$("#shareSidebarRight").css({ 'display': 'none' });
    //$("#shareSidebarRight").click(function () {
    //    $("#shareSidebarContent button").css({ 'display': 'none' });
    //    $("#shareSidebarRight").css({ 'display': 'none' });
    //    $("#shareSidebarLeft").css({ 'display': 'block' });
        
    //});

    //$("#shareSidebarLeft").click(function () {
    //    $("#shareSidebarContent button").css({ 'display': 'block' });
    //    $("#shareSidebarRight").css({ 'display': 'block' });
    //    $("#shareSidebarLeft").css({ 'display': 'none' });
    //});
    
});


jQuery(document).ready(function () {
    "use strict";

    /******************************************
     jQuery MeanMenu
    ******************************************/

    jQuery('.mobile-menu nav').meanmenu({
        meanScreenWidth: "1200",
        meanMenuContainer: ".mobile-menu",
    });





    /******************************************
        subDropdown
    ******************************************/

    jQuery(".subDropdown")[0] && jQuery(".subDropdown").on("click", function () {
        jQuery(this).toggleClass("plus"), jQuery(this).toggleClass("minus"), jQuery(this).parent().find("ul").slideToggle()
    })


    /******************************************
        tooltip
    ******************************************/

    jQuery('[data-toggle="tooltip"]').tooltip();
})

/******************************************
   totop
   ******************************************/
// browser window scroll (in pixels) after which the "back to top" link is shown
var offset = 300,
    //browser window scroll (in pixels) after which the "back to top" link opacity is reduced
    offset_opacity = 1200,
    //duration of the top scrolling animation (in ms)
    scroll_top_duration = 700,
    //grab the "back to top" link
    jQueryback_to_top = jQuery('.totop');

//hide or show the "back to top" link
jQuery(window).scroll(function() {
    (jQuery(this).scrollTop() > offset) ? jQueryback_to_top.addClass('totop-is-visible'): jQueryback_to_top.removeClass('totop-is-visible totop-fade-out');
    if (jQuery(this).scrollTop() > offset_opacity) {
        jQueryback_to_top.addClass('totop-fade-out');
    }
});

//smooth scroll to top
jQueryback_to_top.on('click', function(event) {
    event.preventDefault();
    jQuery('body,html').animate({
        scrollTop: 0,
    }, scroll_top_duration);
});