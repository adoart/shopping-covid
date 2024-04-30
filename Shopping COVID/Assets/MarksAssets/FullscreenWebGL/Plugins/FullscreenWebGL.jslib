mergeInto(LibraryManager.library, {
	requestFullscreen_FullscreenWebGL: function(callback, option) {
		option = UTF8ToString(option);
		document.documentElement.addEventListener('pointerup', function() {
			const canvas = document.getElementsByTagName("canvas")[0];
            const requestFullscreen = canvas.requestFullscreen.bind(canvas) || canvas.mozRequestFullScreen.bind(canvas) || canvas.webkitRequestFullScreen.bind(canvas) || canvas.msRequestFullscreen.bind(canvas);
			requestFullscreen({ navigationUI: option }).then(function() {
				callback !== 0 && Module.dynCall_vi(callback, 0);//success
			}).catch(function(error) {
				callback !== 0 && Module.dynCall_vi(callback, 1);
				console.error(error);
			});
		}, { once: true });
	},
	exitFullscreen_FullscreenWebGL: function(callback) {
		document.documentElement.addEventListener('pointerup', function() {
			const exitFullscreen = document.exitFullscreen.bind(document) || document.mozExitFullscreen.bind(document) || document.webkitExitFullscreen.bind(document) ||document.msExitFullscreen.bind(document);
			exitFullscreen().then(function() {
				callback !== 0 && Module.dynCall_vi(callback, 0);//success
			}).catch(function(error) {
				callback !== 0 && Module.dynCall_vi(callback, 1);
				console.error(error);
			});
		}, { once: true });
	},
	isFullscreen_FullscreenWebGL: function() {
		document.fullscreenElement = document.fullscreenElement || document.mozFullscreenElement || document.webkitFullscreenDocument || document.msFullscreenElement;
		if (document.fullscreenElement) return true;
		else return false;
	},
	isFullscreenSupported_FullscreenWebGL: function() {
		return document.fullscreenEnabled;
	},
	subscribeToFullscreenchangedEvent_FullscreenWebGL: function(callback) {
		Module["FullscreenWebGL"].fullscreenchangeCallback = callback;
		document.addEventListener('fullscreenchange', Module["FullscreenWebGL"].fullscreenchange);
	},
	unsubscribeToFullscreenchangedEvent_FullscreenWebGL: function() {
		document.removeEventListener('fullscreenchange', Module["FullscreenWebGL"].fullscreenchange);
	},
});