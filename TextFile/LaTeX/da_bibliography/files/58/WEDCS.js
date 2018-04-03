window.pmc = window.pmc || {};
window.pmc.codeVersion = "2015dec14v1";
window.pmc.debugConsoleLogEnabled = false;
window.pmc.debugLogMsgs = [];
window.pmc.debugLog = function (inMsg) {
    window.pmc.debugLogMsgs.push(inMsg);
    if (window.pmc.debugConsoleLogEnabled) {
        console.log(inMsg);
    }
};
window.pmc.formatString = function (inValue) {
    var newStr;
    if (typeof inValue == "undefined") {
        return "";
    } else if (typeof inValue == "string") {
        newStr = inValue.toLowerCase().trim();
    } else {
        newStr = inValue.toString().toLowerCase().trim();
    }
    newStr = newStr.replace(/"/g, "'");
    return newStr;
};
window.pmc.passScriptCheck = function (inText) {
    if (typeof inText != "undefined") {
        if (inText.match(/<script/i)) {
            window.pmc.debugLog("passScriptCheck failed");
            return false;
        }
    }
    return true;
};
window.pmc.setAttribute = function (inJQObj, inAttrName, inAttrValue) {
    if (typeof inJQObj == "undefined" || inJQObj.length === 0 || !inAttrName || inAttrValue === "") {
        return;
    }
    inJQObj.attr(inAttrName, inAttrValue);
};
window.pmc.getMetaTagContent = function (inMTName) {
    try {
        var myMTContent = jQuery("meta[name=\'" + inMTName + "\']");
        if (myMTContent.length > 0) {
            myMTContent = myMTContent[myMTContent.length - 1].content;
        } else {
            myMTContent = null;
        }
        return myMTContent;
    } catch (e) {
        return null;
    }
};
window.pmc.setMetaTagContent = function (inMTName, inMTContent) {
    if (!inMTContent) {
        return;
    } else if (window.pmc.getMetaTagContent(inMTName) === null) {
        var myMetaTag = document.createElement("meta");
        myMetaTag.name = inMTName;
        myMetaTag.content = inMTContent;
        if (window.pmc.passScriptCheck(myMetaTag.content)) {

            document.getElementsByTagName("head")[0].appendChild(myMetaTag);
        }
    } else {
        jQuery("meta[name=\'" + inMTName + "\']").attr("content", inMTContent);
    }
};
window.pmc.checkMetaTags = function (name, val) {
    //using javascript due to some sections having old versions of jquery
    var metaTag = document.querySelector('meta[name="' + name + '"]');
    if (!metaTag) {
        window.pmc.setMetaTagContent(name, val);
    }
};
window.pmc.webtrendsCheck = function (name, val, alt) {
    var reg = new RegExp(name, 'i');
    jQuery("meta").each(function () {
        var jqName = jQuery(this).attr("name");
        if (jqName) {
            if (jQuery(this).attr("name").match(reg)) {
                var wtContent = jQuery(this).attr("content");
                wtContent = wtContent.replace(/_/g, "");
                window.pmc.setMetaTagContent(val, wtContent); //assign wt content to wedecs variable
            } else {  //webtrends variable does not exist on the page
                if (alt) {
                    window.pmc.setMetaTagContent(val, alt);
                }
            }
        }
    });
};
window.pmc.webtrendsLangLocCheck = function (msLang, msLoc) {
    var checkText = "DCSext.WClocale";
    var reg = new RegExp(checkText, 'i');
    var allMetas = document.querySelectorAll("meta");
    for (var i = 0; i < allMetas.length; i++) {
        if (allMetas[i].name.search(reg) > -1) {
            //webtrends variable exists
            //this function is specific to windows/ pages pertaining to lang and loc
            var wtContent = allMetas[i].content;
            wtContent = wtContent.replace(/_/g, "");
            wtLang = wtContent.split("-")[0];
            wtLoc = wtContent.split("-")[1];
            window.pmc.setMetaTagContent(msLang, wtLang); //assign wt content to wedecs variable
            window.pmc.setMetaTagContent(msLoc, wtLoc); //assign wt content to wedecs variable
            break;
        }
    }
};
window.pmc.pageLoad = function () {
    window.pmc.libraryTaggingEnabled = false;
    var pageURL = window.location.hostname + window.location.pathname;
    //apply common non-site section specific values to only technet hosted pages
    if (window.location.hostname.match(/technet\.microsoft\.com/i) ||
      window.location.hostname.match(/technet\.com/i) ||
      window.location.hostname.match(/^technetappsmain\./i) || //search and forums pre-prod
      window.location.hostname.match(/^tngmain\./i) || //galleries pre-prod
      window.location.hostname.match(/^techblogsrc\./i)) { //blogs pre-prod
        //Environment
        if (window.location.hostname.match(/^int\./) ||
          window.location.hostname.match(/^technetappsmain\./i) || //search and forums pre-prod
          window.location.hostname.match(/^tngmain\./i) || //galleries pre-prod
          window.location.hostname.match(/^techblogsrc\./i)) { //blogs pre-prod
            window.pmc.checkMetaTags("ms.env", "staging");
        } else {
            window.pmc.checkMetaTags("ms.env", "production");
        }
        //language/location
        var lang, loc;
        var langLoc = window.location.pathname.match(/\/..-..\//);
        if (langLoc) {
            var grabbed_langLoc = langLoc[0];
            var grabbed_lang = grabbed_langLoc.split("-")[0];
            var grabbed_loc = grabbed_langLoc.split("-")[1];
            lang = grabbed_lang.replace("/", "");
            loc = grabbed_loc.replace("/", "");
            window.pmc.checkMetaTags("ms.lang", lang);
            window.pmc.checkMetaTags("ms.loc", loc);
        } else {
            //due to not having a consistent presence of lang/loc in the url, we user navigation.language
            var userLang = navigator.language || navigator.userLanguage;
            lang = userLang.split("-")[0];
            loc = userLang.split("-")[1];
            window.pmc.checkMetaTags("ms.lang", lang);
            window.pmc.checkMetaTags("ms.loc", loc);
        }
        //get html title element to eventually pass into ms.title
        window.pmc.pageTitle = window.pmc.formatString(jQuery(document).find("title").text());
        //truncate substring of the title
        window.pmc.pageTitle = window.pmc.pageTitle.substring(0, 100);
        if (pageURL.match(/technet\.microsoft\.com(\/..-..)?\/library/i)) {  //library (TECHNET LIBRARY ONLY)
            window.pmc.checkMetaTags("ms.sitesec", "library");
            window.pmc.msTitle = "library;" + window.pmc.pageTitle;
            //webtrends mapping to WEDCS; windows specific content in Library
            window.pmc.webtrendsCheck("DCSext.WCarea", "ms.sitesec");
            window.pmc.webtrendsLangLocCheck("ms.lang", "ms.loc");
            window.pmc.webtrendsCheck("DCSext.Wclifecycle", "ms.mktglfcyl");
            window.pmc.webtrendsCheck("DCSext.WCversion", "ms.prod");
            window.pmc.webtrendsCheck("DCSext.WCzone", "ms.pagetype");
            window.pmc.libraryTaggingEnabled = true;
        } else if (pageURL.match(/social\.technet\.microsoft\.com\/wiki/i)) { //wiki
            window.pmc.checkMetaTags("ms.sitesec", "wiki");
            window.pmc.msTitle = "wiki;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com(\/..-..)?\/bb291022/i) || pageURL.match(/technet\.microsoft\.com(\/..-..)?\/dd644554/i) || pageURL.match(/technet\.microsoft\.com(\/..-..)?\/cc138021/i) || pageURL.match(/technet\.microsoft\.com(\/..-..)?\/ff871920/i)) { //Learn has all shortened links
            window.pmc.checkMetaTags("ms.sitesec", "learn");
            window.pmc.msTitle = "learn;" + window.pmc.pageTitle;
        } else if (pageURL.match(/gallery\.technet\.microsoft/i)) { //gallery
            window.pmc.checkMetaTags("ms.sitesec", "gallery");
            window.pmc.msTitle = "gallery;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com(\/..-..)?\/bb403698/i)) {  //downloads
            window.pmc.checkMetaTags("ms.sitesec", "downloads");
            window.pmc.msTitle = "downloads;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com(\/..-..)?\/ms772425/i)) {  //support
            window.pmc.checkMetaTags("ms.sitesec", "support");
            window.pmc.msTitle = "support;" + window.pmc.pageTitle;
        } else if (pageURL.match(/social\.technet\.microsoft.com\/forums/i)) {  //forums
            window.pmc.checkMetaTags("ms.sitesec", "forums");
            window.pmc.msTitle = "forums;" + window.pmc.pageTitle;
        } else if (pageURL.match(/blogs\.technet\.com/i) || window.location.hostname.match(/^techblogsrc\./i)) { //blogs
            window.pmc.checkMetaTags("ms.sitesec", "blogs");
            window.pmc.msTitle = "blogs;" + window.pmc.pageTitle;
        } else if (pageURL.match(/social\.technet\.microsoft\.com\/search\//i)) {  //search
            window.pmc.checkMetaTags("ms.sitesec", "search");
            window.pmc.msTitle = "search;" + window.pmc.pageTitle;
            //apply search query on load using ms.searchquery
            var queryString = window.location.search;
            if (queryString.match(/query=/i)) {
                var titleToSearch = window.pmc.msTitle.split(" - microsoft technet search")[0];
                var removedSec = titleToSearch.split(";")[1];
                window.pmc.checkMetaTags("ms.searchquery", removedSec);
            }
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/windows/i)) {  //windows
            window.pmc.msTitle = "windows;" + window.pmc.pageTitle;
            //webtrends variable mapping process
            window.pmc.webtrendsCheck("DCSext.WCarea", "ms.sitesec", "windows");
            window.pmc.webtrendsLangLocCheck("ms.lang", "ms.loc");
            window.pmc.webtrendsCheck("DCSext.Wclifecycle", "ms.mktglfcyl");
            window.pmc.webtrendsCheck("DCSext.WCversion", "ms.prod");
            window.pmc.webtrendsCheck("DCSext.WCzone", "ms.pagetype");
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/ie/i)) { //internet explorer
            window.pmc.checkMetaTags("ms.sitesec", "internet explorer");
            window.pmc.msTitle = "internet explorer;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/sqlserver/i)) {  //sql server
            window.pmc.checkMetaTags("ms.sitesec", "sql server");
            window.pmc.msTitle = "sql server;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/scriptcenter\//i)) {  //script center
            window.pmc.checkMetaTags("ms.sitesec", "script center");
            window.pmc.msTitle = "script center;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office/i)) { //office
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "office;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788774/i)) { //office 365
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "office 365;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788775/i)) { //exchange server
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "exchange server;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788776/i)) { //sharepoint products
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "sharepoint products;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788773/i)) { //lync
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "lync;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788954/i)) { //lync downloads and updates
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "lync;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788774/i)) { //office 365 for IT Pros
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "office 365 for it pros;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/fp160948/i)) { //office for IT pros
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "office for it pros;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788776/i)) { //sharepoint for IT pros
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "sharepoint for it pros;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788775/i)) { //exchange for IT pros
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "exchange for it pros;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788773/i)) { //Lync server for IT pros
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "lync server for it pros;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn788778/i)) { //Project server for IT pros
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "project server for it pros;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/..-..\/office\/dn770220/i)) { //support for Office products
            window.pmc.checkMetaTags("ms.sitesec", "office");
            window.pmc.msTitle = "support for office products;" + window.pmc.pageTitle;
        } else if (pageURL.match(/technet\.microsoft\.com\/((..-..)?\/?(((default)(\.aspx)?)?|(ms\d+(\.aspx)?)?))$/i) || pageURL.match(/technet\.microsoft\.com\/..-..\/.*$/i)) { //home page
            window.pmc.checkMetaTags("ms.sitesec", "home");
            window.pmc.msTitle = "home;" + window.pmc.pageTitle;
        }
        //universal checkMetaTags for ms.title
        window.pmc.checkMetaTags("ms.title", window.pmc.msTitle);
    }
};
window.pmc.setAttribute = function (inJQObj, inAttrName, inAttrValue) {
    if (typeof inJQObj == "undefined" || inJQObj.length === 0 || !inAttrName || inAttrValue === "") {
        return;
    }
    inJQObj.attr(inAttrName, inAttrValue);
};
window.pmc.wedcsUCTracking = [];
window.pmc.currUC = "";
window.pmc.setWEDCSAttributes = function (inJQObj, inCmpgrp, inCmpnm, inIndex) {
    var newUCTLen = window.pmc.wedcsUCTracking.push(window.pmc.currUC);
    window.pmc.setAttribute(inJQObj, "data-pmcucidx", (newUCTLen - 1));
    window.pmc.setAttribute(inJQObj, "ms.cmpgrp", inCmpgrp);
    window.pmc.setAttribute(inJQObj, "ms.cmpnm", inCmpnm);
    window.pmc.setAttribute(inJQObj, "ms.index", inIndex);

};
window.pmc.processWEDCSCustomEventFromArray = function (inArray) {
    if (typeof window.MscomCustomEvent != "function") {
        return;
    }
    if (typeof inArray == "undefined" || inArray === null || inArray.length === 0) {
        window.MscomCustomEvent();
        return;
    }
    window.MscomCustomEvent.apply(this, inArray);
};
window.pmc.processWEDCSCustomEventFromJQObj = function (inJQObj) {
    if (typeof inJQObj == "undefined" || inJQObj === null || inJQObj.length === 0) {
        window.pmc.processWEDCSCustomEventFromArray();
        return;
    }
    var myWEDCDAttrArray = [];
    jQuery(inJQObj[0].attributes).each(function () {
        if (this.nodeName && this.nodeName.indexOf("ms.") === 0) {
            myWEDCDAttrArray.push(this.nodeName, this.nodeValue);
        }
    });
    window.pmc.processWEDCSCustomEventFromArray(myWEDCDAttrArray);
};
window.pmc.cleanSearchQuery = function (searchVal) {
    var cleanedQuery;
    cleanedQuery = searchVal.replace(/(<([^>]+)>)/ig, "");
    cleanedQuery = searchVal.replace(/"/g, "'");
    cleanedQuery = searchVal.replace(/&/g, "+");
    return cleanedQuery;
};
window.pmc.tagForSearch = function () {
    //apply common non-site section specific search values to only technet hosted pages
    if (window.location.hostname.match(/technet\.microsoft\.com/i) ||
      window.location.hostname.match(/technet\.com/i) ||
      window.location.hostname.match(/^technetappsmain\./i) || //search and forums pre-prod
      window.location.hostname.match(/^tngmain\./i) || //galleries pre-prod
      window.location.hostname.match(/^techblogsrc\./i)) { //blogs pre-prod
        //Tag search boxes across all of technet
        var cleanedQuery;
        //bing search at top of page
        window.pmc.currUC = "Bing Searchbox; top of page";
        jQuery("button#HeaderSearchButton").mousedown(function () {
            try {
                var searchVal = window.pmc.formatString(jQuery("#HeaderSearchTextBox").val());
                cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
            } catch (e) { }
        });
        jQuery("input#HeaderSearchButton").mousedown(function () {
            try {
                var searchVal = window.pmc.formatString(jQuery("#HeaderSearchTextBox").val());
                cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
            } catch (e) { }
        });
        //case for wiki
        window.pmc.currUC = "Bing Searchbox; top of page on wiki sections";
        jQuery("#Header_SearchButton").mousedown(function () {
            try {
                var searchVal = window.pmc.formatString(jQuery("#Header_SearchTextBox").val());
                cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
            } catch (e) { }
        });
        //case for wiki
        jQuery("input#Header_SearchTextBox").keydown(function (event) {
            if (event.which == 13) {
                try {
                    var searchVal = window.pmc.formatString(jQuery("#Header_SearchTextBox").val());
                    cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                    window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                } catch (e) { }
            }
        });
        window.pmc.currUC = "Bing small Searchbox; top center of page";
        jQuery("#SearchButton").mousedown(function () {
            try {
                var searchVal = window.pmc.formatString(jQuery("#SearchTextBox").val());
                cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
            } catch (e) { }
        });
        //When user presses the enter key to submit search query
        jQuery("#HeaderSearchTextBox").keydown(function (event) {
            if (event.which == 13) {
                try {
                    var searchVal = window.pmc.formatString(jQuery("#HeaderSearchTextBox").val());
                    cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                    window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                } catch (e) { }
            }
        });
        //case for blogs parent page
        jQuery("#HeaderSearchTextBox").off("mousedown").on("mousedown", function () {
            jQuery("#SuggestionContainer ul").off("mousedown").on("mousedown", function () {
                try {
                    var term = window.pmc.formatString(jQuery("input[title*='Search']").val());
                    cleanedQuery = window.pmc.cleanSearchQuery(term);
                    window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                } catch (e) { }
            });
        });
        //search box in search section & blogs sub pages
        jQuery("#SuggestionContainer ul").off("mousedown").on("mousedown", function () {
            try {
                var term = "";
                if (window.location.hostname.match(/blogs\.technet\.com/i) || window.location.hostname.match(/^techblogsrc\./i)) {
                    term = window.pmc.formatString(jQuery("input[title*='Search']:visible").val());
                } else {
                    term = window.pmc.formatString(jQuery("input[title*='Search']").val());
                }
                cleanedQuery = window.pmc.cleanSearchQuery(term);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                if (window.location.pathname.match(/\/search\//i)) {
                    window.pmc.setMetaTagContent("ms.searchquery", cleanedQuery);
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                } else {
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            } catch (e) { }
        });
        //searchbox and button on search page
        jQuery("#SearchTextBox").on("input", function () {
            try {
                var searchText = jQuery(this).val();
                cleanedQuery = window.pmc.cleanSearchQuery(searchText);
                jQuery(this).attr("ms.searchquery", cleanedQuery);
                jQuery("a#SearchSubmitImage").attr("ms.searchquery", cleanedQuery);
            } catch (e) { }
        });
        jQuery("#SearchSubmitImage").on("mousedown", function () {
            try {
                var searchVal = window.pmc.formatString(jQuery("#SearchTextBox").val());
                cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                if (window.location.pathname.match(/\/search\//i)) {
                    window.pmc.setMetaTagContent("ms.searchquery", cleanedQuery);
                }
            } catch (e) { }
        });
        //button on blogs
        jQuery("input.search-button[title*='Search TechNet with Bing']").mousedown(function () {
            try {
                var term = "";
                if (window.location.hostname.match(/blogs\.technet\.com/i) || window.location.hostname.match(/^techblogsrc\./i)) {
                    term = window.pmc.formatString(jQuery("input[title*='Search']:visible").val());
                } else {
                    term = window.pmc.formatString(jQuery("input[name*='SearchTextBox']").val());
                }
                cleanedQuery = window.pmc.cleanSearchQuery(term);
                window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                if (window.location.pathname.match(/\/search\//i)) {
                    window.pmc.setMetaTagContent("ms.searchquery", cleanedQuery);
                }
            } catch (e) { }
        });
        //on enter in search box
        jQuery("input[name*='SearchTextBox']").on("keydown", function (event) {
            try {
                //13 equals enter key on keyboard
                if (event.which == 13) {
                    var searchVal = window.pmc.formatString(jQuery(this).val());
                    cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                    window.pmc.setAttribute(jQuery(this), "ms.searchquery", cleanedQuery);
                    if (window.location.pathname.match(/\/search\//i)) {
                        window.pmc.setMetaTagContent("ms.searchquery", cleanedQuery);
                        window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                    } else {
                        window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                    }
                }
            } catch (e) { }
        });
    }
};
window.pmc.tagLeftNav = function () {
    //apply library section specific custom values to only technet hosted pages
    if (window.location.hostname.match(/technet\.microsoft\.com/i) || window.location.hostname.match(/technet\.com/i) || window.location.hostname.match(/^technetappsmain\./i) || window.location.hostname.match(/^tngmain\./i)) {
        var tCmpGrp = "";
        var tCmpNm = "";
        //left nav panel
        window.pmc.navIndex = "0";
        var currentLevel, prevLevel, prevCmpNmLevel, splitCmpNmLevel;
        window.pmc.currUC = "Left nav dropdown items";
        jQuery('#leftNav div[data-toclevel] a:not([class*="toc_"]):not([data-pmcucidx])').each(function () {
            var parentDropText = window.pmc.formatString(jQuery(this).closest('div[data-toclevel]').prevAll('.toclevel1:first').find("a:not([href='#'])").text());
            var levelNum = parseInt(jQuery(this).closest('div[data-toclevel]').attr("data-toclevel"), 10);
            //special cases for parent dropdowns
            if (levelNum === 0 || levelNum === 1) {
                parentDropText = window.pmc.formatString(jQuery(this).text());
            } else if (levelNum === -1) {
                levelNum = 0;
                parentDropText = window.pmc.formatString(jQuery(this).text());
            }
            tCmpNm = parentDropText + ';' + levelNum;
            currentLevel = jQuery(this).closest("div").attr("data-toclevel");
            prevLevel = jQuery(this).closest("div").prev().attr("data-toclevel");
            window.pmc.setWEDCSAttributes(jQuery(this), "table of contents", tCmpNm);
            try {
                if (currentLevel !== prevLevel) {
                    //new dropdown list item
                    window.pmc.navIndex = "0";
                    window.pmc.setAttribute(jQuery(this), "ms.index", window.pmc.navIndex);
                } else {
                    window.pmc.navIndex++;
                    window.pmc.setAttribute(jQuery(this), "ms.index", window.pmc.navIndex);
                }
            } catch (e) { }
        });
    }
};
window.pmc.tagElements = function () {
    //Temporary variables
    var tCmpGrp = "";
    var tCmpNm = "";
    var sideNavCounter = 0;
    var cmpnmCheck = "";
    window.pmc.tagLeftNav();
    //In this library; library home page
    window.pmc.currUC = "body directory links";
    jQuery("#content .navpage ul li a").each(function (index) {
        tCmpNm = jQuery(this).closest("ul").prev().text();
        if (tCmpNm === "\xA0") {
            tCmpNm = jQuery(this).closest("ul").prev().parent().prev().children(":header").text();
        }
        tCmpNm = window.pmc.formatString(tCmpNm);
        tCmpGrp = window.pmc.formatString(jQuery(this).closest(".navpage").find(":header:first").text());
        window.pmc.setWEDCSAttributes(jQuery(this), tCmpGrp, tCmpNm, index);
    });
    //See also links
    window.pmc.currUC = "alternative see also directory links";
    jQuery(".navpage div[style*='float'] a:not([data-pmcucidx])").each(function (index) {  //careful! style may change over time
        tCmpNm = window.pmc.formatString(jQuery(this).closest("ul").prev().text());
        if (tCmpNm === "") {
            tCmpNm = window.pmc.formatString(jQuery(this).closest(".navpage").find("div[style*='clear:both']:first").next().text());
            tCmpNm = tCmpNm.replace(":", "");
        }
        tCmpGrp = window.pmc.formatString(jQuery(this).closest(".navpage").find(":header:first").text());
        window.pmc.setWEDCSAttributes(jQuery(this), tCmpGrp, tCmpNm, index);
    });
    window.pmc.verbatim = "";
    //update the verbatim variable to ensure transfer of data when the submit button is clicked
    jQuery(".feedbackContainer textarea").on("input", function () {
        try {
            window.pmc.verbatim = jQuery(this).val().substring(0, 500).replace(/(<([^>]+)>)/ig, "").replace(/"/g, "'").replace(/&/g, "+");
        } catch (e) { }
    });
    //Submit button in feedback section
    jQuery(".feedbackContainer #feedbackSubmit").on("mousedown", function () {
        try {
            var jqFsButton = jQuery(this);
            //survey
            window.pmc.setAttribute(jqFsButton, "ms.srv_id", "was this page helpful?");
            //survey 
            window.pmc.setAttribute(jqFsButton, "ms.scnum", "feedbackform");
            //set cmpgrp, cmpnm, index
            tCmpNm = window.pmc.formatString(jQuery("#feedbackSection1").children(".left:first").text());
            window.pmc.setWEDCSAttributes(jqFsButton, "user feedback", tCmpNm, "0");
            //radio button feedback
            window.pmc.radioChoice = (jQuery(".feedbackContainer input[type=radio]:checked").attr("id") === "feedbackYes" ? "yes" : "no");
            //get checkbox feedback
            window.pmc.checkText = "";
            var tCBid = "";
            jQuery('.feedbackContainer input[name*="chkbxNo"]:checked').each(function () {
                tCBid = jQuery(this).attr("id");
                if (tCBid === "checkboxNo201") {
                    window.pmc.checkText += "not accurate|";
                } else if (tCBid === "checkboxNo202") {
                    window.pmc.checkText += "not enough depth|";
                } else if (tCBid === "checkboxNo203") {
                    window.pmc.checkText += "need more code examples|";
                } else if (tCBid === "checkboxNo204") {
                    window.pmc.checkText += "the translation needs improvement|";
                }
            });
            //Set ms.srv_resp to combination of radio and checkbox user choices
            var srv_resp = window.pmc.radioChoice;
            if (window.pmc.checkText.length > 0 && window.pmc.radioChoice === "no") {
                //checkbox value(s) were chosen
                //take off trailing pipe character from window.pmc.checkText
                srv_resp += ";" + window.pmc.checkText.substring(0, window.pmc.checkText.length - 1);
            }
            window.pmc.setAttribute(jqFsButton, "ms.srv_resp", srv_resp);
            if (window.pmc.verbatim === "") {
                window.pmc.verbatim = "no comment";
            }
            window.pmc.setAttribute(jqFsButton, "ms.srv_v", window.pmc.verbatim);
            window.pmc.processWEDCSCustomEventFromJQObj(jqFsButton);
        } catch (e) { }
    });
};
window.pmc.callMSJS = function () {
    window.pmc.pageLoad();
    //WEDCS base settings
    window.varClickTracking = 1;
    window.varCustomerTracking = 0;
    window.varAutoFirePV = 1;
    window.route = "";
    window.ctrl = "";
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = window.location.protocol + "//c.microsoft.com/ms.js";
    script.onload = function () {
        if (window.location.hostname.match(/msdn\.microsoft\.com/i) || window.location.hostname.match(/msdnapps/i)) {  //msdn search only
            window.pmc.setupScrollTracking();
            if (window.location.pathname.match(/\/search/i)) {    //matches search page
                window.pmc.runRefinementCode();
            } else {  //not on the search page, ok to run readLanguageSwitcher function
                window.pmc.readLanguageSwitcher();
            }
        }
    };
    document.getElementsByTagName("head")[0].appendChild(script);
    // WEDCS vNext
    if (typeof AmbientContext !== "undefined" && AmbientContext["insight.use_wedcs_vnext"] === true) {
        script = document.createElement("script");
        script.type = "text/javascript";
        script.src = "//az725175.vo.msecnd.net/scripts/jsll-3.2.0.js";
        script.onload = function () {
            window.Asimov.clickstreamTracker.init();
        };
        document.getElementsByTagName("head")[0].appendChild(script);
    }
    //tag search textboxes and buttons
    window.pmc.tagForSearch();
    //tag for click events
    if (window.pmc.libraryTaggingEnabled) {
        window.pmc.tagElements();
    }
    //AJAX Listener
    jQuery(document).ajaxComplete(function () {
        if (window.pmc.libraryTaggingEnabled) {
            window.pmc.tagLeftNav();
        }
    });
};
//SCROLL TRACKING
//Scroll Area definition
window.pmc.scrollArea = function (inName) {
    this.name = inName;
    this.scrollNA = false;
    this.scrollQuarter = false;
    this.scrollHalf = false;
    this.scrollThreeQuarter = false;
    this.scrollBot = false;
};
//Track multiple scroll areas per page
window.pmc.currScrollArea = null;
window.pmc.scrollAreaList = [];
window.pmc.setCurrScrollArea = function (inIndex) {
    if (typeof inIndex != "number") {
        return;
    } else if (window.pmc.scrollAreaList.length <= inIndex) {
        return;
    }
    window.pmc.currScrollArea = window.pmc.scrollAreaList[inIndex];
};
//Page dimension variables
window.pmc.scrollBottomElement = null;
window.pmc.scrollPageHeight = -1;
window.pmc.viewportBottom = -1;
window.pmc.refreshScrollPageDimVars = function () {
    var tBottom = 0;
    if (window.pmc.scrollBottomElement) {
        tBottom = window.pmc.scrollBottomElement.getBoundingClientRect().top + window.pageYOffset;
    }
    if (tBottom <= 0) {
        tBottom = document.body.getBoundingClientRect().height;
    }
    window.pmc.scrollPageHeight = tBottom;
    window.pmc.viewportBottom = window.pageYOffset + window.innerHeight;
};
window.pmc.suppressMobileEvents = false;
window.pmc.fireScrollEvent = function (inScrollValue) {
    window.pmc.debugLog("stepped inside fireScrollEvent function");
    var tArray = [];
    tArray.push("ms.pgarea", "body");
    tArray.push("ms.scnum", "scroll-" + inScrollValue);
    tArray.push("ms.interactiontype", "4");
    tArray.push("ms.scn", "scroll");
    window.pmc.processWEDCSCustomEventFromArray(tArray);
};
window.pmc.checkForNAScroll = function () {
    window.pmc.refreshScrollPageDimVars();
    if (window.pmc.scrollPageHeight <= 0) {
        return;
    } else if (window.pmc.suppressMobileEvents && window.innerWidth <= 510) {
        return;
    }
    if (window.innerHeight > window.pmc.scrollPageHeight * 0.90) {
        //Visitor can see the whole page, fire special -na event
        window.pmc.debugLog("calling n/a scroll event");
        window.pmc.currScrollArea.scrollNA = true;
        window.pmc.fireScrollEvent("na");
    } else {
        window.pmc.debugLog("calling 0 percent scroll");
        window.pmc.fireScrollEvent("0%");
    }
};
window.pmc.processScroll = function () {
    window.clearTimeout(window.pmc.scrollResizeTimer);
    if (window.pmc.currScrollArea === null) {
        return;
    }
    window.pmc.refreshScrollPageDimVars();
    if (window.pmc.scrollPageHeight <= 0) {
        return;
    } else if (window.pmc.suppressMobileEvents && window.innerWidth <= 510) {
        return;
    }
    if (window.innerHeight > window.pmc.scrollPageHeight * 0.90) {
        //Visitor can see the whole page, fire special -na event
        if (!window.pmc.currScrollArea.scrollNA) {
            window.pmc.currScrollArea.scrollNA = true;
            window.pmc.fireScrollEvent("na");
        }
        return;
    }
    var tCurrPercent = window.pmc.viewportBottom / window.pmc.scrollPageHeight;
    if (tCurrPercent > 0.25 && !window.pmc.currScrollArea.scrollQuarter) {
        window.pmc.currScrollArea.scrollQuarter = true;
        window.pmc.fireScrollEvent("25%");
    }
    if (tCurrPercent > 0.50 && !window.pmc.currScrollArea.scrollHalf) {
        window.pmc.currScrollArea.scrollHalf = true;
        window.pmc.fireScrollEvent("50%");
    }
    if (tCurrPercent > 0.75 && !window.pmc.currScrollArea.scrollThreeQuarter) {
        window.pmc.currScrollArea.scrollThreeQuarter = true;
        window.pmc.fireScrollEvent("75%");
    }
    if (tCurrPercent > 0.99 && !window.pmc.currScrollArea.scrollBot) {
        window.pmc.currScrollArea.scrollBot = true;
        window.pmc.fireScrollEvent("100%");
    }
};
window.pmc.scrollResizeTimer = 0;
window.pmc.setupScrollTracking = function () {
    window.pmc.scrollBottomElement = jQuery("footer:first")[0];
    window.pmc.scrollAreaList.push(new window.pmc.scrollArea("body"));
    window.pmc.setCurrScrollArea(0);
    window.addEventListener("scroll", function () {
        window.pmc.processScroll();
    });
    //Note: resize also catches zoom in/out
    jQuery(window).resize(function () {
        window.clearTimeout(window.pmc.scrollResizeTimer);
        window.pmc.scrollResizeTimer = window.setTimeout(function () {
            window.pmc.processScroll();
        }, 500);
    });
    //fire 0% scroll or n/a scroll depending on window height
    window.pmc.checkForNAScroll();
};
///////////////////////////
//Search Refinements Code//
///////////////////////////
window.pmc.findRefinements = function () {
    window.pmc.refinementTextArray = [];
    window.pmc.urlHashString = window.location.hash;
    window.pmc.urlQueryString = window.location.search;
    var searchQuery, searchHash, refinements;
    if (window.pmc.urlQueryString) {
        try {
            var rIndex = window.pmc.urlQueryString.indexOf("refinement=");
            if (rIndex > -1) {
                var choppedQueryString = window.pmc.urlQueryString.substr(rIndex, window.pmc.urlQueryString.length);
                refinements = choppedQueryString.substr(0, choppedQueryString.indexOf("&"));
                window.pmc.debugLog("search hash before '=' split " + refinements);
                searchQuery = refinements.split("=")[1];
                window.pmc.debugLog("search hash after '=' split " + searchQuery);
            }
            //search in query string for preexisting refinements. "refinement="
            if (searchQuery && searchQuery.length > 0) {
                if (searchQuery.match(/%2C/i)) {
                    window.pmc.debugLog("multiple refinements in refinement query string");
                    searchQuery = searchQuery.split("%2C");
                    for (i = 0; i < searchQuery.length; i++) {
                        try {
                            if (searchQuery[i].match("-")) {
                                //do nothing;in some cases a negative number indicates a refinement was deselected
                            } else {
                                window.pmc.refinementTextArray.push(searchQuery[i]);
                            }
                        } catch (e) {
                            window.pmc.debugLog("error in refinement= -> multiple refinement logic. Message: " + e);
                        }
                    }
                } else {
                    window.pmc.debugLog("one refinement");
                    //only one refinement
                    if (searchQuery.length > 0) {
                        //There is a refinement active. Reason for this extra check is that a user can deselect a refinement and
                        //the "refinementChanges" hash parameter will still be in the url address
                        try {
                            if (searchQuery.match("-")) {
                                //do nothing;in some cases a negative number indicates a refinement was deselected
                            } else {
                                window.pmc.refinementTextArray.push(searchQuery);
                            }
                        } catch (e) {
                            window.pmc.debugLog("error in refinement= -> one refinement logic. Message: " + e);
                        }
                    }
                }
                window.pmc.setMetaTagContent("ms.search_productfilter", window.pmc.refinementTextArray.toString());
            }
        } catch (e) {
            window.pmc.debugLog("Error in query string parsing logic. Error: " + e);
        }
    }
    if (window.pmc.urlHashString) {
        try {
            var rcIndex = window.pmc.urlHashString.indexOf("refinementChanges=");
            if (rcIndex > -1) {
                var choppedHashString = window.pmc.urlHashString.substr(rcIndex, window.pmc.urlHashString.length);
                refinements = choppedHashString.substr(0, choppedHashString.indexOf("&"));
                window.pmc.debugLog("search hash before '=' split " + refinements);
                searchHash = refinements.split("=")[1];
                window.pmc.debugLog("search hash after '=' split " + searchHash);
            }
            //search in hash string for refinements. "refinementChanges="
            if (searchHash && searchHash.length > 0) {
                //There is a refinement active. Reason for this extra check is that a user can deselect a refinement and
                //the "refinementChanges" hash parameter will still be in the url address
                if (searchHash.match(/,/i)) {
                    window.pmc.debugLog("multiple refinements in refinementChanges= hash string");
                    searchHash = searchHash.split(",");
                    for (i = 0; i < searchHash.length; i++) {
                        try {
                            if (searchHash[i].match("-")) {
                                //check against refinements in query string to see if you need to exclude items in refinement array
                                if (String(searchQuery).match(searchHash[i].split("-")[1])) {
                                    window.pmc.debugLog("refinement change matches item in refinement query string");
                                    //remove refinement id from refinementTextArray
                                    window.pmc.refinementTextArray = window.pmc.refinementTextArray.filter(function (val) {
                                        return val != searchHash[i].split("-")[1];
                                    });
                                }
                            } else {
                                window.pmc.refinementTextArray.push(searchHash[i]);
                            }
                        } catch (e) {
                            window.pmc.debugLog("Error in refinementChanges= -> multiple refinement logic. Message: " + e);
                        }
                    }
                } else {
                    window.pmc.debugLog("one refinement");
                    //only one refinement
                    if (searchHash.length > 0) {
                        //There is a refinement active. Reason for this extra check is that a user can deselect a refinement and
                        //the "refinementChanges" hash parameter will still be in the url address
                        try {
                            if (searchHash.match("-")) {
                                var splitHash = searchHash.split("-")[1];
                                //check against refinements in query string to see if you need to exclude items in refinement array
                                if (String(searchQuery).match(splitHash)) {
                                    window.pmc.debugLog("refinement change matches item in refinement query string");
                                    //remove refinement id from refinementTextArray
                                    window.pmc.refinementTextArray = window.pmc.refinementTextArray.filter(function (val) {
                                        return val != splitHash;
                                    });
                                }
                            } else {
                                window.pmc.refinementTextArray.push(searchHash);
                            }
                        } catch (e) {
                            window.pmc.debugLog("Error in refinementChanges= -> one refinement logic. Message: " + e);
                        }
                    }
                }
                window.pmc.setMetaTagContent("ms.search_productfilter", window.pmc.refinementTextArray.toString());
            }
        } catch (e) {
            window.pmc.debugLog("Error found in hash string parsing logic. Error: " + e);
        }
    }
    if (window.pmc.refinementTextArray.length === 0) {
        window.pmc.setMetaTagContent("ms.search_productfilter", "no refinements");
    }
    window.pmc.debugLog("array of all refinement ids in query params " + searchQuery);
    window.pmc.debugLog("array of all refinement ids in hash params " + searchHash);
    window.pmc.debugLog("array of all refinement id values " + window.pmc.refinementTextArray);
};
window.pmc.applyNodeListener = function () {
    var interaction_type, jqthis, tArray = [], limiter = 0;
    jQuery(".RefinementListContainer").on("click", "li", function () {
        try {
            if (limiter === 0) {
                limiter = 1;
                jqthis = jQuery(this).context;
                window.setTimeout(function () {
                    interaction_type = jQuery(jqthis).find("input:first").attr("checked") ? "16" : "17";
                    //find which refinements are active
                    window.pmc.findRefinements();
                    if (window.pmc.refinementTextArray.length > 0) {
                        tArray.push("ms.cmptyp", "checkbox");
                        tArray.push("ms.interactiontype", interaction_type);
                        tArray.push("ms.title", window.pmc.formatString(jQuery(jqthis).closest("li").text()));
                        window.pmc.setMetaTagContent("ms.search_productfilter", window.pmc.refinementTextArray.toString());
                        window.pmc.processWEDCSCustomEventFromArray(tArray);
                        limiter = 0;
                    } else {
                        window.pmc.debugLog("there are no refinements");
                        tArray.push("ms.cmptyp", "checkbox");
                        tArray.push("ms.interactiontype", interaction_type);
                        tArray.push("ms.title", window.pmc.formatString(jQuery(jqthis).text()));
                        window.pmc.setMetaTagContent("ms.search_productfilter", "no refinements");
                        window.pmc.processWEDCSCustomEventFromArray(tArray);
                        limiter = 0;
                    }
                }, 1000);
            }
        } catch (e) {
            window.pmc.debugLog("error in refinement list custom event building. Message: " + e);
        }
    });
    //results in body
    jQuery("#ContentTableCell").on("mousedown", "a.resultTitleLink", function () {
        jqthis = jQuery(jQuery(this).context);
        try {
            jqthis.attr({
                "ms.cmptyp": "text link",
                "ms.interaction_type": "1",
                "ms.title": window.pmc.formatString(jqthis.text())
            });
        } catch (e) {
            window.pmc.debugLog("error in assigning attributes to body result. Message: " + e);
        }
    });
};
window.pmc.runRefinementCode = function () {
    try {
        window.pmc.urlHashString = window.location.hash;
        window.pmc.urlQueryString = window.location.search;
        window.pmc.refinementText = "";
        //check for pre-existing refinements on the page
        if ((window.pmc.urlHashString && window.pmc.urlHashString.match(/refinementChanges=/i)) || (window.pmc.urlQueryString && window.pmc.urlQueryString.match(/refinement=/i))) {
            window.pmc.findRefinements();
        } else {
            window.pmc.setMetaTagContent("ms.search_productfilter", "no refinements");
        }
        //anytime the refinement list refreshes we want to find the refinements active and add click
        //listeners on the new refinement list
        window.pmc.applyNodeListener();
    } catch (e) {
        window.pmc.debugLog("Error found in runRefinementCode function. Error: " + e);
    }
};
////////////////////////////////////
//End MSDN Search Refinements code//
////////////////////////////////////
//assign url query string "query" parameter -> ms.searchquery metatag
window.pmc.assignSearchQuery = function () {
    var urlQueryString = window.location.search;
    if (urlQueryString && urlQueryString.match(/query=/i)) {
        try {
            var queryIndex = urlQueryString.indexOf("query=");
            //everything after query= in the query string
            var tailQS = urlQueryString.substr(queryIndex, urlQueryString.length);
            //get query name & value
            var searchParam = urlQueryString.substr("query=", tailQS.indexOf("&") + 1);
            searchParam = searchParam.split("=")[1];
            window.pmc.setMetaTagContent("ms.searchquery", searchParam);
        } catch (e) {
            window.pmc.debugLog(e + ": error assigning ms.searchquery");
        }
    }
};
////////////////////////////////
//End MSDN ms.searchquery code//
////////////////////////////////

window.pmc.checkDataDashLoaded = function (numTrys) {
    if (typeof window.epx_loaded != "undefined" && window.epx_loaded) {
        window.setTimeout(function () {  //add buffer
            window.pmc.epxLoaded = true;
            window.pmc.runMSDNCustomSearchTagging();
        }, 500);
    } else {
        if (numTrys <= 10) {
            window.setTimeout(function () {
                numTrys++;
                window.pmc.checkDataDashLoaded(numTrys);
            }, 500);
        } else {
            window.pmc.debugLog("checking for epx_loaded=true timed out inside of checkDataDashLoaded function");
        }
    }
};

window.pmc.runMSDNCustomSearchTagging = function () {
    ////////////////////////
    //Search Buttons/Icons//
    ////////////////////////
    //header search button
    window.pmc.headerSearchButton = jQuery("[data-searchtype='icon'][data-pgarea='header']");
    window.pmc.headerSearchButton.attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "header",
        "ms.searchtype": "icon"
    });
    window.pmc.headerSearchButton.on("mousedown", function () {
        try {
            var searchVal = jQuery("[data-searchtype='searchbox'][data-pgarea='header']").val();
            if (searchVal && searchVal.length > 0) {
                var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                if (!jQuery(this).is(jQuery("a,img,input,area"))) {
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            }
        } catch (e) {
            window.pmc.debugLog("Error in header search button code. Error: " + e);
        }
    });
    window.pmc.headerSearchButton.prev().attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "header",
        "ms.searchtype": "icon"
    });
    window.pmc.headerSearchButton.prev().on("mousedown", function () {
        try {
            var searchVal = jQuery("[data-searchtype='searchbox'][data-pgarea='header']").val();
            if (searchVal && searchVal.length > 0) {
                var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                if (!jQuery(this).is(jQuery("a,img,input,area"))) {
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            }
        } catch (e) {
            window.pmc.debugLog("Error in header search button code. Error: " + e);
        }
    });
    //body search button
    window.pmc.bodySearchButton = jQuery("[data-searchtype='icon'][data-pgarea='body']");
    window.pmc.bodySearchButton.attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "body",
        "ms.searchtype": "icon"
    });
    window.pmc.bodySearchButton.on("mousedown", function () {
        try {
            var searchVal = jQuery("[data-searchtype='searchbox'][data-pgarea='body']").val();
            if (searchVal && searchVal.length > 0) {
                var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                if (!jQuery(this).is(jQuery("a,img,input,area"))) {
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            }
        } catch (e) {
            window.pmc.debugLog("Error in body search button code. Error: " + e);
        }
    });
    ////////////////////////////
    //End Search Buttons/Icons//
    ////////////////////////////
    ///////////////////////////////////////
    //Searchbox Event Tracking with Enter//
    ///////////////////////////////////////
    //header searchbox
    window.pmc.headerSearchbox = jQuery("[data-searchtype='searchbox'][data-pgarea='header']");
    window.pmc.headerSearchbox.attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "header",
        "ms.searchtype": "enter-key"
    });
    window.pmc.headerSearchbox.keydown(function (event) {
        if (event.which == 13) {
            try {
                var searchVal = jQuery(this).val();
                if (searchVal && searchVal.length > 0) {
                    var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                    window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            } catch (e) {
                window.pmc.debugLog("Error in header searchbox enter event code. Error: " + e);
            }
        }
    });
    //body searchbox
    window.pmc.bodySearchbox = jQuery("[data-searchtype='searchbox'][data-pgarea='body']");
    window.pmc.bodySearchbox.attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "body",
        "ms.searchtype": "enter-key"
    });
    window.pmc.bodySearchbox.keydown(function (event) {
        if (event.which == 13) {
            try {
                var searchVal = jQuery(this).val();
                if (searchVal && searchVal.length > 0) {
                    var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                    window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            } catch (e) {
                window.pmc.debugLog("Error in body searchbox enter event code. Error: " + e);
            }
        }
    });
    ///////////////////////////////////////////
    //End Searchbox Event Tracking with Enter//
    ///////////////////////////////////////////
    ///////////////////
    //Search dropdown//
    ///////////////////
    //header search dropdown
    window.pmc.headerSearchDropdown = jQuery("[data-searchtype='search dropdown'][data-pgarea='header']");
    window.pmc.headerSearchDropdown.attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "header",
        "ms.searchtype": "suggested keyword"
    });
    window.pmc.headerSearchDropdown.on("mousedown", function () {
        try {
            var searchVal = window.pmc.headerSearchbox.val();
            if (searchVal && searchVal.length > 0) {
                var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                if (!jQuery(this).is(jQuery("a,img,input,area"))) {
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            }
        } catch (e) {
            window.pmc.debugLog("Error in header search dropdown code. Error: " + e);
        }
    });
    //body search dropdown
    window.pmc.bodySearchDropdown = jQuery("[data-searchtype='search dropdown'][data-pgarea='body']");
    window.pmc.bodySearchDropdown.attr({
        "ms.interactiontype": "2",
        "ms.title": "search",
        "ms.pgarea": "body",
        "ms.searchtype": "suggested keyword"
    });
    window.pmc.bodySearchDropdown.on("mousedown", function () {
        try {
            var searchVal = window.pmc.bodySearchbox.val();
            if (searchVal && searchVal.length > 0) {
                var cleanedQuery = window.pmc.cleanSearchQuery(searchVal);
                window.pmc.setMetaTagContent("ms.searchquery", window.pmc.formatString(cleanedQuery));
                if (!jQuery(this).is(jQuery("a,img,input,area"))) {
                    window.pmc.processWEDCSCustomEventFromJQObj(jQuery(this));
                }
            }
        } catch (e) {
            window.pmc.debugLog("Error in header search dropdown code. Error: " + e);
        }
    });
    ///////////////////////
    //End Search dropdown//
    ///////////////////////
};
///////////////////////////////
//End MSDN custom search code//
///////////////////////////////
///////////////////////////////
//MSDN language switcher code//
///////////////////////////////

window.pmc.getCookie = function (name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return unescape(c.substring(nameEQ.length, c.length));
    }
    return null;
};
window.pmc.scrapePageForRadioChecked = function () {
    var radioChecked = jQuery(".TranslationViewSwitcher input:checked").attr("value");
    if (radioChecked === "0" || radioChecked === "1") {
        var langVal = radioChecked === "1" ? "original" : "translated";
        window.MscomCustomEvent("ms.title", langVal, "ms.cmptyp", "language switcher");
    } else {
        window.pmc.debugLog("Language translator switcher value attribute != 0|1");
    }
};
window.pmc.readLanguageSwitcher = function () {
    try {
        var languageSwitcherVal = window.pmc.getCookie("MtpsLibraryTranslator");
        //find initial language option and fire custom event
        if (!window.location.pathname.match(/\/library\//i)) {    //if the page is not under library/, default to scraping the page
            window.pmc.scrapePageForRadioChecked();
        } else if (languageSwitcherVal !== null) { //user is on a library page && the languageSwitcherVal variable is not null
            if (languageSwitcherVal === "1") {  //original
                window.MscomCustomEvent("ms.title", "original", "ms.cmptyp", "language switcher");
            } else {  //translated content
                window.MscomCustomEvent("ms.title", "translated", "ms.cmptyp", "language switcher");
            }
        } else {  //backup when cookie is not present and the user was not on a library page
            window.pmc.scrapePageForRadioChecked();
        }
    } catch (e) {
        window.pmc.debugLog("error in readLanguageSwitcher function. Native error: " + e);
    }
    jQuery(".TranslationViewSwitcher input").each(function () {
        if (jQuery(this).attr("value")) {
            if (jQuery(this).attr("value") === "0" || jQuery(this).attr("value") === "1") {
                langVal = jQuery(this).attr("value") === "0" ? "translated" : "original";
                jQuery(this).attr("ms.title", langVal);
            } else {
                window.pmc.debugLog("Language translator switcher value attribute != 0|1");
            }
        } else {
            window.pmc.debugLog("Language translator switcher does not have a value attribute");
        }
        jQuery(this).attr({
            "ms.cmptyp": "language selector",
            "ms.interactiontype": "16"
        });
    });
};
///////////////////////////////////
//End MSDN language switcher code//
///////////////////////////////////


jQuery(document).ready(function () {
    if (window.location.hostname.match(/msdn\.microsoft\.com/i) || window.location.hostname.match(/msdnapps/i)) {  //msdn only
        window.pmc.assignSearchQuery();
    }
    window.pmc.callMSJS();  //need for technet and msdn
    if (window.location.hostname.match(/msdn\.microsoft\.com/i) || window.location.hostname.match(/msdnapps/i)) {  //msdn only
        window.pmc.checkDataDashLoaded(1);
    }
});