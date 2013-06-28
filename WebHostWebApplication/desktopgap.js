
var __desktopgap = null;

function InitializeDesktopGap() {
  if(__desktopgap == null){
    __desktopgap = new DesktopGap();
  }
}

function DesktopGap_AssignID(guid) {
  window.documentHandle = guid;
}


function DesktopGap_RetrieveID() {
  return window.documentHandle;
}

function DesktopGap() {
  this._registered_events = {};
  
  try{
    this._identifier = window.external.GetService(window.documentHandle, "GuidService");
    this._windowCreator = window.external.GetService(window.documentHandle, "WindowManagerService");
  
  } catch(err) {
    this._identifier = null;
    this._windowCreator = null;
  }
  
  this._servicesAvailable = this._identifier != null && this._windowCreator != null;
}

DesktopGap.prototype.isAvailable = function() {
	return this._servicesAvailable && DesktopGap_RetrieveID() !== undefined && DesktopGap_RetrieveID() != null;
};

DesktopGap.prototype.register = function (guid, e, f) {
  this._registered_events[guid] = { "func": f, "element": e };

};

DesktopGap.prototype.assertAvailable = function() {
  if(!this.isAvailable())
  	throw "DesktopGap unavailable";
};

DesktopGap.prototype.onDragOver = function (element, dragover) {
  this.assertAvailable();
  
  var guid = this._identifier.CreateGuid();
  
  this.register(guid, element, dragover);
  window.external.addEventListener(DesktopGap_RetrieveID(), "DragOver", "DesktopGap_EventDispatch", "DragAndDrop",
      { DocumentHandle: DesktopGap_RetrieveID(), EventID: guid, Criteria: { elementID: element.id } });

};

DesktopGap.prototype.onDragEnter = function (element, dragenter) {
  this.assertAvailable();
  
  var guid = this._identifier.CreateGuid();
  
  this.register(guid, element, dragenter);
  window.external.addEventListener(DesktopGap_RetrieveID(), "DragEnter", "DesktopGap_EventDispatch", "DragAndDrop",
      { DocumentHandle: DesktopGap_RetrieveID(), EventID: guid, Criteria: { elementID: element.id } });

};

DesktopGap.prototype.onDragLeave = function (element, dragleave) {
  this.assertAvailable();
  
  var guid = this._identifier.CreateGuid();
  
  this.register(guid, element, dragleave);
  window.external.addEventListener(DesktopGap_RetrieveID(), "DragLeave", "DesktopGap_EventDispatch", "DragAndDrop",
      { DocumentHandle: DesktopGap_RetrieveID(), EventID: guid, Criteria: { elementID: element.id } });
};

DesktopGap.prototype.onDrop = function (element, dragdrop) {
  this.assertAvailable();
  
  var guid = this._identifier.CreateGuid();
  
 this.register(guid, element, dragdrop);
  window.external.addEventListener(DesktopGap_RetrieveID(), "DragDrop", "DesktopGap_EventDispatch", "DragAndDrop",
      { DocumentHandle: DesktopGap_RetrieveID(), EventID: guid, Criteria: { elementID: element.id } });

};

DesktopGap.prototype.getRegisteredEvent = function (eventID) {
  return this._registered_events[eventID];
};

DesktopGap.prototype.prepareModalPopUp = function(url) {
  if(this.isAvailable())
  	return this._windowCreator.PrepareNewWindow(url, "popup", "modal");
  return null;
};

DesktopGap.prototype.preparePopUp = function(url) {
  if(this.isAvailable())
    return this._windowCreator.PrepareNewWindow(url, "popup", "active");
  return null;
};

DesktopGap.prototype.prepareTab = function(url) {
  if(this.isAvailable())
    return this._windowCreator.PrepareNewWindow(url, "tab", "active");
  return null;
};

DesktopGap.prototype.prepareBackgroundTab = function(url) {
  if(this.isAvailable())
    return this._windowCreator.PrepareNewWindow(url, "tab", "background");
  return null;
};



function DesktopGap_EventDispatch(args) {
  if(__desktopgap != null){
    var arguments = JSON.parse(args);

    var event = __desktopgap.getRegisteredEvent(arguments.EventID);
    var func = event.func;
    var element = event.element;
    func.call(element, arguments);
  }
}

function OnDragOver(element, dragOver) {
  if(__desktopgap == null) 
    InitializeDesktopGap();
  if(__desktopgap.isAvailable()) {
    __desktopgap.onDragOver(element, dragOver);
  } else {
	element.addEventListener("dragover", dragOver);
  }  
}

function OnDragEnter(element, dragEnter) {
  if(__desktopgap == null) 
    InitializeDesktopGap();
  if(__desktopgap.isAvailable()) {
  	__desktopgap.onDragEnter(element, dragEnter);  
  } else {
  	element.addEventListener("dragenter", dragEnter);
  }  
}

function OnDragLeave(element, dragLeave) {
  if(__desktopgap == null) 
    InitializeDesktopGap();
  if(__desktopgap.isAvailable()) {
    __desktopgap.onDragLeave(element, dragLeave);
  } else {
	element.addEventListener("dragexit", dragLeave);
  }  
}

function OnDrop(element, drop) {
  if(__desktopgap == null) 
    InitializeDesktopGap();
  if(__desktopgap.isAvailable()) {
    __desktopgap.onDrop(element, drop);
  } else {
	element.addEventListener("drop", drop);
  }
}