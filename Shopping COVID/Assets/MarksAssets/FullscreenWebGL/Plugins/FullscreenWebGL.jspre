Module["FullscreenWebGL"] = Module["FullscreenWebGL"] || {};

Module["FullscreenWebGL"].fullscreenchangeCallback = undefined;
Module["FullscreenWebGL"].fullscreenchange = function() {
	Module.dynCall_v(Module["FullscreenWebGL"].fullscreenchangeCallback);
};