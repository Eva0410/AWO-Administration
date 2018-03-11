


<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml" lang='en-US'>
<head id="Head1"><link href="https://i1.code.msdn.s-msft.com/RequestReduceContent/8f7e53a9441e429d980ba2573cbed3aa-3dfe38fda390b175bb2db73fb34b95d1-RequestReducedStyle.css" rel="Stylesheet" type="text/css" /><link type="text/css" rel="stylesheet" xmlns="http://www.w3.org/1999/xhtml" />
<link href="https://i1.code.msdn.s-msft.com/RequestReduceContent/9d6d830ae886f68d9f93b0e14618160b-558a556f0c311a008b19f3e59153997d-RequestReducedStyle.css" rel="Stylesheet" type="text/css" /><link type="text/css" rel="stylesheet" xmlns="http://www.w3.org/1999/xhtml" />

    <meta name="description" content="" />
    <meta name="Search.BRAND" content="Msdn"/>
    <meta name="Search.IROOT" content="msdn"/>

    <link rel="shortcut icon" href="https://i1.code.msdn.s-msft.com/GlobalResources/images/Msdn/favicon.ico" type="image/x-icon" /> <link href="https://i1.code.msdn.s-msft.com/RequestReduceContent/8b3b96f8aab4f4f2c11195f3b2d5af95-6ed3c6d8c94adb8a189dd9c5b19c84af-RequestReducedStyle.css" rel="Stylesheet" type="text/css" /><meta name="CommunityInfo" content=" B=Msdn;A=Samples;L=en-US;" /><meta name="ms.puidhash" content="" />
    <script src="https://ajax.aspnetcdn.com/ajax/jquery/jquery-1.11.0.min.js" type="text/javascript" language="javascript"></script>
    <script src="https://ajax.aspnetcdn.com/ajax/jquery.migrate/jquery-migrate-1.2.1.min.js" type="text/javascript" language="javascript"></script>
    <script src="https://i1.code.msdn.s-msft.com/RequestReduceContent/5b5d0501ebc7a43f520ffa950466adce-74bb8d583b7b303ea8b434638ed453ae-RequestReducedScript.js" type="text/javascript" ></script>    
    <title>Sample Code - MSDN Examples in C#, VB.NET, C++, JavaScript, F#</title> 
        <link href="/site/feeds/searchRss" rel="Alternate" type="application/rss+xml"
        title="Sample Code - MSDN Examples in C#, VB.NET, C++, JavaScript, F#" />
    <link rel="P3Pv1" href="/W3C/p3p.xml" />
    
    <style type="text/css">
        #sideNav
        {
            width: 0px;
        }
    </style>

    <meta name="ContentLanguage" content="L=en-US;"/>


<!--
Third party scripts and code linked to or referenced from this website are licensed to you by the parties that own such code, not by Microsoft.  See ASP.NET Ajax CDN Terms of Use â€“ http://www.asp.net/ajaxlibrary/CDN.ashx.
-->
    <script type="text/javascript" >
        function addAsyncScript(src) {
            var script = document.createElement('script');
            script.type = 'text/javascript';
            script.async = true;
            script.src = src;
            return script;
        };
    </script>
    
        <script type="text/javascript" language="javascript">
            jQuery(window).load(function () {
                window.appInsights = {
                    queue: [],
                    start: function (n) {
                        function r(n, t) {
                            n[t] = function () {
                                var i = arguments;
                                n.queue.push(function () { n[t].apply(n, i); });
                            };
                        }
                        var t, i;
                        this.applicationInsightsId = n;
                        this.accountId = null;
                        this.appUserId = null;
                        r(this, "logEvent");
                        r(this, "logPageView");
                    }
                };
                window.appInsights.start("da4169d5-ce7e-4d99-b383-f5de813a9516");
                window.appInsights.logPageView();
            });
        </script>
    </head>
<body class="Chrome en-us Samples " dir= "ltr">
    <div data-chameleon-template="megablade" ></div>
    

<div data-chameleon-template="epxheader"></div>
<div data-chameleon-template="header">


    
    
    
        
    <input type="hidden" id="isHeaderBleeding" value="true" xmlns="http://www.w3.org/1999/xhtml" />
    <div id="ux-header" dir="ltr" class="ltr msdn" ms.pgarea="header" xmlns="http://www.w3.org/1999/xhtml">
        
            
            
        <div id="ux-banner">
            <div id="msccBanner" dir="ltr" data-site-name="code.msdn.microsoft.com" data-mscc-version="0.3.6" data-nver="aspnet-3.0.0" data-sver="0.1.2" class="cc-banner" role="alert"><div class="cc-container"><svg class="cc-icon cc-v-center" x="0px" y="0px" viewBox="0 0 44 44" height="30px" fill="none" stroke="currentColor"><circle cx="22" cy="22" r="20" stroke-width="2"></circle><line x1="22" x2="22" y1="18" y2="33" stroke-width="3"></line><line x1="22" x2="22" y1="12" y2="15" stroke-width="3"></line></svg> <span class="cc-v-center cc-text">This site uses cookies for analytics, personalized content and ads. By continuing to browse this site, you agree to this use.</span> <a href="https://go.microsoft.com/fwlink/?linkid=845480" aria-label="Learn more about Microsoft's Cookie Policy" id="msccLearnMore" class="cc-link cc-v-center cc-float-right" data-mscc-ic="false">Learn more</a></div></div>
        </div>

      <header>
        <span id="singleCol"></span>
        <span id="doubleCol"></span>
        <span id="isMobile"></span>
        <div>
          <div class="row topRow" role="banner">
            <div class="top">
              <div class="left">
                <a class="msdnLogoImg" href="https://msdn.microsoft.com/en-us" title="MSDN home" ms.cmpgrp="mslogo">
                  <div class="clip70x15">
                    <img alt="Microsoft Logo" id="msft-logo" class="msft-logo" src="https://i-msdn.sec.s-msft.com/Areas/Centers/Themes/StandardDevCenter/Content/HeaderFooterSprite.png?v=636475076592256654" />
                  </div>
                </a>
                <div class="GrayPipeDiv clip1x18">
                  <img alt="Gray Pipe" class="GrayPipe" src="https://i-msdn.sec.s-msft.com/Areas/Centers/Themes/StandardDevCenter/Content/HeaderFooterSprite.png?v=636475076592256654" />
                </div>
                  <a class="DevCenterFullNameNonMegaBlade" href="https://msdn.microsoft.com/en-us">Developer Network</a>
                <a class="DevCenterFullName" href="https://msdn.microsoft.com/en-us">Developer Network</a>
                <a class="DevCenterShortName" href="https://msdn.microsoft.com/en-us">Developer</a>
              </div>
              <div class="right" ms.cmpgrp="aux nav">
<div id="signIn" aria-label="Profile button">
<a class="SignedOutProfileElement createProfileLink" href="" title=""></a>      <span class=":ProfileElement:  profileName"></span>

  <div class="profileImage"></div>


<a class="SignedOutProfileElement createViewProfileLink" href="" title=":CreateViewProfileText:">:CreateViewProfileText:</a>
<a class="scarabLink" href="/Account/Login?ReturnUrl=https%3a%2f%2fcode.msdn.microsoft.com%2fsite%2ferror%2f404%3f404%3bhttps%3a%2f%2fcode.msdn.microsoft.com%3a443%2fAreas%2fEpx%2fThemes%2fWindows%2fContent%2fUhfHeader.js">Sign in</a></div>



                <div class="auxNav">
                  <div>
                    <div data-fragmentName="Subscriptions" id="Fragment_Subscriptions" xmlns="http://www.w3.org/1999/xhtml">
  <a href="https://my.visualstudio.com/?wt.mc_id=o~msft~msdn~nav~subscriber&amp;campaign=o~msft~msdn~nav~subscriber" id="Subscriptions_2153_1" xmlns="http://www.w3.org/1999/xhtml">
    <p xmlns="">Subscriber portal</p>
  </a>
</div>
                    <div data-fragmentName="GetTools" id="Fragment_GetTools" xmlns="http://www.w3.org/1999/xhtml">
  <a href="https://www.visualstudio.com/free-developer-offers/" id="GetTools_2153_3" xmlns="http://www.w3.org/1999/xhtml">Get tools</a>
</div>
                  </div>
                </div>
              </div>
            </div>

          </div>
          <div class="row middleRow">
            <div class="expandTop">
              <div class="left"></div>
              <div class="right"></div>
            </div>
          </div>
        </div>
        <div id="buttomRowWrapper" class="bg_default">
          <div class="row buttomRow bg_default">
            <div class="bottom">
              <div class="left" role="navigation" aria-label="header toc" ms.cmpgrp="main nav">
                <a id="grip" class="menu-icon" href="javascript:void(0)" role="button" aria-label="navigation menu" data-mscc-ic="false"></a>
                
                <div id="drawer">
                  <div class="toc">
                    
        <nav>
            <ul class="navL1">
                        <li class="inactive toggle">
                                <a href="javascript:void(0)" role="button" aria-expanded="false" data-mscc-ic="false">Downloads</a>
                                <ul class="navL2">
                                        <li class="inactive">

                                                <a href="https://www.visualstudio.com/downloads/download-visual-studio-vs" role="link" title="Visual Studio">Visual Studio</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://msdn.microsoft.com/microsoft-sdks-msdn" role="link" title="SDKs">SDKs</a>
                                        </li>
                                        <li class="inactive toggle">

                                                <a href="javascript:void(0)" title="Trial software" role="button" aria-expanded="false" data-mscc-ic="false">Trial software</a>
                                                <ul class="navL3">
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/evalcenter" title="Free downloads" role="link">Free downloads</a>
                                                        </li>
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/officeevaluationresources" title="Office resources" role="link">Office resources</a>
                                                        </li>
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/sharepoint2013resources" title="SharePoint Server 2013 resources" role="link">SharePoint Server 2013 resources</a>
                                                        </li>
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/sqlserver2014expressresources" title="SQL Server 2014 Express resources" role="link">SQL Server 2014 Express resources</a>
                                                        </li>
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/windowsserver2012r2resources" title="Windows Server 2012 resources" role="link">Windows Server 2012 resources</a>
                                                        </li>
                                                </ul>
                                        </li>
                                </ul>
                        </li>
                        <li class="inactive toggle">
                                <a href="javascript:void(0)" role="button" aria-expanded="false" data-mscc-ic="false">Programs</a>
                                <ul class="navL2">
                                        <li class="inactive toggle">

                                                <a href="javascript:void(0)" title="Subscriptions" role="button" aria-expanded="false" data-mscc-ic="false">Subscriptions</a>
                                                <ul class="navL3">
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/msdn-subscriptions-overview" title="Overview" role="link">Overview</a>
                                                        </li>
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/msdn-subscriptions-administration" title="Administrators" role="link">Administrators</a>
                                                        </li>
                                                </ul>
                                        </li>
                                        <li class="inactive toggle">

                                                <a href="javascript:void(0)" title="Students" role="button" aria-expanded="false" data-mscc-ic="false">Students</a>
                                                <ul class="navL3">
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/imagine/imagine-home" title="Microsoft Imagine" role="link">Microsoft Imagine</a>
                                                        </li>
                                                        <li class="inactive">
                                                            <a href="https://msdn.microsoft.com/microsoftstudentpartners" title="Microsoft Student Partners" role="link">Microsoft Student Partners</a>
                                                        </li>
                                                </ul>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://msdn.microsoft.com/applicationbuilder" role="link" title="ISV">ISV</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://www.microsoft.com/bizspark" role="link" title="Startups">Startups</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://events.microsoft.com/" role="link" title="Events">Events</a>
                                        </li>
                                </ul>
                        </li>
                        <li class="inactive toggle">
                                <a href="javascript:void(0)" role="button" aria-expanded="false" data-mscc-ic="false">Community</a>
                                <ul class="navL2">
                                        <li class="inactive">

                                                <a href="https://msdn.microsoft.com/magazine/dd767791" role="link" title="Magazine">Magazine</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://social.msdn.microsoft.com/forums/" role="link" title="Forums">Forums</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://blogs.msdn.microsoft.com/" role="link" title="Blogs">Blogs</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://channel9.msdn.com/" role="link" title="Channel 9">Channel 9</a>
                                        </li>
                                </ul>
                        </li>
                        <li class="inactive current toggle">
                                <a href="javascript:void(0)" role="button" aria-expanded="false" data-mscc-ic="false">Documentation</a>
                                <ul class="navL2">
                                        <li class="inactive">

                                                <a href="https://msdn.microsoft.com/library" role="link" title="APIs and reference">APIs and reference</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://msdn.microsoft.com/developer-centers-msdn" role="link" title="Dev centers">Dev centers</a>
                                        </li>
                                        <li class="inactive current">

                                                <a href="https://code.msdn.microsoft.com/" role="link" title="Samples">Samples</a>
                                        </li>
                                        <li class="inactive">

                                                <a href="https://msdn.microsoft.com/mt703209" role="link" title="Retired content">Retired content</a>
                                        </li>
                                </ul>
                        </li>
            </ul>
        </nav>

                  </div>
                </div>
              </div>
              <div class="right" ms.title="search" role="search">
                <div data-fragmentName="SearchBox" id="Fragment_SearchBox" xmlns="http://www.w3.org/1999/xhtml">
  <div class="SearchBox">
    <form id="HeaderSearchForm" name="HeaderSearchForm" method="get">
      <button id="FakeHeaderSearchButton" value="Search" type="submit" class="header-search-button" role="button" aria-label="search finder">
        <div id="search-finder-div" class="clip16x20">
          <img alt="search finder" id="search-finder" class="search-finder" src="https://i-msdn.sec.s-msft.com/Areas/Centers/Themes/StandardDevCenter/Content/HeaderFooterSprite.png?v=636475076592256654" />
        </div>
      </button>
      <button id="HeaderSearchButton" style="display:none"></button>
      <div id="searchSplitter"></div>
      <div id="searchCloseIconDiv" class="clip16x20" tabindex="0">
        <img alt="search clear" id="searchCloseIcon" class="search-clear-x" src="https://i-msdn.sec.s-msft.com/Areas/Centers/Themes/StandardDevCenter/Content/HeaderFooterSprite.png?v=636475076592256654" />
      </div>
      <div id="searchTextContainer" style="width: 0;">
        <input id="HeaderSearchTextBox" name="query" type="text" aria-label="search edit textbox" maxlength="200" onfocus="Epx.Controls.SearchBox.watermarkFocus(event, this.title)" onblur="Epx.Controls.SearchBox.watermarkBlur(event, this.title)" />
      </div>
    </form>
    
    
  </div>
</div>
              </div>
            </div>
          </div>
        </div>

      </header>

    </div>

    
    
    
    <div id="jumpInfo" style="display: none" xmlns="http://www.w3.org/1999/xhtml">We’re sorry. The content you requested has been removed. You’ll be auto redirected in 1 second.</div>
<script type="text/javascript" xmlns="http://www.w3.org/1999/xhtml">
/*<![CDATA[*/
/**/
(window.MTPS || (window.MTPS = {})).cdnDomains || (window.MTPS.cdnDomains = { 
	"image": "https://i-msdn.sec.s-msft.com", 
	"js": "https://i2-msdn.sec.s-msft.com", 
	"css": "https://i-msdn.sec.s-msft.com", 
	"ttf": "https://i-msdn.sec.s-msft.com"
});
/**/
/*]]>*/
</script><script src="https://i1.code.msdn.s-msft.com/RequestReduceContent/226fb6e91933cf5ee0f2c8b70d6774bf-4575db404796601d81f7a927898924ef-RequestReducedScript.js" type="text/javascript" ></script><script type="text/javascript" src="https://i2-msdn.sec.s-msft.com/Areas/Epx/Themes/Base/Content/SearchBox.js" xmlns="http://www.w3.org/1999/xhtml"></script><script type="text/javascript" src="https://i1.services.social.microsoft.com/search/Widgets/SearchBox.jss?boxid=HeaderSearchTextBox&amp;btnid=HeaderSearchButton&amp;minimumTermLength=2&amp;pgArea=header&amp;brand=Msdn&amp;loc=en-US&amp;focusOnInit=false&amp;emptyWatermark=true&amp;searchButtonTooltip=Search MSDN" xmlns="http://www.w3.org/1999/xhtml"></script><script type="text/javascript" src="https://i2-msdn.sec.s-msft.com/Combined.js?resources=0:Layout,1:Header,0:JumpRedirect;/Areas/Epx/Themes/Base/Content:0,/Areas/Centers/Themes/StandardDevCenter/Content:1&amp;amp;hashKey=E67A5D22DC2F4AA2FB8F5A76E18E75E3&amp;amp;v=7EB011C712CD165608D5A0C58212077C" xmlns="http://www.w3.org/1999/xhtml"></script></div>
    <div id="BodyBackground" class="ltr">
        <div id="JelloSizer">    
            <div id="JelloExpander">
                <div id="JelloWrapper">
                    <div class="Clear"> </div>
                    
                   
                    
                    
                    <div class="Clear"> </div>
                          
                    <div class="topleftcorner"></div>           
                    <div class="toprightcorner"></div>                                
                    <div class="alley"> <!-- Left Side -->
                        <div class="wrapper"> <!-- Right Side -->
                            <div class="inner">  
                            
                                                 
                                <div class="Clear"></div>   


                                
                                
                                
                                
                                <div id="MainContent" class="Msdn">
                                    
    
    

    <script language="javascript" type="text/javascript">
        $(function () { Galleries.cookie.setTimezoneOffsetCookie(); })
    </script>

    <div id="container">
        
            <div id="GalleriesNavigation">
                
            </div>            
        
        
            <div id="subHeader">
                <div id="eyebrow" class="left">
                    











<div class="EyebrowContainer">
    
    

    
<div itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="EyebrowElement">
    <a href=https://msdn.microsoft.com/en-US/ itemprop="url">
        <span itemprop="title">Microsoft Developer Network</span>
    </a>
</div>
<text>></text>

    
    
    
    <div itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="EyebrowElement">
             <a href="/" itemprop="url">
                <span itemprop="title">Samples</span>
            </a> 
    </div>




    >
    <div itemscope itemtype="http://data-vocabulary.org/Breadcrumb" class="EyebrowElement">
            <span itemprop="title">The page does not exist</span>
    </div>
</div>
                </div>
            </div>
            <div class="clear"></div>
        
        
            <div id="siteMessage">
                






            </div>
            <div class="clear"></div>
        
        

<div class="subMenu">
    <script language="javascript" type="text/javascript">
        $(document).ready(function () {
            $('.internav > a').each(function (index, elem) {
                if ($(elem).attr('href').toUpperCase().indexOf('CODE') != -1) {
                    $(elem).addClass('active');
                }
                if ($(elem).attr('href').toUpperCase().indexOf('SMPGDB') != -1) {
                    $(elem).addClass('active');
                }

            });
        });
    </script>
</div>      
        <div class="clear"></div>

        <div id="bodyContainer">
            
    <h4>The page does not exist</h4>
    <p>Sorry, the page you requested was not found.</p>
    <p>If you would like to contact us regarding this, please click <a href="https://lab.msdn.microsoft.com/mailform/contactus.aspx?refurl=http%3a%2f%2fcode.msdn.microsoft.com%2f">here</a>.</p>
    <p>Return to the <a href="/">Samples Gallery home page</a>.</p>

        </div>
        <div class="clear">
        </div>
    </div>
    <div class="advertisment">
        
    </div>

                                    <div class="Clear"></div>
                                </div>
    
                            </div>
                        </div>
                    </div>
                
                    <div class="Clearbottom"></div>          
                    <div class="bottomleftcorner"></div>            
                    <div class="bottomrightcorner"></div> 
                    
                     
                    

                           
                </div>
            </div>
        </div>
    </div>
    

<div data-chameleon-template="epxfooter" ></div>
<div data-chameleon-template="footer" >

    
    
    
        
    <div id="ux-footer" class="" style="" dir="ltr" ms.pgarea="footer" xmlns="http://www.w3.org/1999/xhtml">
            
            <div id="footerSock" class="">
                <div id="footerSockInner">


                    <div class="footerSockCenter">
                        <div class="userVoice">
    <div class="title">
        Help us improve MSDN.
    </div>
        <div class="description">
            Visit our UserVoice Page to submit and vote on ideas!
        </div>
    <div class="buttons">
        <a class="button" id="userVoiceButton" href="https://aka.ms/msdnfeedback" target="_blank">Make a suggestion</a>
    </div>
</div>  
                    </div>
                    <div class="clear"></div>
                </div>
            </div>
            

        <footer class="top" role="navigation" aria-label="footer">
            <div data-fragmentName="LeftLinks" id="Fragment_LeftLinks" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="linkList">
    <h4 class="linkListTitle">Dev centers</h4>
    <ul class="links">
      <li>
        <a href="https://developer.microsoft.com/en-us/windows" id="LeftLinks_2148_1" class="windowsBlue" xmlns="http://www.w3.org/1999/xhtml">Windows</a>
      </li>
      <li>
        <a href="https://dev.office.com/" id="LeftLinks_2148_3" class="office" xmlns="http://www.w3.org/1999/xhtml">Office</a>
      </li>
      <li>
        <a href="https://www.visualstudio.com/" id="LeftLinks_2148_4" class="visualStudio" xmlns="http://www.w3.org/1999/xhtml">Visual Studio</a>
      </li>
      <li>
        <a href="https://docs.microsoft.com/en-us/azure/" target="_blank" id="LeftLinks_2148_12" xmlns="http://www.w3.org/1999/xhtml">Microsoft Azure</a>
      </li>
      <li>
        <a href="https://msdn.microsoft.com/developer-centers-msdn" id="LeftLinks_2148_5" xmlns="http://www.w3.org/1999/xhtml">More...</a>
      </li>
    </ul>
  </div>
</div>
            <div id="rightLinks">
                <div data-fragmentName="CenterLinks1" id="Fragment_CenterLinks1" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="linkList">
    <h4 class="linkListTitle">Learning resources</h4>
    <ul class="links">
      <li>
        <a href="https://mva.microsoft.com/" id="CenterLinks1_2151_4" xmlns="http://www.w3.org/1999/xhtml">Microsoft Virtual Academy</a>
      </li>
      <li>
        <a href="https://channel9.msdn.com/" id="CenterLinks1_2151_5" xmlns="http://www.w3.org/1999/xhtml">Channel 9</a>
      </li>
      <li>
        <a href="https://msdn.microsoft.com/magazine/" id="CenterLinks1_2151_7" xmlns="http://www.w3.org/1999/xhtml">MSDN Magazine</a>
      </li>
    </ul>
  </div>
</div>
                <div data-fragmentName="CenterLinks2" id="Fragment_CenterLinks2" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="linkList">
    <h4 class="linkListTitle">Community</h4>
    <ul class="links">
      <li>
        <a href="https://social.msdn.microsoft.com/forums/en-us/home" id="CenterLinks2_2151_8" xmlns="http://www.w3.org/1999/xhtml">Forums</a>
      </li>
      <li>
        <a href="https://blogs.msdn.microsoft.com/developer-tools/" id="CenterLinks2_2151_9" xmlns="http://www.w3.org/1999/xhtml">Blogs</a>
      </li>
      <li>
        <a href="https://www.codeplex.com/" id="CenterLinks2_2151_10" xmlns="http://www.w3.org/1999/xhtml">Codeplex</a>
      </li>
    </ul>
  </div>
</div>
                <div data-fragmentName="CenterLinks3" id="Fragment_CenterLinks3" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="linkList">
    <h4 class="linkListTitle">Support</h4>
    <ul class="links">
      <li>
        <a href="https://msdn.microsoft.com/hh361695" id="CenterLinks3_2151_11" xmlns="http://www.w3.org/1999/xhtml">Self support</a>
      </li>
    </ul>
  </div>
</div>
                <div data-fragmentName="CenterLinks4" id="Fragment_CenterLinks4" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="linkList">
    <h4 class="linkListTitle">Programs</h4>
    <ul class="links">
      <li>
        <a href="https://bizspark.microsoft.com/Startups/Index" id="CenterLinks4_2151_13" xmlns="http://www.w3.org/1999/xhtml">BizSpark (for startups)</a>
      </li>
      <li>
        <a href="https://imagine.microsoft.com/en-us" id="CenterLinks4_2151_22" xmlns="http://www.w3.org/1999/xhtml">Microsoft Imagine (for students)</a>
      </li>
    </ul>
  </div>
</div>
            </div>
        </footer>

        <footer class="bottom" role="contentinfo">
            <span class="localeContainer">
                
    <form class="selectLocale" id="selectLocaleForm" action="https://msdn.microsoft.com/en-US/selectlocale-dmc">
        <input type="hidden" name="fromPage" value="https%3a%2f%2fcode.msdn.microsoft.com%2fsite%2ferror%2f404%3f404%3bhttps%3a%2f%2fcode.msdn.microsoft.com%3a443%2fAreas%2fEpx%2fThemes%2fWindows%2fContent%2fUhfHeader.js" />
        <a href="#" onclick="$('#selectLocaleForm').submit();return false;" title="Change your language">United States (English)</a>
    </form>


            </span>

            <div data-fragmentName="BottomLinks" id="Fragment_BottomLinks" xmlns="http://www.w3.org/1999/xhtml">
  
  <div class="linkList">
    <ul class="links horizontal">
      <li>
        <a href="https://msdn.microsoft.com/en-us/flashnewsletter" id="BottomLinks_2148_7" xmlns="http://www.w3.org/1999/xhtml">Newsletter</a>
      </li>
      <li>
        <a href="https://msdn.microsoft.com/en-us/dn529288" id="BottomLinks_2148_8" xmlns="http://www.w3.org/1999/xhtml">Privacy &amp; cookies</a>
      </li>
      <li>
        <a href="https://msdn.microsoft.com/en-us/cc300389" id="BottomLinks_2148_9" xmlns="http://www.w3.org/1999/xhtml">Terms of use</a>
      </li>
      <li>
        <a href="https://www.microsoft.com/en-us/legal/intellectualproperty/Trademarks/" id="BottomLinks_2148_10" xmlns="http://www.w3.org/1999/xhtml">Trademarks</a>
      </li>
    </ul>
  </div>
</div>
            <span class="logoLegal">
                <span class="logoSpan clip67x13" role="img" tabindex="0" aria-label="microsoft logo">
                    <img alt="logo" class="logo" src="https://i-msdn.sec.s-msft.com/Areas/Centers/Themes/StandardDevCenter/Content/HeaderFooterSprite.png?v=636475076592256654" />
                </span>
                <span class="copyright">© 2017 Microsoft</span>
            </span>
        </footer>
    </div>
    
<script type="text/javascript" xmlns="http://www.w3.org/1999/xhtml">
/*<![CDATA[*/
/**/
(window.MTPS || (window.MTPS = {})).cdnDomains || (window.MTPS.cdnDomains = { 
	"image": "https://i-msdn.sec.s-msft.com", 
	"js": "https://i2-msdn.sec.s-msft.com", 
	"css": "https://i-msdn.sec.s-msft.com", 
	"ttf": "https://i-msdn.sec.s-msft.com"
});
/**/
/*]]>*/
</script><script type="text/javascript" src="https://i2-msdn.sec.s-msft.com/Combined.js?resources=0:NewFooterSock,1:Footer;/Areas/Epx/Themes/Base/Content:0,/Areas/Centers/Themes/StandardDevCenter/Content:1&amp;amp;hashKey=EB5B0152122C286919648AB36220C327&amp;amp;v=506913847CD0B223F9422D09B7ADAAE1" xmlns="http://www.w3.org/1999/xhtml"></script></div>  
    
    

    <script type="text/javascript">
        
    </script>

    <script type="text/javascript">
        Galleries.utility.loadJavaScript("https://widgets.membership.s-msft.com/v1/loader.js?brand=Msdn&lang=en-US", true);
        Galleries.utility.loadJavaScript('https://www.microsoft.com/library/svy/sto/broker.js', true);
    </script>

    
    <script src="https://i1.code.msdn.s-msft.com/RequestReduceContent/d61181630c512757a23098a427af92ef-c769c4ecaf33d04f3dea6af4337b9847-RequestReducedScript.js" type="text/javascript" ></script>        
    <script type="text/javascript">
        jQuery(window).load(
            function () {
                
                document.getElementsByTagName('body')[0].appendChild(addAsyncScript("https://i1.code.msdn.s-msft.com/GlobalResources/scripts/JSLLRecord.js?cver=0001"));
            });
        </script>
        <noscript><a href="https://www.omniture.com" title="Web Analytics">
        <img src="https://msstonojssocial.112.2O7.net/b/ss/msstonojssocial/1/H.20.2--NS/0" height="1" width="1" alt="" />
        </a></noscript>
    
    <script type="text/javascript" >
        $(document).ready(function (){
			$("img.DropDownArrow").attr("src","https://i1.code.msdn.s-msft.com/GlobalResources/images/Msdn/DropDownArrow_Gray.png");
		});
    </script>
</body>
</html>
