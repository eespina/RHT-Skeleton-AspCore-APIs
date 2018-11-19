/* jshint latedef: nofunc */

function isMobile() {
	var mobileW = 767;
	if (window.matchMedia("(min-width: " + mobileW + "px)").matches) {
		return false;
	} else {
		return true;
	}
}

function isTablet() {
	var tabletW = 992;
	if (window.matchMedia("(min-width: " + tabletW + "px)").matches) {
		return false;
	} else {
		return true;
	}
}

function isDesktop() {
	return !isMobile() && !isTablet();
}

var addEvent = function (object, type, callback) {
	if (object == null || typeof (object) == 'undefined') { return; }
	if (object.addEventListener) {
		object.addEventListener(type, callback, false);
	} else if (object.attachEvent) {
		object.attachEvent("on" + type, callback);
	} else {
		object["on" + type] = callback;
	}
};
