$(document).ready(function(){	
	$(window).scroll(function () {
		if ($(this).scrollTop() > 20) {
		    $('#back-top').stop(true).css({ "display": "block" }).animate({ "opacity": 1 }, 400);
		} else {
			$('#back-top').stop(true).animate({"opacity": 0}, 400, function(){$(this).css({"display":"none"})});
		}
	});
	$('#back-top').click(function () {
		$('body, html').stop(true).animate({scrollTop: 0}, 600);
		return false;
	});

	$('#back-to-bottom').on("click", function () {
	    var percentageToScroll = 100;
	    var height = $(document).innerHeight();
	    var scrollAmount = height * percentageToScroll / 100;
	    var overheight = jQuery(document).height() - jQuery(window).height();
	    jQuery("html, body").animate({ scrollTop: scrollAmount }, 900);
	});
})