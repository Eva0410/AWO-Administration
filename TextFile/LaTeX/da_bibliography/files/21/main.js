function goIssue() {
	var issue = $("#allIssues").val();
	if (null !== issue) {
		window.location = issue;
	}
}
function getLastNameStart(name) {
  var result = 0;

  for(i=1;i<name.length;i++) {
console.log(name.charCodeAt(i));
    if(name.charCodeAt(i) < 97) {
      result = i;
      break;
    }
}
  return result;
}

function insertSpaceName(name) {
  var result = "";
  var pos = getLastNameStart(name);

  if(pos > 0 ){
    result = name.slice(0,pos) + " " + name.slice(pos);
  }
  return result;
}


function fixUnevenElementHeights($elements) {
	var heights = new Array();
	$elements.each(function() {
		$(this).css('height', 'auto');
		heights.push($(this).outerHeight());
	});
	var maxHeight = Math.max.apply(Math, heights);
	$elements.each(function() {
		$(this).css("height", maxHeight + "px");
	})
}

function balanceColumns($elements) {
	var heights = new Array();
	$elements.each(function() {
		$(this).css('height', 'auto');
		heights.push($(this).outerHeight());
	});
    /*if $(window).width() < 992, we should only have one column*/
    if($(window).width() > 992) {
	/*for each pair, set max height*/
	$elements.each(function(index) {
		if (index % 2 == 0) {
			if (heights[index] > heights[index + 1]) {
				heights[index + 1] = heights[index];
			} else {
				heights[index] = heights[index + 1];
			}
		}
	});
	$elements.each(function(index) {
		$(this).css("height", heights[index] + "px");
	});
    }
}

function buildItem(data) {
	var result = "<div class='feedItem col-xs-12 col-md-6'>";
	result += "<h2 class='fiTitle'>" + data.Title + "</h2>";
	var dates = data.PublishDate.split(" ");
	var pubDate = dates[2] + " " + dates[3];
	result += "<div class='fiDate'>" + pubDate + "</div>";
	result += "<div class='fiDesc'>" + data.Content + "</div>";
	result += "<div><a class='button' href='" + data.Link + "'>Read article</a>";
	result += "</div>";
	return result;
}
function decode(text) {
    var result = decodeURIComponent(text);
    result = result.replace("+", " ");
    return result;
}
function sxpRender(data) {
	/*need to figure out which query this is for*/
    var query = urlParam("author");
    if("" == query) {
        query = urlParam("topic");
    }
    if("" == query) {
        query = urlParam("column");
    }
	$(".pgTitle").html(decode(query));
	$("#feedContent").html("");

	$.each(data.Items, function(i, item) {

		$("#feedContent").append(buildItem(item));
	});
	balanceColumns($(".feedItem"));
}

function urlParam(variable) {
	var query = window.location.search.substring(1);
	var vars = query.split("\u0026");
	for (var i = 0; i < vars.length; i++) {
		var pair = vars[i].split("=");
		if (pair[0] == variable) {
			return pair[1];
		}
	}
	return (false);
}
$(window).load(function() {
	if ($("#LeftContent .nosidebar").length > 0) {
		$("div#content").css({
			"width": "100%",
			"max-width": "100%"
		});
		$("#RightContent").hide();
		$("#LeftContent").css({
			"width": "100%"
		});
	}
	if ($(".articleBlock").length > 0) {
		balanceColumns($(".articleBlock"));
	}
	if ($(".feedItem").length > 0) {
		balanceColumns($(".articleBlock"));
	}
	/*$("#LeftContent .nosidebar").parents("#LeftContent").siblings("#RightContent").hide();*/
});
$(window).resize(function() {
	if ($(".articleBlock").length > 0) {
		balanceColumns($(".articleBlock"));
	}
	if ($(".feedItem").length > 0) {
		balanceColumns($(".articleBlock"));
	}
})