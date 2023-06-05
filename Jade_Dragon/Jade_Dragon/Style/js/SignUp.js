$(window).on("hashchange", function () {
	if (location.hash.slice(1) == "XnGmail") {
		$(".card").addClass("extend");
		$("#User").removeClass("selected");
		$("#XnGmail").addClass("selected");
	} else {
		$(".card").removeClass("extend");
		$("#User").addClass("selected");
		$("#XnGmail").removeClass("selected");
	}
});
$(window).trigger("hashchange");
